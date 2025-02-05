using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Limitation : MonoBehaviour
{
    [SerializeField] float distanceXMax;
    [SerializeField] float distanceYmax;
    [SerializeField] float distanceYBas;

    [SerializeField] GameObject zoom;
    

    public void RestreindrePosition()
    {
        Vector3 scale = zoom.transform.localScale * 2.5f;
        Vector2 positionInitiale = transform.localPosition;
        float positionX = Mathf.Clamp(positionInitiale.x, -distanceXMax * scale.x, distanceXMax * scale.x);
        float positionY = Mathf.Clamp(positionInitiale.y, -distanceYmax * scale.y, distanceYBas * scale.y);
        transform.localPosition = new Vector3(positionX, positionY, 0);
    }
}
 