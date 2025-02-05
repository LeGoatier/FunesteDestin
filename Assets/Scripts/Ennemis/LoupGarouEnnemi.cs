using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le loup garou est le boss final du jeu
//Il peut se déplacer en même temps qu'il attaque
//Après 4 attaques normales, il entre dans un mode rage qui le fait attaquer sans arrêt
//Auteur : Justin Gauthier
//Date : 26 février 2024

public class LoupGarouEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.LoupGarou;
    const float DÉGAT_ATTAQUE = 15;
    protected override float PV_INITIAL => 20;
    protected override float VitesseAttaque => 13;

    protected override float Range => 3;
    protected override float TempsEntreAttaques => 1;
    private float duréeAttaqueNormale = 0.4f;
    //La durée attaque sera celle durant la rage, car il est plus facile d'attendre plus longtemps que de réécrire des fonctions
    protected override float DuréeAttaque => 0.15f;
    private bool estEnRage = false;

    protected override void AttaquerJoueur()
    {
        StartCoroutine(AttendrePuisVérifierAttaqueMêlée());
    }
    private IEnumerator AttendrePuisVérifierAttaqueMêlée()
    {
        yield return new WaitForSeconds(duréeAttaqueNormale - DuréeAttaque);
        StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 20));
    }

    protected override void Update()
    {
        base.Update();
        if(étatActuel == ÉtatEnnemi.PréparationAttaque || étatActuel == ÉtatEnnemi.Attaque)
        {
            MouvementAttaque();
        }
        if (estEnRage)
        {
            if(tempsDernièreAttaque + DuréeAttaque < Time.time)
            {
                tempsDernièreAttaque = Time.time;
                StartCoroutine(VérifierSiAttaqueMêléeTouche(DÉGAT_ATTAQUE, 3, 20));
            }
        }
    }

    private void Start()
    {
        agent.angularSpeed = agent.angularSpeed * 3;
        agent.acceleration = agent.acceleration * 3;
    }

    int nombreAttaqueNormale = 0;
    const int NOMBRE_ATTAQUE_AVANT_RAGE = 4;
    protected override void VérifierSiAttaque()
    {
        if (EstAttaquePrête() && EstEnRange() && !estEnRage)
        {
            if (nombreAttaqueNormale >= NOMBRE_ATTAQUE_AVANT_RAGE)
            {
                estEnRage = true;
                tempsDernièreAttaque = Time.time;
                étatActuel = ÉtatEnnemi.Attaque;
                animator.SetTrigger("Attaque2");
            }
            else
            {
                AttaquerJoueur();
                tempsDernièreAttaque = Time.time;
                étatActuel = ÉtatEnnemi.Attaque;
                animator.SetTrigger("Attaque");
                ++nombreAttaqueNormale;
            }
        }
    }
}
