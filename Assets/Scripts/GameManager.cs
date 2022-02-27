using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    // perticles container
    public Transform particlesContainer;

    public  Dictionary<string, GameObjectsPool> GMPools = new Dictionary<string, GameObjectsPool>();

    private void Awake() {
        _instance = this;
        saveGame = GetComponent<SaveGame>();
        if(particlesContainer is null){
            var go = GameObject.FindGameObjectWithTag("SpecialGO/Particles");
            if(go != null)
                particlesContainer = go.transform;
        }
            
    }

    public void SaveGame(){
        if(saveGame != null)
            saveGame.UpdateSaveGame();
    }

    #region Pool GM Manager
        public void ReturnToPool(string name, GameObject gm){
            var nameFiltered = name.Replace("(Clone)", "").Trim();
            //Debug.LogFormat("Returning '{0}' Game Object to pool", nameFiltered);
            if(GMPools.ContainsKey(nameFiltered)){
                GMPools[nameFiltered].ReturnToPool(gm);
            }
        }

        public void RequestAndExecuteGameObject(string name, Vector3 position){
            if(!GMPools.ContainsKey(name)){
                //Debug.LogFormat("Creating pool for : {0}, actual Count {1}", name, GMPools.Count);
                UnityEngine.Object PartObject = (UnityEngine.Object)Resources.Load("Particles/" + name);
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
            }
        }
    #endregion

    #region Player property Getters

    public int GetPlayerTotalAttack(){
        return Random.Range(saveGame.data.basicAttack, saveGame.data.basicAttack + saveGame.data.attackBonus);
    }

    public int GetMaxLife(){
        return 50 + (saveGame.data.lifeBonus * 10);
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

    #endregion
}
