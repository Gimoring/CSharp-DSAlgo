using System;
using System.Collections.Generic;

namespace Exercise
{
    #region Graph
    class Graph
    {

        int[,] adj = new int[6, 6]
        {
            { -1, 15, -1, 35, -1, -1 },
            { 15, -1, 05, 10, -1, -1 },
            { -1, 05, -1, -1, -1, -1 },
            { 35, 10, -1, -1, 05, -1 },
            { -1, -1, -1, 05, -1, 05 },
            { -1, -1, -1, -1, 05, -1 },
        };

        List<int>[] adj2 = new List<int>[]
        {
            // 각 정점, 해당하는 목록
            new List<int>() { 1, 3 },
            new List<int>() { 0, 2, 3 },
            new List<int>() { 1 },
            new List<int>() { 0, 1, 4 },
            new List<int>() { 3, 5 },
            new List<int>() { 4 },
        };
        bool[] visited = new bool[6];
        
        public void Dijikstra(int start)
        {
            bool[] visited = new bool[6];
            int[] distance = new int[6];
            int[] parent = new int[6];
            Array.Fill(distance, Int32.MaxValue);

            distance[start] = 0;
            parent[start] = start;

            while (true)
            {
                // 제일 좋은 후보를 찾는다 (가장 가까이에 있는 후보)

                // 가장 유력한 후보의 거리(clo)와 번호(now)를 저장한다.
                int closest = Int32.MaxValue;
                int now = -1;
                for (int i = 0; i < 6; i++)
                {
                    // 이미 방문한 정점은 스킵
                    if (visited[i])
                        continue;
                    // 아직 발견(예약)된 적이 없거나, 기존 후보보다 멀리 있으면 스킵
                    if (distance[i] == Int32.MaxValue || distance[i] >= closest)
                        continue;
                    // 여태껏 발견한 가장 가까운 후보라는 의미, 정보 갱신
                    closest = distance[i];
                    now = i;
                }

                // 다음 후보가 하나도 없다 -> 종료
                if (now == -1)
                    break;

                // 제일 좋은 후보를 찾았으니까 방문
                visited[now] = true;

                // 방문한 정점과 인접한 정점들을 조사해서
                // 상황에 따라 발견한 최단거리를 갱신한다
                for (int next = 0; next < 6; next++)
                {
                    // 연결되지 않은 정점 스킵
                    if (adj[now, next] == -1)
                        continue;
                    // 이미 방문한 정점 스킵
                    if (visited[next])
                        continue;

                    // 새로 조사된 정점의 최단거리를 계산한다
                    int nextDist = distance[now] + adj[now, next];
                    if (nextDist < distance[next])
                    {
                        distance[next] = nextDist;
                        parent[next] = now;
                    }
                }
            }
        }

        public void BFS(int start)
        {
            bool[] found = new bool[6];
            int[] parent = new int[6];
            int[] distance = new int[6];
            // 예약목록
            Queue<int> q = new Queue<int>();
            q.Enqueue(start); // 예약목록에 삽입
            found[start] = true; // 삽입하면서 찾았다고 전함
            parent[start] = start;
            distance[start] = 0;

            while (q.Count > 0)
            {
                // 예약목록에 있던 곳 방문
                int now = q.Dequeue();
                Console.WriteLine(now);

                // 방문 후 인접한 곳 체크하고 방문하지 않은 곳이면 예약
                for (int next = 0; next <6; next++)
                {
                    if (adj[now, next] == 0) // 인접하지 않았으면 스킵
                        continue;
                    if (found[next]) // 이미 발견한 곳이면 스킵
                        continue;
                    // 방문하지 않은 곳 예약
                    q.Enqueue(next);
                    found[next] = true;
                    parent[next] = now; // ex) 1번의 부모는 0번 ( 0번에서 1번으로 갔다.)
                    distance[next] = distance[now] + 1;
                }
            }
        }
    }
    #endregion

    #region Tree
    class TreeNode<T>
    {
        public T Data { get; set; }
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();

    }
    #endregion

