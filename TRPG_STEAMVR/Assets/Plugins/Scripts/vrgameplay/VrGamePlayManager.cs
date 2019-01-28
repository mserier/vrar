using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Designed for the behaviour of the vrplayer. Also holds all the players in the game.
/// </summary>
public class VrGamePlayManager : MonoBehaviour
{


    private List<BasePlayer> players = new List<BasePlayer>();
    private Dictionary<BasePlayer, PlayerPanel> panels = new Dictionary<BasePlayer, PlayerPanel>();
    private BasePlayer currentDungeonMaster;
    private Roles role;
    public PlayerPanel playerPanelPrefab;
    public Transform playerPanelHolder;

    /// <summary>
    /// Add a player to the list
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(BasePlayer player)
    {
        /*
        //Set the dungeon master if the given player is a dungeon master
        if (player.role == Roles.RoleEnum.DungeonMaster)
        {
            Debug.Log("Adding a dungeon master");
            currentDungeonMaster = player;
        }
        //Create a panel if the player is not a dungeon master
        else
        {
            Debug.Log("Adding " + player.GetRoleId());
            players.Add(player);
            InstantiatePanel(player);
            GameObject g =  Instantiate(role.GetRoleModel(player.role));
            g.transform.position = FindObjectOfType<LevelManager>().getWorldPosFromTilePos(player.spawnLocation.x, player.spawnLocation.y, 1);
            player.player = g;
        }
        */


        //spawn the players
        player.SpawnPlayer(player.spawnLocation);
        InstantiatePanel(player);
        //player.GetObject().transform.position = FindObjectOfType<LevelManager>().getWorldPosFromTilePos(player.spawnLocation.x, player.spawnLocation.y, 1);
        //player.GetObject().transform.position = TileInteractor.instance.lvlManager.getWorldPosFromTilePos(player.spawnLocation.x, player.spawnLocation.y);
        VRAR_Tile tile = GameStateManager.instance.getCurrentLevel().getTileFromIndexPos(player.spawnLocation.x, player.spawnLocation.y);
        player.GetObject().transform.position = TileInteractor.instance.lvlManager.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y, tile.height_ + 1);
    }
    
    private void Awake()
    {
        role = GetComponent<Roles>();
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
        panels[basePlayer].UpdateHealth(basePlayer.GetHealth());
    }

    public void UpdatePanelEnergy(BasePlayer basePlayer)
    {
        panels[basePlayer].UpdateEnergy(basePlayer.GetHealth());
    }

    /// <summary>
    /// Change the status of the turn for a player.
    /// </summary>
    /// <param name="basePlayer"></param>
    public void UpdatePanelTurn(BasePlayer basePlayer)
    {
        panels[basePlayer].UpdateTurn(basePlayer.HasTurn());
    }

    /// <summary>
    /// Give all players a turn(Including the dungeon master)
    /// </summary>
    public void GiveAllPlayersATurn()
    {
        foreach (BasePlayer player in players)
        {
            Debug.Log("Give turn");
            player.GiveTurn();
        }
        currentDungeonMaster.GiveTurn();
    }



    /// <summary>
    /// Create the panels for the dungeon master. 
    /// </summary>
    /// <param name="basePlayer"></param>
    private void InstantiatePanel(BasePlayer basePlayer)
    {
        PlayerPanel playerPanel = Instantiate(playerPanelPrefab);
        playerPanel.PlayerPanelConstruct(basePlayer.GetName(), basePlayer.GetMaxHealth(), basePlayer.GetMaxEnergy());
        playerPanel.transform.SetParent(playerPanelHolder, false);
        playerPanel.transform.localPosition = new Vector3(playerPanel.transform.localPosition.x, playerPanel.transform.localPosition.y, 0);
        //Add the panel to the dictionary with the basePlayer as the key so we can easily change it later
        panels.Add(basePlayer, playerPanel);

    }

    /// <summary>
    /// Give all players a new turn
    /// </summary>
    public void NewTurn()
    {
        foreach (BasePlayer player in players)
        {
            Debug.Log("New turn");
            player.GiveTurn();
        }

    }
    /// <summary>
    /// Take all the turns from all the players except the dungeon master
    /// </summary>
    public void TakeAllTurns()
    {
        foreach (BasePlayer player in players)
        {
            Debug.Log("Taking turn");
            player.TakeTurn();
        }
    }



    //TODO add some more cases
    /// <summary>
    /// Get class information
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    /*
    public Roles.RoleProperties getClassProperties(Roles.RoleEnum c)
    {
        Roles.RoleProperties returningClass = null;
        Debug.Log("Tring to get " + c + "properties");
        switch (c)
        {
            case Roles.RoleEnum.Thief:
                returningClass = role.GetThief();
                break;
            case Roles.RoleEnum.Necromancer:
                returningClass = role.GetNecromancer();
                break;
            case Roles.RoleEnum.Ranger:
                returningClass = role.GetRanger();
                break;
            case Roles.RoleEnum.DungeonMaster:
                Debug.Log("Player is a dungeon master so we dont return a RoleProperties");
                break;
        }
        return returningClass;
    }*/

    // Use this for initialization
    void Start()
    {
        role = GetComponent<Roles>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Save the players
    /// </summary>
   public void Save()
    {

        //TODO need to get the name of the campaign somehow so we can link the save to the right campaign
        TextWriter tw = new StreamWriter(Application.dataPath + "/PlayerSaves/" + "campaign" + "_player" + ".sav");
        foreach (BasePlayer player in players)
        {
            Debug.Log("Save player");
            tw.Write(player.GetPlayerId() + ";" + player.GetName() + ";" + player.GetRoleId() + ";" + player.GetHealth() + ";" + player.GetEnergy() + ";" + player.spawnLocation.x + ";" + player.spawnLocation.y);
            tw.Flush();
            tw.Close();
        }
    }

    /// <summary>
    /// Load the players of a campaign
    /// TODO whe should add campaign as an argument so we can get the save belonging to a certain campaign
    /// </summary>
    public void LoadPlayers()
    {

        TextReader tr;
        tr = new StreamReader(Application.dataPath + "/PlayerSaves/campaign_player.sav");
        string line;
        while ((line = tr.ReadLine()) != null)
        {
            string[] splitted = line.Split(';');
            if (splitted.Length > 1)
            {
                //BasePlayer bp = new BasePlayer(splitted[1], role.GetRoleByString(splitted[2]), this, new Vector2Int(int.Parse(splitted[5]), int.Parse(splitted[6])), int.Parse(splitted[0]));
                //AddPlayer(bp);
            }
        }

        }
    }
