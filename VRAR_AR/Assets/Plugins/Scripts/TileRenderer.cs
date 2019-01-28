// dnSpy decompiler from Assembly-CSharp-firstpass.dll
/**
 * Unity collab desided to destroy this class :(
 * 
 * Press F to pay respects to
 * Our class layout, constants, enums
 * and ofcourse our comments and spongebob references
 * 
 * But say hello! to unnecessarily constructors; namespaces; and hella weird print functions
 **/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TileRenderer : MonoBehaviour
{
    private static TileRenderer _instance;

    public static bool fadeWorldEdge;
    
    private Transform targetTransform;

    public BasePlayer localPlayer;

    private bool lateInit;

    private float speed = 2f;
    
    public static TileRenderer instance
    {
        get
        {
            if (TileRenderer._instance == null)
            {
                TileRenderer._instance = UnityEngine.Object.FindObjectOfType<TileRenderer>();
            }
            return TileRenderer._instance;
        }
    }

    private void Awake()
    {
        TileRenderer._instance = this;
    }

    private void Start()
    {
        //this.localPlayer = GamePlayManagerAR.instance.localPlayer;
        //this.lvlManager = base.gameObject.AddComponent<LevelManager>();
        this.lateInit = true;
    }

    //THis should probably be changed at some point but its better then late init, init?
    public void Init()
    {
        GameStateManager.getInstance().setCurrentLevel(LevelManager.Instance.getVRARLevels()[0]);
        //GameStateManager.getInstance().setCurrentLevel(this.lvlManager.getVRARLevels()[0]);
        GameStateManager.getInstance().setGlobalStateIndex(GameStateManager.STATE_PLAYING);

        //We make this from code now, why the fuck didnt we do that first??
        GameObject targetObject = new GameObject();
        targetTransform = targetObject.transform;
        targetObject.name = "LevelTilesParent";
        LevelManager.Instance.tilesParent = targetTransform;
        //this.spawnLevel();
    }

    public void spawnLevel(int x, int y)
    {
        VRAR_Level currentLevel = GameStateManager.getInstance().getCurrentLevel();
        List<VRAR_Tile> list;
        if (this.localPlayer == null)
        {
            MonoBehaviour.print("localPlayer null! ,spawning around 0,0 s5");
            //list = currentLevel.selectRadius(0, 0, 3);
            list = currentLevel.selectRadius(x, y, 3);
        }
        else
        {
            list = currentLevel.selectRadius(x, y, this.localPlayer.GetSight());
            //list = currentLevel.selectRadius(this.localPlayer.GetCurrentVec().x, this.localPlayer.GetCurrentVec().y, this.localPlayer.GetSight());
        }
        foreach (VRAR_Tile tile in list)
        {
            this.updateTile(tile);
            //StartCoroutine(DropIn(tile, 2f, null));
        }
        
    }

    public void updateTileHighlighter(Transform transform, bool active)
    {
        if(TileObjectManger.STATIC_HEX_HIGHLIGHTER==null)
        {
            TileObjectManger.STATIC_HEX_HIGHLIGHTER = Instantiate<GameObject>(TileObjectManger.STATIC_HEX_HIGHLIGHTER_PREFAB);
            TileObjectManger.STATIC_HEX_HIGHLIGHTER.transform.localScale = new Vector3(LevelManager.TILE_SCALE, LevelManager.TILE_SCALE * 0.1f, LevelManager.TILE_SCALE);
        }

        TileObjectManger.STATIC_HEX_HIGHLIGHTER.SetActive(active);
        TileObjectManger.STATIC_HEX_HIGHLIGHTER.transform.position = new Vector3(transform.position.x, transform.localScale.y*0.447f * LevelManager.TILE_SCALE, transform.position.z);

    }

    public void clearScene()
    {
        GameObject[] array = GameObject.FindGameObjectsWithTag("vrar_lvl");
        foreach (GameObject obj in array)
        {
            UnityEngine.Object.DestroyImmediate(obj);
        }
    }

    Animator m_Animator;

    public void walkLocalPlayer(int currentX, int currentY, int dir)
    {
        /*
        targetTransform.Translate(VRAR_Level.getNeighborDistance(VRAR_Level.getCounterTile(dir)));
        float y = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(currentX, currentY).hexObject.lossyScale.y * 0.447f - this.localPlayer.GetObject().transform.position.y;
        localPlayer.GetObject().transform.Translate(new Vector3(0f, y * LevelManager.TILE_SCALE, 0f));
        this.spawnLevel(currentX, currentY);
        this.updateSight(currentX, currentY);

        foreach (BasePlayer basePlayer in GamePlayManagerAR.instance.GetPlayers().Values)
        {
            if (basePlayer.GetPlayerId() != this.localPlayer.GetPlayerId())
            {
                this.walkExternalPlayer(currentX, currentY, basePlayer, VRAR_Level.getCounterTile(dir));
            }
        }*/
        
        if (this.localPlayerTranslateWrapper == null)
        {
            this.localPlayerTranslateWrapper = new TileRenderer.SlowTranslateWrapper(this.targetTransform);
        }
        this.SlowTranslate(this.localPlayerTranslateWrapper, VRAR_Level.getNeighborDistance(VRAR_Level.getCounterTile(dir)), speed * LevelManager.TILE_SCALE, () =>
        {
            if (GamePlayManagerAR.instance.localPlayer != null)
            {//update animation
                GamePlayManagerAR.instance.localPlayer.GetObject().transform.localEulerAngles = new Vector3(0, directionToRotation(dir), 0);
                m_Animator = GamePlayManagerAR.instance.localPlayer.GetObject().GetComponent<Animator>();
                m_Animator.SetBool("isWalkingLocally", true);
            }

            this.spawnLevel(currentX, currentY);
            this.updateSight(currentX, currentY);
        }, () =>{
            if (GamePlayManagerAR.instance.localPlayer != null)
            {//update animation

                m_Animator = GamePlayManagerAR.instance.localPlayer.GetObject().GetComponent<Animator>();
                m_Animator.SetBool("isWalkingLocally", false);
            }
            this.spawnLevel(currentX, currentY);
            this.updateSight(currentX, currentY);
        });

        //float y = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(this.localPlayer.GetCurrentVec().x, this.localPlayer.GetCurrentVec().y).hexObject.lossyScale.y * 0.447f - this.localPlayer.GetObject().transform.position.y;  
        //float y = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(currentX, currentY).hexObject.lossyScale.y * 0.447f - this.localPlayer.GetObject().transform.position.y;
        float y = localPlayer.GetObject().transform.position.y - GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(currentX, currentY).height_ * 0.1339f;
        this.SlowTranslatePlayer(this.localPlayer, new Vector3(0f, -y, 0f), speed * LevelManager.TILE_SCALE, false);
        //localPlayer.GetObject().transform.position = new Vector3(localPlayer.GetObject().transform.position.x, GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(currentX, currentY).height_ * LevelManager.TILE_SCALE * 0.447f, localPlayer.GetObject().transform.position.z);

        foreach (BasePlayer basePlayer in GamePlayManagerAR.instance.GetPlayers().Values)
        {
            if (basePlayer.GetPlayerId() != this.localPlayer.GetPlayerId())
            {
                this.walkExternalPlayer(currentX, currentY, basePlayer, VRAR_Level.getCounterTile(dir), false);
            }
        }
    }

    //todo y propably not correct
    public void teleportLocalPlayer(int tileX, int tileY)
    {
        float tx = LevelManager.TILE_SIZE * (LevelManager.SQRT_THREE * (float)tileX + LevelManager.SQRT_THREE / 2 * (float)tileY);
        float ty = LevelManager.TILE_SIZE * (3f / 2f * (float)tileY);

        Vector3 position = new Vector3(tx, 0f, ty);
        this.clearScene();
        this.targetTransform.position = position;
        this.spawnLevel(tileX, tileY);
        /*
        if (this.localPlayer.player.scene.name == null)
        {
            this.localPlayer.player = UnityEngine.Object.Instantiate<GameObject>(this.localPlayer.player);
        }
        */

        float y = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(this.localPlayer.GetCurrentVec().x, this.localPlayer.GetCurrentVec().y).hexObject.lossyScale.y * 0.447f;
        //this.localPlayer.player.transform.position = this.localPlayer.player.transform.position + new Vector3(0f, y, 0f);
    }

    public void walkExternalPlayer(int currentX, int currentY, BasePlayer player, int dir, bool animated)
    {
        if (animated)
        {
            player.GetObject().transform.localEulerAngles = new Vector3(0, directionToRotation(dir), 0);
            //player.GetObject().transform.position = new Vector3(player.GetObject().transform.position.x, GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(currentX, currentY).height_ * 0.1339f, player.GetObject().transform.position.z);
            player.GetObject().transform.position = new Vector3(player.GetObject().transform.position.x, GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(player.GetCurrentVec().x, player.GetCurrentVec().y).height_ * LevelManager.TILE_SCALE * 0.447f, player.GetObject().transform.position.z);
        }

        //print("walk external :" + player.GetName() + " local x :" + currentX + " local y :" + currentY);
        //List<VRAR_Tile> list = GameStateManager.getInstance().getCurrentLevel().selectRadius(localPlayer.GetCurrentVec().x, localPlayer.GetCurrentVec().y, localPlayer.GetSight());
        List<VRAR_Tile> list = GameStateManager.getInstance().getCurrentLevel().selectRadius(currentX, currentY, localPlayer.GetSight());
        VRAR_Tile tileFromIndexPos = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(player.GetCurrentVec().x, player.GetCurrentVec().y);
        if (list.Contains(tileFromIndexPos))
        {

            //Debug.Log("EXTERNAL we can see the other player");
            player.GetObject().GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            //float y = tileFromIndexPos.hexObject.lossyScale.y * 0.447f - player.GetObject().transform.position.y;
            //float y = player.GetObject().transform.position.y - GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(currentX, currentY).height_ * 0.1339f;
            //this.SlowTranslatePlayer(player, VRAR_Level.getNeighborDistance(dir) + new Vector3(0f, y, 0f), speed * LevelManager.TILE_SCALE);
            this.SlowTranslatePlayer(player, VRAR_Level.getNeighborDistance(dir), speed * LevelManager.TILE_SCALE, animated);

        }
        else
        {
            //Debug.Log("EXTERNAL we CANNOT see the other player");
            player.GetObject().GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            this.SlowTranslatePlayer(player, VRAR_Level.getNeighborDistance(dir), speed * LevelManager.TILE_SCALE, false);
        }
    }

    //todo y propably not correct
    public void teleportExternalPlayer(BasePlayer player)
    {
        MonoBehaviour.print(string.Concat(new object[]
        {
            "teleporting external player to :",
            player.GetCurrentVec().x,
            "  ",
            player.GetCurrentVec().y
        }));
        //Get listof our tiles
        List<VRAR_Tile> list = GameStateManager.getInstance().getCurrentLevel().selectRadius(localPlayer.GetCurrentVec().x, localPlayer.GetCurrentVec().y, localPlayer.GetSight());
        //get the tile of the external player
        VRAR_Tile tileFromIndexPos = GameStateManager.getInstance().getCurrentLevel().getTileFromIndexPos(player.GetCurrentVec().x, player.GetCurrentVec().y);
        //check if external playertile is in our list of tiles
        if (list.Contains(tileFromIndexPos))
        {
            if (player.GetObject().scene.name == null)
            {
                float y = tileFromIndexPos.hexObject.lossyScale.y * 0.447f;
                player.SpawnPlayer(player.GetCurrentVec());
                //player.player = UnityEngine.Object.Instantiate<GameObject>(player.player, this.lvlManager.getWorldPosFromTilePos(player.tileLocation.x, player.tileLocation.y) + new Vector3(0f, y, 0f), default(Quaternion));
            }
            else
            {
                float y2 = tileFromIndexPos.hexObject.lossyScale.y * 0.447f;
                player.GetObject().transform.position = LevelManager.Instance.getWorldPosFromTilePos(player.GetCurrentVec().x, player.GetCurrentVec().y) + new Vector3(0f, y2, 0f);
            }
            player.GetObject().GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        }        
        else
        {
            if (player.GetObject().scene.name == null)
            {
                player.SpawnPlayer(player.GetCurrentVec());
                //player.player = UnityEngine.Object.Instantiate<GameObject>(player.player, this.lvlManager.getWorldPosFromTilePos(player.tileLocation.x, player.tileLocation.y), default(Quaternion));
            }
            else
            {
                player.GetObject().transform.position = LevelManager.Instance.getWorldPosFromTilePos(player.GetCurrentVec().x, player.GetCurrentVec().y);
            }
            player.GetObject().GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }
        MonoBehaviour.print("external player is standing on :" + player.GetObject().transform.position);
    }

    private void SlowTranslate(TileRenderer.SlowTranslateWrapper objectToMove, Vector3 translation, float speed, UnityAction stepCallBack, UnityAction endCallback)
    {
        if (objectToMove.translating)
        {
            objectToMove.translations.Enqueue(this.SlowTranslateCoroutine(objectToMove, translation, speed, stepCallBack, endCallback));
        }
        else
        {
            base.StartCoroutine(this.SlowTranslateCoroutine(objectToMove, translation, speed, stepCallBack, endCallback));
        }
    }

    private IEnumerator SlowTranslateCoroutine(TileRenderer.SlowTranslateWrapper objectToMove, Vector3 translation, float speed, UnityAction stepCallBack, UnityAction endCallback)
    {
        if (stepCallBack != null)
        {
            stepCallBack();
        }

        objectToMove.translating = true;
        Vector3 endposition = objectToMove.transform.position + translation;
        float stepsize = speed / (objectToMove.transform.position - endposition).magnitude * Time.fixedDeltaTime;
        float distance = Vector3.Distance(objectToMove.transform.position, endposition);
        float moved = 0f;
        while (distance - moved >= 0f)
        {        
            objectToMove.transform.Translate(translation * stepsize, Space.World);
            moved += (translation * stepsize).magnitude;

            //This is a secret update to the water material, i uses world coordinates so we need to update its offset while we are moving
            Vector2 textureOffset = new Vector2(distance - moved, 0);
            //TileObjectManger.getMaterial("Water").mainTextureOffset = -textureOffset;

            yield return new WaitForFixedUpdate();
        }
        objectToMove.transform.position = endposition;
        if (objectToMove.translations.Count > 0)
        { 
            base.StartCoroutine(objectToMove.translations.Dequeue());
        }
        else
        {
            objectToMove.translating = false;

            if (endCallback != null)
            {
                endCallback();
            }
        }


        yield break;
    }

    private void SlowTranslatePlayer(BasePlayer objectToMove, Vector3 translation, float speed, bool animated)
    {
        if (objectToMove.translating)
        {
            objectToMove.translations.Push(this.SlowTranslatePlayerCoroutine(objectToMove, translation, speed, animated));
            MonoBehaviour.print(string.Concat(new object[]
            {
                "added translation to queue :",
                objectToMove.GetName(),
                " queue count :",
                objectToMove.translations.Count
            }));
        }
        else
        {
            base.StartCoroutine(this.SlowTranslatePlayerCoroutine(objectToMove, translation, speed, animated));
        }
    }

    private IEnumerator SlowTranslatePlayerCoroutine(BasePlayer objectToMove, Vector3 translation, float speed, bool animated)
    {
        if (animated)
        {
            //objectToMove.GetObject().transform.localEulerAngles = new Vector3(0, directionToRotation(dir), 0);
            m_Animator = objectToMove.GetObject().GetComponent<Animator>();
            m_Animator.SetBool("isWalkingLocally", true);
        }

        objectToMove.translating = true;
        Vector3 endposition = objectToMove.GetObject().transform.position + translation;
        float stepsize = speed / (objectToMove.GetObject().transform.position - endposition).magnitude * Time.fixedDeltaTime;
        float distance = Vector3.Distance(objectToMove.GetObject().transform.position, endposition);
        float moved = 0f;
        while (distance - moved > 0f)
        {        
            objectToMove.GetObject().transform.Translate(translation * stepsize, Space.World);
            moved += (translation * stepsize).magnitude;
            yield return new WaitForFixedUpdate();
        }
        objectToMove.GetObject().transform.position = endposition;
        if (objectToMove.translations.Count > 0)
        {
            base.StartCoroutine(objectToMove.translations.Pop());
        }
        else
        {
            objectToMove.translating = false;

            if (animated)
            {
                m_Animator = objectToMove.GetObject().GetComponent<Animator>();
                m_Animator.SetBool("isWalkingLocally", false);
            }
        }
        yield break;
    }

    private void updateSight(int x, int y)
    {

        //List<VRAR_Tile> list = GameStateManager.getInstance().getCurrentLevel().selectCircle(this.localPlayer.GetCurrentVec().x, this.localPlayer.GetCurrentVec().y, this.localPlayer.GetSight());
        List<VRAR_Tile> list = GameStateManager.getInstance().getCurrentLevel().selectCircle(x, y, this.localPlayer.GetSight());
        foreach (VRAR_Tile vrar_Tile in list)
        {
            if (vrar_Tile.tileGameObject != null)
            {
                UnityEngine.Object.DestroyImmediate(vrar_Tile.tileGameObject.gameObject);
            }
        }
    }

    public void updateTile(VRAR_Tile tile)
    {
        if (tile != null)
        {
            if (GameStateManager.getInstance().getGlobalStateIndex() == GameStateManager.STATE_PLAYING)
            {
                if (tile.tileGameObject == null)
                {
                    /*
                    tile.tileGameObject = new GameObject(tile.ToString())
                    {
                        tag = "vrar_lvl",
                        transform =
                        {
                            position = LevelManager.Instance.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y),
                            parent = this.targetTransform
                        }
                    }.transform;*/
                    tile.tileGameObject = new GameObject(tile.ToString()).transform;
                    tile.tileGameObject.tag = "vrar_lvl";
                    tile.tileGameObject.transform.position = LevelManager.Instance.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y);
                    tile.tileGameObject.transform.parent = this.targetTransform;

                    //tile.tileGameObject.localScale = new Vector3(LevelManager.TILE_SCALE, LevelManager.TILE_SCALE, LevelManager.TILE_SCALE);
                    StartCoroutine(ScaleIn(tile.tileGameObject, new Vector3(LevelManager.TILE_SCALE, LevelManager.TILE_SCALE, LevelManager.TILE_SCALE), 0.5f, true, null));
                } else
                {
                    //THIS GOT ADDED BY ME (RONALD) TO PREVENT DUPLICATE OBJECTS I DONT THINK IT BREAKS ANYTHING
                    return;
                }

                Transform tileGameObject = tile.tileGameObject;
                IEnumerator enumerator = tileGameObject.GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        object obj = enumerator.Current;
                        Transform transform = (Transform)obj;
                    }
                }
                finally
                {
                    IDisposable disposable;
                    if ((disposable = (enumerator as IDisposable)) != null)
                    {
                        disposable.Dispose();
                    }
                }

                Transform transform2 = UnityEngine.Object.Instantiate<Transform>(TileObjectManger.STATIC_HEX_PREFAB);
                transform2.parent = tileGameObject;
                transform2.localPosition = new Vector3(0f, 0f, 0f);
                tile.hexObject = transform2;
                transform2.gameObject.GetComponent<Renderer>().material = TileObjectManger.getMaterial(tile.terrain);
                StartCoroutine(ScaleIn(tile.hexObject, new Vector3( 1f, tile.height_, 1f), 0.5f, true, null));
                //transform2.localScale = new Vector3(1f, tile.height_, 1f);

                //Water gets special treatment, its a bit lower scale
                if (tile.terrain == "Water")
                    transform2.gameObject.transform.localScale = new Vector3(1f, tile.height_ * 0.7f, 1f);

                
                if (TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab() != null)
                {
                    Transform transform3 = UnityEngine.Object.Instantiate<Transform>(TileObjectManger.TILE_OBJECTS[tile.getInteractableObject().getObjectID()].getPrefab(), LevelManager.Instance.getWorldPosFromTilePos(tile.tileIndex_X, tile.tileIndex_Y), default(Quaternion));
                    transform3.parent = tileGameObject.transform;
                    transform3.name = tile.getInteractableObject().getObjectID().ToString();
                }
                foreach (BaseTileObject baseTileObject in tile.getDumbObjectsList())
                {
                    if (baseTileObject.getPrefab() != null)
                    {
                        int objectID = baseTileObject.getObjectID();
                        Transform transform4 = UnityEngine.Object.Instantiate<Transform>(baseTileObject.getPrefab());
                        transform4.localScale = tile.GetScale(objectID);
                        transform4.position = new Vector3(0f, tile.height_ * 0.1f * 4.47f, 0f);
                        transform4.SetParent(tileGameObject.transform, false);
                        transform4.name = baseTileObject.getObjectID().ToString();
                    }
                    else
                    {
                        UnityEngine.Debug.Log("Prefab is null");
                    }
                }
                if (tile.getInterior() != null)
                {
                    Transform interOjbect = Instantiate(tile.getInterior().getPrefab());
                    interOjbect.position = new Vector3(0f, tile.height_ * 0.1f * 4.47f, 0f);
                    interOjbect.SetParent(tileGameObject.transform, false);
                    interOjbect.name = tile.getInterior().getName();
                }
                if (tile.getNPC() != null)
                {
                    //Debug.Log("SPAWNING AN NPC AT: " + tile.tileIndex_X + "   " + tile.tileIndex_Y);
                    Transform npcObject = tile.getNPC().SpawnNPC(new Vector2Int(tile.tileIndex_X, tile.tileIndex_Y)).transform;
                    npcObject.position = new Vector3(0f, tile.height_ * 0.1f * 4.47f, 0f);
                    npcObject.SetParent(tileGameObject.transform, false);
                    npcObject.name = tile.getNPC().ToString();
                }
            }
            else
            {
                UnityEngine.Debug.LogError("TileRenderer.updateTile() called while game is in invalid state. State should be GameStateManager.STATE_LEVEL_EDITOR while the game is in :" + GameStateManager.getInstance().getGlobalStateIndex());
            }
        }
    }

    private TileRenderer.SlowTranslateWrapper localPlayerTranslateWrapper;

    public class SlowTranslateWrapper
    {
        public SlowTranslateWrapper(Transform transform)
        {
            this.transform = transform;
        }

        public Queue<IEnumerator> translations = new Queue<IEnumerator>(5);

        public bool translating;

        public Transform transform;
    }

    public void returnFromInterior()
    {
        StartCoroutine(FadeOut(Color.black, 0.75f, false, () =>
        {
            clearScene();
            spawnLevel(GamePlayManagerAR.instance.localPlayer.GetCurrentVec().x, GamePlayManagerAR.instance.localPlayer.GetCurrentVec().y);
            Camera.main.transform.position = new Vector3(0, 2.4f, -1.8f);
            StartCoroutine(FadeOut(Color.black, 0.75f, true, null));
        }));
    }

    public void loadInterior(VRAR_Interior_Base interior_base)
    {
        StartCoroutine(FadeOut(Color.black, 0.75f, false,() =>
        {
            clearScene();
            Transform transform = Instantiate(TileObjectManger.INTERIOR_BASE_OBJECTS[interior_base.getObjectID()].getInteriorModelPrefab());
            //FallbackManagerScript.instance.usedCamera.transform.position = new Vector3();
            Camera.main.transform.position = new Vector3(0, 0.5f, 0);

            StartCoroutine(FadeOut(Color.black, 0.75f, true, null));
        }));
    }
    
    private IEnumerator FadeOut(Color color, float fadeTime, bool fadeIN, UnityAction callback)
    {
        if (TileObjectManger.FADE_OUT_OVERLAY_IMAGE != null)
        {
            float alphaStep = Time.fixedDeltaTime / fadeTime;
            float timer = 0f;
            if (fadeIN)
            {
                color.a = 1f;
                alphaStep = -alphaStep;
            }
            else
            {
                color.a = 0f;
            }
            while (timer <= fadeTime)
            {
                timer += Time.fixedDeltaTime;
                color.a += alphaStep;
                TileObjectManger.FADE_OUT_OVERLAY_IMAGE.color = color;
                yield return new WaitForFixedUpdate();
            }


            if (fadeIN)
            {
                color.a = 0f;
            }
            else
            {
                color.a = 1f;
            }
        }

        if(callback!= null)
        {
            callback();
        }
        yield break;

    }

    public float directionToRotation(int dir)
    {
        float res = 0f;
        switch(dir)
        {
            case VRAR_Level.EAST:
                res = 90;
                break;
            case VRAR_Level.NORTH_EAST:
                res = 45;
                break;
            case VRAR_Level.NORTH_WEST:
                res = 315;
                break;
            case VRAR_Level.WEST:
                res = 270;
                break;
            case VRAR_Level.SOUTH_WEST:
                res = 225;
                break;
            case VRAR_Level.SOUTH_EAST:
                res = 135;
                break;
        }
        return res;
        /*
    public const int EAST = 0;
    public const int NORTH_EAST = 1;
    public const int NORTH_WEST = 2;
    public const int WEST = 3;
    public const int SOUTH_WEST = 4;
    public const int SOUTH_EAST = 5;*/
    }



    private IEnumerator ScaleIn(Transform transform, Vector3 size, float scaleTime, bool scaleIn, UnityAction callback)
    {
        if (transform != null)
        {
            int stepCount = (int)(scaleTime / Time.fixedDeltaTime);
            int stepIndex = 0;

            //float step = Time.fixedDeltaTime / scaleTime;
            float step = size.x / stepCount;
            //print("step is :" + step + " scaleTime :" + scaleTime + "wh :" + scaleTime / Time.fixedDeltaTime + " fiDelta :" + Time.fixedDeltaTime);
            Vector3 alphaStep = new Vector3(step, step, step);
            float timer = 0f;
            if (scaleIn)
            {
                transform.localScale = new Vector3();
            }
            else
            {
                transform.localScale = size;
                alphaStep = -alphaStep;
            }
            //while (timer <= scaleTime)
            while (stepIndex <= stepCount)
            {
                stepIndex++;
                //timer += Time.fixedDeltaTime;
                if (transform != null)
                {
                    transform.localScale += alphaStep;
                }
                else
                {
                    break;
                }
                yield return new WaitForFixedUpdate();
            }


            if (transform != null)
            {
                if (scaleIn)
                {
                    transform.localScale = size;
                }
                else
                {
                    transform.localScale = new Vector3();
                }
            }

            if (callback != null)
            {
                callback();
            }
            yield break;
        }

    }

    private const float fallHeight = 10f;
    public IEnumerator DropIn(VRAR_Tile tile, float dropTime, UnityAction callback)
    {
        if (tile.tileGameObject != null)
        {
            tile.tileGameObject.Translate(new Vector3(0,fallHeight*LevelManager.TILE_SCALE));

            int stepCount = (int)(dropTime / Time.fixedDeltaTime);
            int stepIndex = 0;

            //float step = Time.fixedDeltaTime / scaleTime;
            float step = fallHeight / stepCount;
            //print("step is :" + step + " scaleTime :" + scaleTime + "wh :" + scaleTime / Time.fixedDeltaTime + " fiDelta :" + Time.fixedDeltaTime);
            Vector3 alphaStep = new Vector3(0, -step * LevelManager.TILE_SCALE, 0);
            //if (scaleIn)
            {
                //transform.localScale = new Vector3();
            }
            //else
            {
                //transform.localScale = size;
                //alphaStep = -alphaStep;
            }
            //while (timer <= scaleTime)
            while (stepIndex <= stepCount)
            {
                stepIndex++;
                //timer += Time.fixedDeltaTime;
                if (tile.tileGameObject != null)
                {
                    tile.tileGameObject.Translate(alphaStep);
                }
                else
                {
                    break;
                }
                yield return new WaitForFixedUpdate();
            }


            //if (transform != null)
            {
                //if (scaleIn)
                {
                    //transform.localScale = size;
                }
                //else
                {
                    //transform.localScale = new Vector3();
                }
            }

            tile.tileGameObject.position = new Vector3(tile.tileGameObject.position.x, 0, tile.tileGameObject.position.z);

            if (callback != null)
            {
                callback();
            }
            yield break;
        }

    }
}
