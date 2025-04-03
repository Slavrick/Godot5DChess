using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveDChess
{
    public class AnnotatedTurn
    {
        public Turn Turn;
        public string Annotation;
        public List<Move> Arrows;
        public List<CoordFive> Highlights;

        public AnnotatedTurn(Turn turn, string annotation, List<Move> arrows, List<CoordFive> highlights)
        {
            Turn = turn;
            Annotation = annotation;
            Arrows = arrows;
            Highlights = highlights;
        }

        public override string ToString()
        {
            return $"{Turn} ({Annotation})";
        }
    }
}