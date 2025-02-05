using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RésuméPartieCanevas : MonoBehaviour
{
    public static RésuméPartieCanevas instance;

    //Général
    [Header("Zone générale")]
    [SerializeField] TextMeshProUGUI messageTitre;
    [SerializeField] TextMeshProUGUI nomPartie;
    [SerializeField] TextMeshProUGUI difficulté;
    [SerializeField] TextMeshProUGUI seed;

    //Statistiques
    [Header("Zone statistique")]
    [SerializeField] TextMeshProUGUI duréePartieTexte;
    [SerializeField] TextMeshProUGUI nbEnnemisTuésTexte;

    //Barre d'expérience

    [Header("Zone barre d'xp")]
    [SerializeField] Slider barreProgressionXP;
    [SerializeField] TextMeshProUGUI nombreGaucheXPTexte;
    [SerializeField] TextMeshProUGUI nombreDroiteXPTexte;
    static float ancienneProgression = 0;
    static float ancienNiveau = 1;


    [Header("Zone Achievements")]
    [SerializeField] GameObject succes;
    [SerializeField] VerticalLayoutGroup liste;
    public Sprite[] badges = new Sprite[(int)Rareté.NbRareté];

    private void Awake()
    {
        instance = this;
    }

    private void OnEnable()
    {
        InitialiserMessagesTextesGénéral();
        InitialiserTextesStatistiques();
        AfficherAchievementsPartie();
    }

   

    private void InitialiserMessagesTextesGénéral()
    {
        switch (GestionScenes.étatFinPartieActuel)
        {
            case ÉtatFinPartie.Bateau: 
                messageTitre.text = "Vous vous êtes enfui par bateau... ou presque";
                break;
            case ÉtatFinPartie.Antenne:
                messageTitre.text = "Vous avez appelé de l'aide par une antenne radio... mais personne ne vous sauvera à présent";
                break;
            case ÉtatFinPartie.Fusée:
                messageTitre.text = "Une fusée de détresse a été lancée... pour mieux retomber";
                break;
            case ÉtatFinPartie.Mort:
                messageTitre.text = "Vous êtes mort de votre propre incompétence";
                break;
        }
        if (InfoPartie.nom != "" || InfoPartie.nom != " ")
            nomPartie.text = InfoPartie.nom;
        else
            nomPartie.text = "Nouvelle partie";

        switch (InfoPartie.difficulté)
        {
            case Difficulté.paisible:
                difficulté.text = "Difficulté : Paisible";
                break;
            case Difficulté.facile:
                difficulté.text = "Difficulté : Facile";
                break;
            case Difficulté.modérée:
                difficulté.text = "Difficulté : Modérée";
                break;
            case Difficulté.difficile:
                difficulté.text = "Difficulté : Difficile";
                break;
        }

        seed.text = $"Seed de la partie : {InfoPartie.seed}";
    }

    private void InitialiserTextesStatistiques()
    {
        //Durée de la partie
        int nombreSecondesTotal = (int)Mathf.Round(GestionStatistiques.ObtenirDuréePartieActuelle());
        int nombreMinutes = nombreSecondesTotal / 60;
        int nombreSecondesRestantes = nombreSecondesTotal % 60;

        duréePartieTexte.text = $"Vous avez survécu {nombreMinutes} min {nombreSecondesRestantes} s";

        //Nombre d'ennemis tués
        nbEnnemisTuésTexte.text = $"{GestionStatistiques.ObtenirNombreTotalEnnemisTuésPartie()} monstres tués";
    }

    private void AfficherAchievementsPartie()
    {
        //On commence par aller chercher les prefabs des notifications des achievements
        //gagnés dans la partie qui vient d'être terminée
        List<Achievements> achievementsAAfficher = GestionAchievements.ObtenirAchievementsAAfficherFinPartie();
     
        foreach (Achievements achievement in achievementsAAfficher)
        {
            GameObject succès = Instantiate(succes);
            succès.GetComponent<SuccèsFin>().SetAchievement(achievement, GestionAchievements.GetRaretéAchievement(achievement));
            succès.transform.SetParent(liste.gameObject.transform);
        }
       


        //Ensuite, on instancie et positionne chacune de ces notifications, puis on les désactive
        //List<GameObject> objetsNotifications = new List<GameObject>();
        ////Vous pouvez jouer avec ces constantes pour changer la position d'affichage des achievements
        ////à la fin. Aussi, pour l'instant, ce code affiche tous les achievements dans la même colonne,
        ////mais si éventuellement il y en a trop et on veut gérer 2 ou plusieurs colonnes c'est possible,
        ////il faudra changer la fonction un peu
        //const float DÉCALAGE_Y = 30;
        //const float POSITION_X = -50;
        //const float POSITION_Y_PREMIER = 0;
        //for (int i = 0; i < objetsNotificationsPrefabs.Count; i++)
        //{
        //    objetsNotifications[i] = Instantiate(objetsNotificationsPrefabs[i]);
        //    objetsNotifications[i].transform.position = new Vector3(POSITION_X, POSITION_Y_PREMIER + i * DÉCALAGE_Y, 0);
        //    objetsNotifications[i].SetActive(false);
        //}

        ////Finalement, on les active un après l'autre avec une animation
        //const float DÉLAI_ENTRE_ACHIVEMENTS = 1;
        //for(int i = 0; i < objetsNotifications.Count; i++)
        //{
        //    StartCoroutine(FadeInAprèsDélai(objetsNotifications[i], i * DÉLAI_ENTRE_ACHIVEMENTS));
        //}
    }

    //Comme pour les autres notifications, j'ai seulement activé et non pas un fade in parce que
    //je sais pas d'avance comment ça va être fait avec un gameObject
    //private IEnumerator FadeInAprèsDélai(GameObject notification, float délai)
    //{
    //    yield return new WaitForSeconds(délai);
    //    notification.SetActive(true);
    //}

    //Fonction appelée par le bouton
    public void RetournerAuMenu()
    {
        GestionBruit.instance.JouerChansonMenu();
        GestionScenes.ChargerMenuPrincipal(false);
    }

    public void AnimerBarreXP()
    {
        barreProgressionXP.value = ancienneProgression;
        nombreGaucheXPTexte.text = ancienNiveau.ToString();
        nombreDroiteXPTexte.text = (ancienNiveau + 1).ToString();

        if(ancienNiveau < GestionStatistiques.ObtenirNiveauJoueur())
        {
            StartCoroutine(AvancerBarreProchainNiveau());
        }
        else
        {
            StartCoroutine(AvancerBarreMemeNiveau());
        }
    }
    const float vitesseProgression = 0.5f;
    private IEnumerator AvancerBarreProchainNiveau()
    {
        float valeur = ancienneProgression;
        while(valeur < 1)
        {
            valeur += vitesseProgression * Time.deltaTime;
            barreProgressionXP.value = valeur;
            yield return null;
        }
        ancienNiveau += 1;
        ancienneProgression = 0;
        AnimerBarreXP();
    }
    private IEnumerator AvancerBarreMemeNiveau()
    {
        float valeur = ancienneProgression;
        while (valeur < GestionStatistiques.ObtenirProgressionXP())
        {
            valeur += vitesseProgression * Time.deltaTime;
            barreProgressionXP.value = valeur;
            yield return null;
        }
        ancienneProgression = GestionStatistiques.ObtenirProgressionXP();
    }
}
