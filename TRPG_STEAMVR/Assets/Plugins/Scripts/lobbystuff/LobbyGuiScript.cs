using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LobbyGuiScript : MonoBehaviour {

    public Dropdown levelDropdown;
    public Dropdown rolesDropdown;
    public Text hostOrClientField;
    public Text nameField;
    public InputField nameInputField;
    public Button hostButton;
    public Button readyButton;
    public List<GameObject> others;

    public LevelManager lvlManager;
    public Server server;
    public bool isHost = false;

    // Use this for initialization
    void Start ()
    {
        List<string> m_DropOptions = new List<string> {};
        foreach(VRAR_Level lvl in lvlManager.getVRARLevels())
        {
            m_DropOptions.Add(lvl.levelTile.ToString());
        }
        levelDropdown.ClearOptions();
        levelDropdown.AddOptions(m_DropOptions);

        m_DropOptions.Clear();
        m_DropOptions = Roles.Instance.GetRoles();
        //m_DropOptions = Enum.GetNames(typeof(Roles.RoleEnum)).ToList();
        rolesDropdown.ClearOptions();
        rolesDropdown.AddOptions(m_DropOptions);

        onLevelSelectDropdownChange();
    }

    public void Host()
    {
        if (server.Host())
        {
            isHost = true;
            hostOrClientField.text = "Host";
            hostButton.gameObject.SetActive(false);
            //joinButton.gameObject.SetActive(false);
            levelDropdown.gameObject.SetActive(true);
        }
    }


    public void ToggleReady()
    {
        // ready is available only when connected.
        if (isHost)
        {
            Text readyText = readyButton.GetComponentInChildren<Text>();

            if (readyText.text == "Ready")
            {
                readyText.text = "Unready";
            }
            else
            {
                readyText.text = "Ready";
            }

            bool ready = readyText.text == "Unready";
            
            server.UpdateReady(ready);
        }
    }


	public void UpdateUI(Dictionary<int, BasePlayer> players) {
        int i = 0;
        for (; i < others.Count; i++)
        {
            GameObject other = others[i];
            other.SetActive(true);
            other.transform.Find("Name").GetComponent<Text>().text = "";
            other.transform.Find("Role").GetComponent<Text>().text = "";
            other.transform.Find("Ready").GetComponent<Text>().text = "";
        }

        i = 0;

        foreach (KeyValuePair<int, BasePlayer> entry in players)
        {
            // 4 slots are available. 
            if (i < others.Count)
            {
                
                GameObject other = others[i];
                other.transform.Find("Name").GetComponent<Text>().text = entry.Value.GetPlayerId() + ". " + entry.Value.GetName();
                other.transform.Find("Role").GetComponent<Text>().text = Roles.Instance.GetName(entry.Value.GetRoleId());
                other.transform.Find("Ready").GetComponent<Text>().text = entry.Value.ready ? "Ready" : "Unready";
                i++;
                
            }
        }
	}

    public void onLevelSelectDropdownChange()
    {
        GameStateManager.instance.setCurrentLevel(lvlManager.getVRARLevels()[levelDropdown.value]);

        VRAR_Tile levelTile = GameStateManager.instance.getCurrentLevel().levelTile;

        TextReader tr;
        if (Application.isEditor)
        {//use the project location
            tr = new StreamReader(Application.dataPath + "/levels/" + "/level_" + levelTile.tileIndex_X + "#" + levelTile.tileIndex_Y + ".sav");
        }
        else
        {//use the user save location
            tr = new StreamReader(Application.persistentDataPath + "/levels/" + "/level_" + levelTile.tileIndex_X + "#" + levelTile.tileIndex_Y + ".sav");
        }

        server.UpdateLevel(tr.ReadToEnd());
    }
}