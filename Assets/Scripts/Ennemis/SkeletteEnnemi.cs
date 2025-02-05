using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le squelette n'a pas de caract�ristiques sp�ciales. Il est le premier ennemi cr�� et le plus facile � vaincre.
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024
public class SkeletteEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Squelette;
    protected override float PV_INITIAL => 10;

    protected override bool A2Attaques => true;

    const float D�GAT_ATTAQUE = 10;
    protected override float Dur�eAttaque => 0.5f;
    protected override string bruitAttaque => "AttaqueEnnemiSquelette";
    protected override string bruitDegats => "DegatEnnemiSquelette";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 3, 20));
    }
}
