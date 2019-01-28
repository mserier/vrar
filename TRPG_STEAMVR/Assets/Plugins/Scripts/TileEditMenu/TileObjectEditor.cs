using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Class for the tile properties window
/// </summary>
public class TileObjectEditor : MonoBehaviour
{


    [SerializeField]
    public GameObject addTileObjectMenuPrefab;
    private List<BaseTileObject> tileObjects = new List<BaseTileObject>();
    private Dictionary<string, BaseTileObject> objectTracker = new Dictionary<string, BaseTileObject>();
    private Dictionary<string, int> indexTracker = new Dictionary<string, int>();
    private VRAR_Tile selectedTile;
    //private Transform contentHolder;
    private GameObject selectedIcon;
    private GameObject selectedObject;
    private Dropdown dropdownTerrain;
    private Dropdown dropdownElevation;
    private Dropdown dropdownWalkable;
    private TileInteractor tileInteractor;
    private Camera m_Camera;
    public bool amActive = false;
    public bool autoInit = false;
    public bool addTileObjectsWindowIsOpen;
    private float elevation;
    private bool walkable;
    private string terrain;
    public int spawnDistance;
    private string standartKeyName = "SavedIcon";


    void Start()
    {
        //Debug.Log("Started. Objects : " + tileObjects.Count);

        Valve.VR.InteractionSystem.VRInputHandler.setIntTileObjectEditMenu(true);
    }


    public GameObject getSelectedObject()
    {
        return selectedObject;
    }

    public void Awake()
    {
        if (FallbackManagerScript.instance.isInFallBack)
        {
            m_Camera = FallbackManagerScript.instance.fallbackCam;
            //Debug.Log("in fallback");
        }
        else
        {
            m_Camera = GameObject.Find("VRCamera").gameObject.GetComponent<Camera>();
            //Debug.Log("not in fallback");
        }
        //transform.position = m_Camera.transform.position + m_Camera.transform.forward * spawnDistance;

    }


    /// <summary>
    /// Save the last clicked object
    /// </summary>
    /// <param name="gameObjectInfo"></param>
    public void SetSelectedObject(VRAR_Tile gameObjectInfo)
    {
        selectedTile = gameObjectInfo;
        print("setting selectedTile to :" + selectedTile);
    }

    /// <summary>
    /// Load in objects from a list
    /// </summary>
    /// <param name="objects"></param>
    public void LoadInObjects(List<BaseTileObject> objects)
    {
        tileObjects = objects;
        //contentHolder = this.transform.Find("ObjectPreferences").transform.Find("ScrollView").transform.Find("Viewport").transform.Find("Content");
        foreach (BaseTileObject t in tileObjects)
        {
            if (!t.getInteractable())
            {
                try
                {
                    int id = t.getObjectID();
                    string key = standartKeyName + id;
                    objectTracker.Add(key, t);
                    indexTracker.Add(key, id);
                    //    Debug.Log("Key name " + key);
                    //Debug.Log("Added a new key: " + key);
                    InstantiateObject(t, id);
                    //InstantiateObject(t.GetGameObject(), id);

                }
                catch (System.ArgumentException ex)
                {
                    Debug.LogWarning(ex.Message);
                }
            }
        }
        //Debug.Log("Loaded in objects. Size: " + indexTracker.Count);
    }

    /// <summary>
    /// Instantiate the buttons for display in the content holder
    /// </summary>
    /// <param name="tileObject"></param>
    //private void InstantiateObject(GameObject tileObject, int id)
    private void InstantiateObject(BaseTileObject baseTileObject, int id)
    {
        print("InstantiateObject :" + baseTileObject + "  id :" + id);
        GameObject tileObject = baseTileObject.getPrefab().gameObject;
        if (tileObject == null)
        {
            Debug.LogWarning("Warning given tileObject is null");
        }

        
        GameObject f = Instantiate(tileObject.transform.Find("IconButton").gameObject);

        //GameObject mesh = tileObject.GetComponentInChildren<MeshFilter>().gameObject;
        //print("mesh object :" + mesh.name);

        //Image image = f.GetComponent<Image>();
        //Texture2D texture = AssetPreview.GetMiniThumbnail(TileObjectManger.instance.prefabsThisArrayWillBeReplacedWithAnAutomaticWay[2]);
        //image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

        f.transform.SetParent(GameObject.Find("Content").gameObject.transform);
        f.transform.localScale = new Vector3(1, 1, 1);
        f.transform.rotation = new Quaternion(0, 0, 0, 0);
        //f.transform.localPosition = new Vector3(f.transform.position.x, f.transform.position.y, 0);
        f.transform.localPosition = new Vector3();
        f.GetComponent<BoxCollider>().size = new Vector3(50, 50);
        //Give the object the name of the id
        f.name = standartKeyName + id;
        //Add an click listener for when the button is clicked
        //Button b = f.GetComponent<Button>();
        // f.transform.GetChild(0).gameObject.GetComponent<Indexer>().index = prefabs.Count - 1;
        // b.onClick.AddListener(delegate { this.SelectTileObject(f); });

        //spawn mesh for preview
        GameObject mesh = Instantiate(tileObject.GetComponentInChildren<MeshFilter>().gameObject);
        Renderer meshRenderer = mesh.GetComponent<Renderer>();

        Vector3 size = meshRenderer.bounds.size;
        Vector3 scale = transform.localScale;

        if (size.x > size.y)
        {
            if (size.x > size.z)
                scale /= size.x;
            else
                scale /= size.z;
        }
        else if (size.z > size.y)
        {
            if (size.z > size.x)
                scale /= size.z;
            else
                scale /= size.x;
        }
        else
            scale /= size.y;

        scale.Scale(new Vector3(1, 1, 0.1f));

        //mesh.transform.localScale = scale*40;
        scale.Scale(new Vector3(0.02f, 0.0007f, 1f));
        mesh.transform.localScale = scale;

        mesh.transform.parent = f.transform;

        //mesh.transform.localPosition = new Vector3(f.transform.position.x, f.transform.position.y-30f, -10);
        mesh.transform.localPosition = new Vector3();
        mesh.transform.localEulerAngles = new Vector3();
    }



