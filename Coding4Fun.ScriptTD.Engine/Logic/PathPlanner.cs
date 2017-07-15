using System;
using System.Collections.Generic;
using System.Linq;

namespace Coding4Fun.ScriptTD.Engine.Logic
{
    public class Path
    {
        public Cell End;
        public readonly List<PathNode> Nodes = new List<PathNode>();
        public bool IsAirPath;
        public bool PathFound;

        public Cell CurrentCell;

        private bool _firstRun = true;

        public void ClearData()
        {
            Nodes.Clear();
            End = null;
            IsAirPath = false;
            _firstRun = true;
            PathFound = false;
        }

        public void CompleteNode(PathNode node)
        {
            if (!IsAirPath)
            {
                Nodes.Remove(node);
                CurrentCell = node.Cell;
            }
        }

        public bool IsPathValid()
        {
            if (_firstRun)
            {
                _firstRun = false;
                return false;
            }

            if (IsAirPath)
                return true;

            if (!PathFound)
                return false;

        	return Nodes.All(t => (!t.Cell.IsLandObstruction && t.Cell.Tower == null) && !t.Cell.InvalidCell);
        }
    }

    public class PathNode : IComparable<PathNode>
    {
        public Cell Cell;
        public int CostFromStart;
        public int CostToGoal;
        public PathNode Parent;

        // N, S, E, W
        public PathNode[] Adjacent = new PathNode[4];

        public int TotalCost
        {
            get { return CostFromStart + CostToGoal; }
        }

        public int EstimateCostTo(ref Cell toCell)
        {
            return Math.Abs(toCell.X - Cell.X) + Math.Abs(toCell.Y - Cell.Y);
        }

        public int CompareTo(PathNode other)
        {
            return TotalCost.CompareTo(other.TotalCost);
        }
    }

    public class PathPlanner
    {
        private readonly Queue<Path> _pathPool = new Queue<Path>();
        private readonly List<Path> _allocatedPaths = new List<Path>();
        private int _curMaxPaths = 20;
        private PathNode[,] _nodeGrid;

        private readonly List<PathNode> _open = new List<PathNode>();
        private readonly List<PathNode> _closed = new List<PathNode>();

        public PathPlanner()
        {
            for (int i = 0; i < _curMaxPaths; i++)
            {
                _pathPool.Enqueue(new Path());
            }
        }

        public void InitNodes(ref Grid cells)
        {
            _nodeGrid = new PathNode[cells.Columns, cells.Rows];

            foreach (var gridCell in cells.CellArray)
            {
                var p = new PathNode();
                _nodeGrid[gridCell.X, gridCell.Y] = p;
                p.Cell = gridCell;
            }

            foreach (var node in _nodeGrid)
            {
                node.Adjacent[0] = node.Cell.Y > 0 ? _nodeGrid[node.Cell.X, node.Cell.Y - 1] : null;
                node.Adjacent[1] = node.Cell.Y < (_nodeGrid.GetLength(1) - 1)
                                       ? _nodeGrid[node.Cell.X, node.Cell.Y + 1]
                                       : null;
                node.Adjacent[2] = node.Cell.X < (_nodeGrid.GetLength(0) - 1)
                                       ? _nodeGrid[node.Cell.X + 1, node.Cell.Y]
                                       : null;
                node.Adjacent[3] = node.Cell.X > 0 ? _nodeGrid[node.Cell.X - 1, node.Cell.Y] : null;
            }
        }

        public Path GetPath(Cell start, Cell end, bool isAirPath)
        {
            if (_pathPool.Count == 0)
            {
                for (int i = 0; i < _curMaxPaths / 2; i++)
                {
                    _pathPool.Enqueue(new Path());
                }
                _curMaxPaths += _curMaxPaths / 2;
            }

            var p = _pathPool.Dequeue();
            p.ClearData();
            p.End = end;
            p.IsAirPath = isAirPath;
            p.CurrentCell = start;

            _allocatedPaths.Add(p);
            return p;
        }

        public void ReleasePath(Path p)
        {
            _allocatedPaths.Remove(p);
            _pathPool.Enqueue(p);
        }

        public void UpdatePaths()
        {
            for (int i = 0; i < _allocatedPaths.Count; i++)
            {
                var p = _allocatedPaths[i];
                if (!p.IsPathValid())
                {
                    if (p.IsAirPath)
                    {
                        p.Nodes.Clear();
                        p.Nodes.Add(_nodeGrid[p.End.X, p.End.Y]);
                        p.PathFound = true;
                    }
                    else
                    {
                        CalculateLandPath(ref p);
                    }
                }
            }
        }

        public bool CalculateLandPath(ref Path p)
        {
            _open.Clear();
            _closed.Clear();

            p.Nodes.Clear();

            var startNode = _nodeGrid[p.CurrentCell.X, p.CurrentCell.Y];
            startNode.CostFromStart = 0;
            startNode.CostToGoal = startNode.EstimateCostTo(ref p.End);
            startNode.Parent = null;
            _open.Add(startNode);

            while (_open.Count > 0)
            {
                // Simulate "popping" from _open
                var node = _open[0];
                _open.RemoveAt(0);

                if (node.Cell.X == p.End.X && node.Cell.Y == p.End.Y)
                {
                    var n = node;
                    while (n != startNode)
                    {
                        p.Nodes.Insert(0, n);
                        n = n.Parent;
                    }
                    p.PathFound = true;
                    return true;
                }

            	for (int i = 0; i < 4; i++)
            	{
            		var adjacent = node.Adjacent[i];
            		if (adjacent != null && !adjacent.Cell.IsLandObstruction && adjacent.Cell.Tower == null && !adjacent.Cell.InvalidCell)
            		{
            			int newCost = node.CostFromStart + adjacent.EstimateCostTo(ref node.Cell);
            			bool isInOpen = _open.Contains(adjacent);
            			bool isInClosed = _closed.Contains(adjacent);

            			if ((isInOpen || isInClosed) && adjacent.CostFromStart <= newCost)
            			{
            				continue;
            			}

            			adjacent.Parent = node;
            			adjacent.CostFromStart = newCost;
            			adjacent.CostToGoal = adjacent.EstimateCostTo(ref p.End);
            			
						if (isInClosed)
            			{
            				_closed.Remove(adjacent);
            			}

            			if (isInOpen)
            			{
            				_open.Sort();
            			}
            			else
            			{
            				_open.Add(adjacent);
            			}
            		}
            	}
            	_closed.Add(node);
            }
            p.PathFound = false;
            return false;
        }
    }
}
