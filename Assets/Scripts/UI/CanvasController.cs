using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasController : MonoBehaviour
{
    Animator animator;
    private void Start() {
        animator = GetComponent<Animator>();
    }
    private void Update() {
        if( GameManager.GameState.INGAME.Equals(GameManager.Instance.gameState)){
            if(Input.GetButtonDown("Submit")){
                animator.SetTrigger("pause");
            }
        }
    }
    public void ReturnToGame(){
        if(GameManager.GameState.OPTIONMENU.Equals(GameManager.Instance.gameState)){
            animator.SetTrigger("pause");
        }
    }
    public void GoToOptions(){
        if( GameManager.GameState.OPTIONMENU.Equals(GameManager.Instance.gameState)){
            animator.SetTrigger("options");
        }
    }
    public void ChangeGameState(GameManager.GameState newState){
        GameManager.Instance.gameState = newState;
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
