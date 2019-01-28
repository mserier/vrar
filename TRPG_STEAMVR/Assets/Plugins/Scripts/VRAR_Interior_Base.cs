using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAR_Interior_Base : BaseTileObject
{
    private Transform interiorModelPrefab;

    public VRAR_Interior_Base(int object_ID) : base(object_ID)
    {

    }

    public void setInteriorModelPrefab(Transform prefab)
    {
        this.interiorModelPrefab = prefab;
    }
    public Transform getInteriorModelPrefab()
    {
        return interiorModelPrefab;
    }
}
