using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class VRAR_Level
{
    public VRAR_Tile levelTile;

    private int levelIconID = 0;

    public List<VRAR_Tile> vrarTiles = new List<VRAR_Tile>();

    Dictionary<Vector2Int, VRAR_Tile> vrarTileDict = new Dictionary<Vector2Int, VRAR_Tile>();

    public VRAR_Tile getTileFromIndexPos(int iX, int iY)
    {
        VRAR_Tile res = null;
        //res = vrarTileArray[iX+negOffsetX,iY+negOffsetY];

        Vector2Int key = new Vector2Int(iX, iY);
        if (vrarTileDict.ContainsKey(key))
            res = vrarTileDict[key];
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
     * Returns a list of tiles in a circle around the given coord (circle IS filled in)
     **/
    public List<VRAR_Tile> selectRadius(int xI, int yI, int radius)
    {
        //We go east from the given coord, and go in a circle around the hex tile, for every radius+1 the circle increases with 6 tiles.
        int circleAmount = 0;//= 6 * radius;
        for(int i=0;i<radius;i++)
        {
            circleAmount += (radius - i) * 6;
        }

        List<VRAR_Tile> result = new List<VRAR_Tile>(circleAmount);

        for(int i=0;i<radius;i++)
        {
            result.AddRange(selectCircle(xI, yI, i));
        }


        return result;
    }

    /**
     * Returns a list of tiles in a circle around the coord (circle IS NOT filled in)
     **/
    public List<VRAR_Tile> selectCircle(int xI, int yI, int radius)
    {
        int circleAmount = 6 * radius;
        List<VRAR_Tile> result = new List<VRAR_Tile>(circleAmount);


        /** I thought you said "weast." 
                    ,,,. *,,,,,,,.                   ((  (((((((((((   
                    .,,,((,,,,,,,,,      (((((((((       (**(      ((  
                     ,,,,,,,,,,,,,,,,.  ((        ((     (((       ((  
                     ,,,,,,,,,,/(((/,.,((          (      ((       (   
                     ,,,,,.///((((((/%,(            (       (     (    
                     .,,/((*,*((((((//%,    (             ((   (     
                 .,,.,,/(((/((/( ^//&&&&&*      (        (((    (     
                ,,,.*.,*(((((/&&&&&(&&&&& *        (             ((((((
                    ,,,^/((((/&&&&& ( &&&..,((((     (         ((      (
                   *   ,/((((*&&&&&, *&&&&&*            ((   ((        
                       (/((((((&&@&&&&&& *.  (         (       ((        (
                        *(((((/&&&&&/%^/((/%  .((((((    (((( (    
                        *((((((//&& *(((((((((*          (        ((   (
                        *(((((((((((((((((*...,..       (          (
                        *(((((((((*...,..,/(((((*      (             
                        ^/(((((,,.,,.,((((((((((*        (   (     (   
                   &    ^/((((*,,,./((((((((((((/%           (((     
                &       ^/((((..,((((((((((((((((/,                    
                        ^/(((((((((((((((((((((((((*                   
                       /,(((((((((((((((((((((((((((*                  
                       ,*((((((((((((((((((((((((((((/%                
                       ^/((((((((((((((((((((((((((((((*,              
                      ^/((((((((((((((((((((((((((((((((/%.            
             (( ((   .*(((((((((((((((((((((((((((((((((((/%           
            ((   /   *((((((((((((((((((((((((((^/((((((((((**         
            /% (/// *(((((((((((((((((((((((((((((((((((((((((/%**( */


        VRAR_Tile baseTile = getTileFromIndexPos(xI, yI);
        if(radius==0)
            result.Add(baseTile);
        //you need to go east for radius amount of times
        for (int i = 0; i < radius; i++){baseTile = getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 1);}
        //after going east for radius amount of times, you need to go south-west for radius amount of times.
        for (int i = 0; i < radius; i++){baseTile = getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 3);result.Add(baseTile);}
        //after going south-west, you need to go west for radius amount of times
        for (int i = 0; i < radius; i++){baseTile = (getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 4));result.Add(baseTile);}
        //after going west, you need to go north-west for radius amount of times
        for (int i = 0; i < radius; i++){baseTile = (getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 5));result.Add(baseTile);}
        //after going north-west, you need to go north-east for radius amount of times
        for (int i = 0; i < radius; i++){baseTile = (getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 0));result.Add(baseTile);}
        //after going north-east, you need to go east for radius amount of times
        for (int i = 0; i < radius; i++){baseTile = (getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 1));result.Add(baseTile);}
        //after going east, you need to go south-east for radius-1 amount of times
        for (int i = 0; i < (radius); i++){baseTile = (getAdjacentTile(baseTile.tileIndex_X, baseTile.tileIndex_Y, 2));result.Add(baseTile);}

        return result;
    }

    /**
     *Gets the adjacent tile of the given coord
     * dir:
        0: north east
        1: east
        2: south east
        3: south west
        4: west
        5: north west

        WARNING: returns fantom tile if the tile doesn't exist
     **/
    public VRAR_Tile getAdjacentTile(int xI, int yI, int dir)
    {
        VRAR_Tile res = null;
        /*(Note to math-heads; PLEASE SAVE US!!)*/
        switch(dir)
        {
            case 0://north-east
                if(yI%2==0)
                {
                    res = getTileFromIndexPos(xI, yI + 1);
                    if (res == null) { res = new VRAR_Tile(xI, yI+1, 69f); }
                }
                else
                {
                    res = getTileFromIndexPos(xI+1, yI + 1);
                    if (res == null) { res = new VRAR_Tile(xI+1, yI+1, 69f); }
                }
                break;
                
            case 1://east
                res =  getTileFromIndexPos(xI+1, yI);
                if (res == null) { res = new VRAR_Tile(xI+1, yI, 69f); }
                break;

            case 2://south-east
                if (yI % 2 == 0)
                {
                    res =  getTileFromIndexPos(xI, yI - 1);
                    if (res == null) { res = new VRAR_Tile(xI, yI-1, 69f); }
                }
                else
                {
                    res =  getTileFromIndexPos(xI + 1, yI - 1);
                    if (res == null) { res = new VRAR_Tile(xI+1, yI-1, 69f); }
                }
                break;

            case 3://south-west
                if (yI % 2 == 0)
                {
                    res =  getTileFromIndexPos(xI, yI - 1);
                    if (res == null) { res = new VRAR_Tile(xI, yI-1, 69f); }
                }
                else
                {
                    res =  getTileFromIndexPos(xI - 1, yI - 1);
                    if (res == null) { res = new VRAR_Tile(xI-1, yI-1, 69f); }
                }
                break;

            case 4://west
                res =  getTileFromIndexPos(xI - 1, yI);
                if (res == null) { res = new VRAR_Tile(xI-1, yI, 69f); }
                break;

            case 5://north-west
                if (yI % 2 == 0)
                {
                    res =  getTileFromIndexPos(xI, yI + 1);
                    if (res == null) { res = new VRAR_Tile(xI, yI+1, 69f); }
                }
                else
                {
                    res =  getTileFromIndexPos(xI - 1, yI + 1);
                    if (res == null) { res = new VRAR_Tile(xI-1, yI+1, 69f); }
                }
                break;

        }

        return res;

    }

    /**
     * (WARNING not used right now)
	 * Update a subtile of a lvl
	 **/
    public void tileUpdate(int xI, int yI)
	{
		//notify surrounding tiles of the change
		/*
		*/
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
    public float height_ { get; private set; }

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

    public void setHeight(float height)
    {
        hexObject.gameObject.isStatic = false;
        height_ = height;
        if (hexObject != null)
        {
            hexObject.localScale = new Vector3(1, height_, 1);
            
            //Use this when the tileboject origin is in the middle of the model instead of at the bottom-middle of the model
            //hexObject.transform.SetPositionAndRotationposition.Set(hexObject.localPosition.x, height_*4f, hexObject.localPosition.z);
            //hexObject.gameObject.GetComponent<Renderer>().material = null;
        }
        else
        {
            Debug.Log("eh");
        }
        hexObject.gameObject.isStatic = true;
    }

    override
    public string ToString()
    {
        return "tile[" + tileIndex_X + "][" + tileIndex_Y + ']'+" h["+height_+']';
    }
}