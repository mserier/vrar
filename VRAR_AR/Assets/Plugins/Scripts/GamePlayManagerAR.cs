using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayManagerAR : MonoBehaviour
{

    // key is unique id assigned by server.
    public Dictionary<int, BasePlayer> players = new Dictionary<int, BasePlayer>();
    //private List<BasePlayer> players = new List<BasePlayer>();

    private GameStateManager gameState;
    public bool GUIState = true;
    public int maxTileRadius;
    private Roles role;

    public GameObject objectHit { get; set; }

    public BasePlayer localPlayer { get; private set; }

    public bool turn = false;

    public BaseTileObject currentlySelectedObject;



    public void setLocalPlayer(BasePlayer player)
    {
        localPlayer = player;
    }



    public void AddPlayer(BasePlayer player)
    {
        if (!players.ContainsKey(player.GetPlayerId()))
        {
            players.Add(player.GetPlayerId(), player);
        }
        else
        {
            Debug.LogWarning("AddPlayer called but player was already in added");
        }
    }

    public Dictionary<int, BasePlayer> GetPlayers()
    {
        return players;
    }

    /*
    public void SetSelectedTile(VRAR_Tile tile)
    {
        ClearButtons();
        BaseTileObject baseTileObject = tile.getInteractableObject();
        if (tile.GetWalkable())
        {
            if (baseTileObject.getInteractable())
            {
                LoadInteractableButtons(baseTileObject);
            }
            else
            {
                EnableButton(walkButtonPrefab);
            }
        }
    }*/

    //Wtf does a tile being walkable have to do with the player? this is very bad and confusing
    private bool IsWalkable(VRAR_Tile tile)
    {
        if(localPlayer==null)
        {
            return false;
        }
        VRAR_Tile playerTile = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(localPlayer.GetCurrentTile().tileIndex_X, localPlayer.GetCurrentTile().tileIndex_Y);
        return GameStateManager.getInstance().getCurrentLevel().CheckWalkable(playerTile, tile, maxTileRadius); 
    }

    /*
    private void LoadInteractableButtons(BaseTileObject interact)
    {

        //Pseudo code for what happends in the future when stuff like NPCS and chests are done
        /*
        if(tileObject instance of npc){
            Instantiate(npcbmenu);
        }
         etc etc....
        
        EnableButton(attackButtonPrefab);
        EnableButton(inspectButtonPrefab);
        EnableButton(talkButtonPrefab);

    }


    public void ClearButtons()
    {
        foreach (Transform child in  buttonCanvas.transform)
        {
            child.gameObject.SetActive(false);
            //Destroy(child.gameObject);
        }
    }


    private void EnableButton(GameObject button)
    {
        button.SetActive(true);
    }
    */
    /*
	private static GamePlayManagerAR instance;

	public static GamePlayManagerAR getInstance(){
		if(instance == null){
			instance = new GamePlayManagerAR();
		}
		return instance;
	}


	private GamePlayManagerAR()
	{
	}*/

    public Roles GetRoles()
    {
        if(role == null)
        {
            role = FindObjectOfType<Roles>();
        }
        return role;
    }


    private static GamePlayManagerAR _instance;
    public static GamePlayManagerAR instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Instance is null making a new one");
                _instance = GameObject.FindObjectOfType<GamePlayManagerAR>();
            }

            return _instance;
        }
    }
    
    void Awake()
    {
        Debug.Log("Setting instance");
        _instance = this;
        role = GetComponent<Roles>();
    }

    // Use this for initialization
    void Start () {

		gameState = GameStateManager.getInstance();
        role = GetComponent<Roles>();

    }



	// Update is called once per frame
	void Update () {

	}


    public void startTurn()
    {
        //give roundstart energy
        //allow player to queue actions
    }

    public void stopTurn()
    {

    }

    /// <summary>
    /// Update all the values of a panel
    /// </summary>
    /// <param name="basePlayer"></param>
    public void UpdatePanel(BasePlayer basePlayer)
    {
        UpdatePanelHealth(basePlayer);
        UpdatePanelEnergy(basePlayer);
        UpdatePanelTurn(basePlayer);

    }


    public void UpdatePanelHealth(BasePlayer basePlayer)
    {
        //panels[basePlayer].UpdateHealth(basePlayer.GetHealth());
        Debug.LogWarning("UpdatePanelHealth is called but doesn't exist");
    }

    public void UpdatePanelEnergy(BasePlayer basePlayer)
    {
        //panels[basePlayer].UpdateEnergy(basePlayer.GetHealth());
        Debug.LogWarning("UpdatePanelEnergy is called but doesn't exist");
    }

    /// <summary>
    /// Change the status of the turn for a player.
    /// </summary>
    /// <param name="basePlayer"></param>
    public void UpdatePanelTurn(BasePlayer basePlayer)
    {
        //panels[basePlayer].UpdateTurn(basePlayer.HasTurn());
        Debug.LogWarning("UpdatePanelTurn is called but doesn't exist");
    }


    public void queuePlayerAction()
    {
        ARPlayerAction action = new ARPlayerAction().AttackAction();
        if(localPlayer.GetEnergy()>action.actionEnergyCost)
        {
            localPlayer.DecreaseEnergy(action.actionEnergyCost);
            Debug.Log("no spooky networking implemented :(");
        }
        else
        {
            Debug.Log("player hasn't got enough energy for this action");
        }
    }

    public void showGUI(bool shown)
    {
        GUIState = shown;
        GameObject.Find("GUI").SetActive(GUIState);
    }

    public void dumbWayToInstatiateGameObject(GameObject gameObject)
    {
        Instantiate(gameObject);
    }
}
