using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Scenes { Accueil, MenuPrincipal, Loading, Game, Fin, NbScenes}

public enum �tatFinPartie { Bateau, Antenne, Fus�e, Mort, Nb�tats}

public static class GestionScenes
{
    private static Scenes sceneAChargerApresLoading;

    public static �tatFinPartie �tatFinPartieActuel;

    //Fonction appel�e par Loading pour savoir quelle sc�ne il doit changer
    public static int ObtenirSceneACharger()
    {
        return (int)sceneAChargerApresLoading;
    }
    public static bool premi�reFoisMenuPrincipal;
    public static void ChargerMenuPrincipal(bool premi�reFois)
    {
        premi�reFoisMenuPrincipal = premi�reFois;
        SceneManager.LoadScene((int)Scenes.MenuPrincipal);
    }

    public static void ChargerPartie()
    {
        GestionInventaire.R�initialiserInventaire();
        ModificationInventaire.R�initialiserTout();
        GestionStatistiques.R�initialiserStatistiquesPartie();
        
        sceneAChargerApresLoading = Scenes.Game;
        GestionBruit.instance.LancerMusiquePartie();
        SceneManager.LoadScene((int)Scenes.Loading);
    }

    public static void ChargerFin(�tatFinPartie �tatFinPartie)
    {
        //L'ordre de ces 3 lignes l� est importante faut pas changer �a
        �tatFinPartieActuel = �tatFinPartie;
        GestionStatistiques.R�agirFinDePartie();
        SceneManager.LoadScene((int)Scenes.Fin);
    }

    




}
