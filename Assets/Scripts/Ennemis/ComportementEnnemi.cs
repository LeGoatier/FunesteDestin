using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

//La classe ComportementEnnemi est parente � tous les comportements sp�cifiques des ennemis
//Elle s'occupe de plusieurs choses comme :
//-La machine � �tats finis
//-La prise de d�g�ts
//-Les animations
//-Le mouvement (patrouille et attaque) (avec la navigation � l'aide d'un navMeshAgent)
//
//Les seules choses que les comportements enfants ont � pr�ciser sont :
//-L'attaque (par contre les ennemis qui attaque en m�l�e peuvent facilement utiliser la m�thode V�rifierSiAttaqueM�l�eTouche)
//-Les diff�rentes caract�ristiques comme la vie, la vitesse de d�placement en attaque, la dur�e de l'attaque,
//le temps entre les attaques, la port�e (range), etc
//Cependant, la plupart des m�thodes sont virtuelles et peuvent donc �tre pr�cis�es par les enfants si n�cessaire
//
//Auteur : Justin Gauthier
//Date : 26 f�vrier 2024


public abstract class ComportementEnnemi : MonoBehaviour
{
    //L'enum �tatEnnemi repr�sente les diff�rents �tats de la machine � �tats finis de tous les ennemis
    protected enum �tatEnnemi { Patrouille, MouvementAttaque, Pr�parationAttaque, Attaque, Mort }


    //Navigation
    protected NavMeshAgent agent;
    protected GameObject joueur;
    protected virtual float VitesseAttaque => 4;
    protected virtual float VitessePatrouille => 2.5f;
    protected virtual float RayonDeD�tection => 22;

    float tempsDernierChangementDeDirectionPatrouille;
    const float TempsEntreChangementsDeDirectionPatrouille = 4;

    //Vie
    protected abstract float PV_INITIAL { get; }
    protected float PV { get; set; }

    //Attaque
    protected virtual float TempsEntreAttaques => 2;
    protected float tempsDerni�reAttaque;
    protected virtual float Dur�eAttaque => 0.5f;
    protected GestionVieJoueur gestionVieJoueur;

    //�tats
    protected �tatEnnemi �tatActuel = �tatEnnemi.Patrouille;

    //Animations
    protected Animator animator;
    protected virtual bool A2Attaques => false;

