//-----------------------------------------------------------------------
// <copyright file="StandardRulebook.cs">
//     Copyright (c) Michael Szvetits. All rights reserved.
// </copyright>
// <author>Michael Szvetits</author>
//-----------------------------------------------------------------------
namespace Chess.Model.Rule
{
    using Chess.Model.Command;
    using Chess.Model.Data;
    using Chess.Model.Game;
    using Chess.Model.Piece;
    using Chess.Model.Visitor;
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;

    /// <summary>
    /// Represents the standard chess rulebook.
    /// </summary>
    public class StandardRulebook : IRulebook
    {
        /// <summary>
        /// Represents the check rule of a standard chess game.
        /// </summary>
        private readonly CheckRule checkRule;

        /// <summary>
        /// Represents the end rule of a standard chess game.
        /// </summary>
        private readonly EndRule endRule;

        /// <summary>
        /// Represents the movement rule of a standard chess game.
        /// </summary>
        private readonly MovementRule movementRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardRulebook"/> class.
        /// </summary>
        public StandardRulebook()
        {
            var threatAnalyzer = new ThreatAnalyzer();
            var castlingRule = new CastlingRule(threatAnalyzer);
            var enPassantRule = new EnPassantRule();
            var promotionRule = new PromotionRule();

            this.checkRule = new CheckRule(threatAnalyzer);
            this.movementRule = new MovementRule(castlingRule, enPassantRule, promotionRule, threatAnalyzer);
            this.endRule = new EndRule(this.checkRule, this.movementRule);
        }

        /// <summary>
        /// Creates a new chess game according to the standard rulebook.
        /// </summary>
        /// <returns>The newly created chess game.</returns>
        public ChessGame CreateGame()
        {
            IEnumerable<PlacedPiece> makeBaseLine(int row, Color color)
            {
                yield return new PlacedPiece(new Position(row, 0), new Rook(color));
                yield return new PlacedPiece(new Position(row, 1), new Knight(color));
                yield return new PlacedPiece(new Position(row, 2), new Bishop(color));
                yield return new PlacedPiece(new Position(row, 3), new Queen(color));
                yield return new PlacedPiece(new Position(row, 4), new King(color));
                yield return new PlacedPiece(new Position(row, 5), new Bishop(color));
                yield return new PlacedPiece(new Position(row, 6), new Knight(color));
                yield return new PlacedPiece(new Position(row, 7), new Rook(color));
            }

            IEnumerable<PlacedPiece> makePawns(int row, Color color) =>
                Enumerable.Range(0, 8).Select(
                    i => new PlacedPiece(new Position(row, i), new Pawn(color))
                );

            IImmutableDictionary<Position, ChessPiece> makePieces(int pawnRow, int baseRow, Color color)
            {
                var pawns = makePawns(pawnRow, color);
                var baseLine = makeBaseLine(baseRow, color);
                var pieces = baseLine.Union(pawns);
                var empty = ImmutableSortedDictionary.Create<Position, ChessPiece>(PositionComparer.DefaultComparer);
                return pieces.Aggregate(empty, (s, p) => s.Add(p.Position, p.Piece));
            }

            var whitePlayer = new Player(Color.White);
            var whitePieces = makePieces(1, 0, Color.White);
            var blackPlayer = new Player(Color.Black);
            var blackPieces = makePieces(6, 7, Color.Black);
            var board = new Board(whitePieces.AddRange(blackPieces));

            return new ChessGame(board, whitePlayer, blackPlayer);
        }

        //PRO250 chess960 stuff


        //variables for use in unit testing. they are public, which is not good standards, but it is what it is.
        public int[] TakenSpots = null;
        public int kingInt = -1;
        public int rook1Int = -1;
        public int rook2Int = -1;
        public int bishopOdd = -1;
        public int bishopEven = -1;
        public int queenInt = -1;
        public int knight1Int = -1;
        public int knight2Int = -1;



