using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariables : MonoBehaviour
{
    public static GlobalVariables Instance{get; private set;}

    internal SaveGame saveGame;
    private void Awake() {
        Instance = this;
        saveGame = GetComponent<SaveGame>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void prepareSceneWithSaveGameParametters(){
        
    }
}
