using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class SaveGame : MonoBehaviour
{
    public SaveData data;

    private void Awake() {
        data = LoadGameData();
    }

    private void Start() {
        GameManager.Instance.SaveDataRefresh(this);
    }

    internal void UpdateSaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath 
                    + "/data.dat"); 
        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Game data saved!");
    }

    private SaveData LoadGameData()
    {
        if (File.Exists(Application.persistentDataPath 
                    + "/data.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = 
                    File.Open(Application.persistentDataPath 
                    + "/data.dat", FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            return data;
        }
        else
            Debug.LogError("There is no save data!, generating new dataFile");
        // if not exists we create a new saveGame from default values
        SaveData newSaveData = new SaveData();
        newSaveData.actualLife = 50 + (newSaveData.lifeBonus * 10);
        // Debug.Log("lang:" + newSaveData.language);
        data = newSaveData;
        UpdateSaveGame();
        return newSaveData;
    }

    [Serializable]
    public class SaveData{
        internal int[] hearts = new int[10]{0,0,0,0,0,0,0,0,0,0};
        internal int[] flowers = new int[10]{0,0,0,0,0,0,0,0,0,0};
        internal bool doubleJumpUnlocked = false;
        internal bool duckSlashUnlocked = false;
        internal string actualScene = string.Empty;
        internal float playerLocationX = 0;
        internal float playerLocationY = 0;
        internal bool vialTime = false;
        internal bool vialLife = false;
        internal string language = "EN";
        internal int attackBonus = 0;
        internal int lifeBonus = 0;
        internal int actualLife = 50;
        internal int basicAttack = 5;
        internal float musicLvl = 1;
        internal float chunkLvl = 0.75f;
        internal int coins =0;
        internal bool savedBool = false;
    }
}
