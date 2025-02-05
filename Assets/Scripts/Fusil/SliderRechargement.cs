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

    public void LancerAnimation(float dur�e)
    {
        slider.gameObject.SetActive(true);
        StartCoroutine(ModifierValeurSlider(dur�e));
    }

    public void LancerAnimation(float dur�e, Color couleurSlider)
    {
        ImageFill.color = couleurSlider;
        slider.gameObject.SetActive(true);
        StartCoroutine(ModifierValeurSlider(dur�e));
    }

    IEnumerator ModifierValeurSlider(float dur�e)
    {
        float timer = 0;
        while (timer < dur�e)
        {
            slider.value = Mathf.Lerp(0, 1, timer / dur�e);
            timer += Time.deltaTime;
            yield return null;
        }
        slider.gameObject.SetActive(false);
        ImageFill.color = CouleurInitiale;
    }
}
