using System;
using System.Collections.Generic;

namespace Engine
{
    public class AnnotatedTurn
    {
        public Turn Turn;
        public string Annotation;

        public List<Move> Arrows;

        public List<CoordFive> Highlights;

        public AnnotatedTurn(Turn turn, string annotation)
        {
            Turn = turn;
            Annotation = annotation;
        }

        public override string ToString()
        {
            return $"{Turn} ({Annotation})";
        }
    }
}