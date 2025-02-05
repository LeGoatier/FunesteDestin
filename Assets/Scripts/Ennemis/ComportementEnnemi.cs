using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

//La classe ComportementEnnemi est parente à tous les comportements spécifiques des ennemis
//Elle s'occupe de plusieurs choses comme :
//-La machine à états finis
//-La prise de dégâts
//-Les animations
//-Le mouvement (patrouille et attaque) (avec la navigation à l'aide d'un navMeshAgent)
//
//Les seules choses que les comportements enfants ont à préciser sont :
//-L'attaque (par contre les ennemis qui attaque en mêlée peuvent facilement utiliser la méthode VérifierSiAttaqueMêléeTouche)
//-Les différentes caractéristiques comme la vie, la vitesse de déplacement en attaque, la durée de l'attaque,
//le temps entre les attaques, la portée (range), etc
//Cependant, la plupart des méthodes sont virtuelles et peuvent donc être précisées par les enfants si nécessaire
//
//Auteur : Justin Gauthier
//Date : 26 février 2024


public abstract class ComportementEnnemi : MonoBehaviour
{
    //L'enum ÉtatEnnemi représente les différents états de la machine à états finis de tous les ennemis
    protected enum ÉtatEnnemi { Patrouille, MouvementAttaque, PréparationAttaque, Attaque, Mort }


    //Navigation
    protected NavMeshAgent agent;
    protected GameObject joueur;
    protected virtual float VitesseAttaque => 4;
    protected virtual float VitessePatrouille => 2.5f;
    protected virtual float RayonDeDétection => 22;

    float tempsDernierChangementDeDirectionPatrouille;
    const float TempsEntreChangementsDeDirectionPatrouille = 4;

    //Vie
    protected abstract float PV_INITIAL { get; }
    protected float PV { get; set; }

    //Attaque
    protected virtual float TempsEntreAttaques => 2;
    protected float tempsDernièreAttaque;
    protected virtual float DuréeAttaque => 0.5f;
    protected GestionVieJoueur gestionVieJoueur;

    //États
    protected ÉtatEnnemi étatActuel = ÉtatEnnemi.Patrouille;

    //Animations
    protected Animator animator;
    protected virtual bool A2Attaques => false;

    protected virtual float Range => 3.3f;
    private float tempsDernièreAnimationPriseDégât;

    //Constantes
    const float TEMPS_MORT = 3.3f;
    const float VITESSE_ROTATION = 5;

    // UI hitmark
    ComportementHitmark hitMark;

    //Bruit des ennemis
    protected virtual string bruitAttaque => "AttaqueEnnemiDefault";
    protected virtual string bruitDegats => "DegatEnnemiDefault";

    //Type pour l'enum
    protected abstract Ennemi typeEnnemi { get; }

    private void Awake()
    {
        //Initialisation des composantes pour la navigation
        joueur = GameObject.FindGameObjectWithTag("Player");
        gestionVieJoueur = joueur.GetComponent<GestionVieJoueur>();
        agent = GetComponent<NavMeshAgent>();
        
        //Pour l'animation
        animator = GetComponent<Animator>();

        hitMark = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ComportementHitmark>();
    }

    private void OnEnable()
    {
        PV = PV_INITIAL;
        tempsDernierChangementDeDirectionPatrouille = float.MinValue;//Pour qu'il se choisisse une direction tout de suite
        //Pour la patrouille, l'ennemi doit se rendre à la destination qu'il se fixe
        //Les fonctions de mouvement d'attaque préciseront si la distance d'arrêt change
        agent.stoppingDistance = 0;
        agent.speed = VitessePatrouille;
        étatActuel = ÉtatEnnemi.Patrouille;
        animator.Rebind();
        animator.Update(0);
    }

    protected virtual void Update()
    {
        //La gestion de la machine à état finie pourrait être optimisée, mais présentement, selon l'état actuel
        //le comportement est différent et chaque état a une manière de changer l'état actuel selon des conditions
        //propres à chaque état
        if(étatActuel == ÉtatEnnemi.Patrouille)
        {
            TenterDétecterJoueur();
            MouvementPatrouille();
        }
        else if(étatActuel == ÉtatEnnemi.MouvementAttaque)
        {
            MouvementAttaque();
            VérifierSiInRange();
        }
        else if(étatActuel == ÉtatEnnemi.PréparationAttaque)
        {
            VérifierSiInRange();
            VérifierSiAttaque();
            if (EstEnRange())
            {
                TournerVersJoueur();
            }
        }
        else if(étatActuel == ÉtatEnnemi.Attaque)
        {
            if (EstAttaqueFinie())
            {
                étatActuel = ÉtatEnnemi.PréparationAttaque;
            }
            if (EstEnRange())
            {
                TournerVersJoueur();
            }
        }
    }

    private bool EstAttaqueFinie()
    {
        return tempsDernièreAttaque + DuréeAttaque < Time.time;
    }

