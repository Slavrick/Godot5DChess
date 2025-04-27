using FileIO5D;
using System;
using System.Collections.Generic;

namespace FiveDChess
{
	public class AnnotationTree
	{

		public Node Root;
		public static int GlobalIDCTR = 0;

		public AnnotationTree(Turn rootData)
		{
			Root = new Node(rootData, null);
		}

		public class Node
		{
			public AnnotatedTurn AT;
			public int NodeID;
			public Node Parent;
			public List<Node> Children;

			public Node( Turn turn, Node parent)
			{
				this.Parent = parent;
				this.NodeID = GlobalIDCTR++;
				this.Children = new List<Node>();
				AT = new AnnotatedTurn( turn );
			}

			public Node (AnnotatedTurn at, Node parent)
			{
				this.Parent = parent;
				this.NodeID = GlobalIDCTR++;
				this.Children = new List<Node>();
				AT = at;
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

			public override string ToString()
			{
				return ToString_(0,0);
			}
			
			public string ToString_(int depth, int child)
			{
				string returnString = $"{depth} , {child} , " + this.AT.T.ToString();
				for (int i = 0; i < depth; i++)
				{
					returnString = "-" + returnString;
				}
				returnString += '\n';
				int id = depth;
				child = 0;
				foreach (Node node in Children)
				{
					returnString += node.ToString_(depth + 1, child++);
				}
				return returnString;
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

		/// <summary>
		/// Contains, but only goes to depth 1
		/// </summary>
		/// <param name="tree"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		public static bool ContainsChild(Node tree, Turn target)
		{
			if (tree.AT.T.Equals(target)) return true;
			foreach (Node node in tree.Children)
			{
				if (node.AT.T.Equals(target)) return true;
			}
			return false;
		}

		public static Node FindNode(Node n, Turn target)
		{
			if (n.AT.T.Equals(target))
			{
				return n;
			}
			if (target.TurnNum <= n.AT.T.TurnNum)
			{
				return null;
			}
			foreach (var child in n.Children)
			{
				Node find = FindNode(child, target);
				if (find != null)
				{
					return find;
				}
			}
			return null;
		}

		public static List<int> NavPath(Node tree, int target)
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

		public static List<AnnotatedTurn> GetPastTurns(Node n)
		{
			List<AnnotatedTurn> pastTurns = new List<AnnotatedTurn>();
			Node index = n;
			while(index != null)
			{
				pastTurns.Add(index.AT);
				index = index.Parent;
			}
			pastTurns.Reverse();
			return pastTurns;
		}

		public static List<string> GetLabels(Node tree, int nesting = 0)
		{
			List<string> labels = new List<string>();
			string label = new string(' ', nesting * 4);
			labels.Add(label + tree.AT.T.ToString());
			if (tree.Children.Count > 0)
			{
				for (int i = 1; i < tree.Children.Count; i++)
				{
					labels.AddRange(GetLabels(tree.Children[i], nesting + 1));
				}
				labels.AddRange(GetLabels(tree.Children[0], nesting));
			}
			return labels;
		}
		
		public static List<Node> GetNodesLinear(Node tree)
		{
			List<Node> nodes = new List<Node> { tree };
			if (tree.Children.Count > 0)
			{
				for (int i = 1; i < tree.Children.Count; i++)
				{
					nodes.AddRange(GetNodesLinear(tree.Children[i]));
				}
				nodes.AddRange(GetNodesLinear(tree.Children[0]));
			}
			return nodes;
		}

		//findnode -> node
		//getnodes/etc linear
	}
}
