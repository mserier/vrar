using UnityEngine;

public class NonPlayer : Character
{

    private int _npcID;
    private EnemyData _data;

    public NonPlayer(int npcID) : base()
    {
        _npcID = npcID;
        _data = Enemies.Instance.enemyData[_npcID];
    }

    public int GetNonPlayerID()
    {
        return _npcID;
    }

    public GameObject SpawnNPC(Vector2Int spawnLocation)
    {
        _object = GameObject.Instantiate(_data.prefab);

        _currentTilePos = spawnLocation;

        _maxHealth = _data.maxHealth;
        _health = _maxHealth;

        return _object;
    }
}
