using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System;

namespace Capstone_Project.Fundamentals
{
    public static class Pathfinding
    {
        public static Point FindTile(Vector2 position, int tileSize)
        {
            return new Point((int)(position.X / tileSize), (int)(position.Y / tileSize));
        }

        public static List<Vector2> FindPath(Array2D<bool> tileMap, Vector2 position, Vector2 targetPosition, int tileSize = 128)
        {
            Point pos = FindTile(position, tileSize);
            Point target = FindTile(targetPosition, tileSize);

            //List<Point> path = AStar(tileMap, pos, target);
            List<Point> path = AStar(tileMap, pos, target);
            if (path == null)
            {
                Debug.WriteLine("No path found");
                return new List<Vector2>();
            }

            path = ExtractCorners(path);

            int halfTile = tileSize / 2;
            return path.Select(p => new Vector2(p.X * tileSize + halfTile, p.Y * tileSize + halfTile)).ToList();
        }

        private static List<Point> ExtractCorners(List<Point> path)
        {
            // can't find any corners without at least three points
            if (path.Count < 3)
                return new() { path.Last() };

            List<Point> cornerless = new List<Point>();

            // if [prev]sameX == false, then that means the [prev]sameY is true
            bool prevSameX = path[0].X == path[1].X;
            for (int i = 1; i < path.Count - 1; i++)
            {
                bool sameX = path[i].X == path[i + 1].X;

                // if the X is the same previously, but now the Y is the same, then it is a corner
                if (prevSameX != sameX)
                    cornerless.Add(path[i]);

                prevSameX = sameX;
            }
            // always add the target
            cornerless.Add(path.Last());

            return cornerless;
        }

        private static List<Point> AStar(Array2D<bool> tileMap, Point start, Point target)
        {
            var open = new PriorityQueue<Node, float>();
            var closed = new List<Node>();

            var startNode = new Node(start, null, 0, H(start, target));
            open.Enqueue(startNode, startNode.F);

            while (open.TryDequeue(out Node cur, out _))
            {
                closed.Add(cur);

                if (cur.Pos == target)
                {
                    var path = new List<Point> { cur.Pos };
                    for (var node = cur.From; node != null; node = node.From)
                        path.Insert(0, node.Pos);
                    return path;
                }

                var adjacentNodes = GetAdjacentPoints(tileMap, cur.Pos).Select(p => new Node(p, cur, cur.G + 1, H(p, target)));
                foreach (var adj in adjacentNodes)
                {
                    Node actual = closed.Find(n => n.Pos == adj.Pos);
                    if (actual == null)
                    {
                        // if adj is not in open, or adj has a lower F than the one in open
                        if ((actual = open.UnorderedItems.FirstOrDefault(n => n.Element.Pos == adj.Pos, (null, 0)).Element) == null || actual.F > adj.F)
                        {
                            open.Enqueue(adj, adj.F);
                        }
                    }
                }
            }

            return null;
        }

        /*private static List<Point> AStar(Array2D<bool> tileMap, Point start, Point target)
        {
            SortedLinkedList<Node> open = new SortedLinkedList<Node>(Comparer<Node>.Create((a, b) => a.F.CompareTo(b.F)));
            HashSet<Node> closed = new();

            {
                Node startNode = new Node(start, null, 0, H(start, target));
                open.Add(startNode);
            }

            while (open.Count > 0)
            {
                Node cur = open.First();
                open.RemoveFirst();
                closed.Add(cur);

                if (target == cur.Pos)
                {
                    List<Point> path = new List<Point>();
                    for (Node node = cur; node != null; node = node.From)
                        path.Insert(0, node.Pos);

                    return path;
                }

                List<Node> adjacentNodes = GetAdjacentNodes(tileMap, cur, target);
                foreach (var node in adjacentNodes)
                {
                    if (closed.Contains(node))
                        continue;

                    var actual = open.Find(node);
                    if (actual != null)
                    {
                        if (node.G < actual.Value.G)
                        {
                            open.Remove(actual);
                            open.Add(node);
                        }
                    }
                    else
                    {
                        open.Add(node);
                    }
                }
            }

            return null;
        }*/

        private class Node
        {
            public Node From;
            public Point Pos;       // only thing regarded in the hash code
            public int G;
            public float H;
            public float F => G + H;

            public Node(Point pos, Node parent, int g, float h)
            {
                From = parent;
                Pos = pos;
                G = g;
                H = h;
            }

            public override int GetHashCode()
            {
                return Pos.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (obj != null && obj is Node other)
                    return other.Pos == this.Pos;
                return false;
            }
        }

        // currently just Euclidean Distance squared
        private static float H(Point pos, Point target)
        {
            Point dist = target - pos;
            return MathF.Sqrt(dist.X * dist.X + dist.Y * dist.Y);
        }

        /*private static List<Node> GetAdjacentNodes(Array2D<bool> tileMap, Node node, Point target)
        {
            List<Point> adjacentPoints = GetAdjacentPoints(tileMap, node.Pos);
            List<Node> adjacentNodes = adjacentPoints.Select(point => new Node(point, node, node.G + 1, H(point, target))).ToList();

            return adjacentNodes;
        }*/

        private static List<Point> GetAdjacentPoints(Array2D<bool> tileMap, Point pos)
        {
            List<Point> adj = new List<Point>();

            // if there's a wall at the tile (x,y), then wall[x,y] = true
            if (pos.X > 0 && !tileMap[pos.X - 1, pos.Y])                    // left
                adj.Add(new Point(pos.X - 1, pos.Y));
            if (pos.X < tileMap.Width - 1 && !tileMap[pos.X + 1, pos.Y])    // right
                adj.Add(new Point(pos.X + 1, pos.Y));
            if (pos.Y > 0 && !tileMap[pos.X, pos.Y - 1])                    // up
                adj.Add(new Point(pos.X, pos.Y - 1));
            if (pos.Y < tileMap.Height - 1 && !tileMap[pos.X, pos.Y + 1])   // down
                adj.Add(new Point(pos.X, pos.Y + 1));

            return adj;
        }
    }
}
