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
    const float DistanceMinimalPointTouch� = 0.5f;

    private void Start()
    {
        Volume = 0.1f;
        AudioSource.PlayClipAtPoint(EquipedClip, transform.position, Volume);
        D�laisEntreTir = 0.3f;
        D�laisRecharge = 1;
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
            //Si le rayon n'a rien touch�, on oriente la balle vers un point tr�s loins dans la direction du foward du joueur
            instance.transform.rotation = Quaternion.LookRotation(CameraJoueur.transform.position + direction * DistanceMaxTirVide - Embout.transform.position, Vector3.up);
        }

        AjouterD�viation(instance);
        AjouterTrain�e(hit, instance);
    }

    void OrienterBalleVersViseur(RaycastHit hit, GameObject instance)
    {
        //Si la point touch� et tr�s proche de l'embout du fusil
        if (EstProcheDeLEmbout(hit))
        {
            instance.transform.rotation = CameraJoueur.transform.rotation;
        }
        else
        {
            instance.transform.rotation = Quaternion.LookRotation(hit.point - Embout.transform.position, Vector3.up);
        }
    }

    bool EstProcheDeLEmbout(RaycastHit hit) //si l'embout est trop proche du point touch�
    {
        float distanceEntreJoueurEtPointTouch� = Vector3.Distance(hit.point, CameraJoueur.transform.position);
        float distanceEntreEmboutEtPointTouch� = Vector3.Distance(Embout.transform.position, CameraJoueur.transform.position);
        return Mathf.Abs(distanceEntreJoueurEtPointTouch� - distanceEntreEmboutEtPointTouch�) < DistanceMinimalPointTouch�;
    }

    void AjouterD�viation(GameObject instance)
    {
        Vector3 differenceRotation = new Vector3(Random.Range(-ImperfectionTir, ImperfectionTir), Random.Range(-ImperfectionTir, ImperfectionTir), 0);
        instance.transform.Rotate(differenceRotation);
    }

    void AjouterTrain�e(RaycastHit hit, GameObject instance)
    {
        if (hit.distance < DistanceMinimalPointTouch� && hit.distance != 0)
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
