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
    //Vrai ou faux l'achievement est réussi en général tout court
    private static bool[] achievementsRéussi = new bool[(int)Achievements.NbAchievements];
    private static string[] titresAchievements = new string[(int)Achievements.NbAchievements];
    private static (string nom, int xp, Rareté rareté)[] nomEtXPEtRaretéAchievements = new (string, int, Rareté)[(int)Achievements.NbAchievements];

    private static List<string> titresDébloqués = new List<string>();

    public static string GetTitreAchievement(Achievements achievement)
    {
        return titresAchievements[(int)achievement];
    }
    private static void SetTitreAchievement(Achievements achievement, string titre)
    {
        titresAchievements[(int)(achievement)] = titre;
    }
    private static void SetNomEtRaretéAchievement(Achievements achievement, string nom, int xp, Rareté rareté)
    {
        nomEtXPEtRaretéAchievements[(int)(achievement)] = (nom, xp, rareté);
    }

    public static string GetNomAchievement(Achievements achievement)
    {
        return nomEtXPEtRaretéAchievements[(int)achievement].nom;
    }
    public static Rareté GetRaretéAchievement(Achievements achievement)
    {
        return nomEtXPEtRaretéAchievements[(int)achievement].rareté;
    }

    //Constructeur statique
    static GestionAchievements()
    {
        //Initialiser les valeurs de tous les achievements
        SetNomEtRaretéAchievement(Achievements.MaisOuSuisJe, "Mais où suis-je?", 20, Rareté.Commun);
        //Branche feu
        SetNomEtRaretéAchievement(Achievements.CaRechauffeLeCoeur, "Ça réchauffe le cœur", 30, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.AuFeu, "Au feu!", 50, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.CaBrule, "Ça brûle!", 110, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.AuBucher, "Au bucher!", 210, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.LaLumiereDansLesTenebres, "La lumière dans les ténèbres", 350, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.RetourneEnEnfer, "Retourne en enfer", 400, Rareté.Légendaire);
        //Branche survivant partie
        SetNomEtRaretéAchievement(Achievements.CaTientBon, "Ça tient bon", 50, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.ToujoursVivant, "Toujours vivant!", 80, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.EncoreVivant, "Encore vivant", 130, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.APeineVivant, "À peine vivant", 400, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.LaMortMEchappe, "La mort m'échappe...", 900, Rareté.Légendaire);
        //Branche survivant durée
        SetNomEtRaretéAchievement(Achievements.PasSiPire, "Pas si pire", 50, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.OnSEnSortBien, "On s'en sort bien", 80, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.JeMYHabitue, "Je m'y habitue", 110, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.Confortable, "Confortable", 130, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.ALAise, "À l'aise", 190, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.UneExpertise, "Une expertise", 350, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.LaSurvieNAPlusDeSecret, "La survie n'a plus de secret", 600, Rareté.Légendaire);
        //Branche combat
        SetNomEtRaretéAchievement(Achievements.DuSangSurLesMains, "Du sang sur les mains", 30, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.ParIciLaVermine, "Par ici la vermine", 50, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.APlusBestioles, "À plus bestioles", 100, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.PrendsCa, "Prends ça!", 150, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.UnDePlusUnDeMoins, "Un de plus, un de moins", 200, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.LetHimCook, "Let him cook!", 350, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.Genocide, "Génocide", 450, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.FemmeDeMenage, "Femme de ménage", 600, Rareté.Légendaire);
        //Combat 2e partie
        SetNomEtRaretéAchievement(Achievements.QuelleHorreur, "Quelle horreur", 35, Rareté.Commun);
        SetNomEtRaretéAchievement(Achievements.MemePasPeur, "Même pas peur!", 60, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.BriseurDOs, "Briseur d'os", 60, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.BroyeurDOs, "Broyeur d'os", 120, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.AdieuArachnides, "Adieu arachnides", 75, Rareté.Atypique);
        SetNomEtRaretéAchievement(Achievements.Esquive, "Esquive", 130, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.TourDeMagie, "Tour de magie", 250, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.IlsSontMignons, "Ils sont mignons", 130, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.TueurSousPleineLune, "Tueur sous pleine lune", 500, Rareté.Épique);
        SetNomEtRaretéAchievement(Achievements.CombattreSesPropresDemons, "Combattre ses propres démons", 110, Rareté.Rare);
        SetNomEtRaretéAchievement(Achievements.Boo, "Quelle horreur", 400, Rareté.Épique);
        //Branche récolte et craft
        SetNomEtRaretéAchievement(Achievements.JePrends, "Je prends", 35, Rareté.Commun);
        

        //Set les tires (il y en a pas pour tous les achievements)
        titresDébloqués.Add("Déboussolé");//Je l'ajoute dès le début pour qu'il y en ait au moins un
        //Branche feu
        SetTitreAchievement(Achievements.CaBrule, "Pyromane");
        SetTitreAchievement(Achievements.AuBucher, "Pyromane Effronté");
        SetTitreAchievement(Achievements.LaLumiereDansLesTenebres, "Brûleur de loups-garous");
        SetTitreAchievement(Achievements.RetourneEnEnfer, "Survivant démoniaque");
        //Branche survivant partie
        SetTitreAchievement(Achievements.ToujoursVivant, "Survivant persévérant");
        SetTitreAchievement(Achievements.EncoreVivant, "Survivant tenace");
        SetTitreAchievement(Achievements.APeineVivant, "Survivant résistant");
        SetTitreAchievement(Achievements.LaMortMEchappe, "Invincible");
        //Branche survivant durée
        SetTitreAchievement(Achievements.PasSiPire, "Survivant");
        SetTitreAchievement(Achievements.JeMYHabitue, "Survivant confirmé");
        SetTitreAchievement(Achievements.ALAise, "Survivant aguerri");
        SetTitreAchievement(Achievements.UneExpertise, "Expert survivaliste");
        SetTitreAchievement(Achievements.LaSurvieNAPlusDeSecret, "Légende de la survie");
        //Branche combat
        SetTitreAchievement(Achievements.ParIciLaVermine, "Combattant");
        SetTitreAchievement(Achievements.PrendsCa, "Combattant expérimenté");
        SetTitreAchievement(Achievements.UnDePlusUnDeMoins, "Vétéran");
        SetTitreAchievement(Achievements.LetHimCook, "Boucher");
        SetTitreAchievement(Achievements.FemmeDeMenage, "John Wick");
        SetTitreAchievement(Achievements.QuelleHorreur, "Terrifié");
        SetTitreAchievement(Achievements.MemePasPeur, "Brave combattant");
        SetTitreAchievement(Achievements.BriseurDOs, "Briseur d'os");
        SetTitreAchievement(Achievements.BroyeurDOs, "Broyeur d'os");
        SetTitreAchievement(Achievements.AdieuArachnides, "Exterminateur d'araignées");
        SetTitreAchievement(Achievements.TourDeMagie, "Magicien d'Oz");
        SetTitreAchievement(Achievements.TueurSousPleineLune, "Sanguinaire");
        SetTitreAchievement(Achievements.CombattreSesPropresDemons, "Demon Slayer");
        SetTitreAchievement(Achievements.Boo, "Ghostbuster");
    }



    public static bool EstAchievementRéussi(Achievements achievement)
    {
        return achievementsRéussi[(int)achievement];
    }
    public static void ActiverAchievement(Achievements achievement)
    {
        //Je vérifie si l'achievement est déjà débloqué pour ne pas repop une notification pour rien
        if (!achievementsRéussi[(int)achievement])
        {
            achievementsRéussi[(int)achievement] = true;
            ActiverNotification(achievement);
            achievementsPartieActuelle.Add(achievement);
            GestionStatistiques.AjouterXP(nomEtXPEtRaretéAchievements[(int)achievement].xp);
            if(!string.IsNullOrEmpty(titresAchievements[(int)achievement]))
                titresDébloqués.Add(titresAchievements[(int)achievement]);
        }
    }
    //-----------------------------------------------------
    //Afficher l'achievement au moment de sa réussite
    private static void ActiverNotification(Achievements achievement)
    {
        NotificationSystem.instance.LancerNotification(achievement);

    }
    //-------------------------
    //Garder en mémoire les achievements débloqués dans la partie en cours et les afficher à la fin de la partie
    private static List<Achievements> achievementsPartieActuelle = new List<Achievements>();

    public static List<Achievements> ObtenirAchievementsAAfficherFinPartie()
    {
        List<Achievements> listeCopiée = new List<Achievements>();
        foreach(Achievements achievement in achievementsPartieActuelle)
        {
            listeCopiée.Add(achievement);
        }
        //On efface la liste parce que cette fonction est appellée à la fin de la partie et après
        //c'est une autre partie qui recommence
        achievementsPartieActuelle.Clear();
        return listeCopiée;
    }
    

    //Pour la sauvegarde
    public static bool[] ObtenirAchievementsTableau()//Pour copier en mémoire (save)
    {
        bool[] tableauCopié = new bool[achievementsRéussi.Length];
        for(int i = 0; i < achievementsRéussi.Length; ++i)
        {
            tableauCopié[i] = achievementsRéussi[i];
        }
        return tableauCopié;
    }
    public static void ChargerTableauAchievements(bool[] tableau)//Pour charger la sauvegarde (load)
    {
        titresDébloqués.Clear();
        titresDébloqués.Add("Déboussolé");
        for (int i = 0; i < achievementsRéussi.Length; ++i)
        {
            achievementsRéussi[i] = tableau[i];
            if (tableau[i])
                if (!string.IsNullOrEmpty(titresAchievements[i]))
                    titresDébloqués.Add(GetTitreAchievement((Achievements)i));
        }
    }



    //-------------------------------------------
    public static List<string> ObtenirListeTitresDébloqués() //Juste pour pas briser l'encapsulation
    {
        List<string> listeCopiée = new List<string>();
        foreach(string s in titresDébloqués)
        {
            listeCopiée.Add(s);
        }
        return listeCopiée;
    }

}
