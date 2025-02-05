using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le dragon attaque le joueur en lan�ant une boule de feu qui est affect�e par la gravit�
//selon un angle qu'il calcule pour atteindre la position du joueur au moment du lancer
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024
public class DragonEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Dragon;
    [SerializeField] GameObject prefabProjectile;
    [SerializeField] Transform emboutBouche;
    protected override float PV_INITIAL => 14;
    const float VITESSE_PROJECTILE = 17;
    protected override float Range => 15;
    //Voil� la formule si on veut le range max, je m'en servais au d�but mais plus maintenant
    //Mathf.Pow(VITESSE_PROJECTILE, 2) / Physics.gravity.magnitude; 
    
    protected override float Dur�eAttaque => 2;
    protected override float TempsEntreAttaques => 3;
    protected override string bruitAttaque => "AttaqueEnnemiDragon";
    protected override float VitesseAttaque => base.VitesseAttaque + 4;


    protected override void AttaquerJoueur()
    {
        StartCoroutine(AttendreEtLancerBalle());
    }
    private IEnumerator AttendreEtLancerBalle()
    {
        yield return new WaitForSeconds(Dur�eAttaque);
        if(�tatActuel != �tatEnnemi.Mort)
        {
            GameObject balle = Instantiate(prefabProjectile);
            balle.transform.position = emboutBouche.position;
            Vector3 directionLancement = ObtenirDirectionLancement();
            balle.GetComponent<Rigidbody>().velocity = directionLancement * VITESSE_PROJECTILE;

            GestionBruit.instance.JouerSon(bruitAttaque);
        }
    }

    //Les math�matiques derri�re l'algorithme utilis� sont expliqu�es dans le document word algorithmes
    private Vector3 ObtenirDirectionLancement()
    {
        Vector3 positionCible = joueur.transform.position;
        Vector3 positionRelativeCible = positionCible - emboutBouche.transform.position;

        float distanceHorizontale = Mathf.Sqrt(Mathf.Pow(positionRelativeCible.x, 2) + Mathf.Pow(positionRelativeCible.z, 2));
        float angle;
        float rangeMax = Mathf.Pow(VITESSE_PROJECTILE, 2) / Physics.gravity.magnitude;
        if (distanceHorizontale < rangeMax)
        {
            //Pr�sentement, on utilise la formule qui donne l'angle le plus petit, ce qui est logique pour un ennemi qui lance des projectiles
            angle = Mathf.Asin(Physics.gravity.magnitude * distanceHorizontale / Mathf.Pow(VITESSE_PROJECTILE, 2)) / 2;
        }
        else
        {
            //La fonction ne devrait pas �tre appel�e si le joueur est hors de range, mais si c'est le cas,
            //on prendra simplement un angle de 45 deg
            angle = Mathf.PI / 4;
        }
        //Il y a un l�ger probl�me caus� par le fait que j'assume que le d�placement en y du projectile est nul,
        //mais ce n'est pas le cas. Ceci fait que dans de grandes pentes le dragon peut rater sa cible.
        //Les 2 lignes ci-dessous servent � rectifier la situation. Math�matiquement, ce n'est pas valide.
        //Les valeurs trouv�es (Pi/45) ont �t� trouv�es exp�rimentalement, et le dragon est relativement assez pr�cis
        //avec ces modifications
        float d�calageY = positionRelativeCible.y;
        angle += Mathf.PI / 45 * d�calageY;

        Vector3 XZ = new Vector3(positionRelativeCible.x, 0, positionRelativeCible.z);
        float y = Mathf.Tan(angle) * XZ.magnitude;
        Vector3 nouveauVecteur = (new Vector3(XZ.x, y, XZ.z)).normalized;
        //On rotate le vecteur en y avec l'angle
        return nouveauVecteur;
    }
}
