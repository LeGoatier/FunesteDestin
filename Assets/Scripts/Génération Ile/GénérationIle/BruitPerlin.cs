using UnityEngine;

public static class BruitPerlin
{
    public static float[,] G�n�rerMatrice(int dimensions, float grandeurPerlin, int octaves, float persistance, float lacunarit�, Vector2 d�calage)
    {
        // Matrice/grille avec les valeurs de Perlin
        float[,] matrice = new float[dimensions, dimensions];

        // Tableau pour stocker les d�calages de chaque octave
        Vector2[] d�calgeOctaves = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            // G�n�ration de d�calages al�atoires pour chaque octave
            float offsetX = UnityEngine.Random.Range(-100000, 100000) + d�calage.x;
            float offsetY = UnityEngine.Random.Range(-100000, 100000) + d�calage.y;
            d�calgeOctaves[i] = new Vector2(offsetX, offsetY);
        }

        // Variables pour suivre les valeurs maximales et minimales
        float maxActuel = float.MinValue;
        float minActuel = float.MaxValue;

        // Assure que grandeurPerlin n'est pas �gale � z�ro
        if (grandeurPerlin <= 0)
            grandeurPerlin = 0.001f;

        // Boucle � travers chaque point de la matrice
        for (int y = 0; y < dimensions; y++)
            for (int x = 0; x < dimensions; x++)
            {
                float temp;

                // Assure que i et j sont entre -1 et 1
                float i = x / (float)dimensions * 2 - 1;
                float j = y / (float)dimensions * 2 - 1;
                float distanceCentre = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(i, 2) + Mathf.Pow(j, 2)));

                // Param�tres du bruit qui pourraient �tre chang�s pour donner un relief int�ressant, mais pas n�c�ssaire
                float amplitude = 1;
                float fr�quence = 1;
                float hauteurBruit = 0;

                float positionX = (x - dimensions / 2);
                float positionY = (y - dimensions / 2);

                const float RAYON_CARR�_PLAT = 10;

                //Dans le fond on cr�e un carr� au milieu que toute la valeur d'input du bruit est la m�me, donc
                //la hauteur est la m�me aussi, et on commence � cr�er le reste avec le bruit � partir de cet input l� apr�s
                //donc �a fait une transition de bruit normal
                if (Mathf.Abs(positionX) < RAYON_CARR�_PLAT)
                {
                    positionX = 0;
                }
                else
                {
                    if (positionX > 0)
                    {
                        positionX -= RAYON_CARR�_PLAT;
                    }
                    else
                    {
                        positionX += RAYON_CARR�_PLAT;
                    }
                }
                if (Mathf.Abs(positionY) < RAYON_CARR�_PLAT)
                {
                    positionY = 0;
                }
                else
                {
                    if (positionY > 0)
                    {
                        positionY -= RAYON_CARR�_PLAT;
                    }
                    else

                        positionY += RAYON_CARR�_PLAT;

                }

                // G�n�re le bruit de Perlin pour chaque octave
                for (int w = 0; w < octaves; w++)
                {
                    //On trouve une coordonn�e en x et y qui prend en compte plusieurs param�tres pour ins�rer dans la fonction de Perlin
                    float bruitX = positionX / grandeurPerlin * fr�quence + d�calgeOctaves[w].x;
                    float bruitY = positionY / grandeurPerlin * fr�quence + d�calgeOctaves[w].y;

                    // D�termine la valeur avec bruit de perlin
                    float valeurPerlin = Mathf.PerlinNoise(bruitX, bruitY);

                    // D�termine la hauteur de chaque point de la matrice
                    hauteurBruit += valeurPerlin * amplitude;

                    // Diminue chaque octave
                    amplitude *= persistance;

                    // Augmente chaque octave si la lacunarit� est sup�rieure � 0
                    fr�quence *= lacunarit�;
                }

                // Garde en m�moire les valeurs les plus hautes et plus basses
                if (hauteurBruit > maxActuel)
                    maxActuel = hauteurBruit;
                else if (hauteurBruit < minActuel)
                    minActuel = hauteurBruit;

                // Normalise la valeur temporelle
                temp = Mathf.InverseLerp(minActuel, maxActuel, hauteurBruit);

                // Permet de cr�er une forme d'�le, donc que les points du centre soient �lev�s et ceux du contour soient plus bas
                // On soustrait � la valeur temp une autre valeur qui est proche de 1 si temp est loin du centre et proche de 0 s'il est proche
                temp = Mathf.Clamp01(temp - �valuerHauteursPourIle(distanceCentre));
                matrice[x, y] = temp;
            }
        return matrice;
    }

    // Fonction pour �valuer la valeur de la hauteur de chaque point en fonction de la distance au centre
    static float �valuerHauteursPourIle(float valeur)
    {
        // Indice exponentiel
        float a = 3;

        // Calcule une valeur en fonction de sa distance par rapport au centre de fa�on exponentielle
        //Si la valeur de la distance au centre est �lev�e, on retourne un valeur plus �lev�e que si elle est petite
        return Mathf.Pow(valeur, a) / (Mathf.Pow(valeur, a) + Mathf.Pow(1 - valeur, a));
    }
}
