using UnityEngine;

public enum TypeInventaire { Ressource, Objet, Arme }
public class Menu_Inventaire : MonoBehaviour
{
    public static Menu_Inventaire instance;

    Sprite[] ressources;
    Sprite[] outils;
    Sprite[] armes;
    Sprite[] soins;

    private Slot[] slotsObjets;
    private Slot[] slotsArmes;
    private Slot[] slotsArmes�quip�es;
    private Slot slotHeal;

    private void Awake()
    {
        instance = this;

        GameObject[] objs = GameObject.FindGameObjectsWithTag("SlotObjet");
        slotsObjets = new Slot[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            slotsObjets[i] = objs[i].GetComponent<Slot>();
            objs[i].GetComponent<RectTransform>().SetAsLastSibling();
        }

        GameObject[] temp = GameObject.FindGameObjectsWithTag("SlotArme");
        slotsArmes = new Slot[temp.Length];
        for (int i = 0; i < temp.Length; i++)
        {
            slotsArmes[i] = temp[i].GetComponent<Slot>();
            temp[i].GetComponent<RectTransform>().SetAsLastSibling();
        }


        GameObject[] temp1 = GameObject.FindGameObjectsWithTag("SlotArme�quip�e");
        slotsArmes�quip�es = new Slot[temp1.Length];
        for (int i = 0; i < temp1.Length; i++)
        {
            slotsArmes�quip�es[i] = temp1[i].GetComponent<Slot>();
            temp1[i].GetComponent<RectTransform>().SetAsLastSibling();
        }

        slotHeal = GameObject.FindGameObjectWithTag("SlotHeal").GetComponent<Slot>();
        slotHeal.GetComponent<RectTransform>().SetAsLastSibling();
        

        ressources = ComportementInterface.instance.ressources;
        outils = ComportementInterface.instance.outils;
        armes = ComportementInterface.instance.armes;
        soins = ComportementInterface.instance.soins;
    }
  

    private void OnEnable()
    {
        ModificationInventaire.AppliquerModifications();
    }

    public void Afficher(Ressource ressource, int compte)
    {
        Item item = slotsObjets[TrouverIndice(ressource, Outil.Null, Soin.Null)].GetComponentInChildren<Item>();
        if (compte > 0)
            item.AfficherItem(ressources[(int)ressource], ressource, Outil.Null, Soin.Null, compte);
        else
            item.AfficherVide();

    }

    public void Afficher(Outil outil, bool actif)
    {
        Item item = slotsObjets[TrouverIndice(Ressource.Null, outil, Soin.Null)].GetComponentInChildren<Item>();

        if (actif)
            item.AfficherItem(outils[(int)outil], Ressource.Null, outil, Soin.Null);
        else
            item.AfficherVide();
    }

    public void Afficher(Arme arme)
    {
        int indice = TrouverIndiceArme();
        Item item = null;
        
        if (indice < slotsArmes�quip�es.Length && indice != -1)
        {
            item = slotsArmes�quip�es[indice].GetComponentInChildren<Item>();
        }
        else if (indice != -1)
        {
            item = slotsArmes[indice - slotsArmes�quip�es.Length].GetComponentInChildren<Item>();
        }
        item.AfficherItem(armes[(int)arme], arme);
    }

   
    public void Afficher(Soin soin, int compte)
    {
        Item item;
        
        if (compte == 1 && slotHeal.GetComponentInChildren<Item>().estVide || slotHeal.GetComponentInChildren<Item>().soin == soin)
        {
            item = slotHeal.GetComponentInChildren<Item>();
            slotHeal.GetComponent<SlotHeal>().croix.gameObject.SetActive(false);
        }
        else
        {
            item = slotsObjets[TrouverIndice(Ressource.Null, Outil.Null, soin)].GetComponentInChildren<Item>();
        }


        if (compte > 0)
        {
            item.AfficherItem(soins[(int)soin], Ressource.Null, Outil.Null, soin, compte);
        }
        else
        {
            item.AfficherVide(); 
            slotHeal.GetComponent<SlotHeal>().croix.gameObject.SetActive(true);
        }
            


    }

    private int TrouverIndice(Ressource ressource, Outil outil, Soin soin)
    {

        Item item;
        int indicePremierVide = -1;
        for (int i = 0; i < slotsObjets.Length; i++)
        {
            
            item = slotsObjets[i].GetComponentInChildren<Item>();
            if (!item.estVide)
            {
                if (item.ressource == ressource && item.outil == outil && item.soin == soin)
                    return i;
            }
            else if (indicePremierVide == -1) // si l'item est vide
            {
                indicePremierVide = i;
            }

        }
        return indicePremierVide;
    }

    

    private int TrouverIndiceArme()
    {
        for (int i = 0; i < slotsArmes�quip�es.Length; i++)
        {
            Item item = slotsArmes�quip�es[i].GetComponentInChildren<Item>();
            if (item.estVide)
                return i;
        }

        for (int i = 0; i < slotsArmes.Length; i++)
        {
            Item item = slotsArmes[i].GetComponentInChildren<Item>();
            if (item.estVide)
                return i + slotsArmes�quip�es.Length;
        }
        return -1;
    }

    public void V�rifierQueDraggableEstAssoci�()
    {
        if (ComportementInterface.instance.draggableActuel != null && ComportementInterface.instance.draggableActuel.transform.parent.GetComponent<Slot>() == null)
        {
            ComportementInterface.instance.draggableActuel.OnEndDrag(null);
        }
    }
}
