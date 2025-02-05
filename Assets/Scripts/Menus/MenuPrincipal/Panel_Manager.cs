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
    [SerializeField] GameObject canvasCr�ationProfil;
    [SerializeField] GameObject canvasS�lectionBoutons;
    [SerializeField] GameObject canvasSucc�s;
    [SerializeField] GameObject canvasEtesVousSur;
    [SerializeField] GameObject canvasStatistiques;
    [SerializeField] GameObject canvasCredits;

    [SerializeField] TextMeshProUGUI champNomProfil;
    [SerializeField] TextMeshProUGUI titreNomProfil;
    [SerializeField] TextMeshProUGUI champNomV�rification;

    [SerializeField] Image imageCr�ationProfil;
    [SerializeField] Image imageCr�ationPartie;
    [SerializeField] TextMeshProUGUI dateCr�ationProfilTexte;

    public int indiceProfilADetruire = -1;

    ArbreAchievement arbre;

    void Start()
    {
        arbre = GameObject.FindGameObjectWithTag("ArbreSucc�s").GetComponent<ArbreAchievement>();
        canvasPartie.SetActive(false);
        canvasCr�ationProfil.SetActive(false);
        canvasS�lectionBoutons.SetActive(false);
        canvasSucc�s.SetActive(false);
        canvasEtesVousSur.SetActive(false);
        canvasStatistiques.SetActive(false);
        canvasCredits.SetActive(false);
        dateCr�ationProfilTexte.text = DateTime.Now.ToLongDateString();
        if (!GestionScenes.premi�reFoisMenuPrincipal)
        {
            AllerS�lectionBoutons();
        }
    }
    
    public void AllerCr�dits()
    {
        canvasS�lectionBoutons.SetActive(false);
        canvasCredits.SetActive(true);
    }

    //Appel� lorsque l'utilisateur cr�e un nouveau profil
    public void AllerCr�ationProfil()
    {
        //Ce n'est peut-�tre pas l'endroit id�al pour appeler �a mais �a devrait faire la job
        R�initialiserDonn�es();
        imageCr�ationProfil.sprite = ProfilManager.instance.ObtenirImage();
        canvasCr�ationProfil.SetActive(true);
        canvasSauvegardes.SetActive(false);
    }

    private void R�initialiserDonn�es()
    {
        bool[] achievement = new bool[(int)Achievements.NbAchievements];
        int[] ress = new int[(int)Ressource.NbRessources];
        int[] enn = new int[(int)Ennemi.NbEnnemis];
        GestionAchievements.ChargerTableauAchievements(achievement);
        GestionStatistiques.InitialiserSatistiques(0, ress, enn, 0, 0, 1, 0, 0);
    }

    public void RetourCr�ationProfil()
    {
        canvasCr�ationProfil.SetActive(false);
        canvasSauvegardes.SetActive(true);
    }


    public void AllerS�lectionBoutons()
    {
        canvasSauvegardes.SetActive(false);
        canvasS�lectionBoutons.SetActive(true);
        canvasPartie.SetActive(false);
        canvasSucc�s.SetActive(false);
        canvasCr�ationProfil.SetActive(false);
        canvasStatistiques.SetActive(false);
        canvasCredits.SetActive(false);
        AjusterNomJoueur(SauvegardeProfils.nomProfilActuel);
    }
    public void AllerCr�ationPartie()
    {
        imageCr�ationPartie.sprite = ProfilManager.instance.ObtenirImage();
        canvasSauvegardes.SetActive(false);
        canvasS�lectionBoutons.SetActive(false);
        canvasPartie.SetActive(true);
    }


    //Appel� par un bouton pour aller sur le panel de succ�s
    public void AllerSucc�s()
    {
        canvasS�lectionBoutons.SetActive(false);
        canvasSucc�s.SetActive(true);
        arbre.ActiverPremierNoeud();
    }

    public void SauvegarderNomProfil()
    {
        SauvegardeProfils.SetNomProfilActuel(champNomProfil.text);
        titreNomProfil.text = champNomProfil.text;
        SauvegardeProfils.SetTitreProfilActuel("D�boussol�");
    }

    public void ChangerUtilisateur()
    {
        ProfilManager.instance.ChangerUtilisateur();
        canvasS�lectionBoutons.SetActive(false);
        canvasSauvegardes.SetActive(true);
    }

    public void AllerEtesVousSur()
    {
        champNomV�rification.text = SauvegardeProfils.nomProfilActuel;
        canvasSauvegardes.SetActive(false);
        canvasEtesVousSur.SetActive(true);
    }

    public void V�rifierConfirmation(bool oui)
    {
        canvasSauvegardes.SetActive(true);
        canvasEtesVousSur.SetActive(false);

        if (oui)
        {
            SauvegardeProfils.D�truireProfil(indiceProfilADetruire);
        }
    }

    public void AjusterNomJoueur(string nom)
    {
        titreNomProfil.text = nom;
    }

    public void AllerStatistiques()
    {
        canvasS�lectionBoutons.SetActive(false);
        canvasStatistiques.SetActive(true);
        ProfilPanel.instance.InitialiserChampsTextes();
    }

}
