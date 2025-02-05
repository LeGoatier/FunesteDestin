using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlotContenant : Slot
{
    // Ce slot est situ� dans le menu de construction, il repr�sente un
    // emplacement o� le joueur peut acc�der � ces ressources pour les prendres
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
    //  la m�thode Drop() sera override
    protected override void Drop()
    {
        if (EstConditionRemplie())
        {
            SlotConstruction slotConstruction = draggable.parentInitial.GetComponent<SlotConstruction>();
            if (slotConstruction != null) slotConstruction.estCombl� = false;

            AugmenterIndice(); // Augmente le nombre de ressources disponible
            Destroy(draggable.gameObject); // Puisque l'item est de surplus, on le supprime

            GestionInterfaceCraft.instance.UpdateButton(); // On r��value si l'on peut d�sormais contruire l'objet
        }
    }

    // V�rifie si l'item qui lui arrive correspond au type qu'il produit
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

    // R�duit le nombre de ressources restantes dans l'inventaire (si on construit)
    public void R�duireIndice()
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

    // Duplique l'item pour que le joueur puisse en drag un � nouveau
    // N�cessaire, car � la diff�rence des autres slots, celui-ci ne re�oit pas un item
    // � tous les coups, il doit donc en avoir un au cas o�
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
