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
    //La propri�t� servait � rien finalement le but c'�tait que le set g�re lui m�me le changement de
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
        
        R�initialiserVie();

        OnMort += G�rerMort;
    }
    private void Update()
    {
       UpdateBarre(); //Yoooo, c'est tu vraiment n�cessaire messemble �a bouffe du CPU pour rien? (Justin - 16 mars) (c'est Sofia qui a �crit �a)
        //Ok finalement j'ai compris pourquoi on a pas le choix de v�rifier dans le update,
        //cependant c'est pas g�nial comme code, on serait beaucoup mieux d'utiliser des coroutines
        //mais c'est pas trop grave c'est juste une v�rification d'un bool dans bien des cas

        if(pv <= pvCritique)
            AfficherVieCritique();
    }
    float tempsDernierLancementAnimation = float.MinValue;
    private void AfficherVieCritique()
    {
        if (tempsDernierLancementAnimation + afficheDegat.dur�eAnimation < Time.time)
        {
            tempsDernierLancementAnimation = Time.time;
            AfficherD�gat();
        }
    }

    public void RecevoirVie(float vie)
    {
        pv = Mathf.Min(pv + vie, PV_INITIAL);
    }

    public void RecevoirD�gat(float d�gat)
    {
        AfficherD�gat();
        if (pv - d�gat > 0)
        {
            pv -= d�gat;
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

    public void AfficherD�gat()
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

    
    void G�rerMort(object sender, EventArgs e)
    {
        Debug.Log("Le Joueur est mort");

        if (PeutMourir)
        {
            StartCoroutine(GestionBruit.instance.JouerSonFin("D�faiteMusique"));
            ComportementInterface.instance.Activ��tatMenu();
            GameObject camPerso = GetComponentInChildren<Camera>().gameObject;
            GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(AnimationMort(2f, camPerso));
        }

    }

    IEnumerator AnimationMort(float dur�e, GameObject camera)
    {
        float timer = 0;
        Quaternion rotationInitiale = camera.transform.rotation;
        Quaternion rotationfinale = Quaternion.LookRotation(Vector3.down); 

        while (timer < dur�e)
        {
            transform.rotation = Quaternion.Slerp(rotationInitiale, rotationfinale, timer/dur�e);
            transform.position-= new Vector3(0, 0.5f*Time.deltaTime, 0);
            timer+=Time.deltaTime;
            yield return null;
        }
        GestionScenes.ChargerFin(�tatFinPartie.Mort);
    }

    //Pourrait �tre utile si il y a un medKit qui heal au complet �ventuellement
    //est pr�sentement utilis� par niveau ennemis spawner, un script pour tester
    public void R�initialiserVie()
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