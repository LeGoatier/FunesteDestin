using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NiveauxEnnemisSpawner : MonoBehaviour
{
    //Ennemis faciles
    [SerializeField] GameObject prefabSquelette;
    [SerializeField] GameObject prefabSalamandre;
    [SerializeField] GameObject prefabDragon;

    //Ennemis moyens
    [SerializeField] GameObject prefabSpectre;
    [SerializeField] GameObject prefabMage;
    [SerializeField] GameObject prefabSpider;
    [SerializeField] GameObject prefabOrc;

    //Ennemis difficiles
    [SerializeField] GameObject prefabLézard;
    [SerializeField] GameObject prefabDemonKing;
    [SerializeField] GameObject prefabLoupGarou;

    List<GameObject> ennemisEnJeu = new List<GameObject>();
    int niveauActuel = 0;
    const int TEMPS_ENTRE_NIVEAUX = 5;
    Vector3 positionInitiale = new Vector3(0, 1, -20);

    GameObject joueur;
    GestionVieJoueur gestionVieJoueur;

    private bool estNiveauFini = true;
    private bool estNiveauEnCours = false;
    private void Awake()
    {
        joueur = GameObject.FindGameObjectWithTag("Player");
        gestionVieJoueur = joueur.GetComponent<GestionVieJoueur>();
    }

    private void Update()
    {
        if(estNiveauFini)
        {
            StartCoroutine(AttendreEtChangerNiveau());
            estNiveauFini = false;
        }
        if (estNiveauEnCours)
        {
            if (SontTousEnnemisMorts())
            {
                estNiveauFini = true;
                estNiveauEnCours = false;
            }
        }
    }

    private bool SontTousEnnemisMorts()
    {
        bool unEnVie = false;
        foreach(GameObject ennemi in ennemisEnJeu)
        {
            if (ennemi.activeInHierarchy)
            {
                unEnVie = true;
            }
        }
        return !unEnVie;
    }

    private IEnumerator AttendreEtChangerNiveau()
    {
        yield return new WaitForSeconds(TEMPS_ENTRE_NIVEAUX);
        DémarrerNiveau(++niveauActuel);
        estNiveauEnCours = true;
    }

    private void DémarrerNiveau(int niveau)
    {
        gestionVieJoueur.RéinitialiserVie();
        StartCoroutine(ReplacerJoueur());
        switch (niveau)
        {
            case 1 :
                Niveau1();
                break;
            case 2:
                Niveau2();
                break;
            case 3:
                Niveau3();
                break;
            case 4:
                Niveau4();
                break;
            case 5:
                Niveau5();
                break;
        }
    }

    private IEnumerator ReplacerJoueur()
    {
        yield return new WaitForEndOfFrame();
        joueur.transform.position = positionInitiale;
        joueur.transform.rotation = Quaternion.Euler(Vector3.forward);
    }

    private void Niveau1()
    {
        ennemisEnJeu.Add(Instantiate(prefabSquelette, new Vector3(-10, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSquelette, new Vector3(0, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSquelette, new Vector3(10, 1, 10), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabSalamandre, new Vector3(-20, 1, 15), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSalamandre, new Vector3(20, 1, 15), Quaternion.identity));
    }

    private void Niveau2()
    {
        ennemisEnJeu.Add(Instantiate(prefabSpectre, new Vector3(-10, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSpectre, new Vector3(10, 1, 10), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabDragon, new Vector3(-20, 1, 0), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabDragon, new Vector3(20, 1, 0), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabOrc, new Vector3(0, 1, 15), Quaternion.identity));
    }

    private void Niveau3()
    {
        ennemisEnJeu.Add(Instantiate(prefabSpider, new Vector3(-10, 1, 0), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSpider, new Vector3(0, 1, 0), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSpider, new Vector3(10, 1, 0), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabMage, new Vector3(-10, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabMage, new Vector3(0, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabMage, new Vector3(10, 1, 10), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabOrc, new Vector3(-20, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabOrc, new Vector3(20, 1, 10), Quaternion.identity));
    }

    private void Niveau4()
    {
        ennemisEnJeu.Add(Instantiate(prefabSpectre, new Vector3(-10, 1, 0), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSpectre, new Vector3(0, 1, 0), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabSpectre, new Vector3(10, 1, 0), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabDemonKing, new Vector3(0, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabMage, new Vector3(-10, 1, 15), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabMage, new Vector3(10, 1, 15), Quaternion.identity));

        ennemisEnJeu.Add(Instantiate(prefabLézard, new Vector3(-20, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabLézard, new Vector3(20, 1, 10), Quaternion.identity));
    }

    private void Niveau5()
    {
        ennemisEnJeu.Add(Instantiate(prefabDemonKing, new Vector3(0, 1, 0), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabLoupGarou, new Vector3(-10, 1, 10), Quaternion.identity));
        ennemisEnJeu.Add(Instantiate(prefabLoupGarou, new Vector3(10, 1, 10), Quaternion.identity));
    }

}
