using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
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
    public Transform selectedHexPrefab;
    private Transform selectedHexPrefabItem;
    //public Material selectedTileMaterial;
    //public Material defaultTileMaterial;
    //public Material waterTileMaterial;
    public Transform leftHandCursor;
    public Transform rightHandCursor;
    public Transform cursorMarker;
    public Transform tilesParent;
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
		
        //previousSelectedTileMaterial = TileObjectManger.getMaterial("Mountain");
        List<VRAR_Level>  lvls = lvlManager.getVRARLevels();
        if (lvls.Count == 0)
        {
            Debug.LogWarning("Trying to spawn lvls overview but none seem to be loaded.\nPossibly this script (TileRenderer) is loaded before the LevelManager script.\nYou can change the order in Unity editor: Edit -> Project Settings -> Script Execution Order + LevelManager + TileRenderer and make sure TileRenderer is below LevelManager");
        }
        else
        {
            if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR)
            {
                //Debug.Log("EDITOR");
                spawnLevelsOverview();
            }
            else if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
            {
                //Debug.Log("PLAY");
                selectedTiles.Add(GameStateManager.instance.getCurrentLevel().levelTile);
                StartCoroutine(slowSpawnLVL());
            }
        }
    }

    //private GameObject previousSelectedTile =null;
    private VRAR_Tile previousSelectedTile =null;
    //private Material previousSelectedTileMaterial = null;

    //TODO This should propably be kept in a gamestate manager
    static VRAR_Level lvl = null;
    public static VRAR_Level getCurrLVL() { return lvl; }

    private Vector3 truncY = new Vector3(1, 0, 1);
    //private Vector3 offX = new Vector3(1, 0, 0);
    private Vector3 offY = new Vector3(0, 1, 0);

    private VRAR_Tile selTile;
    private VRAR_Tile prevSelTile;
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
                        previousSelectedTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(previousSelectedTile.GetTerrain());
                    }

                    //Set material of selected tile
                    lvl.levelTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial("Selection");

                    //Add update list of selected tiles
                    selectedTiles.Clear();
                    selectedTiles.Add(lvl.levelTile);

                    //update debug text of current selected tile
                    selectedLevelText.text = "lvl :" + lvl.levelTile.tileIndex_X + "  " + lvl.levelTile.tileIndex_Y;
                    
                    previousSelectedTile = lvl.levelTile;
                }
                break;
            case GameStateManager.STATE_LEVEL_EDITOR:
                lvl = GameStateManager.instance.getCurrentLevel();
                cursorMarker.position = Vector3.Scale(rightHandCursor.position, (truncY)) + offY;
                selTile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(cursorMarker.position).x, lvlManager.getTilePosFromWorldPos(cursorMarker.position).y);
                if (selTile != null && selTile.hexObject !=null && !Valve.VR.InteractionSystem.VRInputHandler.isInTileObjectEditMenu)
                {

                    //if selectedHexPrefab is a prefab (so not in any scene)
                    if (selectedHexPrefab.gameObject.scene.name == null)
                    {
                        selectedHexPrefabItem = selectedHexPrefab;
                        //spawn prefab is scene
                        selectedHexPrefab = Instantiate(selectedHexPrefab);
                        
                    }


                    //Reset the previous tile back to normal
                    if (previousSelectedTile != null)
                    {
                        //previousSelectedTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(previousSelectedTile.GetTerrain());
                    }

                    //Set material of selected tile

                    //selTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial("Selection");
                    selectedHexPrefab.gameObject.transform.position = selTile.hexObject.position + (new Vector3(0,(float)(selTile.height_+0.1),0));

                    //Add update list of selected tiles
                    selectedTiles.Clear();
                    selectedTiles.Add(selTile);

                    //update debug text of current selected tile
                    selectedLevelText.text = "lvl :" + selTile.tileIndex_X + "  " + selTile.tileIndex_Y;

                    previousSelectedTile = selTile;

                    lvl.tileUpdate(0, 0);
                }
                break;
            case GameStateManager.STATE_PLAYING:
                lvl = GameStateManager.instance.getCurrentLevel();
                cursorMarker.position = Vector3.Scale(rightHandCursor.position, (truncY)) + offY;
                selTile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(cursorMarker.position).x, lvlManager.getTilePosFromWorldPos(cursorMarker.position).y);
                if (selTile != null && selTile.hexObject != null && !Valve.VR.InteractionSystem.VRInputHandler.isInTileObjectEditMenu)
                {
                    if(prevSelTile!=selTile)
                    {

                        //var tileMessage = new TileMessage(selTile);
                        if (prevSelTile != null && prevSelTile.hexObject!=null)
                        {
                            foreach(VRAR_Tile tile in GameStateManager.instance.getCurrentLevel().dirtyTiles)
                            {
                                var tileMessage = new TileMessage(tile, false);
                                NetworkServer.SendToAll(CustomNetMsg.Tile, tileMessage);
                            }
                            GameStateManager.instance.getCurrentLevel().dirtyTiles.Clear();
                        }
                    }

                    //if selectedHexPrefab is a prefab (so not in any scene)
                    if (selectedHexPrefab.gameObject.scene.name == null)
                    {
                        selectedHexPrefabItem = selectedHexPrefab;
                        //spawn prefab is scene
                        selectedHexPrefab = Instantiate(selectedHexPrefab);
                    }

                    //Set material of selected tile
                    selectedHexPrefab.gameObject.transform.position = selTile.hexObject.position + (new Vector3(0, selTile.height_ - 1, 0));

                    prevSelTile = selTile;
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

        //Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

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
            //nodeMarker.gameObject.GetComponent<Renderer>().material = defaultTileMaterial;
            nodeMarker.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial("Grass");
        }
    }

    public void spawnLevel()
    {
        //Vector2Int tPos = new Vector2Int(selectedTiles[0].tileIndex_X, selectedTiles[0].tileIndex_Y);
        VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();
        //VRAR_Level lvl = lvlManager.getLevelObjectFromTilePos(tPos);
        foreach (VRAR_Tile tile in lvl.vrarTiles)
        {
            //Create new empty gameobject to hold the tile and tileobjects
            /*GameObject tileObject = new GameObject(tile.ToString());
            tileObject.tag = "vrar_lvl";
            tileObject.transform.position = lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, 1);
            tileObject.transform.parent = tilesParent;
            tile.tileGameObject = tileObject.transform;*/

            updateTile(tile);

            /*
            //create the tile itself
            Transform nodeMarker = Instantiate(hexPREFAB);
            nodeMarker.parent = tileObject.transform;
            nodeMarker.localPosition = new Vector3(0,0,0);

            tile.hexObject = nodeMarker;
            nodeMarker.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(tile.terrain);
            nodeMarker.localScale = new Vector3(1, tile.height_, 1);

            //spawn interactable object
            if (TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab() != null)
            {
                Transform dumbObject = Instantiate(TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_+2), new Quaternion());
                dumbObject.parent = tileObject.transform;
            }

            //spawn dumb objects
            foreach (BaseTileObject dumbObject in tile.getDumbObjectsList())
            {
                if (dumbObject.getPrefab() != null)
                {
                    Transform interOjbect =Instantiate(dumbObject.getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 2), new Quaternion());
                    interOjbect.parent = tileObject.transform;
                }
            }*/

        }
    }

    public bool isSpawningLevel { get; private set; }

    public IEnumerator slowSpawnLVL()
    {
        isSpawningLevel = true;
        VRAR_Level lvl = GameStateManager.instance.getCurrentLevel();

        //do not use a foreach here incase the list gets edited while spawning
        for(int i=0;i<lvl.vrarTiles.Count;i++)
        {
            updateTile(lvl.vrarTiles[i]);
            yield return new WaitForSeconds(0.005f);
        }

        isSpawningLevel = false;

    }


    public void clearScene()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("vrar_lvl");
        foreach (GameObject ob in objects)
        {
            DestroyImmediate(ob);
        }
        if (selectedHexPrefab==null)
            selectedHexPrefab = selectedHexPrefabItem;
    }

    /*
	public void updateTile(VRAR_Tile newTile)
    {
        if(GameStateManager.instance.getGlobalStateIndex()==GameStateManager.STATE_LEVEL_EDITOR)
        {
            //newTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(newTile.terrain);
            newTile.hexObject.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(newTile.terrain);
            newTile.hexObject.localScale = new Vector3(1, newTile.height_, 1);

            foreach (Transform child in newTile.hexObject.transform)
            {
                Destroy(child.gameObject);
            }


            //spawn interactable object
            if (TileObjectManger.TILE_OBJECTS[newTile.getInteractableObject().getObjectID()].getPrefab() != null)
            {
                Transform dumbObject = Instantiate(TileObjectManger.TILE_OBJECTS[newTile.getInteractableObject().getObjectID()].getPrefab(), lvlManager.getWorldPosFromTilePos(newTile.tileIndex_X, newTile.tileIndex_Y, newTile.height_), new Quaternion());
                dumbObject.parent = newTile.hexObject;
            }

            //spawn dumb objects
            foreach (BaseTileObject dumbObject in newTile.getDumbObjectsList())
            {
                if (dumbObject.getPrefab() != null)
                {
                    Transform interOjbect = Instantiate(dumbObject.getPrefab(), lvlManager.getWorldPosFromTilePos(newTile.tileIndex_X, newTile.tileIndex_Y, newTile.height_ + 2), new Quaternion());
                    interOjbect.parent = newTile.hexObject;
                }
            }
        }
        else
        {
            Debug.LogError("TileRenderer.updateTile() called while game is in invalid state. State should be GameStateManager.STATE_LEVEL_EDITOR while the game is in :" + GameStateManager.instance.getGlobalStateIndex());
        }
    }*/

    public void updateTile(VRAR_Tile tile)
    {
        if (tile != null)
        {

            if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
            {
                if (!tile.isPhantom)
                {//spawn/update tile
                    if (tile.tileGameObject == null)
                    {
                        GameObject tileObject = new GameObject(tile.ToString());
                        tileObject.tag = "vrar_lvl";
                        tileObject.transform.position = lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, 1);
                        tileObject.transform.parent = tilesParent;
                        tile.tileGameObject = tileObject.transform;
                    }

                    Transform tileGameObjectTransform = tile.tileGameObject;
                    foreach (Transform child in tileGameObjectTransform)
                    {
                        Destroy(child.gameObject);
                    }

                    //create the tile itself
                    Transform nodeMarker = Instantiate(hexPREFAB);
                    nodeMarker.parent = tileGameObjectTransform;
                    nodeMarker.localPosition = new Vector3(0, 0, 0);
                    tile.hexObject = nodeMarker;

                    //Transform nodeMarker = tileGameObjectTransform.Find(hexPREFAB.name+"(Clone)");
                    nodeMarker.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(tile.terrain);
                    nodeMarker.localScale = new Vector3(1, tile.height_, 1);

                    //spawn interactable object
                    if (TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab() != null)
                    {
                        Transform dumbObject = Instantiate(TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 1), new Quaternion());
                        dumbObject.parent = tileGameObjectTransform.transform;
                        dumbObject.name = tile.getInteractableObject().getObjectID().ToString();
                    }

                    //spawn dumb objects
                    foreach (BaseTileObject dumbObject in tile.getDumbObjectsList())
                    {
                        if (dumbObject.getPrefab() != null)
                        {
                            int id = dumbObject.getObjectID();
                            Transform interOjbect = Instantiate(dumbObject.getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 1), new Quaternion());
                            Vector3 pos = tile.GetObjectPosition(id);
                            interOjbect.position = new Vector3(pos.x, tile.height_ + 1, pos.z);
                            interOjbect.localScale = tile.GetScale(id);
                            interOjbect.parent = tileGameObjectTransform.transform;
                            interOjbect.name = dumbObject.getObjectID().ToString();
                            interOjbect.localRotation = tile.GetRotation(id);
                        }
                        else
                        {
                            Debug.Log("Prefab is null");
                        }
                    }
                    //tile.addNPCTileObject();
                    if(tile.getNPC()!=null)
                    {
                        Transform interOjbect = tile.getNPC().SpawnNPC(new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y)).transform;
                        interOjbect.position = lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 1);
                        //Transform interOjbect = Instantiate(tile.getNPCTileObject().getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 1), new Quaternion());

                        //interOjbect.position = new Vector3(pos.x, tile.height_ + 1, pos.z);
                        //interOjbect.localScale = tile.GetScale(id);
                        interOjbect.parent = tileGameObjectTransform.transform;
                        interOjbect.name = tile.getNPC().ToString();
                        //interOjbect.localRotation = tile.GetRotation(id);
                    }
                    if (tile.getInterior() != null)
                    {
                        Transform interOjbect = Instantiate(tile.getInterior().getPrefab(), lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 1), new Quaternion());

                        //interOjbect.position = new Vector3(pos.x, tile.height_ + 1, pos.z);
                        //interOjbect.localScale = tile.GetScale(id);
                        interOjbect.parent = tileGameObjectTransform.transform;
                        interOjbect.name = tile.getInterior().getName();
                        //interOjbect.localRotation = tile.GetRotation(id);
                    }
                }
                else
                {//delete tile from world
                    if (tile.tileGameObject != null)
                    {
                        Destroy(tile.tileGameObject.gameObject);
                    }
                }
            }
            else
            {
                Debug.LogError("TileRenderer.updateTile() called while game is in invalid state. State should be GameStateManager.STATE_LEVEL_EDITOR while the game is in :" + GameStateManager.instance.getGlobalStateIndex());
            }
        }
    }

    /**********************************************
	 * Gui functions
	 **********************************************/


    public void loadLevelOnclick()
    {
        if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR && GameStateManager.instance.getCurrentLevel()!=null)
        {
            previousSelectedTile = null;
            clearScene();
            GameStateManager.instance.setGlobalStateIndex(GameStateManager.STATE_LEVEL_EDITOR);
            StartCoroutine(slowSpawnLVL());
            //GameStateManager.instance.setCurrentLevel(lvl);
        }
    }

    public void unloadLevelOnClick()
    {
        if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
        {
            previousSelectedTile = null;
            clearScene();
            GameStateManager.instance.setGlobalStateIndex(GameStateManager.STATE_CAMPAIGN_EDITOR);
            spawnLevelsOverview();
        }
    }

    public void saveLVLClick()
    {

        if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
        {
            //if in level, save that level
            //lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(leftHandCursor.position)).saveLVL();
            GameStateManager.instance.getCurrentLevel().saveLevel();
        }
    }
}
