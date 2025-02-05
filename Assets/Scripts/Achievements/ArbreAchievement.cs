using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArbreAchievement : MonoBehaviour
{
    [SerializeField] NoeudAchievement premierNoeud;
    public EventHandler OnLancementRechercheArbreAchievement;
    public void ActiverPremierNoeud()
    {
        OnLancementRechercheArbreAchievement?.Invoke(this, EventArgs.Empty);
        premierNoeud.ActiverNoeudEtEnfants();
    }
}
