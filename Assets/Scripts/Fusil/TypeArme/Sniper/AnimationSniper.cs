using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AnimationSniper : AnimationGun
{
    
    [SerializeField] Canvas G�n�riqueUI;
    [SerializeField] Canvas VisueurUI;
    [SerializeField] GameObject meshSniper;

    float Timer;
    bool EstVis� = false;
    bool EstReset = true;

    private void Awake()
    {
        ChampsDeVisionVis�e = 15;
        VariationRecul = 0.2f;
        PositionRepos = new Vector3(0.3f, -0.28f, 0.38f);
        PositionAim = new Vector3(0, -0.19f, 0.38f);
        VitesseMouvementVis� = 8;
        Dur�eRecul = 0.5f;
        AngleRecul = 13;
        VitesseVariationChampsDeVision = 100;
        G�n�riqueUI.enabled = true;
        VisueurUI.enabled = false;
        meshSniper.SetActive(true);

        Initialiser();
    }

    protected override void Viser()
    {
        if (Input.GetMouseButton(1) && !EstEnRecharge)//vise si le joueur clic droit
        {
            MouvementViser(PositionAim);
            Zoomer(-1);

            EstReset = false;
            G�n�riqueUI.enabled = false;
            if (!EstVis�)
            {
                Timer += VitesseMouvementVis� * Time.deltaTime;
            }
        }
        else//sinon reviens � sa position initiale
        {
            MouvementViser(PositionRepos);
            Zoomer(1);
            if(!EstReset)
            {
                meshSniper.SetActive(true);
                G�n�riqueUI.enabled = true;
                VisueurUI.enabled = false;
                EstVis� = false;
                Timer = 0;
                EstReset = true;
            }
            

        }
    }

    //Translate le fusil � sa position de vis� ou ;a sa position originale
    void MouvementViser(Vector3 positionFinale)
    {
        
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionFinale, VitesseMouvementVis� * Time.deltaTime);
        

        if(Timer > 2.3f && !EstVis�)
        {
            VisueurUI.enabled = true;
            meshSniper.SetActive(false);
            EstVis� = true;
            Timer = 0;
        }

    }

    
    
}
