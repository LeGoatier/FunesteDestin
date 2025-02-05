using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventaireFusils : MonoBehaviour
{
    public static InventaireFusils instance;

    public List<GameObject> armesÉquipées;

    [SerializeField] GameObject mainVide;

    [SerializeField] GameObject pompe;
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject sniper;
    [SerializeField] GameObject pistolet;
    [SerializeField] GameObject arbalète;

    private GameObject[] armes;

    Camera cam; 

    const int conversionChar = 49;

    GameObject armeSélectionnée;//Gun présentement visible
    int PositionCourante;


    bool PeutChanger = true;
    float VitesseMvt = 0.2f;
    float Timer;

    private void Start()
    {

        instance = this;

        armes = new GameObject[5] { arbalète, pistolet, sniper, rifle, pompe };
        armesÉquipées = new List<GameObject>();

        //Instancie et désactive tous le fusils
        for (int i = 0; i < armes.Length; i++)
        {
            GameObject gunInstancié = Instantiate(armes[i], transform);

            if (i == 2 || i == 3)//si le gun est crafté (sniper et riffle)
            {
                gunInstancié.GetComponent<ComportementFusil>().UnDesMenusEstActif = false;
            }

            armes[i] = gunInstancié;
            gunInstancié.SetActive(false);

            
        }

        //Active  la main vide par défault
        mainVide = Instantiate(mainVide, transform);
        armeSélectionnée = mainVide;

        armesÉquipées.Add(mainVide);
        
        cam = GetComponentInParent<Camera>();

        ComportementInterface visibilitéMenu = GameObject.FindWithTag("GameManager").GetComponent<ComportementInterface>();
        visibilitéMenu.OnMenuChangement += ÉtatMenu;

    }

    void Update()
    {
        if (PeutChanger)
        {
            ChangementNuméroPesé();
            ChangementSourisScroll();
            PrendreMainVide();
        }


    }

    
    int positionInitiale;
    public void DésactivéTemporairementFusil()
    {
        PeutChanger = false;
        positionInitiale = PositionCourante;
        ChangerArme(0);
    }

    public void RéactivéFusil()
    {
        PeutChanger = true;
        ChangerArme(positionInitiale);
    }

    public void ÉquiperArme(Arme arme)
    {
        GameObject armeÀÉquiper = TrouverFusil(arme);
        armesÉquipées.Add(armeÀÉquiper);

        Vérification();
        PositionCourante = armesÉquipées.Count - 1;//(armesÉquipées.Count) < 3? armesÉquipées.Count-1: 2;
        //Debug.Log("equip");
        ChangerArme(PositionCourante);
    }

    public void ÉquiperArme(Arme arme, int index) //Pour conserver la cohérence des positions fusils
    {
        GameObject armeÀÉquiper = TrouverFusil(arme);
        if(index ==1 && armesÉquipées.Count == 2)
        {
            GameObject temp = armesÉquipées[1];
            armesÉquipées.RemoveAt(1);
            armesÉquipées.Add(armeÀÉquiper);
            armesÉquipées.Add(temp);
            Vérification();
            PositionCourante = index;
        }
        else
        {
            armesÉquipées.Add(armeÀÉquiper);
            Vérification();
            PositionCourante = armesÉquipées.Count - 1;
        }
        ChangerArme(PositionCourante);
    }

    void Vérification()
    {
        if (armesÉquipées.Count > 3)
        {
            armesÉquipées.RemoveAt(armesÉquipées.Count - 1);
        }
    }

    public void DéséquiperArme(Arme arme)
    {
        GameObject armeÀDéséquiper = TrouverFusil(arme);
        armesÉquipées.Remove(armeÀDéséquiper);
        if (PositionCourante >= armesÉquipées.Count)
            PositionCourante = armesÉquipées.Count - 1;
        ChangerArme(PositionCourante);
    }

    private void ChangerArme(int position)
    {
        PositionCourante = position;
        StartCoroutine(MouvementChangementArme(PositionCourante));
    }

    public void InverserArmeSlot1ArmeSlot2()
    {
        if(armesÉquipées.Count ==3)
        {
            GameObject armeSlot1 = armesÉquipées[1];
            GameObject armeSlot2 = armesÉquipées[2];

            armesÉquipées.RemoveAt(2);
            armesÉquipées.RemoveAt(1);

            armesÉquipées.Add(armeSlot2);
            armesÉquipées.Add(armeSlot1);

            ChangerArme(PositionCourante);
        }
        

    }

    public GameObject TrouverFusil(Arme arme) => armes[(int)arme];
   
    void ChangementSourisScroll()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            ChangerArme( ( (PositionCourante - 1) % armesÉquipées.Count + armesÉquipées.Count) % armesÉquipées.Count);
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
           ChangerArme((PositionCourante + 1) % armesÉquipées.Count);
    }
    void PrendreMainVide()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("J'appuie sur le mousescroll button");
            ChangerArme(0);
        }
    }
    void ChangementNuméroPesé()
    {

        if (Input.anyKeyDown)
        {
            // Loop des keys 1 à 9 
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {

                    int valeur = i - conversionChar;
                    valeur = Mathf.Clamp(valeur, 0, armesÉquipées.Count - 1);

                    if (valeur != PositionCourante)
                    {
                        ChangerArme(valeur);

                    }
                }
            }
        }


    }



    //Animation du changement
    IEnumerator MouvementChangementArme(int index)
    {
        Timer = 0;
        while (Timer < VitesseMvt)
        {
            Timer += Time.deltaTime / VitesseMvt;
            Vector3 positionInitiale = armeSélectionnée.transform.position;
            armeSélectionnée.transform.position = Vector3.Lerp(positionInitiale, positionInitiale - cam.transform.up * 0.5f, Timer);
            yield return null;
        }
        while (Timer < 2 * VitesseMvt)
        {
            armeSélectionnée.SetActive(false);

            armesÉquipées[index].SetActive(true);
            armeSélectionnée = armesÉquipées[index];

            Timer += Time.deltaTime / VitesseMvt;
            Vector3 positionInitiale = armeSélectionnée.transform.position;
            armeSélectionnée.transform.position = Vector3.Lerp(positionInitiale - cam.transform.up * 0.5f, positionInitiale, Timer - VitesseMvt);
            yield return null;
        }
    }

    void ÉtatMenu(bool JeuActif)
    {
        PeutChanger = JeuActif;

        
        
    }
}
