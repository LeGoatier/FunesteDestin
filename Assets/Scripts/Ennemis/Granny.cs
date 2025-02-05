using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Granny : MonoBehaviour
{
    [SerializeField] GameObject granny;
    Vector3 centre; //centre de l'ile
    float rayonBordure;
    public bool EstChangementNuit = false;
    GameObject grannyTemp;

    public static Granny instance;

    void Start()
    {
        if (instance == null)
            instance = this;

        // Obtient la position du centre de la bordure
        centre = Placement.VecteurZeroWorld;

        grannyTemp = Instantiate(granny, Vector3.zero, Quaternion.identity);
        grannyTemp.SetActive(false);
    }

    //il faut ajouter les jumpscares

    void Update()
    {
        if (EstChangementNuit)
        {
            // 1 chance sur trois que granny apparaisse par nuit
            if (Random.Range(0, 3) <= 1)
                GérerApparitionGranny();

            EstChangementNuit = false;
        }
    }

    void GérerApparitionGranny()
    {
        StartCoroutine(ApparaitreEtDisparaitreGranny());
    }

    IEnumerator ApparaitreEtDisparaitreGranny()
    {
        // Détermine les positions d'apparition
        Vector3 position1 = TrouverPositionRandom();
        Vector3 position2 = TrouverPositionRandom();

        //On attend 10 secondes pour pas que granny apparaisse dès que la nuit commence
        yield return new WaitForSeconds(10);

        // On va faire apparaitre la granny entre 4-7 secondes autour de la bordure 2x a des endroits differents
        grannyTemp.SetActive(true);
        grannyTemp.transform.SetPositionAndRotation(position1, Quaternion.LookRotation(FeuDeCamp.instance.transform.position - position1));
        yield return new WaitForSeconds(Random.Range(4, 7));

        grannyTemp.transform.SetPositionAndRotation(position1, Quaternion.LookRotation(FeuDeCamp.instance.transform.position - position2));
        yield return new WaitForSeconds(Random.Range(4, 7));
        grannyTemp.SetActive(false);
    }

    private Vector3 TrouverPositionRandom()
    {
        // Calcule une position autour de la bordure
        float angle = Random.value;
        float x = centre.x + Mathf.Cos(angle) * rayonBordure;
        float z = centre.z + Mathf.Sin(angle) * rayonBordure;
        float y = TrouverHauteur(x, z);

        return new(x, y, z);
    }

    private float TrouverHauteur(float x, float z)
    {
        Ray ray = new(new Vector3(x, 50f, z), Vector3.down);

        Physics.Raycast(ray, out RaycastHit hit, 100);
        return hit.point.y;
    }

    public void GérerChangementNiveauFeu()
    {
        rayonBordure = FeuDeCamp.instance.GetRayonSelonNiveau(FeuDeCamp.instance.niveauActuel) + 6;

        if (grannyTemp.activeSelf)
            grannyTemp.SetActive(false);
    }



    private void RotaterGranny()
    {
        Vector3 directionToCamera = GameManager.instance.GetPositionCam() - transform.position ;
        grannyTemp.transform.rotation = Quaternion.LookRotation(new Vector3(directionToCamera.x, 0, directionToCamera.z));
    }
}
