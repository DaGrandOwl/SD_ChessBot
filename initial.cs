using System.Diagnostics;

namespace initial;
class Program
{
  static void Main()
  {
    while (true)
    {
    string cmd = Console.ReadLine().Split()[0];
      switch (cmd)
      {
        case "uci":
          Console.WriteLine("id name uhhhhhh"); //TEMP
          Console.WriteLine("uciok");
          break;
        case "isready":
          Console.WriteLine("readyok");
          break;
        case "go": 
          Console.WriteLine("bestmove e2e4"); //TEMP
          break;
        case "quit":
          return;
        default:
          Debugger.Launch();
          break;
      }
    }
  }
}