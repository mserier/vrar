using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TileInteractor : MonoBehaviour
{
    public LevelManager lvlManager;

    //temporary for setting the materials
    public TileRenderer tileRenderer;

    public Transform ingameCursor;

    public bool menuOpen = false;

    public Dropdown brushActionDropdown;
    public Slider brushSizeSlider;

    public TileObjectEditor TileEditMenuPrefab;
    private int currentBrush = 0;
   

    [HideInInspector]
    public bool TileToolEditTick = false;

    private bool brushTicking = false;
    public void setBrushTicking(bool ticking) { brushTicking = ticking; }
   // public void setBrushTickingT(bool b) { brushTicking = b; }

    private int brushSize = 1;
    public void setBrushSize(Slider slider) { brushSize = ((int)slider.value); }
    public void setBrushSizeByInt(int value) { brushSize = value; }
    public void setBrushSize(int size){brushSize = size ;}
    //private VRAR_Tile currentlySelected;

    private static TileInteractor _instance;
    public static TileInteractor instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TileInteractor>();
            }

            return _instance;
        }
    }


    //-------------------------------------------------
    void Awake()
    {
        _instance = this;


    }

   // UIRayCastScript uiRayCastScript;

    void Start ()
    {
        //uiRayCastScript = GameObject.Find("ScriptGameObject").GetComponent<UIRayCastScript>();


    }

    private VRAR_Tile lastSelectedTile;

    private int brushTickDivider = 4;
    private float maxTileHeight = 8f;
    private float minTileHeight = 1f;

	void FixedUpdate ()
    {
        //Debug.Log(brushTicking);

        
        if (brushTicking && (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR || GameStateManager.instance.getGlobalStateIndex() != GameStateManager.STATE_PLAYING) && !Valve.VR.InteractionSystem.VRInputHandler.isInTileObjectEditMenu)
        {
            //EditTile(brushActionDropdown.value);
            EditTile(currentBrush);
        }
        else if(brushTicking && GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
        {
            EditTile(currentBrush);
        }
        else if (TileToolEditTick && GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
        {
            TileToolEditTick = false;
            /*
            VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();//TileRenderer.getCurrLVL();


            VRAR_Tile tile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);
            if (tile != null)
            {
                //currentlySelected = tile;

                if (!menuOpen)
                {
                    menuOpen = true;
                    OpenTileProperties(tile);
                }
            }*/
        }
        else if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
        {
            VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();
            VRAR_Tile tile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);

            if (tile != null)
            {
                if (tile != lastSelectedTile)
                {
                    OpenTileProperties(tile);
                }
                lastSelectedTile = tile;
            }
        }


    }


    public void EditTile(int num)
    {

        VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();
        VRAR_Tile tile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);

        currentBrush = num;
        if (brushTickDivider-- == 0) // || true)
        {
           
            brushTickDivider = 4;
            //VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();//TileRenderer.getCurrLVL();

            //VRAR_Tile tile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);
            if (tile != null)
            {
               
                //currentlySelected = tile;
                List<VRAR_Tile> effectedTiles = lvl.selectRadius(tile.tileIndex_X, tile.tileIndex_Y, brushSize);

                //resetPrevous.Clear();
                // resetPrevous.Add(lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(leftHandCursor.position).x, lvlManager.getTilePosFromWorldPos(leftHandCursor.position).y));
                foreach (VRAR_Tile surroundTile in effectedTiles)
                {

                    if (surroundTile.hexObject != null)//phantom tiles shouldn't be changed
                    {
                        switch (num)
                        {
                            case 0:
                                if (surroundTile.height_ < maxTileHeight)
                                {
                                    surroundTile.setHeight(surroundTile.height_ + 0.1f);
                                    TileRenderer.instance.updateTile(surroundTile);//todo only do this after the user stopped changing the height (for optimization)
                                }
                                //else { surroundTile.setHeight(minTileHeight); }
                                break;
                            case 1:
                                if (surroundTile.height_ > minTileHeight)
                                {

                                    surroundTile.setHeight(surroundTile.height_ - 0.1f);
                                    TileRenderer.instance.updateTile(surroundTile);//todo only do this after the user stopped changing the height (for optimization)
                                }
                                //else { surroundTile.setHeight(maxTileHeight); }
                                break;

                            case 3:
                                {

                                    //remove tile
                                    VRAR_Tile remTile = lvl.removeTile(surroundTile.tileIndex_X, surroundTile.tileIndex_Y);

                                    var tileMessage = new TileMessage(remTile, true);
                                    NetworkServer.SendToAll(CustomNetMsg.Tile, tileMessage);

                                    TileRenderer.instance.updateTile(surroundTile);

                                }
                                break;
                            case 4:
                                if (surroundTile.hexObject != null)
                                {

                                    //surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.defaultTileMaterial;
                                    surroundTile.SetTerrain("Mountain");
                                    TileRenderer.instance.updateTile(surroundTile);
                                }
                                break;
                            case 5:
                                if (surroundTile.hexObject != null)
                                {

                                    //surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.waterTileMaterial;
                                    surroundTile.SetTerrain("Water");
                                    TileRenderer.instance.updateTile(surroundTile);
                                }
                                break;
                            case 6:
                                if (surroundTile.hexObject != null)
                                {

                                    //surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.selectedTileMaterial;
                                    surroundTile.SetTerrain("Grass");
                                    TileRenderer.instance.updateTile(surroundTile);
                                }
                                break;
                        }

                    }
                    else
                    {//functions that run on both existing and phantom tiles but when the cursor is still on a existing tile!
                        switch (num)
                        {
                            case 2:
                                //if tile is phantom tile, create actual
                                if (surroundTile.isPhantom)
                                {
                                    TileRenderer.instance.updateTile(lvl.addNewTile(surroundTile.tileIndex_X, surroundTile.tileIndex_Y));
                                }
                                break;
                        }
                    }
                }
                //lastSelectedTile = tile;
            }
            else
            {//cursor is not actually on a tile

                switch (num)
                {
                    case 2:
                        //spawn middle tile first (and the rest gets spawned in the next tick), and select amount around it
                        TileRenderer.instance.updateTile(lvl.addNewTile(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y));
                        break;
                    case 3:
                        {
                            VRAR_Tile middleTempTile = (lvl.addNewTile(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y));
                            List<VRAR_Tile> deletingTiles = lvl.selectRadius(middleTempTile.tileIndex_X, middleTempTile.tileIndex_Y, brushSize);

                            foreach (VRAR_Tile surroundTile in deletingTiles)
                            {
                                TileRenderer.instance.updateTile(lvl.removeTile(surroundTile.tileIndex_X, surroundTile.tileIndex_Y));
                            }
                        }
                        break;
                }

            }
        }


    }

    internal void genTerrain()
    {
        VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();
        foreach (VRAR_Tile tile in lvl.vrarTiles)
        {
            List<BaseTileObject> tileObjectList = tile.getDumbObjectsList();
            for(int i=0;i<tileObjectList.Count;i++)
            {
                if(tileObjectList[i].getObjectID()==5 || tileObjectList[i].getObjectID() == 2)
                {
                    tile.removeDumbObject(tileObjectList[i]);
                }
            }

            /*
            for(int i=0;i< tile.hexObject.transform.childCount;i++)
            {
                if (tile.hexObject.GetChild(i).CompareTag("vrar_generated_terrain"))
                {
                    print("removing :" + tile.hexObject.GetChild(i));
                    //tile.removeDumbObject()
                    Destroy(tile.hexObject.GetChild(i).gameObject);
                }
            }*/

            switch(tile.GetTerrain())
            {
                case "Water":
                    break;
                case "Mountain":
                    if(UnityEngine.Random.Range(0,10)>8)
                    {
                        addTileObject(tile, TileObjectManger.TILE_OBJECTS[5]);
                    }
                    break;
                case "Grass":
                    if (UnityEngine.Random.Range(0, 10) > 7)
                    {
                        addTileObject(tile, TileObjectManger.TILE_OBJECTS[2]);
                    }
                    break;
            }
        }
        TileRenderer.instance.clearScene();
        StartCoroutine(TileRenderer.instance.slowSpawnLVL());
    }

    private void addTileObject(VRAR_Tile tile, BaseTileObject tileObject)
    {
        //tile.AddPosition(tileObject.getObjectID(), tile.tileGameObject.position+new Vector3(UnityEngine.Random.Range(0.0f, 1f), 0, UnityEngine.Random.Range(0.0f, 1f)));
        Vector3 position = tile.tileGameObject.position + new Vector3(UnityEngine.Random.Range(0.0f, 1f), 0, UnityEngine.Random.Range(0.0f, 1f));
        float scale = UnityEngine.Random.Range(0.5f, 1f);
        //tile.AddScale(tileObject.getObjectID(), new Vector3(scale, scale, scale));
        Vector3 scaleVector = new Vector3(scale, scale, scale);
        //tile.AddRotation(tileObject.getObjectID(), Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 360), 0)));
        Quaternion rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 360), 0));
        tile.addDumbObject(tileObject, position, rotation, scaleVector);
        //TileRenderer.instance.updateTile(tile);
    }

    private void OpenTileProperties(VRAR_Tile tile)
    {
        if (TileObjectMenuReDoneEditionScript.Instance != null)
        {
            TileObjectMenuReDoneEditionScript.Instance.updateTileObjectsList(tile);
        }
        /*
        //Debug.Log("Object pressed");
        TileObjectEditor a = Instantiate(TileEditMenuPrefab);//, lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_)+new Vector3(0f, 20f, 0), new Quaternion());
        HandMenu.instance.ObjectToolTab = a.gameObject;
        print("set objectToolTab :"+HandMenu.instance.ObjectToolTab);
        a.transform.SetParent(handMennuButtonHolder);
        a.transform.localPosition = new Vector3();
        a.transform.localRotation = new Quaternion();
        a.transform.localScale = new Vector3(1, 1, 1);
        //UIRayCastScript.tileObjectEditor = a;

        updateTileProperties(tile, a);*/
    }

    public void updateTileProperties(VRAR_Tile tile, TileObjectEditor a)
    {
        a.LoadInObjects(tile.getDumbObjectsList());
        a.SetElevationCaption(tile.height_);
        a.SetTerrainCaption(tile.terrain);
        a.SetWalkable(tile.walkable);
        a.SetSelectedObject(tile);
        a.SetTileInteractor(this);
    }

    /*
    private void addTileObject(VRAR_Tile tile, BaseTileObject tileObject)
    {
        tile.AddPosition(tileObject.getObjectID(), tile.tileGameObject.position+new Vector3(UnityEngine.Random.Range(0.0f, 1f), 0, UnityEngine.Random.Range(0.0f, 1f)));
        float scale = UnityEngine.Random.Range(0.5f, 1f);
        tile.AddScale(tileObject.getObjectID(), new Vector3(scale, scale, scale));
        tile.AddRotation(tileObject.getObjectID(), Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 360), 0)));
        tile.addDumbObject(tileObject);
        //TileRenderer.instance.updateTile(tile);
    }

    private void OpenTileProperties(VRAR_Tile tile)
    {
        //Debug.Log("Object pressed");
        TileObjectEditor a = Instantiate(TileEditMenuPrefab, lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_)+new Vector3(0f, 20f, 0), new Quaternion());
        UIRayCastScript.tileObjectEditor = a;
        
        a.LoadInObjects(tile.getDumbObjectsList());
        a.SetElevationCaption(tile.height_);
        a.SetTerrainCaption(tile.terrain);
        a.SetWalkable(tile.walkable);
        a.SetSelectedObject(tile);
        a.SetTileInteractor(this);


    }*/
}
