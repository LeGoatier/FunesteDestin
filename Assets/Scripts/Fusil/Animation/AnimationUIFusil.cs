using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUIFusil : MonoBehaviour
{
    [SerializeField] int directionX;
    [SerializeField] int directionY;


    const float LongueurDéplacement = 30;


    Vector3 PositionDépart;
    Vector3 PositionFin;
    float VitesseMouvementVisé;
    RectTransform DonnéesImage;

    private void Start()
    {
        DonnéesImage = GetComponent<RectTransform>();
        PositionDépart = DonnéesImage.localPosition;
        PositionFin = PositionDépart + new Vector3(DonnéesImage.localPosition.x + LongueurDéplacement * directionX, DonnéesImage.localPosition.y + LongueurDéplacement * directionY, DonnéesImage.localPosition.z);

        
        VitesseMouvementVisé = GetComponentInParent<AnimationGun>().VitesseMouvementVisé;
    }

    private void Update()
    {
        Viser();
    }

    void Viser()
    {
        if (Input.GetMouseButton(1))//vise si le joueur clic droit
        {
            MouvementDéplacemement(PositionDépart);
        }
        else//sinon reviens à sa position initiale
        {
            MouvementDéplacemement(PositionFin);
        }

    }

    void MouvementDéplacemement(Vector3 positionFinale)
    {
        DonnéesImage.localPosition = Vector3.Lerp(DonnéesImage.localPosition, positionFinale, VitesseMouvementVisé * Time.deltaTime);
    }
}
