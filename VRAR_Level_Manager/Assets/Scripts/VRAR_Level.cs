using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class VRAR_Level
{
    public VRAR_Tile levelTile;

    private int LevelIconID = 0;


    public List<VRAR_Tile> vrarTiles = new List<VRAR_Tile>();

    Dictionary<Vector2Int, VRAR_Tile> vrarTileDict = new Dictionary<Vector2Int, VRAR_Tile>();

    public VRAR_Tile getTileFromIndexPos(int iX, int iY)
    {
        VRAR_Tile res = null;
        //res = vrarTileArray[iX+negOffsetX,iY+negOffsetY];
        res = vrarTileDict[new Vector2Int(iX, iY)];
        return res;
    }

    public VRAR_Level(int iX, int iY)
    {
        levelTile = new VRAR_Tile(iX, iY, 1);
        //createLVLFile();////////(use this when you need to generate VRAR_Levels on the spot)
        readLVLFile();
    }


    public void createLVLFile()
    {
        //read which levels there are and create the VRAR_Level object
        //read for every level the tiles and create the VRAR_Tile objects

        //TODO generate the /levels/ directory if this doesn't exist yet.
        TextWriter tw;
        if (Application.isEditor)
        {//use the project location
            tw = new StreamWriter(Application.dataPath + "/levels/" + "/level_" + levelTile.tileIndex_X + "#" + levelTile.tileIndex_Y + ".sav");
        }
        else
        {//use the user save location
            tw = new StreamWriter(Application.persistentDataPath + "/levels/" + "/level_" + levelTile.tileIndex_X + "#" + levelTile.tileIndex_Y + ".sav");
        }
        tw.Write(vrarTiles.Count);
        tw.Write("\n");

        //save every tile in the level
        foreach (VRAR_Tile tile in vrarTiles)
        {
            bool firstSeperator = true;
            tw.Write(tile.tileIndex_X + ";" + tile.tileIndex_Y + ";" + tile.height_ + ";" + tile.getInteractableObject().getObjectID() + ";");
            foreach (BaseTileObject tileOb in tile.getDumbObjectsList())
            {
                if (firstSeperator)
                {
                    firstSeperator = false;
                    tw.Write(tileOb.getObjectID());
                }
                else
                {
                    tw.Write("<" + tileOb.getObjectID());
                }
            }
            tw.Write("\n");
        }
        tw.Flush();
        tw.Close();
    }

    public void readLVLFile()
    {
        TextReader tr;
        if (Application.isEditor)
        {//use the project location
            tr = new StreamReader(Application.dataPath + "/levels/" + "/level_" + levelTile.tileIndex_X + "#" + levelTile.tileIndex_Y + ".sav");
        }
        else
        {//use the user save location
            tr = new StreamReader(Application.persistentDataPath + "/levels/" + "/level_" + levelTile.tileIndex_X + "#" + levelTile.tileIndex_Y + ".sav");
        }
        string line;
        while ((line = tr.ReadLine()) != null)
        {
            string[] splitted = line.Split(';');
            if (splitted.Length < 2)
            {

            }
            else
            {
                VRAR_Tile tile = new VRAR_Tile(int.Parse(splitted[0]), int.Parse(splitted[1]), float.Parse(splitted[2]));
                BaseTileObject interactableObject = TileObjectManger.TILE_OBJECTS[int.Parse(splitted[3])];
                if (interactableObject.getInteractable())
                {
                    tile.setInteractableObject(interactableObject);
                }

                //Debug.Log("Reading lvl file interactable object :" + interactableObject.getName());
                if (splitted[4].Length > 0)
                {
                    string[] dumbObjectIDs = splitted[4].Split('<');
                    foreach (string dumbString in dumbObjectIDs)
                    {
                        tile.addDumbObject(TileObjectManger.TILE_OBJECTS[int.Parse(dumbString)]);
                    }
                }
                vrarTiles.Add(tile);
                vrarTileDict.Add(new Vector2Int(int.Parse(splitted[0]), int.Parse(splitted[1])), tile);
                //Debug.Log("added tile :" +tile);
            }


        }


        //if level empty, spawn default lvl tiles
        if (vrarTiles.Count == 0)
        {
            int radius = Random.Range(1, 8);
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        VRAR_Tile tile = new VRAR_Tile(x, y, Random.Range(1, 7));
                        vrarTiles.Add(tile);
                        vrarTileDict.Add(new Vector2Int(x, y), tile);
                    }
                }
            }
        }

    }

    /**
	 * Update a subtile of a lvl
	 **/
	public void tileUpdate(int xI, int yI)
	{
		//notify surrounding tiles of the change
		/*
		VRAR_Tile left = getTileFromIndexPos(xI-1, yI);
		VRAR_Tile bttm = getTileFromIndexPos(xI, yI-1);
		VRAR_Tile bttmRight = getTileFromIndexPos(xI+1, yI-1);
		VRAR_Tile right = getTileFromIndexPos(xI+1, yI);
		VRAR_Tile top = getTileFromIndexPos(xI, yI+1);
		VRAR_Tile topLeft = getTileFromIndexPos(xI-1, yI+1);

		left.isDirty = true;
		bttm.isDirty = true;
		bttmRight.isDirty = true;
		right.isDirty = true;
		top.isDirty = true;
		topLeft.isDirty = true;

		top.hexObject.gameObject.GetComponent<Renderer>().material = null;
		right.hexObject.gameObject.GetComponent<Renderer>().material = null;
		bttmRight.hexObject.gameObject.GetComponent<Renderer>().material = null;
		bttm.hexObject.gameObject.GetComponent<Renderer>().material = null;
		left.hexObject.gameObject.GetComponent<Renderer>().material = null;
		topLeft.hexObject.gameObject.GetComponent<Renderer>().material = null;*/
	}

	//vrar level binary layout
	//line based
	//first line contains magic number, information, metadata. etc.
	//every line contains 1 level (all tiles)

}


