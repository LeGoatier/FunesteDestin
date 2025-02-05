using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplicationsMouvements : �tapeDidacticiel
{
    [SerializeField] GameObject texte1;
    [SerializeField] float temps1 = 2f;
    [SerializeField] GameObject texte2;
    [SerializeField] float temps2;
    [SerializeField] GameObject texte3;
    [SerializeField] float temps3;
    [SerializeField] GameObject texte4;
    [SerializeField] float temps4;

    protected override void Start()
    {
        base.Start();
        texte1.SetActive(false);
        texte2.SetActive(false);
        texte3.SetActive(false);
        texte4.SetActive(false);

    }



    void Update()
    {
        if (actif)
        {
            switch (progression)
            {
                case 1: �tape1(); break;
                case 2: �tape2(); break;
                case 3: �tape3(); break;
            }
        }
    }

    protected override IEnumerator D�buter()
    {
        yield return new WaitForSeconds(4.5f);
        D�but�tape1 ();
        actif = true;
    }
    void D�but�tape1()
    {
        texte1.SetActive(true);
        Time.timeScale = 0.01f;
    }
    void �tape1()
    {
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            // �tape finale de l'�tape 1
            texte1.SetActive(false);
            progression++;
            Time.timeScale = 1;
            pretFaireEtape = false;

            // �tat initial de l'�tape 2
            StartCoroutine(AttendreEtLancerFonction(temps1, D�but�tape2));
            actif = false;
        }
    }
   
    void D�but�tape2()
    {
        actif = true;
        texte2.SetActive(true);
        Time.timeScale = 0.01f;
    }

    void �tape2()
    {
        if(Input.GetButtonDown("Jump"))
        {
            texte2.SetActive(false);
            progression++;
            Time.timeScale = 1;
            StartCoroutine(AttendreEtLancerFonction(temps2, D�but�tape3));
            actif = false;
        }
    }
    void D�but�tape3()
    {
        texte3.SetActive(true);
        actif = true;
        Time.timeScale = 0.01f;
    }

    void �tape3()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            texte3.SetActive(false);
            progression++;
            Time.timeScale = 1;

            StartCoroutine(AttendreEtLancerFonction(temps3, D�but�tape4));
            actif = false;
        }
    }

    void D�but�tape4()
    {
        texte4.SetActive(true);
        StartCoroutine(Pr�parerNext());//Il n'y a pas de v�rification pour l'�tape 4, on active le texte 1 ou 2 seconde et passe � la prochaine �tape du tutoriel
    }

    IEnumerator Pr�parerNext()
    {
        yield return new WaitForSeconds(temps4);
        texte4.SetActive(false);
        GestionDidacticiel.instance.Next();
    }
}



