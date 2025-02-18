﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTool_Cupboard : KitchenTool
{
    
    public float p_offsetOpen;
    public float p_offestClosed;

    public override void ChangeToolTransform(float p_percent)
    {
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, Mathf.Lerp(p_offestClosed, p_offsetOpen, m_animCurve.Evaluate(p_percent)));
    }
}
