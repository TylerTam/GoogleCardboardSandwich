using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeldFoodObject_Cooked : HeldFoodObject
{
    public override void UseObject()
    {
        gameObject.SetActive(false);
        base.UseObject();
    }
}
