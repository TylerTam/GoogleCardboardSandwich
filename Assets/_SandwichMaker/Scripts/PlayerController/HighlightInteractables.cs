using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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



    private void StillInRange()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_fireSpot.position, m_fireSpot.forward,out hit,m_raycastLength,  m_interactableLayer))
        {
            if(hit.transform.gameObject != m_currentObject)
            {
                Debug.Log("Current: " + m_currentObject.name + " | New: " + hit.transform.gameObject.name);
                m_currentObject.GetComponent<IInteractable>().OnHoverLeft();
                m_currentObject = hit.transform.gameObject;
                m_currentObject.GetComponent<IInteractable>().OnHoverOver();
                return;
            }
            return;
        }
        m_currentObject.GetComponent<IInteractable>().OnHoverLeft();
        m_currentObject = null;
    }

    private void CheckForObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(m_fireSpot.position, m_fireSpot.forward, out hit, m_raycastLength, m_interactableLayer))
        {
            m_currentObject = hit.transform.gameObject;
            m_currentObject.GetComponent<IInteractable>().OnHoverOver();
            return;
        }
    }
}