        /// <summary>
        /// Creates the placements for all the pieces.
        /// Used in CreateGame960() to create a chess960 game.
        /// returns an int[].
        /// spots taken does not refer to which piece is occupying which column, only which column is occupied.
        /// </summary>
        /// <returns>The spots that are taken in row form</returns>
        public int[] nineSixty_initializePlacements()
        {
            int[] takenSpots = new int[8];
            for (int i = 0; i < 8; i++)
            {
                takenSpots[i] = -1;
            }
            kingInt = new Random().Next(1, 7);
            takenSpots[kingInt] = 1;

            rook1Int = new Random().Next(0, kingInt);
            takenSpots[rook1Int] = 1;

            rook2Int = new Random().Next(kingInt + 1, 8);
            takenSpots[rook2Int] = 1;

            bishopOdd = new Random().Next(0, 8);
            bishopEven = new Random().Next(0, 8);
            while (bishopEven % 2 != 0 || takenSpots[bishopEven] != -1)
            {
                bishopEven = new Random().Next(0, 8);
            }
            takenSpots[bishopEven] = 1;
            while (bishopOdd % 2 == 0 || takenSpots[bishopOdd] != -1)
            {
                bishopOdd = new Random().Next(0, 8);
            }
            takenSpots[bishopOdd] = 1;

            queenInt = new Random().Next(0, 8);
            while (takenSpots[queenInt] != -1)
            {
                queenInt = new Random().Next(0, 8);
            }
            takenSpots[queenInt] = 1;

            knight1Int = new Random().Next(0, 8);
            while (takenSpots[knight1Int] != -1)
            {
                knight1Int = new Random().Next(0, 8);
            }
            takenSpots[knight1Int] = 1;

            knight2Int = new Random().Next(0, 8);
            while (takenSpots[knight2Int] != -1)
            {
                knight2Int = new Random().Next(0, 8);
            }
            takenSpots[knight2Int] = 1;

            TakenSpots = takenSpots;

            return TakenSpots;
        }

        public ChessGame CreateGame960()
        {
            nineSixty_initializePlacements();

            IEnumerable<PlacedPiece> makeBaseLine(int row, Color color)
            {
                yield return new PlacedPiece(new Position(row, kingInt), new King(color));
                yield return new PlacedPiece(new Position(row, rook1Int), new Rook(color));
                yield return new PlacedPiece(new Position(row, rook2Int), new Rook(color));
                yield return new PlacedPiece(new Position(row, bishopEven), new Bishop(color));
                yield return new PlacedPiece(new Position(row, bishopOdd), new Bishop(color));
                yield return new PlacedPiece(new Position(row, queenInt), new Queen(color));
                yield return new PlacedPiece(new Position(row, knight1Int), new Knight(color));
                yield return new PlacedPiece(new Position(row, knight2Int), new Knight(color));
            }

            IEnumerable<PlacedPiece> makePawns(int row, Color color) =>
                Enumerable.Range(0, 8).Select(
                    i => new PlacedPiece(new Position(row, i), new Pawn(color))
                );

            IImmutableDictionary<Position, ChessPiece> makePieces(int pawnRow, int baseRow, Color color)
            {
                var pawns = makePawns(pawnRow, color);
                var baseLine = makeBaseLine(baseRow, color);
                var pieces = baseLine.Union(pawns);
                var empty = ImmutableSortedDictionary.Create<Position, ChessPiece>(PositionComparer.DefaultComparer);
                return pieces.Aggregate(empty, (s, p) => s.Add(p.Position, p.Piece));
            }

            var whitePlayer = new Player(Color.White);
            var whitePieces = makePieces(1, 0, Color.White);
            var blackPlayer = new Player(Color.Black);
            var blackPieces = makePieces(6, 7, Color.Black);
            var board = new Board(whitePieces.AddRange(blackPieces));

            return new ChessGame(board, whitePlayer, blackPlayer);
        }

        //

        /// <summary>
        /// Gets the status of a chess game, according to the standard rulebook.
        /// </summary>
        /// <param name="game">The game state to be analyzed.</param>
        /// <returns>The current status of the game.</returns>
        public Status GetStatus(ChessGame game)
        {
            return this.endRule.GetStatus(game);
        }

        /// <summary>
        /// Gets all possible updates (i.e., future game states) for a chess piece on a specified position,
        /// according to the standard rulebook.
        /// </summary>
        /// <param name="game">The current game state.</param>
        /// <param name="position">The position to be analyzed.</param>
        /// <returns>A sequence of all possible updates for a chess piece on the specified position.</returns>
        public IEnumerable<Update> GetUpdates(ChessGame game, Position position)
        {
            var piece = game.Board.GetPiece(position, game.ActivePlayer.Color);
            var updates = piece.Map(
                p =>
                {
                    var moves = this.movementRule.GetCommands(game, p);
                    var turnEnds = moves.Select(c => new SequenceCommand(c, EndTurnCommand.Instance));
                    var records = turnEnds.Select
                    (
                        c => new SequenceCommand(c, new SetLastUpdateCommand(new Update(game, c)))
                    );
                    var futures = records.Select(c => c.Execute(game).Map(g => new Update(g, c)));
                    return futures.FilterMaybes().Where
                    (
                        e => !this.checkRule.Check(e.Game, e.Game.PassivePlayer)
                    );
                }
            );

            return updates.GetOrElse(Enumerable.Empty<Update>());
        }
    }
}