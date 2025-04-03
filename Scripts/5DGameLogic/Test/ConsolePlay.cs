using FiveDChess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test;
using FileIO5D;

//This class is used for debugging.
namespace TestRewrite
{
    class ConsolePlay
    {
        GameState g;
        public ConsolePlay(string filepath)
        {
            g = FENParser.ShadSTDGSM(filepath);//("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\Standard.PGN5.txt");
            Console.WriteLine(g.ToString());
        }

        public void MakeMove()
        {
            string input = Console.ReadLine();
            Turn t = FENParser.StringToTurn(g,input, false);
            g.MakeTurn(t.Moves);
            g.CalcPresent();
            g.IsMated();
            Console.WriteLine(g.ToString());
        }
    }
}
