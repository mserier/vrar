using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Valve.VR.InteractionSystem
{
    //-------------------------------------------------------------------------
    public class MapMoverScript : MonoBehaviour
    {
        [SteamVR_DefaultAction("Teleport", "default")]
        public SteamVR_Action_Boolean teleportAction;
        [SteamVR_DefaultAction("Squeeze", "default")]
        public SteamVR_Action_Single squeezeAction;
        [SteamVR_DefaultAction("SelectingBrush", "default")]
        public SteamVR_Action_Vector2 selectingBrushAction;
        [SteamVR_DefaultAction("TouchingTrackpad", "default")]
        public SteamVR_Action_Boolean touchingTrackpadAction;

        public TileInteractor tileInteractor;

        public UIRayCastScript uiRayCastScript;

        private Hand pointerHand = null;
        private Player player = null;

        private bool visible = false;

        // Events

        public static SteamVR_Events.Event<float> ChangeScene = new SteamVR_Events.Event<float>();
        public static SteamVR_Events.Action<float> ChangeSceneAction(UnityAction<float> action) { return new SteamVR_Events.Action<float>(ChangeScene, action); }

        public static SteamVR_Events.Event<TeleportMarkerBase> Player = new SteamVR_Events.Event<TeleportMarkerBase>();
        public static SteamVR_Events.Action<TeleportMarkerBase> PlayerAction(UnityAction<TeleportMarkerBase> action) { return new SteamVR_Events.Action<TeleportMarkerBase>(Player, action); }

        public static SteamVR_Events.Event<TeleportMarkerBase> PlayerPre = new SteamVR_Events.Event<TeleportMarkerBase>();
        public static SteamVR_Events.Action<TeleportMarkerBase> PlayerPreAction(UnityAction<TeleportMarkerBase> action) { return new SteamVR_Events.Action<TeleportMarkerBase>(PlayerPre, action); }

        //-------------------------------------------------
        private static MapMoverScript _instance;
        public static MapMoverScript instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GameObject.FindObjectOfType<MapMoverScript>();
                }

                return _instance;
            }
        }


        //-------------------------------------------------
        void Awake()
        {
            _instance = this;

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

        private bool startMovingMap = false;
        private Vector3 movingMapDelta;
        //-------------------------------------------------
        void Update()
        {
            Hand oldPointerHand = pointerHand;
            Hand newPointerHand = null;

            foreach (Hand hand in player.hands)
            {
                if (visible)
                {
                    if (WasTeleportButtonReleased(hand))
                    {
                        if (pointerHand == hand) //This is the pointer hand
                        {
                            //TryTeleportPlayer();
                        }
                    }
                }

                if (WasTeleportButtonPressed(hand))
                {
                    newPointerHand = hand;
                }

                if(hand.handType == SteamVR_Input_Sources.LeftHand)
                {
                    if (getSqueeze(hand) > 0.9)
                    {
                        if(!startMovingMap)
                        {
                            startMovingMap = true;
                            movingMapDelta = hand.transform.position;
                        }
                        //else
                        {
                            Vector3 changePos = movingMapDelta - hand.transform.position;
                            player.transform.position += changePos*15f;
                            //player.transform = new Vector3( ( ((startMovingMapPos*2f - hand.transform.position*2f)) );
                            
                        }
                        movingMapDelta = hand.transform.position;
                    }
                    else
                    {
                        startMovingMap = false;
                    }
                }
                if(hand.handType == SteamVR_Input_Sources.RightHand && (GameStateManager.instance.getGlobalStateIndex()==GameStateManager.STATE_LEVEL_EDITOR || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR))
                {
                    if (getSqueeze(hand)>0.5)
                    {
                        uiRayCastScript.clicked = true;
                    }
                    if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
                    {
                        updateBrushSize(hand);
                        updateBrushType(hand);
                    }
                    //Debug.Log(brushSize + "      " + brushType);

                    //Debug.Log(isTouchingTrackpad(hand));
                    //tileInteractor.setBrushSize()
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

            return false;
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

            return false;
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

            return false;
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

        private float getSqueeze(Hand hand)
        {
            return squeezeAction.GetAxis(hand.handType);
        }

        bool didTick = false;
        private int brushSize;
        private void updateBrushSize(Hand hand)
        {
            if(selectingBrushAction.GetAxisDelta(hand.handType).x>=0.3f)
            {
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
            else if(selectingBrushAction.GetAxisDelta(hand.handType).x <= -0.3f)
            {
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
            else if (selectingBrushAction.GetAxisDelta(hand.handType).x == 0)
            {
                didTick = false;
            }
        }

        private int brushType;
        private void updateBrushType(Hand hand)
        {
            if(selectingBrushAction.GetAxisDelta(hand.handType).y>=0.3f)
            {
                if (!didTick)
                {
                    if (brushType < tileInteractor.brushActionDropdown.options.Count)
                    {
                        brushType++;
                        tileInteractor.brushActionDropdown.value = brushType;
                    }
                }
            }
            else if(selectingBrushAction.GetAxisDelta(hand.handType).y <= -0.3f)
            {
                if (!didTick)
                {
                    if (brushType > 0)
                    {
                        brushType--;
                        tileInteractor.brushActionDropdown.value = brushType;
                    }
                }
            }
            else if (selectingBrushAction.GetAxisDelta(hand.handType).y == 0)
            {
                didTick = false;
            }
        }

        private bool isTouchingTrackpad(Hand hand)
        {
            return touchingTrackpadAction.GetActive(hand.handType);
        }

    }
}
