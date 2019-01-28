using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MoveRequest
{
    public int id;
    public int direction;
}

public class Server : MonoBehaviour {

    public LobbyGuiScript lobbyGui;
    List<Vector2Int> spawnLocations = new List<Vector2Int>();

    Dictionary<int, BasePlayer> players = new Dictionary<int, BasePlayer>();
    List<MoveRequest> moveRequests = new List<MoveRequest>();
    bool ready = false;
    string level = "";

    Dissonance.Integrations.UNet_LLAPI.UNetCommsNetwork UNetDissonance;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public bool Host()
    {
        if (NetworkServer.Listen(45555))
        {
            Debug.Log("Server is listening...");
            NetworkServer.RegisterHandler(MsgType.Connect, GotClient);
            NetworkServer.RegisterHandler(MsgType.Disconnect, LostClient);

            // custom msgs
            NetworkServer.RegisterHandler(CustomNetMsg.Ready, GotReady);
            NetworkServer.RegisterHandler(CustomNetMsg.Name, GotName);
            NetworkServer.RegisterHandler(CustomNetMsg.Role, GotRole);
            NetworkServer.RegisterHandler(CustomNetMsg.MoveRequest, GotMoveRequest);
            NetworkServer.RegisterHandler(CustomNetMsg.AttackRequest, GotAttackRequest);

            UNetDissonance = GetComponent<Dissonance.Integrations.UNet_LLAPI.UNetCommsNetwork>();
            // initialize dissonance as server.
            UNetDissonance.InitializeAsServer();
            // make sure game master can talk to everyone.
            GetComponent<Dissonance.VoiceBroadcastTrigger>().ChannelType = Dissonance.CommTriggerTarget.Room;

            return true;
        }

        return false;
    }

    public void UpdateReady(bool ready)
    {
        this.ready = ready;

        CheckEveryoneReady();
    }

    public void UpdateLevel(string level)
    {
        this.level = level;
    }

    void GotReady(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ReadyMessage>();

        players[netMsg.conn.connectionId].ready = msg.ready;

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }

        // broadcast
        var readyMessage = new ReadyMessage();
        readyMessage.id = netMsg.conn.connectionId;
        readyMessage.ready = msg.ready;
        NetworkServer.SendToAll(CustomNetMsg.Ready, readyMessage);

