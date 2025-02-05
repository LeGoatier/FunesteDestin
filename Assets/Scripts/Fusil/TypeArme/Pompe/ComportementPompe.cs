using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class ComportementPompe : ComportementFusil
{
    // --- Audio ---
    [SerializeField] AudioClip GunShotClip;
    [SerializeField] AudioClip EquipedClip;


    [SerializeField] GameObject explosion;

    int NombreBallePhysiqueParTir = 8;
    float ImperfectionRange = 3f;

    private void Start()
    {
        AudioSource.PlayClipAtPoint(EquipedClip, transform.position,0.1f);
        DélaisEntreTir = 0.6f;
        DélaisRecharge = 1.5f;
        NombreBallesTotalesChargeur = 20;

        Initialiser();

        NombreBalleParTir = 5;
    }

    

    protected override bool ImputTirer()
    {
        return Input.GetMouseButtonDown(0);
    }
    protected override void InitiationRotationBalle(GameObject instance)
    {

        
        Vector3 Origine = CameraJoueur.transform.position;
        Vector3 direction = CameraJoueur.transform.forward;

        Ray ray = new Ray(Origine, direction);
        RaycastHit hit;



        if (Physics.Raycast(ray, out hit, RayonMaxDistance, ~LayerAIgnorer))
        {
           
            if (Mathf.Abs(Vector3.Distance(hit.point, CameraJoueur.transform.position) - Vector3.Distance(Embout.transform.position, CameraJoueur.transform.position)) < 0.5f)
            {
                instance.transform.rotation = CameraJoueur.transform.rotation;
            }
            else
            {
                instance.transform.rotation = Quaternion.LookRotation(hit.point - Embout.transform.position, Vector3.up);
            }
            
        }
        else
        {
            instance.transform.rotation = Quaternion.LookRotation(CameraJoueur.transform.position + direction * DistanceMaxTirVide - Embout.transform.position, Vector3.up);
        }

        Vector3 differenceRotation = new Vector3(Random.Range(-ImperfectionRange, ImperfectionRange), Random.Range(-ImperfectionRange, ImperfectionRange), 0);
        instance.transform.Rotate(differenceRotation);


        if(hit.distance < 1 && hit.distance != 0)
        {
            instance.GetComponent<TrailRenderer>().enabled = false;
        }
    }

    protected override void InstancierBalle()
    {
        for(int i =0; i < NombreBallePhysiqueParTir; i++)
        {
            var balleTemp = ObjectPool.instance.GetPoolObject(Balle);
            balleTemp.transform.position = Embout.transform.position;
            balleTemp.GetComponent<TrailRenderer>().enabled = true;
            InitiationRotationBalle(balleTemp);

            balleTemp.SetActive(true);
        }
       
    }

    

    protected override void EffetsSonoresEtVisuelTirer()
    {
        Instantiate(explosion, Embout.transform);

        AudioSource.PlayClipAtPoint(GunShotClip, transform.position, 0.1f);
    }
}

