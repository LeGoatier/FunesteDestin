using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.VersionControl.Asset;

public class GestionDidacticiel : MonoBehaviour
{
    [SerializeField] ÉtapeDidacticiel[] étapes;
    private int progression = 0;
    public static GestionDidacticiel instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if(InfoPartie.jouerDidacticiel) étapes[progression].CommencerÉtape();
    }

    private void OnEnable()
    {
        progression = 0;
        if (InfoPartie.jouerDidacticiel) étapes[progression].CommencerÉtape();
    }

    public void Next()
    {
        progression++;
        if(étapes.Length > progression)
        {
            étapes[progression].CommencerÉtape();
        }
    }
}
