using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FeuDeCamp : ComportementStructure
{
    public override Étapes ÉtapeLiée => Étapes.Établi; //Il n'y a pas d'étape pour feu de camp, mais établi est toujours présent alors ça devrait fonctionner
    public int niveauActuel { get; private set; }

    [SerializeField] float incrementAngle;
    [SerializeField] public List<Feu> feux;
    [SerializeField] GameObject brume_Prefab;
    [SerializeField] GameObject FuméFeu;
    //feu
    public static FeuDeCamp instance;
    DataObjectInteragissable ceFeuDeCamps;
    UIFeuDeCamp uIFeuDeCamp;
    public GameObject FeuCourant;
    private Light lumière;

    public int nbBoisRequis;
    public int nbPierreRequis;

    bool EstPremiereInteraction = true;
    bool futAmelioré = false;
    new AudioSource audio;

    //regénérer vie
    GestionVieJoueur gestionVieJoueur;
    bool veutRegénérerVie = true;
    float dernièreRegénération;
    float délaisRegénération = 1;

    //brume enlver?
    //private List<GameObject> brumes = new();
    //ObjectPool pool;
    
    
    Cylindre bordure;


    void Awake()
    {
        if (instance == null)       
            instance = this;      

        InitialiserComportementStructure(); //voir classse parent
        Position = new Vector2(0, 0);
        étatInitialisation = ÉtatInitialisation.positionFixe;

        //pool = GameObject.FindGameObjectWithTag("Object_Pool").GetComponent<ObjectPool>();
        uIFeuDeCamp = GetComponentInChildren<UIFeuDeCamp>();
        uIFeuDeCamp.gameObject.SetActive(false);
        gestionVieJoueur = GameObject.FindWithTag("Player").GetComponentInChildren<GestionVieJoueur>();
        lumière = GetComponentInChildren<Light>();
        lumière.enabled = false;

        niveauActuel = 0;

        FeuCourant = Instantiate(feux[niveauActuel].objet, transform.position, transform.rotation);
        InitialiserDataFeuDeCamps();

        InitialiserBordure();
        //if (brumes != null) DésactiverBrume();
        //GénérerBrume();

        (nbBoisRequis, nbPierreRequis) = DéterminerNbRessourcesNecessaires(niveauActuel);
       
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<Placement>().OnStructureInstancié += InstancierFeuPrefab;

        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (veutRegénérerVie)
            if (Time.time - dernièreRegénération >= délaisRegénération)
            {
                gestionVieJoueur.RecevoirVie(2);
                dernièreRegénération = Time.time;
            }
    }

    void InstancierFeuPrefab(object sender, EventArgs e)
    {
        FeuCourant.transform.position = transform.position;
        FeuCourant.transform.rotation = transform.rotation;
    }


    void InitialiserBordure()
    {
        GameObject Bordure_Prefab = (GameObject)Resources.Load("Prefabs/Primitives/BordureTempete"); //Sinon il ya une erreur idk why :(
        var instanceBordure = Instantiate(Bordure_Prefab);
        bordure = instanceBordure.GetComponent<Cylindre>();
        bordure.GénérerBordure(feux[niveauActuel].rayon);
    }

    void InitialiserDataFeuDeCamps()
    {
        ceFeuDeCamps.objet = gameObject;
        ceFeuDeCamps.mr = FeuCourant.GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[ceFeuDeCamps.mr.Length];

        for (int i = 0; i < ceFeuDeCamps.mr.Length; i++)
            couleurs[i] = ceFeuDeCamps.mr[i].material.color;

        ceFeuDeCamps.CouleursOriginale = couleurs;
    }



    protected void AllumerFeu()
    {
        uIFeuDeCamp.gameObject.SetActive(true);
        Instantiate(FuméFeu, transform);
        lumière.enabled = true;
        uIFeuDeCamp.ResetUI();
        futAmelioré = true;
        audio.Play();
        GestionAchievements.ActiverAchievement(Achievements.CaRechauffeLeCoeur);
    }

    protected override void EstEntréEnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Add(ceFeuDeCamps);
            veutRegénérerVie = true;
        }
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Remove(ceFeuDeCamps);
            veutRegénérerVie = false;
        }
    }

    public override void InteragirObjet() //ameliore feu
    {
        (bool peutAmeliorer, Cout cout) = PeutAméliorerSelonRessources();

        if (EstPremiereInteraction)
        {          
            EstPremiereInteraction = false;
            AllumerFeu();
        }

        else if (peutAmeliorer)
        {            
            if (niveauActuel < feux.Count - 1)
            {               
                //Actualise niveau, nb ressources dans inventaire, UI et granny
                niveauActuel++;
                GestionInventaire.AcheterCout(cout);
                (nbBoisRequis, nbPierreRequis) = DéterminerNbRessourcesNecessaires(niveauActuel);
                uIFeuDeCamp.ResetUI();
                Granny.instance.GérerChangementNiveauFeu();

                //Instantie le nouveau feu
                ObjectRamassableVisible.Remove(ceFeuDeCamps);
                Destroy(FeuCourant);
                FeuCourant = Instantiate(feux[niveauActuel].objet, transform.position, transform.rotation);
                InitialiserDataFeuDeCamps();

                //comme OnTriggerEnter ne va pas s'activer car le joueur est déja dedans, on set à la main la possibilité d'interactions
                ObjectRamassableVisible.Add(ceFeuDeCamps);
                GestionInteraction.RendrePlusClair(ceFeuDeCamps);
                bordure.GénérerBordure(feux[niveauActuel].rayon);

                futAmelioré = true;

                if(InfoPartie.difficulté != Difficulté.paisible)
                {
                    if (niveauActuel == 5)
                    {
                        GestionAchievements.ActiverAchievement(Achievements.AuFeu);
                    }
                    if (niveauActuel == feux.Count - 1)
                    {
                        uIFeuDeCamp.DésactiverUI();
                        GestionAchievements.ActiverAchievement(Achievements.CaBrule);
                        if (InfoPartie.difficulté == Difficulté.difficile)
                        {
                            GestionAchievements.ActiverAchievement(Achievements.AuBucher);
                        }
                    }
                }
            }
        }
    }

    public (bool peutAmeliorer, Cout cout) PeutAméliorerSelonRessources()
    {
        Cout coutAmelioration = new();
        coutAmelioration.AjouterRessource(Ressource.Bois, nbBoisRequis);
        coutAmelioration.AjouterRessource(Ressource.Pierre, nbPierreRequis);

        return (GestionInventaire.EstCoutRempli(coutAmelioration), coutAmelioration);
    }


    public (int nbBois, int nbPierre) DéterminerNbRessourcesNecessaires(int niveau)
    {
        Cout cout = feux[niveau].cout;
        int bois = cout.ressourcesRequises[0];
        int pierre = cout.ressourcesRequises[1];

        return (bois, pierre);
    }

    public override string DéterminerTexteUI() => "Améliorer le feu";

    public override bool JouerSonInteraction()
    {
        if (futAmelioré)
        {
            GestionBruit.instance.JouerSon("AllumerFeu");
            futAmelioré = false;
            return true;
        }
        return false;
    }


    //Brume: a enlever??
    //private void DésactiverBrume()
    //{
    //    foreach (GameObject brume in brumes)
    //        brume.SetActive(false);
    //}

    //public void GénérerBrume()
    //{
    //    //Feu feuActuel = feux[niveauActuel];

    //    //for (float angle = 0; angle <= 360; angle += incrementAngle)
    //    //{
    //    //    GameObject brume = pool.GetPoolObject(brume_Prefab);

    //    //    if (brume != null)
    //    //    {
    //    //        brume.transform.position = CalculerPositionBrume(feuActuel.rayon, angle);
    //    //        brume.transform.LookAt(transform.position);
    //    //        brume.SetActive(true);
    //    //    }
    //    //}
    //}

    //private Vector3 CalculerPositionBrume(float rayon, float angle)
    //{
    //    return new Vector3(Mathf.Cos(angle / Mathf.Rad2Deg) * rayon, transform.position.y, Mathf.Sin(angle / Mathf.Rad2Deg) * rayon);
    //}


    public float GetRayonSelonNiveau(int niveau)
    {
        if(niveau < 0)
        {
            return 0;
        }
        
        return niveau < feux.Count? feux[niveau].rayon : feux[feux.Count-1].rayon;
    }
}

[System.Serializable]
public struct Feu
{
    public GameObject objet;
    public float rayon;
    public Cout cout;
}

