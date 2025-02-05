using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Paramètres : MonoBehaviour
{
    [SerializeField] Slider sensibilitéSlider;
    [SerializeField] TextMeshProUGUI texteSensibilité;

    //Méthode appelée OnValueChanged du slider
    public void AjusterSensibilité()
    {
        ControlPersonnage.instance.ChangerSensibilité(sensibilitéSlider.value);
        texteSensibilité.text = $"{sensibilitéSlider.value:F2}";
    }
}
