using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotArmeÉquipée : Slot
{

    [SerializeField] int index; // indice correspondant à la position de l'arme équipé

    // L'idée ici est de regarder si l'objet était une arme équipée pour voir si il faut simplement inverser
    // deux armes déjà équipé ou en déséquiper une et en équiper une nouvelle
    protected override void Spécificités()
    {
        SlotArmeÉquipée slotArmeÉquipée = draggable.parentInitial.gameObject.GetComponent<SlotArmeÉquipée>();
        if (slotArmeÉquipée != null) // provient d'un slot arme équipé
        {
            InventaireFusils.instance.InverserArmeSlot1ArmeSlot2();
        }
        else // provient d'un slot arme
        {
            if (item.GetComponent<Item>().arme != Arme.Null) InventaireFusils.instance.DéséquiperArme(item.GetComponent<Item>().arme);
            InventaireFusils.instance.ÉquiperArme(dragItem.arme, index);
        }
    }

    protected override bool EstConditionRemplie()
    {
        return draggable != null && draggable.peutDrag && dragItem.ressource == Ressource.Null &&
            dragItem.outil == Outil.Null && dragItem.soin == Soin.Null && dragItem.arme != Arme.Null;
    }
}

