using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used on objects that can be held by the player
/// </summary>
public interface IHoldable
{
    /// <summary>
    /// Returns the layermask that the held object can be used with
    /// Used in the PlayerHand script to cast a raycast
    /// </summary>
    /// <returns></returns>
    LayerMask ReturnUsableLayerMask();

    /// <summary>
    /// Performs the function of the held object
    /// </summary>
    void UseObject();

    /// <summary>
    /// Activates the object's rigidbody
    /// </summary>
    void DropObject();

    /// <summary>
    /// Returns the game object that this script is attached to
    /// </summary>
    /// <returns></returns>
    GameObject ReturnCurrentObject();

    /// <summary>
    /// Activates the object's rigidbody, and applies a velocity to it
    /// </summary>
    /// <param name="p_speed"></param>
    /// <param name="p_dir"></param>
    void ThrowObject(float p_speed, Vector3 p_dir);
}
