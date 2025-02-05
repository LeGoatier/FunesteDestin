using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class NotificationSuccès : Notification
{
    private TextMeshProUGUI nom;
    private TextMeshProUGUI rareté;
    private Image image;
    public override void Awake()
    {
        base.Awake();
        TextMeshProUGUI[] temp = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (TextMeshProUGUI tmp in temp)
        {
            if(tmp.gameObject.name == "Nom")
                nom = tmp;
            else if(tmp.gameObject.name == "Rareté")
                rareté = tmp;
        }
        Image[] temp2 = GetComponentsInChildren<Image>();
        foreach (Image i in temp2)
        {
            if (i.gameObject.name == "Badge")
                image = i;
        }
    }


    public void LancerNotification(string nom, Rareté rareté)
    {
        this.nom.text = nom;
        this.rareté.text = "Succès " + rareté.ToString();
        image.sprite = NotificationSystem.instance.badges[(int)rareté];

        GestionBruit.instance.JouerSon("NotificationSuccès");
        LancerAnimation(false);
    }
}
