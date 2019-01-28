using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Holds the selectable objects that the user can add to a tile
/// </summary>
public class TileObjectHolder : MonoBehaviour
{

    
    //[SerializeField
    [SerializeField]
    public GameObject tileProperties;
    [SerializeField]
    public GameObject mainParent;
    //private GameObject selectedObject;
    private int selectedIconIndex;
    //Used to link the icons to the tileobjects 
    Dictionary<GameObject, int> indexTracker = new Dictionary<GameObject, int>();
    private void Start()
    {

        for(int i = 1; i < TileObjectManger.TILE_OBJECTS.Count; i++)
        {
            if (!TileObjectManger.TILE_OBJECTS[i].getInteractable())
            {

                GameObject go = Instantiate(TileObjectManger.TILE_OBJECTS[i].getPrefab().transform.Find("IconButton").gameObject);
                go.transform.SetParent(this.transform.Find("lPanel").Find("Scroll View").Find("Viewport").Find("Content"), false);
                //go.transform.localPosition = new Vector3(go.transform.position.x, go.transform.position.y, 0);
                go.transform.localPosition = new Vector3();
                go.GetComponent<BoxCollider>().size = new Vector3(50, 50);
                go.name = "Icon:" + TileObjectManger.TILE_OBJECTS[i].getObjectID();
                //Button b = go.GetComponent<Button>();
                indexTracker.Add(go,TileObjectManger.TILE_OBJECTS[i].getObjectID());
                //b.onClick.AddListener(delegate { this.Clicked(go); });
                //Debug.Log("Done with adding icon");

                //spawn mesh for preview
                GameObject mesh = Instantiate(TileObjectManger.TILE_OBJECTS[i].getPrefab().gameObject);
                Renderer meshRenderer = mesh.GetComponentInChildren<Renderer>();

                if(meshRenderer!=null)
                { 
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

                    //mesh.transform.localScale = scale * 40;
                    scale.Scale(new Vector3(0.02f, 0.0007f, 1f));
                    mesh.transform.localScale = scale;
                    //mesh.transform.localScale = new Vector3(0.001f, 0.00004f, 0.001f);

                    mesh.transform.parent = go.transform;

                    //mesh.transform.localPosition = new Vector3(go.transform.position.x, go.transform.position.y - 30f, -10);
                    mesh.transform.localPosition = new Vector3();
                    mesh.transform.localEulerAngles = new Vector3();
                }
            }
        }
    }


   


    /// <summary>
    /// The user selected an icon
    /// </summary>
    /// <param name="gameObject"></param>
    public void Clicked(GameObject gameObject)
    {
        int i = indexTracker[gameObject];
        //selectedObject = TileObjectManger.TILE_OBJECTS[i].getObjectID().getPrefab().gameObject;
        selectedIconIndex = i;
        //Debug.Log("Selected prefab = "  + selectedObject + "  index :" + i);

    }

    /// <summary>
    /// Add a tileobject to the properties screen
    /// </summary>
    public void Add() {
        try
        {
            BaseTileObject tileObject = TileObjectManger.TILE_OBJECTS[selectedIconIndex];
            //tileProperties.GetComponent<TileObjectEditor>().AddTileObject(tileObject);
            UIRayCastScript.GetTileObjectEditor().AddTileObject(tileObject);
        }
        catch(IndexOutOfRangeException ex)
        {
            Debug.LogWarning(ex.Message);
        }
    }



    /// <summary>
    /// Close the current window
    /// </summary>
    public void CloseWindow()
    {
        tileProperties.GetComponent<TileObjectEditor>().addTileObjectsWindowIsOpen = false;
        Destroy(this.gameObject);
        /*
        HandMenu.instance.ObjectToolTab.GetComponent<Canvas>().enabled = true;
        foreach (MeshRenderer meshR in HandMenu.instance.ObjectToolTab.GetComponentsInChildren<MeshRenderer>())
        {
            meshR.enabled = true;
        }

        tileProperties.GetComponent<TileObjectEditor>().addTileObjectsWindowIsOpen = false;
        
        //Dont ask, just SetActive(false) don't work
        Transform parent = this.gameObject.transform.parent;
        HandMenu.instance.ObjectAdderToolSubTab = Instantiate(this.gameObject, parent);
        Destroy(this.gameObject);
        HandMenu.instance.ObjectAdderToolSubTab.gameObject.SetActive(false);*/
    }

    public void Update()
    {
        //transform.rotation = tileProperties.transform.rotation;
    }

}
