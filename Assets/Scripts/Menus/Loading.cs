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

        // commence � g�n�rer la prochaine sc�ne � l'aide d'une coroutine
        StartCoroutine(Chargement());
    }

    // Update is called once per frame
    void Update()
    {
        barreChagement.value = Time.time - tempsInitial; // uptade la barre de chargement pour qu'elle progresse
    }

    IEnumerator Chargement() // charge la prochaine sc�ne durant le temps voulu
    {
        yield return SceneManager.LoadSceneAsync(GestionScenes.ObtenirSceneACharger());
        //GestionStatistiques.InitialiserTempsD�butPartie();
        //R�activerCam�raNouvelleScene();
        //print("Fin du loading");//On a pas l'air de se rendre ici
        //GestionAchievements.ActiverAchievement(Achievements.MaisOuSuisJe);
        //Je vais donc mettre �a � une autre place mais c'est bizarre
    }

    private void R�activerCam�raNouvelleScene()
    {
        //TODO : actually faire fonctionner �a, pour l'instant la cam�ra est d�j� activ�e
        //dans la sc�ne game, mais Pierre-�tienne il dit qu'il peut y avoir des probl�mes d'affichage pour
        //la sc�ne de loading si on fait �a, et la sc�ne loading fonctionne pas au complet en ce moment
    }
}