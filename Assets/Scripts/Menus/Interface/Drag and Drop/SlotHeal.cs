using UnityEngine;
using UnityEngine.EventSystems;

public class SlotHeal : Slot
{
    public UnityEngine.UI.Image croix; // l'image de fond de ce slot
    
    public override void Awake()
    {
        base.Awake();

        // pour aller chercher le component de l'image voulue
        UnityEngine.UI.Image[] images = GetComponentsInChildren<UnityEngine.UI.Image>();
        foreach(UnityEngine.UI.Image i in images)
            if (i.gameObject.name == "Croix")
            {
                croix = i;
                break;
            }
    }

    // Désactive simplement la croix lorsque le slot est rempli
    protected override void Spécificités()
    {
        croix.gameObject.SetActive(false);
    }

    protected override bool EstConditionRemplie()
    {
        return draggable != null && draggable.peutDrag && dragItem.ressource == Ressource.Null && (dragItem.soin != Soin.Null || dragItem.estVide);
    }
}