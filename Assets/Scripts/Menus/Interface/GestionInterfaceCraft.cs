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

    // Éléments visuels
    [SerializeField] TextMeshProUGUI titre;
    [SerializeField] Button boutonConstruire;
    [SerializeField] Slider sliderConstruction;
    [SerializeField] Scrollbar scrollBar;
    [SerializeField] Transform sectionConstruction;
    [SerializeField] Craft[] crafts;

    public Craft craftSelectionné { get; private set; }

    void Awake()
    {
        instance = this;

        foreach (Craft craft in crafts)
        {
            if (craft.état == ÉtatCraft.Inconnu)
                craft.proposition.GetComponentInChildren<TextMeshProUGUI>().text = "???";
        }
        InstancierCanvasConstruction();
        craftSelectionné = crafts[0];
        SélectionnerCraft(0);

        sliderConstruction.value = sliderConstruction.minValue;

    }


    private void OnEnable()
    {
        ModificationInventaire.AppliquerNouveauxCrafts();
        sliderConstruction.value = sliderConstruction.minValue;
        scrollBar.value = 1;
        OrganiserListe();
        UpdateButton();
        craftSelectionné.canvasConstruction.GetComponent<CanvasConstruction>().UpdateSlots();
    }


    public void Construire()
    {
        if (GestionInventaire.EstCoutRempli(craftSelectionné.couts))
        {
            boutonConstruire.interactable = false;
            StartCoroutine(AnimerConstruction(craftSelectionné));
           
        }      
    }

    public void OrganiserListe()
    {
        List<Craft> temp = new List<Craft>(crafts.Length);

        List<Craft> possibles = new List<Craft>();
        List<Craft> découvert = new List<Craft>();
        List<Craft> inconnu = new List<Craft>();

        foreach (Craft craft in crafts)
        {
            if (craft.interactable)
            {
                if (GestionInventaire.EstCoutRempli(craft.couts) && (craft.état == ÉtatCraft.Découvert || craft.état == ÉtatCraft.Possible))
                {
                    craft.état = ÉtatCraft.Possible;
                    possibles.Add(craft);
                }
                else
                {
                    if (craft.état == ÉtatCraft.Découvert || craft.état == ÉtatCraft.Possible)
                    {
                        craft.état = ÉtatCraft.Découvert;
                        découvert.Add(craft);
                    }
                    else
                    {
                        craft.état = ÉtatCraft.Inconnu;
                        inconnu.Add(craft);
                    }
                }
            }
        }
            
        inconnu.Reverse();
        découvert.Reverse();
        possibles.Reverse();

        temp.AddRange(inconnu);
        temp.AddRange(découvert);
        temp.AddRange(possibles);

        Craft[] craftsOrdonnées = temp.ToArray();
        AfficherCrafts(craftsOrdonnées);

        
    }

    private  void AfficherCrafts(Craft[] craftsOrdonnées)
    {
        foreach (Craft craft in craftsOrdonnées)
        {
            craft.proposition.GetComponent<RectTransform>().SetAsFirstSibling();
        }
    }

    public void DécouvrirCraft(Ressource ressource)
    {
        Craft craft = TrouverCraft(ressource);
        craft.proposition.GetComponentInChildren<TextMeshProUGUI>().text = craft.nom;
        craft.état = ÉtatCraft.Découvert;
    }

    public void DécouvrirCraft(Outil outil)
    {
        Craft craft = TrouverCraft(outil);
        craft.proposition.GetComponentInChildren<TextMeshProUGUI>().text = craft.nom;
        craft.état = ÉtatCraft.Découvert;
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

    // Méthode qui permet au boutons de la liste de craft de changer des proposition 
    public void SélectionnerCraft(int numéroProposition)
    {
        if (craftSelectionné.canvasConstruction != null) craftSelectionné.canvasConstruction.SetActive(false); // Désactive l'ancien

        craftSelectionné = crafts[numéroProposition];

       
        if (craftSelectionné.état != ÉtatCraft.Inconnu)
        {
            titre.text = craftSelectionné.nom;
            craftSelectionné.canvasConstruction.GetComponent<CanvasConstruction>().UpdateSlots();
        }
        else
        {
            titre.text = "???";
            
        }

        GestionBruit.instance.JouerSon("ClickCraft");
        UpdateButton();
        

        if (craftSelectionné.canvasConstruction != null && craftSelectionné.état != ÉtatCraft.Inconnu) craftSelectionné.canvasConstruction.SetActive(true);
    }

    public void UpdateButton()
    {
        boutonConstruire.GetComponentInChildren<TextMeshProUGUI>().text = craftSelectionné.estRéparable ? "Réparer" :"Construire";
        if (craftSelectionné.état == ÉtatCraft.Possible && craftSelectionné.canvasConstruction.GetComponent<CanvasConstruction>().EstComplet())
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
            timer += 1 / 200f; // arbitraire, puisque le temps est arrêté, nous devons se fier aux frames
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
            craft.état = ÉtatCraft.Découvert;
        }
        else if (craft.ressource != Ressource.Null)
            GestionInventaire.AjouterRessource(craft.ressource);
        else if (craft.arme != Arme.Null)
        {
            GestionInventaire.AjouterArme(craft.arme);
            if (InventaireFusils.instance.armesÉquipées.Count < 3) InventaireFusils.instance.ÉquiperArme(craft.arme);
            craft.proposition.GetComponentInChildren<Button>().interactable = false;
            craft.interactable = false;
            craft.état = ÉtatCraft.Découvert;
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

    public ÉtatCraft état;
    public float tempsConstruction;
    public bool estRéparable;
    public Cout couts;

    public bool interactable = true;
}


public enum ÉtatCraft { Inconnu, Découvert, Possible, NbrÉtat }