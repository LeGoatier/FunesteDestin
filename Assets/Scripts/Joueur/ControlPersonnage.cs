using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UIElements;

public class ControlPersonnage : MonoBehaviour
{
    public static ControlPersonnage instance;

    //Valeurs modifiables pour la fluidit�
    #region
    [Header("Charcacter")]
    [SerializeField] float Vitesse = 10;
    [Range(0.1f, 1)]
    [SerializeField] float multiVitesseMarche;
    [SerializeField] float vitesseCameraMax = 150;
    [Range(0.1f, 1)]
    [SerializeField] float multiVitesseVis�e;

    [Header("Saut")]
    [SerializeField] float forceSaut = 5;
    [SerializeField] float gravit� = 10;

    [Header("Crouch")]
    [Range(0.4f, 1)]
    [SerializeField] float hauteurCrouch;
    [Range(0.01f, 1)]
    [SerializeField] float vitesseCrouch;

    const float dur�eAnimationCrouch = 0.25f;

    //Valeurs dash
    const float D�LAI_MAX_TOUCHES_DASH = 0.27f;

    const float VITESSE_DASH = 42;
    const float DUR�E_DASH = 0.2f;
    #endregion

    //Variables n�cessaire au fonctionnement mais dont la valeur n'est pas assign�e ou ne doit pas �tre modifi�e
    #region
    float hauteurBase;
    float vitesseActuelle;

    //Cam
    Camera cam;
    Vector3 rotationCamera = Vector3.zero;
    Vector3 posCamBase;

    // Character
    public CharacterController characterController;

    Vector3 saut = new Vector3(0, 0, 0);

    float VitesseVis�e;
    float vitesseMarche;

    float vitesseCamera;
    float vitesseCameraMin = 80;

    //bruits
    bool enSaut = false;
    bool sonMarcher = true;
    
    //Collider
    CapsuleCollider colliderPersonnage;

    //Curseur
    public bool EstCurseurFig� = true;
    private float multiplicateurSensibilit� = 1;
    #endregion

    private void Awake()
    {
        instance = this;
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        
        colliderPersonnage = GetComponent<CapsuleCollider>();
        hauteurBase = colliderPersonnage.height;

        ComportementInterface visibilit�Menu = GameObject.FindWithTag("GameManager").GetComponent<ComportementInterface>();
        visibilit�Menu.OnMenuChangement += �tatMenu;

        VitesseVis�e = Vitesse * multiVitesseVis�e;
        vitesseMarche = Vitesse * multiVitesseMarche;
        vitesseCamera = vitesseCameraMax;
        vitesseActuelle = Vitesse;

        
        cam = GetComponentInChildren<Camera>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        posCamBase = cam.transform.localPosition;
    }

    private void Update()
    {
        if (EstCurseurFig�)
        {
            RalentirVitesseSiVis�();
            
            RotationCamera();
            RalentirSiVis�();
            V�rifierDash();
        }
        D�placement();
    }

    void RalentirSiVis�()
    {
        if(Input.GetMouseButtonDown(1))
        {
            vitesseCamera = vitesseCameraMin;
        }
        if(Input.GetMouseButtonUp(1))
        {
            vitesseCamera = vitesseCameraMax;
        }
    }
    
    void D�placement()
    {
        Vector3 direction = cam.transform.forward * Input.GetAxis("Vertical") + cam.transform.right * Input.GetAxis("Horizontal");
        direction = new Vector3(direction.x, 0, direction.z);
        direction = direction.normalized;
        
        // V�rifie si le joueur essaie de sauter et valide qu'il est pr�sentement sur le sol 
        if (!characterController.isGrounded)
        {
            saut += Vector3.down * Time.deltaTime * gravit�;
        }
        else
        {
            if (EstCurseurFig�)
            {
                G�rerSaut();
                G�rerCrouch();
            }
            
        }

        // D�place le joueur selon tout les forces et Inputs
        if (!EstEau(transform.position + direction * vitesseActuelle))
        {
            if (!EstCurseurFig�)
            {
                characterController.Move((Vector3.zero * vitesseActuelle + saut) * Time.deltaTime);
            }
            else
            {
                characterController.Move((direction * vitesseActuelle + saut) * Time.deltaTime);
            }
        }
            



        //Sons
        if (direction.magnitude > 0 && sonMarcher && EstCurseurFig�)       
            GestionBruit.instance.JouerSon("Marcher");
        else
            GestionBruit.instance.ArreterSon("Marcher");

        if (characterController.isGrounded && enSaut && EstCurseurFig�)
        {
            GestionBruit.instance.JouerSon("Sauter");
            enSaut = false;
            sonMarcher = true;
        }
    }

    private void G�rerSaut()
    {
        // Si touche au sol, est-ce que le joueur essaie de sauter?
        if (Input.GetButtonDown("Jump"))
        {
            saut = transform.up * forceSaut;

            sonMarcher = false;
            enSaut = true;
        }

        // Si touche au sol, la valeur devrait �tre la plus haute entre -1 et Jump, pour que
        // lorsque l'on saute pas, une force nous pousse vers le sol, mais lorsque l'on saute,
        // la valeur soit gard�e
        saut.y = Mathf.Max(-1, saut.y);
    }

