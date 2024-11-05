using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manger : MonoBehaviour
{
    public static Manger instance;
    public GameObject cardsPrefab;

    canvasCtrl canvasCtrl;

    public List<int> SelectedCards;
    public bool isMeldValid;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canvasCtrl = canvasCtrl.instance;

        for (int i = 0; i < 65; i++)
        {
            GameObject card = Instantiate(cardsPrefab, (i < 52) ? canvasCtrl.CardsBoards.transform: canvasCtrl.RandomBoard.transform);
            CardCtrl cardCtrl = card.GetComponent<CardCtrl>();
            cardCtrl.count = (i < 52) ? i : Random.Range(0, 50);
        }

    }

    public void addCard(int CardCount)
    {
        SelectedCards.Add(CardCount);
    }

    public void RemoveCard(int CardCount)
    {

        SelectedCards.Remove(CardCount);
    }

    public void Down()
    {
        MeldValid(SelectedCards);
        print(IsMeldValid(SelectedCards));
    }
    private void MeldValid(List<int> cards)
    {
        int Jokers = 0;

        for (int i = 0; i < cards.Count; i++)
        {
            if (cards[i] > 54)
            {
                Jokers++;
            }
        }

        if (cards.Count < 3 || Jokers>2) {
            isMeldValid = false;
            return;
        }
        ///Ex:[5,36,9,13]


        List<int> SortedOrders = new List<int>(cards);

        SortedOrders.Sort();
        ///store every card Type and Order

        List<int> Types = new();
        List<int> Ranks = new();

        //used for to cover 3 Cards or more
        for (int i = 0; i < SortedOrders.Count; i++)
        {
            if (SortedOrders[i] < 53)
            {

            int cardIndex = SortedOrders[i];

            // if it's Black ,Heart or Whatever  
            int cardType = cardIndex / 13;
            ///EX => Types:[0,2,0,1]

            int cardRank = cardIndex - (13 * cardType);
            ///Ex => Orders:[5,10,9,0]

            Ranks.Add(cardRank);
            Types.Add(cardType);
            }
        }

        List<int> SortedCards = new List<int>(Ranks);

        //Repeated Card: 
        bool RepeatedType = false;
        bool RepeatedRank = false;
        for (int i = 0; i < Types.Count; i++)
        {
            for (int j = 0; j < Types.Count; j++)
            {
                if (i != j)
                {
                    if (Types[i] == Types[j]) RepeatedType = true;
                    if (Ranks[i] == Ranks[j]) RepeatedRank = true;
                }
            }
        }
        if (RepeatedRank && RepeatedType)
        {
            isMeldValid = false;
            return;
        }


        //Same Rank Diff Suit:
        float FixedCount = Ranks[0];
        for (int i = 0; i < Ranks.Count; i++)
        {
            if (Ranks[i] != FixedCount) break;
            else if (i == Ranks.Count - 1 && !RepeatedType)
            {
                isMeldValid = true;
                
                return;
            }
        }


        //Same Suits Series Number 
        float FixedRank = Types[0];

        List<int> newSortedOrders = new List<int>(Ranks);
        newSortedOrders.Sort();

        bool isSeries = true;
        bool HasKingAndAce = newSortedOrders[newSortedOrders.Count - 1] == 12 && newSortedOrders[0] == 0;

        List<int> deltaCards = new();
        int M = -1;
        bool reached = false;
        for (int i = 0; i < Types.Count; i++)
        {
            if (Types[i] != FixedRank) break;
            else if (i == Ranks.Count - 1)
            {
                for (int j = 0; j < newSortedOrders.Count - 1; j++)
                {
                    if (!HasKingAndAce)
                    {
                        if (Mathf.Abs(newSortedOrders[j] - newSortedOrders[j + 1]) != 1 || RepeatedRank)
                        {
                            isSeries = false;
                        }
                    }
                    else
                    {
                        if (!reached)
                        {
                            M++;
                            if (Mathf.Abs(SortedCards[M] - SortedCards[M + 1]) != 1 || RepeatedRank) reached = true;
                            deltaCards.Add(SortedCards[M]);
                            newSortedOrders[M] = deltaCards[M] + 13;
                        }
                    }
                }
                newSortedOrders.Sort();

                for (int j = 0; j < newSortedOrders.Count - 1; j++)
                {
                    if (Mathf.Abs(newSortedOrders[j] - newSortedOrders[j + 1]) != 1 || RepeatedRank)
                    {
                        isSeries = false;
                    }

                }
                return;

            }
        }




        isMeldValid = false;

        return;


    }

    public bool IsMeldValid(List<int> cards)
    {
        if (cards.Count < 3)
        {

            return false;
        }

        List<int> SortedOrders = new List<int>(cards);

        SortedOrders.Sort();
        ///store every card Type and Order

        List<int> Types = new();
        List<int> Ranks = new();

        //used for to cover 3 Cards or more
        for (int i = 0; i < SortedOrders.Count; i++)
        {
            int cardIndex = SortedOrders[i];

            // if it's Black ,Heart or Whatever  
            int cardType = cardIndex / 13;
            ///EX => Types:[0,2,0,1]

            int cardRank = cardIndex % 13;
            ///Ex => Orders:[5,10,9,0]

            Ranks.Add(cardRank);
            Types.Add(cardType);
        }

        List<int> SortedCards = new List<int>(Ranks);

        //check if there are Repeated Card: 
        bool RepeatedType = false;
        bool RepeatedRank = false;
        for (int i = 0; i < Types.Count; i++)
        {
            for (int j = 0; j < Types.Count; j++)
            {
                if (i != j)
                {
                    if (Types[i] == Types[j]) RepeatedType = true;
                    if (Ranks[i] == Ranks[j]) RepeatedRank = true;
                }
            }
        }
        if (RepeatedRank && RepeatedType)
        {
            return false;
        }


        //Same Rank Diff Suit:
        float FixedCount = Ranks[0];
        for (int i = 0; i < Ranks.Count; i++)
        {
            if (Ranks[i] != FixedCount) break;
            else if (i == Ranks.Count - 1 && !RepeatedType)
            {
                return true;
            }
        }


        //Same Suits Series Number 
        float FixedRank = Types[0];

        List<int> newSortedOrders = new List<int>(Ranks);
        newSortedOrders.Sort();

        bool isSeries = true;
        bool HasKingAndAce = newSortedOrders[newSortedOrders.Count - 1] == 12 && newSortedOrders[0] == 0;

        List<int> deltaCards = new();
        int M = -1;
        bool reached = false;
        for (int i = 0; i < Types.Count; i++)
        {
            if (Types[i] != FixedRank) break;
            else if (i == Ranks.Count - 1)
            {
                for (int j = 0; j < newSortedOrders.Count - 1; j++)
                {
                    if (!HasKingAndAce)
                    {
                        if (Mathf.Abs(newSortedOrders[j] - newSortedOrders[j + 1]) != 1 || RepeatedRank)
                        {
                            isSeries = false;
                        }
                    }
                    else
                    {
                        if (!reached)
                        {
                            M++;
                            if (Mathf.Abs(SortedCards[M] - SortedCards[M + 1]) != 1 || RepeatedRank) reached = true;
                            deltaCards.Add(SortedCards[M]);
                            newSortedOrders[M] = deltaCards[M] + 13;
                        }
                    }
                }
                newSortedOrders.Sort();

                for (int j = 0; j < newSortedOrders.Count - 1; j++)
                {
                    if (Mathf.Abs(newSortedOrders[j] - newSortedOrders[j + 1]) != 1 || RepeatedRank)
                    {
                        isSeries = false;
                    }

                }
                return isSeries;

            }
        }
        return false;
    }


}
