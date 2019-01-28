using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallbackManagerScript : MonoBehaviour
{
    public Camera fallbackCam;
    public Transform LeftCursor;
    public Transform rightCursor;
    public Canvas leftOnHandCanvas;
    public Canvas informationCanvas;
    private FallbackManagerScript instance_;
    public bool isInFallBack { get; private set; }

    //-------------------------------------------------
    private static FallbackManagerScript _instance;
    public static FallbackManagerScript instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FallbackManagerScript>();
            }

            return _instance;
        }
    }


    //-------------------------------------------------
    void Awake()
    {
        _instance = this;

    }

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        //if(FindObjectOfType<>)
        if (Valve.VR.SteamVR.instance == null)
        {
            isInFallBack = true;
            Debug.Log("Using Fallback mode!");
            LeftCursor.SetParent(null);
            rightCursor.SetParent(null);
            leftOnHandCanvas.transform.SetParent(null);
            leftOnHandCanvas.transform.Find("HandMenu").localPosition = new Vector3(-310, 90, 0);
            //leftOnHandCanvas.renderMode = RenderMode.WorldSpace;
            //uhm, more stuff in HandMenu.cs
            leftOnHandCanvas.planeDistance = 1;


        }
        else
        {
            isInFallBack = false;
            Debug.Log("Not using fallback mode!");
        }
    }

    private bool singleScrollWheelTick = true;
    // Update is called once per frame
    void Update()
    {
        if(isInFallBack)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                rightCursor.transform.position = fallbackCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, fallbackCam.transform.position.y));
            if (Input.GetKey(KeyCode.LeftControl))
                LeftCursor.transform.position = fallbackCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, fallbackCam.transform.position.y));

            if (Input.GetKey(KeyCode.Tab))
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


            if (GameStateManager.instance.getGlobalStateIndex() == GameStateManager.STATE_LEVEL_EDITOR)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //TileInteractor.instance.setBrushTicking(true);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    //TileInteractor.instance.setBrushTicking(false);
                }
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    TileInteractor.instance.TileToolEditTick =true;
                }
                /*
                if (Input.GetAxis("Mouse ScrollWheel") > 0f && singleScrollWheelTick)
                {

                    //Debug.Log("Something");
                    singleScrollWheelTick = false;
                    TileInteractor.instance.brushSizeSlider.value = TileInteractor.instance.brushSizeSlider.value + 1f;
                    TileInteractor.instance.setBrushSize((int)(TileInteractor.instance.brushSizeSlider.value));
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f && singleScrollWheelTick)
                {
                    //Debug.Log("Something");
                    singleScrollWheelTick = false;
                    TileInteractor.instance.brushSizeSlider.value = TileInteractor.instance.brushSizeSlider.value - 1f;
                    TileInteractor.instance.setBrushSize((int)(TileInteractor.instance.brushSizeSlider.value));
                }
                else 
                {
                    singleScrollWheelTick = true;
                }
                if (Input.GetMouseButtonDown(2))
                {
                    if (TileInteractor.instance.brushActionDropdown.value+1 >= TileInteractor.instance.brushActionDropdown.options.Count)
                    {
                        TileInteractor.instance.brushActionDropdown.value = 0;
                    }
                    else
                    {
                        TileInteractor.instance.brushActionDropdown.value = TileInteractor.instance.brushActionDropdown.value + 1;
                    }
                }*/

            }
        }
    }
}
