using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TowerRules
{
    public static Dictionary<int, CardRelation> DefaultCardRelationships = new Dictionary<int, CardRelation>();

    static TowerRules()
    {
        DefaultCardRelationships.Add(0,  new CardRelation(new List<int>() {    }));
        DefaultCardRelationships.Add(1,  new CardRelation(new List<int>() {0   }));
        DefaultCardRelationships.Add(2,  new CardRelation(new List<int>() {0   }));
        DefaultCardRelationships.Add(3,  new CardRelation(new List<int>() {1   }));
        DefaultCardRelationships.Add(4,  new CardRelation(new List<int>() {1, 2}));
        DefaultCardRelationships.Add(5,  new CardRelation(new List<int>() {2   }));
        DefaultCardRelationships.Add(6,  new CardRelation(new List<int>() {3   }));
        DefaultCardRelationships.Add(7,  new CardRelation(new List<int>() {3, 4}));
        DefaultCardRelationships.Add(8,  new CardRelation(new List<int>() {4, 5}));
        DefaultCardRelationships.Add(9,  new CardRelation(new List<int>() {5   }));
        DefaultCardRelationships.Add(10, new CardRelation(new List<int>() {6   }));
        DefaultCardRelationships.Add(11, new CardRelation(new List<int>() {6, 7}));
        DefaultCardRelationships.Add(12, new CardRelation(new List<int>() {7, 8}));
        DefaultCardRelationships.Add(13, new CardRelation(new List<int>() {8, 9}));
        DefaultCardRelationships.Add(14, new CardRelation(new List<int>() {9   }));
    }

    public static List<int> getAdjacentIndexes(int IndexToLookup)
    {
        if (DefaultCardRelationships.Keys.Contains(IndexToLookup))
            return DefaultCardRelationships[IndexToLookup].AdjacentCards;
        else
            return new List<int>();   //Index is *NOT* in the card relation list
    }
}

public class CardRelation
{
    public List<int> AdjacentCards = new List<int>();

    public CardRelation(List<int> adjacentCards)
    {
        AdjacentCards = adjacentCards;
    }
}
