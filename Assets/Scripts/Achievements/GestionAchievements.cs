using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;


public enum Achievements { MaisOuSuisJe, 
    CaRechauffeLeCoeur, AuFeu, CaBrule, AuBucher, LaLumiereDansLesTenebres, RetourneEnEnfer,
    CaTientBon, ToujoursVivant, EncoreVivant, APeineVivant, LaMortMEchappe,
    PasSiPire, OnSEnSortBien, JeMYHabitue, Confortable, ALAise, UneExpertise, LaSurvieNAPlusDeSecret,
    DuSangSurLesMains, ParIciLaVermine, APlusBestioles, PrendsCa, UnDePlusUnDeMoins, LetHimCook, Genocide,
    FemmeDeMenage, QuelleHorreur, MemePasPeur, BriseurDOs, BroyeurDOs, AdieuArachnides, Esquive, TourDeMagie,
    IlsSontMignons, TueurSousPleineLune, CombattreSesPropresDemons, Boo,
    JePrends,
    NbAchievements}

public static class GestionAchievements
{
    //Vrai ou faux l'achievement est r�ussi en g�n�ral tout court
    private static bool[] achievementsR�ussi = new bool[(int)Achievements.NbAchievements];
    private static string[] titresAchievements = new string[(int)Achievements.NbAchievements];
    private static (string nom, int xp, Raret� raret�)[] nomEtXPEtRaret�Achievements = new (string, int, Raret�)[(int)Achievements.NbAchievements];

    private static List<string> titresD�bloqu�s = new List<string>();

    public static string GetTitreAchievement(Achievements achievement)
    {
        return titresAchievements[(int)achievement];
    }
    private static void SetTitreAchievement(Achievements achievement, string titre)
    {
        titresAchievements[(int)(achievement)] = titre;
    }
    private static void SetNomEtRaret�Achievement(Achievements achievement, string nom, int xp, Raret� raret�)
    {
        nomEtXPEtRaret�Achievements[(int)(achievement)] = (nom, xp, raret�);
    }

    public static string GetNomAchievement(Achievements achievement)
    {
        return nomEtXPEtRaret�Achievements[(int)achievement].nom;
    }
    public static Raret� GetRaret�Achievement(Achievements achievement)
    {
        return nomEtXPEtRaret�Achievements[(int)achievement].raret�;
    }

    //Constructeur statique
    static GestionAchievements()
    {
        //Initialiser les valeurs de tous les achievements
        SetNomEtRaret�Achievement(Achievements.MaisOuSuisJe, "Mais o� suis-je?", 20, Raret�.Commun);
        //Branche feu
        SetNomEtRaret�Achievement(Achievements.CaRechauffeLeCoeur, "�a r�chauffe le c�ur", 30, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.AuFeu, "Au feu!", 50, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.CaBrule, "�a br�le!", 110, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.AuBucher, "Au bucher!", 210, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.LaLumiereDansLesTenebres, "La lumi�re dans les t�n�bres", 350, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.RetourneEnEnfer, "Retourne en enfer", 400, Raret�.L�gendaire);
        //Branche survivant partie
        SetNomEtRaret�Achievement(Achievements.CaTientBon, "�a tient bon", 50, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.ToujoursVivant, "Toujours vivant!", 80, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.EncoreVivant, "Encore vivant", 130, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.APeineVivant, "� peine vivant", 400, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.LaMortMEchappe, "La mort m'�chappe...", 900, Raret�.L�gendaire);
        //Branche survivant dur�e
        SetNomEtRaret�Achievement(Achievements.PasSiPire, "Pas si pire", 50, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.OnSEnSortBien, "On s'en sort bien", 80, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.JeMYHabitue, "Je m'y habitue", 110, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.Confortable, "Confortable", 130, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.ALAise, "� l'aise", 190, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.UneExpertise, "Une expertise", 350, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.LaSurvieNAPlusDeSecret, "La survie n'a plus de secret", 600, Raret�.L�gendaire);
        //Branche combat
        SetNomEtRaret�Achievement(Achievements.DuSangSurLesMains, "Du sang sur les mains", 30, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.ParIciLaVermine, "Par ici la vermine", 50, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.APlusBestioles, "� plus bestioles", 100, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.PrendsCa, "Prends �a!", 150, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.UnDePlusUnDeMoins, "Un de plus, un de moins", 200, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.LetHimCook, "Let him cook!", 350, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.Genocide, "G�nocide", 450, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.FemmeDeMenage, "Femme de m�nage", 600, Raret�.L�gendaire);
        //Combat 2e partie
        SetNomEtRaret�Achievement(Achievements.QuelleHorreur, "Quelle horreur", 35, Raret�.Commun);
        SetNomEtRaret�Achievement(Achievements.MemePasPeur, "M�me pas peur!", 60, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.BriseurDOs, "Briseur d'os", 60, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.BroyeurDOs, "Broyeur d'os", 120, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.AdieuArachnides, "Adieu arachnides", 75, Raret�.Atypique);
        SetNomEtRaret�Achievement(Achievements.Esquive, "Esquive", 130, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.TourDeMagie, "Tour de magie", 250, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.IlsSontMignons, "Ils sont mignons", 130, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.TueurSousPleineLune, "Tueur sous pleine lune", 500, Raret�.�pique);
        SetNomEtRaret�Achievement(Achievements.CombattreSesPropresDemons, "Combattre ses propres d�mons", 110, Raret�.Rare);
        SetNomEtRaret�Achievement(Achievements.Boo, "Quelle horreur", 400, Raret�.�pique);
        //Branche r�colte et craft
        SetNomEtRaret�Achievement(Achievements.JePrends, "Je prends", 35, Raret�.Commun);
        

        //Set les tires (il y en a pas pour tous les achievements)
        titresD�bloqu�s.Add("D�boussol�");//Je l'ajoute d�s le d�but pour qu'il y en ait au moins un
        //Branche feu
        SetTitreAchievement(Achievements.CaBrule, "Pyromane");
        SetTitreAchievement(Achievements.AuBucher, "Pyromane Effront�");
        SetTitreAchievement(Achievements.LaLumiereDansLesTenebres, "Br�leur de loups-garous");
        SetTitreAchievement(Achievements.RetourneEnEnfer, "Survivant d�moniaque");
        //Branche survivant partie
        SetTitreAchievement(Achievements.ToujoursVivant, "Survivant pers�v�rant");
        SetTitreAchievement(Achievements.EncoreVivant, "Survivant tenace");
        SetTitreAchievement(Achievements.APeineVivant, "Survivant r�sistant");
        SetTitreAchievement(Achievements.LaMortMEchappe, "Invincible");
        //Branche survivant dur�e
        SetTitreAchievement(Achievements.PasSiPire, "Survivant");
        SetTitreAchievement(Achievements.JeMYHabitue, "Survivant confirm�");
        SetTitreAchievement(Achievements.ALAise, "Survivant aguerri");
        SetTitreAchievement(Achievements.UneExpertise, "Expert survivaliste");
        SetTitreAchievement(Achievements.LaSurvieNAPlusDeSecret, "L�gende de la survie");
        //Branche combat
        SetTitreAchievement(Achievements.ParIciLaVermine, "Combattant");
        SetTitreAchievement(Achievements.PrendsCa, "Combattant exp�riment�");
        SetTitreAchievement(Achievements.UnDePlusUnDeMoins, "V�t�ran");
        SetTitreAchievement(Achievements.LetHimCook, "Boucher");
        SetTitreAchievement(Achievements.FemmeDeMenage, "John Wick");
        SetTitreAchievement(Achievements.QuelleHorreur, "Terrifi�");
        SetTitreAchievement(Achievements.MemePasPeur, "Brave combattant");
        SetTitreAchievement(Achievements.BriseurDOs, "Briseur d'os");
        SetTitreAchievement(Achievements.BroyeurDOs, "Broyeur d'os");
        SetTitreAchievement(Achievements.AdieuArachnides, "Exterminateur d'araign�es");
        SetTitreAchievement(Achievements.TourDeMagie, "Magicien d'Oz");
        SetTitreAchievement(Achievements.TueurSousPleineLune, "Sanguinaire");
        SetTitreAchievement(Achievements.CombattreSesPropresDemons, "Demon Slayer");
        SetTitreAchievement(Achievements.Boo, "Ghostbuster");
    }



