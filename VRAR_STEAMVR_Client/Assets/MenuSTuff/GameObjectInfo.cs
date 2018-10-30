using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameObjectInfo : MonoBehaviour {

    public TileProperties propertiesMenuPrefab;
    private List<GameObject> objects = new List<GameObject>();
    //public GameObject example;
    private string terrain;
    private float elevation;
    private bool walkable;
    private bool clicked = false;



    // Use this for initialization
    void Start () {
  
        terrain = "Grass";
        elevation = 1f;
        walkable = true;
    }

    public void MenuClosed()
    {
        clicked = false; 
    }

    public bool GetWalkable()
    {
        return walkable;
    }
    public void SetWalkable(bool walk)
    {
        walkable = walk;
    }

    public void SetElevation(float ele)
    {
        elevation = ele;
    }

    public float GetElevation()
    {
        return elevation;
    }

    public void SetTerrain(string terra)
    {
        terrain = terra;
    }

    public string GetTerrain()
    {
        return terrain;
    }
	
	
    //Apply changes from the properties window
    public void UpdateList(List<GameObject> editedList)
    {
        this.objects = editedList;
        //this.objects.Clear;
        for(int i = 0; i < editedList.Count; i++)
        {
           Debug.Log("object " + i +  " " + editedList[i]);
        }
        int a = 4;
    }

    
    public void OnMouseUp()
    {

        if (!clicked)
        {
          //  clicked = true;
          //  Debug.Log("Object pressed");
         //   TileProperties a = Instantiate(propertiesMenuPrefab, transform.position, transform.transform.rotation);
           // a.SetSelectedObject(this);
           // a.LoadInObjects(objects);
          //  a.SetElevationCaption(elevation);
      //      a.SetTerrainCaption(terrain);
     //       a.SetWalkable(walkable);
        }
    }
    





}
