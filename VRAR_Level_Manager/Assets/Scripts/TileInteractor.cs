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

    public Dropdown brushActionDropdown;

    private bool brushTicking = false;
    public void setBrushTicking(bool b) { brushTicking = b; }

    private int brushSize = 1;
    public void setBrushSize(Slider slider) { brushSize = ((int)slider.value);}


    void Start ()
    {


	}

    private int brushTickDivider = 4;
    private float maxTileHeight = 8f;
    private float minTileHeight = 1f;

	void Update ()
    {
		if(brushTicking)
        {
            if(brushTickDivider--==0)
            {
                brushTickDivider = 4;
                //VRAR_Level lvl = lvlManager.getLevelObjectFromTilePos(lvlManager.getTilePosFromWorldPos(ingameCursor.position));
                VRAR_Level lvl = TileRenderer.getCurrLVL();

                VRAR_Tile tile = lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y);
                if(tile!=null)
                {
                    List<VRAR_Tile> effectedTiles = lvl.selectRadius(tile.tileIndex_X, tile.tileIndex_Y, brushSize);

                    //resetPrevous.Clear();
                   // resetPrevous.Add(lvl.getTileFromIndexPos(lvlManager.getTilePosFromWorldPos(ingameCursor.position).x, lvlManager.getTilePosFromWorldPos(ingameCursor.position).y));
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
                                    if(surroundTile.hexObject!=null)
                                        surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.defaultTileMaterial;
                                    break;
                                case 3:
                                    if (surroundTile.hexObject != null)
                                        surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.waterTileMaterial;
                                    break;
                                case 4:
                                    if (surroundTile.hexObject != null)
                                        surroundTile.hexObject.gameObject.GetComponent<Renderer>().material = tileRenderer.selectedTileMaterial;
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


}
