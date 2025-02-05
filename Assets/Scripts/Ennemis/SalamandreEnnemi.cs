using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//La salamandre n'a pas de caractéristiques spéciales autres qu'elle est rapide et a peu de vie
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class SalamandreEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Salamandre;
    protected override float PV_INITIAL => 7;

    const float DÉGAT_ATTAQUE = 10;
    protected override float DuréeAttaque => 0.3f;
    protected override float VitesseAttaque => 10;
    protected override string bruitAttaque => "AttaqueEnnemiSalamandre";

    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 15));
    }
}
