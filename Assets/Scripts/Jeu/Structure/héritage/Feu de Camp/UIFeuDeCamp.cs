using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIFeuDeCamp : MonoBehaviour
{
    [SerializeField] Canvas canvas;

    [SerializeField] TextMeshProUGUI textBois;
    [SerializeField] TextMeshProUGUI textPierre;

    [SerializeField] Image imageBois;
    [SerializeField] Image imagePierre;

    [SerializeField] Image imageProgresBois;
    [SerializeField] Image imageProgresPierre;

    [SerializeField] Image imageProgresBoisVert;
    [SerializeField] Image imageProgresPierreVert;

    Color rouge;
    Color vert;

    public static UIFeuDeCamp instance;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    private void Awake()
    {
        rouge = new(0.5f, 0, 0, 1);
        vert = new (0.133f, 0.545f, 0.133f, 1);
    }
    void Update()
    {
        Vector3 directionToCamera = transform.position - GameManager.instance.GetPositionCam();

        // Pour que le slider fasse toujouts face à la cam
        if (directionToCamera.x != 0 && directionToCamera.z != 0)      
            transform.rotation = Quaternion.LookRotation(new Vector3(directionToCamera.x, 0, directionToCamera.z));     
    }

    public void ResetUI()
    {
        SetNombreRessources();
        SetIconesEtBarRouges();        

        UpdateUI();
    }


    public void UpdateUI()
    {
        if(GestionInventaire.ObtenirRessource(Ressource.Bois) >= FeuDeCamp.instance.nbBoisRequis)
        {
            imageBois.color = vert;
            imageProgresBoisVert.enabled = true;
            imageProgresBois.enabled = false;          
        }
        else        
            imageProgresBois.fillAmount = GestionInventaire.ObtenirRessource(Ressource.Bois) / (float)FeuDeCamp.instance.nbBoisRequis;
        

        if (GestionInventaire.ObtenirRessource(Ressource.Pierre) >= FeuDeCamp.instance.nbPierreRequis)
        {
            imagePierre.color = vert;
            imageProgresPierreVert.enabled= true;
            imageProgresPierre.enabled = false;
        }
        else        
            imageProgresPierre.fillAmount = GestionInventaire.ObtenirRessource(Ressource.Pierre) / (float)FeuDeCamp.instance.nbPierreRequis;

    }
    
    private void SetNombreRessources()
    {
        textBois.SetText(FeuDeCamp.instance.nbBoisRequis.ToString());
        textPierre.SetText(FeuDeCamp.instance.nbPierreRequis.ToString());
    }

    private void SetIconesEtBarRouges()
    {
        imageBois.color = rouge;
        imagePierre.color = rouge;

        imageProgresBoisVert.enabled = false;
        imageProgresPierreVert.enabled = false;
        imageProgresBois.enabled = true;
        imageProgresPierre.enabled = true;
    }

    internal void DésactiverUI()
    {
        if(canvas.enabled) canvas.enabled = false;
    }
}
