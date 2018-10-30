using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileInteractor : MonoBehaviour
{
    public LevelManager lvlManager;

    //temporary for setting the materials
    public TileRenderer tileRenderer;

    public Transform ingameCursor;

    public bool menuOpen = false;

    public Dropdown brushActionDropdown;
    public Slider brushSizeSlider;

    //public TileProperties tilePropertiesPrefab;

    private bool brushTicking = false;
    public void setBrushTicking(bool ticking) { brushTicking = ticking; }
    public void setBrushTickingT(bool b) { brushTicking = b; }

    private int brushSize = 1;
    public void setBrushSize(Slider slider) { brushSize = ((int)slider.value); }
    public void setBrushSize(int size) { brushSize = size ;}
    private VRAR_Tile currentlySelected;


    void Start ()
    {


	}

    private int brushTickDivider = 4;
    private float maxTileHeight = 8f;
    private float minTileHeight = 1f;

	void Update ()
    {
        //Debug.Log(brushTicking);
		if(brushTicking && GameStateManager.instance.getGlobalStateIndex()==GameStateManager.STATE_LEVEL_EDITOR)
        {
            if(brushTickDivider--==0)
            {
                brushTickDivider = 4;
                //VRAR_Level lvl = lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(leftHandCursor.position));
                VRAR_Level lvl = TileRenderer.getCurrLVL();

                VRAR_Tile tile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);
                if(tile!=null)
                {
                    currentlySelected = tile;
                    List<VRAR_Tile> effectedTiles = lvl.selectRadius(tile.tileIndex_X, tile.tileIndex_Y, brushSize);
                    if (!menuOpen)
                    {
                        
                  
                        OpenTileProperties(tile);
                        menuOpen = true;
                    }
                    //resetPrevous.Clear();
                   // resetPrevous.Add(lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(leftHandCursor.position).x, lvlManager.getTilePosFromWorldPos(leftHandCursor.position).y));
                    foreach (VRAR_Tile surroundTile in effectedTiles)
                    {
                        if(surroundTile!=null)
                        {
                            switch (brushActionDropdown.value)
                            {
                                case 0:
                                    if (surroundTile.height_ < maxTileHeight)
                                    {
                                        surroundTile.setHeight(surroundTile.height_ + 0.05f);
                                        //Debug.Log(surroundTile);
                                    }
                                    //else { surroundTile.setHeight(minTileHeight); }
                                    break;
                                case 1:
                                    if (surroundTile.height_ > minTileHeight)
                                    {
                                        surroundTile.setHeight(surroundTile.height_ - 0.05f);
                                    }
                                    //else { surroundTile.setHeight(maxTileHeight); }
                                    break;
                                case 2:
                                    if (surroundTile.hexObject != null)
                                    {
                                        //surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.defaultTileMaterial;
                                        surroundTile.SetTerrain("Mountain");
                                        TileRenderer.instance.updateTile(surroundTile);
                                    }
                                    break;
                                case 3:
                                    if (surroundTile.hexObject != null)
                                    {
                                        //surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.waterTileMaterial;
                                        surroundTile.SetTerrain("Water");
                                        TileRenderer.instance.updateTile(surroundTile);
                                    }
                                    break;
                                case 4:
                                    if (surroundTile.hexObject != null)
                                    {
                                        //surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.selectedTileMaterial;
                                        surroundTile.SetTerrain("Grass");
                                        TileRenderer.instance.updateTile(surroundTile);
                                    }
                                    break;
                            }


                        }
                        else
                        {
                            Debug.Log("sur was null");
                        }
                    }
                }
            }
        }
	}




    private void OpenTileProperties(VRAR_Tile tile)
    {

        /*
        Debug.Log("Object pressed");
        TileProperties a = Instantiate(tilePropertiesPrefab);
        a.LoadInObjects(tile.getDumbObjectsList());
        a.SetElevationCaption(tile.height_);
        a.SetTerrainCaption(tile.terrain);
        a.SetWalkable(tile.walkable);
        a.SetSelectedObject(tile);
        a.SetTileInteractor(this);*/


    }
}
