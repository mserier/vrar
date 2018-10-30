using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGuiScript : MonoBehaviour {

    public Dropdown dropdownselecthostlevel;
    public LevelManager lvlManager;

    // Use this for initialization
    void Start ()
    {
        List<string> m_DropOptions = new List<string> {};
        foreach(VRAR_Level lvl in lvlManager.getVRARLevels())
        {
            m_DropOptions.Add(lvl.levelTile.ToString());
        }

        dropdownselecthostlevel.ClearOptions();
        dropdownselecthostlevel.AddOptions(m_DropOptions);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onLevelSelectDropdownChange()
    {

        Debug.Log(lvlManager.getVRARLevels()[dropdownselecthostlevel.value].levelTile.tileIndex_X + " " + lvlManager.getVRARLevels()[dropdownselecthostlevel.value].levelTile.tileIndex_Y);
        Debug.Log("val :" + dropdownselecthostlevel.value);
        GameStateManager.instance.setCurrentLevel(lvlManager.getVRARLevels()[dropdownselecthostlevel.value]);
    }
}
