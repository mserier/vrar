using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Valve.VR
{
    public class VR_Controls : MonoBehaviour
    {

        private SteamVR_TrackedObject tracked;
        //private SteamVR_TrackedController.Device device;

        WaitForSeconds controllerUpdateTime = new WaitForSeconds(5);
        static bool leftController = false;
        static bool rightController = false;
        IEnumerator logInput()
        {
            while (true)
            {
                leftController = false;
                rightController = false;
                foreach (string name in Input.GetJoystickNames())
                {
                    if (name.Contains("Left"))
                        leftController = true;
                    if (name.Contains("Right"))
                        rightController = true;
                }
                yield return controllerUpdateTime;
            }
        }

        void Start()
        {
            StartCoroutine(logInput());
            //trackedObject = GetComponent<SteamVR_TrackedObject>();
        }

        void Awake()
        {
            //trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

        void Update()
        {
            /*
            if (Controller.GetAxis() != Vector2.zero)
            {
                Debug.Log(gameObject.name + Controller.GetAxis());
            }

            // 2
            if (Controller.GetHairTriggerDown())
            {
                Debug.Log(gameObject.name + " Trigger Press");
            }

            // 3
            if (Controller.GetHairTriggerUp())
            {
                Debug.Log(gameObject.name + " Trigger Release");
            }

            // 4
            if (Controller.GetPressDown(SteamVR_Controller.ButtonMask.Grip))
            {
                Debug.Log(gameObject.name + " Grip Press");
            }

            // 5
            if (Controller.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
            {
                Debug.Log(gameObject.name + " Grip Release");
            }*/
        }

        public static bool controllerConnected()
        {
            return leftController && rightController;
        }


    }
}