using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportementArbalète : ComportementFusil
{

    

    private void Start()
    {
        DélaisEntreTir = 0.8f;
        DélaisRecharge = 1.5f;
        NombreBallesTotalesChargeur = 1;

        Initialiser();

    }
    protected override bool ImputTirer()
    {
        return Input.GetMouseButtonDown(0);
    }
    protected override void InitiationRotationBalle(GameObject instance)
    {
        instance.transform.rotation = transform.rotation;
    }

    


    protected override void EffetsSonoresEtVisuelTirer()
    {
        GestionBruit.instance.JouerSon("TirerArbalète");
    }
}
