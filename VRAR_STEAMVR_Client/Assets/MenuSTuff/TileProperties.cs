using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class TileProperties : MonoBehaviour {


    [SerializeField]
    public GameObject addPrefabMenuPrefab;
    private List<BaseTileObject> prefabs = new List<BaseTileObject>();
    private VRAR_Tile selectedObject;
    private Transform contentHolder;
    private GameObject selected;
    private Dropdown dropdownTerrain;
    private Dropdown dropdownElevation;
    private Dropdown dropdownWalkable;
    private TileInteractor tileInteractor;
   

    void Start()
    {
        Debug.Log("Started. Objects : " + prefabs.Count);

    }


    /**
    public TileProperties(string terrain, float elevation, bool walkable)
    {
        

    }
    **/
    /// <summary>
    /// Set the selected where the click came from
    /// </summary>
    /// <param name="gameObjectInfo"></param>
    public void SetSelectedObject(VRAR_Tile gameObjectInfo)
    {
        selectedObject = gameObjectInfo;
    }

    /// <summary>
    /// Load in objects from a list
    /// </summary>
    /// <param name="objects"></param>
    public void LoadInObjects(List<BaseTileObject> objects)
    {
        prefabs = objects;
        contentHolder = this.transform.Find("ObjectPreferences").transform.Find("ScrollView").transform.Find("Viewport").transform.Find("Content");
        for(int i = 1; i < prefabs.Count; i++)
        {
            if (!prefabs[i].getInteractable())
            {
                InstantiateObject(prefabs[i].getPrefab().gameObject);
            }
        }
        Debug.Log("Loaded in objects. Size: " + prefabs.Count);
    }

    /// <summary>
    /// Instantiate the buttons for display in the content holder
    /// </summary>
    /// <param name="prefab"></param>
    private void InstantiateObject(GameObject prefab)
    {
        if(prefab == null)
        {
            Debug.Log("Warning given prefab is null");
        }

        GameObject f = Instantiate(prefab.transform.GetChild(0).gameObject);
        f.transform.SetParent(contentHolder);
        f.transform.localScale = new Vector3(1, 1, 1);
        //Add an click listener for when the button is clicked
        Button b = f.GetComponent<Button>();
       // f.transform.GetChild(0).gameObject.GetComponent<Indexer>().index = prefabs.Count - 1;
        b.onClick.AddListener(delegate { this.SelectPrefab(f); });
    }



    //Add a prefab to the gridview
    public void AddPrefab(BaseTileObject prefab)
    {
        prefabs.Add(prefab);
        InstantiateObject(prefab.getPrefab().gameObject); 
    }

    /// <summary>
    /// Close the properties and apply changes
    /// </summary>
    public void Done()
    {
        selectedObject.UpdateList(prefabs, GetElevation(), GetWalkable(), dropdownTerrain.captionText.text);
        tileInteractor.menuOpen = false;
        GameObject.Destroy(this.gameObject);
    }

    public void SetTileInteractor(TileInteractor tileInteractor)
    {
        this.tileInteractor = tileInteractor;
    }

    private float GetElevation()
    {  
        string a = dropdownElevation.captionText.text;
        return float.Parse(a);
    }

    //TODO elevation should prolly be a text box intead of a dropdown
    public void SetElevationCaption(float ele)
    {
        dropdownElevation = GameObject.Find("DropdownElevation").GetComponent<Dropdown>();
        if (dropdownElevation == null) { Debug.Log("Elevation is null"); }
        string a = ele.ToString();
        SetDropDownValue(a, dropdownElevation);
    }

    public void SetTerrainCaption(string ter)
    {
        dropdownTerrain = GameObject.Find("DropdownTerrain").GetComponent<Dropdown>();
        if (dropdownTerrain == null) { Debug.Log("Terrain is null"); }
        SetDropDownValue(ter, dropdownTerrain);
    }

    public void SetWalkable(bool walk)
    {
        dropdownWalkable = GameObject.Find("DropdownWalkable").GetComponent<Dropdown>();
        if (dropdownWalkable == null) { Debug.Log("Walkable is null"); }
        if (walk == true)
        {
            SetDropDownValue("True", dropdownWalkable);
        }
        else
        {
            SetDropDownValue("False", dropdownWalkable);
        }
    }

    private void SetDropDownValue(string value, Dropdown drop)
    {
        if (drop == null) { Debug.Log("Given dropdown is null"); }

        bool found = false;
        Debug.Log("Trying to find " + value + " in dropdown");
        for (int i = 0; i < drop.options.Count; i++)
        {
            string currentValue = drop.options[i].text;
            Debug.Log("Checking if " + currentValue + " = " + value);
            if (currentValue == value)
            {
                drop.value = i;
                Debug.Log("Found value in dropdown");
                found = true;
                break;
            }
        }
        Debug.Log("Found = " + found);
    }

    private bool GetWalkable()
    {
        string a =dropdownWalkable.captionText.text;
        if(a == "True")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    //Open up the menu that shows the possible prefabs
    public void OpenPrefabAdder()
    {
        GameObject go = Instantiate(addPrefabMenuPrefab);
        PrefabHolder pre =  go.GetComponentInChildren<PrefabHolder>();
        pre.tileProperties = this.GetComponent<TileProperties>();
    }

    //TODO Add an effect so the user knows wich prefab is currently clicked
    //Tell the remove button wich prefab is currently pressed
    public void SelectPrefab(GameObject gameObject)
    {
        selected = gameObject;
    }

    public void DeleteSelection()
    {
        int index = selected.GetComponent<Indexer>().index;
        Debug.Log("Deletion index: " + index);
        Debug.Log("List size before deletion: " + prefabs.Count);
        prefabs.RemoveAt(index);
        Debug.Log("List size after deletion: " + prefabs.Count);
        Destroy(selected);
    }

 

}