    class PriorityQueue
    {
        List<int> _heap = new List<int>();
        public void Push(int data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입한다.
            _heap.Add(data);

            int now = _heap.Count - 1; // 시작 위치
            // now 부터 도장깨기를 시작
            while (now > 0)
            {
                // 도장깨기를 시도
                int next = (now - 1) / 2; // 나의 부모의 인덱스
                if (_heap[now] < _heap[next]) // 만약 내 부모보다 내가 작다면 
                    break; // 실패. 위로 못 올라감.

                // 두 값을 교체한다.
                int temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 교체를 했으면 검사 위치를 이동한다
                now = next;
            }

        }

        public int Pop()
        {
            // 반환할 데이터를 따로 저장
            int ret = _heap[0];

            // 마지막 데이터를 루트로 이동한다.
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            // 역으로 내려가는 도장깨기 시작
            int now = 0;
            while(true)
            {
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now; // 어디로 갈지 모르겠지만 현재 위치로 설정.
                // 왼쪽값이 현재값보다 크면 왼쪽 이동
                if (left <= lastIndex && _heap[next] < _heap[left]) // (left <= lastIndex) 범위에서 안 벗어나면 && ... 
                    next = left;
                // 오른쪽값이 현재(왼쪽 이동 포함)값보다 크면 오른쪽으로 이동
                if (right <= lastIndex && _heap[next] < _heap[right]) // (right <= lastIndex) 범위에서 안 벗어나면 && ... 
                    next = right;

                // 왼쪽 오른쪽 모두 현재값보다 작으면 종료
                if (next == now)
                    break;

                // 두 값을 교체한다
                int temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;
                // 검사 위치를 이동한다
                now = next;

            }

            return ret;
        }

        public int Count()
        {
            return _heap.Count;
        }
    }

    class Program
    {
        #region TreeMethods
        static TreeNode<string> MakeTree()
        {
            TreeNode<string> root = new TreeNode<string>() { Data = "R1 개발실" };
            {
                {
                    TreeNode<string> node = new TreeNode<string>() { Data = "디자인팀" };
                    //node.Children.Add(new TreeNode<string>() { Data = "전투" });
                    //node.Children.Add(new TreeNode<string>() { Data = "경제" });
                    //node.Children.Add(new TreeNode<string>() { Data = "스토리" });
                    root.Children.Add(node);
                }
                {
                    TreeNode<string> node = new TreeNode<string>() { Data = "프로그래밍팀" };
                    node.Children.Add(new TreeNode<string>() { Data = "서버" });
                    node.Children.Add(new TreeNode<string>() { Data = "클라" });
                    node.Children.Add(new TreeNode<string>() { Data = "엔진" });
                    root.Children.Add(node);
                }
                {
                    TreeNode<string> node = new TreeNode<string>() { Data = "아트팀" };
                    //node.Children.Add(new TreeNode<string>() { Data = "배경" });
                    //node.Children.Add(new TreeNode<string>() { Data = "캐릭터" });
                    root.Children.Add(node);
                }
            }
            return root;
        }

        static void PrintTree(TreeNode<string> root)
        {
            Console.WriteLine(root.Data);

            foreach (TreeNode<string> node in root.Children)
                PrintTree(node);
        }

        static int GetHeight(TreeNode<string> root)
        {
            int height = 0;

            foreach (TreeNode<string> child in root.Children)
            {
                int newHeight = GetHeight(child) + 1;
                if(height < newHeight)
                    height = newHeight;
            }

            return height;
        }
        #endregion
        static void Main(string[] args)
        {
            // 그래프를 순회하는 방법
            // 1. DFS (Depth First Search 깊이 우선 탐색) -> 일단 가고 봄.
            // 2. BFS (Breadth First Search 너비 우선 탐색) -> 주변 있는거 보고 감
            // Graph graph = new Graph();
            // graph.Dijikstra(0);

            // 트리 만들고 높이 구하기
            //TreeNode<string> root = MakeTree();
            //// PrintTree(root);
            //Console.WriteLine(GetHeight(root));

            //GetHeight(root);

            PriorityQueue q = new PriorityQueue();
            q.Push(20);
            q.Push(10);
            q.Push(30);
            q.Push(90);
            q.Push(40);

            while (q.Count() > 0)
            {
                Console.WriteLine(q.Pop());
            }

        }
    }
}
