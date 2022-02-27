using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PoolManager : MonoBehaviour
{

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject[] prefabs;
        public int size;
    }

    [System.Serializable]
    public class ChunkPool
    {
        public string tag;
        public AudioClip[] prefabs;
        public int size;
    }

    #region Singleton

    public static PoolManager Instance;
    private Transform poolContainer;

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    /*public void PreparePoolDataFromLevel(List<LevelSettings.PoolLoader> poolsToLoad){
        fillPoolsWithGOInstances(poolsToLoad);
        initializePools();
    }

    private void fillPoolsWithGOInstances(List<LevelSettings.PoolLoader> poolsToLoad){
        pools.Clear();
        foreach(LevelSettings.PoolLoader pool in poolsToLoad){
            Pool p = new Pool();
            p.size = pool.size;
            p.tag = pool.tag;
            p.prefabs = new GameObject[pool.prefabs.Length];
            for(int i =0; i < pool.prefabs.Length; i++){
                UnityEngine.Object obj = (UnityEngine.Object)Resources.Load(pool.folder +"/"+ pool.prefabs[i]);
                if(obj != null) {
                    p.prefabs[i] = (GameObject)Instantiate(obj);
                    p.prefabs[i].SetActive(false);
                    p.prefabs[i].transform.parent = poolContainer;
                }
                else Debug.Log("Problems to load: " + pool.folder +"/"+ pool.prefabs[i]);
            }
            pools.Add(p);
        }
    }*/

    private void initializePools(){
        var goPc = GameObject.FindGameObjectWithTag("PoolContainer");
        if(goPc != null) poolContainer = goPc.transform;
        poolDictionary = new Dictionary<string, Queue<GameObject>>();
        foreach (Pool pool in pools)
        {
            Queue<GameObject> queueOfPool = new Queue<GameObject>();
            int prefabLoop = 0;
            for (int i = 0; i < pool.size; i++)
            {
                if (prefabLoop < pool.prefabs.Length - 1) prefabLoop++;
                else prefabLoop = 0;
                GameObject prefab = Instantiate(pool.prefabs[prefabLoop]);
                prefab.transform.parent = poolContainer;
                prefab.SetActive(false);
                queueOfPool.Enqueue(prefab);
            }
            poolDictionary.Add(pool.tag, queueOfPool);
        }
        Instance = this;
    }

    #endregion

    #region pool manager

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;

    public Dictionary<string, AudioClip> chunkDictionary;

    public GameObject SpawnFromPool( string tag, Vector3 position, Quaternion rotation, Transform origin)
    {
        if(poolDictionary != null) { 
            if (!poolDictionary.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " is not found on pool list");
                return null;
            }
            GameObject dequeueObject = poolDictionary[tag].Dequeue();
            if(dequeueObject != null)
            {
                dequeueObject.SetActive(true);
                dequeueObject.transform.position = position;
                dequeueObject.transform.rotation = rotation;
                if(origin != null)dequeueObject.transform.parent = origin;
            }
            return dequeueObject;
        }
        return null;
    }

    public AudioClip SpawnChunkFromPool( string tag)
    {
        if(chunkDictionary == null ) chunkDictionary = new Dictionary<string, AudioClip>(); 
        AudioClip clip = null;
        if (!chunkDictionary.ContainsKey(tag))
        {
            clip = Resources.Load<AudioClip>("Sounds/Chunks/" + tag);
            if(clip != null)
                chunkDictionary[tag] = clip;
            else{
                Debug.LogWarning("Pool with tag " + tag + " is not found on pool list");
            }
        }
        else clip = chunkDictionary[tag];
        return clip;
    }

    public AudioClip SpawnSongFromPool( string tag)
    {
        if(chunkDictionary == null ) chunkDictionary = new Dictionary<string, AudioClip>(); 
        AudioClip clip = null;
        if (!chunkDictionary.ContainsKey(tag))
        {
            clip = Resources.Load<AudioClip>("Sounds/Songs/" + tag);
            if(clip != null)
                chunkDictionary[tag] = clip;
            else{
                Debug.LogWarning("Pool with tag " + tag + " is not found on pool list");
            }
        }
        else clip = chunkDictionary[tag];
        return clip;
    }

    #endregion
}
