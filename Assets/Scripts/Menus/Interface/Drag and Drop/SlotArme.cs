using UnityEngine;
using UnityEngine.EventSystems;

public class SlotArme : Slot
{
    // L'idée ici est de regarder si l'objet était une arme équipée pour pouvoir la déséquiper ou équiper 
    // une nouvelle si ce slot en contenait une
    protected override void Spécificités()
    {
        SlotArmeÉquipée slotArmeÉquipée = draggable.parentInitial.gameObject.GetComponent<SlotArmeÉquipée>();
        if (slotArmeÉquipée != null) // provient d'un slot arme équipé
        {
            InventaireFusils.instance.DéséquiperArme(dragItem.arme);

            if (item.GetComponent<Item>().arme != Arme.Null) // si on drag une arme
            {
                InventaireFusils.instance.ÉquiperArme(item.GetComponent<Item>().arme);
            }

        }
    }

    protected override bool EstConditionRemplie()
    {
        return draggable != null && draggable.peutDrag && dragItem.ressource == Ressource.Null &&
            dragItem.outil == Outil.Null && dragItem.soin == Soin.Null && dragItem.arme != Arme.Null;
    }
}
