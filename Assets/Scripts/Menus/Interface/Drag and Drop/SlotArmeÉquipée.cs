using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotArme�quip�e : Slot
{

    [SerializeField] int index; // indice correspondant � la position de l'arme �quip�

    // L'id�e ici est de regarder si l'objet �tait une arme �quip�e pour voir si il faut simplement inverser
    // deux armes d�j� �quip� ou en d�s�quiper une et en �quiper une nouvelle
    protected override void Sp�cificit�s()
    {
        SlotArme�quip�e slotArme�quip�e = draggable.parentInitial.gameObject.GetComponent<SlotArme�quip�e>();
        if (slotArme�quip�e != null) // provient d'un slot arme �quip�
        {
            InventaireFusils.instance.InverserArmeSlot1ArmeSlot2();
        }
        else // provient d'un slot arme
        {
            if (item.GetComponent<Item>().arme != Arme.Null) InventaireFusils.instance.D�s�quiperArme(item.GetComponent<Item>().arme);
            InventaireFusils.instance.�quiperArme(dragItem.arme, index);
        }
    }

    protected override bool EstConditionRemplie()
    {
        return draggable != null && draggable.peutDrag && dragItem.ressource == Ressource.Null &&
            dragItem.outil == Outil.Null && dragItem.soin == Soin.Null && dragItem.arme != Arme.Null;
    }
}

