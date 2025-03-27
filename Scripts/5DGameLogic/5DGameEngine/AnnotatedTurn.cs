namespace Engine
{
    public class AnnotatedTurn
    {
        public Turn Turn { get; set; }
        public string Annotation { get; set; }

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