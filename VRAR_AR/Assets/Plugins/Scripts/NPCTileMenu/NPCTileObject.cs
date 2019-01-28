using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCTileObject : BaseTileObject
{
    bool _isEnemy;
    int _maxHealth;
    string _objectName;
    int[] values;

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

    public int getInfo()
    {
        values = new int[5];

         values[1] = _maxHealth;

        return values[1];
    }

    public string getSaveInfo()
    {
        return "";
    }
}
