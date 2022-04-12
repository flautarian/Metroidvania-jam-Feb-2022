using UnityEngine;
using System.Collections;
using TMPro;
public class DialogUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textPanel;

    [SerializeField]
    private TypeWriterEffect typeWriterEffect;

    private ResponseHandler responseHandler;

    internal bool dialogueOccupied = false;

    private void Start() {
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        responseHandler = GetComponent<ResponseHandler>();
        SignController.OnOpenDialogue += ShowDialogue;
        ItemController.OnOpenDialogue += ShowDialogue;
    }

    private void OnDestroy() {
        SignController.OnOpenDialogue -= ShowDialogue;      
        ItemController.OnOpenDialogue -= ShowDialogue;  
    }

    public void ShowDialogue(DialogObject dialogObject){
        if(!dialogueOccupied){
            if(GameManager.Instance.gameState != GameManager.GameState.DIALOGUE)
                GameManager.Instance.ChangeState(GameManager.GameState.DIALOGUE);
            StartCoroutine(ShowDialogeStep(dialogObject));
        }
    }

    private IEnumerator ShowDialogeStep(DialogObject dialogObject){
        dialogueOccupied = true;
        textPanel.text = string.Empty;
        yield return new WaitForSeconds(1);
        for(int i =0; i < dialogObject.Dialoge.Length; i++){
            yield return typeWriterEffect.Run(dialogObject.Dialoge[i], textPanel);
            yield return new WaitUntil(() => Input.GetButton("Fire1") || Input.GetAxisRaw("Horizontal") != 0);
            if(i == dialogObject.Dialoge.Length -1 && dialogObject.HasResponses || Input.GetAxisRaw("Horizontal") != 0)
                break;
        }

        if(dialogObject.HasResponses && Input.GetAxisRaw("Horizontal") == 0){
            responseHandler.ShowResponses(dialogObject.Responses);
        }
        else{
            GameManager.Instance.ChangeState(GameManager.GameState.INGAME);
            yield return new WaitForSeconds(3);
            dialogueOccupied = false;
        }
    }

}
