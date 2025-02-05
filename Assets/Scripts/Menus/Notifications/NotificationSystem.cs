using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using Vector3 = UnityEngine.Vector3;

public class NotificationSystem : MonoBehaviour
{
    public static NotificationSystem instance;

    Sprite[] ressources;
    Sprite[] outils;
    Sprite[] armes;
    Sprite[] soins;

    [SerializeField] GameObject[] notifications = new GameObject[(int)TypeNotification.Nb];

    [Header("Positions pour la notification objet")]
    [SerializeField] Transform PositionObj1;
    [SerializeField] Transform PositionObj2;
    [SerializeField] Transform PositionObj3;

    [Header("Positions pour la notification succès")]
    [SerializeField] Transform PositionSuccès1;
    [SerializeField] Transform PositionSuccès2;
    [SerializeField] Transform PositionSuccès3;

    public Sprite[] badges = new Sprite[(int)Rareté.NbRareté];
    public VerticalLayoutGroup liste;
    public GameObject cible;

    private float tempsDernierSuccès = -10;

    Canvas canvasNotification;

    private void Start()
    {
        //tempsDernierSuccès = Time.time;
        instance = this;

        ressources = ComportementInterface.instance.ressources;
        outils = ComportementInterface.instance.outils;
        armes = ComportementInterface.instance.armes;
        soins = ComportementInterface.instance.soins;

        canvasNotification = GetComponent<Canvas>();
        ComportementInterface.instance.OnMenuChangement += DésactivéCanvasSelonÉtatJeu;
    }

    void DésactivéCanvasSelonÉtatJeu(bool étatJeu)
    {
        canvasNotification.enabled = étatJeu;
    }

    // Notifications pour les objets
    public void LancerNotification(Ressource ressource, int compte)
    {
        GameObject notification = ObjectPool.instance.GetPoolObject(notifications[(int)TypeNotification.objet]);
        NotificationObjet notifObj = notification.GetComponent<NotificationObjet>();
        InitialiserNotificationObjet(notifObj);
        notifObj.LancerNotification(ressources[(int) ressource], $"{(compte > 0 ? " +" : "")} {compte} {InfoItem.ressources[(int)ressource]}");
    }
    public void LancerNotification(Outil outil, bool ajouter)
    {
        GameObject notification = ObjectPool.instance.GetPoolObject(notifications[(int)TypeNotification.objet]);
        NotificationObjet notifObj = notification.GetComponent<NotificationObjet>();
        InitialiserNotificationObjet(notifObj);
        notifObj.LancerNotification(outils[(int)outil], $"{(ajouter ? "+" : "-")} {InfoItem.outils[(int)outil]}");
    }
    public void LancerNotification(Arme arme)
    {
        GameObject notification = ObjectPool.instance.GetPoolObject(notifications[(int)TypeNotification.objet]);
        NotificationObjet notifObj = notification.GetComponent<NotificationObjet>();
        InitialiserNotificationObjet(notifObj);
        notifObj.LancerNotification(armes[(int)arme], $"{InfoItem.armes[(int)arme]}");
    }
    public void LancerNotification(Soin soin, int compte)
    {
        GameObject notification = ObjectPool.instance.GetPoolObject(notifications[(int)TypeNotification.objet]);
        NotificationObjet notifObj = notification.GetComponent<NotificationObjet>();
        InitialiserNotificationObjet(notifObj);
        notifObj.LancerNotification(soins[(int)soin], $"{(compte > 0 ? " +" : "")} {compte} {InfoItem.soins[(int)soin]}");
    }


    // Notifications pour les succès
    public void LancerNotification(Achievements succès) 
    {
      
        GameObject notification = ObjectPool.instance.GetPoolObject(notifications[(int)TypeNotification.succes]);
        NotificationSuccès notifSucces = notification.GetComponent<NotificationSuccès>();
        InitialiserNotificationSuccès(notifSucces);

        float duréeAnimation = notifSucces.infos.temps1 + notifSucces.infos.temps2 + notifSucces.infos.temps3;
        float tempsDepuisDernier = Time.time - tempsDernierSuccès;

        if (tempsDepuisDernier < duréeAnimation)
        {
            StartCoroutine(AttendrePourNotificationSuccès(notifSucces, succès, duréeAnimation - tempsDepuisDernier));
        }
        else
        {
            tempsDernierSuccès = Time.time;
            notifSucces.LancerNotification(GestionAchievements.GetNomAchievement(succès), GestionAchievements.GetRaretéAchievement(succès));
            
        }
    }

    IEnumerator AttendrePourNotificationSuccès(NotificationSuccès notifSucces, Achievements succès, float délais)
    {
        tempsDernierSuccès = Time.deltaTime + délais;
        yield return new WaitForSeconds(délais);
        notifSucces.LancerNotification(GestionAchievements.GetNomAchievement(succès), GestionAchievements.GetRaretéAchievement(succès));
    }

    // Initialiser les notifications
    private void InitialiserNotificationObjet(Notification notif)
    {
        InitialiserNotification(notif);
        notif.infos.position1 = PositionObj1;
        notif.infos.position2 = cible.transform;
        notif.infos.position3 = PositionObj3;

        notif.transform.position = PositionObj1.position;
        
    }

    private void InitialiserNotificationSuccès(Notification notif)
    {
        InitialiserNotification(notif);
        notif.infos.position1 = PositionSuccès1;
        notif.infos.position2 = PositionSuccès2;
        notif.infos.position3 = PositionSuccès3;

        notif.transform.position = PositionSuccès1.position;
    }

    private void InitialiserNotification(Notification notif)
    {
        notif.gameObject.SetActive(true);
        notif.transform.SetParent(transform);
    }
}

public enum TypeNotification { objet, succes, Nb}
