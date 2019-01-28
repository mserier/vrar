using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandFingerScript : MonoBehaviour
{
    private static HandFingerScript _instance;
    public static HandFingerScript instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HandFingerScript>();
            }

            return _instance;
        }
    }


    //-------------------------------------------------
    void Awake()
    {
        _instance = this;

    }


    private static readonly Vector3 handFingerOffset = new Vector3(0, -0.0814f, 0.0323f);

    //private static readonly Vector3 CAMPAIGN_BUTTON_loadLevel = new Vector3(0,0,0);
    //private static readonly Vector3 CAMPAIGN_BUTTON_1 = new Vector3(200, 0, 0);

    public Collider leftHandCanvasCollider;
    public Collider rightHandCanvasCollider;
    public Collider[] campaignMenuButtons;
    public Collider[] levelMenuButtons;
    //public Collider[] brushSliderPlaces;
    public Collider[] handMenuButtons;

    public Valve.VR.InteractionSystem.Hand leftHand;
    public Valve.VR.InteractionSystem.Hand rightHand;

    public GameObject campaignHandMenu;
    public GameObject levelEditorHandMenu;
    



    public bool leftHandMenuOpen { get; private set; }
    public bool rightHandMenuOpen { get; private set; }
    public int brushSliderPos { get; private set; }
    [UnityEngine.HideInInspector]
    public bool brushSliderDirty = false;
    [UnityEngine.HideInInspector]
    public bool clicked = false;
    [UnityEngine.HideInInspector]
    public bool inLeftCanvas;
    [UnityEngine.HideInInspector]
    public bool inRightCanvas;


    void Start() {
    }

    bool isButtonSelected = false;

    public static Button lastSelectedButton;

    void Update()
    {
        if (leftHand != null && rightHand != null)
        {

            Vector3 rightFinger = rightHand.transform.TransformPoint(handFingerOffset);

            if (leftHandCanvasCollider.bounds.Contains(rightFinger))
            {

                inLeftCanvas = true;
                isButtonSelected = false;

                if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR) // || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
                {


                    CheckButtonPressed(campaignMenuButtons, rightFinger);



                    if (!isButtonSelected && lastSelectedButton != null)
                    {
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                        lastSelectedButton = null;
                    }
                }
                else if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
                {
                    CheckButtonPressed(handMenuButtons, rightFinger);

                    if (clicked)
                    {
                        
                        clicked = false;
                        /*
                        for (int i = 0; i < brushSliderPlaces.Length; i++)
                        {
                            if (brushSliderPlaces[i] != null && brushSliderPlaces[i].bounds.Contains(rightFinger))
                            {
                                brushSliderPos = i;
                                brushSliderDirty = true;
                            }
                        }*/
                    }


                    for (int i = 0; i < levelMenuButtons.Length; i++)
                    {
                        if (levelMenuButtons[i] != null)
                        {
                            if (levelMenuButtons[i].bounds.Contains(rightFinger))
                            {

                                isButtonSelected = true;
                                handleButtonCollision(levelMenuButtons[i].gameObject);
                            }

                        }
                    }
                    if (!isButtonSelected && lastSelectedButton != null)
                    {
                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                        lastSelectedButton = null;
                    }
                }


            }
            else
            {
                inLeftCanvas = false;
            }
        }


    }

    public void CheckButtonPressed(Collider[] array, Vector3 finger)
    {

        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                if (array[i].bounds.Contains(finger))
                {
                    isButtonSelected = true;
                    handleButtonCollision(array[i].gameObject);
                }

            }
        }

    }


    private void handleButtonCollision(GameObject button)
    {
        Debug.Log("Hit " + button.name);
        Button btn = button.GetComponent<Button>();
        btn.Select();
        lastSelectedButton = btn;  
    }

    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (leftHand!=null)
        {

            Gizmos.DrawWireSphere(leftHand.transform.TransformPoint(handFingerOffset), 0.02f);
            //for(int i=0;i<leftHandMenuButtons.Length;i++)
            {
                //if(leftHandMenuButtons[i]!=null)
                {
                    //Gizmos.DrawCube(leftHandMenuButtons[i].bounds.center, leftHandMenuButtons[i].bounds.size);
                }
            }
            //Gizmos.DrawWireSphere(otherHandCanvas.transform.TransformPoint(CAMPAIGN_BUTTON_1), 0.02f);
            
            //Gizmos.DrawWireSphere(transform.position, 0.02f);
            //Gizmos.DrawWireSphere(correctedTransformPosition, 0.02f);
            //Gizmos.DrawWireSphere(leftHandCanvas.transform.position, 0.02f);
        }
        if(rightHand!=null)
        {
            Gizmos.DrawWireSphere(rightHand.transform.TransformPoint(handFingerOffset), 0.02f);
        }
    }

    public void openMenu(bool leftHand)
    {
        if(leftHand)
        {

            if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_CAMPAIGN_EDITOR)
            {
                campaignHandMenu.SetActive(true);
            }
            else if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR || GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
            {
                levelEditorHandMenu.SetActive(true);
            }
            leftHandMenuOpen = true;
        }
        else
        {
            rightHandMenuOpen = true;
        }
    }

    public void closeMenus()
    {
        campaignHandMenu.SetActive(false);
        levelEditorHandMenu.SetActive(false);

        leftHandMenuOpen = false;
        rightHandMenuOpen = false;
    }

}
