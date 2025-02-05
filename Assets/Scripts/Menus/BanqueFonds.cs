using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BanqueFonds : MonoBehaviour
{
    [SerializeField] Sprite[] images;
    [SerializeField] string[] slogans;
    [SerializeField] float delais = 2;

    private float dernierTemps;

    private int indexI;
    private int indexS;
    
    Image fond;
    TextMeshProUGUI tmp;
    public void Start()
    {
        fond = GameObject.FindGameObjectWithTag("Image").GetComponent<Image>();
        tmp = GameObject.FindGameObjectWithTag("Slogan").GetComponent<TextMeshProUGUI>();

        indexI = Random.Range(0, images.Length - 1);
        indexS = Random.Range(0, slogans.Length - 1);


        fond.sprite = images[indexI];
        tmp.text = slogans[indexS];
        
    }

    public void Update()
    {
        if (dernierTemps + delais <= Time.time)
        {
            dernierTemps = Time.time;

            indexI = (indexI + 1) % (images.Length - 1);
            fond.sprite = images[indexI];

            indexS = (indexS + 1) % (slogans.Length - 1);
            tmp.text = slogans[indexS];

            
        }

    }
}
