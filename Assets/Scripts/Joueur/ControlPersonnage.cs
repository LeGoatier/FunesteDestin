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

    //Valeurs modifiables pour la fluidité
    #region
    [Header("Charcacter")]
    [SerializeField] float Vitesse = 10;
    [Range(0.1f, 1)]
    [SerializeField] float multiVitesseMarche;
    [SerializeField] float vitesseCameraMax = 150;
    [Range(0.1f, 1)]
    [SerializeField] float multiVitesseVisée;

    [Header("Saut")]
    [SerializeField] float forceSaut = 5;
    [SerializeField] float gravité = 10;

    [Header("Crouch")]
    [Range(0.4f, 1)]
    [SerializeField] float hauteurCrouch;
    [Range(0.01f, 1)]
    [SerializeField] float vitesseCrouch;

    const float duréeAnimationCrouch = 0.25f;

    //Valeurs dash
    const float DÉLAI_MAX_TOUCHES_DASH = 0.27f;

    const float VITESSE_DASH = 42;
    const float DURÉE_DASH = 0.2f;
    #endregion

    //Variables nécessaire au fonctionnement mais dont la valeur n'est pas assignée ou ne doit pas être modifiée
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

    float VitesseVisée;
    float vitesseMarche;

    float vitesseCamera;
    float vitesseCameraMin = 80;

    //bruits
    bool enSaut = false;
    bool sonMarcher = true;
    
    //Collider
    CapsuleCollider colliderPersonnage;

    //Curseur
    public bool EstCurseurFigé = true;
    private float multiplicateurSensibilité = 1;
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

        ComportementInterface visibilitéMenu = GameObject.FindWithTag("GameManager").GetComponent<ComportementInterface>();
        visibilitéMenu.OnMenuChangement += ÉtatMenu;

        VitesseVisée = Vitesse * multiVitesseVisée;
        vitesseMarche = Vitesse * multiVitesseMarche;
        vitesseCamera = vitesseCameraMax;
        vitesseActuelle = Vitesse;

        
        cam = GetComponentInChildren<Camera>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        posCamBase = cam.transform.localPosition;
    }

    private void Update()
    {
        if (EstCurseurFigé)
        {
            RalentirVitesseSiVisé();
            
            RotationCamera();
            RalentirSiVisé();
            VérifierDash();
        }
        Déplacement();
    }

    void RalentirSiVisé()
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
    
    void Déplacement()
    {
        Vector3 direction = cam.transform.forward * Input.GetAxis("Vertical") + cam.transform.right * Input.GetAxis("Horizontal");
        direction = new Vector3(direction.x, 0, direction.z);
        direction = direction.normalized;
        
        // Vérifie si le joueur essaie de sauter et valide qu'il est présentement sur le sol 
        if (!characterController.isGrounded)
        {
            saut += Vector3.down * Time.deltaTime * gravité;
        }
        else
        {
            if (EstCurseurFigé)
            {
                GérerSaut();
                GérerCrouch();
            }
            
        }

        // Déplace le joueur selon tout les forces et Inputs
        if (!EstEau(transform.position + direction * vitesseActuelle))
        {
            if (!EstCurseurFigé)
            {
                characterController.Move((Vector3.zero * vitesseActuelle + saut) * Time.deltaTime);
            }
            else
            {
                characterController.Move((direction * vitesseActuelle + saut) * Time.deltaTime);
            }
        }
            



        //Sons
        if (direction.magnitude > 0 && sonMarcher && EstCurseurFigé)       
            GestionBruit.instance.JouerSon("Marcher");
        else
            GestionBruit.instance.ArreterSon("Marcher");

        if (characterController.isGrounded && enSaut && EstCurseurFigé)
        {
            GestionBruit.instance.JouerSon("Sauter");
            enSaut = false;
            sonMarcher = true;
        }
    }

    private void GérerSaut()
    {
        // Si touche au sol, est-ce que le joueur essaie de sauter?
        if (Input.GetButtonDown("Jump"))
        {
            saut = transform.up * forceSaut;

            sonMarcher = false;
            enSaut = true;
        }

        // Si touche au sol, la valeur devrait être la plus haute entre -1 et Jump, pour que
        // lorsque l'on saute pas, une force nous pousse vers le sol, mais lorsque l'on saute,
        // la valeur soit gardée
        saut.y = Mathf.Max(-1, saut.y);
    }

    float dernierDéclenchementAnimation;
    private void GérerCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            vitesseActuelle *= vitesseCrouch;
            //Je voulais faire que le collider aussi bouge progressivement, mais vu que les valeurs de hauteurs
            //sont différentes de celles de la caméra c'est compliqué pour rien alors c'est instantané
            colliderPersonnage.height *= hauteurCrouch;
            StartCoroutine(AnimerCrouch(posCamBase, posCamBase * hauteurCrouch, duréeAnimationCrouch));
            dernierDéclenchementAnimation = Time.time;
        }
        if (Input.GetButtonUp("Crouch"))
        {
            vitesseActuelle = Vitesse;
            colliderPersonnage.height = hauteurBase;
            StartCoroutine(AnimerCrouch(posCamBase * hauteurCrouch, posCamBase, duréeAnimationCrouch));
            dernierDéclenchementAnimation = Time.time;
        }
    }
    IEnumerator AnimerCrouch(Vector3 positionDépart, Vector3 positionFin, float durée)
    {
        if (Time.time - dernierDéclenchementAnimation < durée)
            yield return new WaitForSeconds(dernierDéclenchementAnimation + durée - Time.time);
        float timer = 0;
        while (timer < durée)
        {
            cam.transform.localPosition = Vector3.Lerp(positionDépart, positionFin, timer / durée);
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

        rotationCamera += new Vector3(-sourisY, sourisX, 0) * vitesseCamera * multiplicateurSensibilité * Time.deltaTime;
        rotationCamera.x = Mathf.Clamp(rotationCamera.x, -70f, 70f);

        cam.transform.rotation = Quaternion.Euler(rotationCamera.x, rotationCamera.y, 0);
    }

    void ÉtatMenu(bool JeuActif)
    {
        EstCurseurFigé = JeuActif;
    }

    void RalentirVitesseSiVisé()
    {
        if (Input.GetMouseButtonDown(1))
        {
            vitesseActuelle = VitesseVisée;
        }
        if (Input.GetMouseButtonUp(1))
        {
            vitesseActuelle = Vitesse;
        }
    }

    //Méthodes pour la gestion du dash
    #region
    //C'est pas idéal mais on peut pas passer une valeur en ref dans une coroutine, faque c'est ma solution de rechange
    private enum TouchesDash { W, A, S, D, nbTouches };
    //Ces bool retiennent si la touche a été pesée dans les dernières .2 secondes
    //Ce système ne fonctionnerait pas présentement avec une manette, il faudrait adapter
    private bool[] touchesActives = new bool[(int)TouchesDash.nbTouches];
    private float tempsDernierDash = float.MinValue;


    private void VérifierDash()
    {
        if (EstDashDisponible())
        {
            VérifierDashDirection(KeyCode.W, TouchesDash.W, cam.transform.forward);
            VérifierDashDirection(KeyCode.A, TouchesDash.A, -cam.transform.right);
            VérifierDashDirection(KeyCode.S, TouchesDash.S, -cam.transform.forward);
            VérifierDashDirection(KeyCode.D, TouchesDash.D, cam.transform.right);
        }
    }
    private bool EstDashDisponible()
    {
        return GestionDash.instance.EstDashPossible();
    }

    private void VérifierDashDirection(KeyCode touchePhysique, TouchesDash toucheType, Vector3 direction)
    {
        if (Input.GetKeyDown(touchePhysique))
        {
            if (touchesActives[(int)toucheType])
            {
                tempsDernierDash = Time.time;
                StartCoroutine(Dash(direction));
                GestionDash.instance.RetirerÉnergie();
            }
            else
            {
                touchesActives[(int)toucheType] = true;
                StartCoroutine(AttendreEtDésactiver(toucheType));
            }
        }
    }
    private IEnumerator AttendreEtDésactiver(TouchesDash touche)
    {
        yield return new WaitForSeconds(DÉLAI_MAX_TOUCHES_DASH);
        touchesActives[(int)touche] = false;
    }
    private IEnumerator Dash(Vector3 direction)
    {
        float timer = 0;
        while (timer < DURÉE_DASH)
        {
            timer += Time.deltaTime;
            characterController.Move(direction.normalized * VITESSE_DASH * Time.deltaTime);
            yield return null;
        }
    }
    #endregion


    public void ChangerSensibilité(float valeurMultiplicateur)
    {
        multiplicateurSensibilité = valeurMultiplicateur;
    }
}
