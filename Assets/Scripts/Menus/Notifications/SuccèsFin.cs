using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SuccèsFin : MonoBehaviour
{
    Image badge;
    TextMeshProUGUI nom;

    // Start is called before the first frame update
    void Awake()
    {
        Image[] temp = GetComponentsInChildren<Image>();
        foreach (Image image in temp) 
        { 
            if(image.gameObject.name == "Badge")
                badge = image;
        }
        nom = GetComponentInChildren<TextMeshProUGUI>();
    }


    public void SetAchievement(Achievements achievement, Rareté rareté)
    {
        badge.sprite = RésuméPartieCanevas.instance.badges[(int)rareté];
        nom.text = GestionAchievements.GetNomAchievement(achievement);
    }

}
