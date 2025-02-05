using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFin : MonoBehaviour
{
    [SerializeField] Sprite[] imagesBateau;
    [SerializeField] Sprite[] imagesFusee;
    [SerializeField] Sprite[] imagesAntenne;

    [SerializeField]Image imageActuelle;

    void Start()
    {
    }


    IEnumerator ChangerAlphaImageGraduellement(float duree, Image image)
    {
        float timer = 0;
        Color couleur = image.color;
        while (timer < duree)
        {
            couleur.a =  Mathf.Lerp(0, 1, timer / duree);

            image.color = couleur;

            timer += Time.deltaTime;
            yield return null;
        }
        couleur.a =  1;
        image.color = couleur;
    }

}
