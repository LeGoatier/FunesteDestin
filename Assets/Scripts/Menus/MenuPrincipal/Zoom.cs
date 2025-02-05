using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    [SerializeField] float vitesseZoom = 0.15f;
    [SerializeField] float scaleMin = 0.3f;
    [SerializeField] float scaleMax = 1.5f;
    [SerializeField] float scaleBase = 0.8f;

    private void Start()
    {
        transform.localScale = new Vector3(scaleBase,scaleBase,scaleBase);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y != 0 && transform.localScale.y <= scaleMax && transform.localScale.y >= scaleMin)
        {
            float modificationScale = Input.mouseScrollDelta.y * vitesseZoom;

            if (transform.localScale.y + modificationScale > scaleMax)
                transform.localScale = new Vector3(scaleMax, scaleMax, scaleMax);
            else if (transform.localScale.y + modificationScale < scaleMin)
                transform.localScale = new Vector3(scaleMin,scaleMin,scaleMin);
            else
            {
                transform.localScale += new Vector3(modificationScale, modificationScale, modificationScale);
            }
        }
    }
}
