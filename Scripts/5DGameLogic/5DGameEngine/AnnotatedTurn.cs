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

        public AnnotatedTurn(Turn turn, string annotation, List<Move> arrows, List<CoordFive> highlights, List<int> arrowColors, List<int> highlightColors)
        {
            T = turn;
            Annotation = annotation;
            Arrows = arrows;
            ArrowColors = arrowColors;
            Highlights = highlights;
            HighlightColors = highlightColors;
        }

        public AnnotatedTurn(Turn t) 
        {
            T = t;
            Annotation = "";
            Arrows = new List<Move>();
            ArrowColors = new List<int>();
            Highlights = new List<CoordFive>();
            HighlightColors = new List<int>();
        }

        public override string ToString()
        {
            return $"{T} ({Annotation})";
        }
    }
}