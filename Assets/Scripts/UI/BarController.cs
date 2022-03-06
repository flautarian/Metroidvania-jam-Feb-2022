using UnityEngine;
using UnityEngine.UI;

public abstract class BarController : MonoBehaviour
{
    internal Slider slider;
    void Start()
    {
        slider = GetComponent<Slider>();
        PostStart();
    }

    public abstract void PostStart();
    public abstract void UpdateMaxValue(float value);
    public abstract void UpdateActualValue(float value);

}
