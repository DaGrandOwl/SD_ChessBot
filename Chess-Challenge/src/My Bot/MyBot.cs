using ChessChallenge.API;
using System;
using System.Linq;
using static System.MathF;

//Trojan Fischer
public class MyBot : IChessBot {
    public float lastScore;
    public int searchDepth;
    Move rootBestMove, searchBestMove;
    Board board;
    Timer timer;

    readonly byte[] maxTime = new byte[24];
    readonly byte[,] MVVLVA = new byte[7, 8];
    readonly byte[] phaseC = new byte[16];
    readonly short[] pieceMaterialOpening = new short[12];
    readonly short[] pieceMaterialEnding = new short[12];
    readonly sbyte[] PSTOpening = new sbyte[384];
    readonly sbyte[] PSTEnding = new sbyte[384];
    readonly byte[] PSTScale = new byte[16];

    public Move Think(Board boardB, Timer timerT) {
        board = boardB;
        timer = timerT;
        searchDepth = 1;
        lastScore = 0f;

        void parse(Array dst, params ulong[] data)
        {
            Buffer.BlockCopy(data, 0, dst, 0, data.Length * 8);
        }

        //Data packed to minimize token use
        parse(maxTime, 0x1011110F0D0A0601, 0x304040406080B0E, 0x101010101010203);
        parse(MVVLVA, 0xFFC8C8C8C8FF00, 0x22504C4C4A3A00, 0x18564839381C00, 0x16544636351A00, 0x125234201E1400, 0xA3214100E0C00);
        parse(pieceMaterialOpening, 0x190012901210064, 0xFEDFFF9C000003CF, 0xFC31FE70FED7);
        parse(pieceMaterialEnding, 0x1D1012C01220064, 0xFEDEFF9C000003A2, 0xFC5EFE2FFED4);
        parse(PSTOpening, 0x345FA22D468346F4, 0xD4ECECC6A2A9E800, 0xEFFFF8F3CFC8F819, 0xEA04FE0CEAC40E0E, 
            0xFA2727380BE2251F, 0x116D7F735328503D, 0x4D7F7F7F74537F7F, 0xF7868D1878C97E3C, 
            0xD0BDE0D3D3D9ADB7, 0xD9EDFFE5D9E4E6D5, 0xDCFADC080FE0EBC4, 0xDF1E1309ECF7FCDF, 
            0x17033D082D26F7F4, 0x173B6E63503D2D15, 0xFAD31E3F5424F3DB, 0xAC14F6443416F9B4, 
            0xEBFFE3EFEFD103E9, 0x5F609E7EA12EB06, 0xFC14FBEAF30413E9, 0xF001F00B06F802D9, 
            0x3E80D061608F3F7, 0xFA111F12190F06EC, 0xA3E6F6070301EBE1, 0xF1C2FEFAFEE7DEF2, 
            0x10D310005070603, 0xFE313E1913101113, 0xFE31401710101011, 0xFF2D32181012100F, 
            0xD2C331911151411, 0x2B48482A2E282821, 0x4C695764645A553C, 0x7F7F7F7F7F7766, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0x562F280DE0DB1243, 0x3B342F12FFFA2946, 0x292322080414143D, 0x1E1915140C191722, 
            0x3214211B261C1D22, 0x25FA1D1F3C3C1B1C, 0x150F1B275268540C, 0x25F22420303300);
        parse(PSTEnding, 0xB02DA6BB181246E0, 0xDCE3BACE9EA7C5C9, 0xDBE6BFC592A2CEC8, 0xE3ECC5C8A5BCD4DD, 
            0xFE07DAE2C6DEF500, 0x35320A22051C3441, 0x7B7F717F60717F7F, 0xC432AB209CE26889, 
            0xE9F6F90503F6EEE3, 0xEEF60E100813F5EA, 0x2070C1D1D07FF01, 0x111616231912F6, 
            0xEE0F0D1B161009EF, 0xEAF1F9020B06F3EB, 0xE1F0ECF5FDF2F3E6, 0xE6DBF3E8EFF0E6DE, 
            0xEBF9F6FEF9F0ECEA, 0xF5F4FFFD00F7F7EE, 0xF3FD01060007F6FB, 0xF9FA030310020102, 
            0xEFFFFA0B020301F4, 0xEDF4FDF8FDFAFDEA, 0xE9F8F5F7F4F9F4EE, 0xF5E9F1EDF4EFF1EE, 
            0x1004100607080E12, 0x2FC0D0C0E0E0D10, 0x3F70809090B0913, 0xFFFA02090C080D11, 
            0xFF07050B101112, 0xF9FF060D090D15, 0xFDFBFAFC050F0F14, 0xF3F9F7FA010210, 
            0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 0x0, 
            0xCDD2EAF0F7F7E4DC, 0xDEEEFF060D0700EA, 0xEF010F1414160BF9, 0xFC10131516181906, 
            0x41A23202326230C, 0x112628202329060D, 0x9251C1814182F0D, 0xE1F14130F0F1A12);
        parse(PSTScale, 0x5A2F5F0058495A2F, 0x5F005849 );
        parse(phaseC, 0x100000502010100, 0x50201 );

        
        do
            try
            {   
                searchBestMove = board.GetLegalMoves().FirstOrDefault();
                if (Abs(lastScore - Negamax(lastScore - 0.01f, lastScore + 0.01f, searchDepth)) >= 0.01f)
                    Negamax(-1f, 1f, searchDepth);
                rootBestMove = searchBestMove;
            }
            catch
            {   //Time ran out
                break;
            }
        while (
            searchDepth++ < 50 &&
            (timer.MillisecondsElapsedThisTurn * 3 < Min(timer.GameStartTimeMilliseconds / 480.0f * maxTime[(int)Min(board.PlyCount >> 3, 23)], timer.MillisecondsRemaining / 12))
        );
        return rootBestMove;
    }
    
