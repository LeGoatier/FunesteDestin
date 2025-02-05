using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Placement : MonoBehaviour
{
    //Instanciation des variables
    #region
    [SerializeField] GameObject Joueur;

    [SerializeField] GameObject[] tableauStructure; //Ce tableau contient toutes les structures dans le d�sordre (prefabs)
    GameObject[] tableauStructuresInstanci�es;
    List<SphereCollider> listeColliderTemporaire = new List<SphereCollider>();

    private List<GameObject> structuresChoisies = new List<GameObject>();
    [SerializeField] GameObject[] listeLivret;
    [SerializeField] GameObject[] ListeFusil;

    public event EventHandler OnStructureInstanci�;
    const float loopMax = 100;

    //Elements nature
    [SerializeField] GameObject[] ListeNature;
    [SerializeField] int[] densit�sNature;

    //Ressources
    [SerializeField] GameObject[] RessourcesRamassables;
    [SerializeField] GameObject[] Heal;
    public Vector3[] listePositionsLibres;
    public List<Vector3> listePositionsLibresHeal;
    float timerRegeneration;

    public static Vector3 VecteurZeroWorld;

    Gen_Ile MapGenerator;

    //Ressources � faire appara�tre en fonction des n�cessit�s selon les �tapes activ�es
    private (Cout cout, int niveauDeFeu)[] ressourcesN�cesairesNiveauFeu = new (Cout, int)[(int)�tapes.nb�tapes];

    Choix�tapesGraphe choix�tapesGraphe;

    #endregion
    void Awake()
    {
        InitialiserCout�tapes();
        MapGenerator = GameObject.Find("MapGenerator").GetComponent<Gen_Ile>();
        MapGenerator.OnG�n�rationTerrainFini += Placer�l�ments;
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
  
    //Cette m�thode est appel�e dans le Awake() et elle associe un Cout � chaque �tape. La classe Cout (impl�ment�e par Justin) permet de garder en m�moire le nombre de chaque ressource n�c�ssaire � chaque �tape.
    //Le nombre de ressources n�c�ssaires � chaque �tape est �tabli dans un document word que toute l��quipe a acc�s �. Ainsi, Le nombre de ressources plac�es est le m�me que le nombre de ressources n�c�ssaires � la fuite.
    //Chaque �tape a un niveau de feu qui lui est associ�. Les ressources associ�es � l��tape se feront placer au niveau de feu ad�quat pour permettre de bien �taler les ressources sur l��le.


    private void InitialiserCout�tapes()
    {
        Cout cout�tabli = new();
        cout�tabli.AjouterRessource(Ressource.Bois, 2);
        cout�tabli.AjouterRessource(Ressource.Fer, 1);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.�tabli].cout = cout�tabli;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.�tabli].niveauDeFeu = 1;

        Cout coutMarteau = new();
        coutMarteau.AjouterRessource(Ressource.Bois, 1);
        coutMarteau.AjouterRessource(Ressource.Fer, 1);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Marteau].cout = coutMarteau;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Marteau].niveauDeFeu = 1;

        Cout coutAbri = new();
        coutAbri.AjouterRessource(Ressource.Bois, 8);
        coutAbri.AjouterRessource(Ressource.Pierre, 4);
        coutAbri.AjouterRessource(Ressource.Plastique, 3);
        coutAbri.AjouterRessource(Ressource.Fer, 7);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Abri].cout = coutAbri;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Abri].niveauDeFeu = 2;

        Cout coutMoteur = new();
        coutMoteur.AjouterRessource(Ressource.Fer, 6);
        coutMoteur.AjouterRessource(Ressource.Plastique, 6);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Moteur].cout = coutMoteur;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Moteur].niveauDeFeu = 3;

        Cout coutRadio = new();
        coutRadio.AjouterRessource(Ressource.Fer, 6);
        coutRadio.AjouterRessource(Ressource.Plastique, 8);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Radio].cout = coutRadio;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Radio].niveauDeFeu = 4;

        Cout coutFus�eD�tresse = new();
        coutFus�eD�tresse.AjouterRessource(Ressource.Plastique, 2);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Fus�eD�tresse].cout = coutFus�eD�tresse;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Fus�eD�tresse].niveauDeFeu = 5;

        Cout coutBateau = new();
        coutBateau.AjouterRessource(Ressource.Bois, 16);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Bateau].cout = coutBateau;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Bateau].niveauDeFeu = 5;

        Cout coutAntenne = new();
        coutAntenne.AjouterRessource(Ressource.Fer, 4);
        coutAntenne.AjouterRessource(Ressource.Plastique, 4);
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Antenne].cout = coutAntenne;
        ressourcesN�cesairesNiveauFeu[(int)�tapes.Antenne].niveauDeFeu = 6;

    }

    void Placer�l�ments(object sender, EventArgs eventArgs)
    {
        Joueur.transform.position = new Vector3(0, 100, 0); //d�placement du joueur pour ne pas qu'il bloque un rayon :)
        InitialiserVecteurZero();

        tableauStructuresInstanci�es = InstancierStructures();
        D�terminer�tapesActiv�es();

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
        (float hauteur, Vector3 normale, bool TerrainTouch�) = TrouverHauteur(0, 0);
        VecteurZeroWorld = new Vector3(0, hauteur, 0);
    }

    private void RennaissanceRessources()
    {
        int niveauJoueur = FeuDeCamp.instance.niveauActuel;
        int[] nb�l�ments�Instancier = new int[4] { 2, 0, 0, 0 };

        switch (niveauJoueur)
        {
            case 2:
                nb�l�ments�Instancier = new int[]{ 2, 1, 1, 0};
                break;
            case 3:
                nb�l�ments�Instancier = new int[] { 3, 2, 1, 0};
                break;
            case 4:
                nb�l�ments�Instancier = new int[] { 3, 2, 2, 0 };
                break;
            case > 4:
                nb�l�ments�Instancier = new int[]{ 3, 2, 2, 1 };
                break;
        }

        int indexD�part = niveauJoueur * 10;
        for (int i = 0; i < nb�l�ments�Instancier.Length; i++)
        {
            for (int j = 0; j < nb�l�ments�Instancier[i]; j++)
            {
                int index = indexD�part + (i > 0 ? nb�l�ments�Instancier[0] : 0) + (i > 1 ? nb�l�ments�Instancier[1] : 0) + (i > 2 ? nb�l�ments�Instancier[2] : 0) + (i > 3 ? nb�l�ments�Instancier[3] : 0) + j;
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
        //Apr�s graphe justin: pour chaque niveau => on boucle dans se code pour chaque niveau => compStrcut.niveau est remplac� par niveau de la boucle
        GameObject[] listInstanceStructure = structuresChoisies.ToArray();

        //List<GameObject> listOrdonn�e = OrdonnerListe(listInstanceStructure);
        List<GameObject> positionFixe = ListePositionFixe(listInstanceStructure);
        List<GameObject> positionCondition = ListePositionConditionelle(listInstanceStructure);
        List<GameObject> positionAutre = ListePositionAutre(listInstanceStructure);

        //Placer ceux-ci sans collider pour �viter une boucle infini
        foreach (GameObject struc in positionFixe)
        {
            ComportementStructure compStruct = struc.GetComponent<ComportementStructure>();
            Placer�tatPositionFixe(struc, compStruct);
        }
        foreach (GameObject struc in positionCondition)
        {
            ComportementStructure compStruct = struc.GetComponent<ComportementStructure>();
            Placer�tatPositionCondition(struc, compStruct);
        }

        AjouterColliderTemporaireListe(positionFixe);
        AjouterColliderTemporaireListe(positionCondition);

        //AutreObjet
        foreach (GameObject struc in positionAutre)
        {
            ComportementStructure compStruct = struc.GetComponent<ComportementStructure>();
            �tatInitialisation �tat = compStruct.�tatInitialisation;

            switch (�tat)
            {
                case �tatInitialisation.positionBorn�:
                    Placer�tatPositionBorn�(struc, compStruct.RayonMin, compStruct.RayonMax, true);
                    break;
                case �tatInitialisation.positionNiveau:
                    //marche que si le feu � �t� instanci� avant (en th�orie le feu est le premier �l�ment de la liste structure
                    Placer�tatPositionNiveau(struc, compStruct.NiveauSpawn);
                    break;
                default:
                    Debug.Log("�tat non-trouv� : " + struc.name);
                    break;
            }

            SphereCollider colliderTemp = struc.AddComponent<SphereCollider>();
            colliderTemp.radius = 3;
            listeColliderTemporaire.Add(colliderTemp);
        }
        OnStructureInstanci�?.Invoke(this, EventArgs.Empty);
    }

    void D�terminer�tapesActiv�es()
    {
        choix�tapesGraphe = new();
        foreach (GameObject structure in tableauStructuresInstanci�es)
        {
            if (choix�tapesGraphe.Est�tapeActiv�e(structure.GetComponent<ComportementStructure>().�tapeLi�e))
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
        //Les objects avec une position fixe s'instancie en permier et doit �tre au niveau 0
        //position conditionnelle (doit �tre au dela du rayon de niveau 0) et le reste apres
        List<GameObject> listeOrdonn�e = new List<GameObject>();

        listeOrdonn�e.AddRange(tableau.Where(obj => obj.GetComponent<ComportementStructure>().�tatInitialisation == �tatInitialisation.positionFixe).ToList());
        listeOrdonn�e.AddRange(tableau.Where(obj => obj.GetComponent<ComportementStructure>().�tatInitialisation == �tatInitialisation.positionCondition).ToList());
        listeOrdonn�e.AddRange(tableau.Where(obj => obj.GetComponent<ComportementStructure>().�tatInitialisation != �tatInitialisation.positionCondition && obj.GetComponent<ComportementStructure>().�tatInitialisation != �tatInitialisation.positionCondition).ToList());

        return listeOrdonn�e;
    }
    List<GameObject> ListePositionFixe(GameObject[] tableau)
    {
        return tableau.Where(obj => obj.GetComponent<ComportementStructure>().�tatInitialisation == �tatInitialisation.positionFixe).ToList();
    }
    List<GameObject> ListePositionConditionelle(GameObject[] tableau)
    {
        return tableau.Where(obj => obj.GetComponent<ComportementStructure>().�tatInitialisation == �tatInitialisation.positionCondition).ToList();
    }
    List<GameObject> ListePositionAutre(GameObject[] tableau)
    {
        return tableau.Where(obj => obj.GetComponent<ComportementStructure>().�tatInitialisation != �tatInitialisation.positionFixe && obj.GetComponent<ComportementStructure>().�tatInitialisation != �tatInitialisation.positionCondition).ToList();
    }

    static void Placer�tatPositionFixe(GameObject structure, ComportementStructure comp)
    {
        (float hauteur, Vector3 normale, bool TerrainTouch�) = TrouverHauteur(comp.Position.x, comp.Position.y);
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
    static void Placer�tatPositionCondition(GameObject structure, ComportementStructure comp)
    {
        List<Vector3> positionsPossibles = comp.ConditionSpawn();
        int indexAl�atoire = UnityEngine.Random.Range(0, positionsPossibles.Count - 1);
        Vector3 PositionAl�aloire = positionsPossibles[indexAl�atoire];

        (float hauteur, Vector3 normale, bool TerrainTouch�) = TrouverHauteur(PositionAl�aloire.x, PositionAl�aloire.z);

        structure.transform.position = new Vector3(PositionAl�aloire.x, hauteur, PositionAl�aloire.z);
        structure.transform.up = normale;

        if (comp.�tapeLi�e == �tapes.Antenne)
        {
            structure.transform.rotation = Quaternion.identity;
        }
        else if(comp.�tapeLi�e == �tapes.Bateau)
        {
            Vector3 position = structure.transform.position + (Vector3.zero - structure.transform.position).normalized * 4;
            (hauteur, normale, TerrainTouch�) = TrouverHauteur(position.x, position.z);
            position.y = hauteur;
            structure.GetComponent<Bateau>().InstancierLivreBateau(position, normale);
        }
    }
    static void Placer�tatPositionBorn�(GameObject structure, float rayonMin, float rayonMax, bool estPerpendiculaireSol)
    {
        int compteur = 0;
        bool ATouch�Terrain = false;

        while (!ATouch�Terrain)
        {
            compteur++;
            Vector2 Coordon�esXZ = TrouverCoordonn�eRandomXZ(rayonMin, rayonMax);
            (float hauteur, Vector3 normale, bool TerrainTouch�) hitInfo = TrouverHauteur(Coordon�esXZ.x, Coordon�esXZ.y);

            if (hitInfo.TerrainTouch� && hitInfo.hauteur > 0.5f)
            {
                structure.transform.position = new Vector3(Coordon�esXZ.x, hitInfo.hauteur, Coordon�esXZ.y);
                structure.transform.LookAt(VecteurZeroWorld, hitInfo.normale);

                if (!estPerpendiculaireSol)
                {
                    structure.transform.right = hitInfo.normale;
                }


                ATouch�Terrain = true;
            }
            if (compteur > loopMax)
            {
                Debug.Log("Placer�tatPositionBorn� : NT");
                break;
            }
        }
    }

    void Placer�tatPositionNiveau(GameObject structure, int niveau)
    {
        int compteur = 0;
        bool ATouch�Terrain = false;

        while (!ATouch�Terrain)
        {
            compteur++;

            Vector2 Coordon�esXZ = TrouverCoordonn�eRandomXZ(FeuDeCamp.instance.GetRayonSelonNiveau(niveau - 1), FeuDeCamp.instance.GetRayonSelonNiveau(niveau));
            (float hauteur, Vector3 normale, bool TerrainTouch�) hitInfo = TrouverHauteur(Coordon�esXZ.x, Coordon�esXZ.y);

            if (hitInfo.TerrainTouch� && hitInfo.hauteur > 0.5f)
            {
                structure.transform.position = new Vector3(Coordon�esXZ.x, hitInfo.hauteur, Coordon�esXZ.y);
                structure.transform.LookAt(VecteurZeroWorld, hitInfo.normale);


                ATouch�Terrain = true;
            }
            if (compteur > loopMax)
            {
                Debug.Log("Placer�tatPositionNiveau : NT");
                break;
            }
        }
    }

    #endregion
    //---Structures-(fin)-------------------------------------------------------------

    void PlacerLivret()
    {
        var livret1 = Instantiate(listeLivret[0]);
        Placer�tatPositionBorn�(livret1, 3, 9, true);

        var livret2 = Instantiate(listeLivret[1]);
        Placer�tatPositionBorn�(livret2, FeuDeCamp.instance.GetRayonSelonNiveau(0), FeuDeCamp.instance.GetRayonSelonNiveau(1), true);

        var livret3 = Instantiate(listeLivret[2]);
        Placer�tatPositionBorn�(livret3, FeuDeCamp.instance.GetRayonSelonNiveau(2), FeuDeCamp.instance.GetRayonSelonNiveau(3), true);
    }

    void PlacerFusils()
    {
        foreach (GameObject fusil in ListeFusil)
        {
            InteragirFusils scriptfusil = fusil.GetComponent<InteragirFusils>();

            if (scriptfusil.TypeArme == Arme.Arbal�te)
            {
                var instance = Instantiate(fusil);
                Placer�tatPositionBorn�(instance, 0, 5, false);
                transform.position += new Vector3(0, 0.2f, 0);


            }
            else if (scriptfusil.TypeArme == Arme.Pompe)
            {
                var instance = Instantiate(fusil);
                Placer�tatPositionBorn�(instance, 15, 30, false);
                transform.position += new Vector3(0, 0.2f, 0);
            }
        }
    }

    void PlacerRessources()
    {
        FeuDeCamp feu = FeuDeCamp.instance;

        // Boucle � travers les niveau du feu pour placer les ressources � chaque niveau de feu
        for (int niveau = 0; niveau < feu.feux.Count; niveau++)
        {
            float rayonMin = feu.GetRayonSelonNiveau(niveau - 1);
            float rayonMax = feu.GetRayonSelonNiveau(niveau);

            // D�termine le nombre de chaque ressources � placer � chaque rayon de feu
            (int nbBois, int nbPierre, int nbFer, int nbPlastique) = CalculerRessourcesN�c�ssaires(niveau);
            int[] nb�l�ments�Instancier = new int[] { nbBois, nbPierre, nbFer, nbPlastique, 0 };

            //Instancie les ressources ramassables selon les rayons min et max
            Instancier�l�ments(RessourcesRamassables, nb�l�ments�Instancier, rayonMin, rayonMax, false);
        }
    }

    void PlacerHeal()
    {
        int nbChamp = 20;
        int NbTrousse = 5;
        int[] nbheal = new int[] { nbChamp, NbTrousse };
        Instancier�l�ments(Heal, nbheal, 10, 75, false);
    }


    void PlacerNature()
    {
        float RAYON_MIN = 12;
        float RAYON_MAX = 200;

        Instancier�l�ments(ListeNature, densit�sNature, RAYON_MIN, RAYON_MAX, true);
    }

    void Instancier�l�ments(GameObject[] tabObjets, int[] nbElements, float rayonMin, float rayonMax, bool scaleRandom)
    {
        for (int i = 0; i < tabObjets.Length; i++)
            for (int j = 0; j < nbElements[i]; j++)
            {
                int compteur = 0;
                bool AToucheTerrain = false;

                while (!AToucheTerrain)
                {
                    compteur++;

                    //on trouve des coordonn�es al�atoires selon les rayons puis la hauteur
                    Vector2 Coordon�esXZ = TrouverCoordonn�eRandomXZ(rayonMin, rayonMax);
                    (float hauteur, Vector3 normale, bool TerrainTouch�) = TrouverHauteur(Coordon�esXZ.x, Coordon�esXZ.y);
                    Vector3 nouvellePos = new(Coordon�esXZ.x, hauteur, Coordon�esXZ.y);

                    //Si l'objet est plus haut que 1 et ne touche rien d'autre et est sur l'�le, on l'instantie
                    if (PeutEtrePlac�(nouvellePos) && TerrainTouch�)
                    {
                        GameObject objetTemp = Instantiate(tabObjets[i]);
                        objetTemp.transform.position = nouvellePos;
                        //On ajoute un collider temporaire pour emp�cher les objets de se superposer, les collider se font d�truire suite au placement de tous les �l�ments
                        AjouterColliderTemporaireGameObject(objetTemp);

                        //Donne rotation al�atoire aux objets puis un scale al�atoire pour donner de la vari�t�
                        objetTemp.transform.up = normale;
                        objetTemp.transform.rotation = Quaternion.Euler(0, UnityEngine.Random.Range(0, 360), 0);
                        if (scaleRandom)
                            objetTemp.transform.localScale *= UnityEngine.Random.Range(1.5f, 2.5f);

                        AToucheTerrain = TerrainTouch�;
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
                bool ATouch�Terrain = false;

                while (!ATouch�Terrain)
                {
                    compteur++;

                    //on trouve des coordonnees random
                    Vector2 Coordon�esXZ1 = TrouverCoordonn�eRandomXZ(rayonMin, rayonMax);
                    (float hauteur, Vector3 normale, bool TerrainTouch�) = TrouverHauteur(Coordon�esXZ1.x, Coordon�esXZ1.y);
                    Vector3 nouvellePos = new(Coordon�esXZ1.x, hauteur, Coordon�esXZ1.y);

                    if (PeutEtrePlac�(nouvellePos) && TerrainTouch�)
                    {
                        listePositionsLibres[i * nbMax + j] = nouvellePos;


                        Coordon�esXZ1 = TrouverCoordonn�eRandomXZ(rayonMin, rayonMax);
                        (hauteur,  normale,  TerrainTouch�) = TrouverHauteur(Coordon�esXZ1.x, Coordon�esXZ1.y);
                        nouvellePos = new(Coordon�esXZ1.x, hauteur, Coordon�esXZ1.y);

                        if(PeutEtrePlac�(nouvellePos) && TerrainTouch�)
                            listePositionsLibresHeal.Add(nouvellePos);

                        ATouch�Terrain = TerrainTouch�;
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

    private (int nbBois, int nbPierre, int nbFer, int nbPlastique) CalculerRessourcesN�c�ssaires(int niveau)
    {
        //V�rifie combien de bois/fer pour le feu
        (int nbBois, int nbPierre) = FeuDeCamp.instance.D�terminerNbRessourcesNecessaires(niveau);
        int nbFer = 0;
        int nbPlastique = 0;

        ///Ajoute le nb de ressources pour les �tapes si elles sont activ�es et si c'est le bon niveau de feu
        for (int �tape = 0; �tape < (int)�tapes.nb�tapes; �tape++)
        {
            if (choix�tapesGraphe.Est�tapeActiv�e((�tapes)�tape))
            {
                if (ressourcesN�cesairesNiveauFeu[�tape].niveauDeFeu == niveau && ressourcesN�cesairesNiveauFeu[�tape].cout != null)
                {
                    (int nbBois�tape, int nbPierre�tape, int nbFer�tape, int nbPlastique�tape) = ExtraireNbRessources(�tape);
                    nbBois += nbBois�tape;
                    nbPierre += nbPierre�tape;
                    nbFer += nbFer�tape;
                    nbPlastique += nbPlastique�tape;
                }
            }
        }

        //on ajoute un petit loose pour que ce ne soit pas trop difficile ou serr�
        return (nbBois + niveau / 2, nbPierre + niveau / 2, nbFer + niveau / 3, nbPlastique + niveau / 3 );
    }


    private (int nbBois, int nbPierre, int nbFer, int nbPlastique) ExtraireNbRessources(int �tape)
    {
        //On extrait le Cout des �tapes cr��s plus t�t
        Cout coutACr�er = ressourcesN�cesairesNiveauFeu[�tape].cout;

        int bois = coutACr�er.ressourcesRequises[(int)Ressource.Bois];
        int pierre = coutACr�er.ressourcesRequises[(int)Ressource.Pierre];
        int fer = coutACr�er.ressourcesRequises[(int)Ressource.Fer];
        int plastique = coutACr�er.ressourcesRequises[(int)Ressource.Plastique];

        return (bois, pierre, fer, plastique);
    }

    private void AjouterColliderTemporaireGameObject(GameObject obj)
    {
        SphereCollider colliderTemp = obj.AddComponent<SphereCollider>();
        colliderTemp.radius = 0.5f;

        listeColliderTemporaire.Add(colliderTemp);
    }

    //Trouver un point random entre un cercleMin et un cercleMax
    static Vector2 TrouverCoordonn�eRandomXZ(float RayonMin, float RayonMax)
    {
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float distance = UnityEngine.Random.Range(RayonMin, RayonMax);

        float x = distance * Mathf.Cos(angle);
        float z = distance * Mathf.Sin(angle);

        return new Vector2(x, z);
    }

    //Trouve la hauteur de la map � un endroit pr�cis
    static (float hauteur, Vector3 normale, bool TerrainTouch�) TrouverHauteur(float positionX, float positionZ)
    {
        const float HAUTEUR_ACCEPTABLE_MAX = 100;

        // on envoie un rayon pour trouver la hauteur du terrain au point al�atoire (X, ?, Z)
        Ray ray = new Ray(new Vector3(positionX, 50f, positionZ), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, HAUTEUR_ACCEPTABLE_MAX))
            if (hit.collider.gameObject.CompareTag("Terrain"))
                return (hit.point.y, hit.normal, true);

        return (HAUTEUR_ACCEPTABLE_MAX, Vector3.zero, false);
    }

    static bool PeutEtrePlac�(Vector3 pos)
    {
        return pos.y > 1 && !(Physics.OverlapSphere(pos, 0.5f).Length > 1);
    }
}
