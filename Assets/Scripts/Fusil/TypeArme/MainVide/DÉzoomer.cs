using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D�zoomer : MonoBehaviour
{
    Camera CameraPersonnage;
    float ChampsDeVisionParD�fault = 60;
    float VitesseVariationChampsDeVision = 50;

    bool EstD�zoomer = false;
    

    private void Start()
    {
        CameraPersonnage = GetComponentInParent<Camera>();
    }

    private void Update()
    {
        if (!EstD�zoomer)
        {
            Zoomer();
        }
        
    }

    void Zoomer()
    {
        CameraPersonnage.fieldOfView += VitesseVariationChampsDeVision * Time.deltaTime;
        if (CameraPersonnage.fieldOfView > ChampsDeVisionParD�fault)
        {
            EstD�zoomer = true;
            CameraPersonnage.fieldOfView = 60;
        }

    }

    private void OnDisable()
    {
        EstD�zoomer = false;
    }
}
