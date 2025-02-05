using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeStructure { moteurBris�, composantPolytechnique, colorant, radioBris�e, essence, voiles}
public class StructureUniquementRamassable : ComportementStructure
{
    private �tapes �tapeLi�e;
    public override �tapes �tapeLi�e { get => �tapeLi�e; }
    //CE script est r�serv� aux objets qui ne font qu'�tre ramass� (et rien d'autre) 
    //la position de tous ces objets est obligatoirement d�fini par un niveau de feu

    DataObjectInteragissable CetObjet;

    [SerializeField] TypeStructure typeStructure;

    void Awake()
    {
        InitialiserComportementStructure();
        �tatInitialisation = �tatInitialisation.positionBorn�;


        if (typeStructure == TypeStructure.voiles)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material.color *= 3f;
        }

        switch (typeStructure)
        {
            case TypeStructure.moteurBris�:
                RayonMax = 35;
                RayonMin = 45;
                �tapeLi�e = �tapes.Moteur;
                
                break;
            case TypeStructure.composantPolytechnique:
                RayonMax = 25;
                RayonMin = 45;
                �tapeLi�e = �tapes.ComposantPyrotechnique;
                break;
            case TypeStructure.colorant:
                RayonMax = 25;
                RayonMin = 45;
                �tapeLi�e = �tapes.Colorant;
                break;
            case TypeStructure.radioBris�e:
                RayonMax = 35;
                RayonMin = 45;
                �tapeLi�e = �tapes.Radio;
                break;
            case TypeStructure.essence:
                RayonMax = 25;
                RayonMin = 35;
                �tapeLi�e = �tapes.Essence;
                break;
            case TypeStructure.voiles:
                RayonMax = 45;
                RayonMin = 55;
                �tapeLi�e = �tapes.Voile;
                gameObject.GetComponentInChildren<MeshRenderer>().material.color *= 3f;
                break;
            default:
                Debug.Log("�tat non-trouv�");
                break;
        }



        InitialiserDataObjet();
        if (typeStructure == TypeStructure.voiles)
        {
            CetObjet.AngleInteraction = 50;
        }
    }



    public override void InteragirObjet()
    {
        switch (typeStructure)
        {
            case TypeStructure.moteurBris�:
                GestionInventaire.AjouterOutil(Outil.MoteurBris�e);
                ModificationInventaire.AjouterCraft(Outil.Moteur);
                break;
            case TypeStructure.composantPolytechnique:
                GestionInventaire.AjouterOutil(Outil.ComposantPyrotechnique);
                ModificationInventaire.AjouterCraft(Outil.FeuDeD�tresse);
                break;
            case TypeStructure.colorant:
                GestionInventaire.AjouterOutil(Outil.Colorant);
                ModificationInventaire.AjouterCraft(Outil.FeuDeD�tresse);
                break;
            case TypeStructure.radioBris�e:
                GestionInventaire.AjouterOutil(Outil.RadioBris�e);
                ModificationInventaire.AjouterCraft(Outil.Radio);
                break;
            case TypeStructure.essence:
                GestionInventaire.AjouterOutil(Outil.Essence);
                break;
            case TypeStructure.voiles:
                GestionInventaire.AjouterOutil(Outil.Voiles);
                break;
            default:
                Debug.Log("�tat non-trouv�");
                break;
        }
        
        ObjectRamassableVisible.Remove(CetObjet);
        Destroy(gameObject);
    }


    


    public override string D�terminerTexteUI()
    {
        string message = "Ramasser ";

        foreach(char c in typeStructure.ToString() )
        {
            if (c.ToString() == c.ToString().ToUpper())
                message += " ";
            message += c.ToString().ToLower();
        }
        return message;
    }

    void InitialiserDataObjet()
    {
        CetObjet.objet = gameObject;
        CetObjet.mr = GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CetObjet.mr.Length];

        for (int i = 0; i < CetObjet.mr.Length; i++)
        {
            couleurs[i] = CetObjet.mr[i].material.color;
        }

        CetObjet.CouleursOriginale = couleurs;
        CetObjet.AngleInteraction = 30;
    }



    protected override void EstEntr�EnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ObjectRamassableVisible.Add(CetObjet);
        }

    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Remove(CetObjet);
    }

    
}
