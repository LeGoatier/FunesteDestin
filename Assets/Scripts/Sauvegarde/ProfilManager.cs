using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;



//ProfilManager s'occupe seulement de g�rer les SauvegardeUI dans la sc�ne MenuPrinicpal
//Toutes les fonctions de sauvegarde sont g�r�es par SauvegardeProfils
public class ProfilManager : MonoBehaviour
{
    [SerializeField] SauvegardeUI[] tableauSauvegardeUI;

    public static ProfilManager instance;

    public void D�truireProfilUI(int indice)
    {
        tableauSauvegardeUI[indice].D�truireSauvegarde();
    }

    public Sprite ObtenirImage()
    {
        return tableauSauvegardeUI[SauvegardeProfils.indiceProfilActuel].imagePartie;
    }
    //Fonction appel�e par le bouton changer d'utilisateur
    public void ChangerUtilisateur()
    {
        SauvegardeProfils.SauvegarderProfil();
        InitialiserUIProfils();
    }
    //Fonction appel�e par le bouton quitter
    public void QuitterApplication()
    {
        //SauvegardeProfils.SauvegarderProfil();
        //SauvegardeProfils.SauvegarderPartie();
        //Normalement il y a un OnApplicationQuit dans sauvegardeProfils qui s'occupe de �a
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
                DateTime date = new DateTime(p.ann�eDate, p.moisDate, p.jourDate);
                tableauSauvegardeUI[i].InitialiserProfil(p.nomProfil, date, p.titre, p.niveauJoueur);
            }
        }
    }
}
