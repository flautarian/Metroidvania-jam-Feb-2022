using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public enum GameState{
        INGAME, PAUSE, OPTIONS, CINEMATICS, DIALOGUE, CHANGESCENE
    }
    public static GameManager Instance { get; private set; }
    // savegame
    internal SaveGame saveGame;
    [SerializeField]
    public GameState gameState;
    [SerializeField]
    // perticles container
    public Transform particlesContainer;
    internal float musicLvl =0;
    internal float chunkLvl =0; 
    internal int maxLife =1; 
    internal int actualLife =1;
    internal int actualCoins =0; 
    public  Dictionary<string, GameObjectsPool> GMPools = new Dictionary<string, GameObjectsPool>();
    public delegate void HurtAction();
    public static event HurtAction OnHurt;
    public static event Action<GameState> OnChangeGameState = delegate { };
    public static event Action<int> OnGainCoins = delegate { };
    private string nextScene = string.Empty;
    public Vector2 nextPlayerPosition = Vector2.zero;
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void SaveDataRefresh(SaveGame save){
        saveGame = save;
        if(saveGame != null){
            //saveGame.data.actualScene = nextScene;
            //saveGame.data.playerLocation = nextPlayerPosition;
            RefreshSaveGameCachedData();
        }
    }

    public void RefreshSaveGameCachedData(){
        musicLvl = saveGame.data.musicLvl;
        chunkLvl = saveGame.data.chunkLvl;
        maxLife = 50 + saveGame.data.lifeBonus * 10;
        actualLife = saveGame.data.actualLife;
        actualCoins = saveGame.data.coins;
        if(OnGainCoins != null)
            OnGainCoins(actualCoins);
        if(OnHurt != null)
            OnHurt();
    }

    public void SaveGame(){
        if(saveGame != null){
            saveGame.data.musicLvl = musicLvl;
            saveGame.data.chunkLvl = chunkLvl;
            saveGame.data.actualLife = actualLife;
            saveGame.data.coins = actualCoins;
            saveGame.data.savedBool = true;
            saveGame.UpdateSaveGame();
        }
    }

    public void ChangeScene(string nextScn, Vector2 nextPlayerPos){
        if(GMPools != null)
            GMPools.Clear();
        nextScene = nextScn;
        if(!nextScn.Equals("Main Menu")){
            nextPlayerPosition = nextPlayerPos;
            saveGame.data.actualScene = nextScene;
            saveGame.data.playerLocationX = nextPlayerPos.x;
            saveGame.data.playerLocationY = nextPlayerPos.y;
        }
        gameState = GameState.CHANGESCENE;
        OnChangeGameState(gameState);
    }

    public void LoadScene(){
        Debug.Log(nextScene);
        SceneManager.LoadScene(nextScene);
    }

    #region Pool GM Manager
        public void ReturnToPool(string name, GameObject gm){
            var nameFiltered = name.Replace("(Clone)", "").Trim();
            //Debug.LogFormat("Returning '{0}' Game Object to pool", nameFiltered);
            if(GMPools.ContainsKey(nameFiltered)){
                GMPools[nameFiltered].ReturnToPool(gm);
            }
        }

        public GameObject RequestAndExecuteGameObject(string nameAndPath, Vector3 position){
            if(particlesContainer == null){
                Debug.Log("Starting new particle relation");
                var go = GameObject.FindGameObjectWithTag("SpecialGO/Particles");
                if(go != null)
                particlesContainer = go.transform;
            }
            var names = nameAndPath.Split('/');
            if(names.Length > 0){
                var name = names[names.Length-1];
                if(!GMPools.ContainsKey(name)){
                    //Debug.LogFormat("Creating pool for : {0}, actual Count {1}", name, GMPools.Count);
                    UnityEngine.Object PartObject = (UnityEngine.Object)Resources.Load(nameAndPath);
                    if(PartObject != null){
                        GameObjectsPool GmPool = new GameObjectsPool();
                        GmPool.objects = new Queue<GameObject>();
                        GameObject prefab = (GameObject)GameObject.Instantiate(PartObject);
                        prefab.transform.parent = particlesContainer.transform;
                        prefab.SetActive(false);
                        GmPool.prefab = prefab;
                        GMPools.Add(name, GmPool);
                        
                    }
                }
                GameObject go = GMPools[name].Get();
                if(go != null){
                    go.SetActive(true);
                    go.transform.position = position;
                    return go;
                }
            }
            return null;
        }
    #endregion

    #region Player property Getters

    public int GetPlayerTotalAttack(){
        return UnityEngine.Random.Range(saveGame.data.basicAttack, saveGame.data.basicAttack + saveGame.data.attackBonus);
    }

    public int GetMaxLife(){
        return maxLife;
    }

    public bool HasNewGame(){
        return saveGame != null && !saveGame.data.savedBool;
    }

    public int GetCoins(){
        return actualCoins;
    }

    public bool UpdateCoins(int otherCoins){
        if(otherCoins < 0 && Math.Abs(otherCoins) > actualCoins)
            return false;
        actualCoins = (actualCoins + otherCoins > 9999 ? 9999 : actualCoins + otherCoins);
        OnGainCoins(actualCoins);
        return true;
    }

    public bool ModifyLife(int alterLife){
        actualLife = (actualLife + alterLife >= GetMaxLife() ?  GetMaxLife() : actualLife + alterLife);
        OnHurt();
        return actualLife > 0;
    }

    public int GetActualLife(){
        return actualLife;
    }
    public bool CanDoubleJump(){
        return saveGame.data.doubleJumpUnlocked;
    }

    public bool CanDuckSlash(){
        return saveGame.data.duckSlashUnlocked;
    }

    public void SetChunkLvl(float value){
        chunkLvl = value;
    }
    public void SetMusicLvl(float value){
        musicLvl = value;
    }
    #endregion

    #region GameStates consultor
    public bool IsIngame(){
        return GameState.INGAME.Equals(gameState);
    }

    public bool IsInCinematics(){
        return GameState.CINEMATICS.Equals(gameState);
    }

    public bool IsInMenu(){
        return GameState.PAUSE.Equals(gameState);
    }
    public void ChangeState(GameState newState){
        gameState = newState;
        OnChangeGameState(newState);
    }
    #endregion
}
