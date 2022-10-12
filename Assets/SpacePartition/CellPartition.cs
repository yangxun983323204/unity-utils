using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YX.SpacePartition
{
    public class CellPartition
    {
        public struct Cell
        {
            public int x, y, z;
            public List<IAgent> agents;

            public void Add(IAgent agent)
            {
                if (agents == null)
                    agents = new List<IAgent>(2);

                agents.Add(agent);
            }

            public void Remove(IAgent agent)
            {
                if (agents!=null)
                {
                    agents.Remove(agent);
                }
            }

            public override string ToString()
            {
                return $"({x},{y},{z}):Cnt {agents?.Count}";
            }
        }

        private Vector3 _pos = Vector3.zero;
        public Vector3 Pos
        {
            get { return _pos; }
            set { _pos = value; UpdateMinCorner(); }
        }

        private Vector3 _pivot = new Vector3(0.5f, 0.5f, 0.5f);
        public Vector3 Pivot
        {
            get { return _pivot; }
            set { _pivot = value; UpdateMinCorner(); }
        }

        private Vector3 _minCorner;

        Cell[] _cells = null;
        ArrayView3D<Cell> _cellsView = new ArrayView3D<Cell>();
        Dictionary<IAgent, int> _agentsDict = new Dictionary<IAgent, int>();

        int _xCnt, _yCnt, _zCnt;
        float _xStep, _yStep, _zStep;

        public void CreateGrid(int xCnt, int yCnt, int zCnt, float xStep, float yStep, float zStep)
        {
            Reset();
            _xCnt = xCnt;
            _yCnt = yCnt;
            _zCnt = zCnt;
            _xStep = xStep;
            _yStep = yStep;
            _zStep = zStep;

            _cells = new Cell[xCnt * yCnt * zCnt];
            _cellsView.Bind(_cells, xCnt, yCnt, zCnt);
            for (int x = 0; x < _xCnt; x++)
            {
                for (int y = 0; y < _yCnt; y++)
                {
                    for (int z = 0; z < _zCnt; z++)
                    {
                        var idx = _cellsView.Idx(x, y, z);
                        _cells[idx].x = x;
                        _cells[idx].y = y;
                        _cells[idx].z = z;
                    }
                }
            }
            UpdateMinCorner();
        }

        public void AddAgent(IAgent agent)
        {
            agent.onMoved += OnAgentMove;
            UpdateAgent(agent);
        }

        public void RemoveAgent(IAgent agent)
        {
            agent.onMoved -= OnAgentMove;
            int i;
            if (_agentsDict.TryGetValue(agent,out i))
            {
                _agentsDict.Remove(agent);
                _cells[i].Remove(agent);
            }
        }

        public void Reset()
        {
            if (_cells != null)
            {
                foreach (var c in _cells)
                {
                    if (c.agents == null)
                        continue;

                    foreach (var agent in c.agents)
                    {
                        agent?.Reset();
                    }
                }
            }
        }

        public void GetAdjoin(IAgent agent, ref List<IAgent> adjList)
        {
            adjList.Clear();
            int x, y, z;
            int i;
            if (_agentsDict.TryGetValue(agent,out i))
            {
                var center = _cells[i];
                x = center.x;
                y = center.y;
                z = center.z;

                var ids = new int[] {
                    // 仅一个轴上相邻
                    _cellsView.Idx(x-1, y ,z), _cellsView.Idx(x+1, y ,z),
                    _cellsView.Idx(x, y-1 ,z), _cellsView.Idx(x, y+1 ,z),
                    _cellsView.Idx(x, y ,z-1), _cellsView.Idx(x, y ,z+1),
                    // x,y轴
                    _cellsView.Idx(x-1, y-1 ,z), _cellsView.Idx(x+1, y-1 ,z),
                    _cellsView.Idx(x-1, y+1 ,z), _cellsView.Idx(x+1, y+1 ,z),
                    // x,z轴
                    _cellsView.Idx(x-1, y ,z-1), _cellsView.Idx(x+1, y ,z-1),
                    _cellsView.Idx(x-1, y ,z+1), _cellsView.Idx(x+1, y ,z+1),
                    // y,z轴
                    _cellsView.Idx(x, y-1 ,z-1), _cellsView.Idx(x, y+1 ,z-1),
                    _cellsView.Idx(x, y-1 ,z+1), _cellsView.Idx(x, y+1 ,z+1),
                    // 三轴
                    _cellsView.Idx(x-1, y-1 ,z-1), _cellsView.Idx(x+1, y-1 ,z-1),
                    _cellsView.Idx(x-1, y-1 ,z+1), _cellsView.Idx(x+1, y-1 ,z+1),
                    _cellsView.Idx(x-1, y+1 ,z-1), _cellsView.Idx(x+1, y+1 ,z-1),
                    _cellsView.Idx(x-1, y+1 ,z+1), _cellsView.Idx(x+1, y+1 ,z+1),
                };

                foreach (var idx in ids)
                {
                    if (idx >= 0 && idx < _cells.Length)
                    {
                        var adj = _cells[idx];
                        if (adj.agents!=null && adj.agents.Count>0)
                        {
                            adjList.AddRange(adj.agents);
                        }
                    }
                }
            }
        }

        public Color GimzosColor = new Color(0,1,0,0.5f);

        public void DrawGizmos()
        {
            var oldColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_pos, Mathf.Min(_xStep, _yStep, _zStep) * 0.1f);
            Gizmos.DrawSphere(_minCorner, Mathf.Min(_xStep, _yStep, _zStep) * 0.1f);
            Gizmos.color = GimzosColor;
            for (int y = 0; y <= _yCnt; y++)
            {
                for (int z = 0; z <= _zCnt; z++)
                {
                    Gizmos.DrawLine(
                        _minCorner + new Vector3(0, y * _yStep, z * _zStep), 
                        _minCorner + new Vector3(_xStep * _xCnt, y * _yStep, z * _zStep));
                }
            }

            for (int x = 0; x <= _xCnt; x++)
            {
                for (int z = 0; z <= _zCnt; z++)
                {
                    Gizmos.DrawLine(
                        _minCorner + new Vector3(x * _xStep, 0, z * _zStep),
                        _minCorner + new Vector3(x * _xStep, _yStep * _yCnt, z * _zStep));
                }
            }

            for (int y = 0; y <= _yCnt; y++)
            {
                for (int x = 0; x <= _xCnt; x++)
                {
                    Gizmos.DrawLine(
                        _minCorner + new Vector3(x * _xStep, y * _yStep, 0),
                        _minCorner + new Vector3(x * _xStep, y * _yStep, _zStep * _zCnt));
                }
            }

            var cubeSize = new Vector3(_xStep*0.95f, _yStep*0.95f, _zStep*0.95f);
            for (int x = 0; x < _xCnt; x++)
            {
                for (int y = 0; y < _yCnt; y++)
                {
                    for (int z = 0; z < _zCnt; z++)
                    {
                        var cell = _cellsView.Get(x, y, z);
                        if (cell.agents!=null && cell.agents.Count >0)
                        {
                            Gizmos.DrawCube(
                                _minCorner + new Vector3((x+0.5f) * _xStep, (y+0.5f) * _yStep, (z+0.5f) * _zStep),
                                cubeSize
                                );
                        }
                    }
                }
            }
            Gizmos.color = oldColor;
        }

        #region Private
        private void UpdateMinCorner()
        {
            float xSize = _xStep * _xCnt;
            float ySize = _yStep * _yCnt;
            float zSize = _zStep * _zCnt;

            _minCorner.x = _pos.x - _pivot.x * xSize;
            _minCorner.y = _pos.y - _pivot.y * ySize;
            _minCorner.z = _pos.z - _pivot.z * zSize;
        }

        private void CalcCoordinate(Vector3 pos,out int x,out int y,out int z)
        {
            x = Mathf.CeilToInt((pos.x - _minCorner.x) / _xStep) - 1;
            y = Mathf.CeilToInt((pos.y - _minCorner.y) / _yStep) - 1;
            z = Mathf.CeilToInt((pos.z - _minCorner.z) / _zStep) - 1;

            x = Mathf.Clamp(x, 0, _xCnt - 1);
            y = Mathf.Clamp(y, 0, _yCnt - 1);
            z = Mathf.Clamp(z, 0, _zCnt - 1);
        }

        private Vector3 _tmpPos;

        private void UpdateAgent(IAgent agent)
        {
            agent.GetPosition(ref _tmpPos);
            int x, y, z;
            CalcCoordinate(_tmpPos, out x, out y, out z);

            int i;
            var idx = _cellsView.Idx(x, y, z);
            if (_agentsDict.TryGetValue(agent, out i))
            {
                var oldCell = _cells[i];
                if (oldCell.x == x && oldCell.y == y && oldCell.z == z)
                {
                    return; // 单元格无变化
                }
                else
                {
                    oldCell.Remove(agent);
                    _cells[i] = oldCell;
                    _cells[idx].Add(agent);
                    _agentsDict[agent] = idx;
                }
            }
            else
            {
                _cells[idx].Add(agent);
                _agentsDict[agent] = idx;
            }
        }

        private void OnAgentMove(IAgent agent)
        {
            UpdateAgent(agent);
        }
        #endregion
    }
}
