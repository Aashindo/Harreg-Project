using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasCtrl : MonoBehaviour
{
    public static CanvasCtrl instance;
    Manger manger;

    public Image DownButon;
    public Image CardsBoards;
    public Image RandomBoard;
    public Image DownBoard;
    public Image JokersBoard;

    public Image CollectionBoard;

    public Sprite[] Suites;

    public string[] RankNames = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        manger = Manger.instance;
        DownButon.color = Color.gray;

        int counter = 0;
        for (int j = 0; j < 5; j++)
        {
            Image cardBoard = Instantiate(CollectionBoard);
            for (int i = 0; i < 13; i++)
            {
                GameObject card = Instantiate(manger.cardsPrefab);
                card.transform.SetParent(cardBoard.transform);

                CardCtrl cardCtrl = card.GetComponent<CardCtrl>();

                int count = (counter < 52) ? counter : Random.Range(0, 50);

                cardCtrl.count = count;

                cardCtrl.Rank = (count < 52) ? count % 13 : -1;

                cardCtrl.Suit = (count < 52) ? count / 13 : 4;

                cardBoard.transform.SetParent(counter < 52 ? CardsBoards.transform : RandomBoard.transform);

                counter++;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject card = Instantiate(manger.cardsPrefab);
            card.transform.SetParent(JokersBoard.transform);

            card.name = "Joker " + i;

            CardCtrl cardCtrl = card.GetComponent<CardCtrl>();
            int count = 200 + i;

            cardCtrl.count = count;

            cardCtrl.Rank = -1;

            cardCtrl.Suit = 4;

        }
    }

    public void DownCards(List<CardCtrl> cards)
    {
        List<CardCtrl> sortedCards = new();
        List<CardCtrl> Jokers = new();
        Jokers.AddRange(cards.FindAll(c => c.Rank == -1));
        
        cards.RemoveAll(c => c.Rank == -1);        

        if (cards.All(c => c.Suit == cards[0].Suit))
        {
            int jokerStep = 0;
            for (int i = 0; i < cards.Count - 1; i+=1)
            {
                sortedCards.Add(cards[i]);
                jokerStep = Mathf.Abs(cards[i].Rank - cards[i + 1].Rank);

                bool isGapped = jokerStep != 1 && Jokers.Count > 0;
                if (isGapped)
                {
                    for (int j = 0; j < jokerStep-1; j++)
                    {
                        if (Jokers.Count <= 0) break;
                        CardCtrl joker = Jokers[0];
                        joker.Rank = cards[i].Rank + 1 + j;
                        if (joker.Rank >= 13) joker.Rank -= 13;

                        sortedCards.Add(joker);
                        Jokers.RemoveAt(0);
                    }
                }
            }

            sortedCards.Add(cards[^1]);
            
            if (Jokers.Count > 0)
            {
                foreach (CardCtrl J in Jokers)
                {
                    sortedCards.Add(J);
                }
            }

            int lord = sortedCards.Max(c => c.Rank);
            int serv = sortedCards.Min(c => c.Rank);

            List<CardCtrl> meanCards = new();

            for (int i = serv; i <= lord; i++)
            {
                if(sortedCards.Find(c => c.Rank == i))
                    meanCards.Add(sortedCards.Find(c => c.Rank == i));
            }

            sortedCards.Clear();

            if (lord == 12 && serv == 0)
            {
                for (int i = meanCards.Count - 1; i > 0; i--)
                {
                    if (meanCards[i].Rank - meanCards[i - 1].Rank == 1)
                    {
                        sortedCards.Add(meanCards[i]);
                        meanCards.Remove(meanCards[i]);
                    }
                    else { break; }
                }
                sortedCards.Add(meanCards[^1]);
                sortedCards.Reverse();
                sortedCards.AddRange(meanCards);

            }
            else sortedCards.AddRange(meanCards);
            
            for (int i = 0; i < sortedCards.Count; i++)
            {
                CardCtrl firstCard = sortedCards.Find(c => c.Rank > 0);
                if (sortedCards[i].name.Contains("Joker"))
                {
                    int rank = sortedCards[(i == 0) ? ^1 : i - 1].Rank + 1;

                    if (rank >= 13) rank -= 13;

                    sortedCards[i].SuiteImage.color = firstCard.SuiteImage.color;

                    foreach (TextMeshProUGUI rankText in sortedCards[i].RanksText)
                    {
                        rankText.color = firstCard.SuiteImage.color;
                        rankText.text = RankNames[rank];
                    }
                }
            }

        }
        else 
        {
            for (int i = 0; i < 4; i++)
            {
                if (cards.Find(c => c.Suit == i))
                {
                    sortedCards.Add(cards.Find(c => c.Suit == i));
                }
                else if (Jokers.Count > 0) 
                {
                    CardCtrl joker = Jokers[0];

                    joker.SuiteImage.sprite = Suites[i];
                    joker.SuiteImage.color = i % 2 == 0 ? Color.red : Color.black;

                    joker.RanksText[0].text = sortedCards.Find(c => c.Rank > 0).RanksText[0].text;

                    foreach (TextMeshProUGUI Rtext in joker.RanksText)
                    {
                        Rtext.color= i % 2 == 0 ? Color.red : Color.black; 
                    }

                    sortedCards.Add(joker);
                    Jokers.RemoveAt(0);
                }
            }
        }

        Image collectionsBoard = Instantiate(CollectionBoard);
        collectionsBoard.transform.SetParent(DownBoard.transform);
        collectionsBoard.GetComponent<HorizontalLayoutGroup>().spacing = -60;
        
        sortedCards.ForEach(C =>{
            C.transform.SetParent(collectionsBoard.transform);
            C.isDowned = true; 
        });
    }
}
