using Microsoft.Xna.Framework;
using System.Linq;
using System.Collections.Generic;

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
            return FindPath(tileMap, position, new Vector2[]{targetPosition}, tileSize);
        }

        public static List<Vector2> FindPath(Array2D<bool> tileMap, Vector2 position, Vector2[] targetPositions, int tileSize = 128)
        {
            Point pos = FindTile(position, tileSize);

            Point[] targets = new Point[targetPositions.Length];
            for (int i = 0; i < targets.Length; i++)
                targets[i] = FindTile(targetPositions[i], tileSize);

            List<Point> path = AStar(tileMap, pos, targets);
            if (path == null)
                return new List<Vector2>();

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

        private static List<Point> AStar(Array2D<bool> tileMap, Point start, Point[] targets)
        {
            SortedLinkedList<Node> open = new SortedLinkedList<Node>(Comparer<Node>.Create((a, b) => a.F.CompareTo(b.F)));
            HashSet<Node> closed = new();

            {
                Node startNode = new Node(start, null, 0, H(start, targets));
                open.Add(startNode);
            }

            while (open.Count > 0)
            {
                Node cur = open.First();
                open.RemoveFirst();
                closed.Add(cur);

                if (targets.Any(target => target == cur.Pos))
                {
                    List<Point> path = new List<Point>();

                    for (Node node = cur; node != null; node = node.From)
                        path.Insert(0, node.Pos);

                    return path;
                }
                else
                {
                    List<Node> adjacent = GetAdjacentNodes(tileMap, cur, targets);

                    foreach (var adj in adjacent)
                    {
                        if (closed.Contains(adj))
                            continue;

                        var actual = open.Find(adj);
                        if (actual == null || adj.F < actual!.Value.F)
                        {
                            if (actual != null)
                                open.Remove(actual);
                            open.Add(adj);
                        }
                    }
                }
            }

            return null;
        }

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
            return dist.X * dist.X + dist.Y * dist.Y;
        }

        private static float H(Point pos, Point[] targets)
        {
            return targets.Min(target => H(pos, target));       // returns the closest (smallest) H-score between all targets
        }

        private static Point ClosestTarget(Point pos, Point[] targets)
        {
            return targets.MinBy(target => H(pos, target));
        }

        private static List<Node> GetAdjacentNodes(Array2D<bool> tileMap, Node node, Point[] targets)
        {
            List<Point> adjacentPoints = GetAdjacentPoints(tileMap, node.Pos);

            List<Node> adjacentNodes = new List<Node>(adjacentPoints.Count);
            foreach (var point in adjacentPoints)
                adjacentNodes.Add(new Node(point, node, node.G + 1, H(point, targets)));

            return adjacentNodes;
        }

        private static List<Point> GetAdjacentPoints(Array2D<bool> tileMap, Point pos)
        {
            List<Point> adj = new List<Point>();

            if (pos.X > 0 && !tileMap[pos.X - 1, pos.Y])
                adj.Add(new Point(pos.X - 1, pos.Y));
            if (pos.X < tileMap.Width - 1 && !tileMap[pos.X + 1, pos.Y])
                adj.Add(new Point(pos.X + 1, pos.Y));
            if (pos.Y > 0 && !tileMap[pos.X, pos.Y - 1])
                adj.Add(new Point(pos.X, pos.Y - 1));
            if (pos.Y > tileMap.Height - 1 && !tileMap[pos.X, pos.Y + 1])
                adj.Add(new Point(pos.X, pos.Y + 1));

            return adj;
        }
    }
}
