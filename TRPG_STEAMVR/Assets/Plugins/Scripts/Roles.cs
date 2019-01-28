using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class holds a list of all the playable roles/classes for the ar players, and all the info related to these everything is static waddup
/// </summary>
public class Roles : MonoBehaviour{

    public List<RoleData> roleData = new List<RoleData>();
    public static Roles Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Debug.Log("Warning: multiple " + this + " in scene!"); }
    }

    /// <summary>
    /// Get character model with roleID
    /// </summary>
    public GameObject GetModel(int roleId)
    {
        return roleData[roleId].prefab;
    }

    /// <summary>
    /// Get rolename with roleid
    /// </summary>
    public string GetName(int roleId)
    {
        return roleData[roleId].roleName;
    }

    /// <summary>
    /// Get a list of all the character names
    /// </summary>
    public List<string> GetRoles()
    {
        List<string> result = new List<string>();

        foreach (RoleData role in roleData)
        {
            result.Add(role.name);
        }

        return result;
    }
}
