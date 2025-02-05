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

    [Header("Positions pour la notification succ�s")]
    [SerializeField] Transform PositionSucc�s1;
    [SerializeField] Transform PositionSucc�s2;
    [SerializeField] Transform PositionSucc�s3;

    public Sprite[] badges = new Sprite[(int)Raret�.NbRaret�];
    public VerticalLayoutGroup liste;
    public GameObject cible;

    private float tempsDernierSucc�s = -10;

    Canvas canvasNotification;

    private void Start()
    {
        //tempsDernierSucc�s = Time.time;
        instance = this;

        ressources = ComportementInterface.instance.ressources;
        outils = ComportementInterface.instance.outils;
        armes = ComportementInterface.instance.armes;
        soins = ComportementInterface.instance.soins;

        canvasNotification = GetComponent<Canvas>();
        ComportementInterface.instance.OnMenuChangement += D�sactiv�CanvasSelon�tatJeu;
    }

    void D�sactiv�CanvasSelon�tatJeu(bool �tatJeu)
    {
        canvasNotification.enabled = �tatJeu;
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


    // Notifications pour les succ�s
    public void LancerNotification(Achievements succ�s) 
    {
      
        GameObject notification = ObjectPool.instance.GetPoolObject(notifications[(int)TypeNotification.succes]);
        NotificationSucc�s notifSucces = notification.GetComponent<NotificationSucc�s>();
        InitialiserNotificationSucc�s(notifSucces);

        float dur�eAnimation = notifSucces.infos.temps1 + notifSucces.infos.temps2 + notifSucces.infos.temps3;
        float tempsDepuisDernier = Time.time - tempsDernierSucc�s;

        if (tempsDepuisDernier < dur�eAnimation)
        {
            StartCoroutine(AttendrePourNotificationSucc�s(notifSucces, succ�s, dur�eAnimation - tempsDepuisDernier));
        }
        else
        {
            tempsDernierSucc�s = Time.time;
            notifSucces.LancerNotification(GestionAchievements.GetNomAchievement(succ�s), GestionAchievements.GetRaret�Achievement(succ�s));
            
        }
    }

    IEnumerator AttendrePourNotificationSucc�s(NotificationSucc�s notifSucces, Achievements succ�s, float d�lais)
    {
        tempsDernierSucc�s = Time.deltaTime + d�lais;
        yield return new WaitForSeconds(d�lais);
        notifSucces.LancerNotification(GestionAchievements.GetNomAchievement(succ�s), GestionAchievements.GetRaret�Achievement(succ�s));
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

    private void InitialiserNotificationSucc�s(Notification notif)
    {
        InitialiserNotification(notif);
        notif.infos.position1 = PositionSucc�s1;
        notif.infos.position2 = PositionSucc�s2;
        notif.infos.position3 = PositionSucc�s3;

        notif.transform.position = PositionSucc�s1.position;
    }

    private void InitialiserNotification(Notification notif)
    {
        notif.gameObject.SetActive(true);
        notif.transform.SetParent(transform);
    }
}

public enum TypeNotification { objet, succes, Nb}