    //Add a tileobject to the gridview and in the world
    public void AddTileObject(BaseTileObject tileObject)
    {
        
        if (tileObject != null)
        {
            //Debug.Log("Adding a tileobject current size = " + indexTracker.Count + " and added id is " + tileObject.getObjectID());
            string key = standartKeyName + tileObject.getObjectID();
            //Debug.Log("Added a new key: " + key);
            int id = tileObject.getObjectID();
            Debug.Log("Trying to get id: " + id);
            foreach(KeyValuePair<string,BaseTileObject> a in objectTracker)
            {
                Debug.Log("Key: " + a.Key);
            }


            if (!objectTracker.ContainsKey(key))
            {
                //print("Did not exists");
                objectTracker.Add(key, tileObject);
                indexTracker.Add(key, id);
                InstantiateObject(tileObject, id);
                //InstantiateObject(tileObject.getPrefab().gameObject, id);

                print("1 :"+ selectedTile);
                print("2 :"+ selectedTile.tileGameObject);
                print("3 :"+ tileObjects);
                selectedTile.AddPosition(id, selectedTile.tileGameObject.position);
                selectedTile.AddScale(id, new Vector3(1, 1, 1));
                selectedTile.AddRotation(id, new Quaternion());
                TileRenderer.instance.updateTile(selectedTile);


                //TODO should be handled better
                tileObjects.Clear();

                foreach (KeyValuePair<string, BaseTileObject> entry in objectTracker)
                {
                    tileObjects.Add(entry.Value);
                }

                selectedTile.UpdateList(tileObjects, GetElevation(), GetWalkable(), terrain);
            }
            else
            {
                print("Already exists");
            }
    
        }
        else
        {
            throw new System.Exception("Null tile object");
        }


    }

    /// <summary>
    /// Close the properties and apply changes
    /// </summary>
    public void Done()
    {
        if (!addTileObjectsWindowIsOpen)
        {

            applyChanges();

            //tileInteractor.menuOpen = false;
            //GameObject.Destroy(this.gameObject);
        }

        Valve.VR.InteractionSystem.VRInputHandler.setIntTileObjectEditMenu(false); ;//.isInTileObjectEditMenu = false;
    }

    public void applyChanges()
    {
        //To keep track of the tileObjects position and scale within the tile
        Dictionary<int, Vector3> locationList = new Dictionary<int, Vector3>();
        Dictionary<int, Vector3> scaleList = new Dictionary<int, Vector3>();
        Dictionary<int, Quaternion> rotationList = new Dictionary<int, Quaternion>();

        tileObjects.Clear();

        foreach (KeyValuePair<string, BaseTileObject> entry in objectTracker)
        {
            tileObjects.Add(entry.Value);
        }

        for (int i = 0; i < selectedTile.tileGameObject.childCount; i++)
        {
            Transform tileObjectTransform = selectedTile.tileGameObject.GetChild(i);
            int id = -1;
            int.TryParse(tileObjectTransform.name, out id);
            if (id > 0)//-1 = invalid, 0 = "air"
            {
                locationList.Add(id, tileObjectTransform.position);
                scaleList.Add(id, tileObjectTransform.localScale);
                rotationList.Add(id, tileObjectTransform.rotation);
            }


        }

        //Debug.Log("Tile object size after properties menu: " + tileObjects.Count);
        //selectedObject.UpdateList(tileObjects, GetElevation(), GetWalkable(), dropdownTerrain.captionText.text);

        selectedTile.UpdateList(tileObjects, locationList, scaleList, rotationList, GetElevation(), GetWalkable(), terrain);
    }

    public void SetTileInteractor(TileInteractor tileInteractor)
    {
        this.tileInteractor = tileInteractor;
    }

    public void Update()
    {
        //transform.parent.transform.LookAt(transform.parent.transform.position + m_Camera.transform.rotation * Vector3.forward, m_Camera.transform.rotation * Vector3.up);

        //transform.rotation = m_Camera.transform.rotation;


    }


    private float GetElevation()
    {
        //string a = dropdownElevation.captionText.text;
        return elevation;
    }

