using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Nous avons besoin d'un script qui contient un NavMeshSurface
/// pour pouvoir bake une fois la cr�ation de l'environnement termin�e.
/// Le script contient seulement une fonction publique qui bake l'environnement et
/// qui devra �tre appel�e par le script de g�n�ration de l'�le une fois que l'environnement est
/// compl�tement cr��.
/// Auteur : Justin Gauthier
/// Date : 5 f�vrier 2024
/// </summary>

public class GestionNavMesh : MonoBehaviour
{
    private static NavMeshSurface surface;
    public static bool EstNavMeshCree = false;
    //Il pourrait y avoir un probl�me d'ordre si la fonction BakeSurface est appell�e
    //avant le awake de ce script
    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }
    public static void BakeSurface()
    {
        surface.BuildNavMesh();
        EstNavMeshCree = true;
    }
}
