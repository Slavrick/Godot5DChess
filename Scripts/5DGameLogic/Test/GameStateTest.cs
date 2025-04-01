using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FileIO5D;
using FiveDChess;

namespace Test
{
    internal class GameStateTest
    {

        public static void TestGameStateMutation()
        {
            Console.Write("     Testing GameState Mutation: ");
            GameState g5 = FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\MateTest\\tesseractMageOChicken.5dpgn");
            GameState g1 = FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\Puzzles\\RookTactics4.PGN5.txt");
            GameState g2 = FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\Puzzles\\RookTactics4.PGN5.txt");
            GameState g3 = FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\MateTest\\test1.txt");
            GameState g4 = FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\MateTest\\test1.txt");
            GameState g6 = FENParser.ShadSTDGSM("C:\\Users\\mavmi\\Documents\\5DRewrite\\5DChess\\5DChess\\PGN\\MateTest\\tesseractMageOChicken.5dpgn");
            if (!TestGameStateEquality(g1, g1)) throw new Exception("GameSate Test error");
            if (TestGameStateEquality(g1, g3)) throw new Exception("GameSate Test error");
            if (!TestGameStateEquality(g1, g2)) throw new Exception("Parser inconsistency error");
            g2.IsMated();
            g4.IsMated();
            g6.IsMated();
            if (!TestGameStateEquality(g1, g2)) throw new Exception("Is Mated Mutates GameState");
            if (!TestGameStateEquality(g3, g4)) throw new Exception("Is Mated Mutates GameState");
            if (!TestGameStateEquality(g5, g6)) throw new Exception("Is Mated Mutates GameState");
            g2.GetCurrentThreats();
            g4.GetCurrentThreats();
            g6.GetCurrentThreats();
            if (!TestGameStateEquality(g1, g2)) throw new Exception("Get Current Threats Mutates GameState");
            if (!TestGameStateEquality(g3, g4)) throw new Exception("Get Current Threats Mutates GameState");
            if (!TestGameStateEquality(g5, g6)) throw new Exception("Get Current Threats Mutates GameState");
            Console.WriteLine("Passed!");
        }

        public static bool TestGameStateEquality( GameState g1, GameState g2 )
        {
            //TODO Test other things in here.
            if(g1.MinTL != g2.MinTL || g1.MaxTL != g2.MaxTL) 
            {
                return false;
            }
            for( int i = g1.MinTL; i <= g1.MaxTL; i++)
            {
                if(!TestTimelineEquality(g1.GetTimeline(i),g2.GetTimeline(i)))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool TestTimelineEquality(Timeline t1, Timeline t2)
        {
            if(t1.Layer != t2.Layer)
            {
                return false;
            }
            if(t1.TStart != t2.TStart)
            {
                return false;
            }
            if(t1.TEnd != t2.TEnd)
            {
                return false;
            }
            if( t1.WhiteEnd != t2.WhiteEnd)
            {
                return false;
            }
            if(t1.WhiteStart != t2.WhiteStart)
            {
                return false;
            }
            if(t1.BlackStart != t2.BlackStart)
            {
                return false;
            }
            if(t1.BlackEnd != t2.BlackEnd)
            {
                return false;
            }
            for(int i = 0; i < t1.WBoards.Count;i++)
            {
                if (!TestBoardEqual(t1.WBoards[i], t2.WBoards[i]))
                {
                    return false;
                }
            }
            for (int i = 0; i < t1.BBoards.Count; i++)
            {
                if (!TestBoardEqual(t1.BBoards[i], t2.BBoards[i])) { return false; }
            }
            return true;
        }

        public static bool TestBoardEqual(Board b1, Board b2)
        {
            if(b1.Width != b2.Width || b1.Height != b2.Height)
            {
                return false;
            }
            if (b1.EnPassentSquare == null && b2.EnPassentSquare != null) { return false; }
            if (b1.EnPassentSquare != null && b2.EnPassentSquare == null) { return false; }
            if (b1.EnPassentSquare != null && b2.EnPassentSquare != null)
            {
                if(!b1.EnPassentSquare.SpatialEquals(b2.EnPassentSquare))
                {
                    return false;
                }
            }
            for(int rank = 0; rank < b1.Height; rank++)
            {
                for(int file = 0; file < b1.Width; file++)
                {
                    if(b1.GetSquare(file,rank) != b2.GetSquare(file, rank))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    }
}
