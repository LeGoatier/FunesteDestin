using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CycleJours : MonoBehaviour
{
    [SerializeField] Light lumière; //Soleil
    [SerializeField] Light lune;

    //Désolé pour le bri d'encapsulation, c'était censé être des constantes
    public static float DURÉE_JOUR = 60; //1 minute
    public static float DURÉE_NUIT = 90;  //1min30 (3 spawns aux 30s)


    [SerializeField] TextMeshProUGUI jourTMP;

    public bool EstJour = true;
    int nbJours;

    Vector3 rotationLever = new Vector3(0, 0, 0);

    const float ANGLE_LATITUDE = 45;

    float tempsDébutNuit;
    float tempsEntreVagues;
    int vagueActuelle = 0;

    float tempsDernierSwitch;

    //skybox
    [SerializeField] Material tintJour;
    [SerializeField] Material tintNuit;
    [SerializeField] Material materialSample;
    [SerializeField] float vitesseRotation;

    [SerializeField] Material couleurBrumeJour;
    [SerializeField] Material couleurBrumeNuit;
    [SerializeField] float luminosteJour;
    [SerializeField] float luminositeNuit;
    [SerializeField] float brumeJour = 0.1f;
    [SerializeField] float BrumeNuit = 0.05f;

    float duréeMélange = 5;

    void Start()
    {
        //Le jeu commence au lever du soleil
        lumière.transform.rotation = Quaternion.Euler(rotationLever);
        //On met la lune à l'opposé du soleil (toujours une pleine lune lol)
        lune.transform.rotation = Quaternion.Euler(lumière.transform.rotation.eulerAngles + new Vector3(180, 0, 0));

        tempsDernierSwitch = Time.time;
        nbJours = 1;
        jourTMP.text = new string($" Jour {1}");

        tempsEntreVagues = DURÉE_NUIT / 3;

        RenderSettings.skybox.SetColor("_Tint", tintJour.color);
        RenderSettings.fogDensity = brumeJour;
        DynamicGI.UpdateEnvironment();
    }

    void Update()
    {
        RotaterLumière();
        if (!EstJour)
        {
            if (Time.time >= tempsDébutNuit + tempsEntreVagues * vagueActuelle)
            {
                ++vagueActuelle;
                EnnemySpawner.instance.SpawnVague(nbJours, vagueActuelle);
            }
        }
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * vitesseRotation);
    }

    private void RotaterLumière()
    {
        float durée = EstJour ? DURÉE_JOUR : DURÉE_NUIT;
        float tempsDepuisSwitch = Time.time - tempsDernierSwitch;
        float progrès = tempsDepuisSwitch / durée;

        float angleX = Mathf.Lerp(0, 180, progrès);
        float angleZ;
        if(progrès < 0.5f)
        {
            angleZ = Mathf.Lerp(0, ANGLE_LATITUDE, progrès * 2);
        }
        else
        {
            angleZ = Mathf.Lerp(ANGLE_LATITUDE, 0, (progrès - 0.5f) * 2);
        }
        Vector3 nouvelAngle = new Vector3(angleX, 0, angleZ);
        if (EstJour)
        {
            lumière.transform.rotation = Quaternion.Euler(nouvelAngle);
        }
        else
        {
            lune.transform.rotation = Quaternion.Euler(nouvelAngle);
        }

        if(progrès >= 1)
        {
            SwitchJourNuit(!EstJour);
        }
    }


    private void SwitchJourNuit(bool estJour)
    {
        tempsDernierSwitch = Time.time;
        if (estJour)
        {
            //Pour garder en memoire combien de jours ont passés
            nbJours++;
            GestionStatistiques.AugmenterJoursSurvécus();
            jourTMP.text = new string($" Jour {nbJours}");

            lumière.transform.rotation = Quaternion.Euler(rotationLever);

            EnnemySpawner.instance.TuerTousEnnemis();
            vagueActuelle = 0;
        }
        else
        {
            lune.transform.rotation = Quaternion.Euler(rotationLever);
            tempsDébutNuit = Time.time;
            Granny.instance.EstChangementNuit = true;
        }

        EstJour = estJour;
    
        lumière.gameObject.SetActive(estJour);
        lune.gameObject.SetActive(!estJour);

        StartCoroutine(ChangerSkybox());
        StartCoroutine(ChangerLuminosité());
        GestionBruit.instance.ChangerChansonJourEtNuit(estJour);
    }

    IEnumerator ChangerLuminosité()
    {
        Material matFin = EstJour ? couleurBrumeJour : couleurBrumeNuit;
        Material matDepart = EstJour ? couleurBrumeNuit : couleurBrumeJour;

        float brumeFin = EstJour ? brumeJour : BrumeNuit;
        float brumeDepart = EstJour ? BrumeNuit: brumeJour;

        float lumFin = EstJour ? luminosteJour : luminositeNuit;
        float lumDepart = EstJour ? luminositeNuit : luminosteJour;

        float timer = 0;
        while (timer < duréeMélange)
        {
            float valueLum = Mathf.Lerp(lumDepart, lumFin, timer / duréeMélange);
            float valueBrume = Mathf.Lerp(brumeDepart, brumeFin, timer / duréeMélange);
            materialSample.Lerp(matDepart, matFin, valueLum);

            RenderSettings.fogColor = materialSample.color;
            RenderSettings.fogDensity = valueBrume;
            RenderSettings.ambientIntensity = valueLum;

            DynamicGI.UpdateEnvironment();

            timer += Time.deltaTime;
            yield return null;
        }

        RenderSettings.ambientIntensity = lumFin;
        RenderSettings.fogColor = materialSample.color;
        RenderSettings.fogDensity = brumeFin;
        DynamicGI.UpdateEnvironment();
    }

    IEnumerator ChangerSkybox()
    {
        Material matFin = EstJour ? tintJour : tintNuit;
        Material matDepart = EstJour ? tintNuit : tintJour;

        float timer = 0;
        while (timer < duréeMélange)
        {
            float value = Mathf.Lerp(0, 1, timer / duréeMélange);
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
