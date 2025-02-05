using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotObjet : Slot
{
    // Puisque certain objet peuvent se déplacer du slot objet au slot heal, l'idée est 
    // de simplement replacer l'image de fond du slot heal (la croix) si il est vidé
    protected override void Spécificités()
    {
        SlotHeal slotHeal = draggable.parentInitial.gameObject.GetComponent<SlotHeal>();
        if (slotHeal != null) //si l'item provient du slot heal
        {
            slotHeal.croix.gameObject.SetActive(!(item.GetComponent<Item>().soin != Soin.Null));
        }
    }

    protected override bool EstConditionRemplie()
    {
        return dragItem != null && dragItem.arme == Arme.Null && VérifierSiPeutÉchangerAvecSlotHeal(draggable, dragItem);
    }


    // Le but de cette méthode est de s'assurer que l'on ne va pas changer un objet avec un soin, car
    // avoir un objet comme soin causerait des problèmes lorsqu'on voudrait se soigner
    private bool VérifierSiPeutÉchangerAvecSlotHeal(Draggable draggable, Item dragItem)
    {
        return dragItem.soin == Soin.Null || !(draggable.parentInitial.gameObject.GetComponent<SlotHeal>() &&
            !(item.GetComponent<Item>().estVide || item.GetComponent<Item>().soin != Soin.Null)); ;
    }

}
