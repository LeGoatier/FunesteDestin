using System.Collections.Generic;
using UnityEngine;

//Positionfixe: on set une position non aléatoire, comme le feu de camp (0,0,0) => doit définir propriété position
//PositionNiveau: ce met entre les bornes d'un niveau spécifique => doit définir propriété NiveauSpawn
//positionBorné: position aléatoire entre 2 rayon (ex: ressources) => doit définir propriétés rayonMin, rayonMax
//positionCondition: poisition définie selon une condition (ex: bateau est proche de l'eau) => doit définir fonction ConditionSpawn
public enum ÉtatInitialisation { positionFixe,positionNiveau, positionBorné, positionCondition };

public abstract class ComportementStructure : Interagir
{
    [SerializeField] protected Canvas canevas;
    public abstract Étapes ÉtapeLiée { get;}
    public ÉtatInitialisation étatInitialisation { get; protected set; }
    protected GestionInteraction Interaction;
    protected List<DataObjectInteragissable> ObjectRamassableVisible;

    public Cout CoutStructure = new Cout();

    //Variable pour placer la structure sur la scène
    public Vector2 Position { get; protected set; } //position (X,Z) (y déterminer lors du placement)
    public int NiveauSpawn { get; protected set; }
    public int RayonMin { get; protected set; }
    public int RayonMax { get; protected set; }
    public virtual List<Vector3> ConditionSpawn() { return null; } //À override si un objet à des conditions de spawn et DOIT avoir comme condition d'être plus loins que le rayon du niveau 0
    
    protected bool EstPremierItérationCollider = true;
    

    protected abstract void EstEntréEnCollision(Collider other);
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

    

    protected virtual bool SontPréalablesRemplis()
    {
        return GestionInventaire.EstCoutRempli(CoutStructure) ;
    }

    


    private void OnTriggerEnter(Collider other)
    {
        EstEntréEnCollision(other);
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
