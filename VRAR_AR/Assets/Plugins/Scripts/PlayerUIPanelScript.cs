using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIPanelScript : MonoBehaviour {

    public Text health;
    public Text energy;
    public Text energyPerTurn;
    public Text sightRange;
    public Image roleIcon;
    public Text roleName;
    public Text playerName;
    public Transform readyIcon;
    public Transform notReadIcon;

    public void UpdatePanelInfo(string name, int roleID, bool ready)
    {
        health.text = Roles.Instance.roleData[roleID].maxHealth.ToString();
        energy.text = Roles.Instance.roleData[roleID].maxEnergy.ToString();
        energyPerTurn.text = Roles.Instance.roleData[roleID].energyPerTurn.ToString();
        sightRange.text = Roles.Instance.roleData[roleID].sightRadius.ToString();
        roleIcon.sprite = Roles.Instance.roleData[roleID].roleIcon;
        roleName.text = Roles.Instance.roleData[roleID].roleName;
        playerName.text = name;

        if (ready)
        {
            readyIcon.gameObject.SetActive(true);
            notReadIcon.gameObject.SetActive(false);
        } else
        {
            readyIcon.gameObject.SetActive(false);
            notReadIcon.gameObject.SetActive(true);
        }            
    }
}