    protected virtual void VérifierSiAttaque()
    {
        if (EstEnRange() && EstAttaquePrête())
        {
            AttaquerJoueur();
            tempsDernièreAttaque = Time.time;
            étatActuel = ÉtatEnnemi.Attaque;
            if (A2Attaques)
            {
                if(UnityEngine.Random.value < 0.5f)
                {
                    animator.SetTrigger("Attaque");
                }
                else
                {
                    animator.SetTrigger("Attaque2");
                }
            }
            else
            {
                animator.SetTrigger("Attaque");
            }
        }
    }

    private void VérifierSiInRange()
    {
        if (EstEnRange())
        {
            animator.SetBool("InRange", true);
            étatActuel = ÉtatEnnemi.PréparationAttaque;
        }
        else
        {
            animator.SetBool("InRange", false);
            étatActuel = ÉtatEnnemi.MouvementAttaque;
        }
    }

    protected bool EstAttaquePrête()
    {
        return tempsDernièreAttaque + TempsEntreAttaques < Time.time;
    }
    protected bool EstEnRange()
    {
        return Vector3.Distance(joueur.transform.position, transform.position) <= Range;
    }
    protected abstract void AttaquerJoueur();

    private void TenterDétecterJoueur()
    {
        if (EstAssezProche()) //J'ai retiré EstEnLigneDeMire parce que c'était pas tant utile et ça causait des problèmes
        {
            DétecterJoueur();
        }
    }

    private void DétecterJoueur()
    {
        étatActuel = ÉtatEnnemi.MouvementAttaque;
        animator.SetTrigger("ADétectéJoueur");
        agent.speed = VitesseAttaque;
        agent.stoppingDistance = Range - 0.3f;
    }

