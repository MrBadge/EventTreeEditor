﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace EventTreeEditor
{
    public class TreeNode
    {
        public static int DefPointRad = 2;
        public static int DefCirclRad = 20;
        //public const int DefDiagRad = 20;
        public static int DefLineWidth = 1;
        public static double zoomFactor = 1;
        public static double MaxZoomFactor = 3;

        internal class Type
        {
            public const int Root = 0;
            public const int Node = 1;
            public const int Leaf = 2;
        }

        public string Data1 { get; set; }
        public GraphNode Data2 { get; set; }
        public string SubTreeName { get; set; }

        public TreeNode(string data1, GraphNode data2)
        {
            Data1 = data1;
            Data2 = data2;
        }

        public TreeNode()
        {
            
        }
        //public Type type;

    }

    public class GraphNode : TreeNode
    {
        public Color color { get; set; }

        private int _X;
        private int _Y;
        private int _Radius;
        public int X { get { return Convert.ToInt32(_X * zoomFactor); }
            set { _X = Convert.ToInt32(value / zoomFactor); } 
        }

        public int Y { get { return Convert.ToInt32(_Y * zoomFactor); }
            set { _Y = Convert.ToInt32(value / zoomFactor); }
        }

        public int Radius { get { return Convert.ToInt32(_Radius * zoomFactor); } set { _Radius = value; } }

        //public List<GraphNode> childNodes;
        public GraphNode Left { get; set; }
        public GraphNode Right { get; set; }
        public GraphNode Parent { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        /*public GraphNode Left
        {
            get { return childNodes.Count > 0 ? childNodes[0] : null; }
            set { childNodes[0] = value; }
        }

        public GraphNode Right
        {
            get { return childNodes.Count > 1 ? childNodes[1] : null; }
            set { childNodes[1] = value; }
        }*/

        public GraphNode(GraphNode parent)
        {
            //childNodes = new List<GraphNode>();
            Parent = parent;
        }

        public bool IsRoot
        {
            get
            {
                return Parent == null;
            }
        }

        public int Level
        {
            get
            {
                return IsRoot ? 0 : Parent.Level + 1;
            }
        }

        public int Height(GraphNode node)
        {
            if (node == null)
                return -1;

            int lefth = Height(node.Left);
            int righth = Height(node.Right);

            if (lefth > righth)
                return lefth + 1;
            else
                return righth + 1;
        }

        /*public void Scale(int rad)
        {
            Radius = rad;
        }*/

        public void ChangeParams(int x, int y, /*Color CurClr,*/ int rad)
        {
            Radius = rad;
            X = x;
            Y = y;
            //color = CurClr;
            //Radius = rad;
        }

        private void DrawConnections(Graphics field)
        {
            var Pen = new Pen(Color.Black, DefLineWidth);
            Pen.EndCap = LineCap.ArrowAnchor;
            var from = new Point(X, Y);
            foreach (var node in new List<GraphNode> {Left, Right})
            {
                if (node == null) continue;
                var to = new Point(node.X, node.Y);
                var lambda = Radius/(Utils.LineLength(from, to) - Radius);
                var X_from = Math.Round((from.X + lambda*node.X)/(1 + lambda));
                var Y_from = Math.Round(from.Y + lambda*node.Y)/(1 + lambda);
                lambda = node.Radius / (Utils.LineLength(from, to) - node.Radius);
                var X_to = Math.Round(node.X + lambda * from.X) / (1 + lambda);
                var Y_to = Math.Round(node.Y + lambda * from.Y) / (1 + lambda);
                try
                {
                    field.DrawLine(Pen, new Point(Convert.ToInt32(X_from), Convert.ToInt32(Y_from)),
                        new Point(Convert.ToInt32(X_to), Convert.ToInt32(Y_to)));
                }
                catch
                {
                    
                }
            }
            Pen.Dispose();
        }

        public void Draw(Graphics field)
        {
            DrawConnections(field);
            var Brush = new SolidBrush(color);
            var Pen = new Pen(Color.Black, DefLineWidth);
            var p = new Point(X, Y);//(Convert.ToInt32(X*zoomFactor), Convert.ToInt32(Y*zoomFactor));
            //var Rad = DefCirclRad;//Convert.ToInt32(DefCirclRad*zoomFactor);
            //Radius = Rad;
            field.FillEllipse(Brush, p.X - Radius, p.Y - Radius, 2 * Radius, 2 * Radius);
            field.DrawEllipse(Pen, p.X - Radius, p.Y - Radius, 2 * Radius, 2 * Radius);
            Pen.Dispose();
            Brush.Dispose();
        }

        public void Move(int x, int y)
        {
            X = x;
            Y = y;
        }

    }
}
