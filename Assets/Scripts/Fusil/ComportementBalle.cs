using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComportementBalle : MonoBehaviour
{
    public float Vitesse { get; protected set; }
    public float DuréeDeVie { get; protected set; }
    public float Dammage { get; protected set; }

    protected Rigidbody rb;
    protected TrailRenderer tr;
    float Timer;
    ObjectPool pool;
    [SerializeField] GameObject impact;
    

    protected virtual void AppliquerForceInitiale()
    {

    }
    protected virtual void AppliquerForceConstante()
    {

    }


    

    protected void Initialiser()
    {
        rb = GetComponent<Rigidbody>();
        tr = rb.GetComponent<TrailRenderer>();

        pool = GameObject.FindGameObjectWithTag("Object_Pool").GetComponent<ObjectPool>();
    }

    private void OnEnable()
    {
        ResetRigidBody();

        AppliquerForceInitiale();

    }

    private void ResetRigidBody()
    {
        if (rb != null)
        {
            rb.angularVelocity = Vector3.zero;
        }

        if (tr != null)
        {
            tr.Clear();
        }


    }

    private void Update()
    {
        Timer += Time.deltaTime;
        if (Timer > DuréeDeVie)
        {
            DésactiverBalle();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        AppliquerForceConstante();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ennemi")
        {
            ComportementEnnemi ennemi = collision.gameObject.GetComponent<ComportementEnnemi>();
            ennemi.PrendreDégats(Dammage);
        }
        
        ActivéParticule();

        DésactiverBalle();

    }

    void ActivéParticule()
    {
        //var impactParticle = Instantiate(impact);
        //impactParticle.transform.position = transform.position;

        GameObject impactParticle = pool.GetPoolObject(impact);

        if (impactParticle != null)
        {
            impactParticle.transform.position = transform.position;

            impactParticle.SetActive(true);
        }


    }

    private void DésactiverBalle()
    {
        Timer = 0;
        rb.velocity = new Vector3(0, 0, 0);
        gameObject.SetActive(false);
    }

    public float GetVitesse()
    {
        return Vitesse;
    }
}