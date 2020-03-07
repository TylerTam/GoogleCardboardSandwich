using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTool : MonoBehaviour, IInteractable
{
    public enum ToolState
    {
        CLOSED, OPEN
    }
    public ToolState m_currentToolState;

    public float m_toolAnimTime;
    public AnimationCurve m_animCurve;
    private float m_percentToClosed;
    public bool InteractionValid()
    {
        switch (m_currentToolState)
        {
            case ToolState.CLOSED:
                m_currentToolState = ToolState.OPEN;
                break;
            case ToolState.OPEN:
                m_currentToolState = ToolState.CLOSED;
                break;
        }
        StopAllCoroutines();
        StartCoroutine(SwitchToolState());
        return true;
    }

    private IEnumerator SwitchToolState()
    {
        float timer = m_percentToClosed * m_toolAnimTime;

        while (!CanStopCoroutine(timer))
        {
            timer += Time.deltaTime * ((m_currentToolState == ToolState.OPEN) ? 1 : -1);

            m_percentToClosed = timer / m_toolAnimTime;
            ChangeToolTransform(m_percentToClosed);

            yield return null;
        }

    }

    public virtual void ChangeToolTransform(float p_percent)
    {
    }

    private bool CanStopCoroutine(float p_currentTimer)
    {
        switch (m_currentToolState)
        {
            case ToolState.CLOSED:
                if (p_currentTimer <= 0)
                {
                    return true;
                }
                break;
            case ToolState.OPEN:
                if (p_currentTimer >= m_toolAnimTime)
                {
                    return true;
                }
                break;
        }
        return false;
    }
}
