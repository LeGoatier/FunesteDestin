using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Établi : ComportementStructure
{
    public override Étapes ÉtapeLiée => Étapes.Établi;
    bool EstRéparé = false;
   
    [SerializeField] GameObject ÉtabliBrisé;
    [SerializeField] GameObject ÉtabliRéparé;
    DataObjectInteragissable CetÉtabli;

    

    string messageÉtabli;
    GameObject instanceBrisé;
    ComportementInterface UICraft;
    // Start is called before the first frame update
    void Awake()
    {
        InitialiserComportementStructure();

        messageÉtabli = "Réparer";
        NiveauSpawn = 0;
        étatInitialisation = ÉtatInitialisation.positionNiveau;

        instanceBrisé = Instantiate(ÉtabliBrisé, transform);
        InitialiserDataÉtabli(instanceBrisé);
        

        UICraft = GameObject.FindWithTag("GameManager").GetComponent<ComportementInterface>();
    }

    

    public override void InteragirObjet()
    {
        if(!EstRéparé)
        {
            RéparerÉtabli();
            
        }
        else
        {
            Construire();   
        }


    }

    void RéparerÉtabli()
    {
        EstRéparé = true;
        messageÉtabli = "Construire";
        ObjectRamassableVisible.Remove(CetÉtabli);
        var instance = Instantiate(ÉtabliRéparé, transform);
        InitialiserDataÉtabli(instance);
        Destroy(instanceBrisé);

        ObjectRamassableVisible.Add(CetÉtabli);
        GestionInteraction.RendrePlusClair(CetÉtabli);
    }

    public void Construire()
    {
        UICraft.ChangerÉtatCraft();
    }

    public override string DéterminerTexteUI()
    {
        return messageÉtabli;
    }

    void InitialiserDataÉtabli(GameObject établi)
    {
        CetÉtabli.objet = gameObject;
        CetÉtabli.mr = établi.GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CetÉtabli.mr.Length];

        for (int i = 0; i < CetÉtabli.mr.Length; i++)
        {
            couleurs[i] = CetÉtabli.mr[i].material.color;
        }

        CetÉtabli.CouleursOriginale = couleurs;
        CetÉtabli.AngleInteraction = 50;
    }



    protected override void EstEntréEnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Add(CetÉtabli);
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Remove(CetÉtabli);
    }
}
