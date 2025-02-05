using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FeuDeCamp : ComportementStructure
{
    public override �tapes �tapeLi�e => �tapes.�tabli; //Il n'y a pas d'�tape pour feu de camp, mais �tabli est toujours pr�sent alors �a devrait fonctionner
    public int niveauActuel { get; private set; }

    [SerializeField] float incrementAngle;
    [SerializeField] public List<Feu> feux;
    [SerializeField] GameObject brume_Prefab;
    [SerializeField] GameObject Fum�Feu;
    //feu
    public static FeuDeCamp instance;
    DataObjectInteragissable ceFeuDeCamps;
    UIFeuDeCamp uIFeuDeCamp;
    public GameObject FeuCourant;
    private Light lumi�re;

    public int nbBoisRequis;
    public int nbPierreRequis;

    bool EstPremiereInteraction = true;
    bool futAmelior� = false;
    new AudioSource audio;

    //reg�n�rer vie
    GestionVieJoueur gestionVieJoueur;
    bool veutReg�n�rerVie = true;
    float derni�reReg�n�ration;
    float d�laisReg�n�ration = 1;

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
        �tatInitialisation = �tatInitialisation.positionFixe;

        //pool = GameObject.FindGameObjectWithTag("Object_Pool").GetComponent<ObjectPool>();
        uIFeuDeCamp = GetComponentInChildren<UIFeuDeCamp>();
        uIFeuDeCamp.gameObject.SetActive(false);
        gestionVieJoueur = GameObject.FindWithTag("Player").GetComponentInChildren<GestionVieJoueur>();
        lumi�re = GetComponentInChildren<Light>();
        lumi�re.enabled = false;

        niveauActuel = 0;

        FeuCourant = Instantiate(feux[niveauActuel].objet, transform.position, transform.rotation);
        InitialiserDataFeuDeCamps();

        InitialiserBordure();
        //if (brumes != null) D�sactiverBrume();
        //G�n�rerBrume();

        (nbBoisRequis, nbPierreRequis) = D�terminerNbRessourcesNecessaires(niveauActuel);
       
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<Placement>().OnStructureInstanci� += InstancierFeuPrefab;

        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (veutReg�n�rerVie)
            if (Time.time - derni�reReg�n�ration >= d�laisReg�n�ration)
            {
                gestionVieJoueur.RecevoirVie(2);
                derni�reReg�n�ration = Time.time;
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
        bordure.G�n�rerBordure(feux[niveauActuel].rayon);
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
        Instantiate(Fum�Feu, transform);
        lumi�re.enabled = true;
        uIFeuDeCamp.ResetUI();
        futAmelior� = true;
        audio.Play();
        GestionAchievements.ActiverAchievement(Achievements.CaRechauffeLeCoeur);
    }

    protected override void EstEntr�EnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Add(ceFeuDeCamps);
            veutReg�n�rerVie = true;
        }
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Remove(ceFeuDeCamps);
            veutReg�n�rerVie = false;
        }
    }

    public override void InteragirObjet() //ameliore feu
    {
        (bool peutAmeliorer, Cout cout) = PeutAm�liorerSelonRessources();

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
                (nbBoisRequis, nbPierreRequis) = D�terminerNbRessourcesNecessaires(niveauActuel);
                uIFeuDeCamp.ResetUI();
                Granny.instance.G�rerChangementNiveauFeu();

                //Instantie le nouveau feu
                ObjectRamassableVisible.Remove(ceFeuDeCamps);
                Destroy(FeuCourant);
                FeuCourant = Instantiate(feux[niveauActuel].objet, transform.position, transform.rotation);
                InitialiserDataFeuDeCamps();

                //comme OnTriggerEnter ne va pas s'activer car le joueur est d�ja dedans, on set � la main la possibilit� d'interactions
                ObjectRamassableVisible.Add(ceFeuDeCamps);
                GestionInteraction.RendrePlusClair(ceFeuDeCamps);
                bordure.G�n�rerBordure(feux[niveauActuel].rayon);

                futAmelior� = true;

                if(InfoPartie.difficult� != Difficult�.paisible)
                {
                    if (niveauActuel == 5)
                    {
                        GestionAchievements.ActiverAchievement(Achievements.AuFeu);
                    }
                    if (niveauActuel == feux.Count - 1)
                    {
                        uIFeuDeCamp.D�sactiverUI();
                        GestionAchievements.ActiverAchievement(Achievements.CaBrule);
                        if (InfoPartie.difficult� == Difficult�.difficile)
                        {
                            GestionAchievements.ActiverAchievement(Achievements.AuBucher);
                        }
                    }
                }
            }
        }
    }

    public (bool peutAmeliorer, Cout cout) PeutAm�liorerSelonRessources()
    {
        Cout coutAmelioration = new();
        coutAmelioration.AjouterRessource(Ressource.Bois, nbBoisRequis);
        coutAmelioration.AjouterRessource(Ressource.Pierre, nbPierreRequis);

        return (GestionInventaire.EstCoutRempli(coutAmelioration), coutAmelioration);
    }


    public (int nbBois, int nbPierre) D�terminerNbRessourcesNecessaires(int niveau)
    {
        Cout cout = feux[niveau].cout;
        int bois = cout.ressourcesRequises[0];
        int pierre = cout.ressourcesRequises[1];

        return (bois, pierre);
    }

    public override string D�terminerTexteUI() => "Am�liorer le feu";

    public override bool JouerSonInteraction()
    {
        if (futAmelior�)
        {
            GestionBruit.instance.JouerSon("AllumerFeu");
            futAmelior� = false;
            return true;
        }
        return false;
    }


    //Brume: a enlever??
    //private void D�sactiverBrume()
    //{
    //    foreach (GameObject brume in brumes)
    //        brume.SetActive(false);
    //}

    //public void G�n�rerBrume()
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

