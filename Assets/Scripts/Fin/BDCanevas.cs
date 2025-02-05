using System.Collections;
using UnityEngine;
using Image = UnityEngine.UI.Image;

public class BDCanevas : MonoBehaviour
{
    [SerializeField] Image[] images;

    const float DURÉE_FADE_IN = 1;
    int indiceImageAChargerActuelle = 0;


    // Start is called before the first frame update
    void Start()
    {
        InitialiserImages(); //Pour mettre leur albedo à 0
    }

    private void InitialiserImages()
    {
        foreach(Image image in images)
        {
            Color couleur = image.color;
            couleur.a = 0;
            image.color = couleur;
            image.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            if(indiceImageAChargerActuelle < images.Length)
            {
                StartCoroutine(ChangerAlphaImageGraduellement(DURÉE_FADE_IN, images[indiceImageAChargerActuelle++]));
            }
            else
            {
                FinManager.instance.LancerRésuméPartie();
                gameObject.SetActive(false); //Pour désactiver le canevas de BD dont on n'a plus besoin
            }
        }
    }

    IEnumerator ChangerAlphaImageGraduellement(float duree, Image image)
    {
        image.gameObject.SetActive(true);
        float timer = 0;
        Color couleur = image.color;
        while (timer < duree)
        {
            couleur.a = Mathf.Lerp(0, 1, timer / duree);

            image.color = couleur;

            timer += Time.deltaTime;
            yield return null;
        }
        couleur.a = 1;
        image.color = couleur;
    }
}
