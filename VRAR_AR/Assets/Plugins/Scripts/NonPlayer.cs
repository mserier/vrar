using UnityEngine;

public class NonPlayer : Character
{

    private int _npcID;
    private EnemyData _data;

    public NonPlayer(int npcID, Vector2Int spawn) : base()
    {
        _npcID = npcID;
        _data = Enemies.Instance.enemyData[_npcID];
        _currentTilePos = spawn;

        _maxHealth = _data.maxHealth;
        _health = _maxHealth;
    }

    public int GetNonPlayerID()
    {
        return _npcID;
    }

    public string GetName()
    {
        return _data.name;
    }

    public string GetInspect()
    {
        return _data.inspect;
    }

    public GameObject SpawnNPC(Vector2Int spawnLocation)
    {
        _object = GameObject.Instantiate(_data.prefab);

        _currentTilePos = spawnLocation;       

        return _object;
    }

    //If the npc dies return true
    public override bool DecreaseHealth(int healthAmount)
    {
        //damage particle effect stuff??
        _health -= healthAmount;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        if (_health == 0)
        {
            //Debug.Log("NPC IS DEAD AT: " + _currentTilePos.x + "   " + _currentTilePos.y);
            GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(_currentTilePos.x, _currentTilePos.y).setNPC(null);
            GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(_currentTilePos.x, _currentTilePos.y).SetWalkable(true);
            GameObject.Destroy(GetObject());
            //YOU DEAD SON
            return true;
        }
        return false;
    }

    public void DoAttackAnim()
    {
        Animator animatior = GetObject().GetComponent<Animator>();
        //do an animation here its also not great to call getcomponent every time we attack but whatever
    }
}
