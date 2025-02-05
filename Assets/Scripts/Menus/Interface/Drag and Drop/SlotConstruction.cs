using UnityEngine;
using UnityEngine.EventSystems;

public class SlotConstruction : Slot
{
    // Ce slot est situ� dans le menu de construction, il repr�sente un
    // emplacement que le joueur doit combler avec une ressource sp�cifique

 
    [SerializeField] Ressource ressource;
    [SerializeField] Outil outil;

    public bool estCombl�; // vrai si le slot contient son item correspondant


    // Puisque ce slot ne permute pas deux items comme les autres type de slots,
    //  la m�thode Drop() sera override
    protected override void Drop()
    {
        if (EstConditionRemplie())
        {
            SlotContenant contenant = draggable.parentInitial.GetComponent<SlotContenant>();

            contenant.DuppliquerItem(); // Cr�e un nouvel item pour pouvoir le d�placer
            contenant.R�duireIndice(); // R�duit le nombre d'item disponible en fonction du nombre de ressource

            // Prend l'objet comme nouvel item
            draggable.parentFinal = transform;
            item = draggable;

            estCombl� = true;
            GestionInterfaceCraft.instance.UpdateButton(); // pour regarder si toutes les conditions sont rempli
        }
       
    }


    // V�rifie si ce slot n'est pas d�j� combl� et si l'item qu'on veux lui envoyer est le type qu'il accepte
    protected override bool EstConditionRemplie()
    {
        return dragItem != null && !estCombl� && dragItem.ressource == ressource && dragItem.outil == outil;
    }

}
