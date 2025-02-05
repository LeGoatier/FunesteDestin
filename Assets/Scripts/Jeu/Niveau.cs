using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Niveau 
{
    /*BrainStorm: chaque niveau cr�er sera une instance de cette classe, les attributs donnerons l'information
     * n�cessaire a "l'instantiateur de niveau (pou l'instant le feu de camps, � voir)cr�er un niveau coh�rent. 
     * Voici quelques exemple d'attribut de niveau
    */


    //Ici on multiplie cette constante par le num�ro du niveau pour obtenir le rayon du niveau
    //Cela implique que la variation de taille de la temp�te augmente du m�me montant � chaque fois
    const float MulitplicateurDeRayon = 10;

    public int Num�roNiveau { get; private set; }
    public float RayonNiveau
    {
        get { return Num�roNiveau * MulitplicateurDeRayon; }
        
    }

    public List<GameObject> listeGameobjectsPositionPr�cise;
    public List<GameObject> listeGameobjectsPositionAl�atoire;



    public Niveau(int num�roNiveau, List<GameObject> lolsie, List<GameObject> PositionAl�atoire)
    {
        Num�roNiveau = num�roNiveau;
        lolsie = listeGameobjectsPositionPr�cise;
        PositionAl�atoire = listeGameobjectsPositionAl�atoire;
    }

}
