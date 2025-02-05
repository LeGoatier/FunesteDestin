using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestionVieJoueur : MonoBehaviour
{
    [SerializeField] Slider BarreVie;
    [SerializeField] AfficheDegat afficheDegat;
    [SerializeField] bool PeutMourir; //mettez le faux pour le test, vrai dans le jeu final 
    public EventHandler OnMort;

    public const float PV_INITIAL = 100;
    bool EstMort;
    Image fill;

    [Range (0,2)]
    [SerializeField] float delaisDiminution;
    float pvBarre;
    float vitesseDiminution;

    public CouleurVie[] couleurs;

    public float pv;
    //La propriété servait à rien finalement le but c'était que le set gère lui même le changement de
    //valeur de la barre de vie mais finalement il faut le checker dans le uptdate parce que c'est une
    //variation progressive
    //public float PV
    //{
    //    get { return pv; }
    //    private set
    //    {
    //        if (pv >= 0)
    //        {
    //            pv = value;
    //        }
    //        else
    //        {
    //            pv = 0;
    //            OnMort(this, EventArgs.Empty);
    //        }
    //    }
    //}
    const float pvCritique = 20;

    private void Awake()
    {
        fill = GameObject.Find("Fill_VieJoueur").GetComponent<Image>();
        BarreVie.maxValue = PV_INITIAL;
        
        RéinitialiserVie();

        OnMort += GérerMort;
    }
    private void Update()
    {
       UpdateBarre(); //Yoooo, c'est tu vraiment nécessaire messemble ça bouffe du CPU pour rien? (Justin - 16 mars) (c'est Sofia qui a écrit ça)
        //Ok finalement j'ai compris pourquoi on a pas le choix de vérifier dans le update,
        //cependant c'est pas génial comme code, on serait beaucoup mieux d'utiliser des coroutines
        //mais c'est pas trop grave c'est juste une vérification d'un bool dans bien des cas

        if(pv <= pvCritique)
            AfficherVieCritique();
    }
    float tempsDernierLancementAnimation = float.MinValue;
    private void AfficherVieCritique()
    {
        if (tempsDernierLancementAnimation + afficheDegat.duréeAnimation < Time.time)
        {
            tempsDernierLancementAnimation = Time.time;
            AfficherDégat();
        }
    }

    public void RecevoirVie(float vie)
    {
        pv = Mathf.Min(pv + vie, PV_INITIAL);
    }

    public void RecevoirDégat(float dégat)
    {
        AfficherDégat();
        if (pv - dégat > 0)
        {
            pv -= dégat;
            GestionBruit.instance.JouerSon("RecevoirDegat");
        }
        else if(!EstMort)
        {
            EstMort = true; 
            pv = 0;
            BarreVie.value = pv;
            fill.color = couleurs[0].couleur;
            OnMort?.Invoke(this, EventArgs.Empty);
        }
    }

    public void AfficherDégat()
    {
        afficheDegat.LancerAnimationAffichage();
    }

    public void UpdateBarre()
    {
        if (pvBarre != pv)
        {
            foreach (CouleurVie c in couleurs)
            {
                if (c.valeur <= pvBarre / PV_INITIAL) fill.color = c.couleur;
                else break;
            }
            
            if (pvBarre >= pv)
            {
                vitesseDiminution = (pvBarre - pv) / delaisDiminution;
                pvBarre -= vitesseDiminution * Time.deltaTime;
            }
            else
            {
                vitesseDiminution = (pv - pvBarre) / delaisDiminution;
                pvBarre += vitesseDiminution * Time.deltaTime;
            }
          
            BarreVie.value = pvBarre;
        }      
    }

    
    void GérerMort(object sender, EventArgs e)
    {
        Debug.Log("Le Joueur est mort");

        if (PeutMourir)
        {
            StartCoroutine(GestionBruit.instance.JouerSonFin("DéfaiteMusique"));
            ComportementInterface.instance.ActivéÉtatMenu();
            GameObject camPerso = GetComponentInChildren<Camera>().gameObject;
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(AnimationMort(2f, camPerso));
        }

    }

    IEnumerator AnimationMort(float durée, GameObject camera)
    {
        float timer = 0;
        Quaternion rotationInitiale = camera.transform.rotation;
        Quaternion rotationfinale = Quaternion.LookRotation(Vector3.down); 

        while (timer < durée)
        {
            transform.rotation = Quaternion.Slerp(rotationInitiale, rotationfinale, timer/durée);
            transform.position-= new Vector3(0, 0.5f*Time.deltaTime, 0);
            timer+=Time.deltaTime;
            yield return null;
        }
        GestionScenes.ChargerFin(ÉtatFinPartie.Mort);
    }

    //Pourrait être utile si il y a un medKit qui heal au complet éventuellement
    //est présentement utilisé par niveau ennemis spawner, un script pour tester
    public void RéinitialiserVie()
    {
        BarreVie.value = PV_INITIAL;
        pv = PV_INITIAL;
        fill.color = couleurs[couleurs.Length - 1].couleur;
        pvBarre = PV_INITIAL;
    }

}

[Serializable]
public struct CouleurVie
{
    public Color couleur;
    [Range (0,1)]
    public float valeur;
}