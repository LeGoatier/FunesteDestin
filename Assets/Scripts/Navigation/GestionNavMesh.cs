using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

/// <summary>
/// Nous avons besoin d'un script qui contient un NavMeshSurface
/// pour pouvoir bake une fois la création de l'environnement terminée.
/// Le script contient seulement une fonction publique qui bake l'environnement et
/// qui devra être appelée par le script de génération de l'île une fois que l'environnement est
/// complètement créé.
/// Auteur : Justin Gauthier
/// Date : 5 février 2024
/// </summary>

public class GestionNavMesh : MonoBehaviour
{
    private static NavMeshSurface surface;
    public static bool EstNavMeshCree = false;
    //Il pourrait y avoir un problème d'ordre si la fonction BakeSurface est appellée
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
