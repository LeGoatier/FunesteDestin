using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dézoomer : MonoBehaviour
{
    Camera CameraPersonnage;
    float ChampsDeVisionParDéfault = 60;
    float VitesseVariationChampsDeVision = 50;

    bool EstDézoomer = false;
    

    private void Start()
    {
        CameraPersonnage = GetComponentInParent<Camera>();
    }

    private void Update()
    {
        if (!EstDézoomer)
        {
            Zoomer();
        }
        
    }

    void Zoomer()
    {
        CameraPersonnage.fieldOfView += VitesseVariationChampsDeVision * Time.deltaTime;
        if (CameraPersonnage.fieldOfView > ChampsDeVisionParDéfault)
        {
            EstDézoomer = true;
            CameraPersonnage.fieldOfView = 60;
        }

    }

    private void OnDisable()
    {
        EstDézoomer = false;
    }
}
