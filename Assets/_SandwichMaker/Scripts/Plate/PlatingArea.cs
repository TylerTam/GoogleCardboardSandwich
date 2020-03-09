using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Unity Events
[System.Serializable]
public class PlatingEvent : UnityEngine.Events.UnityEvent { }

[System.Serializable]
public class PlatingObjectEvent : UnityEngine.Events.UnityEvent<int> { }

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



    #region Plate collider size variables
    [Header("Plate Colldier")]
    public BoxCollider m_plateCollider;
    private float m_colOriginalSizeY;
    private float m_colOriginalPositionY;
    private List<Transform> m_transformsInSandwhich = new List<Transform>();
    #endregion

    #region Plating Objects Variables
    private ObjectPooler m_pooler;
    private PlayerHand m_playerHand;
    public enum IngredientType
    {
        Bread, Meat, Veggies, Sauce
    }

    /// <summary>
    /// The prefabs that are placed on the object
    /// </summary>
    [System.Serializable]
    public struct FoodObjects
    {
        public string m_ingredientName;
        public GameObject m_objectOnSandwich;
        public IngredientType m_ingredientType;
    }
    public List<FoodObjects> m_foodObjects;
    public GameObject m_plateObject;

    public AnimationCurve m_platingAnimCurve;
    public float m_platingAnimTime;
    [Tooltip("Used to determine how high to start the placing animation")]
    public float m_startAnimHeight = 1;
    private BoxCollider m_lastPlacedObject;
    private Vector3 m_lastPlacedPos;

    private SandwhichType m_currentSandwhich = new SandwhichType();

    #endregion

    private void Start()
    {
        m_pooler = ObjectPooler.instance;
        m_playerHand = PlayerHand.Instance;
        m_colOriginalPositionY = transform.position.y + m_plateCollider.center.y;
        m_colOriginalSizeY = m_plateCollider.size.y;
    }

    #region Plating Objects Functions
    /// <summary>
    /// Adds the current held object to the sandwich
    /// </summary>
    /// <param name="p_objectType"></param>
    private void AddObjectToSandwich(int p_objectType)
    {
        m_currentSandwhich.m_hasTopBread = false;
        AddIngredientToSandwhichType(m_foodObjects[p_objectType].m_ingredientType);
        StartCoroutine(PlateFoodAnimation(p_objectType));
    }

    /// <summary>
    /// Adds the ingredient type to the sandwhich type
    /// </summary>
    /// <param name="p_currentIngredient"></param>
    private void AddIngredientToSandwhichType(IngredientType p_currentIngredient)
    {
        switch (p_currentIngredient)
        {
            case IngredientType.Bread:
                if (!m_currentSandwhich.m_hasBottomBread)
                {
                    m_currentSandwhich.m_hasBottomBread = true;
                }
                else
                {
                    m_currentSandwhich.m_hasTopBread = true;
                }
                break;
            case IngredientType.Meat:
                m_currentSandwhich.m_hasMeat = true;
                break;
            case IngredientType.Sauce:
                m_currentSandwhich.m_hasSauce = true;
                break;
            case IngredientType.Veggies:
                m_currentSandwhich.m_hasVegies = true;
                break;
        }
    }

    /// <summary>
    /// The animation of placing a food object onto the plate
    /// </summary>
    /// <param name="p_objectType"></param>
    /// <returns></returns>
    private IEnumerator PlateFoodAnimation(int p_objectType)
    {

        GameObject newFood = m_pooler.NewObject(m_foodObjects[p_objectType].m_objectOnSandwich, transform.position, Quaternion.identity);
        newFood.transform.parent = transform;

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
        currentFoodHeight += m_lastPlacedObject.size.y / 2;
        m_lastPlacedPos = new Vector3(0, currentFoodHeight, 0);
        newFood.transform.position = new Vector3(newFood.transform.position.x, currentFoodHeight + m_startAnimHeight, newFood.transform.position.z);

        RecalculatePlateCollider();
        m_transformsInSandwhich.Add(newFood.transform);


        float timer = 0;
        while (timer < m_platingAnimTime)
        {
            timer += Time.deltaTime;
            float newFoodYPos = Mathf.Lerp(currentFoodHeight + m_startAnimHeight, currentFoodHeight, m_platingAnimCurve.Evaluate(timer / m_platingAnimTime));
            newFood.transform.position = new Vector3(newFood.transform.position.x, newFoodYPos, newFood.transform.position.z);
            yield return null;
        }
        m_platingEvents.m_newObjectPlated.Invoke(p_objectType);

        newFood.GetComponent<FMODUnity.StudioEventEmitter>().Play();
    }

    /// <summary>
    /// Adjusts the plate collider to size up with the height of the sandwich
    /// </summary>
    private void RecalculatePlateCollider()
    {
        float sizeY = (m_colOriginalPositionY - (m_colOriginalSizeY / 2) + (m_lastPlacedObject.size.y / 2) + m_lastPlacedPos.y);
        m_plateCollider.size = new Vector3(m_plateCollider.size.x, sizeY, m_plateCollider.size.z);
        m_plateCollider.center = new Vector3(0, (m_plateCollider.size.y / 2), 0);
    }

    #endregion

    /// <summary>
    /// Checks to see if the interaction should be performed.
    /// </summary>
    /// <param name="p_playerHand"></param>
    /// <returns></returns>
    public bool InteractionValid()
    {
        if (m_playerHand.CurrentHeldObject() == null) return false;
        HeldFoodObject foodObject = m_playerHand.CurrentHeldObject().ReturnCurrentObject().GetComponent<HeldFoodObject>();
        if (foodObject != null)
        {
            if (!m_currentSandwhich.m_hasBottomBread)
            {
                if(foodObject.m_heldFoodIndex == 0)
                {
                    AddObjectToSandwich(foodObject.m_heldFoodIndex);
                    m_playerHand.EmptyHand(true);
                    return true;
                }
            }
            else
            {
                AddObjectToSandwich(foodObject.m_heldFoodIndex);
                m_playerHand.EmptyHand(true);
                return true;
            }
            
        }
        return false;

    }


    /// <summary>
    /// Creates an new plate object, and transfers the sandwich over to that
    /// </summary>
    public void FinishCurrentSandwich()
    {
        HoldablePlate newPlate =  m_pooler.NewObject(m_plateObject, transform.position, Quaternion.identity).GetComponent<HoldablePlate>();
        newPlate.CreatePlate(m_transformsInSandwhich, m_currentSandwhich);
        m_currentSandwhich.ResetMe();
        m_playerHand.PickUpObject(newPlate.gameObject);

        m_transformsInSandwhich.Clear();
        m_lastPlacedObject = null;
        m_plateCollider.size = new Vector3(m_plateCollider.size.x, m_colOriginalSizeY, m_plateCollider.size.z);
        m_plateCollider.center = new Vector3(m_plateCollider.center.x, m_colOriginalPositionY, m_plateCollider.center.z);

        m_playerHand.SetHoldingSandwhichState(true);

    }

    
}

