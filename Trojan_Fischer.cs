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
  {//Change string to test custom positions. Has to be 64 characters
    string startPos = "RNBQKBNRPPPPPPPP................................pppppppprnbqkbnr";
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

  static void PrintBoard(){
  for(int r=7;r>=0;r--){
    for(int f=0;f<8;f++){
      sbyte p=Board[SqIndex(f,r)];
      Console.Write(p==0?'.':p>0?"PNBRQK"[p-1]:"pnbrqk"[-p-1]);
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
          if (cmd.Length == 4) //If true playing as black. Else, default is playing as white
          {
            //TEMP
          }
          //Call to update board position
          WriteLine("position set");
          break;
        case "go":
          WriteLine("bestmove e2e4"); //TEMP
          break;
        case "quit":
          return;
        case "test"://TEMP 
          BoardInit();
          PrintBoard();
          break;
      }
    }
  }
}