using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiveDChess;
using System.IO;

namespace FileIO5D
{
    class FENExporter
    {

        public static void ExportGameState(GameStateManager gsm, string filePath)
        {
            ExportString(filePath, GameStateToFEN(gsm));
        }

        public static void ExportString(string savePath, string fileContents)
        {
            File.WriteAllText(savePath, fileContents);
        }

        public static string GameStateToFEN(GameStateManager gsm)
        {
            string gameStateString = "";
            string header = GetGameStateHeader(gsm);
            string origins = "";
            foreach(Timeline t in gsm.OriginsTL)
            {
                if (t.ColorStart)
                {
                    origins += "[" + BoardToString(t.WBoards[0], t.ColorStart, t.TStart, t.Layer) + "]";
                }
                else
                {
                    origins += "[" + BoardToString(t.BBoards[0], t.ColorStart, t.TStart, t.Layer) + "]";
                }
                origins += '\n';
            }
            string moves = "";
            bool oddTurn = true;
            int turnNum = 1;
            List<AnnotatedTurn> turnList = AnnotationTree.GetPastTurns(gsm.Index);
            foreach (AnnotatedTurn at in turnList)
            {
                Turn t = at.T;
                if (oddTurn)
                {
                    moves += turnNum.ToString() + ". ";
                    turnNum++;
                    moves += StringUtils.TurnExportString(t);
                }
                else
                {
                    moves += StringUtils.TurnExportString(t);
                }
                if (oddTurn)
                {
                    moves += " / ";
                }
                else
                {
                    moves += " \n";
                }
                oddTurn = !oddTurn;
            }
            gameStateString += header + '\n' + origins + '\n' + moves;
            return gameStateString;
        }

        public static string BoardToString(Board b, bool color, int tStart, int layer)
        {
            string FEN = "";
            int count = 0;
            for (int y = 0; y < b.Height; y++)
            {
                for (int x = 0; x < b.Width; x++)
                {
                    int piece = b.GetSquareSafe(x, b.Height-y-1);
                    if(piece == 0)
                    {
                        count++;
                    }
                    else 
                    {
                        if(count > 0)
                        {
                            FEN += count.ToString();
                            count = 0;
                        }
                        if(piece < 0)
                        {
                            FEN += Board.PieceChars[piece * -1] + "*";
                        }
                        else
                        {
                            FEN += Board.PieceChars[piece];
                        }
                    }
                }
                if (count > 0)
                {
                    FEN += count.ToString();
                    count = 0;
                }
                if (y < b.Height - 1)
                {
                    FEN += "/";
                }
            }
            //TODO fix this because even starting games dont export correctly
            FEN += ":" + layer.ToString() + ":" + tStart.ToString() + ":";
            if (color)
            {
                FEN += 'w';
            }
            else
            {
                FEN += 'b';
            }
            return FEN;
        }

        public static string GetGameStateHeader(GameStateManager gsm)
        {
            string headers = "";
            headers += "[board \"custom\"]\n";
            headers += "[size \"" + gsm.Width + "x" + gsm.Height + "\"]";
            return headers;
        }
    }
}
