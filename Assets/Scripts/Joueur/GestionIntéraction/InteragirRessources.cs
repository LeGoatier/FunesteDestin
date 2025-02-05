using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class InteragirRessources : Interagir
{
    [SerializeField] Ressource ressource;
    GestionInteraction Interaction;

    List<DataObjectInteragissable> ObjectRamassableVisible;
    DataObjectInteragissable CeMatériaux;


    private void Awake()
    {
        Interaction = GameObject.FindWithTag("Player").GetComponentInChildren<GestionInteraction>();
        ObjectRamassableVisible = Interaction.ObjetInteragissableVisible;
        
        InitialiserDataMateriaux();

    }

    void InitialiserDataMateriaux()
    {
        CeMatériaux.objet = gameObject;
        CeMatériaux.mr = GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CeMatériaux.mr.Length];

        for (int i = 0; i < CeMatériaux.mr.Length; i++)
        {
            couleurs[i] = CeMatériaux.mr[i].material.color;
        }

        CeMatériaux.CouleursOriginale = couleurs;
        CeMatériaux.AngleInteraction = 25;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ObjectRamassableVisible.Add(CeMatériaux);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ObjectRamassableVisible.Remove(CeMatériaux);

        }
    }

    public override void InteragirObjet()
    {
        GestionInventaire.AjouterRessource(ressource);
        GestionStatistiques.AjouterRessourceCollectée(ressource);
        NotificationSystem.instance.LancerNotification(ressource, 1);

        ObjectRamassableVisible.Remove(CeMatériaux);
        Destroy(gameObject);

        if (UIFeuDeCamp.instance != null)
        {
            UIFeuDeCamp.instance.UpdateUI();
        }

    }

    public override string DéterminerTexteUI()
    {
        return "Ramasser " + ressource.ToString().ToLower();
    }

    public override bool JouerSonInteraction()
    {
        GestionBruit.instance.JouerSon("RamasserRessources");
        return true;
    }
}
