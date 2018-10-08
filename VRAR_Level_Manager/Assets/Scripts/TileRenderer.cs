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
    public Material waterTileMaterial;
    public Transform ingameCursor;
    public Text selectedLevelText;

    private int stateIndex = 0;
    protected List<VRAR_Tile> selectedTiles = new List<VRAR_Tile>();


    // Use this for initialization
    void Start () {
        previousSelectedTileMaterial = defaultTileMaterial;
        List<VRAR_Level>  lvls = lvlManager.getVRARLevels();
        if(lvls.Count==0)
        {
            Debug.LogWarning("Trying to spawn lvls overview but none seem to be loaded.\nPossibly this script (TileRenderer) is loaded before the LevelManager script.\nYou can change the order in Unity editor: Edit -> Project Settings -> Script Execution Order + LevelManager + TileRenderer and make sure TileRenderer is below LevelManager");
        }
        spawnLevelsOverview();
    }

    private GameObject previousSelectedTile =null;
    private Material previousSelectedTileMaterial = null;

    //TODO This should propably be kept in a gamestate manager
    static VRAR_Level lvl = null;
    public static VRAR_Level getCurrLVL() { return lvl; }

    // Update is called once per frame
    void Update ()
    {
        cursorMove();
        switch (stateIndex)
        {
            case 0:
                lvl = lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(ingameCursor.position));
                if (lvl != null)
                {

                    //Reset the previous tile back to normal
                    if (previousSelectedTile != null)
                    {
                        previousSelectedTile.GetComponent<Renderer>().material = previousSelectedTileMaterial;
                        previousSelectedTileMaterial = lvl.levelTile.hexObject.gameObject.GetComponent<Renderer>().material;
                    }

                    //Set material of selected tile
                    lvl.levelTile.hexObject.gameObject.GetComponent<Renderer>().material = selectedTileMaterial;

                    //Add update list of selected tiles
                    selectedTiles.Clear();
                    selectedTiles.Add(lvl.levelTile);

                    //update debug text of current selected tile
                    selectedLevelText.text = "lvl :" + lvl.levelTile.tileIndex_X + "  " + lvl.levelTile.tileIndex_Y;

                    previousSelectedTile = lvl.levelTile.hexObject.gameObject;
                }
                break;
            default:
                VRAR_Tile selTile=  lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);
                if (selTile != null)
                {

                    //Reset the previous tile back to normal
                    if (previousSelectedTile != null)
                    {
                        previousSelectedTile.GetComponent<Renderer>().material = previousSelectedTileMaterial;
                        previousSelectedTileMaterial = selTile.hexObject.gameObject.GetComponent<Renderer>().material;
                    }

                    //Set material of selected tile
                    selTile.hexObject.gameObject.GetComponent<Renderer>().material = selectedTileMaterial;

                    //Add update list of selected tiles
                    selectedTiles.Clear();
                    selectedTiles.Add(selTile);

                    //update debug text of current selected tile
                    selectedLevelText.text = "lvl :" + selTile.tileIndex_X + "  " + selTile.tileIndex_Y;

                    previousSelectedTile = selTile.hexObject.gameObject;

                    lvl.tileUpdate(0, 0);
                }
                break;
        }

    }

    /**
     * Temporary movement function (since we don't have the vive wand controllers
     **/
    private void cursorMove()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        if (moveHorizontal > 0.1f)
            moveHorizontal = 0.1f;
        if (moveVertical > 0.1f)
            moveVertical = 0.1f;
        if (moveHorizontal < -0.1f)
            moveHorizontal = -0.1f;
        if (moveVertical < -0.1f)
            moveVertical = -0.1f;

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        ingameCursor.Translate(new Vector3(moveHorizontal, 0, moveVertical));
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
        //if in level, save that level
        lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(ingameCursor.position)).createLVLFile();
        //else create new lvl from levelmanager
        //else print wut?
    }
}
