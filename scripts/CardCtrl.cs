using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardCtrl : MonoBehaviour
{
    Manger manger;
    CanvasCtrl canvasCtrl;

    public TextMeshProUGUI[] RanksText;
    public Image SuiteImage;

    [HideInInspector] public bool isSelected;
    [HideInInspector] public bool isDowned;

    [HideInInspector] public int count;
    /*[HideInInspector]*/ public int Rank;
    /*[HideInInspector]*/ public int Suit;
    void Start()
    {
        manger = Manger.instance;
        canvasCtrl = CanvasCtrl.instance;
        GetComponentInChildren<Image>().color = Color.gray ;

        SuiteImage.sprite = canvasCtrl.Suites[Suit];
        SuiteImage.color = (Suit % 2 == 0) ? Color.red : Color.black;

        name = "Card " +
            ((Rank < 0) ? "Joker" :
            canvasCtrl.Suites[Suit].name +" "+canvasCtrl.RankNames[Rank]);

        foreach (TextMeshProUGUI RankText in RanksText) 
        {
            RankText.text = (Rank < 0) ? "Joker" : canvasCtrl.RankNames[Rank];
            RankText.color = (Suit % 2 == 0) ? (Rank >= 0) ? Color.red : Color.black : Color.black;
        }
    }

    public void OnClick()
    {
        if (isDowned) return;

        if (!isSelected) manger.AddCard(this, count);
        else manger.RemoveCard(this, count);

        isSelected = !isSelected;
        
        GetComponentInChildren<Image>().color = isSelected ? Color.white : Color.gray;
        canvasCtrl.DownButon.color = manger.IsMeldValid(manger.SelectedCards) ? Color.white : Color.gray;
    }

  


}
