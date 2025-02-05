using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotContenant : Slot
{
    // Ce slot est situé dans le menu de construction, il représente un
    // emplacement où le joueur peut accéder à ces ressources pour les prendres
    // et les mettres dans les slotsConstructions

    TextMeshProUGUI nombre;
    int indice;

    public Ressource ressource;
    public Outil outil;

    private GameObject itemBase;

    public override void Awake()
    {
        base.Awake();
        nombre = GetComponentInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] temp = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmp in temp)
        {
            if (tmp.gameObject.name == "Nombre")
            {
                nombre = tmp;
            }
        }
        Item item = GetComponentInChildren<Item>();
        ressource = item.ressource;
        outil = item.outil;
        itemBase = Instantiate(item.gameObject);
    }

    private void Update()
    {
        // Simplement pour le visuel des emplacements contenants des outils
        if (!item.peutDrag) item.gameObject.SetActive(false);
        else item.gameObject.SetActive(true);
    }

    // Puisque ce slot ne permute pas deux items comme les autres type de slots,
    //  la méthode Drop() sera override
    protected override void Drop()
    {
        if (EstConditionRemplie())
        {
            SlotConstruction slotConstruction = draggable.parentInitial.GetComponent<SlotConstruction>();
            if (slotConstruction != null) slotConstruction.estComblé = false;

            AugmenterIndice(); // Augmente le nombre de ressources disponible
            Destroy(draggable.gameObject); // Puisque l'item est de surplus, on le supprime

            GestionInterfaceCraft.instance.UpdateButton(); // On réévalue si l'on peut désormais contruire l'objet
        }
    }

    // Vérifie si l'item qui lui arrive correspond au type qu'il produit
    protected override bool EstConditionRemplie()
    {
        return draggable != null && draggable != item && dragItem.ressource == ressource && dragItem.outil == outil;
    }


    // Affiche le nombre de ressources restantes dans l'inventaire (si on construit)
    public void AfficherNombre(int nombre)
    {
        indice = nombre;
        this.nombre.text = $"{nombre}";
    }

    // Réduit le nombre de ressources restantes dans l'inventaire (si on construit)
    public void RéduireIndice()
    {
        if (indice - 1 >= 0)
        {
            indice--;
            AfficherNombre(indice);
        }
        UpdatePeutDrag(indice);

    }

    // Augmente le nombre de ressources restantes dans l'inventaire (si on construit)
    private void AugmenterIndice()
    {
        indice++;
        AfficherNombre(indice);
        UpdatePeutDrag(indice);
    }

    // Change la couleur pour rouge si le joueur n'a pas suffisament de cette ressource
    // pour construire l'objet ou pour vert s'il en a suffisament
    public void ChangerCouleurIndice(Color couleur)
    {
        nombre.color = couleur;
    }

    // Duplique l'item pour que le joueur puisse en drag un à nouveau
    // Nécessaire, car à la différence des autres slots, celui-ci ne reçoit pas un item
    // à tous les coups, il doit donc en avoir un au cas où
    public void DuppliquerItem()
    {
        GameObject nouvelItem = Instantiate(itemBase, transform);
        nouvelItem.transform.SetSiblingIndex(1);
        nouvelItem.transform.localPosition = new UnityEngine.Vector3(0, 0, 0);
        nouvelItem.transform.localScale = itemBase.transform.localScale;
        nouvelItem.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
        item = nouvelItem.GetComponentInChildren<Draggable>();
    }


    public void UpdatePeutDrag(int nombre)
    {
        item.peutDrag = nombre == 0 ? false : true;
    }

    
    public void UpdatePeutDrag(bool outilPresent)
    {
        item.peutDrag = outilPresent;

    }
}
