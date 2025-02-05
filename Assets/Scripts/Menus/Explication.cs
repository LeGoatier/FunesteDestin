using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Explication : MonoBehaviour
{

    private Image explications;


    void Awake()
    {
        Image[] temp = GetComponentsInChildren<Image>();
        foreach (Image i in temp)
        {
            if (i.gameObject.name == "Panel")
                explications = i;
        }
        //explications.gameObject.SetActive(false);
    }


    public void AfficherInfos(bool afficher)
    {
        // explications.gameObject.SetActive(afficher);
    }


}
