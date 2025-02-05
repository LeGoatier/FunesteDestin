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
    public static int indiceProfilActuel = -1; //Par d�faut -1 pour pas confondre avec l'indice 0 du profil #1
    public static string nomProfilActuel;
    public static string titreProfilActuel;

    //Constructeur statique appel�e au chargement du programme
    static SauvegardeProfils()
    {
        ChargerDonn�es();
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

    public static void D�truireProfil(int indice)
    {
        tableauProfils.profils[indice] = null;
        ProfilManager.instance.D�truireProfilUI(indice);
    }

    
    //Fonction pour ajouter les donn�es en cours dans les classes statiques � une instance de profil
    public static void SauvegarderProfil()
    {
        Profil nouveauProfil = new Profil();
        nouveauProfil.achievementsR�ussi = GestionAchievements.ObtenirAchievementsTableau();
        nouveauProfil.meilleurTemps = GestionStatistiques.ObtenirMeilleurTemps();
        nouveauProfil.ressourcesCollect�es = GestionStatistiques.ObtenirTableauRessourcesCollect�Profil();
        nouveauProfil.ennemisTu�s = GestionStatistiques.ObtenirTableauEnnemisTu�sProfil();
        nouveauProfil.joursSurv�cus = GestionStatistiques.ObtenirJoursSurv�cusProfil();
        nouveauProfil.xp = GestionStatistiques.ObtenirXP();
        nouveauProfil.niveauJoueur = GestionStatistiques.ObtenirNiveauJoueur();
        nouveauProfil.nombreVictoires = GestionStatistiques.ObtenirNombreVictoires();
        nouveauProfil.nombreTotalEnnemisTu�s = GestionStatistiques.ObtenirNombreTotalEnnemisTu�sProfil();
        nouveauProfil.jourDate = DateTime.Now.Day;
        nouveauProfil.moisDate = DateTime.Now.Month;
        nouveauProfil.ann�eDate = DateTime.Now.Year;
        nouveauProfil.nomProfil = nomProfilActuel;
        nouveauProfil.titre = titreProfilActuel;
        nouveauProfil.estActif = true;

        //Si le profil existe d�j�, on le remplace dans la liste par la version actualis�e,
        //sinon, on le rajoute carr�ment � la liste
        if(indiceProfilActuel != -1)
        {
            tableauProfils.profils[indiceProfilActuel] = nouveauProfil;
        }
        else
        {
            Debug.Log("Vous n'avez pas encore s�lectionner votre profil, ce qui signifie que vous avez commenc�" +
                "� une sc�ne diff�rente que l'accueil (faire une Sofia)");
        }
        
    }
    //Fonction pour �crire le fichier JSON dans le file data
    public static void SauvegarderPartie()
    {
        string fichierJSON = JsonUtility.ToJson(tableauProfils);
        File.WriteAllText(Application.dataPath + "/Data/data.json", fichierJSON);
    }


    //C'est par cette m�thode qu'on regarde si un fichier de sauvegarde existe et qu'on le charge si c'est le cas
    private static void ChargerDonn�es()
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
        GestionAchievements.ChargerTableauAchievements(p.achievementsR�ussi);
        GestionStatistiques.InitialiserSatistiques(p.meilleurTemps, p.ressourcesCollect�es,
            p.ennemisTu�s, p.joursSurv�cus, p.xp, p.niveauJoueur, p.nombreVictoires, p.nombreTotalEnnemisTu�s);
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
//Bon j'ai bris� un peu l'encapsulation mais je veux que ProfilManager y acc�de aussi
[System.Serializable]
public class Profil
{
    //Pusique les profils sont dans un tableau, j'ai besoin de savoir si le profil dans le tableau existe
    //r�ellement lorsque qu'on l'extrait du JSON
    public bool estActif;
    //Ici c'est toutes les statistiques qui restent entre les parties
    public bool[] achievementsR�ussi;
    public float meilleurTemps;
    public int[] ressourcesCollect�es;
    public int[] ennemisTu�s;
    public int joursSurv�cus;
    public int xp;
    public int niveauJoueur;
    public int nombreVictoires;
    public int nombreTotalEnnemisTu�s;
    //Ici c'est des informations sur le profil en soi
    public string nomProfil;
    public string titre;
    //Pour la date, dateTime ne se s�rialize pas, alors on doit sauvegarder manuellement le jour le mois et l'ann�e
    public int moisDate;
    public int jourDate;
    public int ann�eDate;
}