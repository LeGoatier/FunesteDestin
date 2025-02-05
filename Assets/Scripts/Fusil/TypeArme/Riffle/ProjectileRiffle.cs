using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRiffle : ComportementBalle
{
    private void Awake()
    {
        Vitesse = 150;
        Dur�eDeVie = 3;
        Dammage = 2f;
        Initialiser();
    }

    protected override void AppliquerForceConstante()
    {
        rb.velocity = transform.forward * Vitesse;
    }
}
