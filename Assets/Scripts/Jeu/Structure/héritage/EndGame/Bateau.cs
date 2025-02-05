using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using UnityEngine.UIElements;
using System;

public class Bateau : ComportementStructure
{
    public override �tapes �tapeLi�e => �tapes.Bateau;
    bool EstR�par� = false;

    [SerializeField] GameObject BateauBris�;
    [SerializeField] GameObject BateauR�par�Moteur;
    [SerializeField] GameObject BateauR�par�Voile;
    [SerializeField] GameObject Livre;
    DataObjectInteragissable CeBateau;

    Cout cout1;
    bool cout1Utilis�;
    Cout cout2;

    TextMeshProUGUI texte;
    string messageBateau;
    GameObject instanceBris�;
    

    // Start is called before the first frame update
    void Awake()
    {
        InitialiserComportementStructure();

        messageBateau = "R�parer le bateau";
        �tatInitialisation = �tatInitialisation.positionCondition;

        instanceBris� = Instantiate(BateauBris�, transform);
        InitialiserDataBateau(instanceBris�);
        texte = GetComponentInChildren<TextMeshProUGUI>();

        cout1 = new Cout();
        cout2 = new Cout();
        cout1.AjouterOutil(Outil.Moteur);
        cout1.AjouterOutil(Outil.Essence);
        cout1.AjouterOutil(Outil.Marteau);
        cout1.AjouterRessource(Ressource.Bois, 10);


        cout2.AjouterOutil(Outil.Voiles);
        cout2.AjouterOutil(Outil.Marteau);
        cout2.AjouterRessource(Ressource.Bois, 10);

        CoutStructure = cout1;

        
    }

    public void InstancierLivreBateau(Vector3 position, Vector3 normale)
    {
        var instance = Instantiate(Livre);
        instance.transform.position = position; 
        instance.transform.up = normale;
    }

    protected override bool SontPr�alablesRemplis()
    {
        if (GestionInventaire.EstCoutRempli(CoutStructure))
        {
            cout1Utilis� = true;
            return true;
        }
        else if (GestionInventaire.EstCoutRempli(cout2))
        {
            CoutStructure = cout2;
            cout1Utilis� = false;
            return true;
        }
        return false;
    }

    public override List<Vector3> ConditionSpawn()
    {
        return Gen_Ile.GetPointsEau();
    }



    public override void InteragirObjet()
    {
        if (!EstR�par� && SontPr�alablesRemplis())
        {
            R�parerBateau();
        }
        else if (EstR�par�)
        {
            ComportementInterface.instance.Activ��tatMenu();
            GameObject.FindGameObjectWithTag("Player").GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(AnimerVictoire());
        }
    }

    void R�parerBateau()
    {
        EstR�par� = true;

        messageBateau = "S'enfuir";
        ObjectRamassableVisible.Remove(CeBateau);
        

        if (cout1Utilis�)
        {
            var instance = Instantiate(BateauR�par�Moteur, transform);
            InitialiserDataBateau(instance);
        }
        else
        {
            var instance = Instantiate(BateauR�par�Voile, transform);
            InitialiserDataBateau(instance);
        }
        
        Destroy(instanceBris�);
        Destroy(canevas.gameObject);

        ObjectRamassableVisible.Add(CeBateau);
        GestionInteraction.RendrePlusClair(CeBateau);


    }

    IEnumerator AnimerVictoire()
    {
        var joueur = GameObject.FindGameObjectWithTag("Player");
        joueur.GetComponent<ControlPersonnage>().enabled = false;
        Camera cam = joueur.GetComponentInChildren<Camera>();

        transform.position = new(transform.position.x, transform.position.y + 1, transform.position.z);
        cam.transform.SetPositionAndRotation(new(transform.localPosition.x, transform.localPosition.y + 2.5f, transform.localPosition.z), transform.rotation);
        yield return new WaitForSeconds(0.5f);

        StartCoroutine(AnimerRotation(cam));
    }
    IEnumerator AnimerRotation(Camera cam)
    {
        Quaternion rotationDepart = transform.rotation;
        Quaternion rotationFinale = Quaternion.LookRotation(transform.position - FeuDeCamp.instance.FeuCourant.transform.position);

        float timer = 0;
        while (timer < 3)
        {
            cam.transform.rotation = Quaternion.Lerp(rotationDepart, rotationFinale, timer / 3);
            transform.rotation = Quaternion.Lerp(rotationDepart, rotationFinale, timer / 3);

            timer += Time.deltaTime * 1.25f;
            yield return null;
        }
        StartCoroutine(AnimerDeplacement(cam));
        StartCoroutine(GestionBruit.instance.JouerSonFin("VictoireBateau"));
    }
    IEnumerator AnimerDeplacement(Camera cam)
    {
        float timer = 0;

        Vector3 direction = transform.forward;
        direction.y = 0;

        while (timer < 3)
        {

           transform.position += 5f * Time.deltaTime * direction;
            cam.transform.position += 5 * Time.deltaTime * direction;

            timer += Time.deltaTime;
            yield return null;
        }
        
        EndGameUI.instance.CommencerEndGame(�tatFinPartie.Bateau);
    }



    public override string D�terminerTexteUI()
    {
        return messageBateau;
    }

    void InitialiserDataBateau(GameObject bateau)
    {
        CeBateau.objet = gameObject;
        CeBateau.mr = bateau.GetComponentsInChildren<MeshRenderer>();

        Color[] couleurs = new Color[CeBateau.mr.Length];

        for (int i = 0; i < CeBateau.mr.Length; i++)
        {
            couleurs[i] = CeBateau.mr[i].material.color;
        }

        CeBateau.CouleursOriginale = couleurs;
        CeBateau.AngleInteraction = 50;
    }



    protected override void EstEntr�EnCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Add(CeBateau);
    }

    protected override void EstSortiDeCollision(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            ObjectRamassableVisible.Remove(CeBateau);
    }
}
