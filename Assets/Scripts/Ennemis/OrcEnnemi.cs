using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//L'orc a 2 attaques diff�rentes : une attaque normale et un spin sur lui-m�me, qui sont d�clench�es de mani�re al�atoire
//d�s qu'il est en port�e du joueur
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024
public class OrcEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Orc;
    const float DUR�E_SPIN = 2.6f;
    protected override float PV_INITIAL => 35;

    const float D�GAT_ATTAQUE = 14;
    const float D�GAT_SPIN = 16;
    protected override float Dur�eAttaque => 0.3f;
    const float PROBABILIT�_ATTAQUE_NORMALE = 0.7f;
    protected override float TempsEntreAttaques => 1.1f;
    
    protected override void AttaquerJoueur()
    {
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 3, 20));
    }
    protected override void V�rifierSiAttaque()
    {
        if (EstAttaquePr�te() && EstEnRange())
        {
            if (UnityEngine.Random.value < PROBABILIT�_ATTAQUE_NORMALE)
            {
                AttaquerJoueur();
                tempsDerni�reAttaque = Time.time;
                �tatActuel = �tatEnnemi.Attaque;
                animator.SetTrigger("Attaque");
            }
            else
            {
                Spin();
                tempsDerni�reAttaque = Time.time;
                �tatActuel = �tatEnnemi.Attaque;
                animator.SetTrigger("Attaque2");
            }
        }
    }
    //Si le joueur reste dans la zone de port�e de l'orc, il prendra des d�gats 2 fois (1 par tour sur lui-m�me)
    private void Spin()
    {
        StartCoroutine(V�rifierSpin1());
        StartCoroutine(V�rifierSpin2());
    }

    private IEnumerator V�rifierSpin1()
    {
        yield return new WaitForSeconds(DUR�E_SPIN/4);
        if (EstEnRange())
        {
            gestionVieJoueur.RecevoirD�gat(D�GAT_SPIN);
        }
        tempsDerni�reAttaque = Time.time;
    }
    private IEnumerator V�rifierSpin2()
    {
        yield return new WaitForSeconds(DUR�E_SPIN / 2);
        if (EstEnRange())
        {
            gestionVieJoueur.RecevoirD�gat(D�GAT_SPIN);
        }
        tempsDerni�reAttaque = Time.time;
    }
}
