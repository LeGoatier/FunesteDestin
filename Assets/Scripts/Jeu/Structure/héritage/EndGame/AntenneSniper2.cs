using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntenneSniper2 : MonoBehaviour
{

    public static EventHandler OnTirerAvecSniper;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("BalleSniper"))
        {
            
            OnTirerAvecSniper?.Invoke(this, EventArgs.Empty);
            
        }
    }
}
