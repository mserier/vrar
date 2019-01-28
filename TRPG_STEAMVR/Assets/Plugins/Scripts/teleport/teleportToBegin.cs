using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class teleportToBegin : MonoBehaviour {

    public KeyCode reOrient;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(reOrient) == true)
        {
            OpenVR.System.ResetSeatedZeroPose();

            //transform.parent.localEulerAngles -= transform.localEulerAngles + transform.parent.localEulerAngles;
        }
        //transform.parent.localPosition = -transform.localPosition;

        
	}
   
  
}
