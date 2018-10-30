using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ClassType
{
    Mage,
    Warrior,
    Paladin
}

public class Player
{
    public int id;
    public string name;
    public bool ready = false;
    public ClassType classType = ClassType.Mage;
}

public class Client : MonoBehaviour {

    Dictionary<int, Player> players = new Dictionary<int, Player>();

    NetworkClient client;
    int serverAssignedId = -1;

    DebugWindow dWin;

    // Use this for initialization
    void Start () {
        Dropdown classDropdown = GameObject.Find("ClassDropdown").GetComponent<Dropdown>();
        classDropdown.ClearOptions();
        List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
        string[] classNames = Enum.GetNames(typeof(ClassType));
        foreach (string className in classNames)
        {
            options.Add(new Dropdown.OptionData(className));
        }
        classDropdown.options = options;

        dWin = FindObjectOfType<DebugWindow>();
    }

    public void SetupClient(bool fromServer = false)
    {
        client = new NetworkClient();

        client.RegisterHandler(MsgType.Error, OnNetworkError);
        client.RegisterHandler(MsgType.Connect, OnConnected);
        client.RegisterHandler(MsgType.Disconnect, OnDisconnected);

        // custom msgs
        client.RegisterHandler(NetworkMessages.Ready, OnGotReady);
        client.RegisterHandler(NetworkMessages.Name, OnGotName);
        client.RegisterHandler(NetworkMessages.Class, OnGotClass);
        client.RegisterHandler(NetworkMessages.Id, OnGotId);
        client.RegisterHandler(NetworkMessages.PlayerDisconnected, GotPlayerDisconnected);
        client.RegisterHandler(NetworkMessages.Player, GotPlayer);
        client.RegisterHandler(NetworkMessages.File, GotFile);

        client.Connect("127.0.0.1", 45555);

        if (!fromServer)
        {
            GameObject.Find("HostOrClient").GetComponent<Text>().text = "Client";
            GameObject.Find("Host").SetActive(false);
            GameObject.Find("Join").SetActive(false);
        }
    }

    void GotFile(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<FileMessage>();
        dWin.Text("Got file: " + msg.file);
    }

    // get all the other connected players upon connecting myself.
    void GotPlayer(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<PlayerMessage>();
        int id = msg.id;

        if (id != serverAssignedId)
        {
            players[id] = new Player();
            players[id].id = id;
            players[id].name = msg.name;
            players[id].ready = msg.ready;
            players[id].classType = (ClassType)msg.classType;

            UpdateUI();
        }
    }

    void OnGotReady(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ReadyMessage>();

        if (msg.id != serverAssignedId) // already updated myself
        {
            players[msg.id].ready = msg.ready;

            UpdateUI();
        }

        CheckEveryoneReady();
    }

    bool CheckEveryoneReady()
    {
        foreach (KeyValuePair<int, Player> entry in players)
        {
            if (entry.Value.ready == false)
            {
                return false;
            }
        }

        GameStateManager.instance.setGlobalStateIndex(GameStateManager.STATE_PLAYING);
        VRAR_Level levelTile = GameStateManager.instance.getCurrentLevel();
        var fileMessage = new FileMessage();
        TextReader tr;
        if (Application.isEditor)
        {//use the project location
            tr = new StreamReader(Application.dataPath + "/levels/" + "/level_" + levelTile.levelTile.tileIndex_X + "#" + levelTile.levelTile.tileIndex_Y + ".sav");
        }
        else
        {//use the user save location
            tr = new StreamReader(Application.persistentDataPath + "/levels/" + "/level_" + levelTile.levelTile.tileIndex_X + "#" + levelTile.levelTile.tileIndex_Y + ".sav");
        }
        fileMessage.file = tr.ReadToEnd();
        NetworkServer.SendToAll(NetworkMessages.File, fileMessage);

        SceneManager.LoadScene(1);


        return true;
    }

    void OnGotName(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<NameMessage>();

        if (msg.id != serverAssignedId)
        {
            players[msg.id].name = msg.name;

            UpdateUI();
        }
    }

    void OnGotClass(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ClassMessage>();

        if (msg.id != serverAssignedId)
        {
            players[msg.id].classType = (ClassType)msg.classType;

            UpdateUI();
        }
    }

