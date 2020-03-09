﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{

    bool InteractionValid();

    void OnHoverOver();

    void OnHoverLeft();
}
