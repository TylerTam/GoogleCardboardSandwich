using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerHandEvent : UnityEngine.Events.UnityEvent { }
public class PlayerHand : MonoBehaviour
{
    private HeldFoodObject m_heldFoodObject;


    public static PlayerHand Instance;
    private ObjectPooler m_pooler;

    

    

    #region Pickup Variables
    public Camera m_mainCamera;
    public LayerMask m_interactionLayer;
    public Transform m_heldFoodPosition;
    public float m_interactableDistance;

    #endregion

    #region AimAssist
    public float m_aimAssistDistance;
    public LayerMask m_aimAssistMask;
    public float m_throwForce;
    #endregion

    public PlayerEvents m_playerEvents;
    [System.Serializable]
    public struct PlayerEvents
    {
        public PlayerHandEvent m_interactionFailed;
    }

    [Header("Debugging")]
    public bool m_isDebugging;
    public Color m_gizmosColor1;
    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        m_pooler = ObjectPooler.instance;
    }

    public HeldFoodObject CurrentHeldFood()
    {
        return m_heldFoodObject;
    }

    public void PerformInteraction()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, m_interactableDistance, m_interactionLayer))
        {
            ///If the interaction hit cannot be performed, invoke the failed event
            if (!hit.transform.GetComponent<IInteractable>().InteractionValid())
            {
                m_playerEvents.m_interactionFailed.Invoke();
            }
        }
        else
        {
            ThrowObject();
        }
    }

    public void PickUpFood(GameObject p_currentFood)
    {
        m_heldFoodObject = p_currentFood.GetComponent<HeldFoodObject>();
        p_currentFood.transform.position = m_heldFoodPosition.position;
        p_currentFood.transform.parent = m_heldFoodPosition;
        p_currentFood.transform.localRotation = Quaternion.identity;
    }

    public void EmptyHand(bool p_returnToPool)
    {
        m_heldFoodObject.transform.parent = null;
        if (p_returnToPool)
        {
            m_pooler.ReturnToPool(m_heldFoodObject.gameObject);
        }
        else
        {
            m_heldFoodObject.DropFoodItem();
        }
        
        m_heldFoodObject = null;
    }

    public void ThrowObject()
    {
        if (m_heldFoodObject == null) return;
        RaycastHit hit;
        if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, m_aimAssistDistance, m_aimAssistMask))
        {
            m_heldFoodPosition.LookAt(hit.point);
        }
        Rigidbody rb = m_heldFoodObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = m_heldFoodPosition.forward * m_throwForce;
        m_heldFoodPosition.localRotation = Quaternion.identity;
        EmptyHand(false);
    }

    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_interactableDistance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_interactableDistance, .25f);
    }
}
