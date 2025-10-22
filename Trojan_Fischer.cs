using System;
using System.Diagnostics;
using static System.Console;

namespace Trojan_Fischer;
class Program
{
  //---Board Representation(Standard 0x88 board)---
  const int P= 1, N=2, B=3, R=4, Q=5, K=6;
  static sbyte[] Board = new sbyte[128];
  static int SqIndex(int file, int rank) => (rank << 4) | file; //Square Indexing
  static void BoardInit() //Initialize Board as regular starting position
  {
    string startPos = "RNBQKBNRPPPPPPPP................................pppppppprnbqkbnr"; //Change this str to test custom position, keep 64 chars
    for (int i = 0; i < 64; i++)
    {
      char tile = startPos[i];
      int v = tile switch
      {  //White
        'P' => P,
        'N' => N,
        'B' => B,
        'R' => R,
        'Q' => Q,
        'K' => K,
        //Black
        'p' => -P,
        'n' => -N,
        'b' => -B,
        'r' => -R,
        'q' => -Q,
        'k' => -K,
        _ => 0
      };
      Board[SqIndex(i & 7, 7 - (i >> 3))] = (sbyte) v;
    }
  }

  //---Move Making---
  static void MakeMove(int from, int to)
  {
    Board[to] = Board[from];
    Board[from] = 0;
  }

  static void Promote(int from, int to, int promo)
  {
    Board[to] = (sbyte) (Board[from] > 0 ? promo : -promo); //TEMP manually specifically for colour
    Board[from] = 0;
  }

  //---Formatting UCI Moves---
  //Convert LAN to board positions
  static void MoveFromUCIstr(string uciMove, out int from, out int to, out int promo)
  {
      from = SqIndex(uciMove[0] - 'a', uciMove[1] - '1'); //Subtracts ASCII value of 'a'or'1' to get 0-7 range
      to = SqIndex(uciMove[2] - 'a', uciMove[3] - '1');
      promo = uciMove.Length == 5 ? uciMove[4] switch
      {
          'q' => Q,
          'r' => R,
          'b' => B,
          'n' => N,
          _ => 0
      } : 0;
  }

  //TEMP for TESTing
  static void PrintBoard()
  {
    for (int r = 7; r >= 0; r--)
    {
      for (int f = 0; f < 8; f++)
      {
        sbyte p = Board[SqIndex(f, r)];
        Console.Write(p == 0 ? '.' : p > 0 ? "PNBRQK"[p - 1] : "pnbrqk"[-p - 1]);
      }
      Console.WriteLine();
    }
  }

  //---UCI Protocol---
  static void Main()
  {
    while (true)
    {
      string[] cmd = ReadLine().Split();
      switch (cmd[0])
      {
        case "uci":
          WriteLine("id name Trojan Fischer");
          WriteLine("uciok");
          break;
        case "isready":
          WriteLine("readyok");
          break;
        case "position":
          BoardInit();
          if (cmd.Length > 2) // 1 move has been played
          {
            for (int i = 3; i < cmd.Length; i++) //First move is 4th word
            {
              MoveFromUCIstr(cmd[i], out int from, out int to, out int promo);
              if (promo > 0) Promote(from, to, promo);
              else MakeMove(from, to);
            }
          }
          WriteLine("position set");
          break;
        case "go":
          WriteLine("bestmove e2e4"); //TEMP
          break;
        case "quit":
          return;
        case "test"://TEMP 
          PrintBoard();
          break;
      }
    }
  }
}