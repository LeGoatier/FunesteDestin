using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antenne : ComportementStructure
{
    public override �tapes �tapeLi�e => �tapes.Antenne;
    [SerializeField] GameObject mesh;
    bool EstR�par� = false;
    bool SontD�britEnlever = false;
    DataObjectInteragissable CetteAntenne;

    string MessageUI;

    void Awake()
    {

        InitialiserComportementStructure();
        �tatInitialisation = �tatInitialisation.positionCondition;

        CoutStructure.AjouterOutil(Outil.Radio);
        InitialiserDataAntenne();
        MessageUI = "R�parer l'antenne";
    }

    private void Start()
    {
        AntenneSniper2.OnTirerAvecSniper += A�t�TirerAvecSniper;
    }

    void A�t�TirerAvecSniper(object sender, EventArgs e)
    {
        SontD�britEnlever = true;
        Destroy(GameObject.FindWithTag("Toiles"));
    }

    protected override bool SontPr�alablesRemplis()
    {
        if (GestionInventaire.EstCoutRempli(CoutStructure) && SontD�britEnlever)
        {
            return true;
        }
        
        return false;
    }

    void InitialiserDataAntenne()
    {
        CetteAntenne.objet = gameObject;
        CetteAntenne.mr = new MeshRenderer[1] { mesh.GetComponent<MeshRenderer>() };

        Color[] couleurs = new Color[CetteAntenne.mr.Length];

        for (int i = 0; i < CetteAntenne.mr.Length; i++)
        {
            couleurs[i] = CetteAntenne.mr[i].material.color;
        }

        CetteAntenne.CouleursOriginale = couleurs;
        CetteAntenne.AngleInteraction = 50;
    }

    public override void InteragirObjet()
    {
        
        if (!EstR�par� && SontPr�alablesRemplis())
        {
            R�parerAntenne();
            AnimerVictoire();
        }
        

    }

    void R�parerAntenne()
    {
        EstR�par� = true;
        ObjectRamassableVisible.Remove(CetteAntenne);
        Destroy(GetComponent<SphereCollider>());
        MessageUI = "";
    }

    void AnimerVictoire() 
    {
        StartCoroutine(GestionBruit.instance.JouerSonFin("VictoireAntenne"));
        EndGameUI.instance.CommencerEndGame(�tatFinPartie.Antenne);
    }

    public override string D�terminerTexteUI()
    {
        return MessageUI;
    }


    public override List<Vector3> ConditionSpawn()
    {
        return new List<Vector3>() { Gen_Ile.GetPointPlusHaut() };
    }


    protected override void EstEntr�EnCollision(Collider other)
    {
        
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Add(CetteAntenne);
        }
         
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) 
        {
            ObjectRamassableVisible.Remove(CetteAntenne);
        }
            
    }
}
