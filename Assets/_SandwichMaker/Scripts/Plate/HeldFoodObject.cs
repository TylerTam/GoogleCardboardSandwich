using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The holdable food objects
/// </summary>
public class HeldFoodObject : MonoBehaviour, IInteractable, IHoldable
{
    public int m_heldFoodIndex;
    PlayerHand m_playerHand;

    private Rigidbody m_rb;
    public float m_degradeTime;
    public AnimationCurve m_degradeAnimCurve;
    private bool m_initialized;
    private Vector3 m_originalPosition;
    private Vector3 m_originalScale;
    private Quaternion m_originalRotation;
    private Transform m_originalParent;

    public BoxCollider m_collider;

    public LayerMask m_usableLayer;
    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
        GetOriginalMaterials();
    }
    private void InitializeMe()
    {
        m_initialized = true;
        m_rb = GetComponent<Rigidbody>();
        m_originalPosition = transform.localPosition;
        m_originalRotation = transform.localRotation;
        m_originalScale = transform.localScale;
        m_originalParent = transform.parent;
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
        transform.parent = m_originalParent;
        transform.localScale = m_originalScale;
        transform.localPosition = m_originalPosition;
        transform.localRotation = m_originalRotation;
        
    }

    public void SetColliderState(bool p_newState)
    {
        m_collider.enabled = p_newState;
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


    #region IInteractable Highlight
    [Header("Highlight")]
    public List<MeshRenderer> m_renderers;
    private List<Material> m_originalMaterials = new List<Material>();
    public Material m_highlightMaterial;
    private void GetOriginalMaterials()
    {
        foreach (MeshRenderer rend in m_renderers)
        {
            m_originalMaterials.Add(rend.material);
        }
    }
    public void OnHoverLeft()
    {
        foreach (MeshRenderer rend in m_renderers)
        {
            rend.material = m_originalMaterials[m_renderers.IndexOf(rend)];
        }
    }

    public void OnHoverOver()
    {
        foreach (MeshRenderer rend in m_renderers)
        {
            rend.material = m_highlightMaterial;
        }
    }
    #endregion



    #endregion

    #region IUsable
    public LayerMask ReturnUsableLayerMask()
    {
        return m_usableLayer;
    }

    public virtual void UseObject()
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
