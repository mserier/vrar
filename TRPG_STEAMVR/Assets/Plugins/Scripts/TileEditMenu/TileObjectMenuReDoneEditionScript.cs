using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileObjectMenuReDoneEditionScript : MonoBehaviour {

    public Transform tilePropertiesButtonBackgroundPrefab;

    public static TileObjectMenuReDoneEditionScript Instance { get; private set; }

    private VRAR_Tile curTile;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Warning: multiple " + this + " in scene!");
            DestroyImmediate(this.gameObject);
            return;
        }
    }

    public void updateTileObjectsList(VRAR_Tile tile)
    {
        curTile = tile;
        GameObject content = GetComponentInChildren<GridLayoutGroup>().gameObject;
        for(int i=0;i< content.transform.childCount;i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }


        List<BaseTileObject> tileObjects = tile.getDumbObjectsList();
        foreach(BaseTileObject tileObject in tileObjects)
        {
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() => 
            {
                print("Remove TileObject :" + tileObject.getName());
                tile.removeDumbObject(tileObject);
                Destroy(button.gameObject);
                TileRenderer.instance.updateTile(curTile);
            });
            button.name = "TileObject :" + tileObject.getObjectID();
            if (tileObject.getPrefab() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(tileObject.getPrefab(), button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }
        /*
        if(tile.getNPCTileObject()!=null)
        {
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.green;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() =>
            {
                print("Remove NPC :" + tile.getNPCTileObject().getName());
                tile.setNPC(null);
                Destroy(button.gameObject);
                TileRenderer.instance.updateTile(curTile);
            });
            button.name = "NPC :" + tile.getNPCTileObject().getObjectID();
            if (tile.getNPCTileObject().getPrefab() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(tile.getNPCTileObject().getPrefab(), button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }
        */
        if (tile.getNPC() != null)
        {
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.green;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() =>
            {
                print("Remove NPC :" + tile.getNPC().ToString() + "with id :"+tile.getNPC().GetNonPlayerID());
                tile.setNPC(null);
                Destroy(button.gameObject);
                TileRenderer.instance.updateTile(curTile);
            });
            button.name = "NPC :" + tile.getNPC().GetNonPlayerID();
            if (tile.getNPC().GetObject() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(tile.getNPC().GetObject().transform, button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }

        if (tile.getInterior() != null)
        {
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.magenta;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() =>
            {
                print("Remove Interior :" + tile.getInterior().getName());
                tile.setInterior(null);
                Destroy(button.gameObject);
                TileRenderer.instance.updateTile(curTile);
            });
            button.name = "Interior :" + tile.getInterior().getObjectID();
            if (tile.getInterior().getPrefab() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(tile.getInterior().getPrefab(), button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }




    }

    /**
     * Make sure curTile is set!
     **/
    public void openTileObjectList()
    {
        GameObject content = GetComponentInChildren<GridLayoutGroup>().gameObject;
        for (int i = 0; i < content.transform.childCount; i++)
        {
            Destroy(content.transform.GetChild(i).gameObject);
        }


        Dictionary<int, BaseTileObject> tileObjects = TileObjectManger.TILE_OBJECTS;
        foreach (BaseTileObject tileObject in tileObjects.Values)
        {
            if(tileObject.getObjectID()==0)
            {
                continue;
            }
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.gray;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() => 
            {
                print("Adding tileObject" + tileObject.getName());
                Vector3 position = curTile.tileGameObject.position + new Vector3(UnityEngine.Random.Range(0.0f, 1f), 0, UnityEngine.Random.Range(0.0f, 1f));
                float scale = UnityEngine.Random.Range(0.5f, 1f);
                Vector3 scaleVector = new Vector3(scale, scale, scale);
                Quaternion rotation = Quaternion.Euler(new Vector3(UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(0, 360), 0));
                curTile.addDumbObject(tileObject, position, rotation, scaleVector);
                TileRenderer.instance.updateTile(curTile);
                updateTileObjectsList(curTile);
            });
            button.name = "TileObjectListItem :" + tileObject.getObjectID();
            if (tileObject.getPrefab() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(tileObject.getPrefab(), button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }
        /*
        Dictionary<int, NPCTileObject> npcObjects = TileObjectManger.NPC_TILE_OBJECTS;
        foreach (NPCTileObject npc in npcObjects.Values)
        {
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.green;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() =>
            {
                print("Adding NPC" + npc.getName());
                curTile.addNPCTileObject(npc);
                TileRenderer.instance.updateTile(curTile);
                updateTileObjectsList(curTile);
            });
            button.name = "NPCObjectListItem :" + npc.getObjectID();
            if (npc.getPrefab() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(npc.getPrefab(), button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }*/

        List<EnemyData> enemyData = Enemies.Instance.enemyData;
        int npcID = 0;
        foreach (EnemyData npc in enemyData)
        {
            print("reading NPC [" + npc.enemyName + "] with id :" + npcID);
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.green;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() =>
            {
                print("enemyData length :" + enemyData.Count);
                print("Adding NPC [" + npc.enemyName+"] with id :"+(npcID-1));
                curTile.setNPC(new NonPlayer(npcID-1));
                TileRenderer.instance.updateTile(curTile);
                updateTileObjectsList(curTile);
            });
            button.name = "NPCObjectListItem :" + npcID;
            if (npc.prefab != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(npc.prefab.transform, button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
            npcID++;
        }

        Dictionary<int, VRAR_Interior_Base> interiors = TileObjectManger.INTERIOR_BASE_OBJECTS;
        foreach (VRAR_Interior_Base inter in interiors.Values)
        {
            Transform button = Instantiate(tilePropertiesButtonBackgroundPrefab, content.transform);
            button.GetComponent<Image>().color = Color.magenta;
            Button buttonScript = button.GetComponent<Button>();
            buttonScript.onClick.AddListener(() =>
            {
                print("Adding Interior" + inter.getName());
                curTile.setInterior(inter);
                TileRenderer.instance.updateTile(curTile);
                updateTileObjectsList(curTile);
            });
            button.name = "InteriorListItem :" + inter.getObjectID();
            if (inter.getPrefab() != null)
            {
                Transform dumbObjectMiniMesh = Instantiate(inter.getPrefab(), button);
                normalizeMesh(dumbObjectMiniMesh.gameObject);
            }
        }

    }

    public void normalizeMesh(GameObject meshObject)
    {
        Renderer meshRenderer = meshObject.GetComponent<Renderer>();
        if (meshRenderer == null)
        {
            meshRenderer = meshObject.GetComponentInChildren<Renderer>();
        }
        if (meshRenderer != null)
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

            scale.Scale(new Vector3(0.04f, 0.04f, 0.004f));

            //mesh.transform.localScale = scale*40;
            //scale.Scale(new Vector3(0.02f, 0.0007f, 1f));
            meshObject.transform.localScale = scale;

            //mesh.transform.localPosition = new Vector3(f.transform.position.x, f.transform.position.y-30f, -10);
            meshObject.transform.localPosition = new Vector3();
            meshObject.transform.localEulerAngles = new Vector3();
        }
    }
}
