using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InfoItem
{
    public static string[] ressources = new string[(int)Ressource.NbRessources] { "Bois", "Pierre", "Fer", "Plastique", "Champignon", "Fils �lectrique", "Boitier Cylindrique", "Batterie" };
    public static string[] outils = new string[(int)Outil.NbOutils] { "Marteau", "Moteur Bris�", "Moteur", "Radio Bris�e", "Radio", " Essence", "Composant Pyrotechnique", "Colorant", "Voiles", "Fus�e de D�tresse" };
    public static string[] armes = new string[(int)Arme.NbArmes] { "Arbal�te", "Pistolet", "Sniper", "Fusil d'assault", "Fusil � Pompe"};
    public static string[] soins = new string[(int)Soin.NbSoins] { "Champignon", "Trousse" };

}
