using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public enum Ennemi { Squelette, Dragon, Salamandre, Orc, Spider, Mage, Spectre, Lézard, DemonKing, LoupGarou, NbEnnemis}

public static class GestionStatistiques
{
    //Bon thom c'est ça qu'il faut que tu changes
    private static int[] valeursXPNiveaux = new int[21] { 0, 100, 150, 250, 400, 500, 650, 800, 1000, 1400, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5000, 5000, 5000, 100000000};
    //C'est 0 pour le premier niveau (automatique) et après niveau 1 à 2 présentement prend 100 xp

    private static int[] xpParEnnemisTués = new int[] { 1, 1, 1, 1, 1, 2, 2, 2, 3, 4 };

    //La première section sert à conserver les données sur la partie en cours
    #region
    //Durée de la partie
    #region
    private static float tempsDébutPartie;
    public static void InitialiserTempsDébutPartie()
    {
        tempsDébutPartie = Time.time;
    }
    private static float duréePartieActuelle;
    private static void FixerDuréePartieActuelle()
    {
        duréePartieActuelle = Time.time - tempsDébutPartie;
    }
    public static float ObtenirDuréePartieActuelle()
    {
        return duréePartieActuelle;
    }
    #endregion
    //Jours survécus
    private static int joursSurvécusPartie;
    public static void AugmenterJoursSurvécus() //Appelé par cycle jour nuit
    {
        if(InfoPartie.difficulté != Difficulté.paisible)
        {
            ++joursSurvécusPartie;
            ++jourSurvécusProfil;
        }
        //Branche combat
        if(joursSurvécusPartie == 1)
        {
            GestionAchievements.ActiverAchievement(Achievements.QuelleHorreur);
            if(InfoPartie.difficulté == Difficulté.difficile)
            {
                GestionAchievements.ActiverAchievement(Achievements.MemePasPeur);
            }
        }

        //Branche d'achievement survivant partie
        #region
        if (joursSurvécusPartie == 3)
        {
            GestionAchievements.ActiverAchievement(Achievements.CaTientBon);
        }
        else if(joursSurvécusPartie == 6)
        {
            GestionAchievements.ActiverAchievement(Achievements.ToujoursVivant);
        }
        else if(joursSurvécusPartie == 8)
        {
            GestionAchievements.ActiverAchievement(Achievements.EncoreVivant);
        }
        else if (joursSurvécusPartie == 10)
        {
            GestionAchievements.ActiverAchievement(Achievements.APeineVivant);
            if(InfoPartie.difficulté == Difficulté.difficile)
            {
                GestionAchievements.ActiverAchievement(Achievements.LaMortMEchappe);
            }
        }
        #endregion
        //Achievement branche survivant durée
        #region
        if (jourSurvécusProfil == 10)
        {
            GestionAchievements.ActiverAchievement(Achievements.PasSiPire);
        }
        else if (jourSurvécusProfil == 20)
        {
            GestionAchievements.ActiverAchievement(Achievements.OnSEnSortBien);
        }
        else if (jourSurvécusProfil == 30)
        {
            GestionAchievements.ActiverAchievement(Achievements.JeMYHabitue);
        }
        else if (jourSurvécusProfil == 40)
        {
            GestionAchievements.ActiverAchievement(Achievements.Confortable);
        }
        else if (jourSurvécusProfil == 50)
        {
            GestionAchievements.ActiverAchievement(Achievements.ALAise);
        }
        else if (jourSurvécusProfil == 75)
        {
            GestionAchievements.ActiverAchievement(Achievements.UneExpertise);
        }
        else if (jourSurvécusProfil == 100)
        {
            GestionAchievements.ActiverAchievement(Achievements.LaSurvieNAPlusDeSecret);
        }
        #endregion
    }

    //Ressources collectées
    #region
    private static int[] ressourcesCollectéesPartie = new int[(int)Ressource.NbRessources];
    
    public static void AjouterRessourceCollectée(Ressource ressource)
    {
        ressourcesCollectéesPartie[(int)ressource] += 1;
        ressourcesCollectéesProfil[(int)ressource] += 1;
        if(Random.value < 0.1f)//Ça devrait arriver une fois sur 10
        {
            xp += 1;
        }
    }
    public static int ObtenirRessourceCollectéePartie(Ressource ressource)
    {
        return ressourcesCollectéesPartie[(int)ressource];
    }

    #endregion
    //Ennemis tués
    #region
    private static int[] ennemisTuésPartie = new int[(int)Ennemi.NbEnnemis];

