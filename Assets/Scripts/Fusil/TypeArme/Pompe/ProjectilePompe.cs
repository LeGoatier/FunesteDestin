using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePompe : ComportementBalle
{
    

    private void Awake()
    {
        Vitesse =100;
        Dur�eDeVie = 3;
        Dammage = 1.0f;
        Initialiser();
    }
    

    
    protected override void AppliquerForceConstante()
    {
        rb.velocity = transform.forward * Vitesse;
    }
}
