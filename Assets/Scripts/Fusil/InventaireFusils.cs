using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventaireFusils : MonoBehaviour
{
    public static InventaireFusils instance;

    public List<GameObject> armes�quip�es;

    [SerializeField] GameObject mainVide;

    [SerializeField] GameObject pompe;
    [SerializeField] GameObject rifle;
    [SerializeField] GameObject sniper;
    [SerializeField] GameObject pistolet;
    [SerializeField] GameObject arbal�te;

    private GameObject[] armes;

    Camera cam; 

    const int conversionChar = 49;

    GameObject armeS�lectionn�e;//Gun pr�sentement visible
    int PositionCourante;


    bool PeutChanger = true;
    float VitesseMvt = 0.2f;
    float Timer;

    private void Start()
    {

        instance = this;

        armes = new GameObject[5] { arbal�te, pistolet, sniper, rifle, pompe };
        armes�quip�es = new List<GameObject>();

        //Instancie et d�sactive tous le fusils
        for (int i = 0; i < armes.Length; i++)
        {
            GameObject gunInstanci� = Instantiate(armes[i], transform);

            if (i == 2 || i == 3)//si le gun est craft� (sniper et riffle)
            {
                gunInstanci�.GetComponent<ComportementFusil>().UnDesMenusEstActif = false;
            }

            armes[i] = gunInstanci�;
            gunInstanci�.SetActive(false);

            
        }

        //Active  la main vide par d�fault
        mainVide = Instantiate(mainVide, transform);
        armeS�lectionn�e = mainVide;

        armes�quip�es.Add(mainVide);
        
        cam = GetComponentInParent<Camera>();

        ComportementInterface visibilit�Menu = GameObject.FindWithTag("GameManager").GetComponent<ComportementInterface>();
        visibilit�Menu.OnMenuChangement += �tatMenu;

    }

    void Update()
    {
        if (PeutChanger)
        {
            ChangementNum�roPes�();
            ChangementSourisScroll();
            PrendreMainVide();
        }


    }

    
    int positionInitiale;
    public void D�sactiv�TemporairementFusil()
    {
        PeutChanger = false;
        positionInitiale = PositionCourante;
        ChangerArme(0);
    }

    public void R�activ�Fusil()
    {
        PeutChanger = true;
        ChangerArme(positionInitiale);
    }

    public void �quiperArme(Arme arme)
    {
        GameObject arme��quiper = TrouverFusil(arme);
        armes�quip�es.Add(arme��quiper);

        V�rification();
        PositionCourante = armes�quip�es.Count - 1;//(armes�quip�es.Count) < 3? armes�quip�es.Count-1: 2;
        //Debug.Log("equip");
        ChangerArme(PositionCourante);
    }

    public void �quiperArme(Arme arme, int index) //Pour conserver la coh�rence des positions fusils
    {
        GameObject arme��quiper = TrouverFusil(arme);
        if(index ==1 && armes�quip�es.Count == 2)
        {
            GameObject temp = armes�quip�es[1];
            armes�quip�es.RemoveAt(1);
            armes�quip�es.Add(arme��quiper);
            armes�quip�es.Add(temp);
            V�rification();
            PositionCourante = index;
        }
        else
        {
            armes�quip�es.Add(arme��quiper);
            V�rification();
            PositionCourante = armes�quip�es.Count - 1;
        }
        ChangerArme(PositionCourante);
    }

    void V�rification()
    {
        if (armes�quip�es.Count > 3)
        {
            armes�quip�es.RemoveAt(armes�quip�es.Count - 1);
        }
    }

    public void D�s�quiperArme(Arme arme)
    {
        GameObject arme�D�s�quiper = TrouverFusil(arme);
        armes�quip�es.Remove(arme�D�s�quiper);
        if (PositionCourante >= armes�quip�es.Count)
            PositionCourante = armes�quip�es.Count - 1;
        ChangerArme(PositionCourante);
    }

    private void ChangerArme(int position)
    {
        PositionCourante = position;
        StartCoroutine(MouvementChangementArme(PositionCourante));
    }

    public void InverserArmeSlot1ArmeSlot2()
    {
        if(armes�quip�es.Count ==3)
        {
            GameObject armeSlot1 = armes�quip�es[1];
            GameObject armeSlot2 = armes�quip�es[2];

            armes�quip�es.RemoveAt(2);
            armes�quip�es.RemoveAt(1);

            armes�quip�es.Add(armeSlot2);
            armes�quip�es.Add(armeSlot1);

            ChangerArme(PositionCourante);
        }
        

    }

    public GameObject TrouverFusil(Arme arme) => armes[(int)arme];
   
    void ChangementSourisScroll()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
            ChangerArme( ( (PositionCourante - 1) % armes�quip�es.Count + armes�quip�es.Count) % armes�quip�es.Count);
        else if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
           ChangerArme((PositionCourante + 1) % armes�quip�es.Count);
    }
    void PrendreMainVide()
    {
        if (Input.GetMouseButtonDown(2))
        {
            Debug.Log("J'appuie sur le mousescroll button");
            ChangerArme(0);
        }
    }
    void ChangementNum�roPes�()
    {

        if (Input.anyKeyDown)
        {
            // Loop des keys 1 � 9 
            for (int i = (int)KeyCode.Alpha0; i <= (int)KeyCode.Alpha9; i++)
            {
                if (Input.GetKeyDown((KeyCode)i))
                {

                    int valeur = i - conversionChar;
                    valeur = Mathf.Clamp(valeur, 0, armes�quip�es.Count - 1);

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
            Vector3 positionInitiale = armeS�lectionn�e.transform.position;
            armeS�lectionn�e.transform.position = Vector3.Lerp(positionInitiale, positionInitiale - cam.transform.up * 0.5f, Timer);
            yield return null;
        }
        while (Timer < 2 * VitesseMvt)
        {
            armeS�lectionn�e.SetActive(false);

            armes�quip�es[index].SetActive(true);
            armeS�lectionn�e = armes�quip�es[index];

            Timer += Time.deltaTime / VitesseMvt;
            Vector3 positionInitiale = armeS�lectionn�e.transform.position;
            armeS�lectionn�e.transform.position = Vector3.Lerp(positionInitiale - cam.transform.up * 0.5f, positionInitiale, Timer - VitesseMvt);
            yield return null;
        }
    }

    void �tatMenu(bool JeuActif)
    {
        PeutChanger = JeuActif;

        
        
    }
}
