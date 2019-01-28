using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionsGuiScript : MonoBehaviour {

    public GameObject playerActionPanel;
    public Text playerNameField;
    public Text actionNameField;
    public Button redButton;
    public Button neutralButton;
    public Button greenButton;
    public Server server;

    public void OnRedButtonClicked()
    {
        if (server != null)
        {
            server.Deny();
        }
    }

    public void OnNeutralButtonClicked()
    {
        if (server != null)
        {
            server.Deny();
        }
    }

    public void OnGreenButtonClicked()
    {
        if (server != null)
        {
            server.Accept();
        }
    }

    public void UpdateUI(List<MoveRequest> moveRequests)
    {
        if (moveRequests.Count == 0)
        {
            playerActionPanel.SetActive(false);
        }
        else
        {
            playerActionPanel.SetActive(true);

            MoveRequest mr = moveRequests[0];
            playerNameField.text = mr.id + ". " + server.GetPlayerName(mr.id);
            actionNameField.text = "Move " + VRAR_Level.GetDirectionName(mr.direction);
            redButton.GetComponentInChildren<Text>().text = "Deny";
            neutralButton.GetComponentInChildren<Text>().text = "Ignore";
            greenButton.GetComponentInChildren<Text>().text = "Accept";
            OnGreenButtonClicked();
        }
    }
}
