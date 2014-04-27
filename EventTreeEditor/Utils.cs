using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

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

        public static List<GraphNode> TreeToObjArr(System.Windows.Forms.TreeNode tree)
        {
            //var original = arr.Find(item => item.Parent == null);
            var ObjArr = new List<GraphNode>();
            if (tree == null || tree.Nodes.Count == 0)
                return null;

            //var LeftTree = tree.Nodes.Count > 0 ? tree.Nodes[0] : null;
            var operand = tree.Nodes.Count > 1 ? tree.Nodes[1] : null;
            //if (LeftTree == null && operand == null) continue;
            //var RightTree = tree.Nodes.Count >= 2 ? tree.Nodes[2] : null;
            //GraphNode clone = null;//new GraphNode(null) {X = 0, Y = 0, Radius = TreeNode.DefCirclRad, Data1 = tree.Text};
            GraphNode root = new GraphNode(null)
            {
                Radius = TreeNode.DefCirclRad,
                Data1 = operand.Text,
            };
            ObjArr.Add(root);
            //var Left = new GraphNode(rootTmp) { Radius = TreeNode.DefCirclRad, Data1 = LeftTree.Text };
            //var Right = new GraphNode(rootTmp) { Radius = TreeNode.DefCirclRad, Data1 = RightTree.Text };
            //ObjArr.Add(clone);
            //var clone = tree;

            while (tree != null)
            {
                //if (tree.Nodes.Count >= 0)
                var LeftTree = tree.Nodes.Count > 0 ? tree.Nodes[0] : null;
                operand = tree.Nodes.Count > 1 ? tree.Nodes[1] : null;
                if (root.Level != 0 && operand != null && root.Left == null && root.Right == null)
                {
                    /*var root_tmp = new GraphNode(root)
                    {
                        Radius = TreeNode.DefCirclRad,
                        Data1 = operand.Text
                    };*/
                    //root.Left = root_tmp;
                    //ObjArr.Add(root_tmp);
                    root.SubTreeName = root.Data1;
                    root.Data1 = operand.Text;
                    //root = root_tmp;
                }
                //clone = operand;
                //if (LeftTree == null && operand == null) continue;
                var RightTree = tree.Nodes.Count > 2 ? tree.Nodes[2] : null;
                if (LeftTree != null && root.Left == null)
                {
                    var Left = new GraphNode(root) { Radius = TreeNode.DefCirclRad, Data1 = LeftTree.Text };
                    root.Left = Left;
                    /*root = new GraphNode(clone)
                    {
                        Radius = TreeNode.DefCirclRad,
                        Data1 = operand.Text,
                        Left = Left
                    };*/
                    ObjArr.Add(Left);
                    tree = LeftTree;
                    root = root.Left;
                }
                else 
                //if (tree.Nodes.Count >= 2)
                if (RightTree != null && root.Right == null)
                {
                    var Right = new GraphNode(root) { X = 0, Y = 0, Radius = TreeNode.DefCirclRad, Data1 = RightTree.Text };
                    root.Right = Right;
                    /*var rootTmp = new GraphNode(clone)
                    {
                        Radius = TreeNode.DefCirclRad,
                        Data1 = operand.Text,
                        Right = Right
                    };*/
                    ObjArr.Add(Right);
                    tree = RightTree;
                    root = root.Right;
                }
                else
                {
                    tree = tree.Parent;
                    root = root.Parent;
                }
            }

            return ObjArr;
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
