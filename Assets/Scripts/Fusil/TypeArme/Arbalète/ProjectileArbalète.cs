using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileArbalète : ComportementBalle
{
    bool ATrouvéArbalète = false;
    GameObject arbalète;


    private void Awake()
    {
        Vitesse = 0;
        DuréeDeVie = 3;
        Dammage = 7;
        Initialiser();
    }
    protected override void AppliquerForceInitiale()
    {
        if (!ATrouvéArbalète)
        {
            arbalète = GameObject.Find("Arbalète(Clone)");
        }
        
        if (arbalète != null)
        {
            Vitesse = arbalète.GetComponent<PrédictionTrajectoireArbalète>().VitesseCourante;
            ATrouvéArbalète = true;
        }
        
        rb.AddForce(transform.forward * Vitesse, ForceMode.Impulse);
    }
    

   
}
