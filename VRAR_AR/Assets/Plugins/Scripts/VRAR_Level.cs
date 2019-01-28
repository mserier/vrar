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


    /**
     * Hex sides
     **/
    public const int EAST = 0;
    public const int NORTH_EAST = 1;
    public const int NORTH_WEST = 2;
    public const int WEST = 3;
    public const int SOUTH_WEST = 4;
    public const int SOUTH_EAST = 5;

    /**
     * Transposed Hex sides north and south (instead of east and west)
     **/
    public const int T_NORTH = 0;
    public const int T_SOUTH = 3;

    //This is flipped because we use a different axis system
    public static Vector2Int[] axialDirections = new Vector2Int[]
    {
        new Vector2Int(1, 0),   //east
        new Vector2Int(0, 1),   //north east
        new Vector2Int(-1, 1),  //north west
        new Vector2Int(-1, 0),  //west
        new Vector2Int(0, -1),  //south west
        new Vector2Int(1, -1),  //south east
    };


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
        levelTile = new VRAR_Tile(iX, iY);
        //createHexagonMap(40);

        //createLVLFile();////////(use this when you need to generate VRAR_Levels on the spot)
        loadLevel();
    }

    //Create a hexagon shaped map with random terrain
    public void createHexagonMap(int radius)
    {
        for (int x = -radius; x <= radius; x++)
        {
            int r1 = Mathf.Max(-radius, -x - radius);
            int r2 = Mathf.Min(radius, -x + radius);
            for (int y = r1; y <= r2; y++)
            {
                string terrain = "Mountain";
                VRAR_Tile tile;
                switch (Random.Range(0,6))
                {
                    case 0:
                        terrain = "Mountain";
                        tile = new VRAR_Tile(x, y, 1, terrain, false);
                        break;
                    default:
                        terrain = "Grass";
                        tile = new VRAR_Tile(x, y, 1, terrain, true);
                        break;
                }
                 
                vrarTileDict.Add(new Vector2Int(x, y), tile);
                vrarTiles.Add(tile);
            }
        }
    }

    public void saveLevel()
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
            //TODO prolly broke something by adding some extra variables here
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
                int id = tileOb.getObjectID();
                Vector3 position = tile.GetObjectPosition(id);
                tw.Write("," + position.x + "," + position.y + "," + position.z + ",");
                Vector3 scale = tile.GetScale(id);
                tw.Write(scale.x + ",");
                Quaternion rotation = tile.GetRotation(id);
                tw.Write(rotation.x + "," + rotation.y + "," + rotation.z + "," + rotation.w);

            }
            tw.Write(";"+tile.terrain + ";" + tile.walkable);
            tw.Write("\n");
        }

        tw.Flush();
        tw.Close();
    }

    public void loadLevel()
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
            if (splitted.Length < 4)
            {

            }
            
            else
            {

               

                VRAR_Tile tile = new VRAR_Tile(int.Parse(splitted[0]), int.Parse(splitted[1]), float.Parse(splitted[2]), splitted[5], bool.Parse(splitted[6]));
                BaseTileObject interactableObject = TileObjectManger.TILE_OBJECTS[int.Parse(splitted[3])];
                if (interactableObject.getInteractable())
                {
                    tile.setInteractableObject(interactableObject);
                    tile.SetWalkable(false);
                }
                if (splitted.Length>=8&&splitted[7] != "")
                {
                    tile.setInterior(TileObjectManger.INTERIOR_BASE_OBJECTS[int.Parse(splitted[7])]);
                    tile.SetWalkable(false);
                }
                if (splitted.Length >= 9 && splitted[8] != "")
                {
                    tile.setNPC(new NonPlayer(int.Parse(splitted[8]), new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y)));
                    tile.SetWalkable(false);
                }

                //Debug.Log("Reading lvl file interactable object :" + interactableObject.getName());
                if (splitted[4].Length > 0)
                {
                    string[] dumbObjectIDs = splitted[4].Split('<');
                    tile.SetWalkable(false);

                    foreach (string dumbString in dumbObjectIDs)
                    {
                        // Debug.Log(dumbObjectIDs.Length + " size");
                        //   Debug.Log(dumbString);
                        string[] splitty = dumbString.Split(',');
                        if (splitty.Length > 1)
                        {
                            // Debug.Log("Base tile id = " + dumbString[0]);
                            // Debug.Log("Splitty size = " + splitty.Length);
                            int id = int.Parse(splitty[0]);
                            float positionX = float.Parse(splitty[1]);
                            float positionY = float.Parse(splitty[2]);
                            float positionZ = float.Parse(splitty[3]);
                            Vector3 position = new Vector3(positionX, positionY, positionZ);
                            float scaleX = float.Parse(splitty[4]);
                            Vector3 scale = new Vector3(scaleX, scaleX, scaleX);
                            Quaternion rotation = new Quaternion(float.Parse(splitty[5]), float.Parse(splitty[6]), float.Parse(splitty[7]), float.Parse(splitty[8]));
                            /* Debug.Log("posx = " + positionX);
                             Debug.Log("posy = " + positionY);
                             Debug.Log("posz = " + positionZ);
                             Debug.Log("scalex = " + scaleX);
                             Debug.Log("scaley = " + scaleY);
                             Debug.Log("scalez = " + scaleZ);
                             Debug.Log("Next string is " + dumbObjectIDs[2]);
                            */
                            tile.AddPosition(id, position);
                            tile.AddRotation(id, rotation);
                            tile.AddScale(id, scale);
                            tile.addDumbObject(TileObjectManger.TILE_OBJECTS[id]);


                            if (id == TileObjectManger.SPAWN_POINT_ID)
                            {
                                //Debug.Log("Set the spawn points to walkable in a suboptimal way");
                                tile.SetWalkable(true);
                            }
                            
                        }
                    }
                }

                if (tile.terrain == "Water")
                {
                    //Debug.Log("Set water to no walkable in a suboptimal way");
                    tile.SetWalkable(false);
                }

                vrarTiles.Add(tile);
                vrarTileDict.Add(new Vector2Int(int.Parse(splitted[0]), int.Parse(splitted[1])), tile);
                //Debug.Log("added tile :" +tile);
            }
        }


        //if level empty, spawn default lvl tiles
        if (vrarTiles.Count == 0)
        {
            Debug.Log("HOOOOOO");
            int radius = Random.Range(1, 8);
            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radius * radius)
                    {
                        VRAR_Tile tile = new VRAR_Tile(x, y);
                        vrarTiles.Add(tile);
                        vrarTileDict.Add(new Vector2Int(x, y), tile);
                    }
                }
            }
        }
    }

    public VRAR_Tile addNewTile(VRAR_Tile tile)
    {
        Vector2Int key = new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y);

        if (!vrarTileDict.ContainsKey(key))
        {
            vrarTiles.Add(tile);
            vrarTileDict.Add(key, tile);
        }
        else
        {
            Debug.LogWarning("Someone tried to add a new empty tile to level :" + this);
        }
        return vrarTileDict[key];
    }

    public VRAR_Tile addNewTile(int x, int y)
    {
        Vector2Int key = new Vector2Int(x, y);

        if (!vrarTileDict.ContainsKey(key))
        {
            VRAR_Tile tile = new VRAR_Tile(x, y);
            vrarTiles.Add(tile);
            vrarTileDict.Add(key, tile);
        }
        else
        {
            Debug.LogWarning("Someone tried to add a new empty tile to level :" + this);
        }
        return vrarTileDict[key];
    }

    public VRAR_Tile removeTile(int x, int y)
    {
        VRAR_Tile res=null;
        Vector2Int key = new Vector2Int(x, y);
        if(vrarTileDict.ContainsKey(key))
        {
            //vrarTileDict[key].isPhantom = true;
            res = vrarTileDict[key];
            vrarTiles.Remove(res);
            vrarTileDict.Remove(key);
        }

        return res;
    }

    public VRAR_Tile getAdjacentTile(int xI, int yI, int dir)
    {
        VRAR_Tile result = getTileFromIndexPos(xI + axialDirections[dir].x, yI + axialDirections[dir].y);
        return result;
    }

    public List<VRAR_Tile> getWalkableNeighbours(int xI, int yI)
    {
        List<VRAR_Tile> result = new List<VRAR_Tile>();
        for (int dir = 0; dir < 6; dir++)
        {
            VRAR_Tile tile = getAdjacentTile(xI, yI, dir);
            if (tile != null && tile.walkable)
                result.Add(tile);
        }
        return result;
    }

    /**
     * Returns a list of tiles in a circle around the given coord (circle IS filled in)
     **/
    public List<VRAR_Tile> selectRadius(int xI, int yI, int radius)
    {
        List<VRAR_Tile> result = new List<VRAR_Tile>();
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
        List<VRAR_Tile> result = new List<VRAR_Tile>();
        if (radius == 0)
        {
            result.Add(getTileFromIndexPos(xI, yI));
            return result;
        }

        //We iterate through all the tile positions, if there is a tile at that position we add it to the list
        Vector2Int tilePos = new Vector2Int(xI + radius * axialDirections[SOUTH_WEST].x, yI + radius * axialDirections[SOUTH_WEST].y);
        for (int dir = 0; dir < 6; dir++)
        {
            for (int tiles = 0; tiles < radius; tiles++)
            {
                //Get the tile at the tileposition we are currently iterating, and add it to the list, unless its null
                VRAR_Tile tile = getTileFromIndexPos(tilePos.x, tilePos.y);
                if (tile != null)
                    result.Add(tile);

                //Go to the next tile position (neighbour in the diection we are doing right now
                tilePos += axialDirections[dir];
            }
        }
        return result;
    }


    public bool CheckWalkable(VRAR_Tile origin, VRAR_Tile destination, int radius)
    {
        List<VRAR_Tile> reachable = selectRadius(origin.tileIndex_X, origin.tileIndex_Y, radius);
        foreach(VRAR_Tile tile in reachable)
        {
            if(tile == destination)
            {
                return true;
            }
        }
        return false;
    }

    //Not sure if this works atm needs fixing sort of
    public List<VRAR_Tile> selectCircleEdge(int xI, int yI, int radius, int dir)
    {
        List<VRAR_Tile> result = new List<VRAR_Tile>();

        //We iterate through all the tile positions, if there is a tile at that position we add it to the list
        Vector2Int tilePos = new Vector2Int(xI + radius * axialDirections[dir].x, yI + radius * axialDirections[dir].y);
        int lineDir = dir + 2;
        if (lineDir > 5)
            lineDir =- 2;

        for (int tiles = 0; tiles < radius; tiles++)
        {
            //Get the tile at the tileposition we are currently iterating, and add it to the list, unless its null
            VRAR_Tile tile = getTileFromIndexPos(tilePos.x, tilePos.y);
            if (tile != null)
                result.Add(tile);

            //Go to the next tile position (neighbour in the diection we are doing right now
            tilePos += axialDirections[lineDir];
        }

        return result;
    }

    //DO pathfinding here? wowee
    public List<VRAR_Tile> findPath(VRAR_Tile startTile, VRAR_Tile endTile)
    {
        List<VRAR_Tile> result = new List<VRAR_Tile>();
        Dictionary<VRAR_Tile, VRAR_Tile> cameFrom = new Dictionary<VRAR_Tile, VRAR_Tile>();
        Queue<VRAR_Tile> frontier = new Queue<VRAR_Tile>();

        frontier.Enqueue(startTile);
        cameFrom[startTile] = startTile;

        while (frontier.Count > 0)
        {
            VRAR_Tile current = frontier.Dequeue();
            if (current.Equals(endTile))
                break;

            foreach (VRAR_Tile next in getWalkableNeighbours(current.tileIndex_X, current.tileIndex_Y))
            {
                if (!cameFrom.ContainsKey(next))
                {
                    frontier.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        //fill path array
        VRAR_Tile currentTile = endTile;        
        while (true)
        {
            result.Add(currentTile);
            currentTile = cameFrom[currentTile];
            //MAKE THIS BETTER
            if (currentTile.Equals(startTile))
            {
                result.Add(startTile);
                break;
            }
                         
        }
        result.Reverse();
        return result;
    }

    //converts a list of tiles into a list of directions return null if something went wrong
    public List<int> TilesToDirections(List<VRAR_Tile> tiles)
    {
        List<int> results = new List<int>(tiles.Count);

        for (int i = 0; i < tiles.Count - 1; i++)
        {
            int dir = getDirection(tiles[i], tiles[i + 1]);
            if (dir == int.MaxValue)
            {                
                return null;
            }

            results.Add(dir);
        }
        return results;
    }

    //Get the direction of endtile in relaton to starttile, return intmax if the tles are not neighbours
    private int getDirection(VRAR_Tile startTile, VRAR_Tile endTile)
    {
        Vector2Int difference = new Vector2Int(startTile.tileIndex_X - endTile.tileIndex_X, startTile.tileIndex_Y - endTile.tileIndex_Y);
        for(int i = 0; i < axialDirections.Length; i++)
        {
            if (difference == axialDirections[i])
            {
                return i;
            }
        }
        Debug.LogError("These tiles are not neighbours");
        return int.MaxValue;
    }

    /**
     * Gives the counter direction for the given adjacentTile direction
     **/
    public static int getCounterTile(int dir)
    {
        switch(dir)
        {
            case NORTH_EAST:
                return SOUTH_WEST;
            case EAST:
                return WEST;
            case SOUTH_EAST:
                return NORTH_WEST;
            case SOUTH_WEST:
                return NORTH_EAST;
            case WEST:
                return EAST;
            case NORTH_WEST:
                return SOUTH_EAST;
            default:
                return -1; 
        }
    }

    /**
     * The distance to the middle neighboring tile (from the middle of your tile)
     * (+x is right, +y is up and +z is forward)
     **/
    public static Vector3 getNeighborDistance(int dir)
    {
        Vector3 result = new Vector3();
        switch (getCounterTile(dir))
        {
            case NORTH_EAST:
                result = new Vector3(1f, 0, 3.464102f) - new Vector3(1.5f, 0, 4.330127f);
                break;
            case EAST:
                result = new Vector3(2.5f, 0, 4.330127f) - new Vector3(3.5f, 0, 4.330127f);
                break;
            case SOUTH_EAST:
                result = new Vector3(2.5f, 0, 4.330127f) - new Vector3(3f, 0, 3.464102f);
                break;
            case SOUTH_WEST:
                result = new Vector3(2.5f, 0, 4.330127f) - new Vector3(2f, 0, 3.464102f);
                break;
            case WEST:
                result = new Vector3(2.5f, 0, 4.330127f) - new Vector3(1.5f, 0, 4.330127f);
                break;
            case NORTH_WEST:
                result = new Vector3(2.5f, 0, 4.330127f) - new Vector3(2f, 0, 5.196153f);
                break;
            default:
                break;
        }
        result *= LevelManager.TILE_SCALE;
        return result;
        /*
        switch (dir)
        {
            case NORTH_EAST:
                return new Vector3(LevelManager.TILE_WIDTH/2,0,LevelManager.TILE_LENGTH) * LevelManager.TILE_SCALE;
            case EAST:
                return new Vector3(LevelManager.TILE_WIDTH, 0, 0) * LevelManager.TILE_SCALE;
            case SOUTH_EAST:
                return new Vector3(LevelManager.TILE_WIDTH / 2, 0, -LevelManager.TILE_LENGTH) * LevelManager.TILE_SCALE;
            case SOUTH_WEST:
                return new Vector3(-LevelManager.TILE_WIDTH/2, 0, -LevelManager.TILE_LENGTH) * LevelManager.TILE_SCALE;
            case WEST:
                return new Vector3(-LevelManager.TILE_WIDTH, 0, 0) * LevelManager.TILE_SCALE;
            case NORTH_WEST:
                return new Vector3(-LevelManager.TILE_WIDTH / 2, 0, LevelManager.TILE_LENGTH) * LevelManager.TILE_SCALE;
            default:
                return new Vector3();
        }
        */
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
