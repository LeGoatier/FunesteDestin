using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//La salamandre n'a pas de caract�ristiques sp�ciales autres qu'elle est rapide et a peu de vie
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024
public class SalamandreEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Salamandre;
    protected override float PV_INITIAL => 7;

    const float D�GAT_ATTAQUE = 10;
    protected override float Dur�eAttaque => 0.3f;
    protected override float VitesseAttaque => 10;
    protected override string bruitAttaque => "AttaqueEnnemiSalamandre";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 3, 15));
    }
}
