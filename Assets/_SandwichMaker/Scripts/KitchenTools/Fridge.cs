using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : MonoBehaviour, IInteractable
{
    public enum FridgeState
    {
        CLOSED, OPEN
    }
    public FridgeState m_currentFridgeState;
    public Transform m_fridgeDoor;
    public float m_openRotationY;
    public float m_closedRotationY;
    public float m_rotationTime;
    public AnimationCurve m_animCurve;

    
    private float m_percentToClosed;
    public bool InteractionValid()
    {
        switch (m_currentFridgeState)
        {
            case FridgeState.CLOSED:
                m_currentFridgeState = FridgeState.OPEN;
                break;
            case FridgeState.OPEN:
                m_currentFridgeState = FridgeState.CLOSED;
                break;
        }
        StopAllCoroutines();
        StartCoroutine(SwitchDoorState());
        return true;
    }

    private IEnumerator SwitchDoorState()
    {
        float timer = m_percentToClosed * m_rotationTime;

        while (!CanStopCoroutine(timer))
        {
            timer += Time.deltaTime * ((m_currentFridgeState == FridgeState.OPEN)? 1 : -1);

            m_percentToClosed = timer / m_rotationTime;

            m_fridgeDoor.localEulerAngles = new Vector3(0, Mathf.Lerp(m_closedRotationY, m_openRotationY, m_animCurve.Evaluate(m_percentToClosed)), 0);
            yield return null;
        }
        
    }

    private bool CanStopCoroutine(float p_currentTimer)
    {
        switch (m_currentFridgeState)
        {
            case FridgeState.CLOSED:
                if(p_currentTimer <= 0)
                {
                    return true;
                }
                break;
            case FridgeState.OPEN:
                if (p_currentTimer >= m_rotationTime)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
