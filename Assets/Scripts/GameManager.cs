using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public enum GameState{
        INGAME, PAUSE, OPTIONS, CINEMATICS, DIALOGUE
    }
    private static GameManager _instance;
    
    public static GameManager Instance{
        get{
            if(_instance is null)
                Debug.LogError("Game Manager is NULL");
                return _instance;
        }
    }
    // savegame
    internal SaveGame saveGame;
    [SerializeField]
    internal GameState gameState;
    [SerializeField]
    // perticles container
    public Transform particlesContainer;
    internal float musicLvl =0;
    internal float chunkLvl =0; 
    public  Dictionary<string, GameObjectsPool> GMPools = new Dictionary<string, GameObjectsPool>();
    public delegate void HurtAction();
    public static event HurtAction OnHurt;
    public static event Action<GameState> OnChangeGameState = delegate { };

    private void Awake() {
        _instance = this;
        saveGame = GetComponent<SaveGame>();
        if(particlesContainer is null){
            var go = GameObject.FindGameObjectWithTag("SpecialGO/Particles");
            if(go != null)
                particlesContainer = go.transform;
        }
    }

    public void RefreshSaveGameCachedData(){
        musicLvl = saveGame.data.musicLvl;
        chunkLvl = saveGame.data.chunkLvl;
    }

    public void SaveGame(){
        if(saveGame != null){
            saveGame.data.musicLvl = musicLvl;
            saveGame.data.chunkLvl = chunkLvl;
            saveGame.UpdateSaveGame();
        }
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
            var names = nameAndPath.Split('/');
            if(names.Length > 0){
                var name = names[names.Length-1];
                if(!GMPools.ContainsKey(name)){
                    //Debug.LogFormat("Creating pool for : {0}, actual Count {1}", name, GMPools.Count);
                    UnityEngine.Object PartObject = (UnityEngine.Object)Resources.Load(nameAndPath);
                    if(PartObject != null){
                        GameObjectsPool GmPool = new GameObjectsPool();
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
        return 50 + (saveGame.data.lifeBonus * 10);
    }

    public void SetActualLife(int newLife){
        saveGame.data.actualLife = newLife;
        if(OnHurt != null)
            OnHurt();
    }

    public int GetActualLife(){
        return saveGame.data.actualLife;
    }
    public bool CanDoubleJump(){
        return saveGame.data.doubleJumpUnlocked;
    }

    public bool CanDuckSlash(){
        return saveGame.data.duckSlashUnlocked;
    }

    public bool CanBow(){
        return saveGame.data.bowUnlocked;
    }

    public int GetArrows(){
        return saveGame.data.arrows;
    }

    public void SetArrows(int newArrows){
        saveGame.data.arrows = newArrows;
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
