using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCtrl : MonoBehaviour
{
    public TextMeshProUGUI CountText;
    public TextMeshProUGUI RankText;
    public TextMeshProUGUI TypeText;
    public int count;

    public bool Selected;

    string[] types = {"A","B","C","D"};
    void Start()
    {
        GetComponentInChildren<Image>().color = Color.gray ;
        
        CountText.text = count.ToString();
        int type = count / 13;
        TypeText.text = types[type];

        int Rank = count % 13;
        RankText.text=Rank.ToString();
    }

    public void OnClick()
    {
        if (!Selected) Manger.instance.addCard(count);
        else Manger.instance.RemoveCard(count);

        Selected = !Selected;
        GetComponentInChildren<Image>().color = Selected ? Color.white : Color.gray;
        canvasCtrl.instance.DownButon.color = Manger.instance.IsMeldValid(Manger.instance.SelectedCards) ? Color.white : Color.gray;
    }

  
}
