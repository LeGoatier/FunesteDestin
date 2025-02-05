using UnityEngine;
using UnityEngine.EventSystems;

public class SlotConstruction : Slot
{
    // Ce slot est situé dans le menu de construction, il représente un
    // emplacement que le joueur doit combler avec une ressource spécifique

 
    [SerializeField] Ressource ressource;
    [SerializeField] Outil outil;

    public bool estComblé; // vrai si le slot contient son item correspondant


    // Puisque ce slot ne permute pas deux items comme les autres type de slots,
    //  la méthode Drop() sera override
    protected override void Drop()
    {
        if (EstConditionRemplie())
        {
            SlotContenant contenant = draggable.parentInitial.GetComponent<SlotContenant>();

            contenant.DuppliquerItem(); // Crée un nouvel item pour pouvoir le déplacer
            contenant.RéduireIndice(); // Réduit le nombre d'item disponible en fonction du nombre de ressource

            // Prend l'objet comme nouvel item
            draggable.parentFinal = transform;
            item = draggable;

            estComblé = true;
            GestionInterfaceCraft.instance.UpdateButton(); // pour regarder si toutes les conditions sont rempli
        }
       
    }


    // Vérifie si ce slot n'est pas déjà comblé et si l'item qu'on veux lui envoyer est le type qu'il accepte
    protected override bool EstConditionRemplie()
    {
        return dragItem != null && !estComblé && dragItem.ressource == ressource && dragItem.outil == outil;
    }

}
