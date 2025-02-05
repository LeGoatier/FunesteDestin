using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class InteragirSoins : Interagir
{
    [SerializeField] Soin soin;
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
        GestionInventaire.AjouterSoin(soin);
        NotificationSystem.instance.LancerNotification(soin, 1);
        ObjectRamassableVisible.Remove(CeMatériaux);
        Destroy(gameObject);


    }

    public override string DéterminerTexteUI()
    {
        return "Ramasser " + soin.ToString().ToLower();
    }

    public override bool JouerSonInteraction()
    {
        GestionBruit.instance.JouerSon("RamasserSoin");
        return true;
    }
}
