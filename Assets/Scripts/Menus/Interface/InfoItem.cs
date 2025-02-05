using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InfoItem
{
    public static string[] ressources = new string[(int)Ressource.NbRessources] { "Bois", "Pierre", "Fer", "Plastique", "Champignon", "Fils Électrique", "Boitier Cylindrique", "Batterie" };
    public static string[] outils = new string[(int)Outil.NbOutils] { "Marteau", "Moteur Brisé", "Moteur", "Radio Brisée", "Radio", " Essence", "Composant Pyrotechnique", "Colorant", "Voiles", "Fusée de Détresse" };
    public static string[] armes = new string[(int)Arme.NbArmes] { "Arbalète", "Pistolet", "Sniper", "Fusil d'assault", "Fusil à Pompe"};
    public static string[] soins = new string[(int)Soin.NbSoins] { "Champignon", "Trousse" };

}
