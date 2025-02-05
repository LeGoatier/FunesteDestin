using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPistolet : AnimationGun
{
    

    private void Awake()
    {
        ChampsDeVisionVisée = 40;
        PositionRepos = new Vector3(0.31f, -0.28f, 0.42f);
        PositionAim = new Vector3(0, -0.23f, 0.33f);
        VitesseMouvementVisé = 6;
        DuréeRecul = 0.2f;
        AngleRecul = 3;
        VariationRecul = 0.15f;

        Initialiser();
    }
}
