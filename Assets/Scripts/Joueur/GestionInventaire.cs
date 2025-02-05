using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Diagnostics;

public enum Ressource
{
    Bois, Pierre, Fer, Plastique, Champignon, FilsÉlectriques, BoitierCylindrique, Batterie,
    NbRessources, Null
}
public enum Outil { 
    Marteau, MoteurBrisée, Moteur, RadioBrisée, Radio, Essence, ComposantPyrotechnique, Colorant, Voiles, FeuDeDétresse,
    NbOutils, Null
}
 
public enum Arme { Arbalète, Revolver, Sniper, Rifle, Pompe, NbArmes, Null }

public enum Soin { Champignon, Trousse, NbSoins, Null}

public static class GestionInventaire
{
    // Ressources
    private static int[] ressources = new int[(int)Ressource.NbRessources];

    // Outils
    private static bool[] outils = new bool[(int)Outil.NbOutils];

    // Armes
    private static bool[] armes = new bool[(int)Arme.NbArmes];

    // Soins
    private static int[] soins = new int[(int)Soin.NbSoins];


    public static void AjouterRessource(Ressource ressource)
    {
        ressources[(int)ressource] += 1;
        ModificationInventaire.Ajouter(ressource, ressources[(int)ressource]);
    }

    public static void AjouterOutil(Outil outil)
    {
        outils[(int)outil] = true;
        ModificationInventaire.Ajouter(outil, 1);
    }

    public static void RetirerOutil(Outil outil)
    {
        outils[(int)outil] = false;
        ModificationInventaire.Ajouter(outil, -1);
    }

    public static void AjouterArme(Arme arme)
    {
        armes[(int)arme] = true;
        ModificationInventaire.Ajouter(arme);
    }

    public static void AjouterSoin(Soin soin)
    {
        soins[(int)soin] += 1;
        ModificationInventaire.Ajouter(soin, soins[(int)soin]);
    }

    public static void UtiliserSoin(Soin soin)
    {
        
        soins[(int)soin] -= 1;
        if (soins[(int)soin] == 0)
            ModificationInventaire.Ajouter(soin, -1);
        else
            ModificationInventaire.Ajouter(soin, soins[(int)soin]);
        
    }

    public static int ObtenirRessource(Ressource ressource)
    {
        return ressources[(int)ressource];
    }

    public static bool ObtenirOutil(Outil outil)
    {
        return outils[(int)outil];
    }

    public static int ObtenirSoin(Soin soin)
    {
        return soins[(int)soin];
    }

    public static bool EstCoutRempli(Cout cout)
    {
        for(int ressource = 0; ressource < (int)Ressource.NbRessources; ressource++)
        {
            if (ressources[ressource] < cout.ressourcesRequises[ressource])
            {
                return false;
            }
        }
        foreach(Outil outil in cout.outilsRequis)
        {
            if (!outils[(int)outil])
            {
                return false;
            }
        }
        return true; //Si on se rend ici tous les couts sont remplis
    }

    public static bool AcheterCout(Cout cout)
    {
        if (EstCoutRempli(cout))
        {
            for (int ressource = 0; ressource < (int)Ressource.NbRessources; ressource++)
            {
                ressources[ressource] -= cout.ressourcesRequises[ressource];
                if (ressources[ressource] == 0)
                    ModificationInventaire.Ajouter((Ressource)ressource, -1);
                else
                    ModificationInventaire.Ajouter((Ressource)ressource, ressources[ressource]);
            }
            if(UIFeuDeCamp.instance != null)
                UIFeuDeCamp.instance.ResetUI();
            foreach (Outil outil in cout.outilsRequis)
            {
                RetirerOutil(outil);
            }

            return true;
        }
        return false;
    }
    //Appelé lorsque la partie reccomence
    public static void RéinitialiserInventaire()
    {
        ressources = new int[(int)Ressource.NbRessources];
        outils = new bool[(int)Outil.NbOutils];
        armes = new bool[(int)Arme.NbArmes];
        soins = new int[(int)Soin.NbSoins];
    }

}

public static class ModificationInventaire
{
    public static int[] ressources = new int[(int)Ressource.NbRessources];

    public static int[] outils = new int[(int)Outil.NbOutils];

    public static bool[] armes = new bool[(int)Arme.NbArmes];

    public static int[] soins = new int[(int)Soin.NbSoins];

    public static List<Outil> nouveauxCrafts = new List<Outil>();

    public static void Ajouter(Ressource ressource, int modification)
    {
        ressources[(int)ressource] = modification;
    }

    public static void Ajouter(Outil outil, int état)
    {
        outils[(int)outil] = état;
    }

    public static void Ajouter(Arme arme)
    {
        armes[(int)arme] = true;
        AppliquerModifications();
    }

    public static void Ajouter(Soin soin, int modification)
    {
        soins[(int)soin] = modification;
        AppliquerModifications();
    }

    public static void AjouterCraft(Outil outil)
    {
        nouveauxCrafts.Add(outil);
    }
    public static void AppliquerModifications()
    {
        for (int i = 0; i < ressources.Length; i++)
        {
            if (ressources[i] > 0) 
            {
                Menu_Inventaire.instance.Afficher((Ressource)i, ressources[i]);
                ressources[i] = 0;
            }
            if (ressources[i] == -1)
            {
                Menu_Inventaire.instance.Afficher((Ressource)i, 0);
                ressources[i] = 0;
            }
        }        
        for (int i = 0; i < outils.Length; i++)
        {
            if (outils[i] == -1) 
            {
                Menu_Inventaire.instance.Afficher((Outil)i, false);
                outils[i] = 0;
            }
            if (outils[i] == 1)
            {
                Menu_Inventaire.instance.Afficher((Outil)i, true);
                outils[i] = 0;
            }
        }
        for (int i = 0; i < armes.Length; i++)
        {
            if (armes[i] != false) 
            {
                Menu_Inventaire.instance.Afficher((Arme)i);
                armes[i] = false;
                
            }
        }
        for (int i = 0; i < soins.Length; i++)
        {
            if (soins[i] != 0)
            {
                Menu_Inventaire.instance.Afficher((Soin)i, soins[i]);
                soins[i] = 0;

            }
        }
    }

    public static void AppliquerNouveauxCrafts()
    {

        for (int i = 0; i < nouveauxCrafts.Count; i++)
        {
            GestionInterfaceCraft.instance.DécouvrirCraft(nouveauxCrafts[i]);
        }
        nouveauxCrafts.Clear();
    }
    //Appelé lorsque la partie reccomence
    public static void RéinitialiserTout()
    {
        ressources = new int[(int)Ressource.NbRessources];
        outils = new int[(int)Outil.NbOutils];
        armes = new bool[(int)Arme.NbArmes];
        nouveauxCrafts = new List<Outil>();
    }
}

[Serializable]
public class Cout
{
    public int[] ressourcesRequises = new int[(int)Ressource.NbRessources];
    public List<Outil> outilsRequis = new List<Outil>();

    public void AjouterRessource(Ressource ressource, int nombre)
    {
        ressourcesRequises[(int)ressource] = nombre;
    }
    public void AjouterOutil(Outil outil)
    {
        outilsRequis.Add(outil);
    }
}

