using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the material on the object that the player is pointing at
/// </summary>
public class HighlightInteractables : MonoBehaviour
{
    public LayerMask m_interactableLayer;
    public float m_raycastLength;
    
    private GameObject m_currentObject;

    public Transform m_fireSpot;


    
    private void Update()
    {
        CheckRaycast();
    }

    private void CheckRaycast()
    {
        if (m_currentObject != null)
        {
            StillInRange();
        }
        else
        {
            CheckForObject();
        }
    }


    /// <summary>
    /// Checks if the object is still in range, if it isnt reset it's material
    /// </summary>
    private void StillInRange()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_fireSpot.position, m_fireSpot.forward,out hit,m_raycastLength,  m_interactableLayer))
        {
            if(hit.transform.gameObject != m_currentObject)
            {
                m_currentObject.GetComponent<IInteractable>().OnHoverLeft();
                m_currentObject = hit.transform.gameObject;
                m_currentObject.GetComponent<IInteractable>().OnHoverOver();
                return;
            }
            return;
        }
        IInteractable currentHeld = m_currentObject.GetComponent<IInteractable>();
        if (currentHeld != null)
        {
            currentHeld.OnHoverLeft();
        }
        
        m_currentObject = null;
    }

    /// <summary>
    /// Checks if an object is in range, if it is change its material
    /// </summary>
    private void CheckForObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_fireSpot.position, m_fireSpot.forward, out hit, m_raycastLength, m_interactableLayer))
        {
            
            IInteractable newHeld = hit.transform.gameObject.GetComponent<IInteractable>();
            if (newHeld != null)
            {
                m_currentObject = hit.transform.gameObject;
                newHeld.OnHoverOver();

            }
            
            return;
        }
    }
}
