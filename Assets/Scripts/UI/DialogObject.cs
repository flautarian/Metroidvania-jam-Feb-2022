using UnityEngine;

[CreateAssetMenu(fileName = "DialogObject", menuName = "Dialog/DialogObject", order = 0)]
public class DialogObject : ScriptableObject {
    [SerializeField] [TextArea] private string[] dialogue;

    public string[] Dialoge => dialogue;
}
