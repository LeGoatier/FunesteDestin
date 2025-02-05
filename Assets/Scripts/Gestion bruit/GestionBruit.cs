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

        // On boucle � travers chaque Son et on ajoute un AudioSource au GameObject de GestionBruit avec chaque audioclip et ses param�tres
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

    //Cette m�thode est appel�e dans plusieurs scripts de diff�rentes classes afin de jouer un Son
    //Par exemple, le son "Marcher" joue lorsque la magnitude du vecteur de direction de mouvement du joueur est sup�rieur � 0  ou lorsque le joueur ramasse une ressource
    public void JouerSon(string nom)
    {
        Son son = Array.Find(playlistSons.sons, son => son.nom == nom);

        if (son != null && !son.audioSource.isPlaying)       
            son.audioSource.Play();
    }

    //Permet d'arr�ter des Sons qui jouent en boucle, comme le son Marcher
    public void ArreterSon(string nom)
    {
        Son son = Array.Find(playlistSons.sons, son => son.nom == nom);
        son?.audioSource.Stop();
    }

    //Prend en compte la distance de l'objet qui �met le son par rapport a la cam�ra
    public void JouerSon3D(GameObject objet, string nom)
    {
        Son son = Array.Find(playlistSons.sons, son => son.nom == nom);

        AudioSource source = objet.GetComponent<AudioSource>();

        //Ajoute un AudioSource � l'objet qui appelle la m�thode s'il n'en a pas d�j�
        //Par exemple, l'ennemi qui attaque
        if (source == null)
        {
            source = gameObject.AddComponent<AudioSource>();
            //Permet de jouer un son en 3D, donc plus la source est loin de la cam�ra, moins le son est entendu fortement
            source.spatialBlend = 1;
            source.maxDistance = 150;
        }

        //Joue le son avec le nom d�clar� s'il n'est pas null
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

    //M�thode appel�e dans GestionSc�ne lorsque la Sc�ne du jeu est appel�e
    public void LancerMusiquePartie()
    {
        StartCoroutine(FadeOutSon(chansonCourante, 0.5f));
        JouerChanson(TrouverChansonAl�atoire(playlistJour));
    }

    //M�thode appel�e lorsque le joueur gagne ou perd
    //Le son est soit un son sp�cifique � la victoire, par exemple, un bruit de fus�e, ou une musique sombre et triste pour la mort
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
        //On arr�te la chansonCourante et on joue une chanson al�atoire dans la Playlist ad�quate
        StartCoroutine(FadeOutSon(chansonCourante, 2));
        JouerChanson(TrouverChansonAl�atoire(estJour ? playlistJour : playlistNuit));
    }

    private Son TrouverChansonAl�atoire(Playlist playlist) => playlist.sons[UnityEngine.Random.Range(0, playlist.sons.Length - 1)]; // -1, car le dernier Son dans la liste est la chanson du menu

    //Arr�te graduellement un son en utilisant une coroutine
    IEnumerator FadeOutSon(Son son, float dur�e = 3f)
    {
        float t = 0;
        float volumeD�part = son.volume;

        while (t < dur�e)
        {
            t += Time.deltaTime;
            son.audioSource.volume = Mathf.Lerp(volumeD�part, 0, t / dur�e);
            yield return null;
        }

        son.audioSource.Stop();
        son.audioSource.volume = volumeD�part;
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
