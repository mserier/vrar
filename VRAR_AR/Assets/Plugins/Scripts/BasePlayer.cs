using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The player. Could be any class from a thief to the dungeon master himself. 
/// 
/// </summary>
public class BasePlayer : Character  {

    
    private RoleData _role;
    private int _roleId;

    private string _playerName;
    private int _playerId;

    private int _energy;
    private int _maxEnergy;
    private int _energyPerTurn;

    
    private int _sight;

    private bool _turn = true;
    public bool ready = false;

    public Vector2Int spawnLocation = new Vector2Int(int.MaxValue, int.MaxValue);
    private Animator _animator;

    public Stack<IEnumerator> translations = new Stack<IEnumerator>(5);
    public bool translating=false;

    /// <summary>
    /// Create the player
    /// <param name="playerName"></param> Name of the player
    /// </summary>
    public BasePlayer(string playerName, int roleInt, int playerId) : base()
    {
        _playerId = playerId;
        _playerName = playerName;
        _role = Roles.Instance.roleData[roleInt];          
    }

    /// <summary>
    /// Create the playerobject and set its stats
    /// <param name="playerName"></param> Name of the player
    /// </summary>
    public GameObject SpawnPlayer(Vector2Int spawnLocation)
    {
        _object = GameObject.Instantiate(_role.prefab);
        _object.transform.localScale *= LevelManager.TILE_SCALE;
        _animator = _object.GetComponent<Animator>();

        //Debug.Log("GETTING CURRENT LEVEL" + GameStateManager.getInstance().getCurrentLevel());

        _currentTilePos = spawnLocation;

        _maxHealth = _role.maxHealth;
        _health = _maxHealth;

        _maxEnergy = _role.maxEnergy;
        _energy = _maxEnergy;
        _energyPerTurn = _role.energyPerTurn;

        _sight = _role.sightRadius;

        //If we are the localplayer update the healthbar
        if (GamePlayManagerAR.instance.localPlayer == this)
        {
            UIManager.Instance.UpdateHealthBar(_health, _maxHealth);
        }
        return _object;
    }

    public override bool DecreaseHealth(int healthAmount)
    {
        //damage particle effect stuff??
        _health -= healthAmount;
        _health = Mathf.Clamp(_health, 0, _maxHealth);
        //If we are the localplayer update the healthbar
        if (GamePlayManagerAR.instance.localPlayer == this)
        {
            UIManager.Instance.UpdateHealthBar(_health, _maxHealth);
        }
        if (_health == 0)
        {
            _health = _maxHealth;
            _currentTilePos = spawnLocation;
            //YOU DEAD SON W
            if (GamePlayManagerAR.instance.localPlayer == this)
            {
                UIManager.Instance.UpdateHealthBar(_health, _maxHealth);
            }
            return true;
        }      

        return false;
    }

    public void DoAttackAnim()
    {
        Debug.Log("ATTING");
        //do an animation here
        _animator.SetTrigger("Attack");
    }

    public int GetPlayerId()
    {
        return _playerId;
    }

    public string GetName()
    {
        return _playerName;
    }

    public void SetName(string name)
    {
        _playerName = name;
    }

    public int GetRoleId()
    {
        return _roleId;
    }

    public void SetRole(int roleInt)
    {
        _roleId = roleInt;
        _role = Roles.Instance.roleData[roleInt];
    }    

    public int GetEnergy()
    {
        return _energy;
    }

    public int GetMaxEnergy()
    {
        return _maxEnergy;
    }

    public void DecreaseEnergy(int energyAmount)
    {
        _energy -= energyAmount;
        Mathf.Clamp(_energy, 0, _maxEnergy);
    }

    public void IncreaseEnergy(int energyAmount)
    {
        //energy particle effect stuff??
        _energy += energyAmount;
        Mathf.Clamp(_energy, 0, _maxEnergy);
    }

    public int GetSight()
    {
        if (_sight == 0)
        {
            Debug.LogWarning("Forcing max sight!");
            return 8;
        }
        return _sight;
    }   
   
    /// <summary>
    /// Give a turn to the player. Will tell the gameplay manager to update the panel.
    /// </summary>
    public void GiveTurn()
    {
        _turn = true;
    }

    /// <summary>
    /// Take the turn of a player. Will tell the gameplay manager to update the panel.
    /// </summary>
    public void TakeTurn()
    {
        _turn = false;
    }
    public bool HasTurn()
    {
        return _turn;
    }
}
