using ChessChallenge.API;
using System;
using static System.Math;

//Trojan Fischer
public class MyBot : IChessBot {
    public int maxTime, searchDepth, lastScore;
    
    Move rootBestMove, bestMove;
    Board board;
    Timer timer;
    
    public Move Think(Board boardB, Timer timerT) {
        board = boardB;
        timer = timerT;
        maxTime = timer.MillisecondsRemaining/4;
        searchDepth = 1;
        do
            try
            {
                if (Abs(lastScore - Negamax(lastScore-25, lastScore+25, searchDepth)) >= 25) //Window=25, tune if needed
                    Negamax(-32000, 32000, searchDepth);
                rootBestMove = bestMove;
            }
            catch
            {   //Time ran out
                break;
            }
        while (
            searchDepth++ < 50 && (timer.MillisecondsElapsedThisTurn < maxTime/5)//TEMP need to tune during testing
        );
        return rootBestMove;
    }

    public int Negamax(int alpha, int beta, int depth) {
        if (depth == 0)
            return Eval(board);
        
        if (timer.MillisecondsElapsedThisTurn >= maxTime && searchDepth > 1)
            throw new Exception();

        var legalMoves = board.GetLegalMoves();
        Array.Sort(legalMoves, (a, b) => MoveScore(b).CompareTo(MoveScore(a))); //Order moves by heuristic

        int score = -50000;
        foreach (Move move in legalMoves) {
            board.MakeMove(move);
            int eval = -Negamax(-beta, -alpha, depth - 1);
            board.UndoMove(move);

            if (eval > score) {
                score = eval;
                if (depth == searchDepth)
                    bestMove = move;
            }
            alpha = Max(alpha, eval);
            if (alpha >= beta)
                break; //Beta cutoff
        }
        return lastScore = score;
    }

    int MoveScore(Move move) {//TEMP move score heuristic for testing
        if (move.IsPromotion)
        return 200000 + (int)move.PromotionPieceType;

        if (move.IsCapture) 
        return 100000 + (int)move.CapturePieceType * 100 - (int)move.MovePieceType;
        return 0;
    }

    int Eval (Board board) { //TEMP material eval for testing, replace later with PST
        int[] PieceValue = [0,100,300,300,500,900];
        int score = 0;

        for (int i = 1; i <= 5; i++) { //Pawn to Queen, King eval unnecessary
            var white = board.GetPieceList((PieceType)i, true);
            var black = board.GetPieceList((PieceType)i, false);
            score += (white.Count - black.Count) * PieceValue[i];
        }
        return board.IsWhiteToMove ? score : -score;
    }
}