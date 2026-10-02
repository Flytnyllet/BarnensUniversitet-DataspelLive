using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxCourage(int maxCourage)
    {
        slider.maxValue = maxCourage;
        slider.value = maxCourage;
    }
    
    public void SetCourage(int courage)
    {
        slider.value = courage;
    }
}
