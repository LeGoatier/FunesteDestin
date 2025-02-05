using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    
    [SerializeField] ObjectInPool[] ObjectsInPool;
    private List<GameObject> pool = new List<GameObject>();

    public static ObjectPool instance;
    

    void Awake()
    {
        if (instance == null)
            instance = this;

        for (int i = 0; i < ObjectsInPool.Length; i++)
        {
            for (int o = 0; o < ObjectsInPool[i].Quantité; o++)
            {
                GameObject obj = Instantiate(ObjectsInPool[i].objet);
                obj.name = ObjectsInPool[i].objet.name;
                obj.SetActive(false);
                pool.Add(obj);
            }
        }
    }

    

    public GameObject GetPoolObject(GameObject typeObjet)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].name == typeObjet.name && !pool[i].activeInHierarchy)
            {
                return pool[i];
            }
        }

        //Si on est rendu ici, c'est que tous les objets sont déjà activés, alors on ajoutera un objet au pool
        if (EstDansLaListe(typeObjet))
        {
            GameObject obj = Instantiate(typeObjet);
            obj.name = typeObjet.name;
            obj.SetActive(false);
            pool.Add(obj);
            return obj;
        }
        

        return null;
    }

    bool EstDansLaListe(GameObject typeObjet)
    {
        foreach (ObjectInPool obj in ObjectsInPool)
        {
            if(typeObjet == obj.objet)
            {
                return true;
            }
        }

        return false;
    }

    //Code Original

    //[SerializeField] GameObject[] ObjectsToPool;
    //[SerializeField] int[] quantiteParObjet;

    //void Awake()
    //{
    //    if (instance == null)
    //        instance = this;

    //    for(int i = 0; i < Mathf.Min(ObjectsToPool.Length, quantiteParObjet.Length); i++)
    //    {
    //        for(int o = 0; o < quantiteParObjet[i]; o++)
    //        {
    //            GameObject obj = Instantiate(ObjectsToPool[i]);
    //            obj.name = ObjectsToPool[i].name;
    //            obj.SetActive(false);
    //            pool.Add(obj);
    //        }
    //    }
    //}

    //public GameObject GetPoolObject(GameObject typeObjet)
    //{
    //    for(int i = 0; i < pool.Count; i++)
    //    {
    //        if (pool[i].name == typeObjet.name && !pool[i].activeInHierarchy)
    //        {
    //            return pool[i];
    //        }
    //    }

    //    //Si on est rendu ici, c'est que tous les objets sont déjà activés, alors on ajoutera un objet au pool
    //    if (ObjectsToPool.Contains(typeObjet))
    //    {
    //        GameObject obj = Instantiate(typeObjet);
    //        obj.name = typeObjet.name;
    //        obj.SetActive(false);
    //        pool.Add(obj);
    //        return obj;
    //    }


    //    return null;
    //}
}

[System.Serializable]
public struct ObjectInPool
{
    public GameObject objet;
    public int Quantité;
}