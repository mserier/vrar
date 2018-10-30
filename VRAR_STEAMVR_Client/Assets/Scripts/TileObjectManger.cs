using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectManger : MonoBehaviour {
    
    public static readonly Dictionary<int, BaseTileObject> TILE_OBJECTS = new Dictionary<int, BaseTileObject>();

    public List<Material> materials = new List<Material>();
    public static List<Material> staticMaterials = new List<Material>();


    public Transform[] prefabsThisArrayWillBeReplacedWithAnAutomaticWay = new Transform[0];

    public static int mountainTileMaterial = 0;
    public static int grassTileMaterial = 1;
    public static int waterTileMaterial = 2;


    void Start ()
    {
        //TODO add option to load any models from a path and generate the according prefabs
        addTileObject(0);//special TileObject which represents "no TileObject"
        addTileObject(1).setName("Capsule").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[0]);
        addTileObject(2).setName("Cube").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[1]);
        addTileObject(3).setName("Cylinder").setInteractable(true).setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[2]);
        addTileObject(4).setName("Sphere").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[3]);
            foreach(Material mat in materials)
        {
            staticMaterials.Add(mat);
        }
    }
	
	void Update ()
    {

		
	}

    private BaseTileObject addTileObject(int id)
    {
        TILE_OBJECTS[id] = new BaseTileObject(id);
        return TILE_OBJECTS[id];
    }

    public static Material getMaterial(string type)
    {
        Material res=null;
        switch(type)
        {
            case "Water":
                res = staticMaterials[waterTileMaterial];
                break;
            case "Mountain":
                res = staticMaterials[mountainTileMaterial];
                break;
            case "Grass":
                res = staticMaterials[grassTileMaterial];
                break;

        }
        if(res == null)
        {
            Debug.Log("return material is null");
        }
        return res;
    }
}

public class BaseTileObject
{
    private string _objectName;
    private Transform _objectModelPrefab;
    private bool _isInteractable;
    private int _objectID;

    public BaseTileObject(int object_ID)
    {
        _objectName = "";
        _objectID = object_ID;
        _isInteractable = false;
    }

    public BaseTileObject setName(string objectName)
    {
        _objectName = objectName;
        return this;
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

/*
public class InteractableTileObject : BaseTileObject
{
    public InteractableTileObject(int object_ID) : base(object_ID)
    {
    }
}*/