    private bool EstAssezProche()
    {
        return (transform.position - joueur.transform.position).magnitude <= RayonDeDétection;
    }
    //Même si cette méthode n'est plus utilisée, je la conserve au cas où on voudrait s'en servir de nouveau
    private bool EstEnLigneDeMire()
    {
        //On ne précise pas la distance maximale parce que comme le ray est instantané il devrait
        //toucher le joueur à moins qu'il y ait un obstacle avant
        Physics.Raycast(new Ray(transform.position, joueur.transform.position - transform.position), out RaycastHit hit, 50, ~LayerMask.NameToLayer("IgnoreRayFusil"));
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            return true;
        }
        return false;
    }

    //Le mouvement de l'ennemi avant d'avoir détecté le joueur
    //Par défaut, il est pareil pour tous les ennemis, mais les ennemis peuvent le préciser
    protected virtual void MouvementPatrouille()
    {
        //L'idée est qu'en général on veut que l'ennemi se rapproche du joueur,
        //sans toutefois aller vers lui en ligne droite
        if(tempsDernierChangementDeDirectionPatrouille + TempsEntreChangementsDeDirectionPatrouille < Time.time)
        {
            tempsDernierChangementDeDirectionPatrouille = Time.time;
            SélectionnerNouvelleDestinationPatrouille();
        }
    }

    GaussianRandom randomNormal = new GaussianRandom();
    private void SélectionnerNouvelleDestinationPatrouille()
    {
        //Obtenir la direction du vecteur vers le joueur
        Vector3 directionJoueurUnitaire = (joueur.transform.position - transform.position).normalized;
        //Choisir une variation d'angle selon une distribution normale qui sera seulement appliquée en y
        const float ÉCART_TYPE = 70;
        //La moyenne de changement est 0 (vers le joueur), et l'écart-type pourrait changer mais avec 90 on sait que
        //la majorité du temps, l'ennemi va aller vers le joueur, mais qu'il pourrait s'en éloigner
        float changementAngle = (float)randomNormal.NextGaussian(0, ÉCART_TYPE);
        //Appliquer la rotation pour obtenir la nouvelle direction
        Quaternion rotation = Quaternion.Euler(0, changementAngle, 0);
        Vector3 nouvelleDirection = rotation * directionJoueurUnitaire;
        //Calculer la distance. Présentement on prend la distance maximale possible mais on pourrait réduire si
        //on veut que l'ennemi ne soit pas toujours en mouvement lorsqu'il patrouille
        float distanceProchaineDestination = TempsEntreChangementsDeDirectionPatrouille * VitessePatrouille;
        //Définir la nouvelle destination
        Vector3 nouvelleDestination = transform.position + nouvelleDirection * distanceProchaineDestination;
        agent.destination = nouvelleDestination;
    }

    //Le code qui suit vient d'internet (chat GPT) parce que unity ne propose pas de fonction
    //pour implémenter une distribution normale
    internal class GaussianRandom
    {
        private Random random;
        private bool hasDeviate;
        private double storedDeviate;

        public GaussianRandom()
        {
            random = new Random();
            hasDeviate = false;
            storedDeviate = 0;
        }

        public double NextGaussian(double mu = 0, double sigma = 1)
        {
            if (hasDeviate)
            {
                hasDeviate = false;
                return storedDeviate * sigma + mu;
            }
            else
            {
                double v1, v2, s;
                do
                {
                    v1 = 2.0 * random.NextDouble() - 1.0;
                    v2 = 2.0 * random.NextDouble() - 1.0;
                    s = v1 * v1 + v2 * v2;
                } while (s >= 1.0 || s == 0);

                double multiplier = Math.Sqrt(-2.0 * Math.Log(s) / s);
                storedDeviate = v2 * multiplier;
                hasDeviate = true;
                return mu + sigma * v1 * multiplier;
            }
        }
    }

    //La version par défaut du mouvement d'attaque consiste seulement à aller vers le joueur, mais
    //il est possible de override pour un ennemi qui aurait des esquives ou des comportements plus complexes
    protected virtual void MouvementAttaque()
    {
        agent.destination = joueur.transform.position;
    }

    public virtual void PrendreDégats(float dommage)
    {
        if(étatActuel != ÉtatEnnemi.Mort)
        {
            PV -= dommage;
            if (PV <= 0)
            {
                GestionStatistiques.AjouterEnnemiTué(typeEnnemi);
                hitMark.AfficherRouge();
                étatActuel = ÉtatEnnemi.Mort;
                agent.isStopped = true;
                animator.SetTrigger("Mort");
                GetComponent<CapsuleCollider>().enabled = false;
                StartCoroutine(RoutineMort());
            }
            else
            {
                hitMark.AfficherBlanc();
                //Si l'ennemi est dans son mouvement d'attaque, on veut qu'il continue son animation,
                //autrement le joueur prendrait des dégâts sans voir l'ennemi frapper
                //On ajoute un buffer après l'attaque car l'état fini au moment où l'arme frappe, non la fin de l'animation
                const float BUFFER_FIN_ANIMATION = 0.5f;
                if (tempsDernièreAttaque + DuréeAttaque + BUFFER_FIN_ANIMATION < Time.time && EstAssezLoinDernièreAnimation())
                {
                    animator.SetTrigger("AÉtéFrappé");
                    tempsDernièreAnimationPriseDégât = Time.time;
                }
            }
            GestionBruit.instance.JouerSon3D(gameObject, bruitDegats);

            if (étatActuel == ÉtatEnnemi.Patrouille)
                DétecterJoueur(); //Une fois touché, l'ennemi détecte tout de suite le joueur si ce n'était pas fait
        }
    }
    //Cette fonction est seulement appelée par ennemy spawner par cycle jour lorsque le jour commence
    public void Mourir()
    {
        if (étatActuel != ÉtatEnnemi.Mort)
        {
            étatActuel = ÉtatEnnemi.Mort;
            agent.isStopped = true;
            animator.SetTrigger("Mort");
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(RoutineMort());
        }
    }

    const float DURÉE_ANIMATION_DÉGÂT = 0.5f;
    private bool EstAssezLoinDernièreAnimation()
    {
        return tempsDernièreAnimationPriseDégât + DURÉE_ANIMATION_DÉGÂT < Time.time;
    }

    private IEnumerator RoutineMort()
    {
        yield return new WaitForSeconds(TEMPS_MORT);
        EnnemySpawner.instance.RetirerEnnemiDeListe(gameObject);
        Destroy(gameObject);
    }

    private void TournerVersJoueur()
    {
        Vector3 direction = (joueur.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * VITESSE_ROTATION);
    }

    //Fonction générale qui peut être utilisée par les enfants pour vérifier si une attaque de mêlée touche le joueur
    //L'idée est d'envoyer des rayons dans un certain angle devant l'ennemi au moment où son arme devrait toucher
    //de la longueur du range, de manière à vérifier si le joueur s'y trouve encore
    protected IEnumerator VérifierSiAttaqueMêléeTouche(float dégats, int nombreRayons, int angleÉcartRayons)
    {
        yield return new WaitForSeconds(DuréeAttaque);

        bool ATouché = false;

        int nombreDeÉcart = nombreRayons - 1;
        int angleInitial = -(nombreDeÉcart * angleÉcartRayons / 2);//Autant de rayons à droite qu'à gauche

        for (int angle = angleInitial; angle <= -angleInitial; angle += angleÉcartRayons)
        {
            //On fait la rotation du vecteur selon l'axe des y
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position, direction, Range))
            {
                //Je prends pas la peine de vérifier que le collider touché est le joueur, car les situations
                //ou ce ne serait pas le cas sont extrêmement rares
                ATouché = true;
            }
        }

        if (ATouché && étatActuel != ÉtatEnnemi.Mort)
            gestionVieJoueur.RecevoirDégat(dégats);

        GestionBruit.instance.JouerSon3D(gameObject,bruitAttaque);
    }
}
