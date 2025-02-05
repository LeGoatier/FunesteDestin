using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSniper : ComportementBalle
{
    private void Awake()
    {
        Vitesse = 120;
        DuréeDeVie = 3;
        Dammage = 10;
        Initialiser();
    }
    protected override void AppliquerForceInitiale()
    {
        rb.AddForce(transform.forward * Vitesse, ForceMode.Impulse);
    }
}
