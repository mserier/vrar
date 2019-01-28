using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class FallbackManagerScript : MonoBehaviour
{
    private FallbackManagerScript instance_;
    public bool isInFallBack { get; private set; }
    private GameStateManager gameStateManager;
    private bool firstTimeAwake = true;
    public Camera usedCamera { get; private set; }

    //-------------------------------------------------
    private static FallbackManagerScript _instance;
    public static FallbackManagerScript instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FallbackManagerScript>();
            }

            return _instance;
        }
    }


    //-------------------------------------------------
    void Awake()
    {
        _instance = this;

        if (firstTimeAwake)
        {
            firstTimeAwake = false;
        }
        else
        {
            /**
             * If the game is started in the Playing scene, asume we are testing and just load level 0
             **/
            if (SceneManager.GetActiveScene().name == "Playing")
            {
                usedCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
                print("used camera set :" + usedCamera);


                if (Application.platform != RuntimePlatform.Android)
                {
                    isInFallBack = true;
                    if (gameStateManager.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
                    {
                        GameObject.Find("ARCamera").SetActive(false);
                        GameObject.Find("ImageTarget").SetActive(false);
                        foreach (Camera cam in Resources.FindObjectsOfTypeAll(typeof(Camera)))
                        {
                            if (cam.name == "FallbackCamera")
                            {
                                cam.gameObject.SetActive(true);
                                usedCamera = cam;
                            }
                        }
                    }
                }
                else
                {
                    GameObject.Find("MoveButtons").SetActive(false);
                }
            }

        }
    }

    // Use this for initialization
    void Start ()
    {
        DontDestroyOnLoad(this);

        gameStateManager = GameStateManager.getInstance();

        /**
         * If the game is started in the Playing scene, asume we are testing and just load level 0
         **/
        if (SceneManager.GetActiveScene().name == "Playing")
        {
            usedCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
            print("used camera set :" + usedCamera);

            GameStateManager.getInstance().setGlobalStateIndex(GameStateManager.STATE_PLAYING);

            GameStateManager.getInstance().setGlobalStateIndex(GameStateManager.STATE_PLAYING);
            
            //add test player
            //GamePlayManagerAR.instance.setLocalPlayer(new BasePlayer("Jack", 20, 20, 8, Roles.RoleEnum.Alien, new Vector2Int(0, 0), 0, GamePlayManagerAR.instance));
            //GamePlayManagerAR.instance.AddPlayer(new BasePlayer("Freddy", 20, 20, 3, Roles.RoleEnum.Alien, new Vector2Int(2, 2), 1, GamePlayManagerAR.instance));

            if (Application.platform != RuntimePlatform.Android)
            {
                isInFallBack = true;

                if (gameStateManager.getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
                {
                    GameObject.Find("ARCamera").SetActive(false);
                    GameObject imageTarget = GameObject.Find("ImageTarget");
                    imageTarget.SetActive(false);
                    imageTarget.transform.GetChild(0).parent = null;
                    foreach (Camera cam in Resources.FindObjectsOfTypeAll(typeof(Camera)))
                    {
                        if (cam.name == "FallbackCamera")
                        {
                            cam.gameObject.SetActive(true);
                            usedCamera = cam;
                            //scale camera 
                            GameObject.FindGameObjectWithTag("EditorCamera").transform.position *= LevelManager.TILE_SCALE;
                        }
                    }
                }
            }
            else
            {
                GameObject.Find("MoveButtons").SetActive(false);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}


}
