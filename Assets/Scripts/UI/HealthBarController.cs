using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : BarController
{
    [SerializeField]
    private Image fill;

    public override void PostStart(){
        GameManager.OnHurt += ValueChanged;
        ValueChanged();
    }

    private void OnDestroy() {
        GameManager.OnHurt -= ValueChanged;
    }

    void ValueChanged(){
        UpdateMaxValue(GameManager.Instance.GetMaxLife());
        UpdateActualValue(GameManager.Instance.GetActualLife());
    }
    
    
    public override void UpdateMaxValue(float value){
        slider.maxValue = value;
    }
    public override void UpdateActualValue(float value){
        slider.value = value;
        if(slider.value > (slider.maxValue *0.75f))
            fill.color = Color.green;
        else if(slider.value > (slider.maxValue *0.35f))
            fill.color = Color.yellow;
        else
            fill.color = Color.red;
    }
}
