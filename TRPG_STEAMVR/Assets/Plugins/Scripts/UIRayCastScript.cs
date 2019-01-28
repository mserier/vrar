using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UIRayCastScript : MonoBehaviour
{
    private FallbackManagerScript fallbackManagerScript;

    public Valve.VR.InteractionSystem.Hand hand;

    public TileRenderer tileRenderer;
    public TileInteractor tileInteractor;
    public bool inMenu;



    //public static TileObjectEditor tileObjectEditor;
    private static TileObjectEditor t=null;
    public static TileObjectEditor GetTileObjectEditor()
    {
        if(t==null)
        {
            t = FindObjectOfType<TileObjectEditor>();
        }
        return t;
    }
    public string addTileObjectMenuName = "AddTileObjectMenu (3)(Clone)";

    public bool clicked;
    private Button currentlySelected;
    void Start()
    {
        fallbackManagerScript = Object.FindObjectOfType<FallbackManagerScript>();
    }

    void Update()
    {
        //Debug.Log(hand.transform.position);

        if (fallbackManagerScript.isInFallBack)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Ray ray = fallbackManagerScript.fallbackCam.ScreenPointToRay(Input.mousePosition);
                if (!handleRayCastHit(ray))
                {
                    if (Input.GetMouseButtonDown(0) && !inMenu)
                    {
                        //Debug.Log("Ticking");
                        TileInteractor.instance.setBrushTicking(true);
                        //Debug.Log("ticking");
                    }
                    else
                    {
                        //Debug.Log("no ticking");
                    }
                 
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                TileInteractor.instance.setBrushTicking(false);
            }

        }
        else
        {
            if (clicked)
            {
                clicked = false;
                HandFingerScript.instance.clicked = true;

                Ray ray = new Ray(hand.gameObject.transform.position, hand.gameObject.transform.TransformDirection(Vector3.forward));
                if (!handleRayCastHit(ray) && !HandFingerScript.instance.inLeftCanvas)
                {

                    tileInteractor.setBrushTicking(true);
                }

                //if (Physics.Raycast(hand.gameObject.transform.position, hand.gameObject.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))

                if (HandFingerScript.lastSelectedButton != null  )
                {
                   
                    handleCollisionClick(HandFingerScript.lastSelectedButton.gameObject);
                    
                }
               
               

                if (HandFingerScript.instance.brushSliderDirty)
                {
                    tileInteractor.setBrushSize(HandFingerScript.instance.brushSliderPos + 1);
                    tileInteractor.brushSizeSlider.value = HandFingerScript.instance.brushSliderPos + 1;
                    HandFingerScript.instance.brushSliderDirty = false;
                    Debug.Log("Something");
                }

            }
            else
            {
                tileInteractor.setBrushTicking(false);
            }

        }



    }
    private bool handleRayCastHit(Ray ray)
    {
        bool uiInteract = true;
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            //Button button = hit.transform.gameObject.GetComponent<Button>();
            string transformName = hit.transform.gameObject.name;
            switch (transformName)
            {
                case "Load Selected lvl Button":
                    tileRenderer.loadLevelOnclick();
                    break;
                case "Return to lvl selection Button":
                    tileRenderer.unloadLevelOnClick();
                    break;
                case "Save lvl Button":
                    tileRenderer.saveLVLClick();
                    break;
                case "BtnAddTileObject":
                    ////GetTileObjectEditor().OpenTileObjectAdder();
                    //print("add 1");
                    //UIRayCastScript.GetTileObjectEditor();//to set the TileObjectEditor while it's still active.
                    //FindObjectOfType<HandMenu>().loadObjectAdderToolSubTab();
                    break;
                case "RemoveTileObject":
                    GetTileObjectEditor().DeleteSelection();
                    //tileObjectEditor.GetComponent<TileObjectEditor>().DeleteSelection();
                    break;
                case "WalkableButton":
                    GetTileObjectEditor().clickWalkable();
                    break;
                case "WaterButton":
                    GetTileObjectEditor().clickWater();
                    break;
                case "MountainButton":
                    GetTileObjectEditor().clickMOuntain();
                    break;
                case "GrassButton":
                    GetTileObjectEditor().clickGrass();
                    break;
                case "BtnAddToArray":
                    GameObject.FindGameObjectWithTag("doabettersolution").GetComponent<TileObjectHolder>().Add();
                    //GameObject.Find(addTileObjectMenuName).GetComponent<TileObjectHolder>().Add();
                    break;
                case "BtnDoneProperties":
                    GetTileObjectEditor().Done();
                    break;
                case "BtnDoneAdding":
                    GameObject.FindGameObjectWithTag("doabettersolution").GetComponent<TileObjectHolder>().CloseWindow();
                    //GameObject.Find(addTileObjectMenuName).GetComponent<TileObjectHolder>().CloseWindow();
                    break;
                case "GenTerrainButton":
                    tileInteractor.genTerrain();
                    break;
                case "BtnNextTurn":
                    GameObject.Find("ScriptGameObject").GetComponent<VrGamePlayManager>().NewTurn();
                    break;
                case "BtnEndPlayerTurns":
                    GameObject.Find("ScriptGameObject").GetComponent<VrGamePlayManager>().TakeAllTurns();
                    break;
                case "BtnOption1":
                    HandMenu handMenu = GameObject.Find("HandMenu").GetComponent<HandMenu>();
                    handMenu.ClearBottomButtonHolder();
                    handMenu.LoadButtonOption1();
                    break;
                case "BtnOption2":
                    HandMenu handMenu2 = GameObject.Find("HandMenu").GetComponent<HandMenu>();
                    handMenu2.ClearBottomButtonHolder();
                    handMenu2.LoadButtonOption2();
                    break;
                default:
                    if (transformName.Contains("SavedIcon"))
                    {
                        GetTileObjectEditor().SelectTileObject(GameObject.Find(hit.transform.gameObject.name).gameObject);
                    }
                    else if (transformName.Contains("Icon"))
                    {
                        GameObject.FindGameObjectWithTag("doabettersolution").GetComponent<TileObjectHolder>().Clicked(GameObject.Find(hit.transform.gameObject.name));
                        //GameObject.Find(addTileObjectMenuName).GetComponent<TileObjectHolder>().Clicked(GameObject.Find(hit.transform.gameObject.name));
                    }
                    else
                    {
                        uiInteract = false;
                    }
                    break;
            }

        }
        else
        {
            uiInteract = false;
        }

        return uiInteract;

    }

    private bool handleCollisionClick(GameObject button)
    {
        bool uiInteract = true;
        bool shouldCloseHandMenu = false;
        string gameObjectName = button.name;
        //Debug.Log("hi");

        switch (gameObjectName)
        {
            case "LoadLevelHandButton":
                tileRenderer.loadLevelOnclick();
                shouldCloseHandMenu = true;
                break;
            case "SaveLevelHandButton":
                tileRenderer.saveLVLClick();
                break;
            case "OpenCampaingHandButton":
                tileRenderer.unloadLevelOnClick();
                shouldCloseHandMenu = true;
                break;
            case "GenTerainHandButton":
                tileInteractor.genTerrain();
                break;
            case "BtnTerrain":
                //HandMenu handMenu2 = GameObject.Find("HandMenu").GetComponent<HandMenu>();
                HandMenu.instance.ClearBottomButtonHolder();
                HandMenu.instance.LoadButtonOption2(); ;
                break;
            case "BtnPlayerInformation":
                //HandMenu handMenu = GameObject.Find("HandMenu").GetComponent<HandMenu>();
                HandMenu.instance.ClearBottomButtonHolder();
                HandMenu.instance.LoadButtonOption1();
                break;
            case "RightButton":
                //button.GetComponent<Button>().onClick.Invoke();
                HandMenu.instance.NextOption();
                break;
            case "LeftButton":
                //button.GetComponent<Button>().onClick.Invoke();
                HandMenu.instance.PreviousOption();
                break;
            case "RightBrush":
                //button.GetComponent<Button>().onClick.Invoke();
                HandMenu.instance.NextBrushSize();
                break;
            case "LeftBrush":
                //button.GetComponent<Button>().onClick.Invoke();
                HandMenu.instance.PreviousBrushSize();
                break;
            /*TileObjectMenuReDoneEditionButtons*/
            case "GrassButtonReDone":

                break;
            case "MountainButtonReDone":

                break;
            case "WaterButtonReDone":

                break;
            case "btnRemoveTileObjectFromTile":

                break;
            case "btnOpenTileObjectList":
                TileObjectMenuReDoneEditionScript.Instance.openTileObjectList();
                break;
            case "btnSaveTile":
                
                break;
            case "BtnRehost":
                Rehost();
                break;
                

            /*
        case "BtnTerrain":
            HandMenu handMenu2 = GameObject.Find("HandMenu").GetComponent<HandMenu>();
            handMenu2.ClearBottomButtonHolder();
            handMenu2.LoadButtonOption2(); ;
            break;
        case "BtnPlayerInformation":
            HandMenu handMenu = GameObject.Find("HandMenu").GetComponent<HandMenu>();
            handMenu.ClearBottomButtonHolder();
            handMenu.LoadButtonOption1();
            break;
        case "WaterButton":
            GetTileObjectEditor().clickWater();
            break;
        case "MountainButton":
            GetTileObjectEditor().clickMOuntain();
            break;
        case "GrassButton":
            GetTileObjectEditor().clickGrass();
            break;
        case "BtnDoneProperties":
            GetTileObjectEditor().applyChanges();
            break;
        case "BtnAddTileObject":
            print("add");
            FindObjectOfType<HandMenu>().loadObjectAdderToolSubTab();
            break;
        case "RemoveTileObject":
            print("rem");
            GetTileObjectEditor().DeleteSelection();
            break;*/
            default:
                Debug.Log("Default");
                uiInteract = false;
                break;
        }

        if (shouldCloseHandMenu)
        {
            HandFingerScript.instance.closeMenus();
        }

        return uiInteract;
    }

    public void UIHandleClick()
    {
        handleCollisionClick(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject);
        //Debug.Log("uihandleclick");
    }

    public void Rehost()
    {
        SceneManager.LoadScene("EntryScene");
    }
}

