using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class VRInputHandler : MonoBehaviour
    {
        [SteamVR_DefaultAction("Teleport", "default")]
        public SteamVR_Action_Boolean teleportAction;

        public StaticForPlayerScript staticForPlayerScript;
        private GameObject selectedObject;
        private VRAR_Tile vRAR_Tile;

        public SteamVR_Action_Boolean teleportUp;
        public SteamVR_Action_Boolean teleportDown;

        public SteamVR_Action_Single squeezeLeftAction;
        public SteamVR_Action_Single squeezeRightAction;

        public SteamVR_Action_Vector2 trackpadLeft;
        public SteamVR_Action_Vector2 trackpadRight;


        public SteamVR_Action_Boolean touchingTrackpadLeft;
        public SteamVR_Action_Boolean touchingTrackpadRight;


        public SteamVR_Action_Boolean menuRight;
        public SteamVR_Action_Boolean menuLeft;


        public SteamVR_Action_Boolean dpadNorthLeft;
        public SteamVR_Action_Boolean dpadEastLeft;
        public SteamVR_Action_Boolean dpadSouthLeft;
        public SteamVR_Action_Boolean dpadWestLeft;

        public SteamVR_Action_Boolean dpadNorthRight;
        public SteamVR_Action_Boolean dpadEastRight;
        public SteamVR_Action_Boolean dpadSouthRight;
        public SteamVR_Action_Boolean dpadWestRight;

        /*
        if (hand.handType == SteamVR_Input_Sources.RightHand)
        {
            scrollPos += scrollRight.GetAxis(hand.handType).y;
        } 
         */
        public SteamVR_Action_Vector3 scrollLeft;
        public SteamVR_Action_Vector3 scrollRight;

        float teleportFadeTime = 0.2f;
        float currentFadeTime = 0f;

        public TileInteractor tileInteractor;

        public UIRayCastScript uiRayCastScript;
        
        private Hand pointerHand = null;
        private Player player = null;

        private bool visible = false;

        public static bool isInTileObjectEditMenu { get; private set; }
        private static bool released;
        public static void setIntTileObjectEditMenu(bool isIn)
        {
            if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
            {
                
                if (isIn)
                {
                    isInTileObjectEditMenu = true;
                    released = false;
                }
                else
                {
                    isInTileObjectEditMenu = false;
                    released = false;
                }
            }
        }

        public const float MAX_SCALE = 2f;

        // Events

        public static SteamVR_Events.Event<float> ChangeScene = new SteamVR_Events.Event<float>();
        public static SteamVR_Events.Action<float> ChangeSceneAction(UnityAction<float> action) { return new SteamVR_Events.Action<float>(ChangeScene, action); }

        public static SteamVR_Events.Event<TeleportMarkerBase> Player = new SteamVR_Events.Event<TeleportMarkerBase>();
        public static SteamVR_Events.Action<TeleportMarkerBase> PlayerAction(UnityAction<TeleportMarkerBase> action) { return new SteamVR_Events.Action<TeleportMarkerBase>(Player, action); }

        public static SteamVR_Events.Event<TeleportMarkerBase> PlayerPre = new SteamVR_Events.Event<TeleportMarkerBase>();
        public static SteamVR_Events.Action<TeleportMarkerBase> PlayerPreAction(UnityAction<TeleportMarkerBase> action) { return new SteamVR_Events.Action<TeleportMarkerBase>(PlayerPre, action); }

        //-------------------------------------------------
        private static VRInputHandler _instance;
        public static VRInputHandler instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<VRInputHandler>();
                }

                return _instance;
            }
        }


        //-------------------------------------------------
        void Awake()
        {
            _instance = this;

        }


        public void setSelectedObject(VRAR_Tile vRAR_Tile, GameObject select)
        {
            selectedObject = select;
            this.vRAR_Tile = vRAR_Tile;

        }

        //-------------------------------------------------
        void Start()
        {
            player = InteractionSystem.Player.instance;

            if (player == null)
            {
                Debug.LogError("Teleport: No Player instance found in map.");
                Destroy(this.gameObject);
                return;
            }
            
        }


        //-------------------------------------------------
        void OnEnable()
        {
        }


        //-------------------------------------------------
        void OnDisable()
        {
        }


        //-------------------------------------------------
        private void CheckForSpawnPoint()
        {
        }


        //-------------------------------------------------
        public void HideTeleportPointer()
        {
            if (pointerHand != null)
            {
                //HidePointer();
            }
        }
        private float scaleRot = 0f;
        private bool startMovingMap = false;
        private bool startMovingTileObject = false;
        private bool startScaling = false;
        private Vector3 lastHandPosition;

        //-------------------------------------------------
        void Update()
        {
            Hand oldPointerHand = pointerHand;
            Hand newPointerHand = null;

            foreach (Hand hand in player.hands)
            {

                if (WasTeleportButtonPressed(hand))
                {
                    newPointerHand = hand;
                }
                if (squeezeLeftAction.GetAxis(hand.handType) > 0.9 && hand.handType == SteamVR_Input_Sources.LeftHand && !isInTileObjectEditMenu)
                {
                    if(!startMovingMap)
                    {
                        startMovingMap = true;
                        lastHandPosition = hand.transform.position;
                        //staticForPlayerScript.gameObject.SetActive(false);

                    }

                    Vector3 handPositionDelta = lastHandPosition - hand.transform.position;
                    //print("delta :" + handPositionDelta);
                    player.transform.position += handPositionDelta *15f;
                    staticForPlayerScript.transform.position = player.transform.position + new Vector3(-0.7f, -4, 9.3f);

                    lastHandPosition = hand.transform.position;

                }
                else if (hand.handType == SteamVR_Input_Sources.LeftHand)
                {
                    startMovingMap = false;
                    //staticForPlayerScript.gameObject.SetActive(true);

                }



                if (hand.handType == SteamVR_Input_Sources.RightHand &&
                    (GameStateManager.instance.getGlobalStateIndex()==GameStateManager.STATE_LEVEL_EDITOR 
                    || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR
                    || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING))
                {
                    
                    if (squeezeRightAction.GetAxis(hand.handType)>0.5 )
                    {
                       
                        if(released)
                        {
                            if(HandFingerScript.instance.inLeftCanvas||HandFingerScript.instance.inRightCanvas)
                            {
                                released = false;
                            }
                            uiRayCastScript.clicked = true;
                        }
                    }
                    else
                    {
                        
                        released = true;
                    }
                    if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
                    {
                        //updateBrushSize(hand);
                        //updateBrushType(hand);
                    }
                    //Debug.Log(brushSize + "      " + brushType);

                    //Debug.Log(isTouchingTrackpad(hand));
                    //tileInteractor.setBrushSize()
                }

                if (teleportDown.GetStateDown(hand.handType))
                {
                    player.transform.Translate(0, -2, 0);
                    //fadeThing();
                }
                if (teleportUp.GetStateDown(hand.handType))
                {
                    player.transform.Translate(0, 2, 0);
                    //fadeThing();
                }
                if(menuRight.GetStateDown(hand.handType))
                {
                    TileInteractor.instance.TileToolEditTick = true;
                }

                if(dpadWestLeft.GetLastStateDown(hand.handType) && hand.handType == SteamVR_Input_Sources.LeftHand)
                {
                    if (!HandFingerScript.instance.leftHandMenuOpen)
                    {
                        
                        HandFingerScript.instance.openMenu(true);
                    }
                    else
                    {
                        HandFingerScript.instance.closeMenus();
                    }
                }


                moveTileObject(hand);



            }

        }


        private void fadeThing()
        {
            currentFadeTime = teleportFadeTime;
            SteamVR_Fade.Start(Color.clear, 0);
            SteamVR_Fade.Start(Color.black, 0.1f);
            Invoke("TeleportPlayer", currentFadeTime);
        }

        //-------------------------------------------------
        private void TeleportPlayer()
        {
            SteamVR_Fade.Start(Color.clear, currentFadeTime);

        }

        bool clockwise = false;

        float scrollpos = 0;
        float lastScrollpos = 0;
        private void moveTileObject(Hand hand)
        {
            if(isInTileObjectEditMenu)
            {

                if (selectedObject != null)
                {
                    if (squeezeLeftAction.GetAxis(hand.handType) > 0.9 && hand.handType == SteamVR_Input_Sources.LeftHand)
                    {
                        if (!startMovingTileObject)
                        {
                            startMovingTileObject = true;
                            lastHandPosition = hand.transform.position;
                        }

                        Vector3 handPositionDelta = lastHandPosition - hand.transform.position;

                        //LevelManager man = GameObject.Find("ScriptGameObject").GetComponent<LevelManager>();//todo make this a singleton
                        LevelManager man = TileInteractor.instance.lvlManager;//close enough
                        Vector3 tilePosition = man.getWorldPosFromTilePos(vRAR_Tile.tileIndex_X, vRAR_Tile.tileIndex_Y, vRAR_Tile.GetElevation());

                        float tileZ = tilePosition.z;


                        handPositionDelta.y = 0;
                        Vector3 changedPos = selectedObject.transform.position + handPositionDelta * 15f;


                        if (Vector3.Distance(tilePosition, selectedObject.transform.position) < 2.5)
                        {
                            selectedObject.transform.position -= handPositionDelta * 15f;
                        }
                        else
                        {
                            Vector3 dir = (selectedObject.transform.position - tilePosition);
                            dir.y = 0;
                            dir.Normalize();
                            selectedObject.transform.position -= dir * 0.01f;
                        }


                        lastHandPosition = hand.transform.position;

                    }
                    else if (hand.handType == SteamVR_Input_Sources.LeftHand)
                    {
                        startMovingTileObject = false;
                    }
                    if (touchingTrackpadLeft.GetState(hand.handType))
                    {
                        float x = trackpadLeft.GetAxis(hand.handType).x;
                        float y = trackpadLeft.GetAxis(hand.handType).y;

                        float rot = Mathf.Atan2(y, x);
                        if (rot < 0)
                        {
                            rot = 2f * Mathf.PI + rot;
                        }
                        selectedObject.transform.rotation = Quaternion.Euler(0, rot * Mathf.Rad2Deg, 0);

                    }



                    if (touchingTrackpadRight.GetState(hand.handType) && hand.handType == SteamVR_Input_Sources.RightHand)
                    {

                        //scaling
                        scrollpos += scrollRight.GetAxis(hand.handType).y;

                        float scrollDir = lastScrollpos - scrollpos;
                        

                        bool outBounds = true;
                        if (scrollDir > 0)
                        {
                            if (selectedObject.transform.localScale.x > 0.5)
                            {
                                outBounds = false;
                            }
                        }
                        if (scrollDir < 0)
                        {
                            if (selectedObject.transform.localScale.x < MAX_SCALE)
                            {
                                outBounds = false;
                            }
                        }


                        if (!outBounds)
                        {
                            selectedObject.transform.localScale = new Vector3(scrollpos, scrollpos, scrollpos);
                        }
                        else
                        {
                            scrollpos -= scrollRight.GetAxis(hand.handType).y;
                        }


                        lastScrollpos = scrollpos;

                        /*scale by rotating
                        float x = trackpadRight.GetAxis(hand.handType).x;
                        float y = trackpadRight.GetAxis(hand.handType).y;
                        //print("x :" + x + "   y :" + y);
                        float rot = Mathf.Atan2(y, x);
                        //no counter clockwise yet
                        if (y < 0)
                        {
                            //rot = 2f * Mathf.PI + rot;
                            rot = Mathf.PI*2f + rot;
                            clockwise = true;
                        }
                        else
                        {
                            if(clockwise)
                            {
                                rot += Mathf.PI * 2f;
                            }
                        }

                        if (!startScaling)
                        {
                            startScaling = true;
                            scaleRot = rot;

                        }

                        float deltaRot = scaleRot - rot;
                        Vector3 scaleDelta = (new Vector3((1 + deltaRot) * selectedObject.transform.localScale.x, (1 + deltaRot) * selectedObject.transform.localScale.y, (1 + deltaRot) * selectedObject.transform.localScale.z));
                        bool outBounds = true;
                        if (scaleDelta.x < 1)
                        {
                            if (selectedObject.transform.localScale.x > 0.5)
                            {
                                outBounds = false;
                            }
                        }
                        if (scaleDelta.x > 1)
                        {
                            if (selectedObject.transform.localScale.x < MAX_SCALE)
                            {
                                outBounds = false;
                            }
                        }


                        //if (!outBounds)
                        {
                            selectedObject.transform.localScale = scaleDelta;
                        }
                            //selectedObject.transform.localScale = (new Vector3((1 + deltaRot) * selectedObject.transform.localScale.x, (1 + deltaRot) * selectedObject.transform.localScale.y, (1 + deltaRot) * selectedObject.transform.localScale.z));

                        scaleRot = rot;
                    }
                    else if (hand.handType == SteamVR_Input_Sources.RightHand)
                    {
                        startScaling = false;
                    }*/
                    }
                }
               
            }

        }


        //-------------------------------------------------
        void FixedUpdate()
        {
            if (!visible)
            {
                return;
            }

        }


        //-------------------------------------------------
        private void PlayAudioClip(AudioSource source, AudioClip clip)
        {
            source.clip = clip;
            source.Play();
        }


        //-------------------------------------------------
        private void PlayPointerHaptic(bool validLocation)
        {
            if (pointerHand != null)
            {
                if (validLocation)
                {
                    pointerHand.TriggerHapticPulse(800);
                }
                else
                {
                    pointerHand.TriggerHapticPulse(100);
                }
            }
        }

       
  

        //-------------------------------------------------
        private bool WasTeleportButtonReleased(Hand hand)
        {
            //if (IsEligibleForTeleport(hand))
            {
                if (hand.noSteamVRFallbackCamera != null)
                {
                    return Input.GetKeyUp(KeyCode.T);
                }
                else
                {
                    return teleportAction.GetStateUp(hand.handType);

                    //return hand.controller.GetPressUp( SteamVR_Controller.ButtonMask.Touchpad );
                }
            }

            //return false;
        }

        //-------------------------------------------------
        private bool IsTeleportButtonDown(Hand hand)
        {
            //if (IsEligibleForTeleport(hand))
            {
                if (hand.noSteamVRFallbackCamera != null)
                {
                    return Input.GetKey(KeyCode.T);
                }
                else
                {
                    return teleportAction.GetState(hand.handType);

                    //return hand.controller.GetPress( SteamVR_Controller.ButtonMask.Touchpad );
                }
            }

            //return false;
        }


        //-------------------------------------------------
        private bool WasTeleportButtonPressed(Hand hand)
        {
            //if (IsEligibleForTeleport(hand))
            {
                if (hand.noSteamVRFallbackCamera != null)
                {
                    return Input.GetKeyDown(KeyCode.T);
                }
                else
                {
                    return teleportAction.GetStateDown(hand.handType);

                    //return hand.controller.GetPressDown( SteamVR_Controller.ButtonMask.Touchpad );
                }
            }

            //return false;
        }


        //-------------------------------------------------
        private Transform GetPointerStartTransform(Hand hand)
        {
            if (hand.noSteamVRFallbackCamera != null)
            {
                return hand.noSteamVRFallbackCamera.transform;
            }
            else
            {
                return hand.transform;
            }
        }

 /*
        bool didTick = false;
        private int brushSize;
        private void updateBrushSize(Hand hand)
        {
            brushSize = (int)tileInteractor.brushSizeSlider.value;
            //if(trackpadRight.GetAxisDelta(hand.handType).x>=0.3f)
            if (dpadWestRight.GetStateDown(hand.handType))
            {
                Debug.Log("Something");
                if (!didTick)
                {
                    if (brushSize > 1)
                    {
                        brushSize--;
                        didTick = true;
                        tileInteractor.setBrushSize(brushSize);
                        tileInteractor.brushSizeSlider.value = brushSize;
                    }
                }
            }
            //else if(trackpadRight.GetAxisDelta(hand.handType).x <= -0.3f)
            else if (dpadEastRight.GetStateDown(hand.handType))
            {
                Debug.Log("Something");
                if (!didTick)
                {
                    if (brushSize < tileInteractor.brushSizeSlider.maxValue)
                    {
                        brushSize++;
                        didTick = true;
                        tileInteractor.setBrushSize(brushSize);
                        tileInteractor.brushSizeSlider.value = brushSize;
                    }
                }
            }
            else// if (trackpadRight.GetAxisDelta(hand.handType).x == 0)
            {
                didTick = false;
            }
            
        }

        private int brushType;
        private void updateBrushType(Hand hand)
        {
            //if(trackpadRight.GetAxisDelta(hand.handType).y>=0.3f)
            if (dpadSouthRight.GetStateDown(hand.handType))
            {
                if (!didTick)
                {
                    if (brushType < tileInteractor.brushActionDropdown.options.Count-1)
                    {
                        brushType++;
                        tileInteractor.brushActionDropdown.value = brushType;
                    }
                    else
                    {
                        brushType = 0;
                        tileInteractor.brushActionDropdown.value = brushType;
                    }
                }
            }
            //else if(trackpadRight.GetAxisDelta(hand.handType).y <= -0.3f)
            else if (dpadNorthRight.GetStateDown(hand.handType))
            {
                if (!didTick)
                {
                    if (brushType > 0)
                    {
                        brushType--;
                        tileInteractor.brushActionDropdown.value = brushType;
                    }
                    else
                    {
                        brushType = tileInteractor.brushActionDropdown.options.Count;
                        tileInteractor.brushActionDropdown.value = brushType;
                    }
                }
            }
            else// if (trackpadRight.GetAxisDelta(hand.handType).y == 0)
            {
                didTick = false;
            }
        }*/

    }
}
