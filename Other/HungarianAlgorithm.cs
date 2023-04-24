using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class HungarianAlgorithm
    {
        private static int[,] _costMatrix;
        private static int _inf;
        private static int _n;
        private static int[] _lx;
        private static int[] _ly;
        private static bool[] _s;
        private static bool[] _t;
        private static int[] _matchX;
        private static int[] _matchY;
        private static int _maxMatch;
        private static int[] _slack;
        private static int[] _slackx;
        private static int[] _prev;

        public static int[] Run(int[,] costMatrix)
        {
            _costMatrix = costMatrix;

            _n = _costMatrix.GetLength(0);

            _lx = new int[_n];
            _ly = new int[_n];
            _s = new bool[_n];
            _t = new bool[_n];
            _matchX = new int[_n];
            _matchY = new int[_n];
            _slack = new int[_n];
            _slackx = new int[_n];
            _prev = new int[_n];
            _inf = int.MaxValue;


            InitMatches();

            if (_n != _costMatrix.GetLength(1))
                return null;

            InitLbls();

            _maxMatch = 0;

            InitialMatching();

            var q = new Queue<int>();

            #region augment

            while (_maxMatch != _n)
            {
                q.Clear();

                InitSt();

                var root = 0;
                int x;
                var y = 0;
                
                for (x = 0; x < _n; x++)
                {
                    if (_matchX[x] != -1) continue;
                    q.Enqueue(x);
                    root = x;
                    _prev[x] = -2;

                    _s[x] = true;
                    break;
                }

                
                for (var i = 0; i < _n; i++)
                {
                    _slack[i] = _costMatrix[root, i] - _lx[root] - _ly[i];
                    _slackx[i] = root;
                }

                while (true)
                {
                    while (q.Count != 0)
                    {
                        x = q.Dequeue();
                        var lxx = _lx[x];
                        for (y = 0; y < _n; y++)
                        {
                            if (_costMatrix[x, y] != lxx + _ly[y] || _t[y]) continue;
                            if (_matchY[y] == -1) break;
                            _t[y] = true;
                            q.Enqueue(_matchY[y]);

                            AddToTree(_matchY[y], x);
                        }
                        if (y < _n) break;
                    }
                    if (y < _n) break;
                    UpdateLabels();

                    for (y = 0; y < _n; y++)
                    {
                        if (_t[y] || _slack[y] != 0) continue;
                        if (_matchY[y] == -1)
                        {
                            x = _slackx[y];
                            break;
                        }
                        _t[y] = true;
                        if (_s[_matchY[y]]) continue;
                        q.Enqueue(_matchY[y]);
                        AddToTree(_matchY[y], _slackx[y]);
                    }
                    if (y < _n) break;
                }

                _maxMatch++;
               
                int ty;
                for (int cx = x, cy = y; cx != -2; cx = _prev[cx], cy = ty)
                {
                    ty = _matchX[cx];
                    _matchY[cy] = cx;
                    _matchX[cx] = cy;
                }
            }

            #endregion

            return _matchX;
        }

        private static void InitMatches()
        {
            for (var i = 0; i < _n; i++)
            {
                _matchX[i] = -1;
                _matchY[i] = -1;
            }
        }

        private static void InitSt()
        {
            for (var i = 0; i < _n; i++)
            {
                _s[i] = false;
                _t[i] = false;
            }
        }

        private static void InitLbls()
        {
            for (var i = 0; i < _n; i++)
            {
                var minRow = _costMatrix[i, 0];
                for (var j = 0; j < _n; j++)
                {
                    if (_costMatrix[i, j] < minRow) minRow = _costMatrix[i, j];
                    if (minRow == 0) break;
                }
                _lx[i] = minRow;
            }
            for (var j = 0; j < _n; j++)
            {
                var minColumn = _costMatrix[0, j] - _lx[0];
                for (var i = 0; i < _n; i++)
                {
                    if (_costMatrix[i, j] - _lx[i] < minColumn) minColumn = _costMatrix[i, j] - _lx[i];
                    if (minColumn == 0) break;
                }
                _ly[j] = minColumn;
            }
        }

        private static void UpdateLabels()
        {
            var delta = _inf;
            for (var i = 0; i < _n; i++)
                if (!_t[i])
                    if (delta > _slack[i])
                        delta = _slack[i];
            for (var i = 0; i < _n; i++)
            {
                if (_s[i])
                    _lx[i] = _lx[i] + delta;
                if (_t[i])
                    _ly[i] = _ly[i] - delta;
                else _slack[i] = _slack[i] - delta;
            }
        }

        private static void AddToTree(int x, int prevx)
        {
            _s[x] = true;
            _prev[x] = prevx;

            var lxx = _lx[x];

            for (var y = 0; y < _n; y++)
            {
                if (_costMatrix[x, y] - lxx - _ly[y] >= _slack[y]) continue;
                _slack[y] = _costMatrix[x, y] - lxx - _ly[y];
                _slackx[y] = x;
            }
        }

        private static void InitialMatching()
        {
            for (var x = 0; x < _n; x++)
            {
                for (var y = 0; y < _n; y++)
                {
                    if (_costMatrix[x, y] != _lx[x] + _ly[y] || _matchY[y] != -1) continue;
                    _matchX[x] = y;
                    _matchY[y] = x;
                    _maxMatch++;
                    break;
                }
            }
        }
    }
}
