using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;



//ProfilManager s'occupe seulement de gérer les SauvegardeUI dans la scène MenuPrinicpal
//Toutes les fonctions de sauvegarde sont gérées par SauvegardeProfils
public class ProfilManager : MonoBehaviour
{
    [SerializeField] SauvegardeUI[] tableauSauvegardeUI;

    public static ProfilManager instance;

    public void DétruireProfilUI(int indice)
    {
        tableauSauvegardeUI[indice].DétruireSauvegarde();
    }

    public Sprite ObtenirImage()
    {
        return tableauSauvegardeUI[SauvegardeProfils.indiceProfilActuel].imagePartie;
    }
    //Fonction appelée par le bouton changer d'utilisateur
    public void ChangerUtilisateur()
    {
        SauvegardeProfils.SauvegarderProfil();
        InitialiserUIProfils();
    }
    //Fonction appelée par le bouton quitter
    public void QuitterApplication()
    {
        //SauvegardeProfils.SauvegarderProfil();
        //SauvegardeProfils.SauvegarderPartie();
        //Normalement il y a un OnApplicationQuit dans sauvegardeProfils qui s'occupe de ça
        Application.Quit();
    }

    private void OnApplicationQuit()
    {
        SauvegardeProfils.SauvegarderProfil();
        SauvegardeProfils.SauvegarderPartie();
    }

    void Start()
    {
        if (instance == null)
            instance = this;
        
        InitialiserUIProfils();
    }

    private void InitialiserUIProfils()
    {
        for(int i = 0; i < SauvegardeProfils.tableauProfils.profils.Length; i++)
        {
            if (SauvegardeProfils.tableauProfils.profils[i] != null && SauvegardeProfils.tableauProfils.profils[i].estActif)
            {
                Profil p = SauvegardeProfils.tableauProfils.profils[i];
                DateTime date = new DateTime(p.annéeDate, p.moisDate, p.jourDate);
                tableauSauvegardeUI[i].InitialiserProfil(p.nomProfil, date, p.titre, p.niveauJoueur);
            }
        }
    }
}
