using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodObject : MonoBehaviour, IInteractable
{
    public int m_heldFoodIndex;
    PlayerHand m_playerHand;

    private Rigidbody m_rb;
    public float m_degradeTime;
    public AnimationCurve m_degradeAnimCurve;
    private ObjectPooler m_pooler;
    private bool m_initialized;
    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
        m_pooler = ObjectPooler.instance;
    }
    private void InitializeMe()
    {
        m_initialized = true;
        m_rb = GetComponent<Rigidbody>();
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
    }

    /// <summary>
    /// Called to set the food's rigidbody to not kinematic
    /// </summary>
    public void DropFoodItem()
    {
        m_rb.isKinematic = false;
        StartCoroutine(DisposeFood());
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
        m_pooler.ReturnToPool(this.gameObject);
    }

    #region IInteractable
    /// <summary>
    /// Used by the PlayerHand script to interact with this object
    /// </summary>
    /// <returns></returns>
    public bool InteractionValid()
    {
        if (m_playerHand.CurrentHeldFood() != null)
        {
            m_playerHand.EmptyHand(false);
        }
        StopAllCoroutines();
        ResetMe();
        m_playerHand.PickUpFood(gameObject);
        return true;
    }


    #endregion

}
