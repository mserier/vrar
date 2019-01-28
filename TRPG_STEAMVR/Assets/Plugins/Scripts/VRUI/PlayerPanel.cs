using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Used for updating the belonging panel of a player that shows the vrplayer the health, energy and if the player has ended his turn.
/// </summary>
public class PlayerPanel : MonoBehaviour {

    private Text playerNameText;
    private Text playerHealthText;
    private Text playerEnergyText;
    private Text playerTurnText;


    /// <summary>
    /// Used for loading in all the starting valus and getting the components.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="maxHealth"></param>
    /// <param name="maxEnergy"></param>
    public void PlayerPanelConstruct(string name, float maxHealth, float maxEnergy)
    {
        this.name = name;
        playerNameText = transform.Find("PlayerNameText").GetComponent<Text>();
        playerEnergyText = transform.Find("PlayerEnergyText").GetComponent<Text>();
        playerHealthText = transform.Find("PlayerHealthText").GetComponent<Text>();
        playerTurnText = transform.Find("PlayerTurnText").GetComponent<Text>();
        playerNameText.text = "Player: " + name;
        playerHealthText.text = "Health: " + maxHealth;
        playerEnergyText.text = "Energy: " + maxEnergy;
    }
    

    public void UpdateHealth(float amount)
    {
        playerHealthText.text = amount.ToString();
    }

    public void UpdateEnergy(float amount)
    {
        playerEnergyText.text = amount.ToString();
    }

    public void UpdateTurn(bool turn)
    {
        if (turn)
        {
            playerTurnText.text = "Turn";
        }
        else
        {
            playerTurnText.text = "Done";
        }
         
    }


    // Use this for initialization
    void Start () {
    }

    
    public void SetName(string name)
    {
        playerNameText.text = name;
    }
   
	
	// Update is called once per frame
	void Update () {
		
	}
}
