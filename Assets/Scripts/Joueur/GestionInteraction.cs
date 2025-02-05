using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GestionInteraction : MonoBehaviour
{
    [SerializeField] LayerMask layersConsid�r�;
    [SerializeField] GameObject interactionUI;
    [SerializeField] TextMeshProUGUI messageInteraction;

    public List<DataObjectInteragissable> ObjetInteragissableVisible;
    Camera cam;
    DataObjectInteragissable PlusProche;
    DataObjectInteragissable Pr�c�dent;
    const int AngleMax = 15;
    
    public static event EventHandler OnInteraction;

    
    public bool PeutInteragir = true; //celle associer au menu

    void Awake()
    {
        ComportementInterface.instance.OnMenuChangement += ChangementPossibilit�Interaction;
        ObjetInteragissableVisible = new List<DataObjectInteragissable>();
        cam = GetComponentInChildren<Camera>();
        D�sactiverTexteUI();
    }

    void ChangementPossibilit�Interaction(bool EstJeuActif)
    {
        PeutInteragir = EstJeuActif;
    }

    
    void Update()
    {
        if (ObjetInteragissableVisible.Count > 0)
        {
            Pr�c�dent = PlusProche;
            PlusProche = CalculerPlusProche();

            if (PlusProcheAChang�())
            {
                ChangerCouleur();
            }

            if (PlusProche.objet != null)
            {
                ActiverTexteUI();
                messageInteraction.SetText(PlusProche.objet.GetComponent<Interagir>().D�terminerTexteUI());

                if (Input.GetKeyDown(KeyCode.E) && (PeutInteragir || PlusProche.objet.GetComponent<�tabli>() != null || PlusProche.objet.GetComponent<tabouretLivre>() != null))
                {
                    PlusProche.objet.GetComponent<Interagir>().InteragirObjet();
                    OnInteraction?.Invoke(this, EventArgs.Empty);
                    D�sactiverTexteUI();

                    if (!PlusProche.objet.GetComponent<Interagir>().JouerSonInteraction())
                        GestionBruit.instance.JouerSon("InteragirDefault");
                }
            }
            else
            {
                if (EstPr�c�dentValide())
                {
                    ResetColor(Pr�c�dent);
                    D�sactiverTexteUI();
                }
            }
        }
        else
        {
            PlusProche.objet = null;
            if (EstPr�c�dentValide())
            {
                ResetColor(Pr�c�dent);
                D�sactiverTexteUI();
            }
        }


    }

    bool PlusProcheAChang�()
    {
        return PlusProche.objet != null && PlusProche.objet != Pr�c�dent.objet;
    }

    bool EstPr�c�dentValide()
    {
        return Pr�c�dent.objet != null && Pr�c�dent.mr.Length > 0 && Pr�c�dent.mr[0] != null;
    }

    void ChangerCouleur()
    {
        if (Pr�c�dent.objet != null)
        {
            ResetColor(Pr�c�dent);
        }

        RendrePlusClair(PlusProche);
    }

    bool EstInt�rieurZone() //A refaire avec rayon zone et position
    {
        Vector3 Origine = cam.transform.position;
        Vector3 direction = cam.transform.forward;

        Ray ray = new Ray(Origine, direction);

        Physics.Raycast(ray, out RaycastHit hit, 10, layersConsid�r�);

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

            if (nouvelAngle < angle && (nouvelAngle < angleMax) && EstInt�rieurZone())
            {
                angle = nouvelAngle;
                plusProche = obj;
            }
        }

        
        return plusProche;  
    }

 

    float AngleEntreDeuxVecteurs(Transform ObjectObserv�)
    {
        Vector3 VecteurReliantCameraAObject = ObjectObserv�.position - cam.transform.position;
        float produitScalaire = Vector3.Dot(cam.transform.forward, VecteurReliantCameraAObject);
        
        return Mathf.Acos(produitScalaire / VecteurReliantCameraAObject.magnitude) * 180/Mathf.PI; //cam.transform.forward est un vecteur unitaire, unitile de prendre en compte sa longueur
    }

   void ActiverTexteUI() => interactionUI.SetActive(true);
    void D�sactiverTexteUI() => interactionUI.SetActive(false);
}

[System.Serializable]
public struct DataObjectInteragissable
{
    public GameObject objet;
    public MeshRenderer[] mr;
    public Color[] CouleursOriginale;
    public int AngleInteraction; //LE "INT" est important!!! est compar� et je veux pas deal avec le floating point error
}