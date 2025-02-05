using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDésactivation2 : MonoBehaviour
{
    float Timer = 0;
    [SerializeField] float TempsDeVie = 4;

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime;

        if (Timer > TempsDeVie)
        {
            Timer = 0;
            gameObject.SetActive(false);
        }
    }
}
