using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class SauvegardeProfils
{
    public static TableauProfils tableauProfils = new TableauProfils();
    public static int indiceProfilActuel = -1; //Par défaut -1 pour pas confondre avec l'indice 0 du profil #1
    public static string nomProfilActuel;
    public static string titreProfilActuel;

    //Constructeur statique appelée au chargement du programme
    static SauvegardeProfils()
    {
        ChargerDonnées();
    }

    public static void SetIndiceProfilActuel(int indice)
    {
        indiceProfilActuel = indice;
    }
    public static void SetNomProfilActuel(string nom)
    {
        nomProfilActuel = nom;
    }
    public static void SetTitreProfilActuel(string titre)
    {
        titreProfilActuel = titre;
    }

    public static void DétruireProfil(int indice)
    {
        tableauProfils.profils[indice] = null;
        ProfilManager.instance.DétruireProfilUI(indice);
    }

    
    //Fonction pour ajouter les données en cours dans les classes statiques à une instance de profil
    public static void SauvegarderProfil()
    {
        Profil nouveauProfil = new Profil();
        nouveauProfil.achievementsRéussi = GestionAchievements.ObtenirAchievementsTableau();
        nouveauProfil.meilleurTemps = GestionStatistiques.ObtenirMeilleurTemps();
        nouveauProfil.ressourcesCollectées = GestionStatistiques.ObtenirTableauRessourcesCollectéProfil();
        nouveauProfil.ennemisTués = GestionStatistiques.ObtenirTableauEnnemisTuésProfil();
        nouveauProfil.joursSurvécus = GestionStatistiques.ObtenirJoursSurvécusProfil();
        nouveauProfil.xp = GestionStatistiques.ObtenirXP();
        nouveauProfil.niveauJoueur = GestionStatistiques.ObtenirNiveauJoueur();
        nouveauProfil.nombreVictoires = GestionStatistiques.ObtenirNombreVictoires();
        nouveauProfil.nombreTotalEnnemisTués = GestionStatistiques.ObtenirNombreTotalEnnemisTuésProfil();
        nouveauProfil.jourDate = DateTime.Now.Day;
        nouveauProfil.moisDate = DateTime.Now.Month;
        nouveauProfil.annéeDate = DateTime.Now.Year;
        nouveauProfil.nomProfil = nomProfilActuel;
        nouveauProfil.titre = titreProfilActuel;
        nouveauProfil.estActif = true;

        //Si le profil existe déjà, on le remplace dans la liste par la version actualisée,
        //sinon, on le rajoute carrément à la liste
        if(indiceProfilActuel != -1)
        {
            tableauProfils.profils[indiceProfilActuel] = nouveauProfil;
        }
        else
        {
            Debug.Log("Vous n'avez pas encore sélectionner votre profil, ce qui signifie que vous avez commencé" +
                "à une scène différente que l'accueil (faire une Sofia)");
        }
        
    }
    //Fonction pour écrire le fichier JSON dans le file data
    public static void SauvegarderPartie()
    {
        string fichierJSON = JsonUtility.ToJson(tableauProfils);
        File.WriteAllText(Application.dataPath + "/Data/data.json", fichierJSON);
    }


    //C'est par cette méthode qu'on regarde si un fichier de sauvegarde existe et qu'on le charge si c'est le cas
    private static void ChargerDonnées()
    {
        string chemin = Application.dataPath + "/Data/data.json";
        if (File.Exists(chemin))
        {
            string fichierJSON = File.ReadAllText(chemin);
            tableauProfils = JsonUtility.FromJson<TableauProfils>(fichierJSON);
        }
    }

    public static void ChargerSauvegarde()
    {
        Profil p = tableauProfils.profils[indiceProfilActuel];
        GestionAchievements.ChargerTableauAchievements(p.achievementsRéussi);
        GestionStatistiques.InitialiserSatistiques(p.meilleurTemps, p.ressourcesCollectées,
            p.ennemisTués, p.joursSurvécus, p.xp, p.niveauJoueur, p.nombreVictoires, p.nombreTotalEnnemisTués);
    }
    

    [System.Serializable]
    public class TableauProfils
    {
        public Profil[] profils;
        public TableauProfils()
        {
            profils = new Profil[3];
        }
    }
}
//Bon j'ai brisé un peu l'encapsulation mais je veux que ProfilManager y accède aussi
[System.Serializable]
public class Profil
{
    //Pusique les profils sont dans un tableau, j'ai besoin de savoir si le profil dans le tableau existe
    //réellement lorsque qu'on l'extrait du JSON
    public bool estActif;
    //Ici c'est toutes les statistiques qui restent entre les parties
    public bool[] achievementsRéussi;
    public float meilleurTemps;
    public int[] ressourcesCollectées;
    public int[] ennemisTués;
    public int joursSurvécus;
    public int xp;
    public int niveauJoueur;
    public int nombreVictoires;
    public int nombreTotalEnnemisTués;
    //Ici c'est des informations sur le profil en soi
    public string nomProfil;
    public string titre;
    //Pour la date, dateTime ne se sérialize pas, alors on doit sauvegarder manuellement le jour le mois et l'année
    public int moisDate;
    public int jourDate;
    public int annéeDate;
}