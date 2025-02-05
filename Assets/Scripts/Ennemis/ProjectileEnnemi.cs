using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileEnnemi : MonoBehaviour
{
    const float TEMPS_POUR_D�SACTIVER = 12;
    [SerializeField] float d�gat;
    GameObject joueur;
    GestionVieJoueur gestionVieJoueur;
    float tempsD�but;
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
            gestionVieJoueur.RecevoirD�gat(d�gat);
        }
    }
    private void OnEnable()
    {
        tempsD�but = Time.time;
    }
    private void Update()
    {
        if (Time.time > tempsD�but + TEMPS_POUR_D�SACTIVER)
            gameObject.SetActive(false);
    }

}
