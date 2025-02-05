using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Antenne : ComportementStructure
{
    public override Étapes ÉtapeLiée => Étapes.Antenne;
    [SerializeField] GameObject mesh;
    bool EstRéparé = false;
    bool SontDébritEnlever = false;
    DataObjectInteragissable CetteAntenne;

    string MessageUI;

    void Awake()
    {

        InitialiserComportementStructure();
        étatInitialisation = ÉtatInitialisation.positionCondition;

        CoutStructure.AjouterOutil(Outil.Radio);
        InitialiserDataAntenne();
        MessageUI = "Réparer l'antenne";
    }

    private void Start()
    {
        AntenneSniper2.OnTirerAvecSniper += AÉtéTirerAvecSniper;
    }

    void AÉtéTirerAvecSniper(object sender, EventArgs e)
    {
        SontDébritEnlever = true;
        Destroy(GameObject.FindWithTag("Toiles"));
    }

    protected override bool SontPréalablesRemplis()
    {
        if (GestionInventaire.EstCoutRempli(CoutStructure) && SontDébritEnlever)
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
        
        if (!EstRéparé && SontPréalablesRemplis())
        {
            RéparerAntenne();
            AnimerVictoire();
        }
        

    }

    void RéparerAntenne()
    {
        EstRéparé = true;
        ObjectRamassableVisible.Remove(CetteAntenne);
        Destroy(GetComponent<SphereCollider>());
        MessageUI = "";
    }

    void AnimerVictoire() 
    {
        StartCoroutine(GestionBruit.instance.JouerSonFin("VictoireAntenne"));
        EndGameUI.instance.CommencerEndGame(ÉtatFinPartie.Antenne);
    }

    public override string DéterminerTexteUI()
    {
        return MessageUI;
    }


    public override List<Vector3> ConditionSpawn()
    {
        return new List<Vector3>() { Gen_Ile.GetPointPlusHaut() };
    }


    protected override void EstEntréEnCollision(Collider other)
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
