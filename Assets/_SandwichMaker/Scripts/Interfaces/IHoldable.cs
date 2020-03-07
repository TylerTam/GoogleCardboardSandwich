using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldable
{
    LayerMask ReturnUsableLayerMask();
    void UseObject();
    void DropObject();

    GameObject ReturnCurrentObject();

    void ThrowObject(float p_speed, Vector3 p_dir);
}
