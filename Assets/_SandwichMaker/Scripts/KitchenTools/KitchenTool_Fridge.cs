using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTool_Fridge : KitchenTool, IInteractable
{

    public Transform m_fridgeDoor;
    public float m_openRotationY;
    public float m_closedRotationY;
    public override void ChangeToolTransform(float p_percent)
    {
        m_fridgeDoor.localEulerAngles = new Vector3(0, Mathf.Lerp(m_closedRotationY, m_openRotationY, m_animCurve.Evaluate(p_percent)), 0);

    }

   
}
