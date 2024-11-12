using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manger : MonoBehaviour
{
    public static Manger instance;
    public GameObject cardsPrefab;

    CanvasCtrl canvasCtrl;

    [HideInInspector] public List<int> SelectedCards;
    [HideInInspector] public List<CardCtrl> CardsCollection;

    [HideInInspector] public List<CardCtrl> JokersCards;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        canvasCtrl = CanvasCtrl.instance;
    }

    public void AddCard(CardCtrl theCard,int CardCount)
    {
        if (theCard.count >= 52) { JokersCards.Add(theCard); }

        SelectedCards.Add(CardCount);
        CardsCollection.Add(theCard);
    }

    public void RemoveCard(CardCtrl theCard, int CardCount)
    {
        if (JokersCards.Contains(theCard)) { JokersCards.Remove(theCard); }

        SelectedCards.Remove(CardCount);
        CardsCollection.Remove(theCard);
    }

    public void Down()
    {
        if (IsMeldValid(SelectedCards)) 
        {
            canvasCtrl.DownCards(CardsCollection);
            CardsCollection.Clear();
            SelectedCards.Clear();
            JokersCards.Clear();
            canvasCtrl.DownButon.color = Color.gray;
        }
    }

    public bool IsMeldValid(List<int> selectedcards)
    {
        List<int> cards = new();
        List<int> Jokers = new();
        
        cards.AddRange(selectedcards);
        cards.Sort();

        //separate Jokers from other Cards
        Jokers = cards.FindAll(c => c > 52);
        cards.RemoveAll(c => c > 52);

        if (cards.Count + Jokers.Count < 3 || cards.Count == 0)return false;

        //store every card Suite and Rank
        List<int> Suits = new();
        List<int> Ranks = new();

        foreach (int card in cards)
        {
            int cardSuit = (card < 52) ? card / 13 : -1;    // Set card Suit {heart,black, ...]

            int cardRank = (card < 52) ? card % 13 : -1;    // Set card Rank [A,2,3,4..K]

            Suits.Add(cardSuit);
            Ranks.Add(cardRank);
        }

        //check if there are Repeated Cards: 
        bool RepeatedSuit = false;
        bool RepeatedRank = false;

        for (int i = 0; i < Suits.Count; i++)
        {
            for (int j = 0; j < Suits.Count; j++)
            {
                if (i != j)
                {
                    if (Suits[i] == Suits[j]) RepeatedSuit = true;
                    if (Ranks[i] == Ranks[j]) RepeatedRank = true;
                }
            }
        }

        if (RepeatedRank && RepeatedSuit) return false;

        ///Same Rank Diff Suit:        
        if (Ranks.All(R => R == Ranks[0]) && cards.Count + Jokers.Count <= 4) return true;

        //Same Suits Series Number:
        /* STEPS:
        1- convert the Collection into Groups of series nums
        2- add 13 to all groups exept the ones that contain[10-k] Cards
        3- Calculates the Gaps between The groups 
        4- check if Jokers can fit Gap 
        5- collect all groups into New Single series
        */

        foreach (var S in Suits) if (S != Suits[0]) return false;

        List<int> SortedCards = new(Ranks);
        SortedCards.Sort();

        //Atomize Collection into serial groups
        int[][] Groups = new int[1][];
        List<int> subGroups = new();
        int groupsCount = 1;

        //separate Collection into Series Groups
        for (int i = 0; i < SortedCards.Count; i++)
        {
            if (i > 0 && Mathf.Abs(SortedCards[i] - SortedCards[i - 1]) != 1)
            {
                groupsCount++;
                subGroups.Clear();
                System.Array.Resize(ref Groups, groupsCount);
            }

            subGroups.Add(SortedCards[i]);

            System.Array.Resize(ref Groups[groupsCount - 1], subGroups.Count);
            Groups[groupsCount - 1] = subGroups.ToArray();
        }

        //Rearrange new Groups into serial Group start form Last Group:
        List<int> MeanGroup = new();

        //  if it has upper Ranks & lower Ranks
        bool isPeriodic = SortedCards[0] + 13 - SortedCards[^1] <= 4;

        if (Groups.Length > 1 && isPeriodic)
        {
            MeanGroup.AddRange(Groups[^1]);
            List<int> InetialGroup = Groups[0].ToList();

            for (int i = 0; i < Groups.Length - 1; i++)
            {
                bool IsGapped = Mathf.Abs(InetialGroup[^1] - Groups[i][0]) < Mathf.Abs(Groups[^1][0] - Groups[i][^1]);
                
                if(IsGapped) 
                    for (int j = 0; j < Groups[i].Length; j++) 
                        Groups[i][j] += 13;
                
                MeanGroup.AddRange(Groups[i]);
            }
        }
        else
        {
            for (int i = 0; i < Groups.Length; i++)
            {
                MeanGroup.AddRange(Groups[i]);
            }
        }
        MeanGroup.Sort();

        //Find Holes in new Collection:
        List<int> SeriesHoles = new();

        for (int i = 0; i < MeanGroup.Count - 1; i++) 
        {
            if (Mathf.Abs(MeanGroup[i] - MeanGroup[i + 1]) != 1)
            {
                for (int j = MeanGroup[i] + 1; j < MeanGroup[i + 1]; j++) 
                {
                    SeriesHoles.Add(j);
                }
            }
        }
        if (SeriesHoles.Count < 1) return true;

        if (SeriesHoles.Count > Jokers.Count) return false;

        else return true;
    }
}
