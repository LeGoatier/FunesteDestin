using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilPanel : MonoBehaviour
{
    //Pour la plupart de ces champs là, le serializedField fait seulement référence à l'endroit où la valeur
    //est écrite, pas au texte qui l'introduit
    [SerializeField] TextMeshProUGUI nomJoueurTexte;
    [SerializeField] TMP_Dropdown titreJoueurTexte;
    [SerializeField] Slider barreProgressionXP;
    [SerializeField] TextMeshProUGUI nombreGaucheXPTexte;
    [SerializeField] TextMeshProUGUI nombreDroiteXPTexte;
    [SerializeField] TextMeshProUGUI nombreVictoiresTexte;
    [SerializeField] TextMeshProUGUI meilleurTempsTexte;
    [SerializeField] TextMeshProUGUI nombreEnnemisTuésTexte;
    [SerializeField] TextMeshProUGUI niveauTexte;
    [SerializeField] TextMeshProUGUI joursSurvécusTexte;

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
        nombreEnnemisTuésTexte.text = GestionStatistiques.ObtenirNombreTotalEnnemisTuésProfil().ToString();
        niveauTexte.text = GestionStatistiques.ObtenirNiveauJoueur().ToString();
        joursSurvécusTexte.text = $"Jours survécus : {GestionStatistiques.ObtenirJoursSurvécusProfil()}";

        titreJoueurTexte.ClearOptions();
        titreJoueurTexte.AddOptions(GestionAchievements.ObtenirListeTitresDébloqués());
        int indiceTitreSélectionné = GestionAchievements.ObtenirListeTitresDébloqués().IndexOf(SauvegardeProfils.titreProfilActuel);
        titreJoueurTexte.value = indiceTitreSélectionné;

        imageProfil.sprite = ProfilManager.instance.ObtenirImage();
    }

    public void ChangerTitre()
    {
        SauvegardeProfils.SetTitreProfilActuel(titreJoueurTexte.captionText.text);//Jsp si c'est optimal mais j'espère ça fonctionne
    }


}
