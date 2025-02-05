using System.Collections.Generic;
using UnityEngine;

//Positionfixe: on set une position non al�atoire, comme le feu de camp (0,0,0) => doit d�finir propri�t� position
//PositionNiveau: ce met entre les bornes d'un niveau sp�cifique => doit d�finir propri�t� NiveauSpawn
//positionBorn�: position al�atoire entre 2 rayon (ex: ressources) => doit d�finir propri�t�s rayonMin, rayonMax
//positionCondition: poisition d�finie selon une condition (ex: bateau est proche de l'eau) => doit d�finir fonction ConditionSpawn
public enum �tatInitialisation { positionFixe,positionNiveau, positionBorn�, positionCondition };

public abstract class ComportementStructure : Interagir
{
    [SerializeField] protected Canvas canevas;
    public abstract �tapes �tapeLi�e { get;}
    public �tatInitialisation �tatInitialisation { get; protected set; }
    protected GestionInteraction Interaction;
    protected List<DataObjectInteragissable> ObjectRamassableVisible;

    public Cout CoutStructure = new Cout();

    //Variable pour placer la structure sur la sc�ne
    public Vector2 Position { get; protected set; } //position (X,Z) (y d�terminer lors du placement)
    public int NiveauSpawn { get; protected set; }
    public int RayonMin { get; protected set; }
    public int RayonMax { get; protected set; }
    public virtual List<Vector3> ConditionSpawn() { return null; } //� override si un objet � des conditions de spawn et DOIT avoir comme condition d'�tre plus loins que le rayon du niveau 0
    
    protected bool EstPremierIt�rationCollider = true;
    

    protected abstract void EstEntr�EnCollision(Collider other);
    protected abstract void EstSortiDeCollision(Collider other);

    protected void InitialiserComportementStructure()
    {
        GameObject joueur = GameObject.FindWithTag("Player");

        Interaction = joueur.GetComponent<GestionInteraction>();
        ObjectRamassableVisible = Interaction.ObjetInteragissableVisible;

        if(canevas != null)
        {
            canevas.enabled = false;
        }
        

        if (CoutStructure == null)
        {
            CoutStructure = new Cout();
        }
    }

    

    protected virtual bool SontPr�alablesRemplis()
    {
        return GestionInventaire.EstCoutRempli(CoutStructure) ;
    }

    


    private void OnTriggerEnter(Collider other)
    {
        EstEntr�EnCollision(other);
        if(canevas != null)
        {
            canevas.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        EstSortiDeCollision(other);
        if (canevas != null)
        {
            canevas.enabled = false;
        }
    }
}