public class VRAR_Tile
{
    public Transform hexObject;

    public int tileIndex_X { get; set; }
    public int tileIndex_Y { get; set; }
    public float height_ { get; set; }

    private bool isDirty = false;

    private BaseTileObject interactableObject = TileObjectManger.TILE_OBJECTS[0];
    private List<BaseTileObject> dumbObjectsList = new List<BaseTileObject>();


    public VRAR_Tile(int iX, int iY, float height)
    {
        tileIndex_X = iX;
        tileIndex_Y = iY;
        height_ = height;

        //setInteractableObject(TileObjectManger.TILE_OBJECTS[3]);
        //addDumbObject(TileObjectManger.TILE_OBJECTS[1]);
        //addDumbObject(TileObjectManger.TILE_OBJECTS[4]);
    }

    public void setInteractableObject(BaseTileObject tileObject)
    {
        if (tileObject.getInteractable())
        {
            interactableObject = tileObject;
        }
        else
        {
            Debug.LogError("Someone tried to set a non interactable object as the interactable object (" + tileObject.getObjectID() + ") of the tile :" + this);
        }
    }

    public BaseTileObject getInteractableObject()
    {
        return interactableObject;
    }

    public void addDumbObject(BaseTileObject tileObject)
    {
        if (!tileObject.getInteractable())
        {
            dumbObjectsList.Add(tileObject);
        }
        else
        {
            Debug.LogError("Someone tried to set a interactable object as a dumb object of the tile :" + this);
        }
    }

    public void removeDumbObject(BaseTileObject tileObject)
    {
        dumbObjectsList.Remove(tileObject);
    }

    public List<BaseTileObject> getDumbObjectsList()
    {
        return dumbObjectsList;
    }

    override
    public string ToString()
    {
        return "tile[" + tileIndex_X + "][" + tileIndex_Y + ']';
    }
}