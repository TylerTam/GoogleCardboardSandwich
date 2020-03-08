using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KitchenPanEvents : UnityEngine.Events.UnityEvent { }
/// <summary>
/// The class that allows the player to cook food
/// </summary>
public class KitchenTool_Pan : MonoBehaviour, IInteractable
{
    public float m_cookingTime;

    public List<CookedFoodItems> m_cookedVersions;
    public Transform m_cookingSpot;
    [System.Serializable]
    public struct CookedFoodItems
    {
        public string m_objectName;
        public GameObject m_cookedVersion;
        public int m_uncookedVersionIndex;
    }
    [System.Serializable]
    public struct CookingEvents
    {
        public KitchenPanEvents m_startCooking;
        public KitchenPanEvents m_finishedCooking;
    }

    private GameObject m_currentCookedObject;
    private PlayerHand m_playerHand;
    private bool m_isCooking;

    public BoxCollider m_collider;

    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
    }

    private void Update()
    {
        if (m_currentCookedObject != null)
        {
            if(!m_currentCookedObject.gameObject.activeSelf)
            {
                m_collider.enabled = true;
            }
        }
    }
    public bool InteractionValid()
    {
        
        if (m_isCooking || m_playerHand.CurrentHeldObject() == null) return false;

        if (m_currentCookedObject == null)
        {

            GameObject currentItem = m_playerHand.CurrentHeldObject().ReturnCurrentObject();
            m_playerHand.EmptyHand(false, false);
            currentItem.transform.position = m_cookingSpot.position;
            StartCoroutine(StartCooking(currentItem));
            return true;
        }
        else
        {
            if (!m_currentCookedObject.activeSelf)
            {
                GameObject currentItem = m_playerHand.CurrentHeldObject().ReturnCurrentObject();
                m_playerHand.EmptyHand(false, false);
                currentItem.transform.position = m_cookingSpot.position;
                StartCoroutine(StartCooking(currentItem));
                return true;
            }
        }
        return false;
    }

    private bool CanBeCooked(int p_itemIndex, out int p_foodIndex)
    {
        p_foodIndex = 0;
        foreach (CookedFoodItems cooked in m_cookedVersions)
        {
            if (cooked.m_uncookedVersionIndex == p_itemIndex)
            {
                p_foodIndex = m_cookedVersions.IndexOf(cooked);
                return true;
            }
        }
        return false;
    }

    private IEnumerator StartCooking(GameObject p_cookedObject)
    {
        m_isCooking = true;
        yield return new WaitForSeconds(m_cookingTime);
        HeldFoodObject currentCooked = p_cookedObject.GetComponent<HeldFoodObject>();
        if(currentCooked != null)
        {

            currentCooked.ResetMe();
            int currentFoodIndex = 0;
            if (CanBeCooked(currentCooked.m_heldFoodIndex, out currentFoodIndex))
            {
                m_collider.enabled = false;
                m_currentCookedObject = m_cookedVersions[currentFoodIndex].m_cookedVersion;
                m_currentCookedObject.SetActive(true);
            }
        }

        m_isCooking = false;

    }
}
