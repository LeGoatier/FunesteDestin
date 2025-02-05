using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class Panel_Manager : MonoBehaviour
{
    [SerializeField] GameObject canvasSauvegardes;
    [SerializeField] GameObject canvasPartie;
    [SerializeField] GameObject canvasCréationProfil;
    [SerializeField] GameObject canvasSélectionBoutons;
    [SerializeField] GameObject canvasSuccès;
    [SerializeField] GameObject canvasEtesVousSur;
    [SerializeField] GameObject canvasStatistiques;
    [SerializeField] GameObject canvasCredits;

    [SerializeField] TextMeshProUGUI champNomProfil;
    [SerializeField] TextMeshProUGUI titreNomProfil;
    [SerializeField] TextMeshProUGUI champNomVérification;

    [SerializeField] Image imageCréationProfil;
    [SerializeField] Image imageCréationPartie;
    [SerializeField] TextMeshProUGUI dateCréationProfilTexte;

    public int indiceProfilADetruire = -1;

    ArbreAchievement arbre;

    void Start()
    {
        arbre = GameObject.FindGameObjectWithTag("ArbreSuccès").GetComponent<ArbreAchievement>();
        canvasPartie.SetActive(false);
        canvasCréationProfil.SetActive(false);
        canvasSélectionBoutons.SetActive(false);
        canvasSuccès.SetActive(false);
        canvasEtesVousSur.SetActive(false);
        canvasStatistiques.SetActive(false);
        canvasCredits.SetActive(false);
        dateCréationProfilTexte.text = DateTime.Now.ToLongDateString();
        if (!GestionScenes.premièreFoisMenuPrincipal)
        {
            AllerSélectionBoutons();
        }
    }
    
    public void AllerCrédits()
    {
        canvasSélectionBoutons.SetActive(false);
        canvasCredits.SetActive(true);
    }

    //Appelé lorsque l'utilisateur crée un nouveau profil
    public void AllerCréationProfil()
    {
        //Ce n'est peut-être pas l'endroit idéal pour appeler ça mais ça devrait faire la job
        RéinitialiserDonnées();
        imageCréationProfil.sprite = ProfilManager.instance.ObtenirImage();
        canvasCréationProfil.SetActive(true);
        canvasSauvegardes.SetActive(false);
    }

    private void RéinitialiserDonnées()
    {
        bool[] achievement = new bool[(int)Achievements.NbAchievements];
        int[] ress = new int[(int)Ressource.NbRessources];
        int[] enn = new int[(int)Ennemi.NbEnnemis];
        GestionAchievements.ChargerTableauAchievements(achievement);
        GestionStatistiques.InitialiserSatistiques(0, ress, enn, 0, 0, 1, 0, 0);
    }

    public void RetourCréationProfil()
    {
        canvasCréationProfil.SetActive(false);
        canvasSauvegardes.SetActive(true);
    }


    public void AllerSélectionBoutons()
    {
        canvasSauvegardes.SetActive(false);
        canvasSélectionBoutons.SetActive(true);
        canvasPartie.SetActive(false);
        canvasSuccès.SetActive(false);
        canvasCréationProfil.SetActive(false);
        canvasStatistiques.SetActive(false);
        canvasCredits.SetActive(false);
        AjusterNomJoueur(SauvegardeProfils.nomProfilActuel);
    }
    public void AllerCréationPartie()
    {
        imageCréationPartie.sprite = ProfilManager.instance.ObtenirImage();
        canvasSauvegardes.SetActive(false);
        canvasSélectionBoutons.SetActive(false);
        canvasPartie.SetActive(true);
    }


    //Appelé par un bouton pour aller sur le panel de succès
    public void AllerSuccès()
    {
        canvasSélectionBoutons.SetActive(false);
        canvasSuccès.SetActive(true);
        arbre.ActiverPremierNoeud();
    }

    public void SauvegarderNomProfil()
    {
        SauvegardeProfils.SetNomProfilActuel(champNomProfil.text);
        titreNomProfil.text = champNomProfil.text;
        SauvegardeProfils.SetTitreProfilActuel("Déboussolé");
    }

    public void ChangerUtilisateur()
    {
        ProfilManager.instance.ChangerUtilisateur();
        canvasSélectionBoutons.SetActive(false);
        canvasSauvegardes.SetActive(true);
    }

    public void AllerEtesVousSur()
    {
        champNomVérification.text = SauvegardeProfils.nomProfilActuel;
        canvasSauvegardes.SetActive(false);
        canvasEtesVousSur.SetActive(true);
    }

    public void VérifierConfirmation(bool oui)
    {
        canvasSauvegardes.SetActive(true);
        canvasEtesVousSur.SetActive(false);

        if (oui)
        {
            SauvegardeProfils.DétruireProfil(indiceProfilADetruire);
        }
    }

    public void AjusterNomJoueur(string nom)
    {
        titreNomProfil.text = nom;
    }

    public void AllerStatistiques()
    {
        canvasSélectionBoutons.SetActive(false);
        canvasStatistiques.SetActive(true);
        ProfilPanel.instance.InitialiserChampsTextes();
    }

}
