using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CrossHair : MonoBehaviour {

    public GamePlayManagerAR myGamePlayManager;
    public GameObject tileHighlighter;
    private LevelManager levelManager;
    private GameObject lastHit;
    private VRAR_Tile currentTile;

    public GameObject attackButtonPrefab;
    public GameObject inspectButtonPrefab;
    public GameObject walkButtonPrefab;
    public GameObject talkButtonPrefab;
    public GameObject handButtonPrefab;
    public GameObject returnButtonPrefab;
    public GameObject buttonCanvas;
    public Text currentTileText;

    public bool tapSelect = true;

    private Client client;

    void Start()
    {
        myGamePlayManager = GamePlayManagerAR.instance; 

        levelManager = LevelManager.Instance;

        //Bad code happenin here
        GameObject networkManager = GameObject.Find("NetworkManager");
        if (networkManager != null)
            client = networkManager.GetComponent<Client>();

        //TURN ALL THE BUTTONS OFF
        clearButtons();
    }

    
    void FixedUpdate() {
    
        //If we are in an interior ignore everything else
        if (GameStateManager.getInstance().getGlobalStateIndex() != GameStateManager.STATE_PLAYING)
        {
            if (GameStateManager.getInstance().getGlobalStateIndex() == GameStateManager.STATE_INTERIOR)
            {
                clearButtons();
                returnButtonPrefab.SetActive(true);
            }
            return;
        }
        //If we are not in an interior hide the exit interior button
        returnButtonPrefab.SetActive(false);


        if (tapSelect)
        {
            TapToSelect();
        } else
        {
            SelectCrosshair();
        }
    }

    private void TapToSelect()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            if (Input.GetMouseButtonUp(0) && !IsPointerOverUIObject())
            {
                SelectTile(Input.mousePosition);
            }
        } else
        {
            if (Input.touchCount == 1 && !IsPointerOverUIObject())
            {
                SelectTile(Input.GetTouch(0).position);
            }
        }
            
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    //Cast a ray from the specified screen position and select a tile if one is hit, otherwise currenttile is null
    private void SelectTile(Vector2 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);
        Debug.DrawRay(ray.origin, ray.direction, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, int.MaxValue))
        {
            //Debug.Log(hit.collider.gameObject.name);
            //Because we are in an update method. We don't wanna repeat the code over and over. That's why we have a checker.
            if (GiveNextHit(hit.collider.gameObject))
            {
                myGamePlayManager.objectHit = hit.collider.gameObject;
                //Debug.Log("hitTile :" + myGamePlayManager.objectHit.name);
                Vector2Int tilePos = LevelManager.Instance.getTilePosFromWorldPos(hit.transform.position);
                VRAR_Tile tile = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(tilePos.x, tilePos.y);
                if (tile != null)
                {
                    clearButtons();
                    currentTileText.text = tile.GetTerrain();
                    currentTile = tile;
                    updateButtons(tile);


                    TileRenderer.instance.updateTileHighlighter(hit.transform, true);
                }

                lastHit = hit.collider.gameObject;
            }
        }
        else
        {
            clearButtons();
            //we just want to turn it off, so we pass a random transform
            TileRenderer.instance.updateTileHighlighter(this.transform, false);
        }
    }

    private void SelectCrosshair()
    {
        
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));

        if (Application.platform != RuntimePlatform.Android)
        {
            //use mouse position if not on the phone
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            MouseInput();
        }

        RaycastHit hit;
        //Vector3 forward = transform.TransformDirection(Vector3.forward * int.MaxValue);

        Debug.DrawRay(ray.origin, ray.direction, Color.red);


        if (Physics.Raycast(ray, out hit, int.MaxValue))
        {
            //Debug.Log(hit.collider.gameObject.name);
            //Because we are in an update method. We don't wanna repeat the code over and over. That's why we have a checker.
            if (GiveNextHit(hit.collider.gameObject))
            {
                myGamePlayManager.objectHit = hit.collider.gameObject;
                //Debug.Log("hitTile :" + myGamePlayManager.objectHit.name);
                Vector2Int tilePos = LevelManager.Instance.getTilePosFromWorldPos(hit.transform.position);
                VRAR_Tile tile = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(tilePos.x, tilePos.y);
                if (tile != null)
                {
                    clearButtons();
                    currentTileText.text = tile.GetTerrain();
                    currentTile = tile;
                    updateButtons(tile);


                    TileRenderer.instance.updateTileHighlighter(hit.transform, true);
                }

                lastHit = hit.collider.gameObject;
            }
        }
        else
        {
            clearButtons();
            //we just want to turn it off, so we pass a random transform
            TileRenderer.instance.updateTileHighlighter(this.transform, false);
        }
    }
    
    private void MouseInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && attackButtonPrefab.activeSelf)
            AttackButtonPressed();
        if (Input.GetKeyDown(KeyCode.Alpha2) && inspectButtonPrefab.activeSelf)
            InspectButtonPressed();
        if (Input.GetKeyDown(KeyCode.Alpha3) && walkButtonPrefab.activeSelf)
            WalkButtonPressed();
        if (Input.GetKeyDown(KeyCode.Alpha4) && talkButtonPrefab.activeSelf)
            TalkButtonPressed();
        if (Input.GetKeyDown(KeyCode.Alpha5) && handButtonPrefab.activeSelf)
            HandButtonPressed();

        //temporary code to test select radius

        /*
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentTile == null)
                return;

            Debug.Log(currentTile.walkable);

            foreach (VRAR_Tile tile in GameStateManager.getInstance().getCurrentLevel().selectRadius(currentTile.tileIndex_X, currentTile.tileIndex_Y, 2))
            {
                tile.hexObject.GetComponent<MeshRenderer>().material = null;
            }
        }*/
        
    }

    private void clearButtons()
    {
        attackButtonPrefab.SetActive(false);
        inspectButtonPrefab.SetActive(false);
        walkButtonPrefab.SetActive(false);
        talkButtonPrefab.SetActive(false);
        handButtonPrefab.SetActive(false);
        currentTileText.text = "";
    }

    private void updateButtons(VRAR_Tile tile)
    {
        //We can only attack neighbours
        foreach (VRAR_Tile circleTile in GameStateManager.getInstance().getCurrentLevel().selectCircle(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, 1)) {
            if (circleTile == currentTile)
            {
                //if there is an npc show attack button
                NonPlayer npc = tile.getNPC();
                if (npc != null)
                {
                    attackButtonPrefab.SetActive(true);
                }
                //if there is another player also show attack button
                if (client != null)
                {
                    List<BasePlayer> players = client.getPlayerList();
                    foreach (BasePlayer player in players)
                    {
                        if (player.GetCurrentTile() == tile)
                        {
                            attackButtonPrefab.SetActive(true);
                        }
                    }
                }
            }          
        }
        


        if (tile.GetWalkable()) {
            walkButtonPrefab.SetActive(true);
            //if an object is walkable that means there is no object there so we can ignore te rest in this function
            return;
        }
        BaseTileObject baseTileObject = tile.getInteractableObject();
        //show hand if interactable
        if (baseTileObject.getInteractable() || tile.getInterior()!=null)
        {
            handButtonPrefab.SetActive(true);
            inspectButtonPrefab.SetActive(true);
            currentTileText.text = baseTileObject.getName();
        }
        //if its not interactable we want to allow the player to inspec the object, but not if ther is object (id == 0)
        else
        {
            inspectButtonPrefab.SetActive(true);
            currentTileText.text = baseTileObject.getName();
        }
    }
    /// <summary>
    /// A checker for to make sure the an action doesn't get repeated too much and to make sure we don't select the tilehighlighter
    /// </summary>
    /// <param name="hit"></param>
    /// <returns></returns>
    private bool GiveNextHit(GameObject hit)
    {
        if(lastHit == null)
        {
            return true;
        }
        else
        {
            if(hit == lastHit || hit == tileHighlighter)
            {
                return false;
                
            }
            else
            {
                return true;
            }
        }
    }

    public void WalkButtonPressed()
    {
        //Clear buttons and hide tilehighlighter
        clearButtons();
        TileRenderer.instance.updateTileHighlighter(this.transform, false);


        VRAR_Tile start = GamePlayManagerAR.instance.localPlayer.GetCurrentTile();
        VRAR_Tile end = currentTile;

        if (start == end)
            return;

        List<VRAR_Tile> tilePath = GameStateManager.getInstance().getCurrentLevel().findPath(start, end);
        //tilePath.Reverse();
        List<int> dirPath = GameStateManager.getInstance().getCurrentLevel().TilesToDirections(tilePath);

        //foreach (VRAR_Tile tile in tilePath)
        //{
            //tile.hexObject.GetComponent<MeshRenderer>().material = null;
            //Debug.Log(tile.tileIndex_X + "   " + tile.tileIndex_Y);
        //}

        //Debug.Log("TILES " + tilePath.Count + "     DIRS " + dirPath.Count);

        
        //This is a veryvery bad solution, but now it doesnt look like people walk through objects
        if (dirPath != null)
        {
            StartCoroutine("tempMove", dirPath);
        }
        

        /*
        Vector2Int pos = new Vector2Int(currentTile.tileIndex_X, currentTile.tileIndex_Y);
        foreach (int dir in dirPath)
        {

            pos += VRAR_Level.axialDirections[dir];
            GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(pos.x, pos.y).hexObject.GetComponent<MeshRenderer>().material = null;

            if (client != null)
            {
                client.QueueMove(dir);
            }
            else
            {
                GamePlayManagerAR.instance.localPlayer.Move(dir);
                TileRenderer.instance.walkLocalPlayer(dir);
            }                
        }*/
    }

    IEnumerator tempMove(List<int> dirPath)
    {
        foreach (int dir in dirPath)
        {
            if (client != null)
            {
                client.QueueMove(dir);
            }
            else
            {
                GamePlayManagerAR.instance.localPlayer.Move(dir);
                TileRenderer.instance.walkLocalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, dir);
            }
            yield return new WaitForSeconds(0.3f);
            //yield return new WaitForSeconds(0.5f);
        }
        yield return null;
    }


    public void InspectButtonPressed()
    {
        //Clear buttons and hide tilehighlighter
        //clearButtons();
        //TileRenderer.instance.updateTileHighlighter(this.transform, false);


        BaseTileObject baseTileObject = null;
        List<BaseTileObject> objectList = currentTile.getDumbObjectsList();
        if (objectList != null && objectList.Count > 0)
            baseTileObject = currentTile.getDumbObjectsList()[0];


        //show hand if interactable
        if (baseTileObject != null)
        {
            currentTileText.text = baseTileObject.getInspect();
            return;
        }

        NonPlayer npc = currentTile.getNPC();
        if (npc != null)
        {
            currentTileText.text = npc.GetInspect();
            return;
        }

        if (client != null)
        {
            List<BasePlayer> players = client.getPlayerList();
            foreach (BasePlayer player in players)
            {
                if (player.GetCurrentTile() == currentTile)
                {
                    currentTileText.text = "This is " + player.GetName();
                    return;
                }
            }
        }
    }

    public void AttackButtonPressed()
    {
        //Clear buttons and hide tilehighlighter
        //clearButtons();
        //TileRenderer.instance.updateTileHighlighter(this.transform, false);



        if (client != null)
        {
            Vector2Int target = new Vector2Int(currentTile.tileIndex_X, currentTile.tileIndex_Y);
            client.QueueAttack(0, target);
        } else
        {
            Debug.Log("cant do this without network yet");
        }
    }

    public void HandButtonPressed()
    {
        //Clear buttons and hide tilehighlighter
        //clearButtons();
        //TileRenderer.instance.updateTileHighlighter(this.transform, false);



        Debug.Log("HANDBUTTON");
        if(currentTile.getInterior()!=null)
        {
            GameStateManager.getInstance().setGlobalStateIndex(GameStateManager.STATE_INTERIOR);
            TileRenderer.instance.loadInterior(currentTile.getInterior());
        }
    }

    public void TalkButtonPressed()
    {
        //Clear buttons and hide tilehighlighter
        //clearButtons();
        //TileRenderer.instance.updateTileHighlighter(this.transform, false);



        Debug.Log("TALKBUTTON");
    }

    public void ReturnFromInteriorButtonPressed()
    {
        GameStateManager.getInstance().setGlobalStateIndex(GameStateManager.STATE_PLAYING);
        TileRenderer.instance.returnFromInterior();
    }

    private Color RandomColor()
    {
        return new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
        );
    }



}
