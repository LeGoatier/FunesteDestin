using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class AnimationArbalète : AnimationGun
{
   
    private void Awake()
    {
        

        PositionRepos = new Vector3(0.25f, -0.15f, 0.3f);
        PositionAim = new Vector3(0, -0.1f, 0.32f);
        VitesseMouvementVisé = 4;
        DuréeRecul = 0;
        AngleRecul = 0;

        ChampsDeVisionVisée = 50;

        Initialiser();
    }

    //protected override void Viser()
    //{
    //    if (Input.GetMouseButton(1) && !EstEnRecharge)//vise si le joueur clic droit
    //    {
    //        Zoomer(-1);
            
    //    }
    //    else//sinon reviens à sa position initiale
    //    {
    //        Zoomer(1);
    //        transform.localPosition = Vector3.Lerp(transform.localPosition, PositionRepos, VitesseMouvementVisé * Time.deltaTime);
           
    //    }
        
    //}
}
