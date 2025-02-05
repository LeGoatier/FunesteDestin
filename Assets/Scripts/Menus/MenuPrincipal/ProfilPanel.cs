using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilPanel : MonoBehaviour
{
    //Pour la plupart de ces champs l�, le serializedField fait seulement r�f�rence � l'endroit o� la valeur
    //est �crite, pas au texte qui l'introduit
    [SerializeField] TextMeshProUGUI nomJoueurTexte;
    [SerializeField] TMP_Dropdown titreJoueurTexte;
    [SerializeField] Slider barreProgressionXP;
    [SerializeField] TextMeshProUGUI nombreGaucheXPTexte;
    [SerializeField] TextMeshProUGUI nombreDroiteXPTexte;
    [SerializeField] TextMeshProUGUI nombreVictoiresTexte;
    [SerializeField] TextMeshProUGUI meilleurTempsTexte;
    [SerializeField] TextMeshProUGUI nombreEnnemisTu�sTexte;
    [SerializeField] TextMeshProUGUI niveauTexte;
    [SerializeField] TextMeshProUGUI joursSurv�cusTexte;

    [SerializeField] Image imageProfil;

    public static ProfilPanel instance;

    private void Start()
    {
        instance = this;
    }

    public void InitialiserChampsTextes()
    {
        nomJoueurTexte.text = SauvegardeProfils.nomProfilActuel;
        //Faut initialiser le dropdown comme il faut aussi
        barreProgressionXP.value = GestionStatistiques.ObtenirProgressionXP();
        nombreGaucheXPTexte.text = GestionStatistiques.ObtenirNiveauJoueur().ToString();
        nombreDroiteXPTexte.text = (GestionStatistiques.ObtenirNiveauJoueur() + 1).ToString();
        nombreVictoiresTexte.text = GestionStatistiques.ObtenirNombreVictoires().ToString();
        int meilleurTemps = (int)GestionStatistiques.ObtenirMeilleurTemps();
        if(meilleurTemps == 0)
        {
            meilleurTempsTexte.text = "Meilleur temps : N/A";
        }
        else
        {
            int minutes = meilleurTemps / 60;
            int secondesRestantes = meilleurTemps % 60;
            meilleurTempsTexte.text = $"Meilleur temps : {minutes} min {secondesRestantes} s";
        }
        nombreEnnemisTu�sTexte.text = GestionStatistiques.ObtenirNombreTotalEnnemisTu�sProfil().ToString();
        niveauTexte.text = GestionStatistiques.ObtenirNiveauJoueur().ToString();
        joursSurv�cusTexte.text = $"Jours surv�cus : {GestionStatistiques.ObtenirJoursSurv�cusProfil()}";

        titreJoueurTexte.ClearOptions();
        titreJoueurTexte.AddOptions(GestionAchievements.ObtenirListeTitresD�bloqu�s());
        int indiceTitreS�lectionn� = GestionAchievements.ObtenirListeTitresD�bloqu�s().IndexOf(SauvegardeProfils.titreProfilActuel);
        titreJoueurTexte.value = indiceTitreS�lectionn�;

        imageProfil.sprite = ProfilManager.instance.ObtenirImage();
    }

    public void ChangerTitre()
    {
        SauvegardeProfils.SetTitreProfilActuel(titreJoueurTexte.captionText.text);//Jsp si c'est optimal mais j'esp�re �a fonctionne
    }


}
