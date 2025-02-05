using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GestionSkybox : MonoBehaviour
{
    [SerializeField] Material tintJour;
    [SerializeField] Material tintNuit;
    [SerializeField] Material materialSample;
    [SerializeField] float vitesseRotation;

    float duréeMélange = 5;


    public static GestionSkybox instance;
    void Start()
    {
        if(instance == null)
            instance = this;

        RenderSettings.skybox.SetColor("_Tint", tintJour.color);
        DynamicGI.UpdateEnvironment();
    }

    private void Update()
    {
        //RenderSettings.skybox.SetFloat("_Rotation", Time.time * vitesseRotation);
    }
    public void ActiverMélangeSkybox(bool estJour)
    {

        StartCoroutine(ChangerSkybox(estJour));

    }  
    
    IEnumerator ChangerSkybox(bool estJour)
    {
        Material matFin = estJour ? tintJour : tintNuit;
        Material matDepart = estJour ? tintNuit : tintJour;

        float timer = 0;
        while(timer < duréeMélange)
        {
            float value = Mathf.Lerp(0, 1, timer/duréeMélange);
            materialSample.Lerp(matDepart, matFin, value);

            RenderSettings.skybox.SetColor("_Tint", materialSample.color);
            DynamicGI.UpdateEnvironment();

            timer += Time.deltaTime;
            yield return null;
        }

        RenderSettings.skybox.SetColor("_Tint", matFin.color);
        DynamicGI.UpdateEnvironment();
    }
}
