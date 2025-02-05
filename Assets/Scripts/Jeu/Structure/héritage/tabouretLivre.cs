using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class tabouretLivre : ComportementStructure
{
    [SerializeField] Canvas UILivret;
    
    DataObjectInteragissable CeTabouret;

    public override …tapes …tapeLiÈe => throw new System.NotImplementedException();

    void Awake()
    {
        UILivret.enabled = false;
        InitialiserComportementStructure();
        InitialiserDataObjet();
    }

    private void Start()
    {
        ComportementInterface.instance.OnMenuChangement += GestionUI;
        //GetComponentInChildren<Button>().onClick.AddListener(ComportementInterface.instance.ActivÈ…tatJeu);
    }
    public override string DÈterminerTexteUI()
    {
        return "Lire";
    }

    public override void InteragirObjet()
    {
        UILivret.enabled = !UILivret.enabled;
        ComportementInterface.instance.Set…tat(!UILivret.enabled);
       
    }

    void InitialiserDataObjet()
    {
        CeTabouret.objet = gameObject;
        CeTabouret.mr = GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CeTabouret.mr.Length];

        for (int i = 0; i < CeTabouret.mr.Length; i++)
        {
            couleurs[i] = CeTabouret.mr[i].material.color;
        }

        CeTabouret.CouleursOriginale = couleurs;
        CeTabouret.AngleInteraction = 20;
    }



    protected override void EstEntrÈEnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Add(CeTabouret);
        }

    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Remove(CeTabouret);
    }

    public void SetText(string text)
    {
        UILivret.GetComponentInChildren<TextMeshProUGUI>().text = text;   
    }

    

    void GestionUI(bool …tatJeuActif)
    {
        if(…tatJeuActif)
        {
            UILivret.enabled = false;
        }
        
    }
}
