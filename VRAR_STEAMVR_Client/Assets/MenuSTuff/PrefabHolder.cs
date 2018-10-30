using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrefabHolder : MonoBehaviour
{

    
    //[SerializeField]
   // public GameObject[] prefabs = new GameObject[0];
    [SerializeField]
    public TileProperties tileProperties;
    [SerializeField]
    public GameObject mainParent;
    //private GameObject selectedObject;
    private int selectedIconIndex;
    private void Start()
    {

        for(int i = 1; i < TileObjectManger.TILE_OBJECTS.Count; i++)
        {
            if (!TileObjectManger.TILE_OBJECTS[i].getInteractable())
            {
                GameObject go = Instantiate(TileObjectManger.TILE_OBJECTS[i].getPrefab().transform.GetChild(0).gameObject);
                go.transform.SetParent(this.transform.Find("lPanel").Find("Scroll View").Find("Viewport").Find("Content"), false);
                Button b = go.GetComponent<Button>();
                go.GetComponent<Indexer>().index = TileObjectManger.TILE_OBJECTS[i].getObjectID();
                b.onClick.AddListener(delegate { this.Clicked(go); });
            }
        }
    }


   



    public void Clicked(GameObject gameObject)
    {
        
        int i = gameObject.GetComponent<Indexer>().index;
        //selectedObject = TileObjectManger.TILE_OBJECTS[i].getObjectID().getPrefab().gameObject;
        selectedIconIndex = i;
        //Debug.Log("Selected prefab = "  + selectedObject + "  index :" + i);

    }

    public void Add() {
        //int i = selectedObject.transform.GetChild(0).GetComponent<Indexer>().index;
        //BaseTileObject baseTileObject = TileObjectManger.TILE_OBJECTS[i];
        //baseTileObject.setPrefab(selectedObject.transform);
        //tileProperties.AddPrefab(baseTileObject);
        tileProperties.AddPrefab(TileObjectManger.TILE_OBJECTS[selectedIconIndex]);
    }

    public void CloseWindow()
    {
        Destroy(mainParent.gameObject);
    }

}
