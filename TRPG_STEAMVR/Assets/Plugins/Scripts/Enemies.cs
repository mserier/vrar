using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds a list of all the enemies
/// </summary>
public class Enemies : MonoBehaviour{

    public List<EnemyData> enemyData = new List<EnemyData>();
    public static Enemies Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Debug.Log("Warning: multiple " + this + " in scene!"); }
    }

    /// <summary>
    /// Get enemy model with enemyId
    /// </summary>
    public GameObject GetModel(int enemyId)
    {
        return enemyData[enemyId].prefab;
    }

    /// <summary>
    /// Get enemyname with enemyId
    /// </summary>
    public string GetName(int enemyId)
    {
        return enemyData[enemyId].enemyName;
    }
}
