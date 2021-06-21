using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventNotice : MonoBehaviour
{
    [SerializeField] private GameObject noticeActive;
    [SerializeField] private Image noticeImage;
    
    [SerializeField]
    GameObject noticeInteractJumes, noticeInteractV4trek, noticeKill, noticeDiversion;
    private enum selectNotice
    {
        InteractJumes,
        InteractV4trek,
        Kill,
        Diversion
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
                break;
            case selectNotice.InteractV4trek:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(true);
                noticeKill.SetActive(false);
                break;
            case selectNotice.Kill:
                noticeDiversion.SetActive(false);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(true);
                break;
            case selectNotice.Diversion:
                noticeDiversion.SetActive(true);
                noticeInteractJumes.SetActive(false);
                noticeInteractV4trek.SetActive(false);
                noticeKill.SetActive(false);
                break;
            default:
                break;
        }
    }

    private void Start()
    {
        //noticeImage.gameObject.SetActive(false);
        noticeActive.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        //done
        noticeActive.SetActive(true);
        noticeImage.gameObject.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        noticeActive.SetActive(false);
    }
}