    void OnGotId(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<IdMessage>();

        // server must send a unique id once
        Debug.Assert(players.ContainsKey(msg.id) == false);

        players[msg.id] = new Player();
        players[msg.id].id = msg.id;

        // my unique id is not set so it must be mine.
        if (serverAssignedId < 0)
        {
            serverAssignedId = msg.id;
            dWin.Text("Setting my unique id to: " + serverAssignedId);

            // the name and class type might have been changed before connecting/receiving id, so send that.
            string name = GameObject.Find("NameInput").GetComponent<InputField>().text;
            SendNameMessage(name);
            int classType = GameObject.Find("ClassDropdown").GetComponent<Dropdown>().value;
            SendClassMessage(classType);

            // and set them to my local player stats.
            players[serverAssignedId].name = name;
            players[serverAssignedId].classType = (ClassType) classType;
        }
        else
        {
            UpdateUI();
        }
    }

    void GotPlayerDisconnected(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<PlayerDisconnectedMessage>();

        players.Remove(msg.id);

        UpdateUI();
    }

    public void OnNameChanged()
    {
        string name = GameObject.Find("NameInput").GetComponent<InputField>().text;
        GameObject.Find("Name").GetComponent<Text>().text = name;

        if (serverAssignedId > 0)
        {
            players[serverAssignedId].name = name;

            SendNameMessage(name);
        }
    }

    void SendNameMessage(string name)
    {
        var nameMessage = new NameMessage();
        nameMessage.name = name;
        client.Send(NetworkMessages.Name, nameMessage);
    }

    public void ToggleReady()
    {
        if (serverAssignedId > 0)
        {
            Text readyText = GameObject.Find("Ready").GetComponentInChildren<Text>();
            if (readyText.text == "Ready")
            {
                readyText.text = "Unready";
            }
            else
            {
                readyText.text = "Ready";
            }

            bool ready = readyText.text == "Unready";

            // update my player
            players[serverAssignedId].ready = ready;

            var readyMessage = new ReadyMessage();
            readyMessage.ready = ready;
            client.Send(NetworkMessages.Ready, readyMessage);
        }
    }

    public void OnClassSelected()
    {
        int value = GameObject.Find("ClassDropdown").GetComponent<Dropdown>().value;
        // dWin.Text("Hello " + Enum.GetName(typeof(ClassType), value));

        if (serverAssignedId > 0)
        {
            // update my player
            players[serverAssignedId].classType = (ClassType) value;

            SendClassMessage(value);
        }
    }

    void SendClassMessage(int classType)
    {
        var classMessage = new ClassMessage();
        classMessage.classType = classType;
        client.Send(NetworkMessages.Class, classMessage);
    }

    void OnConnected(NetworkMessage netMsg)
    {
        dWin.Text("Client Connected");
    }

    void OnDisconnected(NetworkMessage netMsg)
    {
        dWin.Text("Client Disconnected");
        GameObject.Find("HostOrClient").GetComponent<Text>().text = "Disconnected";
    }

    void OnNetworkError(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ErrorMessage>();
        dWin.Text(msg.errorCode + " " + msg.ToString());
    }

    void UpdateUI()
    {
        int i = 0;
        for (; i < 9; i++)
        {
            GameObject other = GameObject.Find("Other (" + i + ")");
            other.transform.Find("NameOther").GetComponent<Text>().text = "";
            other.transform.Find("ClassOther").GetComponent<Text>().text = "";
            other.transform.Find("Text").GetComponent<Text>().text = "";
        }

        i = 0;

        foreach (KeyValuePair<int, Player> entry in players)
        {
            if (entry.Value.id != serverAssignedId)
            {
                GameObject other = GameObject.Find("Other (" + i + ")");
                other.transform.Find("NameOther").GetComponent<Text>().text = entry.Value.id + ". " + entry.Value.name;
                other.transform.Find("ClassOther").GetComponent<Text>().text = Enum.GetName(typeof(ClassType), entry.Value.classType);
                other.transform.Find("Text").GetComponent<Text>().text = entry.Value.ready ? "Ready" : "Unready";

                i++;
            }
        }
    }

    void Update()
    {

    }

    string GetPlayerStats()
    {
        var rval = "";
        foreach (KeyValuePair<int, Player> entry in players)
        {
            rval += "CLIENT:" + serverAssignedId + "(Player: " + entry.Key + " is a " + Enum.GetName(typeof(ClassType), entry.Value.classType) + " with name " + entry.Value.name + " and ready: " + entry.Value.ready + ")\n";
        }
        return rval;
    }
}
