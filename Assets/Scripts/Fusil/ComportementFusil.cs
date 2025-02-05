using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class RechargementEventArgs : EventArgs
{
    public float TempsRechargement { get; private set; }

    public RechargementEventArgs(float tempsRechargement)
    {
        TempsRechargement = tempsRechargement;
    }
}

public abstract class ComportementFusil : MonoBehaviour
{
    public event EventHandler OnTir;
    public event EventHandler<RechargementEventArgs> OnRechargement;
    RechargementEventArgs RechargeEventData;

    [SerializeField] public GameObject Balle;
    [SerializeField] protected LayerMask LayerAIgnorer;
    [SerializeField] TextMeshProUGUI TextnombreBallesChargeur;
    [SerializeField] TextMeshProUGUI TextnombreBallesChargeurTotal;
    [SerializeField] Canvas CanvasRechargement;

    public bool UnDesMenusEstActif = false; //si vrai, le joueur ne peut pas tirer
    public float DélaisRecharge { get; protected set; }
    public float DélaisEntreTir { get; protected set; }
    protected int NombreBalleParTir =1;

    const float DélaisPréRechargement = 0.3f; //Ajoute un petit délais avant la recharge automatique

    public int NombreBallesTotalesChargeur { get; protected set; }

    //Se charge de garder le compte et d'afficher le nombre de balle qu'il reste dans le chargeur à tout moment
    int nombreBallesPrésentementDansChargeur;
    int NombreBallesPrésentementDansChargeur
    {
        get { return nombreBallesPrésentementDansChargeur; }

        set
        {
            nombreBallesPrésentementDansChargeur = value;
            TextnombreBallesChargeur.text = value.ToString();
        }
    }
    
    float TempsÉcouléDepuisTir;
    
    bool EstEnRecharge = false;
    bool EstEntreDeuxTir = false;

    protected const float DistanceMaxTirVide = 100;
    protected const float RayonMaxDistance = 100;


    protected GameObject CameraJoueur;
    protected GameObject Embout;


    protected abstract bool ImputTirer(); //Change si l'arme est automatique ou semi-automatique
    protected abstract void InitiationRotationBalle(GameObject InstanceBalle);
    protected abstract void EffetsSonoresEtVisuelTirer();

    protected void Initialiser()
    {
        
        TempsÉcouléDepuisTir = 0;

        //Inscrit la fonction ChangerPossibilitéTir() à l'évenement qui averti les scripts qu'un des menu est activé
        GameManager.instance.GetComponent<ComportementInterface>().OnMenuChangement += ChangerPossibilitéTir;

        CameraJoueur = GetComponentInParent<Camera>().gameObject;
        Embout = transform.Find("Embout").gameObject;

        NombreBallesPrésentementDansChargeur = NombreBallesTotalesChargeur;
        RechargeEventData = new(DélaisRecharge);

        TextnombreBallesChargeurTotal.text = NombreBallesTotalesChargeur.ToString();
        
    }


    void Update()
    {
        
        TempsÉcouléDepuisTir += Time.deltaTime;

        if (PeutRecharger())
        {
            Recharger();
        }
        else if (PeutTirer())
        {
            Tirer();
            TempsÉcouléDepuisTir = 0;
        }



    }

    bool PeutRecharger()
    {
        return Input.GetKeyDown(KeyCode.R) && !EstEnRecharge 
                                           && NombreBallesPrésentementDansChargeur != NombreBallesTotalesChargeur;
    }

    bool PeutTirer()
    {
        return !EstEnRecharge && !EstEntreDeuxTir 
                              && !UnDesMenusEstActif 
                              && ImputTirer() 
                              && TempsÉcouléDepuisTir > DélaisEntreTir;
    }

    protected virtual bool ADesBallesDansLeChargeur()
    {
        return NombreBallesPrésentementDansChargeur > 0;
    }

    
    void Tirer()
    {
        if(ADesBallesDansLeChargeur())
        {
            NombreBallesPrésentementDansChargeur-= NombreBalleParTir;
            if (TextnombreBallesChargeur != null)
            {
                TextnombreBallesChargeur.text = NombreBallesPrésentementDansChargeur.ToString();

            }

            //Lancer de l'évennement qui est surtout utilisé pour les animation
            OnTir?.Invoke(this, EventArgs.Empty);

            InstancierBalle();
            EffetsSonoresEtVisuelTirer();
        }

        //recharge automatique si le joueur vient tout juste de tirer la dernière balle
        if(NombreBallesPrésentementDansChargeur <= 0)
        {
            StartCoroutine(AttendreEtRecharger());
        }
        
    }

    private IEnumerator AttendreEtRecharger()
    {
        yield return new WaitForSeconds(DélaisPréRechargement);
        Recharger();
    }

    protected virtual void Recharger()
    {
        OnRechargement?.Invoke(this, RechargeEventData);
        SliderRechargement.instance.LancerAnimation(DélaisRecharge);
        StartCoroutine(ResetChargeurFusil());
    }

    IEnumerator ResetChargeurFusil()
    {
        EstEnRecharge = true;
        
        TextnombreBallesChargeur.text = "--";

        //Attend le bon délais avant de réafficher un chargeur remplis
        yield return new WaitForSeconds(DélaisRecharge);

        EstEnRecharge = false;
       
        NombreBallesPrésentementDansChargeur = NombreBallesTotalesChargeur;

        
    }

    protected virtual void InstancierBalle()
    {
        var balleTemp = ObjectPool.instance.GetPoolObject(Balle);
        balleTemp.transform.position = Embout.transform.position;
        balleTemp.GetComponent<TrailRenderer>().enabled = true;
        InitiationRotationBalle(balleTemp);
        
        balleTemp.SetActive(true);
    }

    //si un des menus est activé ou d
    void ChangerPossibilitéTir(bool JeuEstEnCour)
    {
        
        UnDesMenusEstActif = !JeuEstEnCour; //si le jeu est en cours, aucuns menus n'est actifs

        if(CanvasRechargement != null)
        {
            CanvasRechargement.enabled = JeuEstEnCour;
        }
        
    }

    

    private void OnDisable()
    {
        if (TextnombreBallesChargeur != null)
        {
            TextnombreBallesChargeur.text = NombreBallesPrésentementDansChargeur.ToString();
        }
        
        EstEnRecharge = false;
        StopAllCoroutines();
    }
}
