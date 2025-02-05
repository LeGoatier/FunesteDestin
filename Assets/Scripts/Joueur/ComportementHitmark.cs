using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportementHitmark : MonoBehaviour
{
    [SerializeField] GameObject redHit;
    [SerializeField] GameObject whiteHit;
    [Range(0,1)]
    [SerializeField] float tempsAffichage;
    float chrono = 0;

    private void Start()
    {
        Desactiver();
    }

    // Update is called once per frame
    void Update()
    {
        if (chrono < tempsAffichage)
        {
            chrono += Time.deltaTime;
        }
        else
        {
            Desactiver();
        }
    }


    public void AfficherBlanc()
    {
        Desactiver();
        whiteHit.SetActive(true);
    }

    public void AfficherRouge()
    {
        Desactiver();
        redHit.SetActive(true);
    }

    private void Desactiver()
    {
        chrono = 0;
        redHit.SetActive(false);
        whiteHit.SetActive(false);
    }
}
