using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestionDash : MonoBehaviour
{
    [SerializeField] List<Slider> listeSlider;
    [SerializeField] List<Image> imageListe;

    public static GestionDash instance;
    //Bon pour l'instant j'assume qu'il y aura toujours juste 2 dash si on fait des maniËres pour augmenter
    //le nombre de dash il faudra modifier certaines affaires
    const float …NERGIE_MAX = 2;

    float Ènergie = …NERGIE_MAX; //«a prend 1 d'Ènergie pour faire un dash, mais les valeurs sont continues

    const float COOLDOWN_DASH_INITIAL = 5;

    float regen…nergie = 1/COOLDOWN_DASH_INITIAL;

    bool sliderActifs = false;
    const float D…LAI_FADE_OUT = 1;

    const float DUR…E_FADE_OUT = 1;

    private void Start()
    {
        if (instance == null)
            instance = this;
    }

    public bool EstDashPossible()
    {
        return Ènergie >= 1;
    }

    public void Retirer…nergie()
    {
        Ènergie -= 1;
        ChangerActivation(true);
        listeSlider[1].value = 0; 
        listeSlider[0].value = Ènergie;
    }

    void Update()
    {
        Regenerer…nergie();
    }

    private void Regenerer…nergie()
    {
        if (Est…nergiePleine() && sliderActifs)
        {
            sliderActifs = false;
            StartCoroutine(AttendreEtFadeOut());
        }
        else
        {
            if (Ènergie + regen…nergie * Time.deltaTime >= …NERGIE_MAX)
            {
                Ènergie = …NERGIE_MAX;
            }
            else
            {
                Ènergie += regen…nergie * Time.deltaTime;
            }
            ModifierValueSliders();
        }
    }

    private IEnumerator AttendreEtFadeOut()
    {
        yield return new WaitForSeconds(D…LAI_FADE_OUT);
        if(!sliderActifs)
            StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Color currentColor = imageListe[0].color;
        float timer = 0f;
        while (timer < DUR…E_FADE_OUT)
        {
            if (sliderActifs)
            {
                timer = DUR…E_FADE_OUT;
                break;
            }
            float alpha = Mathf.Lerp(1, 0, timer / DUR…E_FADE_OUT);

            foreach(Image image in imageListe)
            {
                image.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            }
            timer += Time.deltaTime;
            yield return null;
        }
        //Une fois que le fadeOut est terminÈ je rÈtablis l'albedo puis dÈsactive
        foreach (Image image in imageListe)
        {
            image.color = currentColor;
        }
        if (!sliderActifs)
            ChangerActivation(false);
    }

    private void ChangerActivation(bool activer)
    {
        sliderActifs = activer;
        foreach(Slider slider in listeSlider)
        {
            slider.gameObject.SetActive(activer);
        }
    }

    private bool Est…nergiePleine()
    {
        return Ènergie == …NERGIE_MAX;
    }

    private void ModifierValueSliders()
    {
        if (Ènergie <= 1)
        {
            listeSlider[0].value = Ènergie;
        }
        else
        {
            listeSlider[1].value = Ènergie - 1;
        }
    }
}
