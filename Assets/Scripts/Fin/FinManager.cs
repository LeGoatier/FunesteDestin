using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinManager : MonoBehaviour
{
    [SerializeField] GameObject[] canevas; //Important de placer les canevas dans le même ordre que l'enum étatFin
    [SerializeField] GameObject canevasRésuméPartie;
    public static FinManager instance;

    void Start()
    {
        if (instance == null)
            instance = this;

        canevas[(int)GestionScenes.étatFinPartieActuel].SetActive(true);
    }

    //Le résumé de la partie est pareil peu importe l'état de fin
    //C'est le même canevas dans tous les cas
    public void LancerRésuméPartie()
    {
        canevasRésuméPartie.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        canevasRésuméPartie.GetComponent<RésuméPartieCanevas>().AnimerBarreXP();
    }


}
