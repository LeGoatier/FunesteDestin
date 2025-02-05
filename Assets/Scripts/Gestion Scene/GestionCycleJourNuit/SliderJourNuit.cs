using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderJourNuit : MonoBehaviour
{
    [SerializeField] Color couleurJour;
    [SerializeField] Color couleurNuit;
    [SerializeField] Image background;
    [SerializeField] Image fill;

    [SerializeField] Sprite imageSoleil;
    [SerializeField] Sprite imageLune;
    [SerializeField] Image handle;

    Slider slider;

    bool estJour = true;//On commence � 0 temps dans le jour
    float dernierTempsChangement;

    private void Start()
    {
        slider = GetComponent<Slider>();
        dernierTempsChangement = Time.time;
        background.color = couleurJour;
        fill.color = couleurNuit;
        handle.sprite = imageSoleil;
    }

    private void Update()
    {
        float dur�eP�riode = estJour ? CycleJours.DUR�E_JOUR : CycleJours.DUR�E_NUIT;
        ModifierValeurSlider(dur�eP�riode);
        if(Time.time - dernierTempsChangement >= dur�eP�riode)
        {
            SwitchJourNuit();
        }
    }

    private void SwitchJourNuit()
    {
        dernierTempsChangement = Time.time;
        estJour = !estJour;
        if (estJour) //Si c'est le jour pour de vrai
        {
            background.color = couleurJour;
            fill.color = couleurNuit;
            handle.sprite = imageSoleil;
        }
        else //Si c'est la nuit pour de vrai
        {
            background.color = couleurNuit;
            fill.color = couleurJour;
            handle.sprite = imageLune;
        }
        slider.value = 0;
    }

    private void ModifierValeurSlider(float dur�eP�riode)
    {
        float tempsDepuisDernierChangement = Time.time - dernierTempsChangement;
        slider.value = Mathf.Lerp(0, 1, tempsDepuisDernierChangement / dur�eP�riode);
    }
}
