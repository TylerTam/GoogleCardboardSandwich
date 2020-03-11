using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An interface used on every object that can be interacted with by the player
/// </summary>
public interface IInteractable
{

    /// <summary>
    /// Checks if the interaction can happen.
    /// The interaction will vary depending on the inheritor
    /// </summary>
    /// <returns></returns>
    bool InteractionValid();

    /// <summary>
    /// Used to determine if the player's cursor is currently ontop of this object
    /// </summary>
    void OnHoverOver();

    /// <summary>
    /// Used to determine if the player's cursor is has left this object
    /// </summary>
    void OnHoverLeft();
}
