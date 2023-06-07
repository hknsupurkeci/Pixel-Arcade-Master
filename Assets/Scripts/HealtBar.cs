using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealtBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    private void Start()
    {
        fill.color = gradient.Evaluate(1f);
    }
    public void SetHealt(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
