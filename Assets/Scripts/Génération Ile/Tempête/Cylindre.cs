using UnityEngine;


public class Cylindre : PrimitiveDynamique
{
   [Header("Données de base du cylindre")]
   [SerializeField] string NomCylindre = "Cylindre";
   [SerializeField] int NbFacettes = 24;
   [SerializeField] Vector3 ÉtendueCylindre = Vector3.one;
   [SerializeField] Material MatériauCylindre;
   [SerializeField] bool EstCylindreCarrelé;

    // à compléter
    float DeltaAngle;
    const int NbSommetParRangé = 2;
    
   public void GénérerBordure(float rayon)
   {
        ÉtendueCylindre.x = rayon;
        ÉtendueCylindre.z = rayon;
        InitialiserPrimitive(NomCylindre, ÉtendueCylindre, MatériauCylindre, EstCylindreCarrelé);
        
   }
   protected override void CalculerDonnéesInitiales(bool estCarrelé)
   {
        Origine = Vector3.zero;
        NbSommets = NbFacettes * 2;
        DeltaTexture = estCarrelé ? Vector2.one : Vector2.one; 
        DeltaAngle = (360 / NbFacettes) * (Mathf.PI / 180f);
        Sommets = new Vector3[NbSommets];
        GetComponent<MeshRenderer>().material = MatériauCylindre;
    }

   protected override void GénérerSommets()
   {
        

        int index = 0;
        for (int i = 0; i < NbFacettes; i++)
        {
            Sommets[index++] = ÉtendueCylindre.z * new Vector3(Mathf.Cos(DeltaAngle * i), Origine.y, Mathf.Sin(DeltaAngle * i));
            Sommets[index++] = new Vector3(ÉtendueCylindre.x * Mathf.Cos(DeltaAngle * i), ÉtendueCylindre.y, ÉtendueCylindre.x * Mathf.Sin(DeltaAngle * i));

        }
    }

   protected override void GénérerListeTriangles()
   {
        ListeTriangles = new int[NbFacettes * NbTrianglesParTuile * NbSommetsParTriangle];
        int indexTriangle = 0;


        for (int i = 0; i < NbFacettes; i++)
        {
            int ptA = i * 2;
            int ptB = (ptA + 2)%NbSommets; //pour lier la dernière face avec la première, modulo: si on dépasse le nombre de sommet, on repart à 0
            int ptC = ptA + 1;
            int ptD = ptB + 1;
            AjouterTriangle(ListeTriangles, ptA, ptB, ptC, ref indexTriangle);
            AjouterTriangle(ListeTriangles, ptB, ptD, ptC, ref indexTriangle);
        }


    }

   protected override void GénérerCoordonnéesTexture()
   {
        
        CoordonnéesTexture = new Vector2[NbSommets];
        int index = 0;
        for (int i = 0; i < NbFacettes; i++)
        {
            for (int j = 0; j < NbSommetParRangé; j++)
            {
                CoordonnéesTexture[index++] = new Vector2(i * DeltaTexture.x, j * DeltaTexture.y);
            }
        }
    }
}
