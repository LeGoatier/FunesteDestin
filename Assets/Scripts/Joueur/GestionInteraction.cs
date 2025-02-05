using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GestionInteraction : MonoBehaviour
{
    [SerializeField] LayerMask layersConsidéré;
    [SerializeField] GameObject interactionUI;
    [SerializeField] TextMeshProUGUI messageInteraction;

    public List<DataObjectInteragissable> ObjetInteragissableVisible;
    Camera cam;
    DataObjectInteragissable PlusProche;
    DataObjectInteragissable Précédent;
    const int AngleMax = 15;
    
    public static event EventHandler OnInteraction;

    
    public bool PeutInteragir = true; //celle associer au menu

    void Awake()
    {
        ComportementInterface.instance.OnMenuChangement += ChangementPossibilitéInteraction;
        ObjetInteragissableVisible = new List<DataObjectInteragissable>();
        cam = GetComponentInChildren<Camera>();
        DésactiverTexteUI();
    }

    void ChangementPossibilitéInteraction(bool EstJeuActif)
    {
        PeutInteragir = EstJeuActif;
    }

    
    void Update()
    {
        if (ObjetInteragissableVisible.Count > 0)
        {
            Précédent = PlusProche;
            PlusProche = CalculerPlusProche();

            if (PlusProcheAChangé())
            {
                ChangerCouleur();
            }

            if (PlusProche.objet != null)
            {
                ActiverTexteUI();
                messageInteraction.SetText(PlusProche.objet.GetComponent<Interagir>().DéterminerTexteUI());

                if (Input.GetKeyDown(KeyCode.E) && (PeutInteragir || PlusProche.objet.GetComponent<Établi>() != null || PlusProche.objet.GetComponent<tabouretLivre>() != null))
                {
                    PlusProche.objet.GetComponent<Interagir>().InteragirObjet();
                    OnInteraction?.Invoke(this, EventArgs.Empty);
                    DésactiverTexteUI();

                    if (!PlusProche.objet.GetComponent<Interagir>().JouerSonInteraction())
                        GestionBruit.instance.JouerSon("InteragirDefault");
                }
            }
            else
            {
                if (EstPrécédentValide())
                {
                    ResetColor(Précédent);
                    DésactiverTexteUI();
                }
            }
        }
        else
        {
            PlusProche.objet = null;
            if (EstPrécédentValide())
            {
                ResetColor(Précédent);
                DésactiverTexteUI();
            }
        }


    }

    bool PlusProcheAChangé()
    {
        return PlusProche.objet != null && PlusProche.objet != Précédent.objet;
    }

    bool EstPrécédentValide()
    {
        return Précédent.objet != null && Précédent.mr.Length > 0 && Précédent.mr[0] != null;
    }

    void ChangerCouleur()
    {
        if (Précédent.objet != null)
        {
            ResetColor(Précédent);
        }

        RendrePlusClair(PlusProche);
    }

    bool EstIntérieurZone() //A refaire avec rayon zone et position
    {
        Vector3 Origine = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        Ray ray = new Ray(Origine, direction);

        Physics.Raycast(ray, out RaycastHit hit, 10, layersConsidéré);

        if (hit.collider != null)
        {
            return !(hit.collider.tag == "BordureTempete");
        }

        return false;
    }

    

    void ResetColor(DataObjectInteragissable objectRamassable)
    {
        for (int i = 0; i < objectRamassable.mr.Length; i++)        
            objectRamassable.mr[i].material.color = objectRamassable.CouleursOriginale[i];       
    }

    public static void RendrePlusClair(DataObjectInteragissable objectRamassable)
    {
        for (int i = 0; i < objectRamassable.mr.Length; i++)       
            objectRamassable.mr[i].material.color *= 2f;      
    }



    DataObjectInteragissable CalculerPlusProche()
    {
        DataObjectInteragissable plusProche = new DataObjectInteragissable();
            
        float angle = 360;
        
        foreach(DataObjectInteragissable obj in ObjetInteragissableVisible)
        {
            
            float angleMax = obj.AngleInteraction > AngleMax ? obj.AngleInteraction : AngleMax;

            float nouvelAngle = AngleEntreDeuxVecteurs(obj.objet.transform);

            if (nouvelAngle < angle && (nouvelAngle < angleMax) && EstIntérieurZone())
            {
                angle = nouvelAngle;
                plusProche = obj;
            }
        }

        
        return plusProche;  
    }

 

    float AngleEntreDeuxVecteurs(Transform ObjectObservé)
    {
        Vector3 VecteurReliantCameraAObject = ObjectObservé.position - cam.transform.position;
        float produitScalaire = Vector3.Dot(cam.transform.forward, VecteurReliantCameraAObject);
        
        return Mathf.Acos(produitScalaire / VecteurReliantCameraAObject.magnitude) * 180/Mathf.PI; //cam.transform.forward est un vecteur unitaire, unitile de prendre en compte sa longueur
    }

   void ActiverTexteUI() => interactionUI.SetActive(true);
    void DésactiverTexteUI() => interactionUI.SetActive(false);
}

[System.Serializable]
public struct DataObjectInteragissable
{
    public GameObject objet;
    public MeshRenderer[] mr;
    public Color[] CouleursOriginale;
    public int AngleInteraction; //LE "INT" est important!!! est comparé et je veux pas deal avec le floating point error
}