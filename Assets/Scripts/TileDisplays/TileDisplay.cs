using Photon.Pun;
using UnityEngine;
using UnityEngine.Rendering;

//Base script for a tiles appearance on the board.

public abstract class TileDisplay : MonoBehaviourPun
{
    protected SortingGroup sortingGroup;

    [HideInInspector] public TileData tileData;

    public abstract void UpdateDisplay(TileData tileData);

    public virtual void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    public void ChangeSortingLayerName(string name) => sortingGroup.sortingLayerName = name;
}