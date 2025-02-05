using System;
using System.Collections;
using UnityEngine;

public class AnimationGun : MonoBehaviour
{
    //Il faut rentrer manuellement les positions 

    protected Camera CameraPersonnage;
    bool EstCurseurFig� = true; //Si le curseur est fig�, on ne voit pas le menu, vice-versa

    //Aiming

    public Vector3 PositionRepos { get; protected set; }
    public Vector3 PositionAim { get; protected set; }
    public float VitesseMouvementVis� { get; protected set; }
    public float Dur�eRecul { get; protected set; }
    public float AngleRecul { get; protected set; } = 3;
    public float VariationRecul { get; protected set; }
    public float ChampsDeVisionVis�e { get; protected set; } 
    

    //pour le zoom si le joueur vise
    protected float ChampsDeVisionParD�fault = 60;
   
    protected float VitesseVariationChampsDeVision = 50;

    //Balancement Fusil

    float VitesseBalancement = 8;
    float Intensit�Balancement = 4;

    //oscillement si le joueur marche
    const float Intensit�OscillationMax = 0.002f;
    const float Intensit�OscillationMin = 0.0005f;
    float Intensit�Oscillation;
    const float VitesseOscillationMax = 9;
    const float VitesseOscillationMin = 4;
    float VitesseOscillation;

    //pour remettre le fusil � sa positionInitiale
    bool HauteurDoitEtreAjuster = false;
    protected bool EstEnRecharge = false;

    
    protected void Initialiser()
    {
        CameraPersonnage = transform.parent.GetComponentInParent<Camera>();

        transform.localPosition = PositionRepos;
        Intensit�Oscillation = Intensit�OscillationMax;
        VitesseOscillation = VitesseOscillationMax;
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        ComportementInterface visibilit�Menu = gameManager.GetComponent<ComportementInterface>();
    
        visibilit�Menu.OnMenuChangement += �tatMenu;
        
        ComportementFusil compFusil = GetComponent<ComportementFusil>();
        compFusil.OnTir += Tirer;
        compFusil.OnRechargement += Recharger;
    }

    private void Update()
    {

        if (EstCurseurFig�)
        {
            Viser();
            
            BalancerFusilRotation();
            BalancerFusilTranslation();
        }

    }

    void �tatMenu(bool �tat)
    {
        EstCurseurFig� = �tat;
    }

    

    
    protected virtual void Viser()
    {
        if (Input.GetMouseButton(1) && !EstEnRecharge)//vise si le joueur clic droit
        {
            MouvementViser(PositionAim);
            Zoomer(-1);

            //ralentit vitesse joueur et vitesse oscillation fusil
            Intensit�Oscillation = Intensit�OscillationMin;
            VitesseOscillation = VitesseOscillationMin;
        }
        else//sinon reviens � sa position initiale
        {
            MouvementViser(PositionRepos);
            Zoomer(1);

            //acc�lere vitesse joueur et vitesse oscillation fusil
            Intensit�Oscillation = Intensit�OscillationMax;
            VitesseOscillation = VitesseOscillationMax;
        }

    }

    //Translate le fusil � sa position de vis� ou ;a sa position originale
    void MouvementViser(Vector3 positionFinale)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionFinale, VitesseMouvementVis� * Time.deltaTime);

    }

    //zoom/d�zoom la cam�ra
    protected void Zoomer(int directionZoom)
    {
        if (CameraPersonnage != null)
        {
            CameraPersonnage.fieldOfView += directionZoom * VitesseVariationChampsDeVision * Time.deltaTime;
            CameraPersonnage.fieldOfView = Mathf.Clamp(CameraPersonnage.fieldOfView, ChampsDeVisionVis�e, ChampsDeVisionParD�fault);
        }
        
    }

    void Recharger(object sender, RechargementEventArgs e)
    {
        EstEnRecharge = true;
        StartCoroutine(AnimerRecharge(e.TempsRechargement));
    }
    
    IEnumerator AnimerRecharge(float dur�eRecharge)
    {
        float Timer = 0;


        float Demivariation = dur�eRecharge / 3f;
        Quaternion RotationInitiale = transform.localRotation;
        Quaternion RotationCibleX = Quaternion.AngleAxis(40, Vector3.down);
        Quaternion RotationCibleY = Quaternion.AngleAxis(30, Vector3.right);

        Quaternion RotationCible = RotationCibleX * RotationCibleY;

        while (Timer < Demivariation)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, RotationCible, Timer / Demivariation);
            Timer += Time.deltaTime;
            yield return null;
        }

        while(Timer < 2.7* Demivariation && Timer > Demivariation)
        {
            transform.localRotation = RotationCible;
            Timer += Time.deltaTime;
            yield return null;
        }

        while (Timer > 2.7f * Demivariation && Timer < dur�eRecharge)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, RotationInitiale, (Timer - (2.7f * Demivariation)) / Demivariation);
            Timer += Time.deltaTime;
            yield return null;
        }
        EstEnRecharge = false;
    }

    void Tirer(object sender, EventArgs e)
    {
        StartCoroutine(AppliquerRecul());
    }

    //Recul
    IEnumerator AppliquerRecul()
    {
        float TimerRecul = 0;

        
        float DemivariationRecul = Dur�eRecul / 2f;
        Vector3 positionReculFinal = transform.localPosition + Vector3.back * VariationRecul;
        Quaternion RotationCible = Quaternion.AngleAxis(AngleRecul, Vector3.left);

        while (TimerRecul < DemivariationRecul)//applicationRecul
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, positionReculFinal, TimerRecul / DemivariationRecul);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, RotationCible, TimerRecul / DemivariationRecul);
            TimerRecul += Time.deltaTime;
            yield return null;
        }
        

    }


    //Balancement si le joueur tourne
    void BalancerFusilRotation()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");

        Quaternion RotationX = Quaternion.AngleAxis(mouseY * Intensit�Balancement, Vector3.right);
        Quaternion RotationY = Quaternion.AngleAxis(-mouseX * Intensit�Balancement, Vector3.up);

        Quaternion RoatationCible = RotationX * RotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, RoatationCible, VitesseBalancement * Time.deltaTime);

    }

    //Oscillation si le joueur marche
    void BalancerFusilTranslation()
    {
        float VitesseMvt = 6;
        Vector3 positionInitiale = transform.localPosition;
        if (Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
        {
            transform.localPosition += Vector3.up * Mathf.Sin(Time.time * VitesseOscillation) * Intensit�Oscillation;
            HauteurDoitEtreAjuster = true;
        }
        else if (HauteurDoitEtreAjuster) //pour remettre le fusil � sa positionInitiale
        {
            Vector3 positionNeutre = new Vector3(transform.localPosition.x, positionInitiale.y, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, positionNeutre, VitesseMvt * Time.deltaTime);
            HauteurDoitEtreAjuster = false;
        }

    }


}
