using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCurrentPlate :MonoBehaviour, IInteractable
{
    public PlatingArea m_platingArea;
    public bool InteractionValid()
    {
        print("Oi");
        m_platingArea.FinishCurrentSandwich();
        return true;
    }
}
