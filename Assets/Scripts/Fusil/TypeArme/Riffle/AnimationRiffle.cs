using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationRiffle : AnimationGun
{
    private void Awake()
    {
        ChampsDeVisionVis�e = 40;
        VariationRecul = 0.08f;
        PositionRepos = new Vector3(0.3f, -0.43f, 0.37f);
        PositionAim = new Vector3(0, -0.32f, 0.2f);
        VitesseMouvementVis� = 6;
        Dur�eRecul = 0.2f;
        AngleRecul = 2;

        Initialiser();
    }
}
