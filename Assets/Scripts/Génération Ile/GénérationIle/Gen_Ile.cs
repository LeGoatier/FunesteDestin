using System;
using System.Collections.Generic;
using UnityEngine;

public class Gen_Ile : MonoBehaviour
{
    [SerializeField] int nbPointsCote;
    [SerializeField] public float taille;
    [SerializeField] float grandeurBruit;
    [SerializeField] int octaves;
    [Range(0, 1)]
    [SerializeField] float persistance;
    [SerializeField] float lacunarit�;
    [SerializeField] Vector2Int d�calageOctaves;
    [SerializeField] AnimationCurve courbe;
    [SerializeField] float amplitude;
    [SerializeField] public int seed;
    [SerializeField] GameObject eau;


    static Vector3 PlusHaut;
    static List<Vector3> bordDeLeau;

    float[,] cartePerlin;

    Mesh mesh;
    MeshFilter meshFilter;
    MeshCollider meshCollider;


    public EventHandler OnG�n�rationTerrainFini;

    private void Awake()
    {
        bordDeLeau = new List<Vector3>();
    }

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        meshCollider = GetComponent<MeshCollider>();

        G�n�rerMap();
    }

    public void G�n�rerMap()
    {
        // G�n�re la carte de Perlin
        cartePerlin = BruitPerlin.G�n�rerMatrice(nbPointsCote, grandeurBruit, octaves, persistance, lacunarit�, d�calageOctaves);

        // G�n�re le mesh et le collider en utilisant la carte de Perlin
        mesh = G�n�rerMesh(cartePerlin, amplitude, taille, courbe);
        meshFilter.mesh = mesh;
        meshCollider.sharedMesh = mesh;

        // Appelle l'�v�nement pour signaler que la g�n�ration du terrain est termin�e
        OnG�n�rationTerrainFini?.Invoke(this, EventArgs.Empty);

        Instantiate(eau, new Vector3(0, 0, 0), Quaternion.identity);

        // Recalcule la surface du NavMesh
        GestionNavMesh.BakeSurface();
    }

    // G�n�re un mesh en fonction de la carte de Perlin et des param�tres
    public static Mesh G�n�rerMesh(float[,] cartePerlin, float amplitude, float taille, AnimationCurve courbe)
    {
        int dimensions = cartePerlin.GetLength(0);
        float delta = taille / dimensions;
        float dUvs = 1 / (float)dimensions;

        //Tableau des sommets
        Vector3[] sommets = new Vector3[(int)Mathf.Pow(dimensions, 2)];

        // Tableau des UVS
        Vector2[] uvs = new Vector2[(int)Mathf.Pow(dimensions, 2)];

        //Nombre de triangles total
        List<int> triangles = new();
        Mesh tempMesh = new();

        for (int z = 0; z < dimensions; z++)
        {
            for (int x = 0; x < dimensions; x++)
            {
                int i = z * dimensions + x;

                // D�termine les sommets en fonction de la carte de Perlin et de l'amplitude
                sommets[i] = new Vector3(x * delta - taille / 2, courbe.Evaluate(cartePerlin[x, z]) * amplitude, z * delta - taille / 2);

                // Classe les points pour l'eau
                ClasserPoints(sommets[i]);

                // Ici on d�termine les uvs
                uvs[i] = new Vector2(x * dUvs, z * dUvs);

                // Ici on d�termine les triangles pour former les faces du mesh
                if (x < dimensions - 1 && z < dimensions - 1)
                {
                    triangles.Add(z * dimensions + x);
                    triangles.Add((z + 1) * dimensions + x);
                    triangles.Add(z * dimensions + x + 1);

                    triangles.Add(z * dimensions + x + 1);
                    triangles.Add((z + 1) * dimensions + x);
                    triangles.Add((z + 1) * dimensions + x + 1);
                }
            }
        }
        tempMesh.vertices = sommets;
        tempMesh.triangles = triangles.ToArray();
        tempMesh.uv = uvs;
        tempMesh.RecalculateNormals();

        return tempMesh;
    }

    static void ClasserPoints(Vector3 sommet)
    {
        if (sommet.y < 0.8f && sommet.y > 0.3f)
        {
            bordDeLeau.Add(sommet);
        }
        if (sommet.y > PlusHaut.y)
        {
            PlusHaut = sommet;
        }
    }


    public static Vector3 GetPointPlusHaut()
    {
        return PlusHaut;
    }

    public static List<Vector3> GetPointsEau()
    {
        return bordDeLeau;
    }
}
