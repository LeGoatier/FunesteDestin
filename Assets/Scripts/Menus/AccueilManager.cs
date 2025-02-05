using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AccueilManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            GestionScenes.ChargerMenuPrincipal(true);
        }
    }
}
