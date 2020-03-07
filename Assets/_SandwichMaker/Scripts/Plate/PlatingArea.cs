using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Unity Events
[System.Serializable]
public class PlatingEvent : UnityEngine.Events.UnityEvent { }

[System.Serializable]
public class PlatingObjectEvent:UnityEngine.Events.UnityEvent<int> { }

#endregion

/// <summary>
/// The area that the sandwich will be plated upon. Performs the logic of building the sandwich.
/// </summary>
public class PlatingArea : MonoBehaviour, IInteractable
{

    #region Unity Events
    public PlatingEvents m_platingEvents;
    [System.Serializable]
    public struct PlatingEvents
    {
        public PlatingEvent m_falseStartEvent;
        public PlatingObjectEvent m_newObjectPlated;
        public PlatingEvent m_sandwichCompleteEvent;
        
    }
    #endregion



    #region Plating Objects Variables
    private ObjectPooler m_pooler;
    private PlayerHand m_playerHand;
    /// <summary>
    /// The prefabs that are placed on the object
    /// </summary>
    [System.Serializable]
    public struct FoodObjects
    {
        public GameObject m_objectOnSandwich;
    }
    public List<FoodObjects> m_foodObjects;

    public AnimationCurve m_platingAnimCurve;
    public float m_platingAnimTime;
    [Tooltip ("Used to determine how high to start the placing animation")]
    public float m_startAnimHeight = 1;
    private BoxCollider m_lastPlacedObject;
    private Vector3 m_lastPlacedPos;
    #endregion

    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        m_playerHand = PlayerHand.Instance;
    }

    #region Plating Objects Functions
    /// <summary>
    /// Adds the current held object to the sandwich
    /// </summary>
    /// <param name="p_objectType"></param>
    private void AddObjectToSandwich(int p_objectType)
    {
        StartCoroutine(PlateFoodAnimation(p_objectType));
    }

    /// <summary>
    /// The animation of placing a food object onto the plate
    /// </summary>
    /// <param name="p_objectType"></param>
    /// <returns></returns>
    private IEnumerator PlateFoodAnimation(int p_objectType)
    {

        GameObject newFood = m_pooler.NewObject(m_foodObjects[p_objectType].m_objectOnSandwich, transform.position, Quaternion.identity);


        //Gets the correct placement of the new food to be placed;

        float currentFoodHeight = 0;

        //If the player has just placed the first bread for the sandwich
        if (m_lastPlacedObject == null)
        {
            //Get the collider of the plate, and place the first bread on that
            currentFoodHeight = transform.position.y + (GetComponent<BoxCollider>().size.y / 2);
            Debug.DrawLine(transform.position, transform.position + new Vector3(0, GetComponent<BoxCollider>().size.y / 2, 0), Color.magenta, 5);
        }
        else
        {
            currentFoodHeight = (m_lastPlacedObject.size.y / 2) + m_lastPlacedPos.y; 
        }
        
        m_lastPlacedObject = newFood.GetComponent<BoxCollider>();
        currentFoodHeight += m_lastPlacedObject.size.y/2;
        m_lastPlacedPos = new Vector3(0,currentFoodHeight,0);

        newFood.transform.position = new Vector3(newFood.transform.position.x,currentFoodHeight + m_startAnimHeight , newFood.transform.position.z);

        float timer = 0;
        while (timer < m_platingAnimTime)
        {
            timer += Time.deltaTime;
            float newFoodYPos = Mathf.Lerp(currentFoodHeight + m_startAnimHeight, currentFoodHeight, m_platingAnimCurve.Evaluate(timer / m_platingAnimTime));
            newFood.transform.position = new Vector3( newFood.transform.position.x, newFoodYPos, newFood.transform.position.z);
            yield return null;
        }
        m_platingEvents.m_newObjectPlated.Invoke(p_objectType);
    }

    #endregion

    /// <summary>
    /// Checks to see if the interaction should be performed.
    /// </summary>
    /// <param name="p_playerHand"></param>
    /// <returns></returns>
    public bool InteractionValid()
    {
        HeldFoodObject foodObject = m_playerHand.CurrentHeldObject().ReturnCurrentObject().GetComponent<HeldFoodObject>();
        if (foodObject != null)
        {
            AddObjectToSandwich(foodObject.m_heldFoodIndex);
            m_playerHand.EmptyHand(true);
            return true;
        }
        return false;

    }
}
