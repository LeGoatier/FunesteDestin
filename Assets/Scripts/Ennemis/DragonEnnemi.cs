using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Le dragon attaque le joueur en lançant une boule de feu qui est affectée par la gravité
//selon un angle qu'il calcule pour atteindre la position du joueur au moment du lancer
//Auteur : Justin Gauthier
//Date : 26 février 2024
public class DragonEnnemi : ComportementEnnemi
{
    protected override Ennemi typeEnnemi => Ennemi.Dragon;
    [SerializeField] GameObject prefabProjectile;
    [SerializeField] Transform emboutBouche;
    protected override float PV_INITIAL => 14;
    const float VITESSE_PROJECTILE = 17;
    protected override float Range => 15;
    //Voilà la formule si on veut le range max, je m'en servais au début mais plus maintenant
    //Mathf.Pow(VITESSE_PROJECTILE, 2) / Physics.gravity.magnitude; 
    
    protected override float DuréeAttaque => 2;
    protected override float TempsEntreAttaques => 3;
    protected override string bruitAttaque => "AttaqueEnnemiDragon";
    protected override float VitesseAttaque => base.VitesseAttaque + 4;


    protected override void AttaquerJoueur()
    {
        StartCoroutine(AttendreEtLancerBalle());
    }
    private IEnumerator AttendreEtLancerBalle()
    {
        yield return new WaitForSeconds(DuréeAttaque);
        if(étatActuel != ÉtatEnnemi.Mort)
        {
            GameObject balle = Instantiate(prefabProjectile);
            balle.transform.position = emboutBouche.position;
            Vector3 directionLancement = ObtenirDirectionLancement();
            balle.GetComponent<Rigidbody>().velocity = directionLancement * VITESSE_PROJECTILE;

            GestionBruit.instance.JouerSon(bruitAttaque);
        }
    }

    //Les mathématiques derrière l'algorithme utilisé sont expliquées dans le document word algorithmes
    private Vector3 ObtenirDirectionLancement()
    {
        Vector3 positionCible = joueur.transform.position;
        Vector3 positionRelativeCible = positionCible - emboutBouche.transform.position;

        float distanceHorizontale = Mathf.Sqrt(Mathf.Pow(positionRelativeCible.x, 2) + Mathf.Pow(positionRelativeCible.z, 2));
        float angle;
        float rangeMax = Mathf.Pow(VITESSE_PROJECTILE, 2) / Physics.gravity.magnitude;
        if (distanceHorizontale < rangeMax)
        {
            //Présentement, on utilise la formule qui donne l'angle le plus petit, ce qui est logique pour un ennemi qui lance des projectiles
            angle = Mathf.Asin(Physics.gravity.magnitude * distanceHorizontale / Mathf.Pow(VITESSE_PROJECTILE, 2)) / 2;
        }
        else
        {
            //La fonction ne devrait pas être appelée si le joueur est hors de range, mais si c'est le cas,
            //on prendra simplement un angle de 45 deg
            angle = Mathf.PI / 4;
        }
        //Il y a un léger problème causé par le fait que j'assume que le déplacement en y du projectile est nul,
        //mais ce n'est pas le cas. Ceci fait que dans de grandes pentes le dragon peut rater sa cible.
        //Les 2 lignes ci-dessous servent à rectifier la situation. Mathématiquement, ce n'est pas valide.
        //Les valeurs trouvées (Pi/45) ont été trouvées expérimentalement, et le dragon est relativement assez précis
        //avec ces modifications
        float décalageY = positionRelativeCible.y;
        angle += Mathf.PI / 45 * décalageY;

        Vector3 XZ = new Vector3(positionRelativeCible.x, 0, positionRelativeCible.z);
        float y = Mathf.Tan(angle) * XZ.magnitude;
        Vector3 nouveauVecteur = (new Vector3(XZ.x, y, XZ.z)).normalized;
        //On rotate le vecteur en y avec l'angle
        return nouveauVecteur;
    }
}
