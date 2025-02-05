using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notification : MonoBehaviour
{
    public InfoNotif infos;
    private RectTransform rectTransform;

    bool enAnimation = false;
    private bool obj;
    float t = 0;

    bool fait1 = false;
    bool fait2 = false;
    public virtual void Awake()
    {
        rectTransform = GetComponent<RectTransform>();  
    }

    private void Update()
    {
        if(enAnimation) AnimerNotification();

    }
    public void LancerAnimation(bool obj)
    {
        this.obj = obj;
        enAnimation = true;
    }
    private void AnimerNotification()
    {
        if (t < infos.temps1)
        {
            rectTransform.position = Vector3.Lerp(infos.position1.position, infos.position2.position, t / infos.temps1);
            t += Time.deltaTime;
        }
        else if (t < infos.temps2 + infos.temps1)
        {
            if (!fait1 & obj) SetParentListe();
            t += Time.deltaTime;
        }
        else if (t < infos.temps3 + infos.temps2 + infos.temps1)
        {
            if (!fait2 & obj) SetParentSystem();
            rectTransform.position = Vector3.Lerp(infos.position2.position, infos.position3.position, (t - infos.temps1 - infos.temps2) / infos.temps3);
            t += Time.deltaTime;

        }
        else 
        {
            t = 0;
            fait1 = false;
            fait2 = false;
            enAnimation = false;
            gameObject.SetActive(false);
        }
       
    }

    private void SetParentListe()
    {
        transform.SetParent(NotificationSystem.instance.liste.transform);
        // transform.SetSiblingIndex(NotificationSystem.instance.liste.transform.childCount - 1);
        NotificationSystem.instance.cible.transform.SetAsLastSibling();
        fait1 = true;
    }

    private void SetParentSystem()
    {
        transform.SetParent(NotificationSystem.instance.transform);
        transform.position = infos.position2.position;
        fait2 = true;
    }

}

[Serializable]
public class InfoNotif
{
    public Transform position1;
    public Transform position2;
    public Transform position3;
    public float temps1;
    public float temps2;
    public float temps3;
}