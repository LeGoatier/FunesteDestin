using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

public enum Ennemi { Squelette, Dragon, Salamandre, Orc, Spider, Mage, Spectre, L�zard, DemonKing, LoupGarou, NbEnnemis}

public static class GestionStatistiques
{
    //Bon thom c'est �a qu'il faut que tu changes
    private static int[] valeursXPNiveaux = new int[21] { 0, 100, 150, 250, 400, 500, 650, 800, 1000, 1400, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5000, 5000, 5000, 100000000};
    //C'est 0 pour le premier niveau (automatique) et apr�s niveau 1 � 2 pr�sentement prend 100 xp

    private static int[] xpParEnnemisTu�s = new int[] { 1, 1, 1, 1, 1, 2, 2, 2, 3, 4 };

    //La premi�re section sert � conserver les donn�es sur la partie en cours
    #region
    //Dur�e de la partie
    #region
    private static float tempsD�butPartie;
    public static void InitialiserTempsD�butPartie()
    {
        tempsD�butPartie = Time.time;
    }
    private static float dur�ePartieActuelle;
    private static void FixerDur�ePartieActuelle()
    {
        dur�ePartieActuelle = Time.time - tempsD�butPartie;
    }
    public static float ObtenirDur�ePartieActuelle()
    {
        return dur�ePartieActuelle;
    }
    #endregion
    //Jours surv�cus
    private static int joursSurv�cusPartie;
    public static void AugmenterJoursSurv�cus() //Appel� par cycle jour nuit
    {
        if(InfoPartie.difficult� != Difficult�.paisible)
        {
            ++joursSurv�cusPartie;
            ++jourSurv�cusProfil;
        }
        //Branche combat
        if(joursSurv�cusPartie == 1)
        {
            GestionAchievements.ActiverAchievement(Achievements.QuelleHorreur);
            if(InfoPartie.difficult� == Difficult�.difficile)
            {
                GestionAchievements.ActiverAchievement(Achievements.MemePasPeur);
            }
        }

        //Branche d'achievement survivant partie
        #region
        if (joursSurv�cusPartie == 3)
        {
            GestionAchievements.ActiverAchievement(Achievements.CaTientBon);
        }
        else if(joursSurv�cusPartie == 6)
        {
            GestionAchievements.ActiverAchievement(Achievements.ToujoursVivant);
        }
        else if(joursSurv�cusPartie == 8)
        {
            GestionAchievements.ActiverAchievement(Achievements.EncoreVivant);
        }
        else if (joursSurv�cusPartie == 10)
        {
            GestionAchievements.ActiverAchievement(Achievements.APeineVivant);
            if(InfoPartie.difficult� == Difficult�.difficile)
            {
                GestionAchievements.ActiverAchievement(Achievements.LaMortMEchappe);
            }
        }
        #endregion
        //Achievement branche survivant dur�e
        #region
        if (jourSurv�cusProfil == 10)
        {
            GestionAchievements.ActiverAchievement(Achievements.PasSiPire);
        }
        else if (jourSurv�cusProfil == 20)
        {
            GestionAchievements.ActiverAchievement(Achievements.OnSEnSortBien);
        }
        else if (jourSurv�cusProfil == 30)
        {
            GestionAchievements.ActiverAchievement(Achievements.JeMYHabitue);
        }
        else if (jourSurv�cusProfil == 40)
        {
            GestionAchievements.ActiverAchievement(Achievements.Confortable);
        }
        else if (jourSurv�cusProfil == 50)
        {
            GestionAchievements.ActiverAchievement(Achievements.ALAise);
        }
        else if (jourSurv�cusProfil == 75)
        {
            GestionAchievements.ActiverAchievement(Achievements.UneExpertise);
        }
        else if (jourSurv�cusProfil == 100)
        {
            GestionAchievements.ActiverAchievement(Achievements.LaSurvieNAPlusDeSecret);
        }
        #endregion
    }

    //Ressources collect�es
    #region
    private static int[] ressourcesCollect�esPartie = new int[(int)Ressource.NbRessources];
    
