using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Object", menuName = "VRAR/ObjectData")]
public class TileObjectData : ScriptableObject {

    public string objectName;
    public Transform prefab;
    public bool interactable;
}