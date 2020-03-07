using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class used to interact with kitchen tools that can be opened or closed.
/// </summary>
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

    /// <summary>
    /// Performs the tool's animation.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// Has functionality in the inheritor classes. Generally controls the animation for the opening and closing.
    /// </summary>
    /// <param name="p_percent"></param>
    public virtual void ChangeToolTransform(float p_percent)
    {
    }

    /// <summary>
    /// Checks to see if the coroutine can finish, depending on which state is currently active.
    /// </summary>
    /// <param name="p_currentTimer"></param>
    /// <returns></returns>
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
