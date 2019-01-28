using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileObjectManger : MonoBehaviour {
    
    public static readonly Dictionary<int, BaseTileObject> TILE_OBJECTS = new Dictionary<int, BaseTileObject>();
    public static readonly Dictionary<int, NPCTileObject> NPC_TILE_OBJECTS = new Dictionary<int, NPCTileObject>();

    // NPC Tile Objects
    // [System.Obsolete("Old dictionary for holding npc's. They are all loaded in the TILE_OBJECTS dictionary")]
    //public static readonly Dictionary<int, NPCTileObject> NPC_OBJECTS = new Dictionary<int, NPCTileObject>();

    public List<Material> materials = new List<Material>();
    public static List<Material> staticMaterials = new List<Material>();

    //For Unity Editor Only!
    public Transform[] prefabsThisArrayWillBeReplacedWithAnAutomaticWay = new Transform[0];

    public Transform[] interiorPrefabsArray = new Transform[0];
    public static readonly Dictionary<int, VRAR_Interior_Base> INTERIOR_BASE_OBJECTS = new Dictionary<int, VRAR_Interior_Base>();

    public static int mountainTileMaterial = 0;
    public static int grassTileMaterial = 1;
    public static int waterTileMaterial = 2;
    public static int selectedTileMaterial = 3;
    public const int SPAWN_POINT_ID = 7;

    private static TileObjectManger _instance;
    public static TileObjectManger instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<TileObjectManger>();
            }

            return _instance;
        }
    }


    //-------------------------------------------------
    void Awake()
    {
        _instance = this;
        

    }

    void Start ()
    {
        //TODO add option to load any models from a path and generate the according prefabs
        //and a option to save custom made tileobjects
        
        addTileObject(0);//special TileObject which represents "no TileObject"
        addTileObject(1).setName("Big Tree 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[0]);
        addTileObject(2).setName("Pine 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[1]);
        addTileObject(3).setName("Interactable Test Object").setInteractable(true).setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[2]);
        addTileObject(4).setName("Cube").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[3]);
        addTileObject(5).setName("Rock 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[4]);
        addTileObject(6).setName("Rock 2").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[5]);
        addTileObject(SPAWN_POINT_ID).setName("Spawn Point").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[6]);
        addTileObject(8).setName("Mushroom 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[11]);
        addTileObject(9).setName("Mushroom 2").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[12]);
        addTileObject(10).setName("Mushroom 3").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[13]);
        addTileObject(11).setName("GrassPanes").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[10]);

        // NPC TileObjects
        addNPCTileObject(0).setName("Nick").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[7]);
        addNPCTileObject(1).setName("Govert").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[8]);
        addNPCTileObject(2).setName("Max").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[9]);

        addInteriorBaseObject(0).setName("Pub").setPrefab(interiorPrefabsArray[0]);



        foreach (Material mat in materials)
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

    private NPCTileObject addNPCTileObject(int id)
    {
        NPC_TILE_OBJECTS[id] = new NPCTileObject(id);
        return NPC_TILE_OBJECTS[id];
    }
    /*[System.Obsolete("Old way of adding npc's. Use the addTileObject method")]
    private NPCTileObject addNPCTileObject(int id)
    {
        NPC_OBJECTS[id] = new NPCTileObject(id);
        return NPC_OBJECTS[id];
    }*/
    private VRAR_Interior_Base addInteriorBaseObject(int id)
    {
        INTERIOR_BASE_OBJECTS[id] = new VRAR_Interior_Base(id);
        return INTERIOR_BASE_OBJECTS[id];
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
            case "Selection":
                res = staticMaterials[selectedTileMaterial];
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
    //VERBOTEN! private Vector3 scale;
    //VERBOTEN! private Vector3 position;

    //todo make components save
    private List<ActionComponentEnum> components = new List<ActionComponentEnum>();

    public BaseTileObject(int object_ID)
    {
        _objectName = "";
        _objectID = object_ID;
        _isInteractable = false;
    }

    /*
    public BaseTileObject(int object_ID, Vector3 position, Vector3 scale)
    {
        _objectName = "";
        _objectID = object_ID;
        _isInteractable = false;
        this.position = position;
        this.scale = scale;
    }
    */

    /*
     * //VERBOTEN!
    public Vector3 GetScale()
    {
        return scale;
    }

    //VERBOTEN!
    public Vector3 GetPosition()
    {
        return position;
    }*/

    public BaseTileObject setName(string objectName)
    {
        _objectName = objectName;
        return this;
    }

    /*
    //VERBOTEN!
    public GameObject GetGameObject()
    {
        GameObject ga = getPrefab().gameObject;
        ga.transform.position = position;
        ga.transform.localScale = scale;
        return ga;
    }*/

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

    /**
     * Action components
     **/

    public enum ActionComponentEnum
    {
        Health, Attack, Dialogue, Examine, Loot
    }

    public void addComponent(ActionComponentEnum component)
    {
        components.Add(component);
    }

    public void removeComponent(ActionComponentEnum component)
    {
        components.Remove(component);
    }

    public List<ActionComponentEnum> getComponents()
    {
        return components;
    }
}

/*
public class InteractableTileObject : BaseTileObject
{
    public InteractableTileObject(int object_ID) : base(object_ID)
    {
    }
}*/
