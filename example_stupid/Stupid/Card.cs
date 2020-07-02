using System;
using System.Collections.Generic;
using funkylib;

namespace Stupid {
  public enum Suit {
    Hearts,
    Diamonds,
    Clubs,
    Spades
  }

  public enum Rank {
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Ten,
    Jack,
    Queen,
    King,
    Ace
  }

  public struct CardPair {
    public readonly Card placed;
    public readonly Option<Card> cover;

    public CardPair(Card placed, Option<Card> cover) {
      this.placed = placed;
      this.cover = cover;
    }
  }

  public struct Card : IComparable<Card> {
    public readonly Suit suit;
    public readonly Rank rank;

    public Card(Suit suit, Rank rank) {
      this.suit = suit;
      this.rank = rank;
    }

    public static Option<Card> parse(string s) {
      var parts = s.Trim();
      var suitePart = parts.Substring(0, 1).ToCharArray()[0];
      var rankPart = parts.Substring(1, parts.Length - 1).ToCharArray()[0];
      var suiteOpt = parseSuite(suitePart);
      var rankOpt = parseRank(rankPart);
      return suiteOpt.zip(rankOpt, (suite, rank) => new Card(suite, rank));
    }

    public static Option<Suit> parseSuite(char indicator) {
      switch (indicator) {
        case 'C': return Option.Some(Suit.Clubs);
        case 'S': return Option.Some(Suit.Spades);
        case 'H': return Option.Some(Suit.Hearts);
        case 'D': return Option.Some(Suit.Diamonds);
        default: return Option.None;
      }
    }

    public static Option<Rank> parseRank(char indicator) {
      switch (indicator) {
        case '2': return Option.Some(Rank.Two);
        case '3': return Option.Some(Rank.Three);
        case '4': return Option.Some(Rank.Four);
        case '5': return Option.Some(Rank.Five);
        case '6': return Option.Some(Rank.Six);
        case '7': return Option.Some(Rank.Seven);
        case '8': return Option.Some(Rank.Eight);
        case '9': return Option.Some(Rank.Nine);
        case 'T': return Option.Some(Rank.Ten);
        case 'J': return Option.Some(Rank.Jack);
        case 'Q': return Option.Some(Rank.Queen);
        case 'K': return Option.Some(Rank.King);
        case 'A': return Option.Some(Rank.Ace);
        default: return Option.None;
      }
    }

    sealed class RankSuiteRelationalComparer : Comparer<Card> {
      public override int Compare(Card x, Card y) {
        var rankComparison = x.rank.CompareTo(y.rank);
        if (rankComparison != 0) return rankComparison;
        return x.suit.CompareTo(y.suit);
      }
    }

    sealed class TrumpRankSuiteRelationalComparer : Comparer<Card> {
      readonly Suit trumpSuit;

      public TrumpRankSuiteRelationalComparer(Suit trumpSuit) { this.trumpSuit = trumpSuit; }

      public override int Compare(Card x, Card y) {
        if (x.suit == trumpSuit && y.suit != trumpSuit)
          return 1;
        return 
          x.suit != trumpSuit && y.suit == trumpSuit 
          ? -1 
          : rankSuiteComparer.Compare(x, y);
      }
    }

    public static Comparer<Card> comparerForTrumpSuit(Suit trumpSuit) => new TrumpRankSuiteRelationalComparer(trumpSuit);
    static Comparer<Card> rankSuiteComparer { get; } = new RankSuiteRelationalComparer();

    public int CompareTo(Card other) => rankSuiteComparer.Compare(this, other);
  }
}