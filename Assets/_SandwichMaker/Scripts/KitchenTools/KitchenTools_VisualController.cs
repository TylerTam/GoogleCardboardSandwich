using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A script used to determine the animation states of the kitchen tools
/// </summary>
public class KitchenTools_VisualController : MonoBehaviour
{
    public Animator m_aCont;

    public string m_animBool = "Using";

    public void StartAnimation()
    {
        m_aCont.SetBool(m_animBool, true);
    }
    public void StopAnimation()
    {
        m_aCont.SetBool(m_animBool, false);
    }
}
