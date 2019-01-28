using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScript : MonoBehaviour {

    public static CrossHair crossHair;
    public Text upeastButtonText;

    GamePlayManagerAR myGamePlayManager;
    //private GameObject GUI;
    private Client client;

    void Start()
    {
        myGamePlayManager = GamePlayManagerAR.instance;

        if (GameObject.Find("NetworkManager"))
        {
            client = GameObject.Find("NetworkManager").GetComponent<Client>();
        }
        //GUI = GameObject.Find("GUI");
        //myGamePlayManager.GUIState = GUI.activeSelf;
    }

    void Update()
    {
        /*
        if (myGamePlayManager.GUIState == true)
        {
            GUI.SetActive(true);  
        }

        if (myGamePlayManager.GUIState == false)
        {
            GUI.SetActive(false); 
        }*/
    }


	private Color RandomColor()
	{
		return new Color(
			UnityEngine.Random.Range(0f, 1f),
			UnityEngine.Random.Range(0f, 1f),
			UnityEngine.Random.Range(0f, 1f)
		);
	}

    bool upeast = true;
    public void WalkButton()
    {
        //tileRenderer.walk(myGamePlayManager.objectHit.transform.position);
        //myGamePlayManager.objectHit
    }

    public void ToggleUPDirection()
    {
        upeast = !upeast;
        upeastButtonText.text = upeast? "↗" : "↖"; //"⤢" : "⤡";

    }
    public void WalkUp()
    {
        //tileRenderer.walk(x,--y);
        if (client != null)
        {         
            

            client.QueueMove(upeast ? VRAR_Level.NORTH_EAST : VRAR_Level.NORTH_WEST);
        }
        else
        {
            GamePlayManagerAR.instance.localPlayer.Move(upeast ? VRAR_Level.NORTH_EAST : VRAR_Level.NORTH_WEST);
            TileRenderer.instance.walkLocalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, upeast ? VRAR_Level.NORTH_EAST : VRAR_Level.NORTH_WEST);
            //VRAR_Tile t = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.x, GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.y, VRAR_Level.getCounterTile(upeast ? VRAR_Level.NORTH_EAST : VRAR_Level.NORTH_WEST));
            //GamePlayManagerAR.instance.GetPlayers()[0].tileLocation = new Vector2Int(t.tileIndex_X, t.tileIndex_Y);
            //tileRenderer.walkExternalPlayer(GamePlayManagerAR.instance.GetPlayers()[0], upeast ? VRAR_Level.NORTH_EAST : VRAR_Level.NORTH_WEST);
        }
    }
    public void WalkDown()
    {
        //tileRenderer.walk(x, ++y);
        if (client != null)
        {
            client.QueueMove(upeast ? VRAR_Level.SOUTH_WEST : VRAR_Level.SOUTH_EAST);
        }
        else
        {
            GamePlayManagerAR.instance.localPlayer.Move(upeast ? VRAR_Level.SOUTH_WEST : VRAR_Level.SOUTH_EAST);
            TileRenderer.instance.walkLocalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, upeast ? VRAR_Level.SOUTH_WEST : VRAR_Level.SOUTH_EAST);
            //VRAR_Tile t = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.x, GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.y, VRAR_Level.getCounterTile(upeast ? VRAR_Level.SOUTH_WEST : VRAR_Level.SOUTH_EAST));
            //GamePlayManagerAR.instance.GetPlayers()[0].tileLocation = new Vector2Int(t.tileIndex_X, t.tileIndex_Y);
            //tileRenderer.walkExternalPlayer(GamePlayManagerAR.instance.GetPlayers()[0], upeast ? VRAR_Level.SOUTH_WEST : VRAR_Level.SOUTH_EAST);
        }
    }
    public void walkLeft()
    {
        
        //tileRenderer.walk(++x, y);
        if (client != null)
        {
            client.QueueMove(VRAR_Level.WEST);
        }
        else
        {
            GamePlayManagerAR.instance.localPlayer.Move(VRAR_Level.WEST);
            TileRenderer.instance.walkLocalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, VRAR_Level.WEST);
            //VRAR_Tile t = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.x, GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.y, VRAR_Level.getCounterTile(VRAR_Level.WEST));
            //GamePlayManagerAR.instance.GetPlayers()[0].tileLocation = new Vector2Int(t.tileIndex_X, t.tileIndex_Y);
            //tileRenderer.walkExternalPlayer(GamePlayManagerAR.instance.GetPlayers()[0], VRAR_Level.WEST);
        }
    }
    public void walkRight()
    {
        //tileRenderer.walk(--x, y);
        if (client != null)
        {
            client.QueueMove(VRAR_Level.EAST);
        }
        else
        {
            GamePlayManagerAR.instance.localPlayer.Move(VRAR_Level.EAST);
            TileRenderer.instance.walkLocalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, VRAR_Level.EAST);
            //VRAR_Tile t = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.x, GamePlayManagerAR.instance.GetPlayers()[0].tileLocation.y, VRAR_Level.getCounterTile(VRAR_Level.EAST));
            //GamePlayManagerAR.instance.GetPlayers()[0].tileLocation = new Vector2Int(t.tileIndex_X, t.tileIndex_Y);
            //tileRenderer.walkExternalPlayer(GamePlayManagerAR.instance.GetPlayers()[0], VRAR_Level.EAST);
        }
    }

    public void queueButtonThing()
    {
        //GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(GamePlayManagerAR.instance.localPlayer.spawnLocation.x, GamePlayManagerAR.instance.localPlayer.spawnLocation.y, VRAR_Level.getCounterTile(VRAR_Level.SOUTH_WEST));
        print("0 :" + GamePlayManagerAR.instance.gameObject.name);
        print("1 "+ GamePlayManagerAR.instance.localPlayer);
        print("2 " + GamePlayManagerAR.instance.localPlayer.spawnLocation);
        TileRenderer.instance.teleportLocalPlayer(GamePlayManagerAR.instance.localPlayer.spawnLocation.x, GamePlayManagerAR.instance.localPlayer.spawnLocation.y);

        //Client client = GameObject.Find("NetworkManager").GetComponent<Client>();
        //client.tempQueueMove();
    }
}
