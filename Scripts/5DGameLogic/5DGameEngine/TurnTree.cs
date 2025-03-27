using System;
using System.Collections.Generic;

namespace Engine
{
	public class TurnTree
	{
		public Node Root { get; set; }
		public static int GlobalIDCTR = 0;

		public TurnTree(Turn rootData)
		{
			Root = new Node(rootData, null);
		}

		public class Node
		{
			public Node(Turn data, Node parent)
			{
				this.Data = data;
				this.Children = new List<Node>();
				this.Parent = parent;
				this.NodeID = GlobalIDCTR++;
			}

			public Node AddChild(Turn data)
			{
				Node newNode = new Node(data, this);
				Children.Add(newNode);
				return newNode;
			}

			public Turn Data { get; set; }
			public Node Parent { get; private set; }
			public List<Node> Children { get; set; }
			public int NodeID { get; private set; }

			public override string ToString()
			{
				return NodeID + ", " + Data.ToString();
			}
		}

		// All of these functions are depth first searches.
		public bool Contains(Turn target)
		{
			if (Root.Data.Equals(target))
			{
				return true;
			}
			foreach (var child in Root.Children)
			{
				if (Contains(child, target))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contains(Node n, Turn target)
		{
			if (n.Data.Equals(target))
			{
				return true;
			}
			if (target.TurnNum <= n.Data.TurnNum)
			{
				return false;
			}
			foreach (var child in n.Children)
			{
				if (Contains(child, target))
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(int targetNodeID)
		{
			if (Root.NodeID == targetNodeID)
			{
				return true;
			}
			foreach (var child in Root.Children)
			{
				if (Contains(child, targetNodeID))
				{
					return true;
				}
			}
			return false;
		}

		public static bool Contains(Node n, int targetNodeID)
		{
			if (n.NodeID == targetNodeID)
			{
				return true;
			}
			foreach (var child in n.Children)
			{
				if (Contains(child, targetNodeID))
				{
					return true;
				}
			}
			return false;
		}

		public Node FindNode(Turn target)
		{
			if (Root.Data.Equals(target))
			{
				return Root;
			}
			foreach (var child in Root.Children)
			{
				Node n = FindNode(child, target);
				if (n != null)
				{
					return n;
				}
			}
			return null;
		}

		public static Node FindNode(Node n, Turn target)
		{
			if (n.Data.Equals(target))
			{
				return n;
			}
			if (target.TurnNum <= n.Data.TurnNum)
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

		public static List<int> NavPath(Node n, int targetNodeID)
		{
			if (n.NodeID == targetNodeID)
			{
				return new List<int>();
			}
			List<int> path = new List<int>();
			for (int i = 0; i < n.Children.Count; i++)
			{
				List<int> temp = NavPath(n.Children[i], targetNodeID);
				if (temp != null)
				{
					path.Add(i);
					path.AddRange(temp);
					return path;
				}
			}
			return null;
		}

		public List<string> GetLabels()
		{
			List<string> labels = new List<string> { Root.Data.ToString() };
			if (Root.Children.Count > 0)
			{
				for (int i = 1; i < Root.Children.Count; i++)
				{
					labels.AddRange(GetLabels(Root.Children[i], 1));
				}
				labels.AddRange(GetLabels(Root.Children[0], 0));
			}
			return labels;
		}

		public static List<string> GetLabels(Node currentNode, int nesting)
		{
			List<string> labels = new List<string>();
			string label = new string(' ', nesting * 4);
			labels.Add(label + currentNode.Data.ToString());
			if (currentNode.Children.Count > 0)
			{
				for (int i = 1; i < currentNode.Children.Count; i++)
				{
					labels.AddRange(GetLabels(currentNode.Children[i], nesting + 1));
				}
				labels.AddRange(GetLabels(currentNode.Children[0], nesting));
			}
			return labels;
		}

		public List<Turn> GetTurnsLinear()
		{
			List<Turn> turns = new List<Turn> { Root.Data };
			if (Root.Children.Count > 0)
			{
				for (int i = 1; i < Root.Children.Count; i++)
				{
					turns.AddRange(GetTurnsLinear(Root.Children[i]));
				}
				turns.AddRange(GetTurnsLinear(Root.Children[0]));
			}
			return turns;
		}

		public static List<Turn> GetTurnsLinear(Node currentNode)
		{
			List<Turn> turns = new List<Turn> { currentNode.Data };
			if (currentNode.Children.Count > 0)
			{
				for (int i = 1; i < currentNode.Children.Count; i++)
				{
					turns.AddRange(GetTurnsLinear(currentNode.Children[i]));
				}
				turns.AddRange(GetTurnsLinear(currentNode.Children[0]));
			}
			return turns;
		}

		public List<Node> GetNodesLinear()
		{
			List<Node> nodes = new List<Node> { Root };
			if (Root.Children.Count > 0)
			{
				for (int i = 1; i < Root.Children.Count; i++)
				{
					nodes.AddRange(GetNodesLinear(Root.Children[i]));
				}
				nodes.AddRange(GetNodesLinear(Root.Children[0]));
			}
			return nodes;
		}

		public static List<Node> GetNodesLinear(Node currentNode)
		{
			List<Node> nodes = new List<Node> { currentNode };
			if (currentNode.Children.Count > 0)
			{
				for (int i = 1; i < currentNode.Children.Count; i++)
				{
					nodes.AddRange(GetNodesLinear(currentNode.Children[i]));
				}
				nodes.AddRange(GetNodesLinear(currentNode.Children[0]));
			}
			return nodes;
		}

		// Gets path from root to node of data for the node passed
		public static List<Turn> GetNodeLinePath(Node n)
		{
			List<Turn> line = new List<Turn>();
			Node temp = n;
			while (temp.Parent != null)
			{
				line.Add(temp.Data);
				temp = temp.Parent;
			}
			return line;
		}
	}
}
