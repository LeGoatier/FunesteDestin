///Ce script sert à sélectionner les étapes nécessaires à la progression du joueur selon la difficulté choisie.
///Les étapes sont modélisées par un graphe et un algorithme remonte le graphe à partir de la fin pour activer toutes
///les étapes nécessaires à la sortie de l'île par le joueur
///Auteur : Justin Gauthier
///Date : 6 mars 2024

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

//Une étape constitue n'importe quel noeud dans le graphe modélisant la progression.
//Une étape peut soit être une structure qui devra être réparée sur l'île ou un outil qui doit être construit
public enum Étapes { Établi, BoitierCylindrique, Batterie, FilsÉlectrique, Marteau, Sniper, Abri, ComposantPyrotechnique, Essence, Moteur, Radio, Colorant, Voile, FuséeDétresse, Bateau, Antenne, nbÉtapes}

public class ChoixÉtapesGraphe
{
    private bool[] étapesChoisies = new bool[(int)Étapes.nbÉtapes];
    Noeud[] graphe;
    //La création du graphe et la sélection des étapes activées se fait lors de la construction de l'objet
    public ChoixÉtapesGraphe()
    {
        ChoisirÉtapesDansGraphe(InfoPartie.difficulté);
    }
    //La seule méthode publique de cette classe est celle-ci. Ceci veut dire qu'à l'extérieur de cette classe,
    //la seule information accessible est si une étape quelconque est activée ou non
    public bool EstÉtapeActivée(Étapes étape)
    {
        return étapesChoisies[(int)étape];
    }

    private void ChoisirÉtapesDansGraphe(Difficulté difficulté)
    {
        CréerGraphe();
        ActiverGrapheSelonFinsChoisies(difficulté);
        graphe[(int)Étapes.Abri].ActiverNoeudEtPrérequis(); //Puisque l'abri n'est pas nécessaire à aucune victoire je dois l'activer manuellement
        ExtraireÉtapesChoisiesDuGraphe();
    }

    private void ExtraireÉtapesChoisiesDuGraphe()
    {
        for(int i = 0; i < (int)Étapes.nbÉtapes; i++)
        {
            étapesChoisies[i] = graphe[i].EstActivé;
        }
    }

    private void CréerGraphe()
    {
        graphe = new Noeud[(int)Étapes.nbÉtapes];

        //On commence par générer les noeuds qui n'ont pas de précédents, puis on monte
        //progressivement le graphe pour que les noeuds qui ont des précédents les référencent comme il faut
        graphe[(int)Étapes.Établi] = new Noeud(0);
        graphe[(int)Étapes.ComposantPyrotechnique] = new Noeud(0);
        graphe[(int)Étapes.Essence] = new Noeud(0);
        graphe[(int)Étapes.Colorant] = new Noeud(0);
        graphe[(int)Étapes.Voile] = new Noeud(0);
        //Ceux qui ont l'établi comme précédent
        graphe[(int)Étapes.BoitierCylindrique] = new Noeud(1);
        graphe[(int)Étapes.BoitierCylindrique].AjouterNoeudPréRequis(0, Étapes.Établi, graphe);

        graphe[(int)Étapes.Marteau] = new Noeud(1);
        graphe[(int)Étapes.Marteau].AjouterNoeudPréRequis(0, Étapes.Établi, graphe);

        graphe[(int)Étapes.FilsÉlectrique] = new Noeud(1);
        graphe[(int)Étapes.FilsÉlectrique].AjouterNoeudPréRequis(0, Étapes.Établi, graphe);

        graphe[(int)Étapes.Sniper] = new Noeud(1);
        graphe[(int)Étapes.Sniper].AjouterNoeudPréRequis(0, Étapes.Établi, graphe);

        //Un autre étage plus loin
        graphe[(int)Étapes.Abri] = new Noeud(1);
        graphe[(int)Étapes.Abri].AjouterNoeudPréRequis(0, Étapes.Marteau, graphe);

        graphe[(int)Étapes.Batterie] = new Noeud(1);
        graphe[(int)Étapes.Batterie].AjouterNoeudPréRequis(0, Étapes.BoitierCylindrique, graphe);

        graphe[(int)Étapes.Moteur] = new Noeud(1);
        graphe[(int)Étapes.Moteur].AjouterNoeudPréRequis(0, Étapes.FilsÉlectrique, graphe);

        graphe[(int)Étapes.Radio] = new Noeud(1);
        graphe[(int)Étapes.Radio].AjouterNoeudPréRequis(0, Étapes.Batterie, graphe);
        graphe[(int)Étapes.Radio].AjouterNoeudPréRequis(0, Étapes.FilsÉlectrique, graphe);

        //Les étapes finales
        graphe[(int)Étapes.FuséeDétresse] = new Noeud(1);
        graphe[(int)Étapes.FuséeDétresse].AjouterNoeudPréRequis(0, Étapes.ComposantPyrotechnique, graphe);
        graphe[(int)Étapes.FuséeDétresse].AjouterNoeudPréRequis(0, Étapes.BoitierCylindrique, graphe);
        graphe[(int)Étapes.FuséeDétresse].AjouterNoeudPréRequis(0, Étapes.Colorant, graphe);

        graphe[(int)Étapes.Bateau] = new Noeud(2);
        graphe[(int)Étapes.Bateau].AjouterNoeudPréRequis(0, Étapes.Marteau, graphe);
        graphe[(int)Étapes.Bateau].AjouterNoeudPréRequis(0, Étapes.Moteur, graphe);
        graphe[(int)Étapes.Bateau].AjouterNoeudPréRequis(0, Étapes.Essence, graphe);

        graphe[(int)Étapes.Bateau].AjouterNoeudPréRequis(1, Étapes.Marteau, graphe);
        graphe[(int)Étapes.Bateau].AjouterNoeudPréRequis(1, Étapes.Voile, graphe);

        graphe[(int)Étapes.Antenne] = new Noeud(1);
        graphe[(int)Étapes.Antenne].AjouterNoeudPréRequis(0, Étapes.Radio, graphe);
        graphe[(int)Étapes.Antenne].AjouterNoeudPréRequis(0, Étapes.Sniper, graphe);
    }

