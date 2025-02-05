using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnnemi : MonoBehaviour
{
    const float TEMPS_POUR_DÉSACTIVER = 12;
    [SerializeField] float dégat;
    GameObject joueur;
    GestionVieJoueur gestionVieJoueur;
    float tempsDébut;
    private void Awake()
    {
        joueur = GameObject.FindGameObjectWithTag("Player");
        gestionVieJoueur = joueur.GetComponent<GestionVieJoueur>();
    }
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
        if(other.gameObject.tag == "Player")
        {
            gestionVieJoueur.RecevoirDégat(dégat);
        }
    }
    private void OnEnable()
    {
        tempsDébut = Time.time;
    }
    private void Update()
    {
        if (Time.time > tempsDébut + TEMPS_POUR_DÉSACTIVER)
            gameObject.SetActive(false);
    }

}
