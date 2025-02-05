using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportementUI : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Vector3 directionToCamera = transform.position - GameManager.instance.GetPositionCam();

        // Pour que le slider fasse toujouts face à la cam
        if(directionToCamera.x != 0 && directionToCamera.z != 0)
        {
            transform.rotation = Quaternion.LookRotation(new Vector3(directionToCamera.x, 0, directionToCamera.z));
        }
        
    }
}
