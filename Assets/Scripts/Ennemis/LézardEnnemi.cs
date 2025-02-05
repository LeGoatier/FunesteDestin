using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le lézard n'a pas de caractéristiques spéciales autres qu'il est a beaucoup d'attaque et de vie
//Aussi, il a 3 animations d'attaques différentes, alors il précise comment déclencher ces animations
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class LézardEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Lézard;
    protected override float PV_INITIAL => 40;

    const float DÉGAT_ATTAQUE = 25;
    protected override float DuréeAttaque => 0.5f;
    protected override float Range => base.Range + 0.3f;

    protected override void AttaquerJoueur()
    {
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 5, 20));
    }

    protected override void VérifierSiAttaque()
    {
        if (EstEnRange() && EstAttaquePrête())
        {
            AttaquerJoueur();
            tempsDernièreAttaque = Time.time;
            étatActuel = ÉtatEnnemi.Attaque;
            float rValue = UnityEngine.Random.value;
            if (rValue < 0.333f)
            {
                animator.SetTrigger("Attaque");
            }
            else if(rValue < 0.666f)
            {
                animator.SetTrigger("Attaque2");
            }
            else
            {
                animator.SetTrigger("Attaque3");
            }
        }
    }

}
