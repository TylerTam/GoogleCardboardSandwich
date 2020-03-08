using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandwhichHandin : MonoBehaviour, IInteractable
{

    private PlayerHand m_playerHand;

    public Transform m_sandwhichHandInSpot;
    private Coroutine m_sandwhichTakenCoroutine;
    public float m_sandwhichTakenTime;

    private HoldablePlate m_finalSandwhich;

    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
    }
    public bool InteractionValid()
    {
        m_finalSandwhich = m_playerHand.CurrentHeldObject().ReturnCurrentObject().GetComponent<HoldablePlate>();
        m_playerHand.EmptyHand(true,false);

        StartCoroutine(TakeSandwhich());
        return true;

    }

    private IEnumerator TakeSandwhich()
    {
        m_finalSandwhich.transform.position = m_sandwhichHandInSpot.position;
        yield return new WaitForSeconds(m_sandwhichTakenTime);
        m_finalSandwhich.UseObject();

    }


}
