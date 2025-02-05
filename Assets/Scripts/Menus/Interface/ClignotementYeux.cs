using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClignotementYeux : MonoBehaviour
{
    [Range(0.1f, 5f)]
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
        InitialiserImage();
        LancerAnimationAffichage();
    }

    float albedo1 = 1;
    float albedo2 = 0;//0.5f
    float dur�e = 2;


    private void LancerAnimationAffichage()
    {
        StartCoroutine(AttendreEtPremierBattement());
    }

    private IEnumerator AttendreEtPremierBattement()
    {
        yield return new WaitForSeconds(1.3f);
        StartCoroutine(FadeImage(albedo1, albedo2, dur�e));
        //StartCoroutine(AttendreEtDeuxi�meBattement());
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
        int �cranLargeur = Camera.main.pixelWidth;
        int �cranHauteur = Camera.main.pixelHeight;

        int largeur = 250;
        int hauteur = largeur * �cranHauteur / �cranLargeur;

        Texture2D texture = G�n�rerTexture(largeur, hauteur, largeur / 2, hauteur / 2);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        
        image.sprite = sprite;
        image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
    }


    private Texture2D G�n�rerTexture(int largeur, int hauteur, int centreX, int centreY)
    {
        Texture2D texture = new Texture2D(largeur, hauteur);
        Color[] tab = new Color[hauteur * largeur];

        //float distanceMax = CalculerDistanceCentre(centreX, centreY, 0, 0);

        for (int y = 0; y < hauteur; y++)
        {
            for (int x = 0; x < largeur; x++)
            {
                // On change la couleur des pixels juste par rapport � leur distance du centre
                Color couleurPixel = new Color(0, 0, 0, 1);
                tab[y * largeur + x] = couleurPixel;
            }
        }

        texture.SetPixels(tab);
        texture.Apply();

        return texture;
    }

    private float CalculerAlbedo(int centreX, int centreY, int i, int j, float distanceMax)
    {
        float proximit�CentreRatio = CalculerDistanceCentre(centreX, centreY, i, j) / distanceMax;
        return proximit�CentreRatio * 50;
    }

    private float CalculerDistanceCentre(int centreX, int centreY, int i, int j)
    {
        return Mathf.Sqrt(Mathf.Pow(centreX - i, 2) + Mathf.Pow(centreY - j, 2));
    }

    //[SerializeField] float premiereOuverture;
    //[SerializeField] float premiereFermeture;
    //[SerializeField] float dur�ePremiereOuverture;
    //[SerializeField] float dur�eDeuxiemeOuverture;

    ////public Volume volume;

    //PostProcessVolume volume;
    //Vignette vignette;


    //private void Start()
    //{
    //    vignette = ScriptableObject.CreateInstance<Vignette>();
    //    vignette.enabled.Override(true);
    //    vignette.intensity.Override(1);
    //    vignette.center.Override(new Vector2(1.5f, 1.5f));

    //    volume = PostProcessManager.instance.QuickVolume(gameObject.layer, 1, vignette);
    //    StartCoroutine(ClignerYeux());
    //}

    //public IEnumerator ClignerYeux()
    //{
    //    yield return new WaitForSeconds(0.2f);
    //    vignette.center.value = (new Vector2(0.5f, 0.5f));
    //    yield return new WaitForSeconds(0.1f);
    //    vignette.rounded.value = (false);

    //    //ouvre yeux mais pas completement
    //    StartCoroutine(OuvrirYeux(vignette.intensity.value, premiereOuverture, dur�ePremiereOuverture));

    //    //referme un peu les yeux
    //    StartCoroutine(OuvrirYeux(vignette.intensity.value, premiereFermeture, dur�eDeuxiemeOuverture));

    //    //ouvre les yeux completement
    //    StartCoroutine(OuvrirYeux(vignette.intensity.value, 0, 0.5f));

    //    RuntimeUtilities.DestroyVolume(volume, true, true);
    //    Destroy(this);
    //}

    //IEnumerator OuvrirYeux(float ouvertureInitiale, float ouvertureFinale, float dur�e)
    //{
    //    float timer = 0;
    //    while (timer < dur�e)
    //    {
    //        vignette.intensity.value = (Mathf.Lerp(ouvertureInitiale, ouvertureFinale, timer / dur�e));
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //    vignette.intensity.value = (ouvertureFinale);
    //}
}
