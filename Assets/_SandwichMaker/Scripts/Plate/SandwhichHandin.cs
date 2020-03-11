using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SandwichEvent : UnityEngine.Events.UnityEvent { }

/// <summary>
/// The interactable that hands in the sandwich for final review
/// </summary>
public class SandwhichHandin : MonoBehaviour, IInteractable
{

    private PlayerHand m_playerHand;

    public Transform m_sandwhichHandInSpot;
    private Coroutine m_sandwhichTakenCoroutine;
    public float m_sandwhichTakenTime;

    private HoldablePlate m_finalSandwhich;
    private Animator m_aCont;
    public string m_animName;

    private bool m_canTakeSandwich = true;
    private int m_currentCorrectSandwiches;
    private SandwhichType m_currentSandwich;
    private SandwhichType m_desiredSandwich = new SandwhichType();
    


    [Header("Failiure Message")]
    public float m_failureMessageTime;
    public AnimationCurve m_errorFadeCurve;
    private Coroutine m_failureMessageCoroutine;
    public SandwichEvent m_sandwhichWrong, m_sandwichCorrect, m_sandwichHandedIn;


    /// <summary>
    /// Alerts the player to what was wrong within that sandwich, if anything was wrong
    /// </summary>
    #region Error Messages
    public ErrorMessages m_errorMessages;
    [System.Serializable]
    public struct ErrorMessages
    {
        public string m_ingredientsMissing;
        public string m_noBottomBread;
        public string m_noTopBread;
    }

    #endregion

    #region UI
    public UiElements m_uiElements;
    [System.Serializable]
    public struct UiElements
    {
        public UnityEngine.UI.Text m_errorText, m_correctSandwhichesText;
        public CanvasGroup m_errorCanvas;
        public GameObject m_request_meat, m_request_veg, m_request_sauce, m_requestNone;
    }
    #endregion

    

    private void Start()
    {
        m_playerHand = PlayerHand.Instance;
        m_aCont = GetComponent<Animator>();
        GetNewSandwich();
    }
    public bool InteractionValid()
    {
        if (m_canTakeSandwich)
        {
            m_finalSandwhich = m_playerHand.CurrentHeldObject().ReturnCurrentObject().GetComponent<HoldablePlate>();
            m_currentSandwich = m_finalSandwhich.GetComponent<HoldablePlate>().m_currentSandwhichType;
            m_playerHand.EmptyHand(true, false);

            StartTakenSandwichAnim();
            m_sandwichHandedIn.Invoke();
        }
        return true;

    }

    /// <summary>
    /// Calls the animation controller to start the animation
    /// </summary>
    private void StartTakenSandwichAnim()
    {
        m_canTakeSandwich = false;
        m_finalSandwhich.transform.position = m_sandwhichHandInSpot.position;
        m_finalSandwhich.transform.parent = m_sandwhichHandInSpot;
        m_finalSandwhich.transform.rotation = m_sandwhichHandInSpot.rotation;
        m_aCont.SetBool(m_animName, true);

        CheckSandwich();
        GetNewSandwich();
    }

    /// <summary>
    /// Creates a random new sanwich request, and displays it on the request board / ui
    /// </summary>
    private void GetNewSandwich()
    {
        m_desiredSandwich.RandomizeSandwichType();

        m_uiElements.m_correctSandwhichesText.text = m_currentCorrectSandwiches.ToString();

        m_uiElements.m_request_veg.SetActive(false);
        m_uiElements.m_request_meat.SetActive(false);
        m_uiElements.m_request_sauce.SetActive(false);
        m_uiElements.m_requestNone.SetActive(false);

        if (m_desiredSandwich.GenericSandwich())
        {
            m_uiElements.m_requestNone.SetActive(true);
        }
        else
        {
            if (m_desiredSandwich.m_hasMeat)
            {
                m_uiElements.m_request_meat.SetActive(true);
            }
            if (m_desiredSandwich.m_hasSauce)
            {
                m_uiElements.m_request_sauce.SetActive(true);
            }
            if (m_desiredSandwich.m_hasVegies)
            {
                m_uiElements.m_request_veg.SetActive(true);
            }
        }
        
    }

    /// <summary>
    /// Called from an animation event, in order to complete the cycle
    /// </summary>
    public void SandwichTaken()
    {
        m_canTakeSandwich = true;
        m_finalSandwhich.UseObject();
        m_aCont.SetBool(m_animName, false);
        m_finalSandwhich.transform.parent = null;
    }

    /// <summary>
    /// Checks to see if the sandwich was correct. If not, it returns an error message, and displays it on the ui for the player to see
    /// </summary>
    private void CheckSandwich()
    {
        int errorType = 0;
        if (!m_currentSandwich.MatchesSandwich(m_desiredSandwich, out errorType))
        {
            
            m_sandwhichWrong.Invoke();
            if(m_failureMessageCoroutine != null)
            {
                StopCoroutine(m_failureMessageCoroutine);
            }
            m_failureMessageCoroutine = StartCoroutine(FailureMessage(DisplayError(errorType)));
        }
        else
        {
            m_sandwichCorrect.Invoke();
            m_currentCorrectSandwiches++;
        }


    }

    /// <summary>
    /// Fades the failure message if one has been displayed
    /// </summary>
    /// <param name="p_message"></param>
    /// <returns></returns>
    private IEnumerator FailureMessage(string p_message)
    {
        m_uiElements.m_errorText.text = p_message;

        float timer = 0;
        while (timer < m_failureMessageTime)
        {
            timer += Time.deltaTime;
            float percent = timer / m_failureMessageTime;
            m_uiElements.m_errorCanvas.alpha = 1 - m_errorFadeCurve.Evaluate(percent);
            yield return null;
        }
        m_uiElements.m_errorCanvas.alpha = 0;
    }

    /// <summary>
    /// Returns the correct error message
    /// </summary>
    /// <param name="p_errorType"></param>
    /// <returns></returns>
    private string DisplayError(int p_errorType)
    {
        if (p_errorType == 0)
        {
            return m_errorMessages.m_noBottomBread;
        }
        else if (p_errorType == 1)
        {
            return m_errorMessages.m_noTopBread;
        }
        else if (p_errorType == 2)
        {
            return m_errorMessages.m_ingredientsMissing;
        }
        return "Something Went Wrong";
    }

    #region IInteractable Highlight
    [Header("Highlight")]
    public List<MeshRenderer> m_renderers;
    private List<Material> m_originalMaterials = new List<Material>();
    public Material m_highlightMaterial;
    private void GetOriginalMaterials()
    {
        foreach (MeshRenderer rend in m_renderers)
        {
            m_originalMaterials.Add(rend.material);
        }
    }
    public void OnHoverLeft()
    {
        foreach (MeshRenderer rend in m_renderers)
        {
            rend.material = m_originalMaterials[m_renderers.IndexOf(rend)];
        }
    }

    public void OnHoverOver()
    {
        foreach (MeshRenderer rend in m_renderers)
        {
            rend.material = m_highlightMaterial;
        }
    }
    #endregion
}
