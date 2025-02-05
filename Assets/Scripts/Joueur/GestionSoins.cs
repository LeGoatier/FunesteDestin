using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GestionSoins : MonoBehaviour
{
    [SerializeField] GameObject Champigon;
    [SerializeField] GameObject Trousse;

    [SerializeField] AfficheSoin afficheSoin;
    
    bool EstEnTrainDeHeal = false;
    bool PeutSoigner = true;
    public static GestionSoins instance;

    GestionVieJoueur gestionVieJoueur;
    [SerializeField] GameObject caseHeal;
    float[] TableauDePVHeal = new float[(int)Soin.NbSoins] { 20, GestionVieJoueur.PV_INITIAL }; //champignon, trousse

    float Dur�eChampi = 1;
    float Dur�eTrousse = 4;

    // Start is called before the first frame update
    void Start()
    {
        //test
        gestionVieJoueur = GetComponent<GestionVieJoueur>();
        ComportementInterface.instance.OnMenuChangement += ChangerPossibilit�Soin;
        if (instance == null)
        {
            instance = this;
        }
    }

    void ChangerPossibilit�Soin(bool EstJeuActif)
    {
        PeutSoigner = EstJeuActif;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            gestionVieJoueur.RecevoirD�gat(10);
        }
        if (Input.GetButtonDown("Soigner"))
        {
            Soigner(caseHeal.GetComponentInChildren<Item>().soin);
        }
    }

    void Soigner(Soin soin)
    {
        if (Soin.Null != soin && PeutSoigner && !EstEnTrainDeHeal)
        {
            if (GestionInventaire.ObtenirSoin(soin) > 0 && gestionVieJoueur.pv < GestionVieJoueur.PV_INITIAL)
            {
                EstEnTrainDeHeal = true;
                GetComponentInChildren<InventaireFusils>().D�sactiv�TemporairementFusil();
                Animer(soin);//Les lignes dans le else sont appeler � la fin de l'anim
            }
        }
    }

    private void Reg�n�rerVieJoueur(Soin soin)
    {
        gestionVieJoueur.RecevoirVie(TableauDePVHeal[(int)soin]);
    }

    void Animer(Soin soin)
    {
        if (soin == Soin.Champignon)
        {
            StartCoroutine(AnimationSoin(Champigon, soin, Dur�eChampi, 10, 0.01f, new Vector3(0.29f, -0.61f, 1f), Quaternion.Euler(-12.1f, 0f, 0)));
            SliderRechargement.instance.LancerAnimation(Dur�eChampi, Color.green);
        }
        else if (soin == Soin.Trousse)
        {
            StartCoroutine(AnimationSoin(Trousse, soin, Dur�eTrousse, 2, 0.001f, new Vector3(0.1f, -0.28f, 0.82f), Quaternion.Euler(-26.35f, 174, -84.5f)));
            SliderRechargement.instance.LancerAnimation(Dur�eTrousse, Color.green);
        }
    }

    IEnumerator AnimationSoin(GameObject objetSoin, Soin soin, float dur�eAmimation, float VitesseOscillation, float Intensit�Oscillation, Vector3 positionLocale, Quaternion rotationLocale)
    {
        float timer = 0;


        var instance = Instantiate(objetSoin, transform.GetComponentInChildren<Camera>().transform);
        instance.transform.localPosition = positionLocale;
        instance.transform.localRotation = rotationLocale;

        while (timer < dur�eAmimation)
        {
            instance.transform.localPosition += Vector3.up * Mathf.Sin(Time.time * VitesseOscillation) * Intensit�Oscillation;
            timer += Time.deltaTime;
            yield return null;
        }
        Destroy(instance);


        Reg�n�rerVieJoueur(soin);
        GestionInventaire.UtiliserSoin(soin);
        afficheSoin.LancerAnimationAffichage();
        GetComponentInChildren<InventaireFusils>().R�activ�Fusil();
        EstEnTrainDeHeal = false;
    }
}
