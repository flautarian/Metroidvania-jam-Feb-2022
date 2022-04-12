using System;
using UnityEngine;
using TMPro;

public class CoinsIndicator : MonoBehaviour
{
    [SerializeField]
    TMP_Text number;
    void Start()
    {
        GameManager.OnGainCoins += UpdateIndicator;
    }
    
    public void UpdateIndicator(int newValue){
        number.text = ""+newValue;
    }

    private void OnDestroy() {
        GameManager.OnGainCoins -= UpdateIndicator;
    }
}
