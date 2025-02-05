using System.Collections.Generic;
using UnityEngine;

public class InteragirFusils : Interagir
{
    [SerializeField] GameObject prefabUitilisable;
    [SerializeField] public Arme TypeArme;
    InventaireFusils InventaireFusil;

    List<DataObjectInteragissable> ObjectRamassableVisible;
    DataObjectInteragissable cetArme;
    

    private void Awake()
    {
        GameObject joueur = GameObject.FindWithTag("Player");
        InventaireFusil = joueur.GetComponentInChildren<InventaireFusils>();
        ObjectRamassableVisible = joueur.GetComponentInChildren<GestionInteraction>().ObjetInteragissableVisible;

        InitialiserDataArme();
        
    }

    void InitialiserDataArme()
    {
        cetArme.objet = gameObject;
        cetArme.mr = GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[cetArme.mr.Length];

        for (int i = 0; i < cetArme.mr.Length; i++)
        {
            couleurs[i] = cetArme.mr[i].material.color;
        }

        cetArme.CouleursOriginale = couleurs;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ObjectRamassableVisible.Add(cetArme);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            ObjectRamassableVisible.Remove(cetArme);

        }
    }

    public override void InteragirObjet()
    {
        GestionInventaire.AjouterArme(TypeArme);
        if (InventaireFusils.instance.armesÉquipées.Count < 3) InventaireFusils.instance.ÉquiperArme(TypeArme);
        NotificationSystem.instance.LancerNotification(TypeArme);

        ObjectRamassableVisible.Remove(cetArme);
        Destroy(gameObject);
    }
    

    public override string DéterminerTexteUI()
    {
        return "Ramasser fusil";
    }

}
