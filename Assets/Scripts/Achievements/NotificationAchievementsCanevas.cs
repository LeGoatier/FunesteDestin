using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotificationAchievementsCanevas : MonoBehaviour
{
    //C'est hyper important de mettre les achievements dans le m�me ordre que l'enum achievements
    [SerializeField] GameObject[] notificationsPrefab = new GameObject[(int)Achievements.NbAchievements];

    private GameObject[] notificationsObjet = new GameObject[(int)Achievements.NbAchievements];

    public static NotificationAchievementsCanevas instance;

    [SerializeField] float dur�eNotification = 4;

    //Cette fonction sera appel�e par le canevas R�sum�Partie de la sc�ne de fin pour avoir les m�mes
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
        //Pour l'instant, je ne fais qu'activer et d�sactiver la notification,
        //�ventuellement, il serait int�ressant de faire un fade in avec un mouvement vers le haut et un fade out
        notificationObjet.SetActive(true);
        StartCoroutine(D�sactiverNotification(notificationObjet));
    }

    private IEnumerator D�sactiverNotification(GameObject notificationObjet)
    {
        yield return new WaitForSeconds(dur�eNotification);
        notificationObjet.SetActive(false);
    }
}
