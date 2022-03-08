using UnityEngine;
using System.Collections;
using TMPro;
public class DialogUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textPanel;

    [SerializeField]
    private TypeWriterEffect typeWriterEffect;

    private bool dialogueOccupied = false;

    private void Start() {
        typeWriterEffect = GetComponent<TypeWriterEffect>();
        SignController.OnOpenDialogue += ShowDialogue;
    }

    public void ShowDialogue(DialogObject dialogObject){
        if(!dialogueOccupied){
            GameManager.Instance.ChangeState(GameManager.GameState.DIALOGUE);
            StartCoroutine(ShowDialogeStep(dialogObject));
        }
    }

    private IEnumerator ShowDialogeStep(DialogObject dialogObject){
        dialogueOccupied = true;
        textPanel.text = "";
        yield return new WaitForSeconds(1);
        foreach(string dialogue in dialogObject.Dialoge){
            yield return typeWriterEffect.Run(dialogue, textPanel);
            yield return new WaitUntil(() => Input.GetButton("Fire1"));
        }
        GameManager.Instance.ChangeState(GameManager.GameState.INGAME);
        yield return new WaitForSeconds(3);
        dialogueOccupied = false;
    }

}
