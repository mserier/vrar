using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * GameStateManager
 * This class keeps track of the game state. And will be networked
 * If you have gamelogic that relies on the gamestate feel free to add any 
 * members but make sure you add a comment stating the class name in which it is used AND in WHAT globalStateIndex it's valid
 **/
public class GameStateManager
{
    public const int STATE_DEFAULT = 0;
    public const int STATE_CAMPAIGN_EDITOR = 1;
    public const int STATE_LOBBY = 2;
    public const int STATE_PLAYING = 3;
    public const int STATE_LEVEL_EDITOR = 4;

    /********************************
     * 0: default state; For VR/AR player the main menu is shown
     * 1: campaign editor state; The VR player is in the campaign editor 
     * 2: game lobby state; The VR/AR player is in a game lobby
     * 3: game playing state; The VR/AR player is currently playing a game.
     *********************************/
    int globalStateIndex = 1;

    private VRAR_Level currentLvl = null;

    public static GameStateManager instance { get; private set; }

    public GameStateManager()
    {
        if (instance != null)
        {
            //Debug.LogError("Warning someone tried to create a new GameStateManager instance even though one already exists! (Use GameStateManager.instance instead)");
            return;
        }
        instance = this;
    }

    /** (not realy but you get the point)
     * valid states: 2,3
     * used in VRAR_Level.getAdjacentTile(int xI, int yI, int dir) 
     **/
    public string dummyMember { get; set; }

    public int getGlobalStateIndex()
    {
        return globalStateIndex;
    }

    public void setGlobalStateIndex(int index)
    {
        globalStateIndex = index;

    }


    public void setCurrentLevel(VRAR_Level lvl)
    {
        //Debug.Log("SETCURRENTLVL!! :" + lvl.levelTile);
        currentLvl = lvl;
    }

    /**
     * valid states : 3,4
     * used in Client.CheckEveryoneReady()
     * used in TileRenderer.saveLVLClick()
     **/
    public VRAR_Level getCurrentLevel()
    {
        if(!(globalStateIndex== STATE_PLAYING || globalStateIndex== STATE_LEVEL_EDITOR ||           globalStateIndex == STATE_CAMPAIGN_EDITOR))//(added state STATE_CAMPAIGN_EDITOR, please check if this is correct)
        {
            Debug.LogError("Someone requested current level while in wrong gamestate :"+globalStateIndex);
        }
        return currentLvl;
    }


}
