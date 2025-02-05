using UnityEngine;

// Classe Parent des primitives dynamiques
// Dans cette relation, le parent s'occupe des généralités entourant la gestion du calcul 
// des sommets, des triangles, des normales et des tangentes, ainsi que celles entourant la création du maillage en général.
// L'enfant implémente les spécificités entourant le calcul des sommets, des triangles et des coordonnées de texture.
public abstract class PrimitiveDynamique : MonoBehaviour
{
   public const int NbTrianglesParTuile = 2;
   public const int NbSommetsParTriangle = 3;
   public const int NbSommetsParTuiles = 4;

   protected Vector3 Étendue { get; private set; } // Étendue de la primitive dans l'espace
   string NomMaillage { get; set; }
   protected Mesh Maillage { get; set; }
   protected Vector3[] Sommets { get; set; }  // Les différents sommets (vertices) de la surface
   protected int[] ListeTriangles { get; set; }
   protected Vector3 Origine { get; set; }  // L'origine du vecteur est fixée de manière à ce que le centre de la surface 
                                            // (croisement des axes de rotation) soit situé au point (0, 0, 0) de l'espace virtuel.
   protected Vector2[] CoordonnéesTexture { get; set; } // Les coordonnées permettant l'application de la texture sur la primitive.
   protected int NbSommets { get; set; } // Nombre de sommets de la surface
   protected Vector2 DeltaTexture { get; set; } // Variations horizontale et verticale dans les coordonnées de texture

   public virtual void InitialiserPrimitive(string nomMaillage, Vector3 étendue, Material matériau, bool estCarrelé) 
   {
      //Permet de terminer la construction de l'objet
      NomMaillage = nomMaillage;
      Étendue = étendue;
      GetComponent<MeshRenderer>().material = matériau;
      CalculerDonnéesInitiales(estCarrelé);
      GénérerMaillage();
      MeshCollider maillageCollision = GetComponent<MeshCollider>();
      if (maillageCollision != null)
      {
         maillageCollision.sharedMesh = GénérerMaillageCollision();
      }
      // Une fois la construction complétée, on active la primitive
      this.gameObject.SetActive(true);
   }

   protected virtual void CalculerDonnéesInitiales(bool estCarrelé)
   {
      Origine = new Vector3(-Étendue.x / 2, 0, -Étendue.y / 2);
   }

   private void GénérerMaillage()
   {
      Maillage = new Mesh();
      GetComponent<MeshFilter>().mesh = Maillage;
      Maillage.name = NomMaillage;
      GénérerSommets();
      GénérerListeTriangles();
      GénérerCoordonnéesTexture();
      Maillage.vertices = Sommets;
      Maillage.triangles = ListeTriangles;
      Maillage.uv = CoordonnéesTexture;
      Maillage.RecalculateNormals();
      Maillage.RecalculateTangents();
   }

   protected abstract void GénérerSommets();

   protected abstract void GénérerListeTriangles();

   protected abstract void GénérerCoordonnéesTexture();

   protected virtual Mesh GénérerMaillageCollision() //Peut être surchargée pour générer une version simplifiée du Mesh
   {
      return Maillage;
   }

   protected static void AjouterTriangle(int[] triangles, int indexSommetA, int indexSommetB, int indexSommetC, ref int indexTriangle)
   {
      triangles[indexTriangle++] = indexSommetA;
      triangles[indexTriangle++] = indexSommetB;
      triangles[indexTriangle++] = indexSommetC;
   }
}
