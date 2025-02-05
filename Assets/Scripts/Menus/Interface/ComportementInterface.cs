using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComportementInterface : MonoBehaviour
{
    public Sprite[] ressources = new Sprite[(int)Ressource.NbRessources];
    public Sprite[] outils = new Sprite[(int)Outil.NbOutils];
    public Sprite[] armes = new Sprite[(int)Arme.NbArmes];
    public Sprite[] soins = new Sprite[(int)Soin.NbSoins];

    public Draggable draggableActuel;

    public static ComportementInterface instance;

    [SerializeField] Canvas InterfaceMenu;
    [SerializeField] Canvas InterfaceJeu;
    [SerializeField] Canvas InterfaceCraft;
    [SerializeField] Canvas InterfaceInventaire;
    [SerializeField] Canvas CanvasSuccès;
    [SerializeField] Canvas CanevasParamètres;

    private ArbreAchievement arbre;

    //passez en paramètre vrai si vous voulez que le joueur puisse bouger tirer etc (aucun menu d'ouvert), ou faux sinon, par exemple
    //passez faux si un des menus est activé pour empecher le joueur de tirer par exemple
    public delegate void OnMenuChangementEventHandler(bool InterfaceJeuActivé);
    public event OnMenuChangementEventHandler OnMenuChangement;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InterfaceMenu.enabled = false;
        InterfaceJeu.enabled = true;
        
        InterfaceCraft.gameObject.SetActive(false);
        InterfaceInventaire.gameObject.SetActive(false);
        

        //Faut pas changer l'ordre en bas
        arbre = GameObject.FindGameObjectWithTag("ArbreSuccès").GetComponent<ArbreAchievement>();
        ActivéÉtatJeu();
        CanvasSuccès.gameObject.SetActive(false);
        CanevasParamètres.gameObject.SetActive(false);
    }

    bool premièreItération = true;
    void Update()
    {
        //Je (Justin) vais ajouter un commentaire parce que Rosie ne l'a pas fait. Donc, nous avions
        //un problème seulement dans le build du jeu qui faisait que le joueur ne pouvait pas interagir
        //avec certains éléments de l'environnement, mais nous avions remarqué que si nous ouvrions l'inventaire
        //une seule fois tout était réglé. Après avoir cherché le problème pendant plusieurs heures, nous n'avons pas
        //trouvé, alors nous nous sommes résolus à ouvrir et fermer l'inventaire au lancement du programme pour
        //fixer le problème.
        if (premièreItération)
        {
            StartCoroutine(OuvrirInventaireUneFractionDeSecondeAuDébutPourPourInteragir());
            premièreItération = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UnDesMenuEstOuvert())
            {
                ActivéÉtatJeu();
            }
            else
            {
                ChangerÉtatMenuPause();
            }
            

        }
        if (Input.GetButtonDown("Ouvrir Inventaire"))
        {
            ChangerÉtatInventaire();
        }


    }

    IEnumerator OuvrirInventaireUneFractionDeSecondeAuDébutPourPourInteragir()
    {
        yield return new WaitForSeconds(0.01f);

        InterfaceInventaire.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        InterfaceInventaire.gameObject.SetActive(false);
    }

    //Fonction associé au bouton reprendre
    public void ChangerÉtatMenuPause()
    {
        InterfaceMenu.enabled = !InterfaceMenu.enabled;
        ChangerÉtatJeu();

        ArrêterTemps(InterfaceMenu.enabled);
    }

    public void ChangerÉtatCraft()
    {
        if (!InterfaceMenu.enabled && !InterfaceInventaire.gameObject.activeInHierarchy)
        {
            InterfaceCraft.gameObject.SetActive(!InterfaceCraft.gameObject.activeInHierarchy);
            ChangerÉtatJeu();
            ArrêterTemps(InterfaceCraft.gameObject.activeInHierarchy);
        }
    }

    public void ChangerÉtatInventaire()
    {
        if (!InterfaceMenu.enabled && !InterfaceCraft.gameObject.activeInHierarchy)
        {
            if (InterfaceInventaire.gameObject.activeInHierarchy) Menu_Inventaire.instance.VérifierQueDraggableEstAssocié();
            InterfaceInventaire.gameObject.SetActive(!InterfaceInventaire.gameObject.activeInHierarchy);
            ChangerÉtatJeu();
        }

    }

    //toggle entre l'état "pause", ou le jeu est figé et un des canvas (menu, craft, inventaire) est activé et l'état "jeu" ou le canvas générique est 
    //activé et le temps s'écoule normalement
    void ChangerÉtatJeu()
    {
        bool UnDesMenuEstActif = UnDesMenuEstOuvert();
        OnMenuChangement?.Invoke(!UnDesMenuEstActif);
        InterfaceJeu.enabled = !UnDesMenuEstActif;
    }

    public void ActivéÉtatJeu()
    {
        ArrêterTemps(false);
        DésactiverMenu();
        OnMenuChangement?.Invoke(true);
    }

    public void ActivéÉtatMenu()
    {
        DésactiverMenu();
        OnMenuChangement?.Invoke(false);
    }

    public void SetÉtat(bool EstJeuActif)
    {
        DésactiverMenu();
        OnMenuChangement?.Invoke(EstJeuActif);
    }

    void DésactiverMenu()
    {
        InterfaceInventaire.gameObject.SetActive(false);
        InterfaceCraft.gameObject.SetActive(false);
        InterfaceMenu.enabled = false;
        CanevasParamètres.gameObject.SetActive(false);
        CanvasSuccès.gameObject.SetActive(false);

        InterfaceJeu.enabled = true;
    }

    bool UnDesMenuEstOuvert()
    {
        return InterfaceInventaire.gameObject.activeInHierarchy || InterfaceCraft.gameObject.activeInHierarchy || InterfaceMenu.enabled || CanevasParamètres.gameObject.activeInHierarchy || CanvasSuccès.gameObject.activeInHierarchy;
    }

    public void ArrêterTemps(bool arrêter)
    {
        if (arrêter)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }


    public void Rejouer()
    {
        GestionScenes.ChargerPartie();
    }

    //Fonction associé au boutton quitter
    public void Quitter()
    {
        GestionScenes.ChargerMenuPrincipal(false);
        GestionBruit.instance.JouerChansonMenu();
    }


    public void AllerSuccès()
    {
        arbre.ActiverPremierNoeud();
        InterfaceMenu.enabled = false;
        CanvasSuccès.gameObject.SetActive(true);
    }

    public void RetourSuccès()
    {
        InterfaceMenu.enabled = true;
        CanvasSuccès.gameObject.SetActive(false);
    }

    public void AllerParamètres()
    {
        InterfaceMenu.enabled = false;
        CanevasParamètres.gameObject.SetActive(true);
    }
    public void RetourPause()
    {
        InterfaceMenu.enabled = true;
        CanevasParamètres.gameObject.SetActive(false);
    }

}

