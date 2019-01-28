using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class Client : MonoBehaviour {

    // key is unique id assigned by server.
    Dictionary<int, BasePlayer> players = new Dictionary<int, BasePlayer>();

    NetworkClient client;
    int serverAssignedId = -1;
    string ip = "";

    public LobbyGuiScript lobbyGui;

    Dissonance.Integrations.UNet_LLAPI.UNetCommsNetwork UNetDissonance;
    string serverDissonanceId = "";

    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void Join()
    {
        client = new NetworkClient();
        
        client.RegisterHandler(MsgType.Connect, OnConnected);
        client.RegisterHandler(MsgType.Disconnect, OnDisconnected);

        // custom msgs
        client.RegisterHandler(CustomNetMsg.Ready, GotReady);
        client.RegisterHandler(CustomNetMsg.Name, GotName);
        client.RegisterHandler(CustomNetMsg.Role, GotRole);
        client.RegisterHandler(CustomNetMsg.Id, GotId);
        client.RegisterHandler(CustomNetMsg.PlayerDisconnected, GotPlayerDisconnected);
        client.RegisterHandler(CustomNetMsg.Player, GotPlayer);
        client.RegisterHandler(CustomNetMsg.StartGame, GotStartGame);
        client.RegisterHandler(CustomNetMsg.Spawn, GotSpawnLocation);
        client.RegisterHandler(CustomNetMsg.Move, GotMove);
        client.RegisterHandler(CustomNetMsg.DissonanceId, GotDissonanceId);
        client.RegisterHandler(CustomNetMsg.Tile, GotTile);
        client.RegisterHandler(CustomNetMsg.Attack, GotAttack);

        client.Connect(ip, 45555);
    }

    public void UpdateIP(string ip)
    {
        this.ip = ip;
    }

    public void UpdateName(string name)
    {
        if (IsConnected())
        {
            // update local player.
            players[serverAssignedId].SetName(name);

            var nameMessage = new NameMessage();
            nameMessage.name = name;
            client.Send(CustomNetMsg.Name, nameMessage);
        }
    }

    public void UpdateReady(bool ready)
    {
        if (IsConnected())
        {
            // update local player.
            players[serverAssignedId].ready = ready;

            var readyMessage = new ReadyMessage();
            readyMessage.ready = ready;
            client.Send(CustomNetMsg.Ready, readyMessage);
        }
    }

    public void UpdateRole(int role)
    {
        if (IsConnected())
        {
            // update local player.
            players[serverAssignedId].SetRole(role);

            var roleMessage = new RoleMessage();
            roleMessage.role = role;
            client.Send(CustomNetMsg.Role, roleMessage);
        }
    }

    void GotPlayer(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<PlayerMessage>();
        int id = msg.id;

        if (id != serverAssignedId)
        {

            //TODO needs to be made using the constructor or else everything will fall apart
            players[id] = new BasePlayer(msg.name, msg.role, id);

            players[id].ready = msg.ready;
        }

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }
    }

    void GotReady(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ReadyMessage>();

        // variable is already updated locally.
        if (IsMyId(msg.id) == false)
        {
            players[msg.id].ready = msg.ready;            
        }

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }
    }

    void GotName(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NameMessage>();

        if (IsMyId(msg.id) == false)
        {
            players[msg.id].SetName(msg.name);
        }

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }
    }

    void GotRole(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<RoleMessage>();

        if (IsMyId(msg.id) == false)
        {
            players[msg.id].SetRole(msg.role);
        }

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }
    }

    void GotId(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<IdMessage>();

        // server must send a unique id once
        Debug.Assert(players.ContainsKey(msg.id) == false);

        players[msg.id] = new BasePlayer("Player" + msg.id.ToString(), 0, msg.id);

        // my unique id is not set so it must be mine.
        if (serverAssignedId < 0)
        {
            serverAssignedId = msg.id;

            UNetDissonance = GetComponent<Dissonance.Integrations.UNet_LLAPI.UNetCommsNetwork>();
            if (ip == "")
            {
                UNetDissonance.InitializeAsClient("127.0.0.1");
            }
            else
            {
                UNetDissonance.InitializeAsClient(ip);
            }
        }
        else if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }
    }

    void GotStartGame(NetworkMessage netMsg)
    {
        // load level
        var msg = netMsg.ReadMessage<StartGameMessage>();

        string path;
        if (Application.isEditor)
        {//use the project location
            path = (Application.dataPath + "/levels/" + "/level_" + 0 + "#" + 0 + ".sav");
        }
        else
        {//use the user save location
            path = (Application.persistentDataPath + "/levels/" + "/level_" + 0 + "#" + 0 + ".sav");
        }
        System.IO.File.WriteAllText(path, msg.level);

        
        LevelManager.Instance.discoverLevels();
        GameStateManager.getInstance().setCurrentLevel(LevelManager.Instance.getVRARLevels()[LevelManager.Instance.getVRARLevels().Count - 1]);
        
        SceneManager.LoadScene(1);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 1)
        {
            TileRenderer.instance.Init();
            //GamePlayManagerAR gamePlayManager = GameObject.Find("GameManager").GetComponent<GamePlayManagerAR>();
            GamePlayManagerAR gamePlayManager = GamePlayManagerAR.instance;

            //Roles role = gamePlayManager.GetRoles();
            Debug.Log("There are " + players.Count + "Players");
            foreach (KeyValuePair<int, BasePlayer> entry in players)
            {
                BasePlayer orig = entry.Value;

                //I removed this because it seemed very weird to make a new baseplayer and copy all its values
                //BasePlayer basePlayer = new BasePlayer(orig.name, orig.roleId, orig.spawnLocation, orig.id, gamePlayManager);

                //entry.Value.SpawnPlayer(orig.spawnLocation);

                gamePlayManager.AddPlayer(entry.Value);

                if (entry.Value.spawnLocation != new Vector2Int(int.MaxValue, int.MaxValue))
                {
                    // move the world
                    if (IsMyId(entry.Value.GetPlayerId()))
                    {
                        // tile renderer's start method hasn't been called yet when the scene is loaded.
                        StartCoroutine(WaitAndTeleport(1, entry.Value.spawnLocation));
                    }
                    // move the player
                    else
                    {
                        entry.Value.GetObject().transform.position = new Vector3(entry.Value.spawnLocation.x, entry.Value.spawnLocation.y);
                    }
                }
            }
        }
    }
    
    IEnumerator WaitAndTeleport(float waitTime, Vector2Int spawnLocation)
    {
        yield return new WaitForSeconds(waitTime);
        FindObjectOfType<TileRenderer>().teleportLocalPlayer(spawnLocation.x, spawnLocation.y);
    }
    
    void GotSpawnLocation(NetworkMessage netMsg)
    {
        

        var msg = netMsg.ReadMessage<SpawnMessage>();

        players[msg.id].spawnLocation = new Vector2Int((int) msg.spawnLocation.x, (int) msg.spawnLocation.y);
        //players[msg.id].tileLocation = players[msg.id].spawnLocation;

        //Debug.Log("Got spawn message with id: " + msg.id + " my id: " + serverAssignedId + " tileX " + msg.spawnLocation.x + " tileY " + msg.spawnLocation.y);

        if (IsMyId(msg.id))
        {
            //GamePlayManagerAR.instance.AddPlayer(players[msg.id]);
            GamePlayManagerAR.instance.setLocalPlayer(players[msg.id]);
            TileRenderer.instance.localPlayer = players[msg.id];

            //TileRenderer.instance.spawnLevel();

            players[msg.id].SpawnPlayer(players[msg.id].spawnLocation);
            //Debug.Log("SPAWNING AT" + players[msg.id].spawnLocation);

            TileRenderer.instance.teleportLocalPlayer((int)msg.spawnLocation.x, (int)msg.spawnLocation.y);
        }
        else
        {
            players[msg.id].SpawnPlayer(players[msg.id].spawnLocation);
            StartCoroutine(WaitAndTeleportExternal(1, players[msg.id]));
            //TileRenderer.instance.teleportExternalPlayer(players[msg.id]);
            //players[msg.id].GetObject().transform.position = msg.spawnLocation;
        }

        //// move the player (if they already exist).
        //else if (players[msg.id].GetObject() != null)
        //{
        //    // change from tile pos to world pos.
            
        //}
    }

    IEnumerator WaitAndTeleportExternal(float waitTime, BasePlayer player)
    {
        yield return new WaitForSeconds(waitTime);
        TileRenderer.instance.teleportExternalPlayer(player);
        //FindObjectOfType<TileRenderer>().teleportLocalPlayer(spawnLocation.x, spawnLocation.y);
    }

    void GotPlayerDisconnected(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<PlayerDisconnectedMessage>();

        if (players[msg.id].GetObject() != null)
        {
            Destroy(players[msg.id].GetObject());
        }

        players.Remove(msg.id);

        if (lobbyGui != null)
        {
            lobbyGui.UpdateUI(players);
        }
    }

    void Update()
    {
    }

    void GotMove(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<MoveMessage>();

        /*
        BasePlayer player = players[msg.id];
        VRAR_Tile tile = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(player.tileLocation.x, player.tileLocation.y, VRAR_Level.getCounterTile(msg.direction));
        player.tileLocation = new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y);
        */



        BasePlayer player = players[msg.id];

        VRAR_Tile tile = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(player.GetCurrentVec().x, player.GetCurrentVec().y, VRAR_Level.getCounterTile(msg.direction));

        player.Move(msg.direction);

        // local player
        if (IsMyId(msg.id))
        {
            // move the world.
            TileRenderer.instance.walkLocalPlayer(player.GetCurrentVec().x, player.GetCurrentVec().y, msg.direction);
        }
        else
        {
            //TileRenderer.instance.walkExternalPlayer(player.GetCurrentVec().x, player.GetCurrentVec().y, player, msg.direction);
            TileRenderer.instance.walkExternalPlayer(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, player, msg.direction, true);
            // move the other player.
            //otherplayer.tilePosition =  
        }   
    }

    void GotAttack(NetworkMessage netMsg)
    {
        AttackMessage attackMessage = netMsg.ReadMessage<AttackMessage>();
        VRAR_Tile tile = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(attackMessage.targetTileX, attackMessage.targetTileY);
        NonPlayer npc = tile.getNPC();
        if (npc != null)
        {
            //CHeck if we did the attack and show the hitsplat if true
            if (IsMyId(attackMessage.id))
            {
                UIManager.Instance.SpawnHitSplat(tile.getNPC().GetObject().transform, attackMessage.result);
            }
            npc.DecreaseHealth(attackMessage.result);
            players[attackMessage.id].DoAttackAnim();
            
            
        } else //if there is no npc check if there is a player
        {
            foreach (BasePlayer player in players.Values)
            {                
                if (player.GetCurrentTile() == tile)
                {
                    //CHeck if we did the attack and show the hitsplat if true
                    if (IsMyId(attackMessage.id))
                    {
                        UIManager.Instance.SpawnHitSplat(player.GetObject().transform, attackMessage.result);
                    }
                    //the player died ! move them to their spawn location
                    if (player.DecreaseHealth(attackMessage.result))
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
                        
                    }
                    players[attackMessage.id].DoAttackAnim();
                }
            }
        }
    }

    void GotDissonanceId(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<DissonanceIdMessage>();

        // server
        if (msg.id == 0)
        {
            serverDissonanceId = msg.dissonanceId;
            
            // the only person that will receive the voice of the client is the game master.
            GetComponent<Dissonance.VoiceBroadcastTrigger>().PlayerId = serverDissonanceId;
        }
    }

    public List<BasePlayer> getPlayerList()
    {
        List<BasePlayer> playerList = new List<BasePlayer>();
        foreach (BasePlayer player in players.Values)
        {
            playerList.Add(player);
        }

        return playerList;
    }

    void GotTile(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<TileMessage>();
        if(msg.removing)
        {
            if (GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(msg.tileIndex_X, msg.tileIndex_Y).tileGameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(msg.tileIndex_X, msg.tileIndex_Y).tileGameObject.gameObject);
            }
            GameStateManager.getInstance().getCurrentLevel().removeTile(msg.tileIndex_X, msg.tileIndex_Y);
        }
        else
        {

            VRAR_Tile tile = new VRAR_Tile(msg.tileIndex_X, msg.tileIndex_Y);
            tile.setHeight(msg.height_);

            tile.setInteractableObject(TileObjectManger.TILE_OBJECTS[msg.interactableObjectId]);

            List<BaseTileObject> dumbObjectsList = new List<BaseTileObject>();
            for (int i = 0; i < msg.dumbObjectIds.Length; i++)
            {
                //print("adding object :" + msg.dumbObjectIds[i] + "to new tile");
                dumbObjectsList.Add(TileObjectManger.TILE_OBJECTS[msg.dumbObjectIds[i]]);
                tile.SetWalkable(false);
            }

            tile.dumbObjectsList = dumbObjectsList;

            tile.locationList = new Dictionary<int, Vector3>();
            tile.scaleList = new Dictionary<int, Vector3>();
            tile.rotationList = new Dictionary<int, Quaternion>();
            for (int i = 0; i < msg.locationKeys.Length; i++)
            {
                tile.locationList.Add(msg.locationKeys[i], msg.locationValues[i]);
            }
            for (int i = 0; i < msg.scaleKeys.Length; i++)
            {
                tile.scaleList.Add(msg.scaleKeys[i], msg.scaleValues[i]);
            }
            for (int i = 0; i < msg.rotationKeys.Length; i++)
            {
                tile.rotationList.Add(msg.rotationKeys[i], msg.rotationValues[i]);
            }

            tile.terrain = msg.terrain;
            if(tile.terrain=="Water")
            {
                tile.SetWalkable(false);
            }
            tile.walkable = msg.walkable;

            if (GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(msg.tileIndex_X, msg.tileIndex_Y) != null)
            {
                if (GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(msg.tileIndex_X, msg.tileIndex_Y).tileGameObject != null)
                {
                    UnityEngine.Object.DestroyImmediate(GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(msg.tileIndex_X, msg.tileIndex_Y).tileGameObject.gameObject);
                }
                GameStateManager.getInstance().getCurrentLevel().removeTile(tile.tileIndex_X, tile.tileIndex_Y);
            }
            GameStateManager.getInstance().getCurrentLevel().addNewTile(tile);

            List<VRAR_Tile> list = GameStateManager.getInstance().getCurrentLevel().selectRadius(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y, GamePlayManagerAR.instance.localPlayer.GetSight());
            if (list.Contains(tile))
            {
                TileRenderer.instance.updateTile(tile);
                StartCoroutine(TileRenderer.instance.DropIn(tile, 1f, null));
            }

        }

        /*
        string debugStr = "Tile indexX: " + msg.tileIndex_X + " indexY: " + msg.tileIndex_Y;
        debugStr += " height: " + msg.height_ + " isPhantom: " + msg.isPhantom;
        debugStr += " interactableObjectId " + msg.interactableObjectId + " locations: [";

        foreach (KeyValuePair<int, Vector3> entry in tile.locationList)
        {
            debugStr += entry.Key + ":" + entry.Value + "|";
        }

        debugStr += "] dumbObjectIds.Length: " + msg.dumbObjectIds.Length;
        Debug.Log(debugStr);*/
    }

    void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected with server.");
    }

    void OnDisconnected(NetworkMessage netMsg)
    {
        serverAssignedId = -1;
        Debug.Log("Disconnected from server.");
    }
    
    public bool IsConnected()
    {
        return serverAssignedId > 0;
    }

    public bool IsMyId(int id)
    {
        return serverAssignedId == id;
    }

    string GetPlayerStats()
    {
        var rval = "";
        foreach (KeyValuePair<int, BasePlayer> entry in players)
        {
            rval += "CLIENT:" + serverAssignedId + "(Player: " + entry.Key + " is a " + Roles.Instance.GetName(entry.Value.GetRoleId()) + " with name " + entry.Value.GetName() + " and ready: " + entry.Value.ready + ")\n";
        }
        return rval;
    }

    public void QueueMove(int direction)
    {        
        BasePlayer player = GamePlayManagerAR.instance.localPlayer;
        VRAR_Tile tile = GameStateManager.getInstance().getCurrentLevel().getAdjacentTile(player.GetCurrentVec().x, player.GetCurrentVec().y, VRAR_Level.getCounterTile(direction));

        if (tile == null || !tile.walkable)
            return;

        var moveMessage = new MoveRequestMessage();
        moveMessage.direction = direction;
        client.Send(CustomNetMsg.MoveRequest, moveMessage);
    }

    public void QueueAttack(int attackId, Vector2Int targetTile)
    {
        AttackRequestMessage attackMessage = new AttackRequestMessage();
        attackMessage.attackId = attackId;
        attackMessage.targetTileX = targetTile.x;
        attackMessage.targetTileY = targetTile.y;
        client.Send(CustomNetMsg.AttackRequest, attackMessage);
    }
    public void tempQueueMove()
    {
        var moveMessage = new MoveRequestMessage();
        // moveMessage.movement = new Vector3(UnityEngine.Random.value * 3, 0, UnityEngine.Random.value * 3);
        client.Send(CustomNetMsg.MoveRequest, moveMessage);
    }
}
