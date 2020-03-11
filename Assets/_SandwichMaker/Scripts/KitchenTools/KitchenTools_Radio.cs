using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenTools_Radio : MonoBehaviour, IInteractable
{
    public enum RadioState
    {
        ON,OFF
    }
    public RadioState m_radioState;
    public KitchenToolEvent m_radioStarted;
    public KitchenToolEvent m_radioStopped;

    private void Start()
    {
        GetOriginalMaterials();
    }
    public bool InteractionValid()
    {
        switch (m_radioState)
        {
            case RadioState.OFF:
                m_radioState = RadioState.ON;
                m_radioStarted.Invoke();
                break;
            case RadioState.ON:
                m_radioState = RadioState.OFF;
                m_radioStopped.Invoke();
                break;
        }
        return true;
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
