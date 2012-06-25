using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tarneeb.Models
{
    /// <summary>
    /// Handles one stage of game play, the game play object is created upon changing
    /// the state of the game to Game Play state to handle the game operations, and is 
    /// destroyed when the game is complete after 13 tricks and game moved to the
    /// either another bidding state or final score state.
    /// </summary>
    class GamePlay
    {
        /// <summary>
        /// Reference to the game session for callbacks
        /// </summary>
        private GameSession gameSession;

        /// <summary>
        /// The shuffled cards for each player identified by player position
        /// </summary>
        public Dictionary<PlayerPosition, List<Card>> PlayerCards { get; private set; }

        /// <summary>
        /// The cards in the current trick identified by the player position
        /// </summary>
        public Dictionary<PlayerPosition,Card> CurrentTrick { get; private set; }

        /// <summary>
        /// The current player position
        /// </summary>
        public PlayerPosition CurrentTurn { get; private set; }

        /// <summary>
        /// The game play start position, should be the winner of the bid
        /// </summary>
        private PlayerPosition startPosition;
        
        /// <summary>
        /// The number of tricks played (each trick is 4 cards by each player)
        /// </summary>
        private int currentNumberOfTricks;

        /// <summary>
        /// The base suit of the current trick (the suit of the first card played 
        /// in the current trick)
        /// </summary>
        public Suit CurrentTrickBaseSuit { get; private set; }
        
        /// <summary>
        /// The trump suit of the winning bid
        /// </summary>
        private Suit trumpSuit;

        /// <summary>
        /// Dictionary of the number of tricks won by each team during the game play
        /// </summary>
        public Dictionary<TeamPosition, int> TricksWon { get; private set; }

        /// <summary>
        /// Indicates whether the game play is complete (13 tricks played) or not
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return currentNumberOfTricks >= 13;
            }
        }

        public GamePlay(GameSession gameSession,PlayerPosition startPosition,Suit trumpSuit)
        {
            PlayerCards = new Dictionary<PlayerPosition, List<Card>>(4);
            CurrentTrick = new Dictionary<PlayerPosition,Card>(4);
            
            TricksWon = new Dictionary<TeamPosition, int>(4);
            TricksWon[TeamPosition.NorthSouth] = 0;
            TricksWon[TeamPosition.EastWest] = 0;

            this.gameSession = gameSession;
            this.startPosition = startPosition;
            this.trumpSuit = trumpSuit;

            CurrentTurn = startPosition;
            currentNumberOfTricks = 0;

            //Shuffle cards and fill player cards
            CardsShuffler shuffler = new CardsShuffler();
            var shuffledCards = shuffler.Shuffle();
            PlayerCards[PlayerPosition.South] = shuffledCards[0];
            PlayerCards[PlayerPosition.East] = shuffledCards[1];
            PlayerCards[PlayerPosition.North] = shuffledCards[2];
            PlayerCards[PlayerPosition.West] = shuffledCards[3];

        }

        private void NextTurn()
        {
            if (CurrentTurn == PlayerPosition.West)
            {
                CurrentTurn = PlayerPosition.South;
            }
            else
            {
                CurrentTurn++;
            }
        }

        /// <summary>
        /// Place the specified card by the specified player in the current trick.
        /// And evaluate the trick to determine the winner
        /// </summary>
        public void PlaceCard(Player player,Card card)
        {
            PlayerPosition position = gameSession.GetPlayerPosition(player);
            if (position == CurrentTurn)
            {
                Card tempCard;
                if (!CurrentTrick.TryGetValue(position, out tempCard))
                {
                    if (ValidatePlay(position, card))
                    {

                        CurrentTrick[position] = card;
                        PlayerCards[position].Remove(card);
                        if (CurrentTrick.Values.Count == 1)
                        {
                            //First card in the trick
                            CurrentTrickBaseSuit = card.Suit;
                        }

                        if (CurrentTrick.Values.Count >= 4)
                        {
                            //Trick is complete -> evalueate
                            PlayerPosition trickWinner = EvaluateCurrentTrick();
                            CurrentTurn = trickWinner;

                            switch (trickWinner)
                            {
                                case PlayerPosition.North:
                                case PlayerPosition.South:
                                    TricksWon[TeamPosition.NorthSouth]++;
                                    break;
                                case PlayerPosition.East:
                                case PlayerPosition.West:
                                    TricksWon[TeamPosition.EastWest]++;
                                    break;
                            }
                            CleanCurrentTrick();
                            currentNumberOfTricks++;

                            if (IsComplete)
                            {
                                gameSession.GameComplete();
                            }
                        }
                        else
                        {

                            NextTurn();
                        }
                    }
                    else
                    {
                        throw new ArgumentException("The played card is invalid");
                    }
                }
                else
                {
                    throw new InvalidOperationException("The player already placed card for the current trick");
                }
            }
            else
            {
                throw new InvalidOperationException("The player has played not in the right turn");
            }

        }

        private void CleanCurrentTrick()
        {
            CurrentTrick.Remove(PlayerPosition.North);
            CurrentTrick.Remove(PlayerPosition.South);
            CurrentTrick.Remove(PlayerPosition.East);
            CurrentTrick.Remove(PlayerPosition.West);

        }

        private bool ValidatePlay(PlayerPosition position, Card card)
        {
            //TODO: validate the played card
            return true;
        }


        private PlayerPosition EvaluateCurrentTrick()
        {
            Card maxCard=CurrentTrick[CurrentTurn];
            PlayerPosition trickWinner = CurrentTurn;

            foreach(PlayerPosition tempPosition in CurrentTrick.Keys)
            {
                if (tempPosition != CurrentTurn)
                {
                    Card tempCard = CurrentTrick[tempPosition];
                    if (CompareCards(tempCard, maxCard) > 0)
                    {
                        maxCard = tempCard;
                        trickWinner = tempPosition;
                    }
                }
            }
            return trickWinner;
        }

        private int CompareCards(Card card1, Card card2)
        {
            Suit tempTrump = trumpSuit;
            if (trumpSuit == Suit.None)
            {
                //Handle no trump
                trumpSuit = CurrentTrickBaseSuit;
            }
            if (card2.Suit != tempTrump && card1.Suit != tempTrump)
            {
                return 0;
            }
            else if (card2.Suit != tempTrump && card1.Suit == tempTrump)
            {
                return 1;
            }
            else if (card1.Suit != tempTrump && card2.Suit != tempTrump)
            {
                return -1;
            }
            else
            {
                return card1.CompareTo(card2);
            }
        }


    }
}