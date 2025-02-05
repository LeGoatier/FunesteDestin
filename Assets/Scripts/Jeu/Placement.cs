using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Placement : MonoBehaviour
{
    //Instanciation des variables
    #region
    [SerializeField] GameObject Joueur;

    [SerializeField] GameObject[] tableauStructure; //Ce tableau contient toutes les structures dans le désordre (prefabs)
    GameObject[] tableauStructuresInstanciées;
    List<SphereCollider> listeColliderTemporaire = new List<SphereCollider>();

    private List<GameObject> structuresChoisies = new List<GameObject>();
    [SerializeField] GameObject[] listeLivret;
    [SerializeField] GameObject[] ListeFusil;

    public event EventHandler OnStructureInstancié;
    const float loopMax = 100;

    //Elements nature
    [SerializeField] GameObject[] ListeNature;
    [SerializeField] int[] densitésNature;

    //Ressources
    [SerializeField] GameObject[] RessourcesRamassables;
    [SerializeField] GameObject[] Heal;
    public Vector3[] listePositionsLibres;
    public List<Vector3> listePositionsLibresHeal;
    float timerRegeneration;

    public static Vector3 VecteurZeroWorld;

    Gen_Ile MapGenerator;

    //Ressources à faire apparaître en fonction des nécessités selon les étapes activées
    private (Cout cout, int niveauDeFeu)[] ressourcesNécesairesNiveauFeu = new (Cout, int)[(int)Étapes.nbÉtapes];

    ChoixÉtapesGraphe choixÉtapesGraphe;

    #endregion
    void Awake()
    {
        InitialiserCoutÉtapes();
        MapGenerator = GameObject.Find("MapGenerator").GetComponent<Gen_Ile>();
        MapGenerator.OnGénérationTerrainFini += PlacerÉléments;
    }

    private void Update()
    {
        if (timerRegeneration >= 45)
        {
            RennaissanceRessources();
            RenaissanceHeal();
            timerRegeneration = 0;
        }

        timerRegeneration += Time.deltaTime;
    }
  
    //Cette méthode est appelée dans le Awake() et elle associe un Cout à chaque étape. La classe Cout (implémentée par Justin) permet de garder en mémoire le nombre de chaque ressource nécéssaire à chaque Étape.
    //Le nombre de ressources nécéssaires à chaque étape est établi dans un document word que toute l’équipe a accès à. Ainsi, Le nombre de ressources placées est le même que le nombre de ressources nécéssaires à la fuite.
    //Chaque étape a un niveau de feu qui lui est associé. Les ressources associées à l’étape se feront placer au niveau de feu adéquat pour permettre de bien étaler les ressources sur l’île.


    private void InitialiserCoutÉtapes()
    {
        Cout coutÉtabli = new();
        coutÉtabli.AjouterRessource(Ressource.Bois, 2);
        coutÉtabli.AjouterRessource(Ressource.Fer, 1);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Établi].cout = coutÉtabli;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Établi].niveauDeFeu = 1;

        Cout coutMarteau = new();
        coutMarteau.AjouterRessource(Ressource.Bois, 1);
        coutMarteau.AjouterRessource(Ressource.Fer, 1);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Marteau].cout = coutMarteau;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Marteau].niveauDeFeu = 1;

        Cout coutAbri = new();
        coutAbri.AjouterRessource(Ressource.Bois, 8);
        coutAbri.AjouterRessource(Ressource.Pierre, 4);
        coutAbri.AjouterRessource(Ressource.Plastique, 3);
        coutAbri.AjouterRessource(Ressource.Fer, 7);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Abri].cout = coutAbri;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Abri].niveauDeFeu = 2;

        Cout coutMoteur = new();
        coutMoteur.AjouterRessource(Ressource.Fer, 6);
        coutMoteur.AjouterRessource(Ressource.Plastique, 6);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Moteur].cout = coutMoteur;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Moteur].niveauDeFeu = 3;

        Cout coutRadio = new();
        coutRadio.AjouterRessource(Ressource.Fer, 6);
        coutRadio.AjouterRessource(Ressource.Plastique, 8);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Radio].cout = coutRadio;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Radio].niveauDeFeu = 4;

        Cout coutFuséeDétresse = new();
        coutFuséeDétresse.AjouterRessource(Ressource.Plastique, 2);
        ressourcesNécesairesNiveauFeu[(int)Étapes.FuséeDétresse].cout = coutFuséeDétresse;
        ressourcesNécesairesNiveauFeu[(int)Étapes.FuséeDétresse].niveauDeFeu = 5;

        Cout coutBateau = new();
        coutBateau.AjouterRessource(Ressource.Bois, 16);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Bateau].cout = coutBateau;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Bateau].niveauDeFeu = 5;

        Cout coutAntenne = new();
        coutAntenne.AjouterRessource(Ressource.Fer, 4);
        coutAntenne.AjouterRessource(Ressource.Plastique, 4);
        ressourcesNécesairesNiveauFeu[(int)Étapes.Antenne].cout = coutAntenne;
        ressourcesNécesairesNiveauFeu[(int)Étapes.Antenne].niveauDeFeu = 6;

    }

    void PlacerÉléments(object sender, EventArgs eventArgs)
    {
        Joueur.transform.position = new Vector3(0, 100, 0); //déplacement du joueur pour ne pas qu'il bloque un rayon :)
        InitialiserVecteurZero();

        tableauStructuresInstanciées = InstancierStructures();
        DéterminerÉtapesActivées();

        PlacerStructures();
        PlacerLivret();
        PlacerFusils();
        PlacerRessources();
        PlacerHeal();
        PlacerNature();
        TrouverListePositionsLibres();

        SupprimerColliderTemporaire();
        Joueur.transform.position = new Vector3(0, 10, 0);
    }

    static void InitialiserVecteurZero()
    {
        (float hauteur, Vector3 normale, bool TerrainTouché) = TrouverHauteur(0, 0);
        VecteurZeroWorld = new Vector3(0, hauteur, 0);
    }

    private void RennaissanceRessources()
    {
        int niveauJoueur = FeuDeCamp.instance.niveauActuel;
        int[] nbÉlémentsÀInstancier = new int[4] { 2, 0, 0, 0 };

        switch (niveauJoueur)
        {
            case 2:
                nbÉlémentsÀInstancier = new int[]{ 2, 1, 1, 0};
                break;
            case 3:
                nbÉlémentsÀInstancier = new int[] { 3, 2, 1, 0};
                break;
            case 4:
                nbÉlémentsÀInstancier = new int[] { 3, 2, 2, 0 };
                break;
            case > 4:
                nbÉlémentsÀInstancier = new int[]{ 3, 2, 2, 1 };
                break;
        }

        int indexDépart = niveauJoueur * 10;
        for (int i = 0; i < nbÉlémentsÀInstancier.Length; i++)
        {
            for (int j = 0; j < nbÉlémentsÀInstancier[i]; j++)
            {
                int index = indexDépart + (i > 0 ? nbÉlémentsÀInstancier[0] : 0) + (i > 1 ? nbÉlémentsÀInstancier[1] : 0) + (i > 2 ? nbÉlémentsÀInstancier[2] : 0) + (i > 3 ? nbÉlémentsÀInstancier[3] : 0) + j;
                if (listePositionsLibres[0] != Vector3.zero)
                {
                    GameObject objetTemp = Instantiate(RessourcesRamassables[i]);
                    objetTemp.transform.position = listePositionsLibres[index];
                    listePositionsLibres[index] = Vector3.zero;
                }
                else if(niveauJoueur > 0)
                {
                    index -= 10;
                    if (listePositionsLibres[index] != Vector3.zero)
                    {
                        GameObject objetTemp = Instantiate(RessourcesRamassables[i]);
                        objetTemp.transform.position = listePositionsLibres[index];
                        listePositionsLibres[index] = Vector3.zero;
                    }
                }
            }
        }
    }

    void RenaissanceHeal()
    {
        for(int i = 0; i < 2 ; i++)
        {
            int index = UnityEngine.Random.Range(0, listePositionsLibresHeal.Count());
            if (listePositionsLibresHeal[index] != null)
            {
                Instantiate(Heal[0], listePositionsLibresHeal[index], Quaternion.identity);
                //objetTemp.transform.position = listePositionsLibresHeal[index];
            }
        }
    }


    //---Structures-------------------------------------------------------------------
    #region
    void PlacerStructures()
    {
        //Après graphe justin: pour chaque niveau => on boucle dans se code pour chaque niveau => compStrcut.niveau est remplacé par niveau de la boucle
        GameObject[] listInstanceStructure = structuresChoisies.ToArray();

        //List<GameObject> listOrdonnée = OrdonnerListe(listInstanceStructure);
        List<GameObject> positionFixe = ListePositionFixe(listInstanceStructure);
        List<GameObject> positionCondition = ListePositionConditionelle(listInstanceStructure);
        List<GameObject> positionAutre = ListePositionAutre(listInstanceStructure);

        //Placer ceux-ci sans collider pour éviter une boucle infini
        foreach (GameObject struc in positionFixe)
        {
            ComportementStructure compStruct = struc.GetComponent<ComportementStructure>();
            PlacerÉtatPositionFixe(struc, compStruct);
        }
        foreach (GameObject struc in positionCondition)
        {
            ComportementStructure compStruct = struc.GetComponent<ComportementStructure>();
            PlacerÉtatPositionCondition(struc, compStruct);
        }

        AjouterColliderTemporaireListe(positionFixe);
        AjouterColliderTemporaireListe(positionCondition);

        //AutreObjet
        foreach (GameObject struc in positionAutre)
        {
            ComportementStructure compStruct = struc.GetComponent<ComportementStructure>();
            ÉtatInitialisation état = compStruct.étatInitialisation;

            switch (état)
            {
                case ÉtatInitialisation.positionBorné:
                    PlacerÉtatPositionBorné(struc, compStruct.RayonMin, compStruct.RayonMax, true);
                    break;
                case ÉtatInitialisation.positionNiveau:
                    //marche que si le feu à été instancié avant (en théorie le feu est le premier élément de la liste structure
                    PlacerÉtatPositionNiveau(struc, compStruct.NiveauSpawn);
                    break;
                default:
                    Debug.Log("État non-trouvé : " + struc.name);
                    break;
            }

            SphereCollider colliderTemp = struc.AddComponent<SphereCollider>();
            colliderTemp.radius = 3;
            listeColliderTemporaire.Add(colliderTemp);
        }
        OnStructureInstancié?.Invoke(this, EventArgs.Empty);
    }

    void DéterminerÉtapesActivées()
    {
        choixÉtapesGraphe = new();
        foreach (GameObject structure in tableauStructuresInstanciées)
        {
            if (choixÉtapesGraphe.EstÉtapeActivée(structure.GetComponent<ComportementStructure>().ÉtapeLiée))
            {
                structuresChoisies.Add(structure);
            }
            else
            {
                Destroy(structure);
            }
        }
    }

    GameObject[] InstancierStructures()
    {
        GameObject[] tableau = new GameObject[tableauStructure.Length];
        for (int i = 0; i < tableau.Length; i++)
        {
            tableau[i] = Instantiate(tableauStructure[i], new Vector3(0, 120, 0), Quaternion.identity);
        }
        return tableau;
    }

    private void AjouterColliderTemporaireListe(List<GameObject> liste)
    {
        foreach (GameObject struc in liste)
        {
            SphereCollider colliderTemp = struc.AddComponent<SphereCollider>();
            colliderTemp.radius = 2;
            listeColliderTemporaire.Add(colliderTemp);
        }
    }
    void SupprimerColliderTemporaire()
    {
        foreach (SphereCollider sc in listeColliderTemporaire)
        {
            Destroy(sc);
        }
    }

    List<GameObject> OrdonnerListe(GameObject[] tableau)
    {
        //Les objects avec une position fixe s'instancie en permier et doit être au niveau 0
        //position conditionnelle (doit être au dela du rayon de niveau 0) et le reste apres
        List<GameObject> listeOrdonnée = new List<GameObject>();

        listeOrdonnée.AddRange(tableau.Where(obj => obj.GetComponent<ComportementStructure>().étatInitialisation == ÉtatInitialisation.positionFixe).ToList());
        listeOrdonnée.AddRange(tableau.Where(obj => obj.GetComponent<ComportementStructure>().étatInitialisation == ÉtatInitialisation.positionCondition).ToList());
        listeOrdonnée.AddRange(tableau.Where(obj => obj.GetComponent<ComportementStructure>().étatInitialisation != ÉtatInitialisation.positionCondition && obj.GetComponent<ComportementStructure>().étatInitialisation != ÉtatInitialisation.positionCondition).ToList());

        return listeOrdonnée;
    }
    List<GameObject> ListePositionFixe(GameObject[] tableau)
    {
        return tableau.Where(obj => obj.GetComponent<ComportementStructure>().étatInitialisation == ÉtatInitialisation.positionFixe).ToList();
    }
    List<GameObject> ListePositionConditionelle(GameObject[] tableau)
    {
        return tableau.Where(obj => obj.GetComponent<ComportementStructure>().étatInitialisation == ÉtatInitialisation.positionCondition).ToList();
    }
    List<GameObject> ListePositionAutre(GameObject[] tableau)
    {
        return tableau.Where(obj => obj.GetComponent<ComportementStructure>().étatInitialisation != ÉtatInitialisation.positionFixe && obj.GetComponent<ComportementStructure>().étatInitialisation != ÉtatInitialisation.positionCondition).ToList();
    }

    static void PlacerÉtatPositionFixe(GameObject structure, ComportementStructure comp)
    {
        (float hauteur, Vector3 normale, bool TerrainTouché) = TrouverHauteur(comp.Position.x, comp.Position.y);
        structure.transform.position = new Vector3(comp.Position.x, hauteur, comp.Position.y);

        if (comp.Position.x == 0 && comp.Position.y == 0)
        {
            //Si c'est le feu 
            structure.transform.up = normale;
        }
        else
        {
            structure.transform.LookAt(VecteurZeroWorld, normale);
        }
    }
    static void PlacerÉtatPositionCondition(GameObject structure, ComportementStructure comp)
    {
        List<Vector3> positionsPossibles = comp.ConditionSpawn();
        int indexAléatoire = UnityEngine.Random.Range(0, positionsPossibles.Count - 1);
        Vector3 PositionAléaloire = positionsPossibles[indexAléatoire];

        (float hauteur, Vector3 normale, bool TerrainTouché) = TrouverHauteur(PositionAléaloire.x, PositionAléaloire.z);

        structure.transform.position = new Vector3(PositionAléaloire.x, hauteur, PositionAléaloire.z);
        structure.transform.up = normale;

        if (comp.ÉtapeLiée == Étapes.Antenne)
        {
            structure.transform.rotation = Quaternion.identity;
        }
        else if(comp.ÉtapeLiée == Étapes.Bateau)
        {
            Vector3 position = structure.transform.position + (Vector3.zero - structure.transform.position).normalized * 4;
            (hauteur, normale, TerrainTouché) = TrouverHauteur(position.x, position.z);
            position.y = hauteur;
            structure.GetComponent<Bateau>().InstancierLivreBateau(position, normale);
        }
    }
    static void PlacerÉtatPositionBorné(GameObject structure, float rayonMin, float rayonMax, bool estPerpendiculaireSol)
    {
        int compteur = 0;
        bool ATouchéTerrain = false;

        while (!ATouchéTerrain)
        {
            compteur++;
            Vector2 CoordonéesXZ = TrouverCoordonnéeRandomXZ(rayonMin, rayonMax);
            (float hauteur, Vector3 normale, bool TerrainTouché) hitInfo = TrouverHauteur(CoordonéesXZ.x, CoordonéesXZ.y);

            if (hitInfo.TerrainTouché && hitInfo.hauteur > 0.5f)
            {
                structure.transform.position = new Vector3(CoordonéesXZ.x, hitInfo.hauteur, CoordonéesXZ.y);
                structure.transform.LookAt(VecteurZeroWorld, hitInfo.normale);

                if (!estPerpendiculaireSol)
                {
                    structure.transform.right = hitInfo.normale;
                }


                ATouchéTerrain = true;
            }
            if (compteur > loopMax)
            {
                Debug.Log("PlacerÉtatPositionBorné : NT");
                break;
            }
        }
    }

    void PlacerÉtatPositionNiveau(GameObject structure, int niveau)
    {
        int compteur = 0;
        bool ATouchéTerrain = false;

        while (!ATouchéTerrain)
        {
            compteur++;

            Vector2 CoordonéesXZ = TrouverCoordonnéeRandomXZ(FeuDeCamp.instance.GetRayonSelonNiveau(niveau - 1), FeuDeCamp.instance.GetRayonSelonNiveau(niveau));
            (float hauteur, Vector3 normale, bool TerrainTouché) hitInfo = TrouverHauteur(CoordonéesXZ.x, CoordonéesXZ.y);

            if (hitInfo.TerrainTouché && hitInfo.hauteur > 0.5f)
            {
                structure.transform.position = new Vector3(CoordonéesXZ.x, hitInfo.hauteur, CoordonéesXZ.y);
                structure.transform.LookAt(VecteurZeroWorld, hitInfo.normale);


                ATouchéTerrain = true;
            }
            if (compteur > loopMax)
            {
                Debug.Log("PlacerÉtatPositionNiveau : NT");
                break;
            }
        }
    }

    #endregion
    //---Structures-(fin)-------------------------------------------------------------

    void PlacerLivret()
    {
        var livret1 = Instantiate(listeLivret[0]);
        PlacerÉtatPositionBorné(livret1, 3, 9, true);

        var livret2 = Instantiate(listeLivret[1]);
        PlacerÉtatPositionBorné(livret2, FeuDeCamp.instance.GetRayonSelonNiveau(0), FeuDeCamp.instance.GetRayonSelonNiveau(1), true);

        var livret3 = Instantiate(listeLivret[2]);
        PlacerÉtatPositionBorné(livret3, FeuDeCamp.instance.GetRayonSelonNiveau(2), FeuDeCamp.instance.GetRayonSelonNiveau(3), true);
    }

    void PlacerFusils()
    {
        foreach (GameObject fusil in ListeFusil)
        {
            InteragirFusils scriptfusil = fusil.GetComponent<InteragirFusils>();

            if (scriptfusil.TypeArme == Arme.Arbalète)
            {
                var instance = Instantiate(fusil);
                PlacerÉtatPositionBorné(instance, 0, 5, false);
                transform.position += new Vector3(0, 0.2f, 0);


            }
            else if (scriptfusil.TypeArme == Arme.Pompe)
            {
                var instance = Instantiate(fusil);
                PlacerÉtatPositionBorné(instance, 15, 30, false);
                transform.position += new Vector3(0, 0.2f, 0);
            }
        }
    }

    void PlacerRessources()
    {
        FeuDeCamp feu = FeuDeCamp.instance;

        // Boucle à travers les niveau du feu pour placer les ressources à chaque niveau de feu
        for (int niveau = 0; niveau < feu.feux.Count; niveau++)
        {
            float rayonMin = feu.GetRayonSelonNiveau(niveau - 1);
            float rayonMax = feu.GetRayonSelonNiveau(niveau);

            // Détermine le nombre de chaque ressources à placer à chaque rayon de feu
            (int nbBois, int nbPierre, int nbFer, int nbPlastique) = CalculerRessourcesNécéssaires(niveau);
            int[] nbÉlémentsÀInstancier = new int[] { nbBois, nbPierre, nbFer, nbPlastique, 0 };

            //Instancie les ressources ramassables selon les rayons min et max
            InstancierÉléments(RessourcesRamassables, nbÉlémentsÀInstancier, rayonMin, rayonMax, false);
        }
    }

    void PlacerHeal()
    {
        int nbChamp = 20;
        int NbTrousse = 5;
        int[] nbheal = new int[] { nbChamp, NbTrousse };
        InstancierÉléments(Heal, nbheal, 10, 75, false);
    }


    void PlacerNature()
    {
        float RAYON_MIN = 12;
        float RAYON_MAX = 200;

        InstancierÉléments(ListeNature, densitésNature, RAYON_MIN, RAYON_MAX, true);
    }

    void InstancierÉléments(GameObject[] tabObjets, int[] nbElements, float rayonMin, float rayonMax, bool scaleRandom)
    {
        for (int i = 0; i < tabObjets.Length; i++)
            for (int j = 0; j < nbElements[i]; j++)
            {
                int compteur = 0;
                bool AToucheTerrain = false;

                while (!AToucheTerrain)
                {
                    compteur++;

                    //on trouve des coordonnées aléatoires selon les rayons puis la hauteur
                    Vector2 CoordonéesXZ = TrouverCoordonnéeRandomXZ(rayonMin, rayonMax);
                    (float hauteur, Vector3 normale, bool TerrainTouché) = TrouverHauteur(CoordonéesXZ.x, CoordonéesXZ.y);
                    Vector3 nouvellePos = new(CoordonéesXZ.x, hauteur, CoordonéesXZ.y);

                    //Si l'objet est plus haut que 1 et ne touche rien d'autre et est sur l'île, on l'instantie
                    if (PeutEtrePlacé(nouvellePos) && TerrainTouché)
                    {
                        GameObject objetTemp = Instantiate(tabObjets[i]);
                        objetTemp.transform.position = nouvellePos;
                        //On ajoute un collider temporaire pour empêcher les objets de se superposer, les collider se font détruire suite au placement de tous les éléments
                        AjouterColliderTemporaireGameObject(objetTemp);

                        //Donne rotation aléatoire aux objets puis un scale aléatoire pour donner de la variété
                        objetTemp.transform.up = normale;
                        objetTemp.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                        if (scaleRandom)
                            objetTemp.transform.localScale *= UnityEngine.Random.Range(1.5f, 2.5f);

                        AToucheTerrain = TerrainTouché;
                    }
                    if (compteur > loopMax)                    
                        break;
                }
            }
    }

    private void TrouverListePositionsLibres()
    {
        int nbMax = 10;
        FeuDeCamp feu = FeuDeCamp.instance;
        listePositionsLibres = new Vector3[feu.feux.Count* nbMax];
        listePositionsLibresHeal = new List<Vector3>();

        //On trouve 10 positions libres par rayon de feu
        for (int i = 0; i < feu.feux.Count - 1; i++)
        {
            for (int j = 0; j <= nbMax; j++)
            {
                float rayonMin = feu.GetRayonSelonNiveau(i - 1);
                float rayonMax = feu.GetRayonSelonNiveau(i);

                int compteur = 0;
                bool ATouchéTerrain = false;

                while (!ATouchéTerrain)
                {
                    compteur++;

                    //on trouve des coordonnees random
                    Vector2 CoordonéesXZ1 = TrouverCoordonnéeRandomXZ(rayonMin, rayonMax);
                    (float hauteur, Vector3 normale, bool TerrainTouché) = TrouverHauteur(CoordonéesXZ1.x, CoordonéesXZ1.y);
                    Vector3 nouvellePos = new(CoordonéesXZ1.x, hauteur, CoordonéesXZ1.y);

                    if (PeutEtrePlacé(nouvellePos) && TerrainTouché)
                    {
                        listePositionsLibres[i * nbMax + j] = nouvellePos;


                        CoordonéesXZ1 = TrouverCoordonnéeRandomXZ(rayonMin, rayonMax);
                        (hauteur,  normale,  TerrainTouché) = TrouverHauteur(CoordonéesXZ1.x, CoordonéesXZ1.y);
                        nouvellePos = new(CoordonéesXZ1.x, hauteur, CoordonéesXZ1.y);

                        if(PeutEtrePlacé(nouvellePos) && TerrainTouché)
                            listePositionsLibresHeal.Add(nouvellePos);

                        ATouchéTerrain = TerrainTouché;
                    }

                    if (compteur > loopMax)
                    {
                        /// Debug.Log("TrouverListePositionsLibres : NT");
                        break;
                    }
                }
            }
        }
    }

    private (int nbBois, int nbPierre, int nbFer, int nbPlastique) CalculerRessourcesNécéssaires(int niveau)
    {
        //Vérifie combien de bois/fer pour le feu
        (int nbBois, int nbPierre) = FeuDeCamp.instance.DéterminerNbRessourcesNecessaires(niveau);
        int nbFer = 0;
        int nbPlastique = 0;

        ///Ajoute le nb de ressources pour les étapes si elles sont activées et si c'est le bon niveau de feu
        for (int étape = 0; étape < (int)Étapes.nbÉtapes; étape++)
        {
            if (choixÉtapesGraphe.EstÉtapeActivée((Étapes)étape))
            {
                if (ressourcesNécesairesNiveauFeu[étape].niveauDeFeu == niveau && ressourcesNécesairesNiveauFeu[étape].cout != null)
                {
                    (int nbBoisÉtape, int nbPierreÉtape, int nbFerÉtape, int nbPlastiqueÉtape) = ExtraireNbRessources(étape);
                    nbBois += nbBoisÉtape;
                    nbPierre += nbPierreÉtape;
                    nbFer += nbFerÉtape;
                    nbPlastique += nbPlastiqueÉtape;
                }
            }
        }

        //on ajoute un petit loose pour que ce ne soit pas trop difficile ou serré
        return (nbBois + niveau / 2, nbPierre + niveau / 2, nbFer + niveau / 3, nbPlastique + niveau / 3 );
    }


    private (int nbBois, int nbPierre, int nbFer, int nbPlastique) ExtraireNbRessources(int étape)
    {
        //On extrait le Cout des étapes créés plus tôt
        Cout coutACréer = ressourcesNécesairesNiveauFeu[étape].cout;

        int bois = coutACréer.ressourcesRequises[(int)Ressource.Bois];
        int pierre = coutACréer.ressourcesRequises[(int)Ressource.Pierre];
        int fer = coutACréer.ressourcesRequises[(int)Ressource.Fer];
        int plastique = coutACréer.ressourcesRequises[(int)Ressource.Plastique];

        return (bois, pierre, fer, plastique);
    }

    private void AjouterColliderTemporaireGameObject(GameObject obj)
    {
        SphereCollider colliderTemp = obj.AddComponent<SphereCollider>();
        colliderTemp.radius = 0.5f;

        listeColliderTemporaire.Add(colliderTemp);
    }

    //Trouver un point random entre un cercleMin et un cercleMax
    static Vector2 TrouverCoordonnéeRandomXZ(float RayonMin, float RayonMax)
    {
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float distance = UnityEngine.Random.Range(RayonMin, RayonMax);

        float x = distance * Mathf.Cos(angle);
        float z = distance * Mathf.Sin(angle);

        return new Vector2(x, z);
    }

    //Trouve la hauteur de la map à un endroit précis
    static (float hauteur, Vector3 normale, bool TerrainTouché) TrouverHauteur(float positionX, float positionZ)
    {
        const float HAUTEUR_ACCEPTABLE_MAX = 100;

        // on envoie un rayon pour trouver la hauteur du terrain au point aléatoire (X, ?, Z)
        Ray ray = new Ray(new Vector3(positionX, 50f, positionZ), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, HAUTEUR_ACCEPTABLE_MAX))
            if (hit.collider.gameObject.CompareTag("Terrain"))
                return (hit.point.y, hit.normal, true);

        return (HAUTEUR_ACCEPTABLE_MAX, Vector3.zero, false);
    }

    static bool PeutEtrePlacé(Vector3 pos)
    {
        return pos.y > 1 && !(Physics.OverlapSphere(pos, 0.5f).Length > 1);
    }
}
