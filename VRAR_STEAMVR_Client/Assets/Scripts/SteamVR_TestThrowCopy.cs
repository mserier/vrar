//======= Copyright (c) Valve Corporation, All rights reserved. ===============
using UnityEngine;
using System.Collections;

namespace Valve.VR.Extras
{
    [RequireComponent(typeof(SteamVR_TrackedObject))]
    public class SteamVR_TestThrowCopy : MonoBehaviour
    {
        public GameObject prefabC;
        public Rigidbody attachPointC;

        [SteamVR_DefaultAction("Interact")]
        public SteamVR_Action_Boolean spawnC;

        SteamVR_Behaviour_Pose trackedObjC;
        FixedJoint jointC;

        private void Awake()
        {
            trackedObjC = GetComponent<SteamVR_Behaviour_Pose>();
        }

        private void FixedUpdate()
        {
            if (jointC == null && spawnC.GetStateDown(trackedObjC.inputSource))
            {
                var go = GameObject.Instantiate(prefabC);
                go.transform.position = attachPointC.transform.position;

                jointC = go.AddComponent<FixedJoint>();
                jointC.connectedBody = attachPointC;
            }
            else if (jointC != null && spawnC.GetStateUp(trackedObjC.inputSource))
            {
                var go = jointC.gameObject;
                var rigidbody = go.GetComponent<Rigidbody>();
                Object.DestroyImmediate(jointC);
                jointC = null;
                Object.Destroy(go, 15.0f);

                // We should probably apply the offset between trackedObjC.transform.position
                // and device.transform.pos to insert into the physics sim at the correct
                // location, however, we would then want to predict ahead the visual representation
                // by the same amount we are predicting our render poses.

                var origin = trackedObjC.origin ? trackedObjC.origin : trackedObjC.transform.parent;
                if (origin != null)
                {
                    rigidbody.velocity = origin.TransformVector(trackedObjC.GetVelocity());
                    rigidbody.angularVelocity = origin.TransformVector(trackedObjC.GetAngularVelocity());
                }
                else
                {
                    rigidbody.velocity = trackedObjC.GetVelocity();
                    rigidbody.angularVelocity = trackedObjC.GetAngularVelocity();
                }

                rigidbody.maxAngularVelocity = rigidbody.angularVelocity.magnitude;
            }
        }
    }
}