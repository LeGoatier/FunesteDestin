using TMPro;
using UnityEngine;

public class AbrisJoueur : ComportementStructure
{
    public override Étapes ÉtapeLiée => Étapes.Abri;

    [SerializeField] GameObject AbrisBrisé;
    [SerializeField] GameObject AbrisRéparé;
    DataObjectInteragissable CetAbris;

    TextMeshProUGUI texte;
    GameObject instanceBrisé;
    // Start is called before the first frame update
    void Awake()
    {
        InitialiserComportementStructure();

        NiveauSpawn = 1;
        étatInitialisation = ÉtatInitialisation.positionNiveau;

        instanceBrisé = Instantiate(AbrisBrisé, transform);
        InitialiserDataÉtabli(instanceBrisé);

        texte = GetComponentInChildren<TextMeshProUGUI>();
        texte.text = "";
        

        
        CoutStructure.AjouterOutil(Outil.Marteau);
        
    }



    public override void InteragirObjet()
    {
        if (SontPréalablesRemplis())
        {
            RéparerÉtabli();
        }
        
    }


    void RéparerÉtabli()
    {
        ObjectRamassableVisible.Remove(CetAbris);
        Instantiate(AbrisRéparé, transform.position, transform.rotation);
        Destroy(instanceBrisé);
        Destroy(gameObject);
        Destroy(canevas);
        //On rebake parce que sinon les monstres passeraient autravers le nouvel abri
        GestionNavMesh.BakeSurface();
    }


    public override string DéterminerTexteUI()
    {
        return "Réparer";
    }

    void InitialiserDataÉtabli(GameObject établi)
    {
        CetAbris.objet = gameObject;
        CetAbris.mr = établi.GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CetAbris.mr.Length];

        for (int i = 0; i < CetAbris.mr.Length; i++)
        {
            couleurs[i] = CetAbris.mr[i].material.color;
        }

        CetAbris.CouleursOriginale = couleurs;
        CetAbris.AngleInteraction = 50;
    }

    

    

    protected override void EstEntréEnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Add(CetAbris);
        }
            
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Remove(CetAbris);
    }

    
}
