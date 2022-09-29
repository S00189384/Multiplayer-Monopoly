using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public abstract class TileDisplay : MonoBehaviourPun
{
    protected SortingGroup sortingGroup;

    [HideInInspector] public TileData tileData;

    public abstract void UpdateDisplay(TileData tileData);

    public virtual void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();

        //spriteRenderer = GetComponent<SpriteRenderer>();
        //originalColour = spriteRenderer.color;
    }

    public void ChangeSortingLayerName(string name) => sortingGroup.sortingLayerName = name;

    //public virtual void ChangeToGrey()
    //{
    //    spriteRenderer.color = greyColour;
    //}
    //public virtual void ChangeColourToOriginal()
    //{
    //    spriteRenderer.color = originalColour;
    //}

    //public abstract void ProcessPlayer();
}