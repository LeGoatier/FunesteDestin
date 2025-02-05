using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    // Toutes ces valeurs sont cachées dans l'inspecteur
    [HideInInspector] public Image image;
    Sprite spriteVide;
    private Draggable draggable;
    [HideInInspector] public TextMeshProUGUI compte;

    public Ressource ressource;
    public Outil outil;
    public Arme arme;
    public Soin soin;
    public bool estVide;

    private TextMeshProUGUI info;

    private void Start()
    {
        image = GetComponent<Image>();
        TextMeshProUGUI[] temp = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmp in temp)
        {
            if (tmp.gameObject.name == "Nombre")
            {
                compte = tmp;
                compte.text = " ";
            }
            if (tmp.gameObject.name == "Info")
            {
                info = tmp;
                info.gameObject.transform.parent.gameObject.SetActive(false);
            }
        }
        draggable = GetComponent<Draggable>();
        spriteVide = image.sprite;
        estVide = true;
    }

    private void OnEnable()
    {
        AfficherInfos(false);
    }

    public void AfficherItem(Sprite sprite, Ressource ressource, Outil outil, Soin soin, int compte)
    {
        AfficherItem(sprite);
        if (this.compte != null) this.compte.text = $"{compte}";
        this.ressource = ressource;
        this.outil = outil;
        this.soin = soin;
        this.arme = Arme.Null;
    }
    public void AfficherItem(Sprite sprite, Ressource ressource, Outil outil, Soin soin)
    {
        AfficherItem(sprite);
        if (compte != null) this.compte.text = $" ";
        this.ressource = ressource;
        this.outil = outil;
        this.soin = soin;
        this.arme = Arme.Null;
    }

    public void AfficherItem(Sprite sprite, Arme arme)
    {
        AfficherItem(sprite);
        if (compte != null) this.compte.text = $" ";
        this.ressource = Ressource.Null;
        this.outil = Outil.Null;
        this.soin = Soin.Null;
        this.arme = arme;
    }
    private void AfficherItem(Sprite sprite)
    {
        image.sprite = sprite;
        estVide = false;
        draggable.peutDrag = true;
        TrouverText();
    }

    public void AfficherVide()
    {
        image.sprite = spriteVide;
        AfficherInfos(false);
        estVide = true;
        if (this.compte != null) this.compte.text = " ";
        draggable.peutDrag = false;
        this.ressource = Ressource.Null;
        this.outil = Outil.Null;
        this.arme = Arme.Null;
        this.soin = Soin.Null;
    }

    public void AfficherInfos(bool afficher)
    {

            if (!estVide)
            {
                if (afficher) TrouverText();
                if (info != false) info.gameObject.transform.parent.gameObject.SetActive(afficher);
            }
    }

    private void TrouverText()
    {
     
            if (ressource != Ressource.Null)
            {
                info.text = InfoItem.ressources[(int)ressource];
            }
            if (outil != Outil.Null)
            {
                info.text = InfoItem.outils[(int)outil];
            }
            if (arme != Arme.Null)
            {
                info.text = InfoItem.armes[(int)arme];
            }
            if(soin != Soin.Null)
            {
                info.text = InfoItem.soins[(int)soin];
            }   
        
    }
}
