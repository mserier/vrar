using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    //This is the real gamemanager and it really manages the game


    public static GameManager Instance { get; private set; }

    
    public string defaultName;

    private LevelManager levelManager;
    private TileRenderer tileRenderer;

    private bool usingNetwork = false;

    void Awake()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            Debug.Log("Warning: multiple " + this + " in scene!");
            DestroyImmediate(this.gameObject);
            return;
        }

        Debug.Log("Made a new GameManager");
        DontDestroyOnLoad(Instance);

        gameObject.AddComponent<GamePlayManagerAR>();
        levelManager = gameObject.AddComponent<LevelManager>();
        tileRenderer = gameObject.AddComponent<TileRenderer>();
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    //Check if we are in the networked version of our game, otherwise do some standard things
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "_NetworkScene")
        {
            usingNetwork = true;
        } else
        {
            

            //If we are running on the phone we use AR otherwise we dont.
            if (Application.platform == RuntimePlatform.Android)
            {
                
            }
            else
            {
                //Disable vuforia
                //Destroy(GameObject.Find("ImageTarget"));
                GameObject.Find("ImageTarget").SetActive(false);
                Camera.main.GetComponent<VuforiaMonoBehaviour>().enabled = false;

                //Set the camera scale
                Camera.main.transform.position *= LevelManager.TILE_SCALE;
            }

            

            
            

            if (!usingNetwork)
            {

                //Spawn the level
                levelManager.discoverLevels();
                tileRenderer.Init();

                //Spawn the player
                BasePlayer player = new BasePlayer(defaultName, 0, 0);
                GamePlayManagerAR.instance.AddPlayer(player);
                GamePlayManagerAR.instance.setLocalPlayer(player);
                TileRenderer.instance.localPlayer = player;
                player.SpawnPlayer(new Vector2Int(0, 0));

                tileRenderer.spawnLevel(player.GetCurrentVec().x, player.GetCurrentVec().y);
            }            
        }
    }
}