    private void ActiverGrapheSelonFinsChoisies(Difficulté difficulté)
    {
        Noeud[] étapesFinales = new Noeud[] { graphe[(int)Étapes.FuséeDétresse], graphe[(int)Étapes.Bateau], graphe[(int)Étapes.Antenne] };

        if (difficulté == Difficulté.facile || difficulté == Difficulté.paisible)
        {
            foreach (Noeud noeud in étapesFinales)
            {
                noeud.ActiverNoeudEtPrérequis();
            }
        }
        else if (difficulté == Difficulté.modérée)
        {
            int indice1 = ChoisirIndiceAuHasard(3);
            int indice2;
            do
            {
                indice2 = ChoisirIndiceAuHasard(3);
            } while (indice2 == indice1); //Pour pas que les 2 indices soit les mêmes

            étapesFinales[indice1].ActiverNoeudEtPrérequis();
            étapesFinales[indice2].ActiverNoeudEtPrérequis();
        }
        else if (difficulté == Difficulté.difficile)
        {
            étapesFinales[ChoisirIndiceAuHasard(3)].ActiverNoeudEtPrérequis();
        }
    }

    internal class Noeud
    {
        GroupeNoeuds[] préRequisPossibles;
        public bool EstActivé { get; private set; }
        internal Noeud(int nombreGroupePrérequis)
        {
            préRequisPossibles = new GroupeNoeuds[nombreGroupePrérequis];
            for(int i = 0; i < nombreGroupePrérequis; i++)
            {
                préRequisPossibles[i] = new GroupeNoeuds();
            }
        }

        internal void AjouterNoeudPréRequis(int indice, Étapes étape, Noeud[] graphe)
        {
            préRequisPossibles[indice].noeuds.Add(graphe[(int)étape]);
        }

        internal void ActiverNoeudEtPrérequis()
        {
            if (!EstActivé) //On vérifie pour ne pas refaire inutilement l'activation de plusieurs noeuds
            {
                EstActivé = true;

                //Si le noeud n'a pas de prérequisPossibles, c'est qu'il est un noeud de départ et on ne fait donc rien de plus
                //Sinon, on choisit parmi les groupes de prérequis possibles pour en activer un
                if (préRequisPossibles.Length != 0)
                {
                    int indiceChoisi = ChoisirIndiceAuHasard(préRequisPossibles.Length);
                    préRequisPossibles[indiceChoisi].Activer();
                }
            }
        }
        
    }
    internal class GroupeNoeuds
    {
        internal List<Noeud> noeuds = new List<Noeud>();
        internal void Activer()
        {
            foreach(Noeud noeud in noeuds)
            {
                noeud.ActiverNoeudEtPrérequis();
            }
        }
    }

    private static int ChoisirIndiceAuHasard(int nombreIndicesPossibles)
    {
        return Mathf.FloorToInt(nombreIndicesPossibles * Random.value);
    }

}
