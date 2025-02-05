using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Le démon n'a pas de caractéristiques spéciales autres qu'il est a beaucoup d'attaque et de vie
//Auteur : Justin Gauthier
//Date : 26 février 2024

public class DemonEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.DemonKing;
    protected override float PV_INITIAL => 100;

    protected override bool A2Attaques => true;

    const float DÉGAT_ATTAQUE = 35;
    protected override float DuréeAttaque => 0.5f;
    protected override float Range => base.Range + 0.5f;

    protected override string bruitAttaque => "AttaqueEnnemiDemon";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 5, 20));
    }
}
