using UnityEngine;
using UnityEngine.EventSystems;

public class SlotArme : Slot
{
    // L'id�e ici est de regarder si l'objet �tait une arme �quip�e pour pouvoir la d�s�quiper ou �quiper 
    // une nouvelle si ce slot en contenait une
    protected override void Sp�cificit�s()
    {
        SlotArme�quip�e slotArme�quip�e = draggable.parentInitial.gameObject.GetComponent<SlotArme�quip�e>();
        if (slotArme�quip�e != null) // provient d'un slot arme �quip�
        {
            InventaireFusils.instance.D�s�quiperArme(dragItem.arme);

            if (item.GetComponent<Item>().arme != Arme.Null) // si on drag une arme
            {
                InventaireFusils.instance.�quiperArme(item.GetComponent<Item>().arme);
            }

        }
    }

    protected override bool EstConditionRemplie()
    {
        return draggable != null && draggable.peutDrag && dragItem.ressource == Ressource.Null &&
            dragItem.outil == Outil.Null && dragItem.soin == Soin.Null && dragItem.arme != Arme.Null;
    }
}
