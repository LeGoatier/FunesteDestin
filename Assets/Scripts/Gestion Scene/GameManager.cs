using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{

    [SerializeField] Camera camJoueur;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public Vector3 GetPositionCam()
    {
        return camJoueur.transform.position;
    }

    private void OnApplicationQuit()
    {
        SauvegardeProfils.SauvegarderProfil();
        SauvegardeProfils.SauvegarderPartie();
    }

    private void Start()
    {
        GestionStatistiques.InitialiserTempsD�butPartie();
        StartCoroutine(AttendreEtLancerAchievement());
        //On pourrait essayer de r�activer la cam�ra ici
    }

    IEnumerator AttendreEtLancerAchievement()
    {
        yield return new WaitForSeconds(5);
        GestionAchievements.ActiverAchievement(Achievements.MaisOuSuisJe);
    }

}
