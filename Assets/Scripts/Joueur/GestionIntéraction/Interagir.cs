using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public abstract class Interagir : MonoBehaviour
{
    public abstract void InteragirObjet();

    public abstract string D�terminerTexteUI();

    public virtual bool JouerSonInteraction() => false;
}
