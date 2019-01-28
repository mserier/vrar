using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandMenu : MonoBehaviour {



    public GameObject buttonPrefab;
    public GameObject scrollPrefab;
    public GameObject brushDropDownPrefab;
    public GameObject playerInfoInstance;
    public GameObject terrainTab;
    public GameObject campaignTab;
    public GameObject TileObjectMenu;
    //public GameObject ObjectAdderToolSubTab;
    public Text optionText;
    public Text brushText;
    public string[] tileOptions;
    private GameObject buttonHolder;
    private GameObject topButtonHolder;
    public static bool isInObjectTab {get; private set;}

    public UIRayCastScript rayCastScript;

    private int optionIndex = 0;
    private int brushSize = 1;
    public  int maxBrushSize = 6;



    void OnMouseOver()
    {

        //Debug.Log("Hover");
       if(rayCastScript != null)
        {
            rayCastScript.inMenu = true;
        }
    }

    void OnMouseExit()
    {
        //Debug.Log("No hover");
        if (rayCastScript != null)
        {
            rayCastScript.inMenu = false;
        }
    }

    private static HandMenu _instance;
    public static HandMenu instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<HandMenu>();
            }

            return _instance;
        }
    }


    // Use this for initialization
    void Awake()
    {
        _instance = this;
        buttonHolder = GameObject.Find("HandMenuButtonHolder").gameObject;
        topButtonHolder = GameObject.Find("TopButtonHolder").gameObject;
        LoadButtonOption1();
    }


    //
    IEnumerator Start()
    {
        yield return null;
        //Debug.Log(GetComponent<RectTransform>().rect.width);
        // updateColliders(buttonHolder);
        updateColliders(topButtonHolder, new Vector3());
        if (FallbackManagerScript.instance.isInFallBack)
        {
            GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
            GameObject panel = this.transform.Find("Panel").gameObject;
            panel.transform.localScale = new Vector3(0.69f, 0.69f, 0.69f);
            GetComponent<Canvas>().worldCamera = FallbackManagerScript.instance.fallbackCam;
        }
    }
   

    public void LoadButtonOption1()
    {
        ClearBottomButtonHolder();

        if (playerInfoInstance != null)
        {
            playerInfoInstance.SetActive(true);
            GameObject turnButtons = GameObject.Find("TurnButtons").gameObject;
            updateColliders(turnButtons, new Vector3((float)-50.71, 10));
        }
        else
        {
            Debug.LogWarning("No playerinfo set!");
        }
        
        /*
        InstantiateButton("Btn", "1");
        InstantiateButton("Btn", "1");
        InstantiateButton("Btn", "1");
        InstantiateButton("Btn", "1");
        InstantiateButton("Btn", "1");
        InstantiateButton("Btn", "1");
        //StartCoroutine("DelayUpdate", 1);
        */
    }

    public void LoadButtonOption2()
    {
        ClearBottomButtonHolder();
        if (terrainTab != null)
        {
           terrainTab.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No terrain tab set!");
        }
    }

    public void LoadButtonOption3()
    {
        ClearBottomButtonHolder();
        if (campaignTab != null)
        {
            campaignTab.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No campaign tab set!");
        }
    }
    public void LoadButtonOption4()
    {
        ClearBottomButtonHolder();
        if (TileObjectMenu != null)
        {
            TileObjectMenu.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No ObjectTool tab set!");
        }
        isInObjectTab = true;


        /*
        ClearBottomButtonHolder();
        if (ObjectToolTab != null)
        {
            ObjectToolTab.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No ObjectTool tab set!");
        }
        isInObjectTab = true;*/
    }

    /*
    public void loadObjectAdderToolSubTab()
    {

        //ClearBottomButtonHolder();
        ObjectToolTab.GetComponent<Canvas>().enabled = false;
        foreach (MeshRenderer meshR in ObjectToolTab.GetComponentsInChildren<MeshRenderer>())
        {
            meshR.enabled = false;
        }

        if (ObjectAdderToolSubTab != null)
        {
            ObjectAdderToolSubTab.SetActive(true);
        }
        else
        {
            Debug.LogWarning("No LoadObjectAdderTool tab set!");
        }
    }*/

    private void instantiateBrushDropdown()
    {
        GameObject btn = Instantiate(brushDropDownPrefab);
        // btn.name = name;
        btn.transform.SetParent(buttonHolder.transform, false);
    }
    

    public void InstantiateScroll()
    {
        GameObject btn = Instantiate(scrollPrefab);
       // btn.name = name;
        btn.transform.SetParent(buttonHolder.transform, false);
        GameObject handle = GameObject.Find("HandleDeluxe");
        BoxCollider box = handle.GetComponent<BoxCollider>();
        RectTransform rt = box.GetComponent<RectTransform>();
        box.size = new Vector3(rt.rect.width, rt.rect.width, 1);

    }

    public void NextOption()
    {
        optionIndex++;
        if (optionIndex == tileOptions.Length)
        {
            optionIndex = 0;
        }
        UpdateBrushOption();
    }

    public void NextBrushSize()
    {
        brushSize++;
        if(brushSize > maxBrushSize)
        {
            brushSize = 1;
        }
        TileInteractor.instance.setBrushSizeByInt(brushSize);
        brushText.text = brushSize.ToString();
    }

    public void PreviousBrushSize()
    {
        brushSize--;
        if (brushSize < 0)
        {
            brushSize = maxBrushSize;
        }
        TileInteractor.instance.setBrushSizeByInt(brushSize);
        brushText.text = brushSize.ToString();
    }

    public void PreviousOption()
    {
        optionIndex--;
        if (optionIndex == -1)
        {
            optionIndex = tileOptions.Length - 1;
        }
        UpdateBrushOption();
    }

    private void UpdateBrushOption()
    {
        
        optionText.text = tileOptions[optionIndex];
        TileInteractor.instance.EditTile(optionIndex);
    }

    public void ClearBottomButtonHolder()
    {
        for (int i = 0; i < buttonHolder.transform.childCount; i++)
        {


            buttonHolder.transform.GetChild(i).gameObject.SetActive(false);
         
        }
        isInObjectTab = false;
    }

    private void InstantiateButton(string name, string text)
    {
        GameObject btn = Instantiate(buttonPrefab);
        btn.name = name;
        btn.transform.Find("Text").GetComponent<Text>().text = text;
        btn.transform.SetParent(buttonHolder.transform, false);
    }
    
    IEnumerator DelayUpdate(float count)
    {
        yield return new WaitForSeconds(count);
        updateColliders(buttonHolder, new Vector3());
        yield return null;

    }

    private void updateColliders(GameObject holder, Vector3 center)
    {
        for (int i = 0; i < holder.transform.childCount; i++)
        {
            GameObject Go = holder.transform.GetChild(i).gameObject;
            BoxCollider box = Go.GetComponent<BoxCollider>();
            Vector3 si = Go.GetComponent<Collider>().bounds.size;
            RectTransform rt = Go.GetComponent<RectTransform>();
            box.size = new Vector3(rt.rect.width, rt.rect.height, 1);
            box.center = center;
        }
    }

}
