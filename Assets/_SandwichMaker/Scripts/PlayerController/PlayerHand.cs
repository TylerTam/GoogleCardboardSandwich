using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class PlayerHandEvent : UnityEngine.Events.UnityEvent { }
public class PlayerHand : MonoBehaviour
{
    private IHoldable m_holdableObject;
    private Transform m_heldObjectTrasform;


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

    public IHoldable CurrentHeldObject()
    {
        return m_holdableObject;
    }

    public void PerformInteraction()
    {
        RaycastHit hit;

        if (m_holdableObject == null)
        {
            if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, m_interactableDistance, m_interactionLayer))
            {
                ///If the interaction hit cannot be performed, invoke the failed event
                if (!hit.transform.GetComponent<IInteractable>().InteractionValid())
                {
                    m_playerEvents.m_interactionFailed.Invoke();
                }
            }
        }
        else 
        {
            if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, m_interactableDistance, m_holdableObject.ReturnUsableLayerMask()))
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
            
    }

    public void PickUpObject(GameObject p_currentFood)
    {
        m_holdableObject = p_currentFood.GetComponent<IHoldable>();
        m_heldObjectTrasform = p_currentFood.transform;
        p_currentFood.transform.position = m_heldFoodPosition.position;
        p_currentFood.transform.parent = m_heldFoodPosition;
        p_currentFood.transform.localRotation = Quaternion.identity;
    }

    public void EmptyHand(bool p_objectUsed)
    {
        m_heldObjectTrasform.parent = null;
        if (p_objectUsed)
        {
            m_holdableObject.UseObject();
        }
        else
        {
            m_holdableObject.DropObject();
        }

        m_holdableObject = null;
        m_heldObjectTrasform = null;
    }

    public void ThrowObject()
    {
        if (m_heldObjectTrasform == null) return;
        RaycastHit hit;
        if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, m_aimAssistDistance, m_aimAssistMask))
        {
            m_heldFoodPosition.LookAt(hit.point);
        }
        m_holdableObject.ThrowObject(m_throwForce, m_heldObjectTrasform.forward);
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
