using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le scarab�e a une ru�e (dash), qui est toujours sa mani�re d'engager le combat
//Cette ru�e se fait en ligne droite vers la position du joueur au d�but de l'action
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024
public class SpiderEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Spider;
    const float D�GAT_ATTAQUE = 7;
    const float D�GAT_DASH = 15;
    protected override float PV_INITIAL => 5;
    protected override float VitesseAttaque => 13;

    private float rangeAttaque = 2.5f;
    //Le range de base est le range de dash parce que �a fonctionne mieux avec la d�tection
    protected override float Range => 6;
    bool estEnDash = false;
    protected override float TempsEntreAttaques => 1;
    const float TEMPS_DASH = 0.6f;

    protected override void AttaquerJoueur()
    {
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 3, 15));
    }
    

    Vector3 positionCibleDash;
    Vector3 positionAvantDash;
    float tempsD�butDash;
    private void Dash()
    {
        positionAvantDash = transform.position;
        positionCibleDash = new Vector3(joueur.transform.position.x, transform.position.y, joueur.transform.position.z);
        tempsD�butDash = Time.time;
        estEnDash = true;
        //Pas id�al de mettre 2 coroutines imbriqu�es, mais on n'a pas le choix si on veut que le
        //dash soit seulement v�rifi� sur son propre temps
        StartCoroutine(V�rifierSiDashToucheD�lai());
    }

    private IEnumerator V�rifierSiDashToucheD�lai()
    {
        yield return new WaitForSeconds(TEMPS_DASH - Dur�eAttaque);
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_DASH, 5, 20));
    }

    protected override void Update()
    {
        base.Update();
        if (estEnDash)
        {
            //On set manuellement la position de l'agent, donc on lui dit d'arr�ter de bouger automatiquement
            agent.isStopped = true;
            agent.nextPosition = Vector3.Lerp(positionAvantDash, positionCibleDash, (Time.time - tempsD�butDash) / TEMPS_DASH); ;
            estEnDash = Time.time - tempsD�butDash < TEMPS_DASH;
            if (!estEnDash)
                agent.isStopped = false;
        }
    }

    protected override void V�rifierSiAttaque()
    {
        if (EstAttaquePr�te())
        {
            if (EstEnRangeAttaque())
            {
                AttaquerJoueur();
                tempsDerni�reAttaque = Time.time;
                �tatActuel = �tatEnnemi.Attaque;
                animator.SetTrigger("Attaque");
            }
            else if (EstEnRange())
            {
                Dash();
                tempsDerni�reAttaque = Time.time;
                �tatActuel = �tatEnnemi.Attaque;
                animator.SetTrigger("Attaque2");
            }
        }
    }

    private bool EstEnRangeAttaque()
    {
        return Vector3.Distance(joueur.transform.position, transform.position) <= rangeAttaque;
    }

}
