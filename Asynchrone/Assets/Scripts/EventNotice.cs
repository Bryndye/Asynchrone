using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventNotice : MonoBehaviour
{
    [SerializeField] private GameObject noticeActive;
    [SerializeField] private Image noticeImage;
    
    [SerializeField]
    GameObject global,noticeInteractJumes, noticeInteractV4trek, noticeKill, noticeDiversion, noticeVision, noticeSwitch;
    private enum selectNotice
    {
        InteractJumes,
        InteractV4trek,
        Kill,
        Diversion,
        Vision,
        SwitchPerso
    }
    [SerializeField]
    selectNotice selectionNotice;

    private void OnDrawGizmos()
    {
        switch (selectionNotice)
        {
            case selectNotice.InteractJumes:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(true);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(false);
                noticeVision.SetActive(false);
                noticeSwitch.SetActive(false);
                break;
            case selectNotice.InteractV4trek:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(true);
                noticeKill.SetActive(false);
                noticeVision.SetActive(false);
                noticeSwitch.SetActive(false);
                break;
            case selectNotice.Kill:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(true);
                noticeVision.SetActive(false);
                noticeSwitch.SetActive(false);
                break;
            case selectNotice.Diversion:
                noticeDiversion.SetActive(true);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(false);
                noticeVision.SetActive(false);
                noticeSwitch.SetActive(false);
                break;
            case selectNotice.Vision:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(false);
                noticeVision.SetActive(true);
                noticeSwitch.SetActive(false);
                break;
            case selectNotice.SwitchPerso:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(false);
                noticeVision.SetActive(false);
                noticeSwitch.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void ActiveNotice()
    {
        global.SetActive(!global.activeSelf);
    }

    private void Awake()
    {
        //noticeImage.gameObject.SetActive(false);
        noticeActive.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        //done
        if (other.CompareTag("Player"))
        {
            noticeActive.SetActive(true);
            noticeImage.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            noticeActive.SetActive(false);
        }
    }
}
