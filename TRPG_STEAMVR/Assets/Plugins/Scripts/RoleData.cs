using UnityEngine;

[CreateAssetMenu(fileName = "Class", menuName = "VRAR/ClassData")]
public class RoleData : ScriptableObject {

    public string roleName;
    public GameObject prefab;
    public int maxHealth;
    public int maxEnergy;
    public int energyPerTurn;
    public int sightRadius;
}
