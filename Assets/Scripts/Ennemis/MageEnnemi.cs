using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Le mage tire un projectile non affect� par la gravit� dans la direction anticip�e du joueur
//selon un algorithme qui extrapole la position du joueur par sa vitesse actuelle
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024
public class MageEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Mage;
    [SerializeField] Transform EmboutArme;
    [SerializeField] GameObject balleMage;
    protected override float PV_INITIAL => 10;
    protected override float Range => 10;
    protected override float RayonDeD�tection => 20;
    protected override bool A2Attaques => false;
    protected override float Dur�eAttaque => 0.35f;

    const float VITESSE_BALLE = 12;
    protected override string bruitAttaque => "AttaqueEnnemiMage";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(AttendreEtLancerBalle());
    }
    private IEnumerator AttendreEtLancerBalle()
    {
        yield return new WaitForSeconds(Dur�eAttaque);

        GameObject balle = Instantiate(balleMage);
        balle.transform.position = EmboutArme.position;
        Vector3 directionLancement = ObtenirDirectionLancement(EmboutArme.position);
        balle.GetComponent<Rigidbody>().velocity = directionLancement * VITESSE_BALLE;

        GestionBruit.instance.JouerSon(bruitAttaque);
    }

    //Les math�matiques derri�re l'algorithme utilis� sont expliqu�es dans le document word algorithmes
    private Vector3 ObtenirDirectionLancement(Vector3 origine)
    {
        Vector3 positionAbsolueCible = joueur.transform.position;
        Vector3 vitesseInitialeJoueur = joueur.GetComponent<CharacterController>().velocity;
        //On veut changer le syst�me de coordon�es
        Vector3 positionInitialeCible = positionAbsolueCible - origine;
        //On d�finit maintenant les param�tre pour l'�quation quadratique tel qu'expliqu� dans le document d'algorithme
        float A = -Mathf.Pow(VITESSE_BALLE, 2) + Mathf.Pow(vitesseInitialeJoueur.x, 2) + Mathf.Pow(vitesseInitialeJoueur.y, 2) + Mathf.Pow(vitesseInitialeJoueur.z, 2);
        float B = 2 * positionInitialeCible.x * vitesseInitialeJoueur.x + 2 * positionInitialeCible.y * vitesseInitialeJoueur.y + 2 * positionInitialeCible.z * vitesseInitialeJoueur.z;
        float C = Mathf.Pow(positionInitialeCible.x, 2) + Mathf.Pow(positionInitialeCible.y, 2) + Mathf.Pow(positionInitialeCible.z, 2);
        float discriminant = Mathf.Pow(B, 2) - 4 * A * C;
        //Si le discriminant est plus petit que 0 le projectile ne pourra pas atteindre la cible, alors on ne le lancera pas
        if (discriminant >= 0)
        {
            float temps = (-B + Mathf.Sqrt(discriminant))/(2*A);
            //Si le temps est n�gatif, on prendra l'autre solution
            if(temps < 0)
                temps = (-B - Mathf.Sqrt(discriminant)) / (2 * A);

            Vector3 positionFutureJoueur = positionInitialeCible + vitesseInitialeJoueur * temps;
            return positionFutureJoueur.normalized;
        }
        else
        {
            //Devrait pas vraiment arriver
            return transform.forward;
        }
    }
}
