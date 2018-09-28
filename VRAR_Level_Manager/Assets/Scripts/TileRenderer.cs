using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
    Since we spawn the levelsOverview here for now make sure this scripts loads after TileObjectManager
    (In Unity editor: Edit -> Project Settings -> Script Execution Order
    +LevelManger
    +TileRenderer
    and make sure TileRenderer is below LevelManager
     */
public class TileRenderer : MonoBehaviour {


    public LevelManager lvlManager;

    public Transform hexPREFAB;
    public Material selectedTileMaterial;
    public Material defaultTileMaterial;
    public Transform ingameCursor;
    public Text selectedLevelText;

    private int stateIndex = 0;
    protected List<VRAR_Tile> selectedTiles = new List<VRAR_Tile>();


    // Use this for initialization
    void Start () {
        List<VRAR_Level>  lvls = lvlManager.getVRARLevels();
        if(lvls.Count==0)
        {
            Debug.LogWarning("Trying to spawn lvls overview but none seem to be loaded.\nPossibly this script (TileRenderer) is loaded before the LevelManager script.\nYou can change the order in Unity editor: Edit -> Project Settings -> Script Execution Order + LevelManager + TileRenderer and make sure TileRenderer is below LevelManager");
        }
        spawnLevelsOverview();
    }

    // Update is called once per frame
    void Update ()
    {
        VRAR_Level lvl;
        switch (stateIndex)
        {
            case 0:
                lvl = lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(ingameCursor.position));
                if (lvl != null)
                {
                    lvl.levelTile.hexObject.gameObject.GetComponent<Renderer>().material = selectedTileMaterial;
                    selectedTiles.Clear();
                    selectedTiles.Add(lvl.levelTile);
                    selectedLevelText.text = "lvl :" + lvl.levelTile.tileIndex_X + "  " + lvl.levelTile.tileIndex_Y;
                    //Vector2Int pos = getTilePosFromWorldPos(ingameCursor.position);
                }
                break;
            default:
                lvl = lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(ingameCursor.position));
                if (lvl != null)
                {
                    lvl.tileUpdate(0, 0);
                }
                break;
        }

    }

    public void spawnLevelsOverview()
    {
        //cleanup any existing gameobjects (old loaded levels or tiles)
        //place new gameobjects

        foreach (VRAR_Level lvl in lvlManager.getVRARLevels())
        {
            Transform nodeMarker = Instantiate(hexPREFAB, lvlManager.getWorldPosFromTilePos(lvl.levelTile.tileIndex_X, lvl.levelTile.tileIndex_Y, lvl.levelTile.height_), new Quaternion());
            lvl.levelTile.hexObject = nodeMarker;
            nodeMarker.gameObject.GetComponent<Renderer>().material = defaultTileMaterial;
        }
    }

    public void spawnLevel()
    {
        Vector2Int tPos = new Vector2Int(selectedTiles[0].tileIndex_X, selectedTiles[0].tileIndex_Y);
        VRAR_Level lvl = lvlManager.getLevelObjectFromTilePos(tPos);
        foreach (VRAR_Tile tile in lvl.vrarTiles)
        {
            Transform nodeMarker = Instantiate(hexPREFAB, lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_), new Quaternion());
            tile.hexObject = nodeMarker;
            nodeMarker.gameObject.GetComponent<Renderer>().material = defaultTileMaterial;
            nodeMarker.localScale = new Vector3(1, tile.height_, 1);

            //spawn interactable object
            if (TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab() != null)
            {
               Instantiate(TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_), new Quaternion());
            }

            //spawn dumb objects
            foreach (BaseTileObject dumbObject in tile.getDumbObjectsList())
            {
                if (dumbObject.getPrefab() != null)
                    Instantiate(dumbObject.getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_), new Quaternion());
            }

        }
    }
    

    public void clearScene()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("vrar_lvl");
        foreach (GameObject ob in objects)
        {
            Destroy(ob);
        }
    }

    /**********************************************
	 * Gui functions
	 **********************************************/


    public void loadLevelOnclick()
    {
        clearScene();
        spawnLevel();
        stateIndex = 1;
    }

    public void unloadLevelOnClick()
    {
        clearScene();
        spawnLevelsOverview();
        stateIndex = 0;
    }

    public void saveLVLClick()
    {
        lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(ingameCursor.position)).createLVLFile();
    }
}
