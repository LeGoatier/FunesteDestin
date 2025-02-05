using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRiffle : AnimationGun
{
    private void Awake()
    {
        ChampsDeVisionVisée = 40;
        VariationRecul = 0.08f;
        PositionRepos = new Vector3(0.3f, -0.43f, 0.37f);
        PositionAim = new Vector3(0, -0.32f, 0.2f);
        VitesseMouvementVisé = 6;
        DuréeRecul = 0.2f;
        AngleRecul = 2;

        Initialiser();
    }
}
