using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le scarabée a une ruée (dash), qui est toujours sa manière d'engager le combat
//Cette ruée se fait en ligne droite vers la position du joueur au début de l'action
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class SpiderEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Spider;
    const float DÉGAT_ATTAQUE = 7;
    const float DÉGAT_DASH = 15;
    protected override float PV_INITIAL => 5;
    protected override float VitesseAttaque => 13;

    private float rangeAttaque = 2.5f;
    //Le range de base est le range de dash parce que ça fonctionne mieux avec la détection
    protected override float Range => 6;
    bool estEnDash = false;
    protected override float TempsEntreAttaques => 1;
    const float TEMPS_DASH = 0.6f;

    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 15));
    }
    

    Vector3 positionCibleDash;
    Vector3 positionAvantDash;
    float tempsDébutDash;
    private void Dash()
    {
        positionAvantDash = transform.position;
        positionCibleDash = new Vector3(joueur.transform.position.x, transform.position.y, joueur.transform.position.z);
        tempsDébutDash = Time.time;
        estEnDash = true;
        //Pas idéal de mettre 2 coroutines imbriquées, mais on n'a pas le choix si on veut que le
        //dash soit seulement vérifié sur son propre temps
        StartCoroutine(VérifierSiDashToucheDélai());
    }

    private IEnumerator VérifierSiDashToucheDélai()
    {
        yield return new WaitForSeconds(TEMPS_DASH - DuréeAttaque);
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_DASH, 5, 20));
    }

    protected override void Update()
    {
        base.Update();
        if (estEnDash)
        {
            //On set manuellement la position de l'agent, donc on lui dit d'arrêter de bouger automatiquement
            agent.isStopped = true;
            agent.nextPosition = Vector3.Lerp(positionAvantDash, positionCibleDash, (Time.time - tempsDébutDash) / TEMPS_DASH); ;
            estEnDash = Time.time - tempsDébutDash < TEMPS_DASH;
            if (!estEnDash)
                agent.isStopped = false;
        }
    }

    protected override void VérifierSiAttaque()
    {
        if (EstAttaquePrête())
        {
            if (EstEnRangeAttaque())
            {
                AttaquerJoueur();
                tempsDernièreAttaque = Time.time;
                étatActuel = ÉtatEnnemi.Attaque;
                animator.SetTrigger("Attaque");
            }
            else if (EstEnRange())
            {
                Dash();
                tempsDernièreAttaque = Time.time;
                étatActuel = ÉtatEnnemi.Attaque;
                animator.SetTrigger("Attaque2");
            }
        }
    }

    private bool EstEnRangeAttaque()
    {
        return Vector3.Distance(joueur.transform.position, transform.position) <= rangeAttaque;
    }

}
