using System;
using System.Collections;
using UnityEngine;

public class AnimationGun : MonoBehaviour
{
    //Il faut rentrer manuellement les positions 

    protected Camera CameraPersonnage;
    bool EstCurseurFigé = true; //Si le curseur est figé, on ne voit pas le menu, vice-versa

    //Aiming

    public Vector3 PositionRepos { get; protected set; }
    public Vector3 PositionAim { get; protected set; }
    public float VitesseMouvementVisé { get; protected set; }
    public float DuréeRecul { get; protected set; }
    public float AngleRecul { get; protected set; } = 3;
    public float VariationRecul { get; protected set; }
    public float ChampsDeVisionVisée { get; protected set; } 
    

    //pour le zoom si le joueur vise
    protected float ChampsDeVisionParDéfault = 60;
   
    protected float VitesseVariationChampsDeVision = 50;

    //Balancement Fusil

    float VitesseBalancement = 8;
    float IntensitéBalancement = 4;

    //oscillement si le joueur marche
    const float IntensitéOscillationMax = 0.002f;
    const float IntensitéOscillationMin = 0.0005f;
    float IntensitéOscillation;
    const float VitesseOscillationMax = 9;
    const float VitesseOscillationMin = 4;
    float VitesseOscillation;

    //pour remettre le fusil à sa positionInitiale
    bool HauteurDoitEtreAjuster = false;
    protected bool EstEnRecharge = false;

    
    protected void Initialiser()
    {
        CameraPersonnage = transform.parent.GetComponentInParent<Camera>();

        transform.localPosition = PositionRepos;
        IntensitéOscillation = IntensitéOscillationMax;
        VitesseOscillation = VitesseOscillationMax;
        GameObject gameManager = GameObject.FindWithTag("GameManager");
        ComportementInterface visibilitéMenu = gameManager.GetComponent<ComportementInterface>();
    
        visibilitéMenu.OnMenuChangement += ÉtatMenu;
        
        ComportementFusil compFusil = GetComponent<ComportementFusil>();
        compFusil.OnTir += Tirer;
        compFusil.OnRechargement += Recharger;
    }

    private void Update()
    {

        if (EstCurseurFigé)
        {
            Viser();
            
            BalancerFusilRotation();
            BalancerFusilTranslation();
        }

    }

    void ÉtatMenu(bool État)
    {
        EstCurseurFigé = État;
    }

    

    
    protected virtual void Viser()
    {
        if (Input.GetMouseButton(1) && !EstEnRecharge)//vise si le joueur clic droit
        {
            MouvementViser(PositionAim);
            Zoomer(-1);

            //ralentit vitesse joueur et vitesse oscillation fusil
            IntensitéOscillation = IntensitéOscillationMin;
            VitesseOscillation = VitesseOscillationMin;
        }
        else//sinon reviens à sa position initiale
        {
            MouvementViser(PositionRepos);
            Zoomer(1);

            //accélere vitesse joueur et vitesse oscillation fusil
            IntensitéOscillation = IntensitéOscillationMax;
            VitesseOscillation = VitesseOscillationMax;
        }

    }

    //Translate le fusil à sa position de visé ou ;a sa position originale
    void MouvementViser(Vector3 positionFinale)
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, positionFinale, VitesseMouvementVisé * Time.deltaTime);

    }

    //zoom/dézoom la caméra
    protected void Zoomer(int directionZoom)
    {
        if (CameraPersonnage != null)
        {
            CameraPersonnage.fieldOfView += directionZoom * VitesseVariationChampsDeVision * Time.deltaTime;
            CameraPersonnage.fieldOfView = Mathf.Clamp(CameraPersonnage.fieldOfView, ChampsDeVisionVisée, ChampsDeVisionParDéfault);
        }
        
    }

    void Recharger(object sender, RechargementEventArgs e)
    {
        EstEnRecharge = true;
        StartCoroutine(AnimerRecharge(e.TempsRechargement));
    }
    
    IEnumerator AnimerRecharge(float duréeRecharge)
    {
        float Timer = 0;


        float Demivariation = duréeRecharge / 3f;
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

        while (Timer > 2.7f * Demivariation && Timer < duréeRecharge)
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

        
        float DemivariationRecul = DuréeRecul / 2f;
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

        Quaternion RotationX = Quaternion.AngleAxis(mouseY * IntensitéBalancement, Vector3.right);
        Quaternion RotationY = Quaternion.AngleAxis(-mouseX * IntensitéBalancement, Vector3.up);

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
            transform.localPosition += Vector3.up * Mathf.Sin(Time.time * VitesseOscillation) * IntensitéOscillation;
            HauteurDoitEtreAjuster = true;
        }
        else if (HauteurDoitEtreAjuster) //pour remettre le fusil à sa positionInitiale
        {
            Vector3 positionNeutre = new Vector3(transform.localPosition.x, positionInitiale.y, transform.localPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, positionNeutre, VitesseMvt * Time.deltaTime);
            HauteurDoitEtreAjuster = false;
        }

    }


}