    public static void AjouterRessourceCollect�e(Ressource ressource)
    {
        ressourcesCollect�esPartie[(int)ressource] += 1;
        ressourcesCollect�esProfil[(int)ressource] += 1;
        if(Random.value < 0.1f)//�a devrait arriver une fois sur 10
        {
            xp += 1;
        }
    }
    public static int ObtenirRessourceCollect�ePartie(Ressource ressource)
    {
        return ressourcesCollect�esPartie[(int)ressource];
    }

    #endregion
    //Ennemis tu�s
    #region
    private static int[] ennemisTu�sPartie = new int[(int)Ennemi.NbEnnemis];

    public static void AjouterEnnemiTu�(Ennemi ennemi)
    {
        ennemisTu�sPartie[(int)ennemi] += 1;
        ennemisTu�sProfil[(int)ennemi] += 1;
        ++nbTotalEnnemisTu�sProfil;
        xp += xpParEnnemisTu�s[(int)ennemi];
        //Branche combat
        //Juste tuer des ennemis en g�n�ral
        #region
        if (nbTotalEnnemisTu�sProfil == 1)
        {
            GestionAchievements.ActiverAchievement(Achievements.DuSangSurLesMains);
        }
        else if(nbTotalEnnemisTu�sProfil == 20)
        {
            GestionAchievements.ActiverAchievement(Achievements.ParIciLaVermine);
        }
        else if (nbTotalEnnemisTu�sProfil == 50)
        {
            GestionAchievements.ActiverAchievement(Achievements.APlusBestioles);
        }
        else if (nbTotalEnnemisTu�sProfil == 100)
        {
            GestionAchievements.ActiverAchievement(Achievements.PrendsCa);
        }
        else if (nbTotalEnnemisTu�sProfil == 250)
        {
            GestionAchievements.ActiverAchievement(Achievements.UnDePlusUnDeMoins);
        }
        else if (nbTotalEnnemisTu�sProfil == 500)
        {
            GestionAchievements.ActiverAchievement(Achievements.LetHimCook);
        }
        else if (nbTotalEnnemisTu�sProfil == 800)
        {
            GestionAchievements.ActiverAchievement(Achievements.Genocide);
        }
        else if (nbTotalEnnemisTu�sProfil == 1000)
        {
            GestionAchievements.ActiverAchievement(Achievements.FemmeDeMenage);
        }
        #endregion
        //Types d'ennemis sp�cifiques
        #region
        if (ennemi == Ennemi.Squelette)
        {
            if(ObtenirEnnemisTu�Profil(Ennemi.Squelette) == 20)
            {
                GestionAchievements.ActiverAchievement(Achievements.BriseurDOs);
            }
            else if (ObtenirEnnemisTu�Profil(Ennemi.Squelette) == 50)
            {
                GestionAchievements.ActiverAchievement(Achievements.BroyeurDOs);
            }
        }
        else if(ennemi == Ennemi.Spider)
        {
            if (ObtenirEnnemisTu�Profil(Ennemi.Spider) == 30)
            {
                GestionAchievements.ActiverAchievement(Achievements.AdieuArachnides);
            }
        }
        else if (ennemi == Ennemi.Mage)
        {
            if (ObtenirEnnemisTu�Profil(Ennemi.Mage) == 30)
            {
                GestionAchievements.ActiverAchievement(Achievements.Esquive);
            }
            else if (ObtenirEnnemisTu�Profil(Ennemi.Mage) == 50)
            {
                GestionAchievements.ActiverAchievement(Achievements.TourDeMagie);
            }
        }
        else if (ennemi == Ennemi.Dragon)
        {
            if (ObtenirEnnemisTu�Profil(Ennemi.Dragon) == 40)
            {
                GestionAchievements.ActiverAchievement(Achievements.IlsSontMignons);
            }
        }
        else if (ennemi == Ennemi.LoupGarou)
        {
            if (ObtenirEnnemisTu�Profil(Ennemi.LoupGarou) == 40)
            {
                GestionAchievements.ActiverAchievement(Achievements.TueurSousPleineLune);
            }
        }
        else if (ennemi == Ennemi.DemonKing)
        {
            if (ObtenirEnnemisTu�Profil(Ennemi.DemonKing) == 1)
            {
                GestionAchievements.ActiverAchievement(Achievements.CombattreSesPropresDemons);
            }
        }
        else if (ennemi == Ennemi.Spectre)
        {
            if (ObtenirEnnemisTu�Profil(Ennemi.Spectre) == 60)
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

    public static int ObtenirEnnemisTu�Partie(Ennemi ennemi)
    {
        return ennemisTu�sPartie[(int)ennemi];
    }

    public static int ObtenirNombreTotalEnnemisTu�sPartie()
    {
        int total = 0;
        foreach(int nb in ennemisTu�sPartie)
        {
            total += nb;
        }
        return total;
    }
    #endregion
    #endregion
    //La seconde section conserve les donn�es sur le profil du joueur, et ce, autravers les parties
    #region
    //Pour la dur�e de la partie on conserve seulement le meilleur temps
    private static float meilleurTemps;
    public static float ObtenirMeilleurTemps()
    {
        return meilleurTemps;
    }
    //On conserve le total de jour surv�cus par le joueur
    private static int jourSurv�cusProfil;
    public static int ObtenirJoursSurv�cusProfil()
    {
        return jourSurv�cusProfil;
    }

    //Ressources collect�es, assez similaire � celles de partie
    #region
    private static int[] ressourcesCollect�esProfil = new int[(int)Ressource.NbRessources];
    public static int ObtenirRessourceCollect�eProfil(Ressource ressource)
    {
        return ressourcesCollect�esProfil[(int)ressource];
    }
    #endregion
    //Ennemis tu�s, assez similaire � celles de partie aussi
    #region
    private static int[] ennemisTu�sProfil = new int[(int)Ennemi.NbEnnemis];
    public static int ObtenirEnnemisTu�Profil(Ennemi ennemi)
    {
        return ennemisTu�sProfil[(int)ennemi];
    }

    private static int nbTotalEnnemisTu�sProfil = 0;
    public static int ObtenirNombreTotalEnnemisTu�sProfil()
    {
        return nbTotalEnnemisTu�sProfil;
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
    //La 3e section est une liste de m�thodes qui sont appel�es � la fin de chaque partie
    #region
    public static void R�agirFinDePartie()
    {
        FixerDur�ePartieActuelle();

        if(GestionScenes.�tatFinPartieActuel != �tatFinPartie.Mort)
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
            meilleurTemps = ObtenirDur�ePartieActuelle();
        }
        else
        {
            meilleurTemps = Math.Min(meilleurTemps, ObtenirDur�ePartieActuelle());
        }
    }


    public static void R�initialiserStatistiquesPartie()
    {
        joursSurv�cusPartie = 0;
        ressourcesCollect�esPartie = new int[(int)Ressource.NbRessources];
        ennemisTu�sPartie = new int[(int)Ennemi.NbEnnemis];
    }

    #endregion

    //La derni�re section sert seulement � la sauvegarde
    #region


    public static int[] ObtenirTableauRessourcesCollect�Profil()
    {
        return ressourcesCollect�esProfil;
    }
    public static int[] ObtenirTableauEnnemisTu�sProfil()
    {
        return ennemisTu�sProfil;
    }
    

    //Bon je suis d�sol� pour les noms de variables faites juste faire confiance que ProfilManager l'appelle comme il faut
    public static void InitialiserSatistiques(float mT, int[] ress, int[] enn, int js, int x, int nj, int nV, int nbTotalET)
    {
        meilleurTemps = mT;
        ressourcesCollect�esProfil = ress;
        ennemisTu�sProfil = enn;
        jourSurv�cusProfil = js;
        xp = x;
        niveauJoueur = nj;
        nombreVictoires = nV;
        nbTotalEnnemisTu�sProfil = nbTotalET;
    }


    #endregion
}
