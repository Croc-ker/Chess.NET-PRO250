using Chess;
using Chess.Model.Game;
using Chess.Model.Rule;
using Chess.Model.Data;
using Chess.Model.Command;
using Chess.Model.Piece;
using Chess.Model.Visitor;
using System.Drawing;
using System.Net.NetworkInformation;
using NUnit.Framework;

namespace Chess.NET_Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        //happy paths
        [Test]
        public void spotsAreAllTaken() //are spots properly filled
        {
            var rulebook = new StandardRulebook();
            var taken = rulebook.nineSixty_initializePlacements();

            Assert.That(taken.Contains(-1), Is.False);
        }

        [Test]
        public void kingIsBetweenRooks() //checks if the king is between the rooks.
        {
            var rulebook = new StandardRulebook();
            for(int i = 0; i < 5; i++)
            {
                rulebook.nineSixty_initializePlacements();
                int king = rulebook.kingInt;
                int rook1 = rulebook.rook1Int;
                int rook2 = rulebook.rook2Int;
                Assert.That((rook1 < king && rook2 > king), Is.True);
            }
        }

        //bad paths
        [Test]
        public void bishopsOnSameColor() //i just use even/odd to find out if they are on the same color
        {
            var rulebook = new StandardRulebook();
            rulebook.nineSixty_initializePlacements();

            Assert.That(rulebook.bishopEven % 2 == 0 && rulebook.bishopOdd % 2 == 0, Is.True);
        }

        [Test]
        public void sidesAreNotMirrored() //checks if the black pieces are mirrored
        {
            var rulebook = new StandardRulebook();
            rulebook.nineSixty_initializePlacements();
            var game = rulebook.CreateGame960();
            var blackPieces = game.Board.GetPieces(Model.Piece.Color.Black);
            var whitePieces = game.Board.GetPieces(Model.Piece.Color.White);
            var blackArray = blackPieces.ToArray();
            var whiteArray = whitePieces.ToArray();
            bool mirrored = true;
            for (int i = 0; i < blackArray.Length; i++)
            {
                if (blackArray[i].Position.Column != whiteArray[i].Position.Column)
                {
                    mirrored = false;
                    break;
                }
            }
            Assert.That(mirrored, Is.False);
        }
    }
}