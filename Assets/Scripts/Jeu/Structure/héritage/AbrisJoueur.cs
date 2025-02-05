using TMPro;
using UnityEngine;

public class AbrisJoueur : ComportementStructure
{
    public override �tapes �tapeLi�e => �tapes.Abri;

    [SerializeField] GameObject AbrisBris�;
    [SerializeField] GameObject AbrisR�par�;
    DataObjectInteragissable CetAbris;

    TextMeshProUGUI texte;
    GameObject instanceBris�;
    // Start is called before the first frame update
    void Awake()
    {
        InitialiserComportementStructure();

        NiveauSpawn = 1;
        �tatInitialisation = �tatInitialisation.positionNiveau;

        instanceBris� = Instantiate(AbrisBris�, transform);
        InitialiserData�tabli(instanceBris�);

        texte = GetComponentInChildren<TextMeshProUGUI>();
        texte.text = "";
        

        
        CoutStructure.AjouterOutil(Outil.Marteau);
        
    }



    public override void InteragirObjet()
    {
        if (SontPr�alablesRemplis())
        {
            R�parer�tabli();
        }
        
    }


    void R�parer�tabli()
    {
        ObjectRamassableVisible.Remove(CetAbris);
        Instantiate(AbrisR�par�, transform.position, transform.rotation);
        Destroy(instanceBris�);
        Destroy(gameObject);
        Destroy(canevas);
        //On rebake parce que sinon les monstres passeraient autravers le nouvel abri
        GestionNavMesh.BakeSurface();
    }


    public override string D�terminerTexteUI()
    {
        return "R�parer";
    }

    void InitialiserData�tabli(GameObject �tabli)
    {
        CetAbris.objet = gameObject;
        CetAbris.mr = �tabli.GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CetAbris.mr.Length];

        for (int i = 0; i < CetAbris.mr.Length; i++)
        {
            couleurs[i] = CetAbris.mr[i].material.color;
        }

        CetAbris.CouleursOriginale = couleurs;
        CetAbris.AngleInteraction = 50;
    }

    

    

    protected override void EstEntr�EnCollision(Collider other)
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
