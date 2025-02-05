using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//L'orc a 2 attaques différentes : une attaque normale et un spin sur lui-même, qui sont déclenchées de manière aléatoire
//dès qu'il est en portée du joueur
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class OrcEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Orc;
    const float DURÉE_SPIN = 2.6f;
    protected override float PV_INITIAL => 35;

    const float DÉGAT_ATTAQUE = 14;
    const float DÉGAT_SPIN = 16;
    protected override float DuréeAttaque => 0.3f;
    const float PROBABILITÉ_ATTAQUE_NORMALE = 0.7f;
    protected override float TempsEntreAttaques => 1.1f;
    
    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 20));
    }
    protected override void VérifierSiAttaque()
    {
        if (EstAttaquePrête() && EstEnRange())
        {
            if (UnityEngine.Random.value < PROBABILITÉ_ATTAQUE_NORMALE)
            {
                AttaquerJoueur();
                tempsDernièreAttaque = Time.time;
                étatActuel = ÉtatEnnemi.Attaque;
                animator.SetTrigger("Attaque");
            }
            else
            {
                Spin();
                tempsDernièreAttaque = Time.time;
                étatActuel = ÉtatEnnemi.Attaque;
                animator.SetTrigger("Attaque2");
            }
        }
    }
    //Si le joueur reste dans la zone de portée de l'orc, il prendra des dégats 2 fois (1 par tour sur lui-même)
    private void Spin()
    {
        StartCoroutine(VérifierSpin1());
        StartCoroutine(VérifierSpin2());
    }

    private IEnumerator VérifierSpin1()
    {
        yield return new WaitForSeconds(DURÉE_SPIN/4);
        if (EstEnRange())
        {
            gestionVieJoueur.RecevoirDégat(DÉGAT_SPIN);
        }
        tempsDernièreAttaque = Time.time;
    }
    private IEnumerator VérifierSpin2()
    {
        yield return new WaitForSeconds(DURÉE_SPIN / 2);
        if (EstEnRange())
        {
            gestionVieJoueur.RecevoirDégat(DÉGAT_SPIN);
        }
        tempsDernièreAttaque = Time.time;
    }
}
