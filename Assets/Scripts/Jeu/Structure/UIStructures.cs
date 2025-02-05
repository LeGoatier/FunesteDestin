using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIStructures : MonoBehaviour
{
    
    [SerializeField] IntrantUI[] ÉlémentAffichés;

    List<AffichageUI> listeUI = new List<AffichageUI>();
    Color rouge;
    Color vert;
    Cout CoutStructure;
    


    private void Awake()
    {
        rouge = new(0.5f, 0, 0, 1);
        vert = new(0.133f, 0.545f, 0.133f, 1);
    }
    private void Start()
    {
        CoutStructure = GetComponentInParent<ComportementStructure>().CoutStructure;
        GestionInteraction.OnInteraction += UpdateUI;
        Initialiser();
    }

    
    void Update()
    {
        Vector3 directionToCamera = transform.position - GameManager.instance.GetPositionCam();

        // Pour que le slider fasse toujouts face à la cam
        if (directionToCamera.x != 0 && directionToCamera.z != 0)
            transform.rotation = Quaternion.LookRotation(new Vector3(directionToCamera.x, 0, directionToCamera.z));
    }

    
    void Initialiser()
    {
        foreach(IntrantUI intrant in ÉlémentAffichés)
        {
            CréerNouveauÉlémentUI(intrant.gameobj, intrant.typeRessource, intrant.typeOutil);
        }

        foreach (AffichageUI element in listeUI)
        {
            element.BarreRouge.enabled = true;
            element.BarreVerte.enabled = false;
            if (element.typeOutil != Outil.Null)
            {
                element.text.enabled = false;
            }
            else
            {
                element.text.text = GestionInventaire.ObtenirRessource(element.typeRessource)+"/" +CoutStructure.ressourcesRequises[(int)element.typeRessource].ToString();
            }
        }
        UpdateUI(this, EventArgs.Empty);
    }

    

    void UpdateUI(object sender, EventArgs e)
    {
        foreach(AffichageUI element in listeUI)
        {
            if(element.typeRessource != Ressource.Null)
            {
                
                if (GestionInventaire.ObtenirRessource(element.typeRessource) >= CoutStructure.ressourcesRequises[(int)element.typeRessource])
                {
                    element.imageItem.color = vert;
                    element.BarreVerte.enabled = true;
                    element.BarreRouge.enabled = false;
                    element.text.enabled = false;
                }
                else
                {
                    element.text.text = GestionInventaire.ObtenirRessource(element.typeRessource) + "/" + CoutStructure.ressourcesRequises[(int)element.typeRessource].ToString();
                    element.BarreRouge.fillAmount = GestionInventaire.ObtenirRessource(element.typeRessource) / (float)CoutStructure.ressourcesRequises[(int)element.typeRessource];
                    if (element.BarreVerte.enabled)
                    {
                        element.BarreVerte.enabled = false;
                        element.BarreRouge.enabled= true;
                        element.text.enabled = true;
                    }
                }
                    

            }
            else
            {
                if (GestionInventaire.ObtenirOutil(element.typeOutil))
                {
                    element.imageItem.color = vert;
                    element.BarreVerte.enabled = true;
                    element.BarreRouge.enabled = false;
                }
                
            }
        }

    }

    void CréerNouveauÉlémentUI(GameObject element, Ressource ressource, Outil outil)
    {
        AffichageUI nouvelÉlément = new AffichageUI();
        
        nouvelÉlément.typeRessource = ressource;
        nouvelÉlément.typeOutil = outil;
        nouvelÉlément.text = element.transform.Find("Texte").GetComponent<TextMeshProUGUI>();
        nouvelÉlément.imageItem = element.transform.Find("Image").transform.Find("ImageRouge").GetComponent<Image>();
        nouvelÉlément.BarreRouge = element.transform.Find("ProgressBar").transform.Find("ProgressRouge").GetComponent<Image>();
        nouvelÉlément.BarreVerte = element.transform.Find("ProgressBar").transform.Find("ProgressVert").GetComponent<Image>();


        listeUI.Add(nouvelÉlément);

    }

    private void OnDestroy()
    {
        GestionInteraction.OnInteraction -= UpdateUI;
    }
}
public struct AffichageUI
{
    public Ressource typeRessource;
    public Outil typeOutil;
    public TextMeshProUGUI text;
    public Image imageItem;
    public Image BarreRouge;
    public Image BarreVerte;
}
[System.Serializable]
public struct IntrantUI
{
    public Ressource typeRessource;
    public Outil typeOutil;
    public GameObject gameobj;
}