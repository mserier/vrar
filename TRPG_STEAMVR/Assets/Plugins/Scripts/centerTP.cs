using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class centerTP : MonoBehaviour {


    public Transform followHead;

	// Update is called once per frame
	void Update () {
        Vector3 playerPos = followHead.transform.position;

        //Debug.Log(teleportPlane.transform.position);
        
        transform.position = new Vector3(playerPos.x+-2f, playerPos.y + -8f, playerPos.z-2f);
        //teleportPlane.transform.position = new Vector3(playerPos.x + -2f, teleportPlane.transform.position.y, playerPos.z - 2f);


    }
}
