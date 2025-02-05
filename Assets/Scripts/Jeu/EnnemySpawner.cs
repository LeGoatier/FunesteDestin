using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class EnnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject[] prefabsMonstres; //Les monstres sont mis dans ce tableau en ordre de difficulté

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


    int essaisRatésDepuisDernièreApparitionEnnemi = 0;
    const int LIMITE_ESSAIS = 30;
    //Chaque nuit il y a 3 vagues d'ennemis (vague sera 1, 2 ou 3)
    public void SpawnVague(int jour, int vague)
    {
        if(InfoPartie.difficulté != Difficulté.paisible)
        {
            int nombreEnnemis = DéterminerNombreEnnemis(jour, vague);
            for (int i = 0; i < nombreEnnemis; i++)
            {
                SpawnEnnemi(jour, vague);
            }
        }
    }

    private void SpawnEnnemi(int jour, int vague)
    {
        int indiceEnnemi = DéterminerIndiceEnnemi(jour, vague);
        GameObject ennemiCréé = Instantiate(prefabsMonstres[indiceEnnemi], TrouverPositionEnnemi(), Quaternion.identity);
        ennemiCréé.SetActive(true);
        //Ok au début je changeais la position de l'ennemi après l'avoir instantié et il semblait y avoir des problèmes avec le navMesh,
        //c'est pourquoi j'ai créé cette vérification, mais elle semble maintenant inutile
        if (!ennemiCréé.GetComponent<NavMeshAgent>().isOnNavMesh)
        {
            Destroy(ennemiCréé);
            essaisRatésDepuisDernièreApparitionEnnemi++;
            if(essaisRatésDepuisDernièreApparitionEnnemi < LIMITE_ESSAIS)
            {
                SpawnEnnemi(jour, vague);
            }
        }
        else
        {
            ennemisEnJeu.Add(ennemiCréé);
        }
    }

    private int DéterminerNombreEnnemis(int jour, int vague)
    {
        float multiplicateurDifficulté = MULTIPLICATEUR_FACILE;
        if(InfoPartie.difficulté == Difficulté.modérée)
        {
            multiplicateurDifficulté = MULTIPLICATEUR_MOYEN;
        }
        else if(InfoPartie.difficulté == Difficulté.difficile)
        {
            multiplicateurDifficulté = MULTIPLICATEUR_DIFFICILE;
        }
        float ennemisMoyenPourJour = Mathf.Sqrt(jour) * MULTIPLICATEUR_NOMBRE_ENNEMIS * multiplicateurDifficulté;
        float facteurPourVague = 1 / Mathf.Sqrt(vague);
        //float variationAléatoire = Random.Range(-VARIATION_MAXIMALE_NOMBRE_ENNEMIS, VARIATION_MAXIMALE_NOMBRE_ENNEMIS);

        return Mathf.RoundToInt(ennemisMoyenPourJour * facteurPourVague);
    }

    //L'indice retourné doit être entre 0 et 9 inclusivement (10 ennemis créés pour l'instant)
    //Plus l'indice est élevé, plus l'ennemi est difficile à vaincre
    GaussianRandom randomNormal = new GaussianRandom();
    private int DéterminerIndiceEnnemi(int jour, int vague)
    {
        float indiceMoyen = vague - 1 + (jour - 1) * 2;
        indiceMoyen = Mathf.Clamp(indiceMoyen, 0, 7);//Je vais restreindre la moyenne à 7 pour qu'il y ait toujours de la variété
        float écartType = indiceMoyen / 2;
        float indiceFloat = Mathf.Min((float)randomNormal.NextGaussian(indiceMoyen, écartType), indiceMoyen + 2);

        return Mathf.Clamp(Mathf.RoundToInt(indiceFloat), 0, prefabsMonstres.Length - 1);
    }
    //Le code qui suit vient d'internet parce que unity ne propose pas de fonction
    //pour implémenter une distribution normale
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
            Vector2 CoordonéesXZ = TrouverCoordonnéeRandomXZ(RAYON_APPARITION_MIN, RAYON_APPARITION_MAX, joueur.transform.position);
            (float hauteur, bool TerrainTouché) hitInfo = TrouverHauteur(CoordonéesXZ.x, CoordonéesXZ.y);

            if (hitInfo.TerrainTouché)
            {
                return new Vector3(CoordonéesXZ.x, hitInfo.hauteur + HAUTEUR_ENNEMIS, CoordonéesXZ.y);
            }
        }
        Debug.Log("PlacerEnnemi : NT");
        return Vector3.zero; //On ne devrait jamais arriver ici
    }

    static (float hauteur, bool TerrainTouché) TrouverHauteur(float positionX, float positionZ)
    {
        const float HAUTEUR_ACCEPTABLE_MAX = 100;

        // on envoie un rayon pour trouver la hauteur du terrain au point aléatoire (X, ?, Z)
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

    static Vector2 TrouverCoordonnéeRandomXZ(float RayonMin, float RayonMax, Vector3 centre)
    {
        float angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        float distance = UnityEngine.Random.Range(RayonMin, RayonMax);
        float x = distance * Mathf.Cos(angle) + centre.x;
        float z = distance * Mathf.Sin(angle) + centre.z;

        return new Vector2(x, z);
    }

}
