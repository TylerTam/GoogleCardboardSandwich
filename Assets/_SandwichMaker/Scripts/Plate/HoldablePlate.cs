using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldablePlate : MonoBehaviour, IHoldable
{
    private List<Transform> m_sandwhichObjects = new List<Transform>();
    public Rigidbody m_plateRigidbody;
    public float m_lifespan;
    public AnimationCurve m_lifespanAnimCurve;
    private SandwhichType m_currentSandwhichType = new SandwhichType();

    public LayerMask m_interactingLayer;
    private ObjectPooler m_pooler;
    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }

    /// <summary>
    /// Adds all the objects from the sandwich to this plate. Also sets the booleans for the ingredients that are in the sandwich
    /// </summary>
    /// <param name="p_newSandwhich"></param>
    /// <param name="p_currentSandwhichType"></param>
    /// <returns></returns>
    public GameObject CreatePlate(List<Transform> p_newSandwhich, SandwhichType p_currentSandwhichType)
    {


        m_currentSandwhichType.SetSandwhichType(p_currentSandwhichType);
        foreach (Transform sandwich in p_newSandwhich)
        {
            m_sandwhichObjects.Add(sandwich);
            sandwich.transform.parent = transform;
        }

        return gameObject;
    }

    public void DropObject()
    {
        StartCoroutine(StartDegrading());
    }

    public GameObject ReturnCurrentObject()
    {
        return gameObject;
    }

    public LayerMask ReturnUsableLayerMask()
    {
        return m_interactingLayer;
    }

    public void ThrowObject(float p_speed, Vector3 p_dir)
    {
        m_plateRigidbody.isKinematic = false;
        m_plateRigidbody.velocity = p_speed * p_dir;

        foreach (Transform newItem in m_sandwhichObjects)
        {
            Rigidbody rb = newItem.GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.velocity = p_dir * p_speed;
        }

        

    }

    /// <summary>
    /// Start shrinking the object after it is dropped. When their scale reaches zero, return them to the pool
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartDegrading()
    {
        float timer = 0;
        List<Vector3> originalScales = new List<Vector3>();

        foreach (Transform newItem in m_sandwhichObjects)
        {
            originalScales.Add(newItem.transform.localScale);
        }

        while (timer < m_lifespan)
        {
            timer += Time.deltaTime;
            yield return null;

            float percent = timer / m_lifespan;
            m_plateRigidbody.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, m_lifespanAnimCurve.Evaluate(percent));

            foreach (Transform newItem in m_sandwhichObjects)
            {
                newItem.localScale = Vector3.Lerp(originalScales[m_sandwhichObjects.IndexOf(newItem)], Vector3.zero, m_lifespanAnimCurve.Evaluate(percent));
            }
        }

        m_plateRigidbody.isKinematic = true;
        m_plateRigidbody.transform.localPosition = Vector3.zero;
        m_plateRigidbody.transform.localRotation = Quaternion.identity;
        m_plateRigidbody.transform.localScale = Vector3.one;

        foreach (Transform newItem in m_sandwhichObjects)
        {
            
            newItem.localScale = originalScales[m_sandwhichObjects.IndexOf(newItem)];
            newItem.GetComponent<Rigidbody>().isKinematic = true;
            newItem.transform.parent = null;
            m_pooler.ReturnToPool(newItem.gameObject);
        }

        m_sandwhichObjects.Clear();
        m_pooler.ReturnToPool(gameObject);

    }

    public void UseObject()
    {
        foreach (Transform newItem in m_sandwhichObjects)
        {
            newItem.transform.parent = null;
            m_pooler.ReturnToPool(newItem.gameObject);
        }

        m_sandwhichObjects.Clear();
        m_pooler.ReturnToPool(gameObject);
    }
}

