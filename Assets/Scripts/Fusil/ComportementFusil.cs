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
    public float D�laisRecharge { get; protected set; }
    public float D�laisEntreTir { get; protected set; }
    protected int NombreBalleParTir =1;

    const float D�laisPr�Rechargement = 0.3f; //Ajoute un petit d�lais avant la recharge automatique

    public int NombreBallesTotalesChargeur { get; protected set; }

    //Se charge de garder le compte et d'afficher le nombre de balle qu'il reste dans le chargeur � tout moment
    int nombreBallesPr�sentementDansChargeur;
    int NombreBallesPr�sentementDansChargeur
    {
        get { return nombreBallesPr�sentementDansChargeur; }

        set
        {
            nombreBallesPr�sentementDansChargeur = value;
            TextnombreBallesChargeur.text = value.ToString();
        }
    }
    
    float Temps�coul�DepuisTir;
    
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
        
        Temps�coul�DepuisTir = 0;

        //Inscrit la fonction ChangerPossibilit�Tir() � l'�venement qui averti les scripts qu'un des menu est activ�
        GameManager.instance.GetComponent<ComportementInterface>().OnMenuChangement += ChangerPossibilit�Tir;

        CameraJoueur = GetComponentInParent<Camera>().gameObject;
        Embout = transform.Find("Embout").gameObject;

        NombreBallesPr�sentementDansChargeur = NombreBallesTotalesChargeur;
        RechargeEventData = new(D�laisRecharge);

        TextnombreBallesChargeurTotal.text = NombreBallesTotalesChargeur.ToString();
        
    }


    void Update()
    {
        
        Temps�coul�DepuisTir += Time.deltaTime;

        if (PeutRecharger())
        {
            Recharger();
        }
        else if (PeutTirer())
        {
            Tirer();
            Temps�coul�DepuisTir = 0;
        }



    }

    bool PeutRecharger()
    {
        return Input.GetKeyDown(KeyCode.R) && !EstEnRecharge 
                                           && NombreBallesPr�sentementDansChargeur != NombreBallesTotalesChargeur;
    }

    bool PeutTirer()
    {
        return !EstEnRecharge && !EstEntreDeuxTir 
                              && !UnDesMenusEstActif 
                              && ImputTirer() 
                              && Temps�coul�DepuisTir > D�laisEntreTir;
    }

    protected virtual bool ADesBallesDansLeChargeur()
    {
        return NombreBallesPr�sentementDansChargeur > 0;
    }

    
    void Tirer()
    {
        if(ADesBallesDansLeChargeur())
        {
            NombreBallesPr�sentementDansChargeur-= NombreBalleParTir;
            if (TextnombreBallesChargeur != null)
            {
                TextnombreBallesChargeur.text = NombreBallesPr�sentementDansChargeur.ToString();

            }

            //Lancer de l'�vennement qui est surtout utilis� pour les animation
            OnTir?.Invoke(this, EventArgs.Empty);

            InstancierBalle();
            EffetsSonoresEtVisuelTirer();
        }

        //recharge automatique si le joueur vient tout juste de tirer la derni�re balle
        if(NombreBallesPr�sentementDansChargeur <= 0)
        {
            StartCoroutine(AttendreEtRecharger());
        }
        
    }

    private IEnumerator AttendreEtRecharger()
    {
        yield return new WaitForSeconds(D�laisPr�Rechargement);
        Recharger();
    }

    protected virtual void Recharger()
    {
        OnRechargement?.Invoke(this, RechargeEventData);
        SliderRechargement.instance.LancerAnimation(D�laisRecharge);
        StartCoroutine(ResetChargeurFusil());
    }

    IEnumerator ResetChargeurFusil()
    {
        EstEnRecharge = true;
        
        TextnombreBallesChargeur.text = "--";

        //Attend le bon d�lais avant de r�afficher un chargeur remplis
        yield return new WaitForSeconds(D�laisRecharge);

        EstEnRecharge = false;
       
        NombreBallesPr�sentementDansChargeur = NombreBallesTotalesChargeur;

        
    }

    protected virtual void InstancierBalle()
    {
        var balleTemp = ObjectPool.instance.GetPoolObject(Balle);
        balleTemp.transform.position = Embout.transform.position;
        balleTemp.GetComponent<TrailRenderer>().enabled = true;
        InitiationRotationBalle(balleTemp);
        
        balleTemp.SetActive(true);
    }

    //si un des menus est activ� ou d
    void ChangerPossibilit�Tir(bool JeuEstEnCour)
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
            TextnombreBallesChargeur.text = NombreBallesPr�sentementDansChargeur.ToString();
        }
        
        EstEnRecharge = false;
        StopAllCoroutines();
    }
}
