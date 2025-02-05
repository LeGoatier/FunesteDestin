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
    [SerializeField] Canvas CanvasSucc�s;
    [SerializeField] Canvas CanevasParam�tres;

    private ArbreAchievement arbre;

    //passez en param�tre vrai si vous voulez que le joueur puisse bouger tirer etc (aucun menu d'ouvert), ou faux sinon, par exemple
    //passez faux si un des menus est activ� pour empecher le joueur de tirer par exemple
    public delegate void OnMenuChangementEventHandler(bool InterfaceJeuActiv�);
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
        arbre = GameObject.FindGameObjectWithTag("ArbreSucc�s").GetComponent<ArbreAchievement>();
        Activ��tatJeu();
        CanvasSucc�s.gameObject.SetActive(false);
        CanevasParam�tres.gameObject.SetActive(false);
    }

    bool premi�reIt�ration = true;
    void Update()
    {
        //Je (Justin) vais ajouter un commentaire parce que Rosie ne l'a pas fait. Donc, nous avions
        //un probl�me seulement dans le build du jeu qui faisait que le joueur ne pouvait pas interagir
        //avec certains �l�ments de l'environnement, mais nous avions remarqu� que si nous ouvrions l'inventaire
        //une seule fois tout �tait r�gl�. Apr�s avoir cherch� le probl�me pendant plusieurs heures, nous n'avons pas
        //trouv�, alors nous nous sommes r�solus � ouvrir et fermer l'inventaire au lancement du programme pour
        //fixer le probl�me.
        if (premi�reIt�ration)
        {
            StartCoroutine(OuvrirInventaireUneFractionDeSecondeAuD�butPourPourInteragir());
            premi�reIt�ration = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UnDesMenuEstOuvert())
            {
                Activ��tatJeu();
            }
            else
            {
                Changer�tatMenuPause();
            }
            

        }
        if (Input.GetButtonDown("Ouvrir Inventaire"))
        {
            Changer�tatInventaire();
        }


    }

    IEnumerator OuvrirInventaireUneFractionDeSecondeAuD�butPourPourInteragir()
    {
        yield return new WaitForSeconds(0.01f);

        InterfaceInventaire.gameObject.SetActive(true);
        yield return new WaitForSeconds(0.01f);
        InterfaceInventaire.gameObject.SetActive(false);
    }

    //Fonction associ� au bouton reprendre
    public void Changer�tatMenuPause()
    {
        InterfaceMenu.enabled = !InterfaceMenu.enabled;
        Changer�tatJeu();

        Arr�terTemps(InterfaceMenu.enabled);
    }

    public void Changer�tatCraft()
    {
        if (!InterfaceMenu.enabled && !InterfaceInventaire.gameObject.activeInHierarchy)
        {
            InterfaceCraft.gameObject.SetActive(!InterfaceCraft.gameObject.activeInHierarchy);
            Changer�tatJeu();
            Arr�terTemps(InterfaceCraft.gameObject.activeInHierarchy);
        }
    }

    public void Changer�tatInventaire()
    {
        if (!InterfaceMenu.enabled && !InterfaceCraft.gameObject.activeInHierarchy)
        {
            if (InterfaceInventaire.gameObject.activeInHierarchy) Menu_Inventaire.instance.V�rifierQueDraggableEstAssoci�();
            InterfaceInventaire.gameObject.SetActive(!InterfaceInventaire.gameObject.activeInHierarchy);
            Changer�tatJeu();
        }

    }

    //toggle entre l'�tat "pause", ou le jeu est fig� et un des canvas (menu, craft, inventaire) est activ� et l'�tat "jeu" ou le canvas g�n�rique est 
    //activ� et le temps s'�coule normalement
    void Changer�tatJeu()
    {
        bool UnDesMenuEstActif = UnDesMenuEstOuvert();
        OnMenuChangement?.Invoke(!UnDesMenuEstActif);
        InterfaceJeu.enabled = !UnDesMenuEstActif;
    }

    public void Activ��tatJeu()
    {
        Arr�terTemps(false);
        D�sactiverMenu();
        OnMenuChangement?.Invoke(true);
    }

    public void Activ��tatMenu()
    {
        D�sactiverMenu();
        OnMenuChangement?.Invoke(false);
    }

    public void Set�tat(bool EstJeuActif)
    {
        D�sactiverMenu();
        OnMenuChangement?.Invoke(EstJeuActif);
    }

    void D�sactiverMenu()
    {
        InterfaceInventaire.gameObject.SetActive(false);
        InterfaceCraft.gameObject.SetActive(false);
        InterfaceMenu.enabled = false;
        CanevasParam�tres.gameObject.SetActive(false);
        CanvasSucc�s.gameObject.SetActive(false);

        InterfaceJeu.enabled = true;
    }

    bool UnDesMenuEstOuvert()
    {
        return InterfaceInventaire.gameObject.activeInHierarchy || InterfaceCraft.gameObject.activeInHierarchy || InterfaceMenu.enabled || CanevasParam�tres.gameObject.activeInHierarchy || CanvasSucc�s.gameObject.activeInHierarchy;
    }

    public void Arr�terTemps(bool arr�ter)
    {
        if (arr�ter)
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

    //Fonction associ� au boutton quitter
    public void Quitter()
    {
        GestionScenes.ChargerMenuPrincipal(false);
        GestionBruit.instance.JouerChansonMenu();
    }


    public void AllerSucc�s()
    {
        arbre.ActiverPremierNoeud();
        InterfaceMenu.enabled = false;
        CanvasSucc�s.gameObject.SetActive(true);
    }

    public void RetourSucc�s()
    {
        InterfaceMenu.enabled = true;
        CanvasSucc�s.gameObject.SetActive(false);
    }

    public void AllerParam�tres()
    {
        InterfaceMenu.enabled = false;
        CanevasParam�tres.gameObject.SetActive(true);
    }
    public void RetourPause()
    {
        InterfaceMenu.enabled = true;
        CanevasParam�tres.gameObject.SetActive(false);
    }

}

