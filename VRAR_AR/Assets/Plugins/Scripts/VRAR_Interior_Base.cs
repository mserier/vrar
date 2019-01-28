using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAR_Interior_Base : BaseTileObject
{
    private Transform interiorModelPrefab;

    public VRAR_Interior_Base(int object_ID) : base(object_ID)
    {

    }

    public VRAR_Interior_Base setInteriorModelPrefab(Transform prefab)
    {
        this.interiorModelPrefab = prefab;
        return this;
    }
    public Transform getInteriorModelPrefab()
    {
        return interiorModelPrefab;
    }
}


/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Interior", menuName = "VRAR/InteriorBase")]
public class VRAR_Interior_Base : ScriptableObject
{
    public Transform interiorModelPrefab;

    /*
    public VRAR_Interior_Base(int object_ID)
    {
        _objectName = "";
        _objectID = object_ID;
        _isInteractable = false;
    }* /

    public void setInteriorModelPrefab(Transform prefab)
    {
        this.interiorModelPrefab = prefab;
    }
    public Transform getInteriorModelPrefab()
    {
        if(interiorModelPrefab==null)
        {
            return new GameObject().transform;
        }
        return interiorModelPrefab;
    }

    public string _objectName;

    private int _objectID;



    public VRAR_Interior_Base setName(string objectName)
    {
        _objectName = objectName;
        return this;
    }

    public string getName()
    {
        return _objectName;
    }

    public void setObjectID(int id)
    {
        _objectID = id;
    }

    public int getObjectID()
    {
        return _objectID;
    }
}
*/
