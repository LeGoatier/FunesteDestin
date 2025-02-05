using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScintillementLumière : MonoBehaviour
{
    private Light lumière;
    private float intensitéCourrante;
    private float intensitéMin = 4f;
    public float amplitude = 1.5f;
    float amortissement = 1f; 


    private void OnEnable()
    {
        lumière = GetComponent<Light>();
        intensitéCourrante = lumière.intensity;
    }

    void Update()
    {
        //Si le feu est allumé, la lumière est enabled et non nulle
        if (lumière != null)
        {
            float intensitéCible = lumière.intensity * ÉvaluerIntensité();

            //On change la valeur de l'intensité de la lumière en amortissant le changement pour pas que ce soit trop brusque
            intensitéCourrante = Mathf.Max(intensitéMin,Mathf.Lerp(intensitéCourrante, intensitéCible, amortissement * Time.deltaTime));
            lumière.intensity = intensitéCourrante;
        }
    }

    //Détermine l'intensité cible en utilisant une valeur aléatoire afin que ce soit réaliste
    float ÉvaluerIntensité()
    {
        float y = 1f - (Random.value * 2);
        return y * amplitude;
    }
}




