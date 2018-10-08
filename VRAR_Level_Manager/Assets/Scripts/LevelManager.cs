using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelManager : MonoBehaviour {

	const float TILE_WIDTH = 2; //radius = 1 so diameter = 2
	const float TILE_LENGTH = 1.73205f; //sqrt(3)

    //(old tile)
    //const float TILE_WIDTH = 1.73205f; //sqrt(3)
    //const float TILE_LENGTH = 1f;


    //amount of seperate lvls shouldn't get that high so we just use a list we can itterate through
    List<VRAR_Level> vrarLevels = new List<VRAR_Level>();

    void Start () {
        //Create a GameStateManager instance
        GameStateManager gsm = new GameStateManager();

        //Create folders needed
        if (Application.isEditor)
        {//use the project location
            var folder = Directory.CreateDirectory(Application.dataPath + "/levels"); // returns a DirectoryInfo object
            Debug.Log(Application.dataPath);
		}
		else
        {//use the user save location
            var folder = Directory.CreateDirectory(Application.persistentDataPath + "/levels"); // returns a DirectoryInfo object
            Debug.Log(Application.persistentDataPath);
        }

        discoverLevels();
    }

	void Update ()
	{

	}

    public List<VRAR_Level> getVRARLevels()
    {
        return vrarLevels;
    }
		

	/**
	 * Gets the ingame position from a tile index position
	 **/
	public Vector3 getWorldPosFromTilePos(int xI, int yI, float scale)
	{
		return new Vector3(xI*TILE_WIDTH+(yI%2*(TILE_WIDTH/2)),1, yI*TILE_LENGTH);//Random.Range(-0.25f, 0.25f)
        //return new Vector3(xI * TILE_WIDTH + (yI % 2 * (TILE_WIDTH / 2)), scale / 4f, yI * TILE_LENGTH);//Random.Range(-0.25f, 0.25f)
    }

	/**
	 * Gets the tile index position from ingame position
	 **/
	public Vector2Int getTilePosFromWorldPos(Vector3 worldPos)
	{
        int yI = (int)Mathf.Round(worldPos.z / TILE_LENGTH);
        int xI = (int)Mathf.Round(worldPos.x / TILE_WIDTH - (yI % 2f * (TILE_WIDTH / 4f)));

        return new Vector2Int(xI, yI);
	}

    /**
     * Gets the level object from the tileposition of the CAMPAIGN OVERVIEW!
     * Do NOT attempt to use this method from inside a level
     **/
	public VRAR_Level getLevelObjectFromTilePos(Vector2Int index)
	{
		foreach (VRAR_Level lvl in vrarLevels)
		{
			if (lvl.levelTile.tileIndex_X == index.x && lvl.levelTile.tileIndex_Y == index.y)
			{
				return lvl;
			}
		}

		return null;
	}

	/**
	 * Searches the level directory and find the available levels
	 **/
	public void discoverLevels()
	{
		
		DirectoryInfo dirInfo;
		if (Application.isEditor)
		{//use the project location
			dirInfo = new DirectoryInfo(Application.dataPath+"/levels");
		}
		else 
		{//use the user save location
			dirInfo = new DirectoryInfo(Application.persistentDataPath+"/levels");
		}

		FileInfo[] fileInfo = dirInfo.GetFiles("*.*");
		foreach (FileInfo file in fileInfo)
		{
			//Debug.Log(file.Name);
			if(file.Name.EndsWith(".sav"))
			{
				//Debug.Log("ding :"+file.Name.IndexOf('#'));
				string x = file.Name.Substring(6, file.Name.IndexOf('#')-6);
				string y = file.Name.Substring(file.Name.IndexOf('#')+1, file.Name.IndexOf('.')-file.Name.IndexOf('#')-1);
				//Debug.Log("found level :" + x + " " + y);
				vrarLevels.Add(new VRAR_Level(int.Parse(x),int.Parse(y)));

			}
		}

        /* gen square
		for (int x = 0; x < 10; x++)
		{
			for (int y = 0; y < 10; y++)
			{
				VRAR_Level lvl = new VRAR_Level(x, y);
				vrarLevels.Add(lvl);
			}
		}*/

		/* gen circle
		int radius = 5;
		for (int y = -radius; y <= radius; y++)
		{
			for (int x = -radius; x <= radius; x++)
			{
				if (x * x + y * y <= radius * radius)
				{
					vrarLevels.Add(new VRAR_Level(x,y));
				}
			}
		}*/
	}



}

