using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishCurrentPlate :MonoBehaviour, IInteractable
{
    public PlatingArea m_platingArea;


    private void Start()
    {
        GetOriginalMaterials();
    }
    
    public bool InteractionValid()
    {
        m_platingArea.FinishCurrentSandwich();
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
        foreach(MeshRenderer rend in m_renderers)
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
