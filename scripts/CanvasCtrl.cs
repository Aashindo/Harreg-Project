using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
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

        Jokers.AddRange(cards.FindAll(c => c.name.Contains("Joker")));
        
        cards.RemoveAll(c => c.name.Contains("Joker"));        

        if (cards.All(c => c.Suit == cards[0].Suit))
        {
            int serv = cards.Min(c => c.Rank);
            int lord = cards.Max(c => c.Rank);

            for (int i = serv; i <= lord; i++)
            {
                if (cards.Find(c => c.Rank == i)) 
                    sortedCards.Add(cards.Find(c => c.Rank == i));
            }

            cards.Clear();
            cards.AddRange(sortedCards);
            sortedCards.Clear();

            List<CardCtrl> subGroup = new();
            int groupsCount = 1;

            //separate Collection into Groups
            CardCtrl[][] Groups = new CardCtrl[1][];

            for (int i = 0; i < cards.Count; i++)
            {
                if (i > 0 && Mathf.Abs(cards[i].Rank - cards[i - 1].Rank) != 1)
                {
                    groupsCount++;
                    subGroup.Clear();
                    System.Array.Resize(ref Groups, groupsCount);
                }

                subGroup.Add(cards[i]);

                System.Array.Resize(ref Groups[groupsCount - 1], subGroup.Count);
                Groups[groupsCount - 1] = subGroup.ToArray();
            }

            //Rearrange new Groups into serial Group start form Last Group:
            List<CardCtrl> MeanCards = new();

            bool isPeriodic = cards[0].Rank + 13 - cards[^1].Rank <= 4;

            if (Groups.Length > 1 && isPeriodic)
            {
                List<CardCtrl> InetialGroup = Groups[0].ToList();

                for (int i = 0; i < Groups.Length - 1; i++)
                {
                    bool IsGapped = Mathf.Abs(InetialGroup[^1].Rank - Groups[i][0].Rank)
                                  < Mathf.Abs(Groups[^1][0].Rank - Groups[i][^1].Rank);

                    if (IsGapped)
                    {
                        foreach (CardCtrl card in Groups[i])
                        {
                            card.Rank += 13;
                        }
                    }
                    MeanCards.AddRange(Groups[i]);
                }
                MeanCards.AddRange(Groups[^1]);
            }
            else
            {
                for (int i = 0; i < Groups.Length; i++)
                {
                    MeanCards.AddRange(Groups[i]);
                }
            }

            sortedCards.Clear();

            lord = MeanCards.Max(c => c.Rank);
            serv = MeanCards.Min(c => c.Rank);

            for (int i = serv; i <= lord; i++)
            {
                if (MeanCards.Find(c => c.Rank == i)) 
                    sortedCards.Add(MeanCards.Find(c => c.Rank == i));
            }
            
            MeanCards.Clear();
            MeanCards.AddRange(sortedCards);
            sortedCards.Clear();

            for (int i = 0; i < MeanCards.Count ; i++) 
            {
                sortedCards.Add(MeanCards[i]);

                if (i < MeanCards.Count - 1)
                {
                    if (Mathf.Abs(MeanCards[i].Rank - MeanCards[i + 1].Rank) != 1)
                    {
                        for (int j = 0; j < Mathf.Abs(MeanCards[i].Rank - MeanCards[i + 1].Rank) - 1; j++) 
                        {
                            CardCtrl joker = Jokers[0];

                            joker.Rank = sortedCards[^1].Rank + 1;

                            sortedCards.Add(joker);
                            Jokers.Remove(joker);
                        }
                    }

                }
            }

            for (int i = 0; i < Jokers.Count; i++)
            {
                CardCtrl joker = Jokers[i];

                joker.Rank = sortedCards[^1].Rank + 1;

                sortedCards.Add(joker);


            }

            sortedCards.FindAll(c => c.Rank >= 13).ForEach(c => c.Rank -= 13);
            
            CardCtrl prototypeCard = sortedCards.Find(c => c.name.Contains("Card "));
            sortedCards.FindAll(c => c.name.Contains("Joker")).ForEach(c =>
            {
                c.SuiteImage.color = prototypeCard.SuiteImage.color;
                foreach (TextMeshProUGUI rankText in c.RanksText)
                {
                    rankText.text = RankNames[c.Rank];
                    rankText.color = prototypeCard.RanksText[0].color;
                }
            });

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
