using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationObjet : Notification
{
    Image image;
    TextMeshProUGUI tmp;


    public override void Awake()
    {
        base.Awake();

        Image parent = GetComponent<Image>();
        Image[] images = GetComponentsInChildren<Image>();
        foreach (Image i in images)
        {
            if (i.gameObject.name == "Item")
                image = i;
        }
        tmp = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void LancerNotification(Sprite sprite, string message)
    {
        image.sprite = sprite;
        tmp.text = message;
        LancerAnimation(true);
    }
}
