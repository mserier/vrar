using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRAR_Tile
{
    public Transform tileGameObject;
    public Transform hexObject;

    public int tileIndex_X { get; set; }
    public int tileIndex_Y { get; set; }
    public float height_ { get; private set; }

    //private NPCTile
    private BaseTileObject interactableObject = TileObjectManger.TILE_OBJECTS[0];

    public List<BaseTileObject> dumbObjectsList = new List<BaseTileObject>();
    public Dictionary<int, Vector3> locationList = new Dictionary<int, Vector3>();
    public Dictionary<int, Vector3> scaleList = new Dictionary<int, Vector3>();
    public Dictionary<int, Quaternion> rotationList = new Dictionary<int, Quaternion>();

    //private NPCTileObject npcTileObject = null;
    private NonPlayer _npc = null;
    private VRAR_Interior_Base interiorObject = null;

    public string terrain;
    public bool walkable;
    private List<GameObject> objects = new List<GameObject>();




    public VRAR_Tile(int iX, int iY)
    {
        tileIndex_X = iX;
        tileIndex_Y = iY;
        height_ = 1f;
        //Some default values
        terrain = "Grass";
        walkable = true;


        //setInteractableObject(TileObjectManger.TILE_OBJECTS[3]);
        //addDumbObject(TileObjectManger.TILE_OBJECTS[1]);
        //addDumbObject(TileObjectManger.TILE_OBJECTS[4]);
    }

    public void AddScale(int tileObject, Vector3 scale)
    {
        scaleList.Add(tileObject, scale);
    }

    public void AddRotation(int tileObject, Quaternion quaternion)
    {
        rotationList.Add(tileObject, quaternion);
    }

	
    public Quaternion GetRotation(int tileObject)
    {
        return rotationList[tileObject];
    }

    public bool AddPosition(int tileObject, Vector3 position)
    {
        if (!locationList.ContainsKey(tileObject))
        {
            locationList.Add(tileObject, position);
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 GetObjectPosition(int key)
    {
        return locationList[key];
    }

    public Vector3 GetScale(int key)
    {
        return scaleList[key];
    }

    public VRAR_Tile(int iX, int iY, float height, string terrain, bool walkable)
    {
        tileIndex_X = iX;
        tileIndex_Y = iY;
        height_ = height;

        this.terrain = terrain;
        this.walkable = walkable;

        //setInteractableObject(TileObjectManger.TILE_OBJECTS[3]);
        //addDumbObject(TileObjectManger.TILE_OBJECTS[1]);
        //addDumbObject(TileObjectManger.TILE_OBJECTS[4]);
    }

    public bool GetWalkable()
    {
        return walkable;
    }
    public void SetWalkable(bool walk)
    {
        walkable = walk;
    }

    public void SetElevation(float ele)
    {
        height_ = ele;
    }

    public List<GameObject> GetObjects()
    {
        return objects;
    }

    public float GetElevation()
    {
        return height_;
    }

    public void SetTerrain(string terra)
    {
        Debug.Log("setTerra :" + terra);
        terrain = terra;
    }

    public string GetTerrain()
    {
        return terrain;
    }


    //Apply changes from the properties window
    public void UpdateList(List<BaseTileObject> editedList, Dictionary<int, Vector3> locationList, Dictionary<int, Vector3> scaleList, Dictionary<int, Quaternion> rotationList, float elevation, bool walk, string terrain)
    {
        this.dumbObjectsList = editedList;
        this.locationList = locationList;
        this.scaleList = scaleList;
        this.rotationList = rotationList;
        SetElevation(elevation);
        SetTerrain(terrain);
        SetWalkable(walk);
        //TileRenderer.instance.updateTile(this);

    }

    public void setInteractableObject(BaseTileObject tileObject)
    {
        //Debug.Log("THis created a bugged tile so disabling it for now");
        return;

        if (tileObject.getInteractable())
        {
            interactableObject = tileObject;
        }
        else
        {
            Debug.LogError("Someone tried to set a non interactable object as the interactable object (" + tileObject.getObjectID() + ") of the tile :" + this);
        }
    }

    public BaseTileObject getInteractableObject()
    {
        return interactableObject;
    }

    public void setInterior(VRAR_Interior_Base interior)
    {
        interiorObject = interior;
    }

    public VRAR_Interior_Base getInterior()
    {
        return interiorObject;
    }

    public void addDumbObject(BaseTileObject tileObject)
    {
        if (!tileObject.getInteractable())
        {
            dumbObjectsList.Add(tileObject);
        }
        else
        {
            Debug.LogError("Someone tried to set a interactable object as a dumb object of the tile :" + this);
        }
    }

    public void removeDumbObject(BaseTileObject tileObject)
    {
        dumbObjectsList.Remove(tileObject);
    }

    //public void addNPCTileObject(NPCTileObject npc)
    //{
    //    npcTileObject = npc;
    //}

	public NonPlayer getNPC()
    {
        return _npc;
    }

    public void setNPC(NonPlayer npc)
    {
        _npc = npc;
    }
	
    //public NPCTileObject removeTileObjecet()
    //{
    //    NPCTileObject npc = npcTileObject;
    //    npcTileObject = null;
    //    Debug.Log("Removed :" + npc);
    //    return npc;
    //}

    //public NPCTileObject getNPCTileObject()
    //{
    //    return npcTileObject;
    //}

    public List<BaseTileObject> getDumbObjectsList()
    {
        return dumbObjectsList;
    }

    public void setHeight(float height)
    {
        height_ = height;
        if (hexObject != null)
        {
            if (hexObject.gameObject != null)
                hexObject.gameObject.isStatic = false;
            hexObject.localScale = new Vector3(1, height_, 1);

            //Use this when the tileboject origin is in the middle of the model instead of at the bottom-middle of the model
            //hexObject.transform.SetPositionAndRotationposition.Set(hexObject.localPosition.x, height_*4f, hexObject.localPosition.z);
            //hexObject.gameObject.GetComponent<Renderer>().material = null;
            if (hexObject.gameObject != null)
                hexObject.gameObject.isStatic = true;
        }
        else
        {

        }
    }

    override
    public string ToString()
    {
        return "tile[" + tileIndex_X + "][" + tileIndex_Y + ']' + " h[" + height_ + ']';
    }
}