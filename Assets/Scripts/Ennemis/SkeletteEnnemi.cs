using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le squelette n'a pas de caractéristiques spéciales. Il est le premier ennemi créé et le plus facile à vaincre.
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class SkeletteEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Squelette;
    protected override float PV_INITIAL => 10;

    protected override bool A2Attaques => true;

    const float DÉGAT_ATTAQUE = 10;
    protected override float DuréeAttaque => 0.5f;
    protected override string bruitAttaque => "AttaqueEnnemiSquelette";
    protected override string bruitDegats => "DegatEnnemiSquelette";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 20));
    }
}
