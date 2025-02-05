using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabsMonstres; //Les monstres sont mis dans ce tableau en ordre de difficult�

    public static EnnemySpawner instance;
    private List<GameObject> ennemisEnJeu = new List<GameObject>();

    const float MULTIPLICATEUR_FACILE = 0.6f;
    const float MULTIPLICATEUR_MOYEN = 1.3f;
    const float MULTIPLICATEUR_DIFFICILE = 2f;

    const float MULTIPLICATEUR_NOMBRE_ENNEMIS = 2;
    const float VARIATION_MAXIMALE_NOMBRE_ENNEMIS = 2; //N'inclut pas les bornes mais distribution uniforme

    const float RAYON_APPARITION_MIN = 20; //Le rayon d'apparition autour du joueur
    const float RAYON_APPARITION_MAX = 50;

    GameObject joueur;

    void Start()
    {
        if(instance == null)
            instance = this;
        joueur = GameObject.FindGameObjectWithTag("Player");
    }


    int essaisRat�sDepuisDerni�reApparitionEnnemi = 0;
    const int LIMITE_ESSAIS = 30;
    //Chaque nuit il y a 3 vagues d'ennemis (vague sera 1, 2 ou 3)
    public void SpawnVague(int jour, int vague)
    {
        if(InfoPartie.difficult� != Difficult�.paisible)
        {
            int nombreEnnemis = D�terminerNombreEnnemis(jour, vague);
            for (int i = 0; i < nombreEnnemis; i++)
            {
                SpawnEnnemi(jour, vague);
            }
        }
    }

    private void SpawnEnnemi(int jour, int vague)
    {
        int indiceEnnemi = D�terminerIndiceEnnemi(jour, vague);
        GameObject ennemiCr�� = Instantiate(prefabsMonstres[indiceEnnemi], TrouverPositionEnnemi(), Quaternion.identity);
        ennemiCr��.SetActive(true);
        //Ok au d�but je changeais la position de l'ennemi apr�s l'avoir instanti� et il semblait y avoir des probl�mes avec le navMesh,
        //c'est pourquoi j'ai cr�� cette v�rification, mais elle semble maintenant inutile
        if (!ennemiCr��.GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            Destroy(ennemiCr��);
            essaisRat�sDepuisDerni�reApparitionEnnemi++;
            if(essaisRat�sDepuisDerni�reApparitionEnnemi < LIMITE_ESSAIS)
            {
                SpawnEnnemi(jour, vague);
            }
        }
        else
        {
            ennemisEnJeu.Add(ennemiCr��);
        }
    }

    private int D�terminerNombreEnnemis(int jour, int vague)
    {
        float multiplicateurDifficult� = MULTIPLICATEUR_FACILE;
        if(InfoPartie.difficult� == Difficult�.mod�r�e)
        {
            multiplicateurDifficult� = MULTIPLICATEUR_MOYEN;
        }
        else if(InfoPartie.difficult� == Difficult�.difficile)
        {
            multiplicateurDifficult� = MULTIPLICATEUR_DIFFICILE;
        }
        float ennemisMoyenPourJour = Mathf.Sqrt(jour) * MULTIPLICATEUR_NOMBRE_ENNEMIS * multiplicateurDifficult�;
        float facteurPourVague = 1 / Mathf.Sqrt(vague);
        //float variationAl�atoire = Random.Range(-VARIATION_MAXIMALE_NOMBRE_ENNEMIS, VARIATION_MAXIMALE_NOMBRE_ENNEMIS);

        return Mathf.RoundToInt(ennemisMoyenPourJour * facteurPourVague);
    }

    //L'indice retourn� doit �tre entre 0 et 9 inclusivement (10 ennemis cr��s pour l'instant)
    //Plus l'indice est �lev�, plus l'ennemi est difficile � vaincre
    GaussianRandom randomNormal = new GaussianRandom();
    private int D�terminerIndiceEnnemi(int jour, int vague)
    {
        float indiceMoyen = vague - 1 + (jour - 1) * 2;
        indiceMoyen = Mathf.Clamp(indiceMoyen, 0, 7);//Je vais restreindre la moyenne � 7 pour qu'il y ait toujours de la vari�t�
        float �cartType = indiceMoyen / 2;
        float indiceFloat = Mathf.Min((float)randomNormal.NextGaussian(indiceMoyen, �cartType), indiceMoyen + 2);

        return Mathf.Clamp(Mathf.RoundToInt(indiceFloat), 0, prefabsMonstres.Length - 1);
    }
    //Le code qui suit vient d'internet parce que unity ne propose pas de fonction
    //pour impl�menter une distribution normale
    internal class GaussianRandom
    {
        private System.Random random;
        private bool hasDeviate;
        private double storedDeviate;

        public GaussianRandom()
        {
            random = new System.Random();
            hasDeviate = false;
            storedDeviate = 0;
        }

        public double NextGaussian(double mu = 0, double sigma = 1)
        {
            if (hasDeviate)
            {
                hasDeviate = false;
                return storedDeviate * sigma + mu;
            }
            else
            {
                double v1, v2, s;
                do
                {
                    v1 = 2.0 * random.NextDouble() - 1.0;
                    v2 = 2.0 * random.NextDouble() - 1.0;
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1.0 || s == 0);

                double multiplier = Math.Sqrt(-2.0 * Math.Log(s) / s);
                storedDeviate = v2 * multiplier;
                hasDeviate = true;
                return mu + sigma * v1 * multiplier;
            }
        }
    }
    public void TuerTousEnnemis()
    {
        foreach(GameObject ennemi in ennemisEnJeu)
        {
            ennemi.GetComponent<ComportementEnnemi>().Mourir();
        }
        ennemisEnJeu.Clear();
    }

    public void RetirerEnnemiDeListe(GameObject ennemi)
    {
        ennemisEnJeu.Remove(ennemi);
    }


    private Vector3 TrouverPositionEnnemi()
    {
        const int LOOP_MAX = 100;
        const float HAUTEUR_ENNEMIS = 1f; //On fait apparaitre les ennemis plus haut pour laisser la physique les ramener sur le navMesh
        int compteur = 0;

        while (compteur <= LOOP_MAX)
        {
            compteur++;
            Vector2 Coordon�esXZ = TrouverCoordonn�eRandomXZ(RAYON_APPARITION_MIN, RAYON_APPARITION_MAX, joueur.transform.position);
            (float hauteur, bool TerrainTouch�) hitInfo = TrouverHauteur(Coordon�esXZ.x, Coordon�esXZ.y);

            if (hitInfo.TerrainTouch�)
            {
                return new Vector3(Coordon�esXZ.x, hitInfo.hauteur + HAUTEUR_ENNEMIS, Coordon�esXZ.y);
            }
        }
        Debug.Log("PlacerEnnemi : NT");
        return Vector3.zero; //On ne devrait jamais arriver ici
    }

    static (float hauteur, bool TerrainTouch�) TrouverHauteur(float positionX, float positionZ)
    {
        const float HAUTEUR_ACCEPTABLE_MAX = 100;

        // on envoie un rayon pour trouver la hauteur du terrain au point al�atoire (X, ?, Z)
        Ray ray = new Ray(new Vector3(positionX, 50f, positionZ), Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, HAUTEUR_ACCEPTABLE_MAX))
        {
            if (hit.collider.gameObject.CompareTag("Terrain"))
            {
                return (hit.point.y, true);
            }
        }

        return (HAUTEUR_ACCEPTABLE_MAX, false);
    }

    static Vector2 TrouverCoordonn�eRandomXZ(float RayonMin, float RayonMax, Vector3 centre)
    {
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float distance = UnityEngine.Random.Range(RayonMin, RayonMax);
        float x = distance * Mathf.Cos(angle) + centre.x;
        float z = distance * Mathf.Sin(angle) + centre.z;

        return new Vector2(x, z);
    }

}
