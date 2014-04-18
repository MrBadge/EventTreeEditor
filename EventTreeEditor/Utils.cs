using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EventTreeEditor
{   
    class Utils
    {
        private static TreeNode GtoTNode(GraphNode gn)
        {
            return new TreeNode(gn.Data1, gn);
        }
        
        public static Tree<TreeNode> ObjArrToTree(List<GraphNode> arr)
        {
            var original = arr.Find(item => item.Parent == null);
            if (original == null)
                return null;
            var root = new Tree<TreeNode>(GtoTNode(original));
            var clone = root;

            while (original != null)
            {
                if (original.Left != null && clone.Left == null)
                {
                    clone.Left = new Tree<TreeNode>(GtoTNode(original.Left));
                    original = original.Left;
                    clone = clone.Left;
                }
                else if (original.Right != null && clone.Right == null)
                {
                    clone.Right = new Tree<TreeNode>(GtoTNode(original.Right));
                    original = original.Right;
                    clone = clone.Right;
                }
                else
                {
                    original = original.Parent;
                    clone = clone.Parent;
                }
            }
            
            return root;
        }

        public static Double LineLength(Point A, Point B)
        {
            return Math.Sqrt(Math.Pow(B.X - A.X, 2) + Math.Pow(B.Y - A.Y, 2));
        }

        public static void NormalizeGraph(List<GraphNode> graph, PictureBox field)
        {
            if (graph == null || field == null) return;
            var root = graph.Find(item => item.Level == 0);
            if (root == null) return;
            int levels = root.Height(root) + 1;
            int level_Y = field.Height/levels;
            root.Y = level_Y / 2;
            root.X = field.Width / 2;
            for (int i = 1; i < levels; i++)
            {
                var nodes = graph.FindAll(item => item.Level == i);
                int level_X = Convert.ToInt32(field.Width/Math.Pow(2, i));
                var shift = -1;
                foreach (var node in nodes)
                {
                    node.Y = level_Y*(i + 1) - level_Y/2;
                    node.X = node.Parent.X + shift*level_X/2;
                    //node.Value.Data2.X = level_X*(shift + 1) - level_X/2;
                    shift *= -1;
                }
            }
        }
    }
}
