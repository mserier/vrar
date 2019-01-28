using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileObjectManger : MonoBehaviour {

    public RawImage fadeOutOverlayImage;
    public static RawImage FADE_OUT_OVERLAY_IMAGE;
    public Transform HEX_PREFAB;
    //public Material highlightMaterial;
    public static Transform STATIC_HEX_PREFAB;
    public GameObject hexHighlightPrefab;
    public static GameObject STATIC_HEX_HIGHLIGHTER_PREFAB;
    public static GameObject STATIC_HEX_HIGHLIGHTER;
    //public static Material STATIC_HIGHLIGHT_MATERIAL;
    public static readonly Dictionary<int, BaseTileObject> TILE_OBJECTS = new Dictionary<int, BaseTileObject>();

    public Transform[] interiorPrefabsArray = new Transform[2];
    public static readonly Dictionary<int, VRAR_Interior_Base> INTERIOR_BASE_OBJECTS = new Dictionary<int, VRAR_Interior_Base>();

    public List<Material> materials = new List<Material>();
    public static List<Material> staticMaterials = new List<Material>();


    public Transform[] prefabsThisArrayWillBeReplacedWithAnAutomaticWay = new Transform[0];
    
    public static int mountainTileMaterial = 0;
    public static int grassTileMaterial = 1;
    public static int waterTileMaterial = 2;
    public const int SPAWN_POINT_ID = 7;


    void Awake ()
    {
        DontDestroyOnLoad(this.gameObject);

        //TODO add option to load any models from a path and generate the according prefabs
        addTileObject(0);//special TileObject which represents "no TileObject"
        addTileObject(1).setName("Big Tree 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[0]).setInspect("A very large tree");
        addTileObject(2).setName("Pine 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[1]).setInspect("This is a pine tree");
        addTileObject(3).setName("Interactable Test Object").setInteractable(true).setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[2]);
        addTileObject(4).setName("Cube").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[3]);
        addTileObject(5).setName("Rock 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[4]).setInspect("Rocky I");
        addTileObject(6).setName("Rock 2").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[5]).setInspect("Rocky II");
        addTileObject(SPAWN_POINT_ID).setName("Spawn Point").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[6]);
        addTileObject(8).setName("Mushroom 1").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[9]).setInspect("You find a mushroom on your trip");
        addTileObject(9).setName("Mushroom 2").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[10]).setInspect("A surprisingly big mushroom");
        addTileObject(10).setName("Mushroom 3").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[11]).setInspect("Would you look at that");
        addTileObject(11).setName("GrassPanes").setPrefab(prefabsThisArrayWillBeReplacedWithAnAutomaticWay[7]);
        foreach (Material mat in materials)
        {
            staticMaterials.Add(mat);
        }
        STATIC_HEX_PREFAB = HEX_PREFAB;
        STATIC_HEX_HIGHLIGHTER_PREFAB = hexHighlightPrefab;

        FADE_OUT_OVERLAY_IMAGE = fadeOutOverlayImage;

        addInteriorBaseObject(0).setInteriorModelPrefab(interiorPrefabsArray[1]).setPrefab(interiorPrefabsArray[0]).setName("Pub");
    }
	
	void Update ()
    {
	}

    private BaseTileObject addTileObject(int id)
    {
        TILE_OBJECTS[id] = new BaseTileObject(id);
        return TILE_OBJECTS[id];
    }

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

        }
        if(res == null)
        {
            Debug.Log("return material is null");
        }
        return res;
    }
}