using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUIFusil : MonoBehaviour
{
    [SerializeField] int directionX;
    [SerializeField] int directionY;


    const float LongueurD�placement = 30;


    Vector3 PositionD�part;
    Vector3 PositionFin;
    float VitesseMouvementVis�;
    RectTransform Donn�esImage;

    private void Start()
    {
        Donn�esImage = GetComponent<RectTransform>();
        PositionD�part = Donn�esImage.localPosition;
        PositionFin = PositionD�part + new Vector3(Donn�esImage.localPosition.x + LongueurD�placement * directionX, Donn�esImage.localPosition.y + LongueurD�placement * directionY, Donn�esImage.localPosition.z);

        
        VitesseMouvementVis� = GetComponentInParent<AnimationGun>().VitesseMouvementVis�;
    }

    private void Update()
    {
        Viser();
    }

    void Viser()
    {
        if (Input.GetMouseButton(1))//vise si le joueur clic droit
        {
            MouvementD�placemement(PositionD�part);
        }
        else//sinon reviens � sa position initiale
        {
            MouvementD�placemement(PositionFin);
        }

    }

    void MouvementD�placemement(Vector3 positionFinale)
    {
        Donn�esImage.localPosition = Vector3.Lerp(Donn�esImage.localPosition, positionFinale, VitesseMouvementVis� * Time.deltaTime);
    }
}
