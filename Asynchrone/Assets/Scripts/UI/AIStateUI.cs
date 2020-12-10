using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIStateUI : MonoBehaviour
{
    CanvasHolder CH;

    [Header("Composants")]
    public Image FondInterrogation;
    public Image InterrogationAmount;
    public Image Pursuit;
    [HideInInspector]
    public anAI myFighter;

    public void Declaration(anAI ToFollow)
    {
        CH = CanvasHolder.Instance;
        transform.parent = CH.transform.GetChild(3);

        FondInterrogation.gameObject.SetActive(false);
        InterrogationAmount.fillAmount = 0;
        InterrogationAmount.gameObject.SetActive(false);
        Pursuit.gameObject.SetActive(false);

        myFighter = ToFollow;
        GetComponent<aFollowingUI>().theRenderer = ToFollow.GetComponent<Renderer>();
    }

    private void Update()
    {
        if (myFighter)
        {
            if(myFighter.mySituation == Situation.Interrogation)
            {
                if (Pursuit.gameObject.activeSelf)
                    Pursuit.gameObject.SetActive(false);
                if (!FondInterrogation.gameObject.activeSelf)
                    FondInterrogation.gameObject.SetActive(true);
                if (!InterrogationAmount.gameObject.activeSelf)
                    InterrogationAmount.gameObject.SetActive(true);

                float Amount = Mathf.Clamp(myFighter.TempsInterrogation / myFighter.LatenceInterrogation, 0, 1);
                InterrogationAmount.fillAmount = Amount; 
            }

            else if(myFighter.mySituation == Situation.Pursuit)
            {
                if (!Pursuit.gameObject.activeSelf)
                    Pursuit.gameObject.SetActive(true);
                if (FondInterrogation.gameObject.activeSelf)
                    FondInterrogation.gameObject.SetActive(false);
                if (InterrogationAmount.gameObject.activeSelf)
                    InterrogationAmount.gameObject.SetActive(false);
            }

            else
            {
                if (Pursuit.gameObject.activeSelf)
                    Pursuit.gameObject.SetActive(false);
                if (FondInterrogation.gameObject.activeSelf)
                    FondInterrogation.gameObject.SetActive(false);
                if (InterrogationAmount.gameObject.activeSelf)
                    InterrogationAmount.gameObject.SetActive(false);
            }
        }
    }
}
