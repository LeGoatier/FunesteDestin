using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GestionInterfaceCraft : MonoBehaviour
{

    public static GestionInterfaceCraft instance;

    // �l�ments visuels
    [SerializeField] TextMeshProUGUI titre;
    [SerializeField] Button boutonConstruire;
    [SerializeField] Slider sliderConstruction;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] Transform sectionConstruction;
    [SerializeField] Craft[] crafts;

    public Craft craftSelectionn� { get; private set; }

    void Awake()
    {
        instance = this;

        foreach (Craft craft in crafts)
        {
            if (craft.�tat == �tatCraft.Inconnu)
                craft.proposition.GetComponentInChildren<TextMeshProUGUI>().text = "???";
        }
        InstancierCanvasConstruction();
        craftSelectionn� = crafts[0];
        S�lectionnerCraft(0);

        sliderConstruction.value = sliderConstruction.minValue;

    }


    private void OnEnable()
    {
        ModificationInventaire.AppliquerNouveauxCrafts();
        sliderConstruction.value = sliderConstruction.minValue;
        scrollBar.value = 1;
        OrganiserListe();
        UpdateButton();
        craftSelectionn�.canvasConstruction.GetComponent<CanvasConstruction>().UpdateSlots();
    }


    public void Construire()
    {
        if (GestionInventaire.EstCoutRempli(craftSelectionn�.couts))
        {
            boutonConstruire.interactable = false;
            StartCoroutine(AnimerConstruction(craftSelectionn�));
           
        }      
    }

    public void OrganiserListe()
    {
        List<Craft> temp = new List<Craft>(crafts.Length);

        List<Craft> possibles = new List<Craft>();
        List<Craft> d�couvert = new List<Craft>();
        List<Craft> inconnu = new List<Craft>();

        foreach (Craft craft in crafts)
        {
            if (craft.interactable)
            {
                if (GestionInventaire.EstCoutRempli(craft.couts) && (craft.�tat == �tatCraft.D�couvert || craft.�tat == �tatCraft.Possible))
                {
                    craft.�tat = �tatCraft.Possible;
                    possibles.Add(craft);
                }
                else
                {
                    if (craft.�tat == �tatCraft.D�couvert || craft.�tat == �tatCraft.Possible)
                    {
                        craft.�tat = �tatCraft.D�couvert;
                        d�couvert.Add(craft);
                    }
                    else
                    {
                        craft.�tat = �tatCraft.Inconnu;
                        inconnu.Add(craft);
                    }
                }
            }
        }
            
        inconnu.Reverse();
        d�couvert.Reverse();
        possibles.Reverse();

        temp.AddRange(inconnu);
        temp.AddRange(d�couvert);
        temp.AddRange(possibles);

        Craft[] craftsOrdonn�es = temp.ToArray();
        AfficherCrafts(craftsOrdonn�es);

        
    }

    private  void AfficherCrafts(Craft[] craftsOrdonn�es)
    {
        foreach (Craft craft in craftsOrdonn�es)
        {
            craft.proposition.GetComponent<RectTransform>().SetAsFirstSibling();
        }
    }

    public void D�couvrirCraft(Ressource ressource)
    {
        Craft craft = TrouverCraft(ressource);
        craft.proposition.GetComponentInChildren<TextMeshProUGUI>().text = craft.nom;
        craft.�tat = �tatCraft.D�couvert;
    }

    public void D�couvrirCraft(Outil outil)
    {
        Craft craft = TrouverCraft(outil);
        craft.proposition.GetComponentInChildren<TextMeshProUGUI>().text = craft.nom;
        craft.�tat = �tatCraft.D�couvert;
    }

    private Craft TrouverCraft(Outil outil)
    {
        foreach (Craft craft in crafts)
            if (craft.outil == outil)
                return craft;
        return null;
    }
    private Craft TrouverCraft(Ressource ressource)
    {
        foreach (Craft craft in crafts)
            if (craft.ressource == ressource)
                return craft;
        return null;
    }

    // M�thode qui permet au boutons de la liste de craft de changer des proposition 
    public void S�lectionnerCraft(int num�roProposition)
    {
        if (craftSelectionn�.canvasConstruction != null) craftSelectionn�.canvasConstruction.SetActive(false); // D�sactive l'ancien

        craftSelectionn� = crafts[num�roProposition];

       
        if (craftSelectionn�.�tat != �tatCraft.Inconnu)
        {
            titre.text = craftSelectionn�.nom;
            craftSelectionn�.canvasConstruction.GetComponent<CanvasConstruction>().UpdateSlots();
        }
        else
        {
            titre.text = "???";
            
        }

        GestionBruit.instance.JouerSon("ClickCraft");
        UpdateButton();
        

        if (craftSelectionn�.canvasConstruction != null && craftSelectionn�.�tat != �tatCraft.Inconnu) craftSelectionn�.canvasConstruction.SetActive(true);
    }

    public void UpdateButton()
    {
        boutonConstruire.GetComponentInChildren<TextMeshProUGUI>().text = craftSelectionn�.estR�parable ? "R�parer" :"Construire";
        if (craftSelectionn�.�tat == �tatCraft.Possible && craftSelectionn�.canvasConstruction.GetComponent<CanvasConstruction>().EstComplet())
        {
            boutonConstruire.interactable = true;
            
            
        }
        else
        {
            boutonConstruire.interactable = false;
        }

    }

    private IEnumerator AnimerConstruction(Craft craft)
    {
        float vitesseConstruction = sliderConstruction.maxValue / craft.tempsConstruction;
        float timer = 0f;
        
        // Effet sonore lors de la construction
        string nomBruit = UnityEngine.Random.Range(0, 1) > 0.5f ? "Constuire1" : "Constuire2";
        GestionBruit.instance.JouerSon(nomBruit);

        while (timer < craft.tempsConstruction)
        {
            sliderConstruction.value = Mathf.Lerp(sliderConstruction.minValue,sliderConstruction.maxValue, 
                timer / craft.tempsConstruction * sliderConstruction.maxValue);
            timer += 1 / 200f; // arbitraire, puisque le temps est arr�t�, nous devons se fier aux frames
            yield return null;
        }
        GestionBruit.instance.ArreterSon(nomBruit);

        sliderConstruction.value = sliderConstruction.minValue;

        GestionInventaire.AcheterCout(craft.couts);
        if (craft.outil != Outil.Null)
        {
            GestionInventaire.AjouterOutil(craft.outil);
            craft.proposition.GetComponentInChildren<Button>().interactable = false;
            craft.interactable = false;
            craft.�tat = �tatCraft.D�couvert;
        }
        else if (craft.ressource != Ressource.Null)
            GestionInventaire.AjouterRessource(craft.ressource);
        else if (craft.arme != Arme.Null)
        {
            GestionInventaire.AjouterArme(craft.arme);
            if (InventaireFusils.instance.armes�quip�es.Count < 3) InventaireFusils.instance.�quiperArme(craft.arme);
            craft.proposition.GetComponentInChildren<Button>().interactable = false;
            craft.interactable = false;
            craft.�tat = �tatCraft.D�couvert;
        }

        OrganiserListe();
        craft.canvasConstruction.GetComponent<CanvasConstruction>().UpdateSlots();
        UpdateButton();

    }
    
    private void InstancierCanvasConstruction()
    {
        foreach (Craft craft in crafts)
        {
            if (craft.canvasConstruction != null)
            {
                craft.canvasConstruction = Instantiate(craft.canvasConstruction, sectionConstruction);
                craft.canvasConstruction.SetActive(false);
            }
        }
    }
}


[Serializable]
public class Craft
{
    public string nom;
    public GameObject proposition;

    public GameObject canvasConstruction;

    public Outil outil;
    public Ressource ressource;
    public Arme arme;

    public �tatCraft �tat;
    public float tempsConstruction;
    public bool estR�parable;
    public Cout couts;

    public bool interactable = true;
}


public enum �tatCraft { Inconnu, D�couvert, Possible, Nbr�tat }