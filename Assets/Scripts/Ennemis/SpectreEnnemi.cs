using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le spectre n'a pas de caractéristiques spéciales autres qu'il est très rapide (plus que le joueur).
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class SpectreEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Spectre;
    protected override float PV_INITIAL => 10;

    protected override bool A2Attaques => true;

    const float DÉGAT_ATTAQUE = 10;
    protected override float DuréeAttaque => 0.3f;
    protected override float TempsEntreAttaques => base.TempsEntreAttaques - 0.5f;
    protected override float Range => base.Range;
    protected override float VitesseAttaque => 10;

    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 20));
    }
}