    //TODO elevation should prolly be a text box intead of a dropdown
    public void SetElevationCaption(float ele)
    {
        /*
        dropdownElevation = GameObject.Find("DropdownElevation").GetComponent<Dropdown>();
        if (dropdownElevation == null) { Debug.Log("Elevation is null"); }
        string a = ele.ToString();
        SetDropDownValue(a, dropdownElevation);
        */
        elevation = ele;
    }

    /// <summary>
    /// Loads in the dropdown value for the terrain
    /// </summary>
    /// <param name="ter"></param>
    public void SetTerrainCaption(string ter)
    {
        /*
        dropdownTerrain = GameObject.Find("DropdownTerrain").GetComponent<Dropdown>();
        if (dropdownTerrain == null) { Debug.Log("Terrain is null"); }
        SetDropDownValue(ter, dropdownTerrain);
        */
        terrain = ter;
    }

    /// <summary>
    /// Loads in the dropdown value to show if the terrain is walkable
    /// </summary>
    /// <param name="walk"></param>
    public void SetWalkable(bool walk)
    {
        /**
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
    */
        walkable = walk;
    }


    /// <summary>
    /// Loads in the dropdown value to show if the terrain is walkable
    /// </summary>
    /// <param name="walk"></param>
    private void SetDropDownValue(string value, Dropdown drop)
    {
        if (drop == null) { Debug.Log("Given dropdown is null"); }

        //bool found = false;
        //Debug.Log("Trying to find " + value + " in dropdown");
        for (int i = 0; i < drop.options.Count; i++)
        {
            string currentValue = drop.options[i].text;
            //Debug.Log("Checking if " + currentValue + " = " + value);
            if (currentValue == value)
            {
                drop.value = i;
                //Debug.Log("Found value in dropdown");
                //found = true;
                break;
            }
        }
        //Debug.Log("Found = " + found);
    }

    private bool GetWalkable()
    {
        /**
        string a =dropdownWalkable.captionText.text;
        if(a == "True")
        {
            return true;
        }
        else
        {
            return false;
        }
    **/
        return walkable;
    }

    //Open up the menu that shows the possible prefabs
    public void OpenTileObjectAdder()
    {
        if (!addTileObjectsWindowIsOpen)
        {
            //Debug.Log("Open tileobject adder");
            GameObject go = Instantiate(addTileObjectMenuPrefab);
            go.transform.position = new Vector3(transform.position.x + 5, transform.position.y, transform.position.z);
            addTileObjectsWindowIsOpen = true;
            TileObjectHolder pre = go.GetComponentInChildren<TileObjectHolder>();
            pre.tileProperties = this.gameObject;
        }
    }

    //TODO Add an effect so the user knows wich prefab is currently clicked
    //Tell the remove button wich prefab is currently pressed
    public void SelectTileObject(GameObject gameObject)
    {
        if (gameObject != null)
        {
            selectedIcon = gameObject;
            //WTF GOVERT WAT IS DEZE SHIT NOU WEER!?
            //selectedTile.tileIndex_X = 5;
            //selectedTile.tileIndex_X = 5;
            int id = indexTracker[selectedIcon.name];
            //Debug.Log("id = " + id);
            //Debug.Log("Trying to find " + id);
            selectedObject = selectedTile.tileGameObject.transform.Find(indexTracker[selectedIcon.name].ToString()).gameObject;
            Vector3 currentScale = selectedObject.transform.localScale;
            //selectedObject.transform.localScale = new Vector3(currentScale.x++ + 1, currentScale.y + 1, currentScale.z + 1);
            Valve.VR.InteractionSystem.VRInputHandler.instance.setSelectedObject(selectedTile, selectedObject);
            //print("set selectedObject tile :" + selectedTile);


        }
        else
        {
            Debug.Log("FIUI(OJILUjdsf/slf");
        }
    }

    public void DeleteSelection()
    {
        if (selectedIcon != null)
        {
            int id = indexTracker[selectedIcon.name];
            //Debug.Log("List size before deletion: " + indexTracker.Count);
            objectTracker.Remove(selectedIcon.name);
            selectedTile.clearObjectFromlists(id);
            indexTracker.Remove(selectedIcon.name);
            //Debug.Log("List size after deletion: " + indexTracker.Count);
            tileObjects.Clear();

            foreach (KeyValuePair<string, BaseTileObject> entry in objectTracker)
            {
                tileObjects.Add(entry.Value);
            }
            Destroy(selectedIcon);
            selectedTile.UpdateList(tileObjects, elevation, walkable, terrain);
            TileRenderer.instance.updateTile(selectedTile);

        }
        else
        {
            Debug.LogWarning("Selected is null");
        }
    }



    public void clickGrass()
    {
        terrain = "Grass";
    }

    public void clickMOuntain()
    {
        terrain = "Mountain";
    }

    public void clickWater()
    {
        terrain = "Water";
    }

    public void clickWalkable()
    {
        if (walkable)
        {
            walkable = false;
        }
        else
        {
            walkable = true;
        }
    }

    public void setCamera(Camera camera)
    {
        m_Camera = camera;
    }

}
