using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSAlgo
{
    class Pos
    {
        public Pos(int y, int x) { Y = y; X = x; }
        public int Y;
        public int X;
    }
    class Player
    {
        public int PosY { get; private set; }
        public int PosX { get; private set; }
        Random _random = new Random();
        Board _board;

        enum Dir
        {
            Up = 0,     // +3 = 3  % 4  = 3
            Left = 1,   // +3 = 4  % 4  = 0
            Down = 2,   // +3 = 5  % 4  = 1
            Right = 3   // +3 = 6  % 4  = 2
        }

        int _dir = (int)Dir.Up;
        List<Pos> _points = new List<Pos>();

        public void Initialize(int posY, int posX, Board board)
        {
            PosY = posY;
            PosX = posX;
            _board = board;

            BFS();
        }

        void BFS()
        {
            // 상하좌우 좌표 
            int[] deltaY = new int[] { -1, 0, 1, 0 };
            int[] deltaX = new int[] { 0, -1, 0, 1 };

            bool[,] found = new bool[_board.Size, _board.Size];
            Pos[,] parent = new Pos[_board.Size, _board.Size];

            Queue<Pos> q = new Queue<Pos>();
            q.Enqueue(new Pos(PosY, PosX));
            found[PosY, PosX] = true;
            parent[PosY, PosX] = new Pos(PosY, PosX); // 처음에는 자기 자신이 부모.(시작점)
            // BFS 탐색
            while (q.Count > 0)
            {
                Pos pos = q.Dequeue();
                int nowY = pos.Y;
                int nowX = pos.X;

                // 현재 좌표를 기준으로 상하좌우 갈 곳있는지 확인
                for (int i = 0; i < 4; i++)
                {
                    int nextY = nowY + deltaY[i];
                    int nextX = nowX + deltaX[i];
                    // 범위 벗어날 경우 예외 처리
                    if (nextX < 0 || nextX >= _board.Size || nextY < 0 || nextY >= _board.Size)
                        continue;
                    if (_board.Tile[nextY, nextX] == Board.TileType.Wall) // 벽이면 스킵
                        continue;
                    if (found[nextY, nextX]) // 이미 발견한 곳이면 스킵
                        continue;

                    q.Enqueue(new Pos(nextY, nextX)); // 다음 갈 곳 예약
                    found[nextY, nextX] = true;
                    // 새로 예약하고 발견한 애(next)의 부모는 현재 Y,X
                    parent[nextY, nextX] = new Pos(nowY, nowX);
                }
            } // 방문한 모든 지점마다 이제 어디서 유래했는지, 부모가 누군지 알 수 있음.

            /**** 최단거리 탐색 ****/
            int y = _board.DestY; // 마지막 점
            int x = _board.DestX; 
            // 부모가 나와 좌표가 똑같다면 시작점 (1, 1)
            while (parent[y, x].Y != y || parent[y, x].X != x) // 마지막부터 처음까지 루프.
            {
                /* 마지막 점부터 시작점까지 */
                _points.Add(new Pos(y, x)); // 나의 좌표기억
                Pos pos = parent[y, x]; // 나의 좌표의 부모좌표 
                y = pos.Y; // 다음에 들어갈 좌표 y에
                x = pos.X; // 다음에 들어갈 좌표 x에
            }
            _points.Add(new Pos(y, x)); // 시작점까지(1,1) 가면 빠져나오므로 여기서.
            _points.Reverse(); // 처음부터 마지막까지가는게 목적이므로 리버스.
        }

        void RightHand()
        {
            // 현재 바라보고 있는 방향을 기준으로, 좌표 변화를 나타낸다.
            int[] frontY = new int[] { -1, 0, 1, 0 }; // 위, 왼, 아래, 오
            int[] frontX = new int[] { 0, -1, 0, 1 };
            // 현재 바라보고 있는 방향을 기준으로, 오른쪽의 좌표
            // Up일 때, 내 오른쪽으로 가는 Y의 좌표는?    0   X의 좌표는?     1           _______
            // LEFT일 때, 내 오른쪽으로 가는 Y의 좌표는?  -1  X의 좌표는?     0           |_|_|_|
            // DOWN일 때, 내 오른쪽으로 가는 Y의 좌표는?  0   X의 좌표는?     -1          |_|o|_|
            // RIGHT일 때, 내 오른쪽으로 가는 Y의 좌표는? 1   X의 좌표는?     0           |_|_|_|
            int[] rightY = new int[] { 0, -1, 0, 1 };
            int[] rightX = new int[] { 1, 0, -1, 0 };

            _points.Add(new Pos(PosY, PosX));

            // 목적지 도착하기 전에는 계속 실행
            while (PosY != _board.DestY || PosX != _board.DestX)
            {
                // 1. 현재 바라보는 방향을 기준으로 오른쪽으로 갈 수 있는지 확인.
                if (_board.Tile[PosY + rightY[_dir], PosX + rightX[_dir]] == Board.TileType.Empty)
                {
                    // 오른쪽 방향으로 90도 회전
                    _dir = (_dir - 1 + 4) % 4;
                    // 앞으로 한 보 전진
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                // 2. 현재 바라보는 방향을 기준으로 전진할 수 있는지 확인.
                else if (_board.Tile[PosY + frontY[_dir], PosX + frontX[_dir]] == Board.TileType.Empty)
                {
                    // 앞으로 한 보 전진.
                    PosY = PosY + frontY[_dir];
                    PosX = PosX + frontX[_dir];
                    _points.Add(new Pos(PosY, PosX));
                }
                else
                {
                    // 왼쪽 방향으로 90도 회전
                    _dir = (_dir + 1 + 4) % 4;
                }
            }
        }

        const int MOVE_TICK = 100; // 밀리세컨드..
        int _sumTick = 0; // 시간세기
        int _lastIndex = 0;
        public void Update(int deltaTick)
        {
            if (_lastIndex >= _points.Count)
                return;

            _sumTick += deltaTick;
            if (_sumTick >= MOVE_TICK)
            {
                _sumTick = 0;

                PosY = _points[_lastIndex].Y;
                PosX = _points[_lastIndex].X;
                _lastIndex++;
            }
        }
    }
}
