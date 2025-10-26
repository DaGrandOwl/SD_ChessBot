using ChessChallenge.API;
using System;

public class MyBot : IChessBot {
    public int maxTime, searchDepth;
    Move rootBestMove;
    
    public Move Think(Board board, Timer timer) {
        do
            try
            { //Iterative deepening call
            }
            catch
            {
                //Time ran out
                break;
            }
        while (
            searchDepth++ < 100 && (timer.MillisecondsElapsedThisTurn < maxTime/5) //TEMP need to tune during testing
            );
            return rootBestMove;
            }

    int Eval (Board board) { //TEMP material eval for testing, replace later with PST
        int[] PieceValue = [0,100,300,300,500,900];
        int score = 0;

        for (PieceType pt = 1; pt <= 5; pt++) { //Pawn to Queen, King eval unnecessary
            var white = board.GetPieceList(pt, true);
            var black = board.GetPieceList(pt, false);
            score += (white.Count - black.Count) * PieceValue[pt];
        }
        return board.IsWhiteToMove ? score : -score;
    }
}