using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Fusée : ComportementStructure
{
    [SerializeField] GameObject PrefabFusée;
    public override Étapes ÉtapeLiée => Étapes.FuséeDétresse;
    bool EstRéparé = false;
    DataObjectInteragissable CetteFusée;

    TextMeshProUGUI texte;
    

    float Vitesse = 10;
    // Start is called before the first frame update
    void Awake()
    {
        InitialiserComportementStructure();

        
        étatInitialisation = ÉtatInitialisation.positionBorné;
        RayonMin = 45;
        RayonMax = 55;

        texte = GetComponentInChildren<TextMeshProUGUI>();

        
        CoutStructure.AjouterOutil(Outil.FeuDeDétresse);
        CoutStructure.AjouterRessource(Ressource.Bois, 3);
        CoutStructure.AjouterRessource(Ressource.Fer, 2);
        InitialiserDataFusée();
    }


    

    public override void InteragirObjet()
    {
        if (!EstRéparé && SontPréalablesRemplis())
        {
            RéparerFusée();
            Destroy(canevas.gameObject);
        }
    }

    void RéparerFusée()
    {
        EstRéparé = true;
        texte.transform.parent.gameObject.SetActive(false);
        ObjectRamassableVisible.Remove(CetteFusée);
        Destroy(GetComponent<SphereCollider>());
        var instanceFusée = Instantiate(PrefabFusée, transform);

        StartCoroutine(Envolée(instanceFusée));
        StartCoroutine(GestionBruit.instance.JouerSonFin("VictoireFusée"));
        EndGameUI.instance.CommencerEndGame(ÉtatFinPartie.Fusée);
    }

    IEnumerator Envolée(GameObject fusée)
    {
        float timer = 0;

        while (timer < 10)
        {
            timer += Time.deltaTime;
            fusée.transform.Translate(Vector3.up * Time.deltaTime * Vitesse);
            yield return null;
        }
        
    }

    public override string DéterminerTexteUI()
    {
        return "Réparer la fusée";
    }

    void InitialiserDataFusée()
    {
        CetteFusée.objet = gameObject;
        CetteFusée.mr = GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CetteFusée.mr.Length];

        for (int i = 0; i < CetteFusée.mr.Length; i++)
        {
            couleurs[i] = CetteFusée.mr[i].material.color;
        }

        CetteFusée.CouleursOriginale = couleurs;
        CetteFusée.AngleInteraction = 30;
    }



    protected override void EstEntréEnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Add(CetteFusée);
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Remove(CetteFusée);
    }
}
