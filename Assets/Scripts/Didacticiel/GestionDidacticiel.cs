using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using static UnityEditor.VersionControl.Asset;

public class GestionDidacticiel : MonoBehaviour
{
    [SerializeField] …tapeDidacticiel[] Ètapes;
    private int progression = 0;
    public static GestionDidacticiel instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        if(InfoPartie.jouerDidacticiel) Ètapes[progression].Commencer…tape();
    }

    private void OnEnable()
    {
        progression = 0;
        if (InfoPartie.jouerDidacticiel) Ètapes[progression].Commencer…tape();
    }

    public void Next()
    {
        progression++;
        if(Ètapes.Length > progression)
        {
            Ètapes[progression].Commencer…tape();
        }
    }
}
