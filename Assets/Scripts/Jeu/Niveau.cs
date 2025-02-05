using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Niveau 
{
    /*BrainStorm: chaque niveau créer sera une instance de cette classe, les attributs donnerons l'information
     * nécessaire a "l'instantiateur de niveau (pou l'instant le feu de camps, à voir)créer un niveau cohérent. 
     * Voici quelques exemple d'attribut de niveau
    */


    //Ici on multiplie cette constante par le numéro du niveau pour obtenir le rayon du niveau
    //Cela implique que la variation de taille de la tempête augmente du même montant à chaque fois
    const float MulitplicateurDeRayon = 10;

    public int NuméroNiveau { get; private set; }
    public float RayonNiveau
    {
        get { return NuméroNiveau * MulitplicateurDeRayon; }
        
    }

    public List<GameObject> listeGameobjectsPositionPrécise;
    public List<GameObject> listeGameobjectsPositionAléatoire;



    public Niveau(int numéroNiveau, List<GameObject> lolsie, List<GameObject> PositionAléatoire)
    {
        NuméroNiveau = numéroNiveau;
        lolsie = listeGameobjectsPositionPrécise;
        PositionAléatoire = listeGameobjectsPositionAléatoire;
    }

}
