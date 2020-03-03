using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    private PlayerHand m_playerHand;

    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
    }

    public void PerformInteraction()
    {
        m_playerHand.PerformInteraction();
    }


}
