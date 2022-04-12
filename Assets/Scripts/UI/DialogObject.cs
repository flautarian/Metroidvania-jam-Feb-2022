using UnityEngine;

[CreateAssetMenu(fileName = "DialogObject", menuName = "Dialog/DialogObject", order = 0)]
public class DialogObject : ScriptableObject {
    [SerializeField] [TextArea] private string[] dialogue;

    [SerializeField] private Response[] responses;

    public string[] Dialoge => dialogue;
    public Response[] Responses => responses;
    
    public bool HasResponses => Responses != null && Responses.Length > 0;
}
