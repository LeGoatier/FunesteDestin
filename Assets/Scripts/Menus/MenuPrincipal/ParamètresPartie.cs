using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime;
using System;

public class ParamètresPartie : MonoBehaviour
{
    [Header("Paramètres Génération")]
    [SerializeField] int maxSeed;

    string nom;
    Difficulté difficulté = Difficulté.facile;
    bool useSeed;
    int seed;
    string date;

    [Header("Objets")]
    [SerializeField] Toggle toggleSeed;
    [SerializeField] TextMeshProUGUI valeurSeed;
    [SerializeField] TextMeshProUGUI dateTMP;
    [SerializeField] Slider sliderSeed;
    [SerializeField] TextMeshProUGUI nomPartieTMP;
    [SerializeField] GameObject objDropDown;
    [SerializeField] TMP_InputField inputfield;
    [SerializeField] TMP_Dropdown dropDown;
    [SerializeField] Toggle toggleDidacticiel;

    // Start is called before the first frame update
    void Start()
    {
        date = new string(DateTime.Now.ToShortDateString());
        dateTMP.text = date;

        sliderSeed.maxValue = maxSeed;

        UtiliserSeed();
    }

    public void ChangerNom()
    {
       nom = inputfield.text;
       nomPartieTMP.text = nom;
    }

    public void ChangerDifficulté()
    {
        difficulté = (Difficulté)dropDown.value;
    }

    public void ChangerSeed()
    {
        seed = (int)sliderSeed.value;
        valeurSeed.text = new string($"{seed}");
    }
    public void UtiliserSeed()
    {
        useSeed = toggleSeed.isOn;
        if (useSeed)
        {
            valeurSeed.gameObject.SetActive(true);
            sliderSeed.interactable = true;
        }
        else 
        {
            sliderSeed.interactable = false;
            valeurSeed.gameObject.SetActive(false);
        }
    }

    public void GénérerPartie()
    {
        System.Random gen = new System.Random();

        InfoPartie.nom = nom;
        InfoPartie.difficulté = difficulté;
        InfoPartie.seed = useSeed ? seed : gen.Next(maxSeed);
        UnityEngine.Random.InitState(InfoPartie.seed);
        InfoPartie.date = date;
        InfoPartie.jouerDidacticiel = toggleDidacticiel.isOn;
        GestionScenes.ChargerPartie();
    }

}

public enum Difficulté
{
    paisible, facile, modérée, difficile
}

// Informations sur la partie
public static class InfoPartie
{
    public static string nom;
    public static Difficulté difficulté;
    public static int seed;
    public static string date;
    public static bool jouerDidacticiel;

}