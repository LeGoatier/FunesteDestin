using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinManager : MonoBehaviour
{
    [SerializeField] GameObject[] canevas; //Important de placer les canevas dans le m�me ordre que l'enum �tatFin
    [SerializeField] GameObject canevasR�sum�Partie;
    public static FinManager instance;

    void Start()
    {
        if (instance == null)
            instance = this;

        canevas[(int)GestionScenes.�tatFinPartieActuel].SetActive(true);
    }

    //Le r�sum� de la partie est pareil peu importe l'�tat de fin
    //C'est le m�me canevas dans tous les cas
    public void LancerR�sum�Partie()
    {
        canevasR�sum�Partie.SetActive(true);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        canevasR�sum�Partie.GetComponent<R�sum�PartieCanevas>().AnimerBarreXP();
    }


}
