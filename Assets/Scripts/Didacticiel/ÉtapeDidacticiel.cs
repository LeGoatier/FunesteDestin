using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ÉtapeDidacticiel : MonoBehaviour
{
    protected bool actif = false;
    protected int progression = 1;
    protected bool pretFaireEtape = false;
    protected CharacterController characterController;

    protected virtual void Start()
    {
        characterController = ControlPersonnage.instance.characterController;
    }

    private void OnDisable()
    {
        actif = false;
    }


    public void CommencerÉtape()
    {
        StartCoroutine(Débuter());
    }

    public IEnumerator AttendreEtLancerFonction(float temps, Action fonction)
    {
        yield return new WaitForSeconds(temps);
        fonction();
    }

    protected abstract IEnumerator Débuter();
}


