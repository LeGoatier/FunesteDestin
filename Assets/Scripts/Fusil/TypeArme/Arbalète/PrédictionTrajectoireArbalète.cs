using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrédictionTrajectoireArbalète : MonoBehaviour
{
    [SerializeField] GameObject TrajectoireObject;

    bool PeutPrédire = true;
    GameObject Embout;
    Vector3[] tableauPoints;
    GameObject[] tableauPointGameObject;
    const int NbPoints = 40;
    const float VariationTemps = 0.028f;
    float AccélérationY;

    const float vitesseAnimaiton = 1;
    const float vitesseMin = 30;
    const float vitesseMax = 60;
    const float vitesseVisée = 0.5f; //vitesse avec laquelle on passe de l'état non-visé à l'état visé
    public float VitesseCourante; //vitesse en cours à cet instant

    float TimerAnim;

    void Start()
    {
        GetComponent<ComportementArbalète>().OnRechargement += OnRechargement;
        VitesseCourante = 50;
       
        AccélérationY = Physics.gravity.y;
        Embout = transform.Find("Embout").gameObject;

        tableauPointGameObject = new GameObject[NbPoints];
        InstancierPoints();
        DétruireTrajectoirePrécédente();
    }

    void Update()
    {
        if (PeutPrédire)
        {
            AvancerTimerAnimation();
            Viser();
            PrédireTrajectoire();
            RepositionnerPoints();
            ActiverPoints();
        }
        
    }

    //Timer qui sert à faire avancé les balles dans la prediction en suivant la trajectoire
    void AvancerTimerAnimation()
    {
        TimerAnim += vitesseAnimaiton * Time.deltaTime;
        if (TimerAnim > 1)
        {
            TimerAnim = 0;
        }
    }

    void OnRechargement(object sender, RechargementEventArgs e)
    {
        PeutPrédire = false;
        DétruireTrajectoirePrécédente() ;
        StartCoroutine(pendantRecharge(e.TempsRechargement));
    }

    IEnumerator pendantRecharge(float durée)
    {
        
        yield return new WaitForSeconds(durée);

        PeutPrédire = true;
    }

    void Viser()
    {
        if(Input.GetMouseButton(1))
        {
            VitesseCourante = Mathf.Lerp(VitesseCourante, vitesseMax, Time.deltaTime/ vitesseVisée);
        }
        else
        {
            VitesseCourante = Mathf.Lerp(VitesseCourante, vitesseMin, Time.deltaTime/ vitesseVisée);
        }
        
    }

    void InstancierPoints()
    {
        for(int i = 0; i < NbPoints; i++)
        {
            GameObject pointTemp = ObjectPool.instance.GetPoolObject(TrajectoireObject);
            tableauPointGameObject[i] = pointTemp;
            pointTemp.SetActive(true);
        }
    }
    private void RepositionnerPoints()
    {
        for (int i = 0; i < NbPoints; i++)
        {
            tableauPointGameObject[i].transform.position = tableauPoints[i];
        }
    }
    private void ActiverPoints()
    {
        foreach (GameObject point in tableauPointGameObject)
        {
            point.SetActive(true);
        }
    }
    void DétruireTrajectoirePrécédente()
    {
        if(tableauPointGameObject != null)
        {
            foreach (GameObject point in tableauPointGameObject)
            {
                if(point != null)
                {
                    point.SetActive(false);
                }
                
            }
        }
        
    }

    void PrédireTrajectoire()
    {
        tableauPoints = new Vector3[NbPoints];

        float TempsDébut = Mathf.Lerp(0, VariationTemps, TimerAnim);
        float angleInitial = -transform.rotation.eulerAngles.x * Mathf.PI/180f;
        Vector3 positionInitiale = Embout.transform.position;
        Vector3 directon = transform.forward;

        Vector3 vitsseXZ = VitesseCourante * directon ;
        float vitsseInitialeY = VitesseCourante * Mathf.Sin(angleInitial);

        for (int i = 0; i < NbPoints; i++)
        {
            Vector3 posFinalXZ = TrouverPointsXZ(TempsDébut + VariationTemps * i, positionInitiale, vitsseXZ);
            float posFinaly = TrouverPointY(TempsDébut + VariationTemps * i, positionInitiale, vitsseInitialeY, AccélérationY);

            tableauPoints[i] = new Vector3(posFinalXZ.x, posFinaly, posFinalXZ.z);
        }
    }

    private static Vector3 TrouverPointsXZ(float instant, Vector3 positionInitiale, Vector3 Vitesse)
    {
        Vector3 positionFinale = positionInitiale + (Vitesse * instant);

        return positionFinale;
    }
   
    private static float TrouverPointY(float instant, Vector3 positionInitiale, float Vitesse, float accélération)
    {
        float positionFinaleY = positionInitiale.y + (Vitesse * instant) + 0.5f * accélération * Mathf.Pow(instant,2) ;


        return positionFinaleY;
    }

    bool Quitter = false;

    private void OnDisable()
    {
        if (!Quitter) //Sinon onDisable est appelé après Quit, ce qui créé une erreur
        {
            DétruireTrajectoirePrécédente();
        }

    }

    private void OnApplicationQuit()
    {
        Quitter = true ;
    }
}
