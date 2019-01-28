using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGuiScript : MonoBehaviour {


    public Dropdown rolesDropdown;
    public InputField ipInputField;
    public InputField nameInputField;
    public Transform _readybuttonChild;

    public Transform lobbyMenuUI;
    public Transform joinMenuUI;
    public List<PlayerUIPanelScript> playerPanels;

    public LevelManager lvlManager;
    public Client client;

    // Use this for initialization
    void Start ()
    {
        List<string> m_DropOptions = new List<string> {};
       
        m_DropOptions.Clear();
        m_DropOptions = Roles.Instance.GetRoles();
        rolesDropdown.ClearOptions();
        rolesDropdown.AddOptions(m_DropOptions);
    }
    
    public void Join()
    {
        /*
        hostOrClientField.text = "Client";
        joinButton.gameObject.SetActive(false);
        ipInputField.gameObject.SetActive(false);
        //readyButton.gameObject.SetActive(false);
        readyButtonGreen.gameObject.SetActive(true);
        nameField.gameObject.SetActive(true);
        nameInputField.gameObject.SetActive(true);
        rolesDropdown.gameObject.SetActive(true);
        playerName.gameObject.SetActive(true);
        voiceButton.gameObject.SetActive(true);
        */

        lobbyMenuUI.gameObject.SetActive(true);
        joinMenuUI.gameObject.SetActive(false);        
        client.Join();
        HiddenUIUpdate();
    }

    public void OnIPChanged()
    {
        string ip = ipInputField.text;
        client.UpdateIP(ip);
    }

    public void OnNameChanged()
    {
        string name = nameInputField.text;
        client.UpdateName(name);
    }

    public void ToggleReady()
    {
        // ready is available only when connected.
        if (client.IsConnected())
        {
            if (_readybuttonChild.gameObject.activeSelf)
            {
                _readybuttonChild.transform.gameObject.SetActive(false);
                client.UpdateReady(false);
            } else
            {
                _readybuttonChild.transform.gameObject.SetActive(true);
                client.UpdateReady(true);
            }            
        }
    }

    public void OnRoleSelected()
    {
        int value = rolesDropdown.value;

        client.UpdateRole(value);
    }
	
	public void UpdateUI(Dictionary<int, BasePlayer> players) {
        int i = 0;
        foreach (KeyValuePair<int, BasePlayer> entry in players)
        {
            if (i > playerPanels.Count)
            {
                Debug.LogWarning("There are more then 4 players, tis is not supposed to happen");
                return;
            }

            playerPanels[i].UpdatePanelInfo(entry.Value.GetName(), entry.Value.GetRoleId(), entry.Value.ready);
            playerPanels[i].gameObject.SetActive(true);
            i++;            
        }
	}

    //this does exactly the same as UpdateUI, but different
    private void HiddenUIUpdate()
    {
        int i = 0;
        foreach (BasePlayer player in client.getPlayerList())
        {
            if (i > playerPanels.Count)
            {
                Debug.LogWarning("There are more then 4 players, tis is not supposed to happen");
                return;
            }

            playerPanels[i].UpdatePanelInfo(player.GetName(), player.GetRoleId(), player.ready);
            playerPanels[i].gameObject.SetActive(true);
            i++;
        }
    }
}
