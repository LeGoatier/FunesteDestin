using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public enum Raret� { Commun, Atypique, Rare, �pique, L�gendaire, NbRaret�}
public enum �tatNoeud { Inconnu, D�couvert, Atteint, Nb�tatNoeud}

public class NoeudAchievement : MonoBehaviour
{
    [Header ("Caract�ristiques du succ�s")]
    [SerializeField] Achievements achievement;
    [SerializeField] Raret� raret�;
    [SerializeField] string nomSucc�s;
    [SerializeField] string explication;
    [SerializeField] bool poss�deTitre; //Le titre se fait set automatiquement avec GestionAchievement, mais de savoir
    //si l'achievement poss�de un titre peu permettre de changer la disposition de l'affichage si tu veux
    [SerializeField] List<NoeudAchievement> enfants;


    [Header("Liens dans le prefab")]
    [SerializeField] TextMeshProUGUI nomTexte;
    [SerializeField] TextMeshProUGUI titreTexte;
    [SerializeField] GameObject explicationObjet;
    TextMeshProUGUI texteExplication;
    [SerializeField] Color[] couleursRaret�s; //Faut les mettre dans l'ordre de l'enum et de la longueur de l'enum
    [SerializeField] Image imagePourCouleurRaret�;
    [SerializeField] Transform pointEntrantLiens;
    [SerializeField] Transform pointSortantLiens;
    [SerializeField] Color couleurLiens;
    [SerializeField] GameObject panelNoirInconnu;
    [SerializeField] GameObject panelD�couvert;
    [SerializeField] GameObject panelAtteint;
    [SerializeField] Image badge;

    [SerializeField] Sprite[] badges = new Sprite[(int)Raret�.NbRaret�];

    GameObject arbre;
    GameObject canevasPrincipal;

    //Gestion de l'�tat des noeuds
    //Par d�faut, l'achievement est incconu, ensuite on part du premier noeud
    //si l'achievement est activ� on change son �tat et on active ses enfants aussi
    //si l'achievement n'est pas activ�, il devient simplement d�couvert
    private �tatNoeud �tat = �tatNoeud.Inconnu;

    public void ActiverNoeudEtEnfants()
    {
        if (GestionAchievements.EstAchievementR�ussi(achievement))
        {
            �tat = �tatNoeud.Atteint;
            foreach(NoeudAchievement noeud in enfants)
            {
                noeud.ActiverNoeudEtEnfants();
            }
        }
        else
        {
            �tat = �tatNoeud.D�couvert;
        }
        InitialiserValeursVariables();
    }
    private void Start()
    {
        canevasPrincipal = GameObject.FindGameObjectWithTag("Panel_Manager");
        if(canevasPrincipal == null)
            canevasPrincipal = GameObject.FindGameObjectWithTag("CanevasSucc�s");

        badge.sprite = badges[(int)raret�];
        arbre = GameObject.FindGameObjectWithTag("ArbreSucc�s");
        arbre.GetComponent<ArbreAchievement>().OnLancementRechercheArbreAchievement += R�initialiser�tat;
        texteExplication = explicationObjet.GetComponentInChildren<TextMeshProUGUI>();
        explicationObjet.SetActive(false);
        InitialiserValeursFixes();
    }


    private void R�initialiser�tat(object sender, EventArgs eventArgs)
    {
        �tat = �tatNoeud.Inconnu;
        InitialiserValeursVariables();
    }


    private void InitialiserValeursFixes()
    {
        nomTexte.text = nomSucc�s;
        if (poss�deTitre)
            titreTexte.text = $"({GestionAchievements.GetTitreAchievement(achievement)})";
        else
            titreTexte.text = "";
        texteExplication.text = explication;
        imagePourCouleurRaret�.color = couleursRaret�s[(int)raret�];
        //Faire les liens (seulement pour les enfants, car le parent s'occupe de faire le lien pour nous)
        foreach (NoeudAchievement noeud in enfants)
        {
            Vector3 positionLocale1 = pointSortantLiens.localPosition;

            Vector3 positionLocale2 = positionLocale1 + noeud.pointEntrantLiens.position - pointSortantLiens.position;

            MakeLine(positionLocale1, positionLocale2);
        }

    }
    




    public void InitialiserValeursVariables()
    {
        if(�tat == �tatNoeud.Inconnu)
        {
            panelNoirInconnu.SetActive(true);
            panelD�couvert.SetActive(false);
            panelAtteint.SetActive(false);
        }
        else if(�tat == �tatNoeud.D�couvert)
        {
            panelNoirInconnu.SetActive(false);
            panelD�couvert.SetActive(true);
            panelAtteint.SetActive(false);
        }
        else if(�tat == �tatNoeud.Atteint)
        {
            panelNoirInconnu.SetActive(false);
            panelD�couvert.SetActive(false);
            panelAtteint.SetActive(true);
        }
    }



    //Ok le line renderer de unity ne fonctionne pas avec le UI, donc voici
    //du code pris � un gars random sur internet (Tom163 de unity forum)
    //qui cr�e des lignes dynamiquement
    //Par contre, j'ai d� adapter �norm�ment sa fonction parce qu'elle ne fonctionnait pas
    //du tout initialement
    [SerializeField] Sprite lineImage;
    float lineWidth = 3;

    void MakeLine(Vector3 positionLocaleInitiale, Vector3 positionLocaleFinale)
    {
        GameObject NewObj = new GameObject();
        NewObj.name = "line from " + positionLocaleInitiale + " to " + positionLocaleFinale;
        Image NewImage = NewObj.AddComponent<Image>();
        NewImage.sprite = lineImage;
        NewImage.color = couleurLiens;
        RectTransform rect = NewObj.GetComponent<RectTransform>();
        rect.SetParent(transform);
        rect.localScale = Vector3.one;

        NewObj.transform.SetAsFirstSibling();

        //Il faut peut-�tre que je multiplie par un graphscale
        //En fait non c'est juste s'il y a des trucs dans ses parents qui ont pas un scale de 1
        Vector3 graphScale = canevasPrincipal.transform.localScale;
        //print(graphScale);
        positionLocaleInitiale = new Vector3(positionLocaleInitiale.x / graphScale.x, positionLocaleInitiale.y / graphScale.y, 0); ;
        positionLocaleFinale = new Vector3(positionLocaleFinale.x/ graphScale.x, positionLocaleFinale.y / graphScale.y, 0);
        //Vector3 a = new Vector3(ax * graphScale.x, ay * graphScale.y, 0);
        //Vector3 b = new Vector3(bx * graphScale.x, by * graphScale.y, 0);
        //rect.anchorMin = Vector2.zero;
        //rect.anchorMax = Vector2.zero;

        rect.localPosition = (positionLocaleInitiale + positionLocaleFinale) / 2;
        Vector3 dif = positionLocaleInitiale - positionLocaleFinale;
        rect.sizeDelta = new Vector3(dif.magnitude, lineWidth);
        rect.rotation = Quaternion.Euler(new Vector3(0, 0, 180 * Mathf.Atan(dif.y / dif.x) / Mathf.PI));

        //On pourrait set les ancrages aussi si n�cessaire, pour l'instant c'est milieu
    }

    public void AfficherExplications(bool afficher)
    {
        explicationObjet.SetActive(afficher);
    }
}

