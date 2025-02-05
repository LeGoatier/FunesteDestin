using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPompe : AnimationGun
{
    

    private void Awake()
    {
        ChampsDeVisionVisée = 40;
        VariationRecul = 0.15f;
        PositionRepos = new Vector3(0.3f, -0.3f, 0.29f);
        PositionAim = new Vector3(0, -0.2f, 0.32f);
        VitesseMouvementVisé = 6;
        DuréeRecul = 0.6f;
        AngleRecul = 8;

        Initialiser();
    }
}
