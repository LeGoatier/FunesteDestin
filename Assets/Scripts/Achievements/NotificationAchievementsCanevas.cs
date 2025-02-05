using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationAchievementsCanevas : MonoBehaviour
{
    //C'est hyper important de mettre les achievements dans le même ordre que l'enum achievements
    [SerializeField] GameObject[] notificationsPrefab = new GameObject[(int)Achievements.NbAchievements];

    private GameObject[] notificationsObjet = new GameObject[(int)Achievements.NbAchievements];

    public static NotificationAchievementsCanevas instance;

    [SerializeField] float duréeNotification = 4;

    //Cette fonction sera appelée par le canevas RésuméPartie de la scène de fin pour avoir les mêmes
    //objets de notification que les notifications en jeu
    public GameObject ObtenirPrefabNotification(Achievements achievement)
    {
        return notificationsPrefab[(int)achievement];
    }


    


    private void Start()
    {
        if (instance == null)
            instance = this;
        InitialiserNotifications();
    }

    private void InitialiserNotifications()
    {
        for(int i = 0; i < (int)Achievements.NbAchievements; i++)
        {
            notificationsObjet[i] = Instantiate(notificationsPrefab[i]);
            notificationsObjet[i].SetActive(false);
        }
    }


    public void AfficherNotification(Achievements achievement)
    {
        AnimerNotification(notificationsObjet[(int)achievement]);
    }

    private void AnimerNotification(GameObject notificationObjet)
    {
        //Pour l'instant, je ne fais qu'activer et désactiver la notification,
        //éventuellement, il serait intéressant de faire un fade in avec un mouvement vers le haut et un fade out
        notificationObjet.SetActive(true);
        StartCoroutine(DésactiverNotification(notificationObjet));
    }

    private IEnumerator DésactiverNotification(GameObject notificationObjet)
    {
        yield return new WaitForSeconds(duréeNotification);
        notificationObjet.SetActive(false);
    }
}
