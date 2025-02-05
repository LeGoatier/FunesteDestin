using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class ComportementPistolet : ComportementFusil
{

    // --- Audio ---
    [SerializeField] AudioClip GunShotClip;
    [SerializeField] AudioClip EquipedClip;
    float Volume;


    [SerializeField] GameObject explosion;

    const float ImperfectionTir = 0.1f;
    const float DistanceMinimalPointTouché = 0.5f;

    private void Start()
    {
        Volume = 0.1f;
        AudioSource.PlayClipAtPoint(EquipedClip, transform.position, Volume);
        DélaisEntreTir = 0.3f;
        DélaisRecharge = 1;
        NombreBallesTotalesChargeur = 10;

        Initialiser();
    }


    protected override bool ImputTirer() //La touche qui active le tir
    {
        return Input.GetMouseButtonDown(0);
    }
    protected override void InitiationRotationBalle(GameObject instance)
    {
        Vector3 Origine = CameraJoueur.transform.position;
        Vector3 direction = CameraJoueur.transform.forward;

        Ray ray = new Ray(Origine, direction);
        RaycastHit hit;


        if (Physics.Raycast(ray, out hit, RayonMaxDistance, ~LayerAIgnorer)) //Ignore certain collider, comme ceux des objets ramassable sur le sol
        {
            OrienterBalleVersViseur(hit, instance);
        }
        else
        {
            //Si le rayon n'a rien touché, on oriente la balle vers un point très loins dans la direction du foward du joueur
            instance.transform.rotation = Quaternion.LookRotation(CameraJoueur.transform.position + direction * DistanceMaxTirVide - Embout.transform.position, Vector3.up);
        }

        AjouterDéviation(instance);
        AjouterTrainée(hit, instance);
    }

    void OrienterBalleVersViseur(RaycastHit hit, GameObject instance)
    {
        //Si la point touché et très proche de l'embout du fusil
        if (EstProcheDeLEmbout(hit))
        {
            instance.transform.rotation = CameraJoueur.transform.rotation;
        }
        else
        {
            instance.transform.rotation = Quaternion.LookRotation(hit.point - Embout.transform.position, Vector3.up);
        }
    }

    bool EstProcheDeLEmbout(RaycastHit hit) //si l'embout est trop proche du point touché
    {
        float distanceEntreJoueurEtPointTouché = Vector3.Distance(hit.point, CameraJoueur.transform.position);
        float distanceEntreEmboutEtPointTouché = Vector3.Distance(Embout.transform.position, CameraJoueur.transform.position);
        return Mathf.Abs(distanceEntreJoueurEtPointTouché - distanceEntreEmboutEtPointTouché) < DistanceMinimalPointTouché;
    }

    void AjouterDéviation(GameObject instance)
    {
        Vector3 differenceRotation = new Vector3(Random.Range(-ImperfectionTir, ImperfectionTir), Random.Range(-ImperfectionTir, ImperfectionTir), 0);
        instance.transform.Rotate(differenceRotation);
    }

    void AjouterTrainée(RaycastHit hit, GameObject instance)
    {
        if (hit.distance < DistanceMinimalPointTouché && hit.distance != 0)
        {
            instance.GetComponent<TrailRenderer>().enabled = false;
        }

    }

    protected override void EffetsSonoresEtVisuelTirer()
    {
        Instantiate(explosion, Embout.transform);

        AudioSource.PlayClipAtPoint(GunShotClip, transform.position, Volume);
        
    }

}
