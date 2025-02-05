using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Le d�mon n'a pas de caract�ristiques sp�ciales autres qu'il est a beaucoup d'attaque et de vie
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024

public class DemonEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.DemonKing;
    protected override float PV_INITIAL => 100;

    protected override bool A2Attaques => true;

    const float D�GAT_ATTAQUE = 35;
    protected override float Dur�eAttaque => 0.5f;
    protected override float Range => base.Range + 0.5f;

    protected override string bruitAttaque => "AttaqueEnnemiDemon";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 5, 20));
    }
}
