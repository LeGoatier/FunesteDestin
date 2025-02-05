using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le loup garou est le boss final du jeu
//Il peut se d�placer en m�me temps qu'il attaque
//Apr�s 4 attaques normales, il entre dans un mode rage qui le fait attaquer sans arr�t
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024

public class LoupGarouEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.LoupGarou;
    const float D�GAT_ATTAQUE = 15;
    protected override float PV_INITIAL => 20;
    protected override float VitesseAttaque => 13;

    protected override float Range => 3;
    protected override float TempsEntreAttaques => 1;
    private float dur�eAttaqueNormale = 0.4f;
    //La dur�e attaque sera celle durant la rage, car il est plus facile d'attendre plus longtemps que de r��crire des fonctions
    protected override float Dur�eAttaque => 0.15f;
    private bool estEnRage = false;

    protected override void AttaquerJoueur()
    {
        StartCoroutine(AttendrePuisV�rifierAttaqueM�l�e());
    }
    private IEnumerator AttendrePuisV�rifierAttaqueM�l�e()
    {
        yield return new WaitForSeconds(dur�eAttaqueNormale - Dur�eAttaque);
        StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 3, 20));
    }

    protected override void Update()
    {
        base.Update();
        if(�tatActuel == �tatEnnemi.Pr�parationAttaque || �tatActuel == �tatEnnemi.Attaque)
        {
            MouvementAttaque();
        }
        if (estEnRage)
        {
            if(tempsDerni�reAttaque + Dur�eAttaque < Time.time)
            {
                tempsDerni�reAttaque = Time.time;
                StartCoroutine(V�rifierSiAttaqueM�l�eTouche(D�GAT_ATTAQUE, 3, 20));
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
    protected override void V�rifierSiAttaque()
    {
        if (EstAttaquePr�te() && EstEnRange() && !estEnRage)
        {
            if (nombreAttaqueNormale >= NOMBRE_ATTAQUE_AVANT_RAGE)
            {
                estEnRage = true;
                tempsDerni�reAttaque = Time.time;
                �tatActuel = �tatEnnemi.Attaque;
                animator.SetTrigger("Attaque2");
            }
            else
            {
                AttaquerJoueur();
                tempsDerni�reAttaque = Time.time;
                �tatActuel = �tatEnnemi.Attaque;
                animator.SetTrigger("Attaque");
                ++nombreAttaqueNormale;
            }
        }
    }
}
