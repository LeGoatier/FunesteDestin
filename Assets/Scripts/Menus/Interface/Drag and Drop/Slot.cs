using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IDropHandler
{
    public Draggable item; // l'objet contenu dans cet emplacement (slot)

    protected GameObject dropped; // le gameObject qui a �t� d�plac�
    protected Draggable draggable; // le script Draggable contenu dans dropped
    protected Item dragItem; // le script Item contenu dans dropped

    public virtual void Awake()
    {
        item = GetComponentInChildren<Draggable>(); // l'objet qui peut �tre d�plac� contenu de base dans le slot
    }


    // M�thode impl�ment� de IDropHandler, qui est appel�e lorsque le joueur l�che l'objet au dessus du slot
    public void OnDrop(PointerEventData eventData)
    {
        dropped = eventData.pointerDrag;
        draggable = dropped.GetComponent<Draggable>();
        dragItem = dropped.GetComponent<Item>();

        Drop();
    }

   
    // Cette m�thode permet de permutter les deux items dans les slots impliqu�s et offre
    // la possibilit� d'ajouter instructions suppl�mentaires (sp�cificit�s)
    protected virtual void Drop()
    {
        if(EstConditionRemplie())
        {
            // Fait en sorte que l'item contenu dans le slot actuel se place dans lui dont l'item d�plac� provient
            item.transform.SetParent(draggable.parentInitial);
            item.transform.localPosition = new Vector3(0, 0, 0);
            draggable.parentInitial.gameObject.GetComponent<Slot>().item = item;

            // Effectu les �tapes sp�cifique au type de slot
            Sp�cificit�s();

            // Place l'item d�plac� dans le slot actuel
            draggable.parentFinal = transform;
            item = draggable;
        }
    }

    // Ici, les enfants devrons sp�cifier les conditions que doit avoir l'item 
    // qui souhaite �tre d�pos� pour accepter celui-ci
    protected abstract bool EstConditionRemplie();

    // M�thode qui sera override dans la plupart des cas
    // Elle sert � �crire les �tapes sp�cifiques � chaque type de slot
    protected virtual void Sp�cificit�s() { }

    // M�thode qui sera appel� par le Event Trigger lorsque le joueur passe sa souris sur le slot
    // Permet d'afficher les nom de l'objet contenu dans le slot
    public void AfficherInfos(bool afficher)
    {
        GestionBruit.instance.JouerSon("SelectInventaire");
        Item item = GetComponentInChildren<Item>();
        if(item != null) item.AfficherInfos(afficher);
    }
}
