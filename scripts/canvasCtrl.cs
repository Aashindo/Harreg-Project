using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvasCtrl : MonoBehaviour
{
    public static canvasCtrl instance;

    public Image CardsBoards;
    public Image RandomBoard;

    public Image DownButon;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        DownButon.color = Color.gray;
    }

}