    float dernierD�clenchementAnimation;
    private void G�rerCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            vitesseActuelle *= vitesseCrouch;
            //Je voulais faire que le collider aussi bouge progressivement, mais vu que les valeurs de hauteurs
            //sont diff�rentes de celles de la cam�ra c'est compliqu� pour rien alors c'est instantan�
            colliderPersonnage.height *= hauteurCrouch;
            StartCoroutine(AnimerCrouch(posCamBase, posCamBase * hauteurCrouch, dur�eAnimationCrouch));
            dernierD�clenchementAnimation = Time.time;
        }
        if (Input.GetButtonUp("Crouch"))
        {
            vitesseActuelle = Vitesse;
            colliderPersonnage.height = hauteurBase;
            StartCoroutine(AnimerCrouch(posCamBase * hauteurCrouch, posCamBase, dur�eAnimationCrouch));
            dernierD�clenchementAnimation = Time.time;
        }
    }
    IEnumerator AnimerCrouch(Vector3 positionD�part, Vector3 positionFin, float dur�e)
    {
        if (Time.time - dernierD�clenchementAnimation < dur�e)
            yield return new WaitForSeconds(dernierD�clenchementAnimation + dur�e - Time.time);
        float timer = 0;
        while (timer < dur�e)
        {
            cam.transform.localPosition = Vector3.Lerp(positionD�part, positionFin, timer / dur�e);
            timer += Time.deltaTime;
            yield return null;
        }
        cam.transform.localPosition = positionFin;
    }

    private bool EstEau(Vector3 position)
    {
        const float HAUTEUR_ACCEPTABLE_MAX = 100;
        Ray ray = new Ray(new Vector3(position.x, 50f, position.z), Vector3.down);
        Physics.Raycast(ray, out RaycastHit hit, HAUTEUR_ACCEPTABLE_MAX);
        return hit.collider.gameObject.CompareTag("Eau");
    }

    void RotationCamera()
    {
        float sourisX = Input.GetAxisRaw("Mouse X");
        float sourisY = Input.GetAxisRaw("Mouse Y");

        rotationCamera += new Vector3(-sourisY, sourisX, 0) * vitesseCamera * multiplicateurSensibilit� * Time.deltaTime;
        rotationCamera.x = Mathf.Clamp(rotationCamera.x, -70f, 70f);

        cam.transform.rotation = Quaternion.Euler(rotationCamera.x, rotationCamera.y, 0);
    }

    void �tatMenu(bool JeuActif)
    {
        EstCurseurFig� = JeuActif;
    }

    void RalentirVitesseSiVis�()
    {
        if (Input.GetMouseButtonDown(1))
        {
            vitesseActuelle = VitesseVis�e;
        }
        if (Input.GetMouseButtonUp(1))
        {
            vitesseActuelle = Vitesse;
        }
    }

    //M�thodes pour la gestion du dash
    #region
    //C'est pas id�al mais on peut pas passer une valeur en ref dans une coroutine, faque c'est ma solution de rechange
    private enum TouchesDash { W, A, S, D, nbTouches };
    //Ces bool retiennent si la touche a �t� pes�e dans les derni�res .2 secondes
    //Ce syst�me ne fonctionnerait pas pr�sentement avec une manette, il faudrait adapter
    private bool[] touchesActives = new bool[(int)TouchesDash.nbTouches];
    private float tempsDernierDash = float.MinValue;


    private void V�rifierDash()
    {
        if (EstDashDisponible())
        {
            V�rifierDashDirection(KeyCode.W, TouchesDash.W, cam.transform.forward);
            V�rifierDashDirection(KeyCode.A, TouchesDash.A, -cam.transform.right);
            V�rifierDashDirection(KeyCode.S, TouchesDash.S, -cam.transform.forward);
            V�rifierDashDirection(KeyCode.D, TouchesDash.D, cam.transform.right);
        }
    }
    private bool EstDashDisponible()
    {
        return GestionDash.instance.EstDashPossible();
    }

    private void V�rifierDashDirection(KeyCode touchePhysique, TouchesDash toucheType, Vector3 direction)
    {
        if (Input.GetKeyDown(touchePhysique))
        {
            if (touchesActives[(int)toucheType])
            {
                tempsDernierDash = Time.time;
                StartCoroutine(Dash(direction));
                GestionDash.instance.Retirer�nergie();
            }
            else
            {
                touchesActives[(int)toucheType] = true;
                StartCoroutine(AttendreEtD�sactiver(toucheType));
            }
        }
    }
    private IEnumerator AttendreEtD�sactiver(TouchesDash touche)
    {
        yield return new WaitForSeconds(D�LAI_MAX_TOUCHES_DASH);
        touchesActives[(int)touche] = false;
    }
    private IEnumerator Dash(Vector3 direction)
    {
        float timer = 0;
        while (timer < DUR�E_DASH)
        {
            timer += Time.deltaTime;
            characterController.Move(direction.normalized * VITESSE_DASH * Time.deltaTime);
            yield return null;
        }
    }
    #endregion


    public void ChangerSensibilit�(float valeurMultiplicateur)
    {
        multiplicateurSensibilit� = valeurMultiplicateur;
    }
}
