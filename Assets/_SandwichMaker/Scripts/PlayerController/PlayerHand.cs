﻿using System.Collections;
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

    public LayerMask m_sandwhichHandinMask;
    private bool m_holdingFinalSandwich;

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

    /// <summary>
    /// Raycasts to check if they can interact with something
    /// </summary>
    public void PerformInteraction()
    {
        RaycastHit hit;

        ///If there is currently no held object, perform a raycast to search for interactables
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

        ///If they are holding an object, call that object to get a layermask from it, which is used to determine if what the player is point at can be interacted with
        else
        {

            if (Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out hit, m_interactableDistance,(m_holdingFinalSandwich)? m_sandwhichHandinMask :  m_holdableObject.ReturnUsableLayerMask()))
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

    /// <summary>
    /// Empties the player's hand, dropping whatever they are currently holding
    /// </summary>
    /// <param name="p_objectUsed"></param>
    public void EmptyHand(bool p_objectUsed, bool p_discardItem = true)
    {
        SetHoldingSandwhichState(false);
        m_heldObjectTrasform.parent = null;
        if (p_discardItem)
        {
            if (p_objectUsed)
            {
                m_holdableObject.UseObject();
            }
            else
            {
                m_holdableObject.DropObject();
            }
        }

        m_holdableObject = null;
        m_heldObjectTrasform = null;
    }

    /// <summary>
    /// Throws whatever object they are currently holding
    /// </summary>
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

    public void SetHoldingSandwhichState(bool p_active)
    {
        m_holdingFinalSandwich = p_active;
    }

    private void OnDrawGizmos()
    {
        if (!m_isDebugging) return;
        Gizmos.color = m_gizmosColor1;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * m_interactableDistance);
        Gizmos.DrawWireSphere(transform.position + transform.forward * m_interactableDistance, .25f);
    }
}
