using System.Collections;
using UnityEngine;
using TMPro;

public class TypeWriterEffect : MonoBehaviour
{

    [SerializeField]
    private float typeWriterSpeed;

    public Coroutine Run(string textToType, TMP_Text textPanel){
        return StartCoroutine(TypeText(textToType, textPanel));
    }

    private IEnumerator TypeText(string textToType, TMP_Text textPanel){
        float t =0;
        int charIndex =0;

        while(charIndex < textToType.Length){
            
            t += Time.deltaTime * (typeWriterSpeed * (Input.GetButton("Fire1") ? 2: 1));

            charIndex = Mathf.FloorToInt(t);
            charIndex = Mathf.Clamp(charIndex, 0, textToType.Length);

            textPanel.text = textToType.Substring(0, charIndex);

            yield return null;
        }
        textPanel.text = textToType;
    }
}
