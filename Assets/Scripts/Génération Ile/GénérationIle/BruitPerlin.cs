using UnityEngine;

public static class BruitPerlin
{
    public static float[,] GénérerMatrice(int dimensions, float grandeurPerlin, int octaves, float persistance, float lacunarité, Vector2 décalage)
    {
        // Matrice/grille avec les valeurs de Perlin
        float[,] matrice = new float[dimensions, dimensions];

        // Tableau pour stocker les décalages de chaque octave
        Vector2[] décalgeOctaves = new Vector2[octaves];
        for (int i = 0; i < octaves; i++)
        {
            // Génération de décalages aléatoires pour chaque octave
            float offsetX = UnityEngine.Random.Range(-100000, 100000) + décalage.x;
            float offsetY = UnityEngine.Random.Range(-100000, 100000) + décalage.y;
            décalgeOctaves[i] = new Vector2(offsetX, offsetY);
        }

        // Variables pour suivre les valeurs maximales et minimales
        float maxActuel = float.MinValue;
        float minActuel = float.MaxValue;

        // Assure que grandeurPerlin n'est pas égale à zéro
        if (grandeurPerlin <= 0)
            grandeurPerlin = 0.001f;

        // Boucle à travers chaque point de la matrice
        for (int y = 0; y < dimensions; y++)
            for (int x = 0; x < dimensions; x++)
            {
                float temp;

                // Assure que i et j sont entre -1 et 1
                float i = x / (float)dimensions * 2 - 1;
                float j = y / (float)dimensions * 2 - 1;
                float distanceCentre = Mathf.Clamp01(Mathf.Sqrt(Mathf.Pow(i, 2) + Mathf.Pow(j, 2)));

                // Paramètres du bruit qui pourraient être changés pour donner un relief intéressant, mais pas nécéssaire
                float amplitude = 1;
                float fréquence = 1;
                float hauteurBruit = 0;

                float positionX = (x - dimensions / 2);
                float positionY = (y - dimensions / 2);

                const float RAYON_CARRÉ_PLAT = 10;

                //Dans le fond on crée un carré au milieu que toute la valeur d'input du bruit est la même, donc
                //la hauteur est la même aussi, et on commence à créer le reste avec le bruit à partir de cet input là après
                //donc ça fait une transition de bruit normal
                if (Mathf.Abs(positionX) < RAYON_CARRÉ_PLAT)
                {
                    positionX = 0;
                }
                else
                {
                    if (positionX > 0)
                    {
                        positionX -= RAYON_CARRÉ_PLAT;
                    }
                    else
                    {
                        positionX += RAYON_CARRÉ_PLAT;
                    }
                }
                if (Mathf.Abs(positionY) < RAYON_CARRÉ_PLAT)
                {
                    positionY = 0;
                }
                else
                {
                    if (positionY > 0)
                    {
                        positionY -= RAYON_CARRÉ_PLAT;
                    }
                    else

                        positionY += RAYON_CARRÉ_PLAT;

                }

                // Génère le bruit de Perlin pour chaque octave
                for (int w = 0; w < octaves; w++)
                {
                    //On trouve une coordonnée en x et y qui prend en compte plusieurs paramètres pour insérer dans la fonction de Perlin
                    float bruitX = positionX / grandeurPerlin * fréquence + décalgeOctaves[w].x;
                    float bruitY = positionY / grandeurPerlin * fréquence + décalgeOctaves[w].y;

                    // Détermine la valeur avec bruit de perlin
                    float valeurPerlin = Mathf.PerlinNoise(bruitX, bruitY);

                    // Détermine la hauteur de chaque point de la matrice
                    hauteurBruit += valeurPerlin * amplitude;

                    // Diminue chaque octave
                    amplitude *= persistance;

                    // Augmente chaque octave si la lacunarité est supérieure à 0
                    fréquence *= lacunarité;
                }

                // Garde en mémoire les valeurs les plus hautes et plus basses
                if (hauteurBruit > maxActuel)
                    maxActuel = hauteurBruit;
                else if (hauteurBruit < minActuel)
                    minActuel = hauteurBruit;

                // Normalise la valeur temporelle
                temp = Mathf.InverseLerp(minActuel, maxActuel, hauteurBruit);

                // Permet de créer une forme d'île, donc que les points du centre soient élevés et ceux du contour soient plus bas
                // On soustrait à la valeur temp une autre valeur qui est proche de 1 si temp est loin du centre et proche de 0 s'il est proche
                temp = Mathf.Clamp01(temp - ÉvaluerHauteursPourIle(distanceCentre));
                matrice[x, y] = temp;
            }
        return matrice;
    }

    // Fonction pour évaluer la valeur de la hauteur de chaque point en fonction de la distance au centre
    static float ÉvaluerHauteursPourIle(float valeur)
    {
        // Indice exponentiel
        float a = 3;

        // Calcule une valeur en fonction de sa distance par rapport au centre de façon exponentielle
        //Si la valeur de la distance au centre est élevée, on retourne un valeur plus élevée que si elle est petite
        return Mathf.Pow(valeur, a) / (Mathf.Pow(valeur, a) + Mathf.Pow(1 - valeur, a));
    }
}
