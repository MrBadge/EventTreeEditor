using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace EventTreeEditor
{   
    class Utils
    {
        private static TreeNode GtoTNode(GraphNode gn)
        {
            return new TreeNode(gn.Name, gn.operand, gn);
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
            var root = new GraphNode(null)
            {
                Radius = TreeNode.DefCirclRad,
                operand = operand.Text,
                SubTreeName = tree.Text.Split(new[] { '.', ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[1],
                ID = tree.Text.Split('.')[0]
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
                    //root.SubTreeName = root.operand.Split(new[] { '.', ' ' }, 2, StringSplitOptions.RemoveEmptyEntries)[1];
                    //root.ID = root.operand.Split('.')[0];
                    root.operand = operand.Text;
                    //root = root_tmp;
                }
                //clone = operand;
                //if (LeftTree == null && operand == null) continue;
                var RightTree = tree.Nodes.Count > 2 ? tree.Nodes[2] : null;
                if (LeftTree != null && root.Left == null)
                {
                    var Left = new GraphNode(root)
                    {
                        Radius = TreeNode.DefCirclRad,
                        SubTreeName = LeftTree.Text.Split(new[] {'.', ' '}, 2, StringSplitOptions.RemoveEmptyEntries)[1],
                        ID = LeftTree.Text.Split('.')[0]
                    };
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
                    var Right = new GraphNode(root)
                    {
                        X = 0,
                        Y = 0,
                        Radius = TreeNode.DefCirclRad,
                        SubTreeName =
                            RightTree.Text.Split(new[] {'.', ' '}, 2, StringSplitOptions.RemoveEmptyEntries)[1],
                        ID = RightTree.Text.Split('.')[0]
                    };
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
                var shift_X = field.Width/(2*nodes.Count);
                var j = 1;
                foreach (var node in nodes)
                {
                    //var chCount = node.Left == null ? 0 : 1;
                    //chCount += node.Right == null ? 0 : 1;
                    //chCount = chCount == 0 ? 2 : chCount;
                    node.Y = level_Y*(i + 1) - level_Y/2;
                    if (nodes.Count > 1)
                        node.X =/* node.Parent.X + */shift_X*j;//shift*level_X/2;
                    else
                        node.X = node.Parent.X;
                    j+=2;
                    //node.Value.Data2.X = level_X*(shift + 1) - level_X/2;
                    shift *= -1;
                }
            }
        }

        public static void DrawTextOnCircle(Graphics gr, Font font, Brush brush,
            float radius, float cx, float cy, string top_text, string bottom_text)
        {
            using (StringFormat string_format = new StringFormat())
            {
                string_format.Alignment = StringAlignment.Center;
                string_format.LineAlignment = StringAlignment.Far;

                double radians_to_degrees = 180.0 / Math.PI;

                // **********************
                // * Draw the top text. *
                // **********************
                List<RectangleF> rects = MeasureCharacters(gr, font, top_text);

                var width_query = from RectangleF rect in rects select rect.Width;
                float text_width = width_query.Sum();

                double width_to_angle = 1 / radius;
                double start_angle = -Math.PI / 2 - text_width / 2 * width_to_angle;
                double theta = start_angle;

                for (int i = 0; i < top_text.Length; i++)
                {
                    theta += rects[i].Width / 2 * width_to_angle;
                    double x = cx + radius * Math.Cos(theta);
                    double y = cy + radius * Math.Sin(theta);

                    gr.RotateTransform((float)(radians_to_degrees * (theta + Math.PI / 2)));
                    gr.TranslateTransform((float)x, (float)y, MatrixOrder.Append);

                    gr.DrawString(top_text[i].ToString(), font, brush,
                        0, 0, string_format);
                    gr.ResetTransform();

                    theta += rects[i].Width / 2 * width_to_angle;
                }

                // *************************
                // * Draw the bottom text. *
                // *************************
                rects = MeasureCharacters(gr, font, bottom_text);

                width_query = from RectangleF rect in rects select rect.Width;
                text_width = width_query.Sum();

                width_to_angle = 1 / radius;
                start_angle = Math.PI / 2 + text_width / 2 * width_to_angle;
                theta = start_angle;

                string_format.LineAlignment = StringAlignment.Near;

                for (int i = 0; i < bottom_text.Length; i++)
                {
                    theta -= rects[i].Width / 2 * width_to_angle;
                    double x = cx + radius * Math.Cos(theta);
                    double y = cy + radius * Math.Sin(theta);

                    gr.RotateTransform((float)(radians_to_degrees * (theta - Math.PI / 2)));
                    gr.TranslateTransform((float)x, (float)y, MatrixOrder.Append);

                    gr.DrawString(bottom_text[i].ToString(), font, brush,
                        0, 0, string_format);
                    gr.ResetTransform();

                    theta -= rects[i].Width / 2 * width_to_angle;
                }
            }
        }

        private static List<RectangleF> MeasureCharacters(Graphics gr, Font font, string text)
        {
            List<RectangleF> results = new List<RectangleF>();

            float x = 0;

            for (int start = 0; start < text.Length; start += 32)
            {
                int len = 32;
                if (start + len >= text.Length) len = text.Length - start;
                string substring = text.Substring(start, len);

                List<RectangleF> rects = MeasureCharactersInWord(gr, font, substring);

                if (start == 0) x += rects[0].Left;

                for (int i = 0; i < rects.Count + 1 - 1; i++)
                {
                    RectangleF new_rect = new RectangleF(
                        x, rects[i].Top,
                        rects[i].Width, rects[i].Height);
                    results.Add(new_rect);

                    x += rects[i].Width;
                }
            }

            return results;
        }

        private static List<RectangleF> MeasureCharactersInWord(
            Graphics gr, Font font, string text)
        {
            List<RectangleF> result = new List<RectangleF>();

            using (StringFormat string_format = new StringFormat())
            {
                string_format.Alignment = StringAlignment.Near;
                string_format.LineAlignment = StringAlignment.Near;
                string_format.Trimming = StringTrimming.None;
                string_format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;

                CharacterRange[] ranges = new CharacterRange[text.Length];
                for (int i = 0; i < text.Length; i++)
                {
                    ranges[i] = new CharacterRange(i, 1);
                }
                string_format.SetMeasurableCharacterRanges(ranges);

                RectangleF rect = new RectangleF(0, 0, 10000, 100);
                Region[] regions =
                    gr.MeasureCharacterRanges(
                        text, font, rect,
                        string_format);

                foreach (Region region in regions)
                    result.Add(region.GetBounds(gr));
            }

            return result;
        }
    }
}
