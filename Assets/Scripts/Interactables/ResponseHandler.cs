using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;


public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;

    private DialogUI dialogUI;
    private bool btnFocused = false;

    private List<GameObject> tempResponseButton = new List<GameObject>();

    private void Start() {
        dialogUI = GetComponent<DialogUI>();
    }

    public void ShowResponses(Response[] responses){
        float responseBoxHeight =0;
        foreach(Response r in responses){
            GameObject responseBtn = Instantiate(responseButtonTemplate.gameObject, responseContainer);
            responseBtn.SetActive(true);
            responseBtn.GetComponent<TMP_Text>().text = r.ResponseText;
            responseBtn.GetComponent<Button>().onClick.AddListener(() => OnPickresponse(r));
            responseBtn.GetComponent<Button>().interactable = IsAbleToInteract(r);
            if(!btnFocused && responseBtn.GetComponent<Button>().interactable){
                var eventSystem = EventSystem.current;
                eventSystem.SetSelectedGameObject( responseBtn.gameObject, new BaseEventData(eventSystem));
                btnFocused = true;
            }
            responseBoxHeight += responseButtonTemplate.sizeDelta.y * 2;
            tempResponseButton.Add(responseBtn);

        }
        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
    }

    private bool IsAbleToInteract(Response r){
        switch(r.TypeResponse){
            case Response.ResponseType.BREADBAKERMONEY:
                return GameManager.Instance.GetCoins() > 5000;
            default:
            return true;
        }
    }

    public void OnPickresponse(Response r){
        responseBox.gameObject.SetActive(false);
        foreach(GameObject go in tempResponseButton){
            Destroy(go);
        }
        btnFocused = false;
        tempResponseButton.Clear();
        dialogUI.dialogueOccupied = false;
        dialogUI.ShowDialogue(r.NextDialogue);
    }
}
