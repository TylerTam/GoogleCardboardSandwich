using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodObject : MonoBehaviour, IInteractable, IHoldable
{
    public int m_heldFoodIndex;
    PlayerHand m_playerHand;

    private Rigidbody m_rb;
    public float m_degradeTime;
    public AnimationCurve m_degradeAnimCurve;
    private bool m_initialized;
    private Vector3 m_originalPosition;
    private Quaternion m_originalRotation;


    public LayerMask m_usableLayer;
    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
    }
    private void InitializeMe()
    {
        m_initialized = true;
        m_rb = GetComponent<Rigidbody>();
        m_originalPosition = transform.position;
        m_originalRotation = transform.rotation;
    }

    private void OnEnable()
    {
        if (!m_initialized)
        {
            InitializeMe();
        }
        ResetMe();
    }
    /// <summary>
    /// Resets the scale and rigidbody physics for this object
    /// </summary>
    public void ResetMe()
    {
        m_rb.isKinematic = true;
        transform.localScale = Vector3.one;
        transform.position = m_originalPosition;
        transform.rotation = m_originalRotation;
    }


    /// <summary>
    /// The coroutine that shrinks the food, and recycles it into the object pooler
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisposeFood()
    {
        float timer = 0;
        while (timer < m_degradeTime)
        {
            yield return null;
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, m_degradeAnimCurve.Evaluate(timer / m_degradeTime));
        }
        UseObject();
    }


    #region IInteractable
    /// <summary>
    /// Used by the PlayerHand script to interact with this object
    /// </summary>
    /// <returns></returns>
    public bool InteractionValid()
    {
        if (m_playerHand.CurrentHeldObject() != null)
        {
            m_playerHand.EmptyHand(false);
        }
        StopAllCoroutines();
        ResetMe();
        m_playerHand.PickUpObject(gameObject);
        return true;
    }






    #endregion

    #region IUsable
    public LayerMask ReturnUsableLayerMask()
    {
        return m_usableLayer;
    }

    public void UseObject()
    {
        ResetMe();
    }

    public void DropObject()
    {
        m_rb.isKinematic = false;
        StartCoroutine(DisposeFood());
    }


    public GameObject ReturnCurrentObject()
    {
        return gameObject;
    }

    public void ThrowObject(float p_speed, Vector3 p_dir)
    {
        m_rb.isKinematic = true;
        m_rb.velocity = p_dir * p_speed;
    }
    #endregion
}
