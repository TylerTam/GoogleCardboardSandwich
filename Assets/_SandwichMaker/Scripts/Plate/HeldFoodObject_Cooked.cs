using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A cooked variation of a held food object.
/// </summary>
public class HeldFoodObject_Cooked : HeldFoodObject
{
    public override void UseObject()
    {
        gameObject.SetActive(false);
        base.UseObject();
    }
}
