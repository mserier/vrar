using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticForPlayerScript : MonoBehaviour {

    public CapsuleCollider playerBody;

    // Update is called once per frame
    void Update()
    {
        //Vector3 playerPos = followHead.transform.position;
        transform.position = playerBody.transform.position + new Vector3(-0.7f,-4,9.3f); 
    }
}
