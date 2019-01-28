using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "VRAR/EnemyData")]
public class EnemyData : ScriptableObject {

    public string enemyName;
    public GameObject prefab;
    public int maxHealth;
    public string inspect;
}
