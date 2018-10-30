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
    public static TileRenderer instance;

    public LevelManager lvlManager;

    public Transform hexPREFAB;
    public Material selectedTileMaterial;
    public Material defaultTileMaterial;
    public Material waterTileMaterial;
    public Transform leftHandCursor;
    public Transform rightHandCursor;
    public Transform cursorMarker;
    public Text selectedLevelText;

    //private int stateIndex = 0;
    protected List<VRAR_Tile> selectedTiles = new List<VRAR_Tile>();

    


    // Use this for initialization
    void Start () {
        if (instance != null)
        {
            Debug.LogError("Warning someone tried to create a new TileRenderer instance even though one already exists! (Use TileRenderer.instance instead)");
            return;
        }
        instance = this;
		
        previousSelectedTileMaterial = defaultTileMaterial;
        List<VRAR_Level>  lvls = lvlManager.getVRARLevels();
        if (lvls.Count == 0)
        {
            Debug.LogWarning("Trying to spawn lvls overview but none seem to be loaded.\nPossibly this script (TileRenderer) is loaded before the LevelManager script.\nYou can change the order in Unity editor: Edit -> Project Settings -> Script Execution Order + LevelManager + TileRenderer and make sure TileRenderer is below LevelManager");
        }
        else
        {
            Debug.Log("thing :"+ GameStateManager.instance.getGlobalStateIndex());
            if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR)
            {
                Debug.Log("EDITOR");
                spawnLevelsOverview();
            }
            else if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
            {
                Debug.Log("PLAY");
                selectedTiles.Add(GameStateManager.instance.getCurrentLevel().levelTile);
                spawnLevel();
            }
        }
    }

    //private GameObject previousSelectedTile =null;
    private VRAR_Tile previousSelectedTile =null;
    private Material previousSelectedTileMaterial = null;

    //TODO This should propably be kept in a gamestate manager
    static VRAR_Level lvl = null;
    public static VRAR_Level getCurrLVL() { return lvl; }

    private Vector3 truncY = new Vector3(1, 0, 1);
    private Vector3 offX = new Vector3(1, 0, 0);
    private Vector3 offY = new Vector3(0, 1, 0);

    // Update is called once per frame
    void Update ()
    {
        //cursorMove();
        switch (GameStateManager.instance.getGlobalStateIndex())
        {
            case GameStateManager.STATE_CAMPAIGN_EDITOR:
                cursorMarker.position = Vector3.Scale(leftHandCursor.position,(truncY))+ offY;
                lvl = lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(leftHandCursor.position));
                if (lvl != null)
                {
                    GameStateManager.instance.setCurrentLevel(lvl);

                    //Reset the previous tile back to normal
                    if (previousSelectedTile != null)
                    {
                        //    previousSelectedTile.GetComponent<Renderer>().material = previousSelectedTileMaterial;
                        //    previousSelectedTileMaterial = lvl.levelTile.hexObject.gameObject.GetComponent<Renderer>().material;
                        previousSelectedTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(previousSelectedTile.GetTerrain());
                    }

                    //Set material of selected tile
                    lvl.levelTile.hexObject.gameObject.GetComponent<Renderer>().material = selectedTileMaterial;

                    //Add update list of selected tiles
                    selectedTiles.Clear();
                    selectedTiles.Add(lvl.levelTile);

                    //update debug text of current selected tile
                    selectedLevelText.text = "lvl :" + lvl.levelTile.tileIndex_X + "  " + lvl.levelTile.tileIndex_Y;

                    //previousSelectedTile = lvl.levelTile.hexObject.gameObject;
                    previousSelectedTile = lvl.levelTile;
                }
                break;
            case GameStateManager.STATE_LEVEL_EDITOR:
                lvl = GameStateManager.instance.getCurrentLevel();
                //Debug.Log("level state :"+lvl.levelTile.ToString());
                cursorMarker.position = Vector3.Scale(rightHandCursor.position, (truncY)) + offY;
                //cursorMarker.position *= 5f;
                //VRAR_Tile selTile=  lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(rightHandCursor.position).x, lvlManager.getTilePosFromWorldPos(rightHandCursor.position).y);
                VRAR_Tile selTile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(cursorMarker.position).x, lvlManager.getTilePosFromWorldPos(cursorMarker.position).y);
                if (selTile != null)
                {

                    //Reset the previous tile back to normal
                    if (previousSelectedTile != null)
                    {
                        //previousSelectedTile.GetComponent<Renderer>().material = previousSelectedTileMaterial;
                        //previousSelectedTileMaterial = selTile.hexObject.gameObject.GetComponent<Renderer>().material;
                        previousSelectedTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(previousSelectedTile.GetTerrain());
                    }

                    //Set material of selected tile
                    selTile.hexObject.gameObject.GetComponent<Renderer>().material = selectedTileMaterial;

                    //Add update list of selected tiles
                    selectedTiles.Clear();
                    selectedTiles.Add(selTile);

                    //update debug text of current selected tile
                    selectedLevelText.text = "lvl :" + selTile.tileIndex_X + "  " + selTile.tileIndex_Y;

                    //previousSelectedTile = selTile.hexObject.gameObject;
                    previousSelectedTile = selTile;

                    lvl.tileUpdate(0, 0);
                }
                break;
            case GameStateManager.STATE_PLAYING:
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

        leftHandCursor.Translate(new Vector3(moveHorizontal, 0, moveVertical));
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
        VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();
        //VRAR_Level lvl = lvlManager.getLevelObjectFromTilePos(tPos);
        foreach (VRAR_Tile tile in lvl.vrarTiles)
        {
            Transform nodeMarker = Instantiate(hexPREFAB, lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_), new Quaternion());
            tile.hexObject = nodeMarker;
            nodeMarker.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(tile.terrain);
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
	
	public void updateTile(VRAR_Tile newTile)
    {
        if(GameStateManager.instance.getGlobalStateIndex()==4)
        {
            newTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(newTile.terrain);
            newTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(newTile.terrain);
            newTile.hexObject.localScale = new Vector3(1, newTile.height_, 1);
        }
        else
        {
            Debug.LogError("TileRenderer.updateTile() called while game is in invalid state. State should be 4 while the game is in :" + GameStateManager.instance.getGlobalStateIndex());
        }
    }

    /**********************************************
	 * Gui functions
	 **********************************************/


    public void loadLevelOnclick()
    {
        previousSelectedTile = null;
        clearScene();
        spawnLevel();
        GameStateManager.instance.setGlobalStateIndex(GameStateManager.STATE_LEVEL_EDITOR);
        //GameStateManager.instance.setCurrentLevel(lvl);
    }

    public void unloadLevelOnClick()
    {
        previousSelectedTile = null;
        clearScene();
        spawnLevelsOverview();
        GameStateManager.instance.setGlobalStateIndex(GameStateManager.STATE_CAMPAIGN_EDITOR);
    }

    public void saveLVLClick()
    {
        //if in level, save that level
        //lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(leftHandCursor.position)).createLVLFile();
        GameStateManager.instance.getCurrentLevel().createLVLFile();
        //else create new lvl from levelmanager
        //else print wut?
    }
}
