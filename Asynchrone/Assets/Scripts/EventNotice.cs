using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventNotice : MonoBehaviour
{
    [SerializeField] private GameObject noticeActive;
    [SerializeField] private Image noticeImage;

    private void Start()
    {
        noticeImage.gameObject.SetActive(false);
        noticeActive.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        //done
        noticeActive.SetActive(true);
    }

    public void ActiveNotice()
    {
        noticeImage.gameObject.SetActive(!noticeImage.gameObject.activeSelf);
    }
}
