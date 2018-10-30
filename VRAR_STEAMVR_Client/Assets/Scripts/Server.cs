using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class Server : MonoBehaviour {

    Dictionary<int, Player> players = new Dictionary<int, Player>();

    DebugWindow dWin;

	void Start()
    {
        dWin = FindObjectOfType<DebugWindow>();

        DontDestroyOnLoad(this.gameObject);

    }

    public void Host()
    {
        NetworkServer.Listen(45555);

        NetworkServer.RegisterHandler(MsgType.Error, OnNetworkError);
        NetworkServer.RegisterHandler(MsgType.Connect, OnServerGotClient);
        NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerLostClient);

        // custom msgs
        NetworkServer.RegisterHandler(NetworkMessages.Ready, OnServerGotReady);
        NetworkServer.RegisterHandler(NetworkMessages.Name, OnServerGotName);
        NetworkServer.RegisterHandler(NetworkMessages.Class, OnServerGotClass);

        GameObject.Find("HostOrClient").GetComponent<Text>().text = "Host";
        GameObject.Find("Host").SetActive(false);
        //GameObject.Find("Join").SetActive(false);

        // local client.
        FindObjectOfType<Client>().SetupClient(true);
    }

    void OnServerGotReady(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ReadyMessage>();

        players[netMsg.conn.connectionId].ready = msg.ready;

        dWin.Text(GetPlayerStats());

        // broadcast
        var readyMessage = new ReadyMessage();
        readyMessage.id = netMsg.conn.connectionId;
        readyMessage.ready = msg.ready;
        NetworkServer.SendToAll(NetworkMessages.Ready, readyMessage);
    }

    void OnServerGotName(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NameMessage>();

        players[netMsg.conn.connectionId].name = msg.name;

        dWin.Text(GetPlayerStats());

        // broadcast
        var nameMessage = new NameMessage();
        nameMessage.id = netMsg.conn.connectionId;
        nameMessage.name = msg.name;
        NetworkServer.SendToAll(NetworkMessages.Name, nameMessage);
    }

    void OnServerGotClass(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ClassMessage>();

        players[netMsg.conn.connectionId].classType = (ClassType) msg.classType;

        dWin.Text(GetPlayerStats());

        // broadcast
        var classMessage = new ClassMessage();
        classMessage.id = netMsg.conn.connectionId;
        classMessage.classType = msg.classType;
        NetworkServer.SendToAll(NetworkMessages.Class, classMessage);
    }

    void OnServerGotClient(NetworkMessage netMsg)
    {
        dWin.Text("Server Connected to client " + netMsg.conn.connectionId);

        // send everyone else to the new player.
        foreach (KeyValuePair<int, Player> entry in players)
        {
            var playerMessage = new PlayerMessage();
            playerMessage.id = entry.Value.id;
            playerMessage.name = entry.Value.name;
            playerMessage.ready = entry.Value.ready;
            playerMessage.classType = (int)entry.Value.classType;
            netMsg.conn.Send(NetworkMessages.Player, playerMessage);
        }

        players[netMsg.conn.connectionId] = new Player();
        players[netMsg.conn.connectionId].id = netMsg.conn.connectionId;

        // send the new player to everyone else.
        var idMessage = new IdMessage();
        idMessage.id = netMsg.conn.connectionId;
        NetworkServer.SendToAll(NetworkMessages.Id, idMessage);

        dWin.Text(GetPlayerStats());
    }

    void OnServerLostClient(NetworkMessage netMsg)
    {
        dWin.Text("Server Disconnected from client " + netMsg.conn.connectionId);

        players.Remove(netMsg.conn.connectionId);

        dWin.Text(GetPlayerStats());

        var playerDisconnectedMessage = new PlayerDisconnectedMessage();
        playerDisconnectedMessage.id = netMsg.conn.connectionId;
        NetworkServer.SendToAll(NetworkMessages.PlayerDisconnected, playerDisconnectedMessage);
    }

    void OnNetworkError(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ErrorMessage>();
        Debug.Log(msg.errorCode + " " + msg.ToString());
    }
	
	void Update () {
		
	}

    string GetPlayerStats()
    {
        var rval = "";
        foreach (KeyValuePair<int, Player> entry in players)
        {
            rval += "SERVER(Player: " + entry.Key + " is a " + Enum.GetName(typeof(ClassType), entry.Value.classType) + " with name " + entry.Value.name + " and ready: " + entry.Value.ready + ")\n";
        }
        return rval;
    }
}
