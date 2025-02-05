using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] Image imageUI;
    public static EndGameUI instance;

    private void Start()
    {
        if (instance == null)
            instance = this;

        gameObject.SetActive(false);
    }


    public void CommencerEndGame(ÉtatFinPartie finPartie)
    {
        gameObject.SetActive(true);
         Color c = imageUI.color;
        c.a = 0;
        imageUI.color = c;
        
        //lerp background
        StartCoroutine(ActiverImageGraduellement(5, imageUI, finPartie));
    }
    IEnumerator ActiverImageGraduellement(float duree, Image image, ÉtatFinPartie finPartie)
    {
        float timer = 0;
        Color couleur = image.color;
        while (timer < duree)
        {
            //faudrait lerp alpha
           couleur.a = Mathf.Lerp(0, 1, timer / duree);

            image.color = couleur;

            timer += Time.deltaTime;
            yield return null;
        }
        GestionScenes.ChargerFin(finPartie);
    }

}