    float Quiescence(float alpha, float beta)
    {
        int side = board.IsWhiteToMove ? 1 : -1;
        var score = Eval(side) * side;
        if (board.IsInCheckmate())
            return board.IsWhiteToMove ? -1 : 1;
        if (score >= beta) 
            return beta;
        alpha = Max(alpha, score);

        var moves = board.GetLegalMoves(true);
        Array.Sort(moves.Select(_ => (int)_.MovePieceType - (int)_.CapturePieceType).ToArray(), moves);
        foreach (var move in moves)
        {
            board.MakeMove(move);
            try {
                 score = -Quiescence(-beta, -alpha);
                }
            finally {
                board.UndoMove(move);
                }
            if (score >= beta) return beta;
            alpha = Max(alpha, score);
        }

        return alpha;
    }

    public float Negamax(float alpha, float beta, int depth) {
        int side = board.IsWhiteToMove ? 1 : -1;
        if (board.IsRepeatedPosition()) 
            return 0;

        if (depth == 0)
            return Quiescence(alpha, beta);
        
        if (board.IsInCheckmate())
            return board.IsWhiteToMove ? -1 : 1;

        if (timer.MillisecondsElapsedThisTurn >= timer.MillisecondsRemaining/8 && searchDepth > 2)
            throw new Exception();
        
        var K = new Move[3, 64];
        var legalMoves = board.GetLegalMoves();
        Array.Sort(legalMoves.Select(_ =>_.IsPromotion ? 0 : _ == K[side + 1, depth] ? 100 : MVVLVA[(int)_.CapturePieceType, (int)_.MovePieceType]).ToArray(), legalMoves);
        float highestScore = -2f;
        foreach (Move move in legalMoves) {
            board.MakeMove(move);
            float score;
            try {
                score = -Negamax(-beta, -alpha, depth - 1);
                }
            finally {
                board.UndoMove(move);
                }

            if (score > highestScore) {
                highestScore = score;
                if (depth == searchDepth)
                    searchBestMove = move;
            }
            alpha = Max(alpha, score);
            if (alpha >= beta)
                break; //Beta cutoff
        }
        return lastScore = highestScore;
    }

    public float Eval(float init) {
        if (board.IsInCheckmate()) return board.IsWhiteToMove ? -1 : 1;

        var (phase, evalO, evalE, i) = (0, init, init, 0);

        foreach (var pieceList in board.GetAllPieceLists())
        {
            var (count, psqt_scale) = (pieceList.Count, PSTScale[i] / 128.0f);
            phase += phaseC[i] * count;
            evalO += pieceMaterialOpening[i] * count;
            evalE += pieceMaterialEnding[i] * count;
            for (int j = 0; j < count; j++)
            {
                var (index, square) = (i * 64, pieceList[j].Square.Index);
                if (i < 6)
                {
                    index += square;
                    evalO += PSTOpening[index] * psqt_scale;
                    evalE += PSTEnding[index] * psqt_scale;
                }
                else
                {
                    index += (square ^ 56) - 384;
                    evalO -= PSTOpening[index] * psqt_scale;
                    evalE -= PSTEnding[index] * psqt_scale;
                }
            }
            i++;
        }

        if (Abs(evalO) > 200 && board.IsDraw()) return 0;
        return Tanh((evalO * phase + evalE * (24 - phase)) / 24000.0f); //Values from -1 to 1
    }

}