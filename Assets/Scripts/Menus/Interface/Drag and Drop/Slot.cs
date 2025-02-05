using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Slot : MonoBehaviour, IDropHandler
{
    public Draggable item; // l'objet contenu dans cet emplacement (slot)

    protected GameObject dropped; // le gameObject qui a été déplacé
    protected Draggable draggable; // le script Draggable contenu dans dropped
    protected Item dragItem; // le script Item contenu dans dropped

    public virtual void Awake()
    {
        item = GetComponentInChildren<Draggable>(); // l'objet qui peut être déplacé contenu de base dans le slot
    }


    // Méthode implémenté de IDropHandler, qui est appelée lorsque le joueur lâche l'objet au dessus du slot
    public void OnDrop(PointerEventData eventData)
    {
        dropped = eventData.pointerDrag;
        draggable = dropped.GetComponent<Draggable>();
        dragItem = dropped.GetComponent<Item>();

        Drop();
    }

   
    // Cette méthode permet de permutter les deux items dans les slots impliqués et offre
    // la possibilité d'ajouter instructions supplémentaires (spécificités)
    protected virtual void Drop()
    {
        if(EstConditionRemplie())
        {
            // Fait en sorte que l'item contenu dans le slot actuel se place dans lui dont l'item déplacé provient
            item.transform.SetParent(draggable.parentInitial);
            item.transform.localPosition = new Vector3(0, 0, 0);
            draggable.parentInitial.gameObject.GetComponent<Slot>().item = item;

            // Effectu les étapes spécifique au type de slot
            Spécificités();

            // Place l'item déplacé dans le slot actuel
            draggable.parentFinal = transform;
            item = draggable;
        }
    }

    // Ici, les enfants devrons spécifier les conditions que doit avoir l'item 
    // qui souhaite être déposé pour accepter celui-ci
    protected abstract bool EstConditionRemplie();

    // Méthode qui sera override dans la plupart des cas
    // Elle sert à écrire les étapes spécifiques à chaque type de slot
    protected virtual void Spécificités() { }

    // Méthode qui sera appelé par le Event Trigger lorsque le joueur passe sa souris sur le slot
    // Permet d'afficher les nom de l'objet contenu dans le slot
    public void AfficherInfos(bool afficher)
    {
        GestionBruit.instance.JouerSon("SelectInventaire");
        Item item = GetComponentInChildren<Item>();
        if(item != null) item.AfficherInfos(afficher);
    }
}
