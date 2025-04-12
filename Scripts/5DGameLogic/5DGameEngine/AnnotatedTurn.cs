using System;
using System.Collections.Generic;

namespace FiveDChess
{
    public class AnnotatedTurn
    {
        public Turn T;
        public string Annotation;
        public List<Move> Arrows;
        public List<int> ArrowColors;
        public List<CoordFive> Highlights;
        public List<int> HighlightColors;

        public enum AnnotationColors
        {
            RED,BLUE,GREEN,YELLOW,ORANGE
        }

        public AnnotatedTurn(Turn turn, string annotation, List<Move> arrows, List<CoordFive> highlights)
        {
            T = turn;
            Annotation = annotation;
            Arrows = arrows;
            Highlights = highlights;
        }

        public AnnotatedTurn(Turn t) 
        {
            T = t;
            Annotation = "";
            Arrows = new List<Move>();
            Highlights = new List<CoordFive>();
        }

        public override string ToString()
        {
            return $"{T} ({Annotation})";
        }
    }
}