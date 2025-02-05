using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AnimationSniper : AnimationGun
{
    
    [SerializeField] Canvas GénériqueUI;
    [SerializeField] Canvas VisueurUI;
    [SerializeField] GameObject meshSniper;

    float Timer;
    bool EstVisé = false;
    bool EstReset = true;

    private void Awake()
    {
        ChampsDeVisionVisée = 15;
        VariationRecul = 0.2f;
        PositionRepos = new Vector3(0.3f, -0.28f, 0.38f);
        PositionAim = new Vector3(0, -0.19f, 0.38f);
        VitesseMouvementVisé = 8;
        DuréeRecul = 0.5f;
        AngleRecul = 13;
        VitesseVariationChampsDeVision = 100;
        GénériqueUI.enabled = true;
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
            GénériqueUI.enabled = false;
            if (!EstVisé)
            {
                Timer += VitesseMouvementVisé * Time.deltaTime;
            }
        }
        else//sinon reviens à sa position initiale
        {
            MouvementViser(PositionRepos);
            Zoomer(1);
            if(!EstReset)
            {
                meshSniper.SetActive(true);
                GénériqueUI.enabled = true;
                VisueurUI.enabled = false;
                EstVisé = false;
                Timer = 0;
                EstReset = true;
            }
            

        }
    }

    //Translate le fusil à sa position de visé ou ;a sa position originale
    void MouvementViser(Vector3 positionFinale)
    {
        
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionFinale, VitesseMouvementVisé * Time.deltaTime);
        

        if(Timer > 2.3f && !EstVisé)
        {
            VisueurUI.enabled = true;
            meshSniper.SetActive(false);
            EstVisé = true;
            Timer = 0;
        }

    }

    
    
}
