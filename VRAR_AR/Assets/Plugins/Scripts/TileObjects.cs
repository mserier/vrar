using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjects : MonoBehaviour {

    public GameObject tilePrefab;
    public Material tileHighlightMat;

    public List<TileObjectData> tileObjectData = new List<TileObjectData>();
    public List<Material> tileMaterials = new List<Material>();

    //We use this list ok
    private Dictionary<int, BaseTileObject> tileObjects = new Dictionary<int, BaseTileObject>();

       
    public static TileObjects Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Debug.Log("Warning: multiple " + this + " in scene!"); }
        FillDictionary();
    }

    private void FillDictionary()
    {
        addTileObject(0);//special TileObject which represents "no TileObject"
        addTileObject(1).setName("Big Tree 1").setPrefab(tileObjectData[0].prefab);
        addTileObject(2).setName("Pine 1").setPrefab(tileObjectData[1].prefab);
        //addTileObject(3).setName("Interactable Test Object").setInteractable(true).setPrefab(tileObjectData[2].prefab);
        //addTileObject(4).setName("Cube").setPrefab(tileObjectData[3].prefab);
        addTileObject(5).setName("Rock 1").setPrefab(tileObjectData[4].prefab);
        addTileObject(6).setName("Rock 2").setPrefab(tileObjectData[5].prefab);
        addTileObject(7).setName("Spawn Point").setPrefab(tileObjectData[6].prefab);

        /*
        for(int i=0;i<interiorPrefabsArray.Count;i++)
        {
            if (interiorPrefabsArray[i] != null)
            {
                INTERIOR_BASE_OBJECTS[i].setObjectID(i);
                INTERIOR_BASE_OBJECTS.Add(i, interiorPrefabsArray[i]);
            }
        }*/
    }

    private BaseTileObject addTileObject(int id)
    {
        tileObjects[id] = new BaseTileObject(id);
        return tileObjects[id];
    }

    //This returns the material from a string, idk why this is really bad, we should start using a terrain ID or something
    public Material GetMaterial(string type)
    {
        Material res = null;
        switch (type)
        {
            case "Water":
                res = tileMaterials[2];
                break;
            case "Mountain":
                res = tileMaterials[0];
                break;
            case "Grass":
                res = tileMaterials[1];
                break;
            default:
                Debug.Log("return material is null");
                res = null;
                break;
        }
        return res;
    }
}

//Using the old basetileobject because otherwise a lot of shit needs to be changed, we could probably use tileobjectdata tho
public class BaseTileObject
{
    private string _objectName;
    private Transform _objectModelPrefab;
    private bool _isInteractable;
    private int _objectID;
    private string _objectInspect;


    public BaseTileObject(int object_ID)
    {
        //_objectName = "";
        _objectID = object_ID;
        _isInteractable = false;
    }

    public BaseTileObject setName(string objectName)
    {
        _objectName = objectName;
        return this;
    }

    public BaseTileObject setInspect(string inspect)
    {
        _objectInspect = inspect;
        return this;
    }

    public string getInspect()
    {
        return _objectInspect;
    }

    public string getName()
    {
        return _objectName;
    }

    public BaseTileObject setPrefab(Transform prefab)
    {
        _objectModelPrefab = prefab;
        return this;
    }

    public Transform getPrefab()
    {
        return _objectModelPrefab;
    }

    public BaseTileObject setInteractable(bool interactable)
    {
        _isInteractable = interactable;
        return this;
    }

    public bool getInteractable()
    {
        return _isInteractable;
    }

    public int getObjectID()
    {
        return _objectID;
    }
}