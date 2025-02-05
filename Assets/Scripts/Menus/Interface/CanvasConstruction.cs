using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class CanvasConstruction : MonoBehaviour
{
    [SerializeField] Color couleurInvalide;
    [SerializeField] Color couleurValide;
    SlotConstruction[] slotsConstruction;
    SlotContenant[] slotsContenant;

    // Start is called before the first frame update
    void Awake()
    {
        slotsConstruction = GetComponentsInChildren<SlotConstruction>();
        slotsContenant = GetComponentsInChildren<SlotContenant>();
    }

    public bool EstComplet()
    {
        bool succes = false;
        foreach (SlotConstruction slot in slotsConstruction)
        {
            succes = slot.estCombl�;
            if (!succes)
            {
                break;
            }
        }
        return succes;
    }

    public void UpdateSlots()
    {
        foreach (SlotContenant slot in slotsContenant)
        {
            if (slot.ressource != Ressource.Null)
            {
                int indice = GestionInventaire.ObtenirRessource(slot.ressource);
                slot.AfficherNombre(indice);
                if (indice >= GestionInterfaceCraft.instance.craftSelectionn�.couts.ressourcesRequises[(int)slot.ressource])
                {
                    slot.ChangerCouleurIndice(couleurValide);
                }
                else
                {
                    slot.ChangerCouleurIndice(couleurInvalide);
                }
       
                slot.UpdatePeutDrag(indice);
            }
            else
            {
                bool estPr�sentInventaire = GestionInventaire.ObtenirOutil(slot.outil);
                if (estPr�sentInventaire)
                {
                    
                }
                else
                {

                }
                slot.UpdatePeutDrag(estPr�sentInventaire);
            }

            
        }
        ViderSlotConstruction();
    }

    public void ViderSlotConstruction()
    {
        foreach (SlotConstruction slot in slotsConstruction)
        {
            Item item = slot.GetComponentInChildren<Item>();
            if (item != null)
            {
                slot.estCombl� = false;
                Destroy(item.gameObject);

            }
        }
    }


}
