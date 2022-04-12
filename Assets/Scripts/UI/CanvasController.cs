using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using System;

public class CanvasController : MonoBehaviour
{
    Animator animator;
    [SerializeField]
    private Slider musicSlider, chunkSlider;

    
    private void Start() {
        animator = GetComponent<Animator>();
        GameManager.OnChangeGameState += ChangeGameState;
    }

    private void OnDestroy() {
        GameManager.OnChangeGameState -= ChangeGameState;
    }
    public void ReturnToGame(){
        GameManager.Instance.ChangeState(GameManager.GameState.INGAME);
    }
    public void ReturnToPause(){
        GameManager.Instance.ChangeState(GameManager.GameState.PAUSE);
        GameManager.Instance.SaveGame();
    }

    public void RefreshIndicators(){
        GameManager.Instance.RefreshSaveGameCachedData();
    }

    public void SaveGoMainMenu(){
        GameManager.Instance.SaveGame();
        GameManager.Instance.ChangeScene("Main Menu", Vector2.zero);
    }

    public void GoToOptions(){
        Debug.LogFormat("{0} music, {1} chunk ", GameManager.Instance.musicLvl, GameManager.Instance.chunkLvl);
        if(GameManager.GameState.PAUSE.Equals(GameManager.Instance.gameState)){
            if(musicSlider != null)
                musicSlider.value = GameManager.Instance.musicLvl;
            if(chunkSlider != null)
                chunkSlider.value = GameManager.Instance.chunkLvl;
        }
        GameManager.Instance.ChangeState(GameManager.GameState.OPTIONS);
    }
    public void ChangeGameState(GameManager.GameState newState){
        switch(newState){
            case GameManager.GameState.CINEMATICS:
                // TODO: this  
            break;
            case GameManager.GameState.DIALOGUE:
                animator.SetTrigger("dialogue");
            break;
            case GameManager.GameState.INGAME:
                animator.SetTrigger("ingame");
            break;
            case GameManager.GameState.PAUSE:
                animator.SetTrigger("pause");
            break;
            case GameManager.GameState.OPTIONS:
                animator.SetTrigger("options");
            break;
            case GameManager.GameState.CHANGESCENE:
                animator.SetTrigger("changeScene");
            break;
            case GameManager.GameState.GAMEOVER:
                animator.SetTrigger("gameover");
            break;
        }
    }

    public void ReloadScene(){
        animator.SetTrigger("changeScene");
    }
    private void ChangeScene(){
        GameManager.Instance.LoadScene();
    }

    public void ChangeTimeScale(float newScale){   
        Time.timeScale = newScale;
    }

    public void OnMusicValueChange(float newValue){
        GameManager.Instance.SetMusicLvl(newValue);
    }

    public void OnChunkValueChange(float newValue){
        GameManager.Instance.SetChunkLvl(newValue);
    }
}
