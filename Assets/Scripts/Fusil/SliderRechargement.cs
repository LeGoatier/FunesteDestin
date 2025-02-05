using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderRechargement : MonoBehaviour
{
    [SerializeField] Slider slider;

    public static SliderRechargement instance;
    Color CouleurInitiale;
    Image ImageFill;

    private void Start()
    {
        if(instance == null)
            instance = this;

        ImageFill = slider.transform.Find("Fill Area").GetComponentInChildren<Image>();
        CouleurInitiale = ImageFill.color;
    }

    public void LancerAnimation(float durée)
    {
        slider.gameObject.SetActive(true);
        StartCoroutine(ModifierValeurSlider(durée));
    }

    public void LancerAnimation(float durée, Color couleurSlider)
    {
        ImageFill.color = couleurSlider;
        slider.gameObject.SetActive(true);
        StartCoroutine(ModifierValeurSlider(durée));
    }

    IEnumerator ModifierValeurSlider(float durée)
    {
        float timer = 0;
        while (timer < durée)
        {
            slider.value = Mathf.Lerp(0, 1, timer / durée);
            timer += Time.deltaTime;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        ImageFill.color = CouleurInitiale;
    }
}