    protected virtual float Range => 3.3f;
    private float tempsDerni�reAnimationPriseD�g�t;

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
        //Pour la patrouille, l'ennemi doit se rendre � la destination qu'il se fixe
        //Les fonctions de mouvement d'attaque pr�ciseront si la distance d'arr�t change
        agent.stoppingDistance = 0;
        agent.speed = VitessePatrouille;
        �tatActuel = �tatEnnemi.Patrouille;
        animator.Rebind();
        animator.Update(0);
    }

    protected virtual void Update()
    {
        //La gestion de la machine � �tat finie pourrait �tre optimis�e, mais pr�sentement, selon l'�tat actuel
        //le comportement est diff�rent et chaque �tat a une mani�re de changer l'�tat actuel selon des conditions
        //propres � chaque �tat
        if(�tatActuel == �tatEnnemi.Patrouille)
        {
            TenterD�tecterJoueur();
            MouvementPatrouille();
        }
        else if(�tatActuel == �tatEnnemi.MouvementAttaque)
        {
            MouvementAttaque();
            V�rifierSiInRange();
        }
        else if(�tatActuel == �tatEnnemi.Pr�parationAttaque)
        {
            V�rifierSiInRange();
            V�rifierSiAttaque();
            if (EstEnRange())
            {
                TournerVersJoueur();
            }
        }
        else if(�tatActuel == �tatEnnemi.Attaque)
        {
            if (EstAttaqueFinie())
            {
                �tatActuel = �tatEnnemi.Pr�parationAttaque;
            }
            if (EstEnRange())
            {
                TournerVersJoueur();
            }
        }
    }

    private bool EstAttaqueFinie()
    {
        return tempsDerni�reAttaque + Dur�eAttaque < Time.time;
    }

    protected virtual void V�rifierSiAttaque()
    {
        if (EstEnRange() && EstAttaquePr�te())
        {
            AttaquerJoueur();
            tempsDerni�reAttaque = Time.time;
            �tatActuel = �tatEnnemi.Attaque;
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

    private void V�rifierSiInRange()
    {
        if (EstEnRange())
        {
            animator.SetBool("InRange", true);
            �tatActuel = �tatEnnemi.Pr�parationAttaque;
        }
        else
        {
            animator.SetBool("InRange", false);
            �tatActuel = �tatEnnemi.MouvementAttaque;
        }
    }

    protected bool EstAttaquePr�te()
    {
        return tempsDerni�reAttaque + TempsEntreAttaques < Time.time;
    }
    protected bool EstEnRange()
    {
        return Vector3.Distance(joueur.transform.position, transform.position) <= Range;
    }
    protected abstract void AttaquerJoueur();

    private void TenterD�tecterJoueur()
    {
        if (EstAssezProche()) //J'ai retir� EstEnLigneDeMire parce que c'�tait pas tant utile et �a causait des probl�mes
        {
            D�tecterJoueur();
        }
    }

    private void D�tecterJoueur()
    {
        �tatActuel = �tatEnnemi.MouvementAttaque;
        animator.SetTrigger("AD�tect�Joueur");
        agent.speed = VitesseAttaque;
        agent.stoppingDistance = Range - 0.3f;
    }

    private bool EstAssezProche()
    {
        return (transform.position - joueur.transform.position).magnitude <= RayonDeD�tection;
    }
    //M�me si cette m�thode n'est plus utilis�e, je la conserve au cas o� on voudrait s'en servir de nouveau
    private bool EstEnLigneDeMire()
    {
        //On ne pr�cise pas la distance maximale parce que comme le ray est instantan� il devrait
        //toucher le joueur � moins qu'il y ait un obstacle avant
        Physics.Raycast(new Ray(transform.position, joueur.transform.position - transform.position), out RaycastHit hit, 50, ~LayerMask.NameToLayer("IgnoreRayFusil"));
        if (hit.collider != null && hit.collider.tag == "Player")
        {
            return true;
        }
        return false;
    }

    //Le mouvement de l'ennemi avant d'avoir d�tect� le joueur
    //Par d�faut, il est pareil pour tous les ennemis, mais les ennemis peuvent le pr�ciser
    protected virtual void MouvementPatrouille()
    {
        //L'id�e est qu'en g�n�ral on veut que l'ennemi se rapproche du joueur,
        //sans toutefois aller vers lui en ligne droite
        if(tempsDernierChangementDeDirectionPatrouille + TempsEntreChangementsDeDirectionPatrouille < Time.time)
        {
            tempsDernierChangementDeDirectionPatrouille = Time.time;
            S�lectionnerNouvelleDestinationPatrouille();
        }
    }

    GaussianRandom randomNormal = new GaussianRandom();
    private void S�lectionnerNouvelleDestinationPatrouille()
    {
        //Obtenir la direction du vecteur vers le joueur
        Vector3 directionJoueurUnitaire = (joueur.transform.position - transform.position).normalized;
        //Choisir une variation d'angle selon une distribution normale qui sera seulement appliqu�e en y
        const float �CART_TYPE = 70;
        //La moyenne de changement est 0 (vers le joueur), et l'�cart-type pourrait changer mais avec 90 on sait que
        //la majorit� du temps, l'ennemi va aller vers le joueur, mais qu'il pourrait s'en �loigner
        float changementAngle = (float)randomNormal.NextGaussian(0, �CART_TYPE);
        //Appliquer la rotation pour obtenir la nouvelle direction
        Quaternion rotation = Quaternion.Euler(0, changementAngle, 0);
        Vector3 nouvelleDirection = rotation * directionJoueurUnitaire;
        //Calculer la distance. Pr�sentement on prend la distance maximale possible mais on pourrait r�duire si
        //on veut que l'ennemi ne soit pas toujours en mouvement lorsqu'il patrouille
        float distanceProchaineDestination = TempsEntreChangementsDeDirectionPatrouille * VitessePatrouille;
        //D�finir la nouvelle destination
        Vector3 nouvelleDestination = transform.position + nouvelleDirection * distanceProchaineDestination;
        agent.destination = nouvelleDestination;
    }

    //Le code qui suit vient d'internet (chat GPT) parce que unity ne propose pas de fonction
    //pour impl�menter une distribution normale
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

    //La version par d�faut du mouvement d'attaque consiste seulement � aller vers le joueur, mais
    //il est possible de override pour un ennemi qui aurait des esquives ou des comportements plus complexes
    protected virtual void MouvementAttaque()
    {
        agent.destination = joueur.transform.position;
    }

    public virtual void PrendreD�gats(float dommage)
    {
        if(�tatActuel != �tatEnnemi.Mort)
        {
            PV -= dommage;
            if (PV <= 0)
            {
                GestionStatistiques.AjouterEnnemiTu�(typeEnnemi);
                hitMark.AfficherRouge();
                �tatActuel = �tatEnnemi.Mort;
                agent.isStopped = true;
                animator.SetTrigger("Mort");
                GetComponent<CapsuleCollider>().enabled = false;
                StartCoroutine(RoutineMort());
            }
            else
            {
                hitMark.AfficherBlanc();
                //Si l'ennemi est dans son mouvement d'attaque, on veut qu'il continue son animation,
                //autrement le joueur prendrait des d�g�ts sans voir l'ennemi frapper
                //On ajoute un buffer apr�s l'attaque car l'�tat fini au moment o� l'arme frappe, non la fin de l'animation
                const float BUFFER_FIN_ANIMATION = 0.5f;
                if (tempsDerni�reAttaque + Dur�eAttaque + BUFFER_FIN_ANIMATION < Time.time && EstAssezLoinDerni�reAnimation())
                {
                    animator.SetTrigger("A�t�Frapp�");
                    tempsDerni�reAnimationPriseD�g�t = Time.time;
                }
            }
            GestionBruit.instance.JouerSon3D(gameObject, bruitDegats);

            if (�tatActuel == �tatEnnemi.Patrouille)
                D�tecterJoueur(); //Une fois touch�, l'ennemi d�tecte tout de suite le joueur si ce n'�tait pas fait
        }
    }
    //Cette fonction est seulement appel�e par ennemy spawner par cycle jour lorsque le jour commence
    public void Mourir()
    {
        if (�tatActuel != �tatEnnemi.Mort)
        {
            �tatActuel = �tatEnnemi.Mort;
            agent.isStopped = true;
            animator.SetTrigger("Mort");
            GetComponent<CapsuleCollider>().enabled = false;
            StartCoroutine(RoutineMort());
        }
    }

    const float DUR�E_ANIMATION_D�G�T = 0.5f;
    private bool EstAssezLoinDerni�reAnimation()
    {
        return tempsDerni�reAnimationPriseD�g�t + DUR�E_ANIMATION_D�G�T < Time.time;
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

    //Fonction g�n�rale qui peut �tre utilis�e par les enfants pour v�rifier si une attaque de m�l�e touche le joueur
    //L'id�e est d'envoyer des rayons dans un certain angle devant l'ennemi au moment o� son arme devrait toucher
    //de la longueur du range, de mani�re � v�rifier si le joueur s'y trouve encore
    protected IEnumerator V�rifierSiAttaqueM�l�eTouche(float d�gats, int nombreRayons, int angle�cartRayons)
    {
        yield return new WaitForSeconds(Dur�eAttaque);

        bool ATouch� = false;

        int nombreDe�cart = nombreRayons - 1;
        int angleInitial = -(nombreDe�cart * angle�cartRayons / 2);//Autant de rayons � droite qu'� gauche

        for (int angle = angleInitial; angle <= -angleInitial; angle += angle�cartRayons)
        {
            //On fait la rotation du vecteur selon l'axe des y
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            if (Physics.Raycast(transform.position, direction, Range))
            {
                //Je prends pas la peine de v�rifier que le collider touch� est le joueur, car les situations
                //ou ce ne serait pas le cas sont extr�mement rares
                ATouch� = true;
            }
        }

        if (ATouch� && �tatActuel != �tatEnnemi.Mort)
            gestionVieJoueur.RecevoirD�gat(d�gats);

        GestionBruit.instance.JouerSon3D(gameObject,bruitAttaque);
    }
}
