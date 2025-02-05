using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SauvegardeUI : MonoBehaviour
{
    Image imageActuelle;

    [SerializeField] public Sprite imagePartie;
    [SerializeField] Sprite imageVide;

    [SerializeField] GameObject trashButton;
    [SerializeField] GameObject TMP_Date;
    [SerializeField] GameObject plus;

    Panel_Manager panelManager;

    [SerializeField] Color couleurVide;

    public bool estVide;
    
    private Color couleurPartie;

    private string nomProfil;
    private string titreProfil;
    [SerializeField] int indicePersonnel;

    [SerializeField] TextMeshProUGUI NomTexte;
    [SerializeField] TextMeshProUGUI DateTexte;
    [SerializeField] TextMeshProUGUI TitreTexte;
    [SerializeField] TextMeshProUGUI NiveauTexte;

    private void Awake()
    {
        imageActuelle = GetComponent<Image>();
        couleurPartie = imageActuelle.color;
        estVide = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        panelManager = GameObject.FindGameObjectWithTag("Panel_Manager").GetComponent<Panel_Manager>();
        UptadeImage();
    }

    //Cette fonction est appelé lorsque l'icone de poubelle est cliqué sur un profil
    public void Delete()
    {
        SauvegardeProfils.SetNomProfilActuel(nomProfil);
        panelManager.indiceProfilADetruire = indicePersonnel;
        panelManager.AllerEtesVousSur();
    }

    public void DétruireSauvegarde()
    {
        estVide = true;
        UptadeImage();
    }



    //Cette fonction est appelée lorsque l'utilisateur clique sur la case de profil ou la case vide
    public void Ouvrir()
    {
        SauvegardeProfils.SetIndiceProfilActuel(indicePersonnel);
        if (!estVide)
        {
            SauvegardeProfils.ChargerSauvegarde();
            panelManager.AllerSélectionBoutons();
            panelManager.AjusterNomJoueur(nomProfil);
            SauvegardeProfils.SetNomProfilActuel(nomProfil);
            SauvegardeProfils.SetTitreProfilActuel(titreProfil);
        }
        else
        {
            panelManager.AllerCréationProfil();
        }
    }
    

    private void UptadeImage()
    {
        if (estVide)
        {
            imageActuelle.sprite = imageVide;
            trashButton.SetActive(false);
            TMP_Date.SetActive(false);
            imageActuelle.color = couleurVide;
            plus.SetActive(true);
            NomTexte.gameObject.SetActive(false);
            DateTexte.gameObject.SetActive(false);
            TitreTexte.gameObject.SetActive(false);
            NiveauTexte.gameObject.SetActive(false);
        }
        else
        {
            imageActuelle.sprite = imagePartie;
            trashButton.SetActive(true);
            TMP_Date.SetActive(true);
            imageActuelle.color = couleurPartie;
            plus.SetActive(false);
            NomTexte.gameObject.SetActive(true);
            DateTexte.gameObject.SetActive(true);
            TitreTexte.gameObject.SetActive(true);
            NiveauTexte.gameObject.SetActive(true);
        }
         
    }

    //Appelé s'il y a une sauvegarde pour le profil en question
    public void InitialiserProfil(string nomProfil, DateTime dateDernièreUtilisation, string titre, int niveau)
    {
        this.nomProfil = nomProfil;
        titreProfil = titre;
        NomTexte.text = nomProfil;
        DateTexte.text = dateDernièreUtilisation.ToShortDateString();
        NiveauTexte.text = $"{niveau}";
        TitreTexte.text = titre;
        estVide = false;
        UptadeImage();
    }
}
