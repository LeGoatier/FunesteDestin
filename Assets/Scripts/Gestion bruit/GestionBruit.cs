using System;
using UnityEngine;
using System.Collections;

public class GestionBruit : MonoBehaviour
{
    public static GestionBruit instance;

    public Playlist playlistJour;
    public Playlist playlistNuit;
    public Playlist playlistSons;

    private Playlist[] playlists;
    public Son chansonCourante;

    private void Awake()
    {
        if (instance == null)
            instance = this;

        playlists = new Playlist[] { playlistNuit, playlistJour, playlistSons };

        // On boucle à travers chaque Son et on ajoute un AudioSource au GameObject de GestionBruit avec chaque audioclip et ses paramètres
        foreach (Playlist p in playlists)
            foreach(Son s  in p.sons)
            {
                s.audioSource = gameObject.AddComponent<AudioSource>();
                s.audioSource.clip = s.clip;
                s.audioSource.loop = s.loop;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
            }

        //Le jeu commence dans le menu, alors on lance la musique de menu dans le Awake
        JouerChansonMenu();
        DontDestroyOnLoad(this.gameObject);
    }

    //Cette méthode est appelée dans plusieurs scripts de différentes classes afin de jouer un Son
    //Par exemple, le son "Marcher" joue lorsque la magnitude du vecteur de direction de mouvement du joueur est supérieur à 0  ou lorsque le joueur ramasse une ressource
    public void JouerSon(string nom)
    {
        Son son = Array.Find(playlistSons.sons, son => son.nom == nom);

        if (son != null && !son.audioSource.isPlaying)       
            son.audioSource.Play();
    }

    //Permet d'arrêter des Sons qui jouent en boucle, comme le son Marcher
    public void ArreterSon(string nom)
    {
        Son son = Array.Find(playlistSons.sons, son => son.nom == nom);
        son?.audioSource.Stop();
    }

    //Prend en compte la distance de l'objet qui émet le son par rapport a la caméra
    public void JouerSon3D(GameObject objet, string nom)
    {
        Son son = Array.Find(playlistSons.sons, son => son.nom == nom);

        AudioSource source = objet.GetComponent<AudioSource>();

        //Ajoute un AudioSource à l'objet qui appelle la méthode s'il n'en a pas déjà
        //Par exemple, l'ennemi qui attaque
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            //Permet de jouer un son en 3D, donc plus la source est loin de la caméra, moins le son est entendu fortement
            source.spatialBlend = 1;
            source.maxDistance = 150;
        }

        //Joue le son avec le nom déclaré s'il n'est pas null
        if(son!= null)
        {
            source.clip = son.clip;
            source.volume = son.volume;
            source.pitch = son.pitch;
            source.Play();
        }
    }

    //Permet de recommencer la musique du menu, par exemple, lorsque le joueur gagne/meurt ou s'il retourne au menu
    public void JouerChansonMenu()
    {
        if(chansonCourante.clip !=null)
            chansonCourante.audioSource.Stop();
        JouerChanson(Array.Find(playlistJour.sons, son => son.nom == "MusiqueMenu"));
    }

    //Méthode appelée dans GestionScène lorsque la Scène du jeu est appelée
    public void LancerMusiquePartie()
    {
        StartCoroutine(FadeOutSon(chansonCourante, 0.5f));
        JouerChanson(TrouverChansonAléatoire(playlistJour));
    }

    //Méthode appelée lorsque le joueur gagne ou perd
    //Le son est soit un son spécifique à la victoire, par exemple, un bruit de fusée, ou une musique sombre et triste pour la mort
    public IEnumerator JouerSonFin(string nom)
    {
        JouerSon(nom);

        //On joue le son seulement 4 secondes
        yield return new WaitForSeconds(4);
        StartCoroutine(FadeOutSon(Array.Find(playlistSons.sons, son => son.nom == nom),2));
    }

    //Musique ambiance / trames sonores
    private void JouerChanson(Son chanson)
    {
        chanson?.audioSource.Play();
        chansonCourante = chanson;
    }

    public void ChangerChansonJourEtNuit(bool estJour)
    {
        //On arrête la chansonCourante et on joue une chanson aléatoire dans la Playlist adéquate
        StartCoroutine(FadeOutSon(chansonCourante, 2));
        JouerChanson(TrouverChansonAléatoire(estJour ? playlistJour : playlistNuit));
    }

    private Son TrouverChansonAléatoire(Playlist playlist) => playlist.sons[UnityEngine.Random.Range(0, playlist.sons.Length - 1)]; // -1, car le dernier Son dans la liste est la chanson du menu

    //Arrête graduellement un son en utilisant une coroutine
    IEnumerator FadeOutSon(Son son, float durée = 3f)
    {
        float t = 0;
        float volumeDépart = son.volume;

        while (t < durée)
        {
            t += Time.deltaTime;
            son.audioSource.volume = Mathf.Lerp(volumeDépart, 0, t / durée);
            yield return null;
        }

        son.audioSource.Stop();
        son.audioSource.volume = volumeDépart;
    }
}

[System.Serializable]
public class Playlist
{
    public Son[] sons;
}

[System.Serializable]
public class Son
{
    public string nom;
    public AudioClip clip;
    public bool loop;

    [Range(0,1)]
    public float volume;
    [Range(0.1f, 3)]
    public float pitch;

    [HideInInspector]
    public AudioSource audioSource;
}