        CheckEveryoneReady();
    }

    void CheckEveryoneReady()
    {
        foreach (KeyValuePair<int, BasePlayer> entry in players)
        {
            if (entry.Value.ready == false)
            {
                return;
            }
        }

        // if all are ready, including the server (me), start the game.
        if (ready)
        {
            // notify all clients to start game.
            var startGameMessage = new StartGameMessage();
            startGameMessage.level = level;
            NetworkServer.SendToAll(CustomNetMsg.StartGame, startGameMessage);

            GameStateManager.instance.setGlobalStateIndex(GameStateManager.STATE_PLAYING);
            SceneManager.LoadScene(1);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void ParseSpawnLocations() {
        foreach (VRAR_Tile tile in GameStateManager.instance.getCurrentLevel().vrarTiles)
        {
            foreach (BaseTileObject baseTileObject in tile.getDumbObjectsList()) {
                if (baseTileObject.getObjectID() == TileObjectManger.SPAWN_POINT_ID)
                {
                    spawnLocations.Add(new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y));
                }
            }
        }

        Debug.Log("Parsed :" + spawnLocations.Count + " spawn locations.");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            GameObject scriptGameObject = GameObject.Find("ScriptGameObject");

            VrGamePlayManager manager = scriptGameObject.GetComponent<VrGamePlayManager>();

            ParseSpawnLocations();

            int i = 0;

            foreach (KeyValuePair<int, BasePlayer> entry in players)
            {
                BasePlayer player = entry.Value;
                // set a random spawn location.
                if (i > spawnLocations.Count)
                {
                    List<VRAR_Tile> vrarTiles = GameStateManager.instance.getCurrentLevel().vrarTiles;
                    VRAR_Tile tile = vrarTiles[UnityEngine.Random.Range(0, vrarTiles.Count)];
                    player.spawnLocation = new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y);
                }
                // set one of the parsed spawn locations.
                else
                {
                    player.spawnLocation = spawnLocations[i++];
                }
                
                manager.AddPlayer(player);

                // broadcast spawn location.
                var spawnMessage = new SpawnMessage();
                spawnMessage.id = entry.Value.GetPlayerId();
                spawnMessage.spawnLocation = player.spawnLocation;
                NetworkServer.SendToAll(CustomNetMsg.Spawn, spawnMessage);
            }

            FindObjectOfType<ActionsGuiScript>().server = this;
        }
    }

    void GotName(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NameMessage>();

        players[netMsg.conn.connectionId].SetName(msg.name);

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }

        // broadcast
        var nameMessage = new NameMessage();
        nameMessage.id = netMsg.conn.connectionId;
        nameMessage.name = msg.name;
        NetworkServer.SendToAll(CustomNetMsg.Name, nameMessage);
    }

    void GotRole(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<RoleMessage>();

        players[netMsg.conn.connectionId].SetRole(msg.role);


        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }

        // broadcast
        var roleMessage = new RoleMessage();
        roleMessage.id = netMsg.conn.connectionId;
        roleMessage.role = msg.role;
        NetworkServer.SendToAll(CustomNetMsg.Role, roleMessage);
    }

    void GotClient(NetworkMessage netMsg)
    {
        Debug.Log("Got client...");

        // send everyone else to the new player.
        foreach (KeyValuePair<int, BasePlayer> entry in players)
        {
            var playerMessage = new PlayerMessage();
            playerMessage.id = entry.Value.GetPlayerId();
            playerMessage.name = entry.Value.GetName();
            playerMessage.ready = entry.Value.ready;
            playerMessage.role = entry.Value.GetRoleId();
            netMsg.conn.Send(CustomNetMsg.Player, playerMessage);
        }

        //default anme is player + id, default roleint is 0
        players[netMsg.conn.connectionId] = new BasePlayer("Player " + netMsg.conn.connectionId, 0, netMsg.conn.connectionId);
        //players[netMsg.conn.connectionId].id = netMsg.conn.connectionId;

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }

        // send the new player to everyone else.
        var idMessage = new IdMessage();
        idMessage.id = netMsg.conn.connectionId;
        NetworkServer.SendToAll(CustomNetMsg.Id, idMessage);

        // send server dissonance id to the new player.
        var dissonanceIdMessage = new DissonanceIdMessage();
        dissonanceIdMessage.id = 0;
        dissonanceIdMessage.dissonanceId = UNetDissonance.PlayerName;
        netMsg.conn.Send(CustomNetMsg.DissonanceId, dissonanceIdMessage);
    }

    void GotMoveRequest(NetworkMessage netMsg)
    {
        var moveRequestMessage = netMsg.ReadMessage<MoveRequestMessage>();
        MoveRequest mr = new MoveRequest();
        mr.id = netMsg.conn.connectionId;
        mr.direction = moveRequestMessage.direction;
        moveRequests.Add(mr);

        UpdateMoveRequestUI();
    }

    //The attack request ignores action queue stuff and auto accepts, so I dont have to rewrite the queue system that seems to only work for movement
    void GotAttackRequest(NetworkMessage netMsg)
    {
        AttackRequestMessage attackRequest = netMsg.ReadMessage<AttackRequestMessage>();

        AttackMessage attackMessage = new AttackMessage();
        attackMessage.id = netMsg.conn.connectionId;
        attackMessage.attackId = attackRequest.attackId;
        attackMessage.targetTileX = attackRequest.targetTileX;
        attackMessage.targetTileY = attackRequest.targetTileY;
        //FOr now we will do random damage
        attackMessage.result = Random.Range(5, 9);

        NetworkServer.SendToAll(CustomNetMsg.Attack, attackMessage);
        handleAttack(attackMessage.id, attackMessage.attackId, attackMessage.targetTileX, attackMessage.targetTileY, attackMessage.result);
    }

    //handle the attack that is SendToAll on the server itself
    void handleAttack(int id, int attackId, int targetTileX, int targetTileY, int result)
    {
        VRAR_Tile tile = GameStateManager.instance.getCurrentLevel().getTileFromIndexPos(targetTileX, targetTileY);
        NonPlayer npc = tile.getNPC();
        if (npc != null)
        {
            npc.DecreaseHealth(result);
        }
        else //if there is no npc check if there is a player
        {
            foreach (BasePlayer player in players.Values)
            {
                if (player.GetCurrentTile() == tile)
                {
                    //the player died ! move them to their spawn location
                    player.DecreaseHealth(result);
                    print("hit player but no teleport stuff implemented on server");
                    /*if (player.DecreaseHealth(result))
                    {
                        //After a player died we have to go through all the players again and make sure all their positios are ok
                        //first the local player and then the external players
                        TileRenderer.instance.teleportLocalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y);
                        foreach (BasePlayer moreplayers in players.Values)
                        {
                            if (moreplayers != GamePlayManagerAR.instance.localPlayer)
                            {
                                TileRenderer.instance.teleportExternalPlayer(moreplayers);
                            }
                        }

                    }*/
                }
            }
        }
    }
    //handle the Move that is SendToAll on the server itself
    void handleMovement(int id, int direction)
    {
        players[id].Move(direction);
    }

    void Update()
    {
        GetComponent<Dissonance.DissonanceComms>().IsMuted = !StaticCanvas.IsTalking;
    }

    public void Accept()
    {
        if (moveRequests.Count > 0)
        {
            MoveRequest mr = moveRequests[0];

            // broadcast.
            var moveMessage = new MoveMessage();
            moveMessage.id = mr.id;
            moveMessage.direction = mr.direction;
            NetworkServer.SendToAll(CustomNetMsg.Move, moveMessage);

            moveRequests.RemoveAt(0);
            UpdateMoveRequestUI();
            handleMovement(moveMessage.id, moveMessage.direction);
        }
    }

    public void Deny()
    {
        if (moveRequests.Count > 0)
        {
            moveRequests.RemoveAt(0);
            UpdateMoveRequestUI();
        }
    }

    void UpdateMoveRequestUI()
    {
        ActionsGuiScript actionsGui = FindObjectOfType<ActionsGuiScript>();
        actionsGui.UpdateUI(moveRequests);
    }

    void LostClient(NetworkMessage netMsg)
    {
        Debug.Log("Lost client...");

        if (players[netMsg.conn.connectionId].GetObject() != null)
        {
            Destroy(players[netMsg.conn.connectionId].GetObject());
        }

        players.Remove(netMsg.conn.connectionId);

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }

        // broadcast
        var playerDisconnectedMessage = new PlayerDisconnectedMessage();
        playerDisconnectedMessage.id = netMsg.conn.connectionId;
        NetworkServer.SendToAll(CustomNetMsg.PlayerDisconnected, playerDisconnectedMessage);
    }

    public string GetPlayerName(int id)
    {
        if (players[id] != null)
        {
            return players[id].GetName();
        }

        return "null:" + id;
    }

    string GetPlayerStats()
    {
        var rval = "";
        foreach (KeyValuePair<int, BasePlayer> entry in players)
        {
            rval += "SERVER(Player: " + entry.Key + " is a " + Roles.Instance.GetName(entry.Value.GetRoleId()) + " with name " + entry.Value.GetName() + " and ready: " + entry.Value.ready + ")\n";
        }
        return rval;
    }
}