public class SandwhichType
{
    public bool m_hasBottomBread;
    public bool m_hasTopBread;

    public bool m_hasMeat;
    public bool m_hasVegies;
    public bool m_hasSauce;
    public bool m_noSpecific;

    public void ResetMe()
    {
        m_hasBottomBread = m_hasTopBread = m_hasMeat = m_hasVegies = m_hasSauce = false;
    }

    public void SetSandwhichType(SandwhichType p_newSandwhichType)
    {
        m_hasBottomBread = p_newSandwhichType.m_hasBottomBread;
        m_hasTopBread = p_newSandwhichType.m_hasTopBread;
        m_hasMeat = p_newSandwhichType.m_hasMeat;
        m_hasVegies = p_newSandwhichType.m_hasVegies;
        m_hasSauce = p_newSandwhichType.m_hasSauce;
    }
    public bool MatchesSandwich(SandwhichType p_matchSandwich, out int p_errorType)
    {
        p_errorType = 0;
        if (!m_hasBottomBread)
        {
            return false;
        }
        if (!m_hasTopBread)
        {

            p_errorType = 1;
            return false;
        }

        if (p_matchSandwich.m_hasMeat)
        {
            if (!m_hasMeat)
            {
                p_errorType = 2;
                return false;
            }
        }

        if (p_matchSandwich.m_hasSauce)
        {
            if (!m_hasSauce)
            {
                p_errorType = 2;
                return false;
            }
        }
        if (p_matchSandwich.m_hasVegies)
        {
            if (m_hasVegies)
            {
                p_errorType = 2;
                return false;
            }
        }
        return true;
    }

    public void RandomizeSandwichType()
    {
        m_noSpecific = false;
        m_hasMeat = (Random.Range(0f, 1f) > .5f) ? true : false;
        m_hasVegies = (Random.Range(0f, 1f) > .5f) ? true : false;
        m_hasSauce = (Random.Range(0f, 1f) > .5f) ? true : false;

        if(m_hasMeat == m_hasVegies == m_hasSauce == false)
        {
            m_noSpecific = true;
        }
    }

    public bool GenericSandwich()
    {
        return m_hasMeat == m_hasVegies == m_hasSauce == false;
    }
}
