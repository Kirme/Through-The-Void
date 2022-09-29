using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameModeHandler : MonoBehaviour
{

    public bool tutorialMode = true;

    public UnityEvent OnTutorialMode;
    public UnityEvent OnRegularMode;
    public GameObject player;

    public void Start()
    {
        if (tutorialMode)
        {
            EnterTutorialMode();
        }
        else
        {
            EnterRegularMode();
        }
    }

    public void ChangeGameMode(bool val)
    {
        if(val == tutorialMode)
        {
            return;
        }

        tutorialMode = val;

        if (tutorialMode)
        {
            EnterTutorialMode();
        } else
        {
            EnterRegularMode();
        }

    }

    public void Break(Fault fault, int numFautls)
    {
        if(numFautls >= 5)
        {
            ChangeGameMode(true);
        }
    }

    public void EnterTutorialMode()
    {
        OnTutorialMode.Invoke();
    }

    public void EnterRegularMode()
    {
        OnRegularMode.Invoke();
    }

}
