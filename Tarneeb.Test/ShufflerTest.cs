using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tarneeb.Models;

namespace Tarneeb.Test
{
    [TestClass]
    public class ShufflerTest
    {
        [TestMethod]
        public void TestShuffle()
        {            
            var shuffler = new CardsShuffler();
            var shuffledCards = shuffler.Shuffle();
            Assert.AreEqual<int>(4, shuffledCards.Count);
            foreach (var playerCards in shuffledCards)
            {
                Assert.AreEqual<int>(13, playerCards.Count);
            }
            
        }

        [TestMethod]
        public void TestSortCards()
        {
            Card testAS = new Card(1, Suit.Spades);
            Card testKS = new Card(13, Suit.Spades);
            Card testQS = new Card(12, Suit.Spades);
            List<Card> cards = new List<Card>() { testAS, testQS, testKS };
            cards.Sort();
            Assert.IsTrue(testAS.CompareTo(testKS) > 0);
            Assert.IsFalse(testKS.CompareTo(testAS) > 0);
            Assert.IsTrue(testAS.CompareTo(testQS) > 0);
            Assert.IsFalse(testQS.CompareTo(testAS) > 0);
            Assert.IsTrue(testKS.CompareTo(testQS) > 0);
            Assert.IsFalse(testQS.CompareTo(testKS) > 0);

        }
    }
}
