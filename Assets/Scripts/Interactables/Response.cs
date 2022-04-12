using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{

    public enum ResponseType{
        BREADBAKERMONEY, NORMALANSWER
    }
    [SerializeField] private string responseText;
    [SerializeField] private DialogObject nextDialogue;

    [SerializeField]private ResponseType responseType;

    public DialogObject NextDialogue => nextDialogue;
    public string ResponseText => responseText;

    public ResponseType TypeResponse => responseType;
}
