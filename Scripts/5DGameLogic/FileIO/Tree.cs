using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FileIO5D
{
    public class TreeNode<T>
    {
        public T value;
        public TreeNode<T> Parent;
        public List<TreeNode<T>> Children = new List<TreeNode<T>>();

        public TreeNode(T value)
        {
            Parent = null;
            this.value = value;
        }

        public TreeNode<T> AddChild(T value)
        {
            var node = new TreeNode<T>(value) { Parent = this };
            Children.Add(node);
            return node;
        }

        public TreeNode<T>[] AddChildren(params T[] values)
        {
            return values.Select(AddChild).ToArray();
        }

        public bool RemoveChild(TreeNode<T> node)
        {
            return Children.Remove(node);
        }

        public void RemoveChild(int childIndex)
        {
            Children.RemoveAt(childIndex);
        }

        public string ToString()
        {
            string returnString = "";
            int id = 0;
            int childnum = 0;
            foreach (TreeNode<T> node in Children)
            {
                returnString += node.ToString_(++id,childnum++);
            }
            return returnString;
        }

        public string ToString_(int global, int childnum, int depth = 0)
        {
            string returnString = $"{global} , {childnum} , " + this.value.ToString();
            for(int i = 0; i < depth; i++)
            {
                returnString = "==" + returnString;
            }
            returnString += '\n';
            int id = global;
            childnum = 0;
            foreach (TreeNode<T> node in Children)
            {
                returnString += node.ToString_(++id, childnum++, depth+1);
            }
            return returnString;
        }
    }
}