    public static void AjouterEnnemiTué(Ennemi ennemi)
    {
        ennemisTuésPartie[(int)ennemi] += 1;
        ennemisTuésProfil[(int)ennemi] += 1;
        ++nbTotalEnnemisTuésProfil;
        xp += xpParEnnemisTués[(int)ennemi];
        //Branche combat
        //Juste tuer des ennemis en général
        #region
        if (nbTotalEnnemisTuésProfil == 1)
        {
            GestionAchievements.ActiverAchievement(Achievements.DuSangSurLesMains);
        }
        else if(nbTotalEnnemisTuésProfil == 20)
        {
            GestionAchievements.ActiverAchievement(Achievements.ParIciLaVermine);
        }
        else if (nbTotalEnnemisTuésProfil == 50)
        {
            GestionAchievements.ActiverAchievement(Achievements.APlusBestioles);
        }
        else if (nbTotalEnnemisTuésProfil == 100)
        {
            GestionAchievements.ActiverAchievement(Achievements.PrendsCa);
        }
        else if (nbTotalEnnemisTuésProfil == 250)
        {
            GestionAchievements.ActiverAchievement(Achievements.UnDePlusUnDeMoins);
        }
        else if (nbTotalEnnemisTuésProfil == 500)
        {
            GestionAchievements.ActiverAchievement(Achievements.LetHimCook);
        }
        else if (nbTotalEnnemisTuésProfil == 800)
        {
            GestionAchievements.ActiverAchievement(Achievements.Genocide);
        }
        else if (nbTotalEnnemisTuésProfil == 1000)
        {
            GestionAchievements.ActiverAchievement(Achievements.FemmeDeMenage);
        }
        #endregion
        //Types d'ennemis spécifiques
        #region
        if (ennemi == Ennemi.Squelette)
        {
            if(ObtenirEnnemisTuéProfil(Ennemi.Squelette) == 20)
            {
                GestionAchievements.ActiverAchievement(Achievements.BriseurDOs);
            }
            else if (ObtenirEnnemisTuéProfil(Ennemi.Squelette) == 50)
            {
                GestionAchievements.ActiverAchievement(Achievements.BroyeurDOs);
            }
        }
        else if(ennemi == Ennemi.Spider)
        {
            if (ObtenirEnnemisTuéProfil(Ennemi.Spider) == 30)
            {
                GestionAchievements.ActiverAchievement(Achievements.AdieuArachnides);
            }
        }
        else if (ennemi == Ennemi.Mage)
        {
            if (ObtenirEnnemisTuéProfil(Ennemi.Mage) == 30)
            {
                GestionAchievements.ActiverAchievement(Achievements.Esquive);
            }
            else if (ObtenirEnnemisTuéProfil(Ennemi.Mage) == 50)
            {
                GestionAchievements.ActiverAchievement(Achievements.TourDeMagie);
            }
        }
        else if (ennemi == Ennemi.Dragon)
        {
            if (ObtenirEnnemisTuéProfil(Ennemi.Dragon) == 40)
            {
                GestionAchievements.ActiverAchievement(Achievements.IlsSontMignons);
            }
        }
        else if (ennemi == Ennemi.LoupGarou)
        {
            if (ObtenirEnnemisTuéProfil(Ennemi.LoupGarou) == 40)
            {
                GestionAchievements.ActiverAchievement(Achievements.TueurSousPleineLune);
            }
        }
        else if (ennemi == Ennemi.DemonKing)
        {
            if (ObtenirEnnemisTuéProfil(Ennemi.DemonKing) == 1)
            {
                GestionAchievements.ActiverAchievement(Achievements.CombattreSesPropresDemons);
            }
        }
        else if (ennemi == Ennemi.Spectre)
        {
            if (ObtenirEnnemisTuéProfil(Ennemi.Spectre) == 60)
            {
                GestionAchievements.ActiverAchievement(Achievements.Boo);
            }
        }
        #endregion

        //Branche feu
        #region
        if (ennemi == Ennemi.LoupGarou)
        {
            if(FeuDeCamp.instance.niveauActuel <= 2)
            {
                GestionAchievements.ActiverAchievement(Achievements.LaLumiereDansLesTenebres);
            }
        }
        else if(ennemi == Ennemi.DemonKing)
        {
            if(FeuDeCamp.instance.niveauActuel <= 3)
            {
                GestionAchievements.ActiverAchievement(Achievements.RetourneEnEnfer);
            }
        }
        #endregion
    }

