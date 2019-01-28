using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class that can be any character ingame, so both players and npc's (anything that can have health and move?)
/// </summary>
public class Character {

    protected int _health;
    protected int _maxHealth;    

    protected Vector2Int _currentTilePos;

    protected GameObject _object;

    public Character()
    {

    }

    //Moves the character and return true if succesful, return false if there is no tile or its not walkable
    public bool Move(int direction)
    {
        VRAR_Tile targetTile = GameStateManager.instance.getCurrentLevel().getAdjacentTile(_currentTilePos.x, _currentTilePos.y, VRAR_Level.getCounterTile(direction));

        //if (targetTile == null || !targetTile.walkable)
           // return false;

        _currentTilePos = new Vector2Int(targetTile.tileIndex_X, targetTile.tileIndex_Y);
        _object.transform.position = targetTile.tileGameObject.position + new Vector3(0, targetTile.height_, 0);
        return true;
    }

    public void DecreaseHealth(int healthAmount)
    {
        //damage particle effect stuff??
        _health -= healthAmount;
        Mathf.Clamp(_health, 0, _maxHealth);
        if (_health == 0)
        {
            //YOU DEAD SON
        }
    }

    public void IncreaseHealth(int healthAmount)
    {
        //healing particle effect stuff??
        _health += healthAmount;
        Mathf.Clamp(_health, 0, _maxHealth);
    }

    public int GetHealth()
    {
        return _health;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    public GameObject GetObject()
    {
        return _object;
    }

    public VRAR_Tile GetCurrentTile()
    {
        return GameStateManager.instance.getCurrentLevel().getTileFromIndexPos(_currentTilePos.x, _currentTilePos.y);
    }

    public Vector2Int GetCurrentVec()
    {
        return _currentTilePos;
    }
}
