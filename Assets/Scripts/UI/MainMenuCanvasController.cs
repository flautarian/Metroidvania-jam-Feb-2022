using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenuCanvasController : MonoBehaviour
{
    private Animator animator;

    [SerializeField]
    private TMP_Text continueNewText;

    private void Start() {
        animator = GetComponent<Animator>();
        animator.SetTrigger("mainMenuEntrance");
    }

    private void Update() {
        continueNewText.text = GameManager.Instance.HasNewGame() ? "New game" : "Continue";
        if(Input.GetButton("Fire1"))
            animator.SetTrigger("skipEntrance");
    }

    public void QuitGame(){
        Application.Quit();
    }

    public void GoToGame(){
        if(GameManager.Instance.HasNewGame())
            GameManager.Instance.ChangeScene("start 1-1", new Vector2(50f, -6.5f));
        else
            GameManager.Instance.ChangeScene(GameManager.Instance.saveGame.data.actualScene,
             new Vector2(GameManager.Instance.saveGame.data.playerLocationX,
                 GameManager.Instance.saveGame.data.playerLocationY));
    }
}
