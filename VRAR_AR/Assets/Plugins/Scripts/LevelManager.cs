using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelManager : MonoBehaviour {

	public const float TILE_WIDTH = 2; //radius = 1 so diameter = 2
    public const float TILE_LENGTH = 1.73205f; //sqrt(3)
    public const float TILE_SCALE = 0.3f;
    
    public const float SQRT_THREE = 1.73205080757f;
    public const float TILE_SIZE = 1 / SQRT_THREE * TILE_SCALE;

    public Transform tilesParent;

    //(old tile)
    //const float TILE_WIDTH = 1.73205f; //sqrt(3)
    //const float TILE_LENGTH = 1f;


    //amount of seperate lvls shouldn't get that high so we just use a list we can itterate through
    List<VRAR_Level> vrarLevels = new List<VRAR_Level>();

    public static LevelManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Debug.Log("Warning: multiple " + this + " in scene!"); }
    }

    void Start ()
    {
        DontDestroyOnLoad(this.gameObject);
        //Create a GameStateManager instance
        GameStateManager gsm = GameStateManager.getInstance();

        //Create folders needed
        if (Application.isEditor)
        {//use the project location
            var folder = Directory.CreateDirectory(Application.dataPath + "/levels"); // returns a DirectoryInfo object
            Debug.Log(Application.dataPath);

            //Set editor camera to not be shit
            //moved to fallback manager
            //GameObject.FindGameObjectWithTag("EditorCamera").transform.position *= TILE_SCALE;
		}
		else
        {//use the user save location
            var folder = Directory.CreateDirectory(Application.persistentDataPath + "/levels"); // returns a DirectoryInfo object
            Debug.Log(Application.persistentDataPath);
        }

        //discoverLevels();
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
	public Vector3 getWorldPosFromTilePos(int xI, int yI)
	{
        float x = TILE_SIZE * (SQRT_THREE * xI + SQRT_THREE / 2 * yI);
        float y = TILE_SIZE * (3f/ 2f * yI);
        return tilesParent.position - new Vector3(x, 0, y);
    }

    public Vector3 getNeighborDistance(int dir)
    {

        Vector3 dirpos;
        dirpos = getWorldPosFromTilePos(VRAR_Level.axialDirections[dir].x, VRAR_Level.axialDirections[dir].y);
        return dirpos * TILE_SCALE;
    }

    /**
    * Gets the tile index position from ingame position
    **/
    public Vector2Int getTilePosFromWorldPos(Vector3 worldPos)
    {
        Vector3 pos = tilesParent.position - worldPos;
        //WORK IN PROGRESS NEEDS TESTING WOWOOIWO
        float x = (SQRT_THREE / 3f * pos.x - 1f / 3f * pos.z) / TILE_SIZE;
        float y = (2f / 3f * pos.z) / TILE_SIZE;
        
        int yI = (int)Mathf.Round(y);
        int xI = (int)Mathf.Round(x);

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

        vrarLevels.Clear();
        FileInfo[] fileInfo = dirInfo.GetFiles("*.*");
		foreach (FileInfo file in fileInfo)
		{
			//Debug.Log(file.Name);
			if(file.Name.EndsWith(".sav"))
			{
				//Debug.Log("ding :"+file.Name.IndexOf('#'));
				string x = file.Name.Substring(6, file.Name.IndexOf('#')-6);
				string y = file.Name.Substring(file.Name.IndexOf('#')+1, file.Name.IndexOf('.')-file.Name.IndexOf('#')-1);
				Debug.Log("found level :" + x + " " + y);
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

