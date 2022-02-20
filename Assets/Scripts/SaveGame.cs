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
        GlobalVariables.Instance.prepareSceneWithSaveGameParametters();
    }

    internal void UpdateSaveGame()
    {
        BinaryFormatter bf = new BinaryFormatter(); 
        FileStream file = File.Create(Application.persistentDataPath 
                    + "/data.dat"); 
        bf.Serialize(file, data);
        file.Close();
        //Debug.Log("Game data saved!");
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
        Debug.Log("lang:" + newSaveData.language);
        data = newSaveData;
        UpdateSaveGame();
        return newSaveData;
    }

    [Serializable]
    public class SaveData{
        // volum de so de les opcions
        //internal float soundValue = 75;
        // volum de efectes de so de les opcions
        // 
        internal int[] hearts = new int[10]{0,0,0,0,0,0,0,0,0,0};
        internal int[] flowers = new int[10]{0,0,0,0,0,0,0,0,0,0};
        internal bool doubleJumpUnlocked = false;
        internal bool duckSlashUnlocked = false;
        internal bool scaleUnlocked = false; // TODO
        internal bool bowUnlocked = false; // TODO
        internal bool vialTime = false;
        internal bool vialLife = false;
        internal string language = "EN";
        bool savedBool;
    }
}