    public static bool EstAchievementR�ussi(Achievements achievement)
    {
        return achievementsR�ussi[(int)achievement];
    }
    public static void ActiverAchievement(Achievements achievement)
    {
        //Je v�rifie si l'achievement est d�j� d�bloqu� pour ne pas repop une notification pour rien
        if (!achievementsR�ussi[(int)achievement])
        {
            achievementsR�ussi[(int)achievement] = true;
            ActiverNotification(achievement);
            achievementsPartieActuelle.Add(achievement);
            GestionStatistiques.AjouterXP(nomEtXPEtRaret�Achievements[(int)achievement].xp);
            if(!string.IsNullOrEmpty(titresAchievements[(int)achievement]))
                titresD�bloqu�s.Add(titresAchievements[(int)achievement]);
        }
    }
    //-----------------------------------------------------
    //Afficher l'achievement au moment de sa r�ussite
    private static void ActiverNotification(Achievements achievement)
    {
        NotificationSystem.instance.LancerNotification(achievement);

    }
    //-------------------------
    //Garder en m�moire les achievements d�bloqu�s dans la partie en cours et les afficher � la fin de la partie
    private static List<Achievements> achievementsPartieActuelle = new List<Achievements>();

    public static List<Achievements> ObtenirAchievementsAAfficherFinPartie()
    {
        List<Achievements> listeCopi�e = new List<Achievements>();
        foreach(Achievements achievement in achievementsPartieActuelle)
        {
            listeCopi�e.Add(achievement);
        }
        //On efface la liste parce que cette fonction est appell�e � la fin de la partie et apr�s
        //c'est une autre partie qui recommence
        achievementsPartieActuelle.Clear();
        return listeCopi�e;
    }
    

    //Pour la sauvegarde
    public static bool[] ObtenirAchievementsTableau()//Pour copier en m�moire (save)
    {
        bool[] tableauCopi� = new bool[achievementsR�ussi.Length];
        for(int i = 0; i < achievementsR�ussi.Length; ++i)
        {
            tableauCopi�[i] = achievementsR�ussi[i];
        }
        return tableauCopi�;
    }
    public static void ChargerTableauAchievements(bool[] tableau)//Pour charger la sauvegarde (load)
    {
        titresD�bloqu�s.Clear();
        titresD�bloqu�s.Add("D�boussol�");
        for (int i = 0; i < achievementsR�ussi.Length; ++i)
        {
            achievementsR�ussi[i] = tableau[i];
            if (tableau[i])
                if (!string.IsNullOrEmpty(titresAchievements[i]))
                    titresD�bloqu�s.Add(GetTitreAchievement((Achievements)i));
        }
    }



    //-------------------------------------------
    public static List<string> ObtenirListeTitresD�bloqu�s() //Juste pour pas briser l'encapsulation
    {
        List<string> listeCopi�e = new List<string>();
        foreach(string s in titresD�bloqu�s)
        {
            listeCopi�e.Add(s);
        }
        return listeCopi�e;
    }

}