    public static int ObtenirEnnemisTuéPartie(Ennemi ennemi)
    {
        return ennemisTuésPartie[(int)ennemi];
    }

    public static int ObtenirNombreTotalEnnemisTuésPartie()
    {
        int total = 0;
        foreach(int nb in ennemisTuésPartie)
        {
            total += nb;
        }
        return total;
    }
    #endregion
    #endregion
    //La seconde section conserve les données sur le profil du joueur, et ce, autravers les parties
    #region
    //Pour la durée de la partie on conserve seulement le meilleur temps
    private static float meilleurTemps;
    public static float ObtenirMeilleurTemps()
    {
        return meilleurTemps;
    }
    //On conserve le total de jour survécus par le joueur
    private static int jourSurvécusProfil;
    public static int ObtenirJoursSurvécusProfil()
    {
        return jourSurvécusProfil;
    }

    //Ressources collectées, assez similaire à celles de partie
    #region
    private static int[] ressourcesCollectéesProfil = new int[(int)Ressource.NbRessources];
    public static int ObtenirRessourceCollectéeProfil(Ressource ressource)
    {
        return ressourcesCollectéesProfil[(int)ressource];
    }
    #endregion
    //Ennemis tués, assez similaire à celles de partie aussi
    #region
    private static int[] ennemisTuésProfil = new int[(int)Ennemi.NbEnnemis];
    public static int ObtenirEnnemisTuéProfil(Ennemi ennemi)
    {
        return ennemisTuésProfil[(int)ennemi];
    }

    private static int nbTotalEnnemisTuésProfil = 0;
    public static int ObtenirNombreTotalEnnemisTuésProfil()
    {
        return nbTotalEnnemisTuésProfil;
    }
    #endregion

    private static int xp;
    public static int ObtenirXP()
    {
        return xp;
    }
    private static int niveauJoueur = 1;
    public static int ObtenirNiveauJoueur()
    {
        return niveauJoueur;
    }
    public static float ObtenirProgressionXP()
    {
        return ((float)xp) / valeursXPNiveaux[niveauJoueur];
    }

    private static int nombreVictoires;
    public static int ObtenirNombreVictoires()
    {
        return nombreVictoires;
    }

    public static void AjouterXP(int nombre)
    {
        if(xp + nombre >= valeursXPNiveaux[niveauJoueur])
        {
            //L'ordre est important
            nombre -= valeursXPNiveaux[niveauJoueur];
            nombre += xp;
            xp = 0;
            niveauJoueur++;
            AjouterXP(nombre);
        }
        else
        {
            xp += nombre;
        }
    }

    #endregion
    //La 3e section est une liste de méthodes qui sont appelées à la fin de chaque partie
    #region
    public static void RéagirFinDePartie()
    {
        FixerDuréePartieActuelle();

        if(GestionScenes.étatFinPartieActuel != ÉtatFinPartie.Mort)
        {
            MettreAJourMeilleurTemps();
            nombreVictoires++;
            xp += 30;
        }
    }

    private static void MettreAJourMeilleurTemps()
    {
        if(meilleurTemps == 0)
        {
            meilleurTemps = ObtenirDuréePartieActuelle();
        }
        else
        {
            meilleurTemps = Math.Min(meilleurTemps, ObtenirDuréePartieActuelle());
        }
    }


    public static void RéinitialiserStatistiquesPartie()
    {
        joursSurvécusPartie = 0;
        ressourcesCollectéesPartie = new int[(int)Ressource.NbRessources];
        ennemisTuésPartie = new int[(int)Ennemi.NbEnnemis];
    }

    #endregion

    //La dernière section sert seulement à la sauvegarde
    #region


    public static int[] ObtenirTableauRessourcesCollectéProfil()
    {
        return ressourcesCollectéesProfil;
    }
    public static int[] ObtenirTableauEnnemisTuésProfil()
    {
        return ennemisTuésProfil;
    }
    

    //Bon je suis désolé pour les noms de variables faites juste faire confiance que ProfilManager l'appelle comme il faut
    public static void InitialiserSatistiques(float mT, int[] ress, int[] enn, int js, int x, int nj, int nV, int nbTotalET)
    {
        meilleurTemps = mT;
        ressourcesCollectéesProfil = ress;
        ennemisTuésProfil = enn;
        jourSurvécusProfil = js;
        xp = x;
        niveauJoueur = nj;
        nombreVictoires = nV;
        nbTotalEnnemisTuésProfil = nbTotalET;
    }


    #endregion
}
