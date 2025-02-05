using UnityEngine;


public class Cylindre : PrimitiveDynamique
{
   [Header("Donn�es de base du cylindre")]
   [SerializeField] string NomCylindre = "Cylindre";
   [SerializeField] int NbFacettes = 24;
   [SerializeField] Vector3 �tendueCylindre = Vector3.one;
   [SerializeField] Material Mat�riauCylindre;
   [SerializeField] bool EstCylindreCarrel�;

    // � compl�ter
    float DeltaAngle;
    const int NbSommetParRang� = 2;
    
   public void G�n�rerBordure(float rayon)
   {
        �tendueCylindre.x = rayon;
        �tendueCylindre.z = rayon;
        InitialiserPrimitive(NomCylindre, �tendueCylindre, Mat�riauCylindre, EstCylindreCarrel�);
        
   }
   protected override void CalculerDonn�esInitiales(bool estCarrel�)
   {
        Origine = Vector3.zero;
        NbSommets = NbFacettes * 2;
        DeltaTexture = estCarrel� ? Vector2.one : Vector2.one; 
        DeltaAngle = (360 / NbFacettes) * (Mathf.PI / 180f);
        Sommets = new Vector3[NbSommets];
        GetComponent<MeshRenderer>().material = Mat�riauCylindre;
    }

   protected override void G�n�rerSommets()
   {
        

        int index = 0;
        for (int i = 0; i < NbFacettes; i++)
        {
            Sommets[index++] = �tendueCylindre.z * new Vector3(Mathf.Cos(DeltaAngle * i), Origine.y, Mathf.Sin(DeltaAngle * i));
            Sommets[index++] = new Vector3(�tendueCylindre.x * Mathf.Cos(DeltaAngle * i), �tendueCylindre.y, �tendueCylindre.x * Mathf.Sin(DeltaAngle * i));

        }
    }

   protected override void G�n�rerListeTriangles()
   {
        ListeTriangles = new int[NbFacettes * NbTrianglesParTuile * NbSommetsParTriangle];
        int indexTriangle = 0;


        for (int i = 0; i < NbFacettes; i++)
        {
            int ptA = i * 2;
            int ptB = (ptA + 2)%NbSommets; //pour lier la derni�re face avec la premi�re, modulo: si on d�passe le nombre de sommet, on repart � 0
            int ptC = ptA + 1;
            int ptD = ptB + 1;
            AjouterTriangle(ListeTriangles, ptA, ptB, ptC, ref indexTriangle);
            AjouterTriangle(ListeTriangles, ptB, ptD, ptC, ref indexTriangle);
        }


    }

   protected override void G�n�rerCoordonn�esTexture()
   {
        
        Coordonn�esTexture = new Vector2[NbSommets];
        int index = 0;
        for (int i = 0; i < NbFacettes; i++)
        {
            for (int j = 0; j < NbSommetParRang�; j++)
            {
                Coordonn�esTexture[index++] = new Vector2(i * DeltaTexture.x, j * DeltaTexture.y);
            }
        }
    }
}
