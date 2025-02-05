using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Comme je ne connaissais pas beaucoup comment fonctionnait la création d'images à partir du code,
//certaines parties sont inspirées de code trouvé sur internet, bien que tout a été écrit
//par moi au final (Justin Gauthier) sur les bases de Thomas Bolduc
public class AfficheDegat : MonoBehaviour
{
    [Range(0.1f, 5f)]
    [SerializeField] float décalage;
    Image image;
    [SerializeField] public float duréeAnimation = 1;

    void Start()
    {
        InitialiserImage();
    }

    public void LancerAnimationAffichage()
    {
        FadeIn(duréeAnimation / 2);
        StartCoroutine(AttendreEtFadeOut(duréeAnimation / 2));

    }

    private IEnumerator AttendreEtFadeOut(float durée)
    {
        yield return new WaitForSeconds(durée);
        FadeOut(durée);
    }

    private void FadeIn(float durée)
    {
        StartCoroutine(FadeImage(0, 1, durée));

    }

    private void FadeOut(float durée)
    {
        StartCoroutine(FadeImage(1, 0, durée));

    }

    IEnumerator FadeImage(float startAlpha, float targetAlpha, float duration)
    {
        Color currentColor = image.color;
        float timer = 0f;

        while (timer < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        image.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }



    private void InitialiserImage()
    {
        int écranLargeur = Camera.main.pixelWidth;
        int écranHauteur = Camera.main.pixelHeight;

        int largeur = 250;
        int hauteur = largeur * écranHauteur / écranLargeur;

        image = GetComponent<Image>();
        Texture2D texture = GénérerTexture(largeur, hauteur, largeur / 2, hauteur / 2);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
    }


    private Texture2D GénérerTexture(int largeur, int hauteur, int centreX, int centreY)
    {
        Texture2D texture = new Texture2D(largeur, hauteur);
        Color[] tab = new Color[hauteur * largeur];

        float distanceMax = CalculerDistanceCentre(centreX, centreY, 0, 0);

        for (int y = 0; y < hauteur; y++)
        {
            for (int x = 0; x < largeur; x++)
            {
                // On change la couleur des pixels juste par rapport à leur distance du centre
                Color couleurPixel = new Color(1, 0, 0, CalculerAlbedo(centreX, centreY, x, y, distanceMax));
                tab[y * largeur + x] = couleurPixel;
            }
        }
        
        
        texture.SetPixels(tab);
        texture.Apply();

        return texture;
    }

    private float CalculerAlbedo(int centreX, int centreY, int i, int j, float distanceMax)
    {
        float proximitéCentreRatio = CalculerDistanceCentre(centreX, centreY, i, j) / distanceMax;
        //return proximitéCentreRatio;
        return (Mathf.Pow(proximitéCentreRatio, 3) / (Mathf.Pow(proximitéCentreRatio, 3) + Mathf.Pow(décalage - décalage * proximitéCentreRatio, 3)));
    }

    private float CalculerDistanceCentre(int centreX, int centreY, int i, int j)
    {
        return Mathf.Sqrt(Mathf.Pow(centreX - i, 2) + Mathf.Pow(centreY - j, 2));
    }
}

