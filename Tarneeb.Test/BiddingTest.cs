using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tarneeb.Models;

namespace Tarneeb.Test
{
    [TestClass]
    public class BiddingTest
    {
        [TestMethod]
        public void TestBasicBidding()
        {
            GameSession gameSession = PrepareGameSession();
            Assert.AreEqual<GameSessionStatus>(GameSessionStatus.Bidding, gameSession.Status);

            BiddingState tempBiddingState;
            //--1st bid: East 1 Diamonds
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.East,tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(0, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreateBid(
                gameSession.GetPlayer(PlayerPosition.East), 
                1, Suit.Diamonds)
                );
            
            //--2nd bid: North Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.North, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(1, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.North)));

            //--3rd bid: West Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.West, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(2, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.West)));

            //--4th bid: South 1 Spades
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.South, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(3, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreateBid(
                gameSession.GetPlayer(PlayerPosition.South),
                1,Suit.Spades)
                );

            //--5th bid: East 2 Diamonds
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.East, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(4, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreateBid(
                gameSession.GetPlayer(PlayerPosition.East),
                2, Suit.Diamonds)
                );

            //--6th bid: North Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.North, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(5, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.North)));

            //--7th bid: West Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.West, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(6, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.West)));

            //--8th bid: South 2 Spades
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.South, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(7, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreateBid(
                gameSession.GetPlayer(PlayerPosition.South),
                2, Suit.Spades)
                );
            //--9th bid: East Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.East, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(8, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.East))
                );

            //--10th bid: North Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.North, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(9, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.North)));

            //--11th bid: West Pass
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(false, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition>(PlayerPosition.West, tempBiddingState.CurrentTurn);
            Assert.AreEqual<int>(10, tempBiddingState.Bids.Count);
            gameSession.PlaceBid(
                Bid.CreatePassBid(
                gameSession.GetPlayer(PlayerPosition.West)));
            //--Auction Closed
            tempBiddingState = gameSession.GetGameBiddingState();
            Assert.AreEqual<bool>(true, gameSession.GetGameBiddingState().IsClosed);
            Assert.AreEqual<PlayerPosition?>(PlayerPosition.South, tempBiddingState.Winner);
            Assert.AreEqual<GameSessionStatus>(GameSessionStatus.GamePlay, gameSession.Status);
        }

        [TestMethod]
        public void TestDoubleBidding()
        {
        }

        [TestMethod]
        public void TestReDoubleBidding()
        {
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

            return gameSession;
        }
    }
}
