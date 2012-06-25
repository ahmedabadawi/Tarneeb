using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tarneeb.Models;

namespace Tarneeb.Test
{
    [TestClass]
    public class GamePlayTest
    {
        [TestMethod]
        public void TestBasicGame()
        {
            GameSession gameSession = PrepareGameSession();

            Assert.AreEqual<GameSessionStatus>(GameSessionStatus.GamePlay, gameSession.Status);

            Player north = gameSession.GetPlayer(PlayerPosition.North);
            Player south = gameSession.GetPlayer(PlayerPosition.South);
            Player east = gameSession.GetPlayer(PlayerPosition.East);
            Player west = gameSession.GetPlayer(PlayerPosition.West);

            Assert.AreEqual<Suit>(Suit.Spades, gameSession.CurrentBid.Suit);
            Assert.AreEqual<int>(2, gameSession.CurrentBid.Tricks);
            Assert.AreEqual<TeamPosition>(
                TeamPosition.NorthSouth, 
                gameSession.GetTeamPosition(
                gameSession.GetPlayerPosition(gameSession.CurrentBid.Player)));

            //---
            while (gameSession.Status == GameSessionStatus.GamePlay)
            {
                PlayerPosition currentTurn = gameSession.CurrentTurn.Value;

                GamePlayState tempState;
                tempState = gameSession.GetGamePlayState(gameSession.GetPlayer(currentTurn));
                List<Card> cards = tempState.CurrentCards;


                Card card = PlayRandomCard(cards, tempState.CurrentTrickBaseSuit);
                gameSession.PlaceCard(gameSession.GetPlayer(currentTurn), card);
                Console.WriteLine("{0}-{1}", currentTurn.ToString(), card.ToString());
            }
            Console.WriteLine("NS:{0} - EW:{1}",
                gameSession.MatchScore.Scores[TeamPosition.NorthSouth],
                gameSession.MatchScore.Scores[TeamPosition.EastWest]);
        }

        private Card PlayRandomCard(List<Card> cards, Suit baseSuit)
        {
            IEnumerable<Card> validCards = null;
            int numberOfValidCards = cards.Where(c => c.Suit == baseSuit).Count();
            if (numberOfValidCards > 0)
            {
                validCards = cards.Where(c => c.Suit == baseSuit).Select(c => c);
            }
            else
            {
                validCards = new List<Card>(cards);
            }

            Random random = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

            int randomCardIndex = random.Next(0, validCards.Count() - 1);

            return new List<Card>(validCards)[randomCardIndex];
        }

        private GameSession PrepareGameSession()
        {
            GameSession gameSession = new GameSession();
            Player north = new Player(Guid.NewGuid(), "North");
            Player south = new Player(Guid.NewGuid(), "South");
            Player east = new Player(Guid.NewGuid(), "East");
            Player west = new Player(Guid.NewGuid(), "West");
            gameSession.Join(north, PlayerPosition.North);
            gameSession.Join(south, PlayerPosition.South);
            gameSession.Join(east, PlayerPosition.East);
            gameSession.Join(west, PlayerPosition.West);

            //--1st bid: East 1 Diamonds
            gameSession.PlaceBid(Bid.CreateBid(gameSession.GetPlayer(PlayerPosition.East),1, Suit.Diamonds));
            //--2nd bid: North Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.North)));
            //--3rd bid: West Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.West)));
            //--4th bid: South 1 Spades
            gameSession.PlaceBid(Bid.CreateBid(gameSession.GetPlayer(PlayerPosition.South),1, Suit.Spades));
            //--5th bid: East 2 Diamonds
            gameSession.PlaceBid(Bid.CreateBid(gameSession.GetPlayer(PlayerPosition.East),2, Suit.Diamonds));
            //--6th bid: North Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.North)));
            //--7th bid: West Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.West)));
            //--8th bid: South 2 Spades
            gameSession.PlaceBid(Bid.CreateBid(gameSession.GetPlayer(PlayerPosition.South),2, Suit.Spades));
            //--9th bid: East Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.East)));
            //--10th bid: North Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.North)));
            //--11th bid: West Pass
            gameSession.PlaceBid(Bid.CreatePassBid(gameSession.GetPlayer(PlayerPosition.West)));
            //--Auction Closed =>NS - 2 Spades

            return gameSession;
        }
    }
}
