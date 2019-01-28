using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTileObject : BaseTileObject
{
    bool _isEnemy;
    int _maxHealth;


    public NPCTileObject(int object_ID) : base(object_ID)
    {

    }

    public bool isEnemy()
    {
        return _isEnemy;
    }

    public NPCTileObject setEnemy(bool enemy)
    {
        _isEnemy = enemy;
        return this;
    }

    public NPCTileObject setMaxHealth(int maxHealth)
    {
        _maxHealth = maxHealth;
        return this;
    }

    public int getMaxHealth()
    {
        return _maxHealth;
    }

}
