using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class Curseur : MonoBehaviour
{




    private void Awake()
    {
        CacherCurseurBugUnity();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Start()
    {
        ComportementInterface visibilitéMenu = GetComponent<ComportementInterface>();
        visibilitéMenu.OnMenuChangement += ChangerÉtatCurseur;
    }




    private void ChangerÉtatCurseur(bool JeuActif)
    {

        if (JeuActif)
        {
            LockCursor();
        }
        else
        {
            UnlockCursor();
        }

        //EstCurseurBloqué = JeuActif;
    }

    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


    }

    private void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;


    }

    void CacherCurseurBugUnity()
    {
        //Code volé d'internet, sinon faut cliquer au début pour enelver la souris 
        // https://forum.unity.com/threads/why-is-there-no-way-to-hide-the-mouse-cursor-when-entering-play-mode.1437727/
        //-----------------------------------------------------------
#if UNITY_EDITOR
        var gameWindow = EditorWindow
            .GetWindow(typeof(EditorWindow).Assembly.GetType("UnityEditor.GameView"));
        gameWindow.Focus();
        gameWindow.SendEvent(new Event
        {
            button = 0,
            clickCount = 1,
            type = EventType.MouseDown,
            mousePosition = gameWindow.rootVisualElement.contentRect.center
        });
#endif
        //-----------------------------------------------------------
    }
}
