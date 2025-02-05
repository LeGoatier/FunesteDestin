using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    [SerializeField] float tempsChargement = 3.0f;

    string niveauACharger = "Game"; 
    Slider barreChagement;
    float tempsInitial;

    // Start is called before the first frame update
    void Start()
    {
        
        if (PlayerPrefs.HasKey("NiveauACharger"))
            niveauACharger = PlayerPrefs.GetString("NiveauACharger");

        // initialise la bare de chargement
        barreChagement = GetComponentInChildren<Slider>(); 
        barreChagement.value = 0;
        barreChagement.maxValue = tempsChargement;

        tempsInitial = Time.time;

        // commence à générer la prochaine scène à l'aide d'une coroutine
        StartCoroutine(Chargement());
    }

    // Update is called once per frame
    void Update()
    {
        barreChagement.value = Time.time - tempsInitial; // uptade la barre de chargement pour qu'elle progresse
    }

    IEnumerator Chargement() // charge la prochaine scène durant le temps voulu
    {
        yield return SceneManager.LoadSceneAsync(GestionScenes.ObtenirSceneACharger());
        //GestionStatistiques.InitialiserTempsDébutPartie();
        //RéactiverCaméraNouvelleScene();
        //print("Fin du loading");//On a pas l'air de se rendre ici
        //GestionAchievements.ActiverAchievement(Achievements.MaisOuSuisJe);
        //Je vais donc mettre ça à une autre place mais c'est bizarre
    }

    private void RéactiverCaméraNouvelleScene()
    {
        //TODO : actually faire fonctionner ça, pour l'instant la caméra est déjà activée
        //dans la scène game, mais Pierre-Étienne il dit qu'il peut y avoir des problèmes d'affichage pour
        //la scène de loading si on fait ça, et la scène loading fonctionne pas au complet en ce moment
    }
}