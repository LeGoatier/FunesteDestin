using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePistolet : ComportementBalle
{
     


    private void Awake()
    {
        Vitesse = 100;
        Dur�eDeVie = 3;
        Dammage = 3;
        Initialiser();
    }
    
    protected override void AppliquerForceConstante()
    {
        rb.velocity = transform.forward * Vitesse;
    }
}
