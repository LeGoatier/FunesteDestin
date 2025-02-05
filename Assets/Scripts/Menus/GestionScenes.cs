using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { Accueil, MenuPrincipal, Loading, Game, Fin, NbScenes}

public enum ÉtatFinPartie { Bateau, Antenne, Fusée, Mort, NbÉtats}

public static class GestionScenes
{
    private static Scenes sceneAChargerApresLoading;

    public static ÉtatFinPartie étatFinPartieActuel;

    //Fonction appelée par Loading pour savoir quelle scène il doit changer
    public static int ObtenirSceneACharger()
    {
        return (int)sceneAChargerApresLoading;
    }
    public static bool premièreFoisMenuPrincipal;
    public static void ChargerMenuPrincipal(bool premièreFois)
    {
        premièreFoisMenuPrincipal = premièreFois;
        SceneManager.LoadScene((int)Scenes.MenuPrincipal);
    }

    public static void ChargerPartie()
    {
        GestionInventaire.RéinitialiserInventaire();
        ModificationInventaire.RéinitialiserTout();
        GestionStatistiques.RéinitialiserStatistiquesPartie();
        
        sceneAChargerApresLoading = Scenes.Game;
        GestionBruit.instance.LancerMusiquePartie();
        SceneManager.LoadScene((int)Scenes.Loading);
    }

    public static void ChargerFin(ÉtatFinPartie étatFinPartie)
    {
        //L'ordre de ces 3 lignes là est importante faut pas changer ça
        étatFinPartieActuel = étatFinPartie;
        GestionStatistiques.RéagirFinDePartie();
        SceneManager.LoadScene((int)Scenes.Fin);
    }

    




}
