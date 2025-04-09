using System;
using System.Collections.Generic;

namespace FiveDChess
{
    class AnnotationTree
    {

        public Node Root;
        public static int GlobalIDCTR = 0;


        public class Node
        {
            public AnnotatedTurn AT;
            public int NodeID;
            public Node Parent;
            public List<Node> Children;

            public Node( Turn turn, Node parent)
            {
                this.Children = new List<Node>();
                AT = new AnnotatedTurn( turn );
                this.NodeID = GlobalIDCTR++;
            }

            public Node (AnnotatedTurn at, Node parent)
            {
                AT = at;
                this.NodeID = GlobalIDCTR++;
                this.Parent = parent;
            }

            public Node AddChild( Turn t )
            {
                Node newchild = new Node( t,this);
                Children.Add( newchild );
                return newchild;
            }

            public Node AddChild(AnnotatedTurn at)
            {
                Node newchild = new Node(at,this);
                Children.Add(newchild);
                return newchild;
            }
        }
        
        public static bool Contains(Node tree, int target)
        {
            if(tree.NodeID == target) return true;
            foreach(Node node in tree.Children)
            {
                if(AnnotationTree.Contains(node, target)) return true;
            }
            return false;
        }

        public static bool Contains(Node tree, Turn target)
        {
            if(tree.AT.T.Equals(target)) return true;
            foreach (Node node in tree.Children)
            {
                if(node.AT.T.Equals(target)) return true;
            }
            return false;
        }

        public static bool ContainsChild(Node tree, Turn target)
        {
            if (tree.AT.T.Equals(target)) return true;
            foreach (Node node in tree.Children)
            {
                if (node.AT.T.Equals(target)) return true;
            }
            return false;
        }

        public List<int> NavPath(Node tree, int target)
        {
            if(tree.NodeID == target) return new List<int>();
            for(int i = 0; i < tree.Children.Count;i++)
            {
                List<int> path = NavPath(tree.Children[i],target);
                if(path != null)
                {
                    path.Insert(0,i);
                    return path;
                }    
            }
            return null;
        }

        //findnode -> node
        //getnodes/labels/etc linear

    }
}
