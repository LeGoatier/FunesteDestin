using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum TypeStructure { moteurBrisé, composantPolytechnique, colorant, radioBrisée, essence, voiles}
public class StructureUniquementRamassable : ComportementStructure
{
    private Étapes étapeLiée;
    public override Étapes ÉtapeLiée { get => étapeLiée; }
    //CE script est réservé aux objets qui ne font qu'être ramassé (et rien d'autre) 
    //la position de tous ces objets est obligatoirement défini par un niveau de feu

    DataObjectInteragissable CetObjet;

    [SerializeField] TypeStructure typeStructure;

    void Awake()
    {
        InitialiserComportementStructure();
        étatInitialisation = ÉtatInitialisation.positionBorné;


        if (typeStructure == TypeStructure.voiles)
        {
            gameObject.GetComponentInChildren<MeshRenderer>().material.color *= 3f;
        }

        switch (typeStructure)
        {
            case TypeStructure.moteurBrisé:
                RayonMax = 35;
                RayonMin = 45;
                étapeLiée = Étapes.Moteur;
                
                break;
            case TypeStructure.composantPolytechnique:
                RayonMax = 25;
                RayonMin = 45;
                étapeLiée = Étapes.ComposantPyrotechnique;
                break;
            case TypeStructure.colorant:
                RayonMax = 25;
                RayonMin = 45;
                étapeLiée = Étapes.Colorant;
                break;
            case TypeStructure.radioBrisée:
                RayonMax = 35;
                RayonMin = 45;
                étapeLiée = Étapes.Radio;
                break;
            case TypeStructure.essence:
                RayonMax = 25;
                RayonMin = 35;
                étapeLiée = Étapes.Essence;
                break;
            case TypeStructure.voiles:
                RayonMax = 45;
                RayonMin = 55;
                étapeLiée = Étapes.Voile;
                gameObject.GetComponentInChildren<MeshRenderer>().material.color *= 3f;
                break;
            default:
                Debug.Log("État non-trouvé");
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
            case TypeStructure.moteurBrisé:
                GestionInventaire.AjouterOutil(Outil.MoteurBrisée);
                ModificationInventaire.AjouterCraft(Outil.Moteur);
                break;
            case TypeStructure.composantPolytechnique:
                GestionInventaire.AjouterOutil(Outil.ComposantPyrotechnique);
                ModificationInventaire.AjouterCraft(Outil.FeuDeDétresse);
                break;
            case TypeStructure.colorant:
                GestionInventaire.AjouterOutil(Outil.Colorant);
                ModificationInventaire.AjouterCraft(Outil.FeuDeDétresse);
                break;
            case TypeStructure.radioBrisée:
                GestionInventaire.AjouterOutil(Outil.RadioBrisée);
                ModificationInventaire.AjouterCraft(Outil.Radio);
                break;
            case TypeStructure.essence:
                GestionInventaire.AjouterOutil(Outil.Essence);
                break;
            case TypeStructure.voiles:
                GestionInventaire.AjouterOutil(Outil.Voiles);
                break;
            default:
                Debug.Log("État non-trouvé");
                break;
        }
        
        ObjectRamassableVisible.Remove(CetObjet);
        Destroy(gameObject);
    }


    


    public override string DéterminerTexteUI()
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



    protected override void EstEntréEnCollision(Collider other)
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
