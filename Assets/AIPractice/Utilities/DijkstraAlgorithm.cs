using System.Collections.Generic;
using CollectItem.Stage;
using UnityEngine;


namespace Algorithm
{
    public class DijkstraAlgorithm
    {
        /// <summary>
        /// ダイクストラ法による最短経路探索
        /// </summary>
        /// <param name="grid">探索マップ</param>
        /// <param name="startPos">スタートポジション</param>
        /// <param name="maxCost">経路の最大コスト</param>
        /// <returns>コストが高いほど遠い位置</returns>
        public static int[,] GetDijkstraMap(int[,] grid, Vector2Int startPos, out int maxCost)
        {
            maxCost = 0;
            int height = grid.GetLength(0);
            int width = grid.GetLength(1);
            int[,] costs = InitializeCostGrid(height, width);

            // 開始地点が配列外なら終了
            if (IsOutOfBounds(startPos, width, height))
                return costs;

            // 開始地点のコストを0にする
            costs[startPos.y, startPos.x] = 0;
            List<Vector2Int> unsearchedNodes = new List<Vector2Int> { startPos };
            Vector2Int currentPos = startPos;

            while (unsearchedNodes.Count > 0)
            {
                currentPos = GetNodeWithMinimumCost(unsearchedNodes, costs);
                List<Vector2Int> canMoveNodes = GetCanMoveNodes(grid, currentPos);
                foreach (var pos in canMoveNodes)
                {
                    int newCost = costs[currentPos.y, currentPos.x] + 1;
                    if (costs[pos.y, pos.x] > newCost)
                    {
                        costs[pos.y, pos.x] = newCost;
                        unsearchedNodes.Add(pos);
                        if (maxCost < newCost)
                            maxCost = newCost;
                    }
                }

                unsearchedNodes.Remove(currentPos);
            }

            return costs;
        }
        
        /// <summary>
        /// ダイクストラ法による最短経路探索
        /// </summary>
        /// <param name="grid">探索マップ</param>
        /// <param name="startPos">スタートポジション</param>
        /// <param name="maxCost">最大コスト</param>
        /// <returns>コストが低いほど遠い位置</returns>
        public static int[,] ReverseCalculateDijkstraMap(int[,] grid, Vector2Int startPos, out int maxCost)
        {
            int [,] ret = GetDijkstraMap(grid, startPos, out maxCost);
            for (int i = 0; i < ret.GetLength(0); i++)
            {
                for (int j = 0; j < ret.GetLength(1); j++)
                {
                    if (ret[i, j] != int.MaxValue)
                    {
                        ret[i, j] = maxCost - ret[i, j];
                    }
                }
            }

            return ret;
        }
        
        /// <summary>
        /// ダイクストラ法による最短経路探索
        /// </summary>
        /// <param name="grid">探索マップ</param>
        /// <param name="startPos">スタートポジション</param>
        /// <returns>正規化されたマップ　コストが低いほど遠い位置</returns>
        public static float[,] ReverseNormalizeDijkstraMap(int[,] grid, Vector2Int startPos)
        {
            int [,] dijkstraMap = ReverseCalculateDijkstraMap(grid, startPos, out int maxCost);
            float[,] ret = GetNormalizeDijkstraMap(dijkstraMap, maxCost);
            return ret;
        }
        
        public static void PrintDijkstraMap(int[,] dijkstraMap)
        {
            int height = dijkstraMap.GetLength(0);
            int width = dijkstraMap.GetLength(1);
            string xStr = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //Debug.Log(dijkstraMap[i, j] + " ");
                    //3桁で表示
                    xStr += dijkstraMap[i, j].ToString("D3") + "   ";
                }
                //改行
                xStr += "\n \n";
            }
            
            Debug.Log(xStr);
        }
        
        public static void PrintNormalizedDijkstraMap(int[,] dijkstraMap, int maxCost)
        {
            float[,] normalizedMap = GetNormalizeDijkstraMap(dijkstraMap, maxCost);
            int height = normalizedMap.GetLength(0);
            int width = normalizedMap.GetLength(1);
            string xStr = "";
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // 2桁で表示
                    //Debug.Log(normalizedMap[i, j].ToString("F2") + "  ");
                    xStr += dijkstraMap[i, j].ToString("F2") + "   ";
                }
                xStr += "\n \n";
            }
            
            Debug.Log(xStr);
        }
        
        /// <summary>
        /// ダイクストラマップを正規化する
        /// </summary>
        /// <param name="dijkstraMap">ダイクストラマップ</param>
        /// <param name="maxCost">最大コスト</param>
        /// <returns>正規化されたダイクストラマップ</returns>
        public static float[,] GetNormalizeDijkstraMap(int[,] dijkstraMap, int maxCost)
        {
            int height = dijkstraMap.GetLength(0);
            int width = dijkstraMap.GetLength(1);
            float[,] normalizedMap = new float[height, width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if(dijkstraMap[i, j] == int.MaxValue)
                        normalizedMap[i, j] = 1f;
                    else
                        //正規化(0.0 ~ 1.0
                        normalizedMap[i, j] = (float)dijkstraMap[i, j] / maxCost;
                }
            }

            return normalizedMap;
        }
        
        /// <summary>
        /// 2つのダイクストラマップを比較し小さい値を代入する
        /// </summary>
        /// <param name="dijkstraMap1">比較するマップ1</param>
        /// <param name="dijkstraMap2">比較するマップ2</param>
        /// <returns>比較されたマップ</returns>
        public static int[,] CompareMinDijkstraMap(int[,] dijkstraMap1, int[,] dijkstraMap2, out int maxCost)
        {
            maxCost = 0;
            int height = dijkstraMap1.GetLength(0);
            int width = dijkstraMap1.GetLength(1);
            int[,] ret = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int value = 0;
                    if(dijkstraMap1[i,j] > dijkstraMap2[i,j])
                        value = dijkstraMap2[i, j];
                    else
                        value = dijkstraMap1[i, j];
                    
                    if (maxCost < value && value != int.MaxValue)
                        maxCost = value;
                    
                    ret[i, j] = value;
                }
            }

            return ret;
        }

        /// <summary>
        /// 2つのダイクストラマップを比較し大きい値を代入する
        /// </summary>
        /// <param name="dijkstraMap1">比較するマップ1</param>
        /// <param name="dijkstraMap2">比較するマップ2</param>
        /// <returns>比較されたマップ</returns>
        public static int[,] CompareMaxDijkstraMap(int[,] dijkstraMap1, int[,] dijkstraMap2, out int maxCost)
        {
            maxCost = 0;
            int height = dijkstraMap1.GetLength(0);
            int width = dijkstraMap1.GetLength(1);
            int[,] ret = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int value = 0;
                    if(dijkstraMap1[i,j] < dijkstraMap2[i,j])
                        value = dijkstraMap2[i, j];
                    else
                        value = dijkstraMap1[i, j];
                    
                    if (maxCost < value && value != int.MaxValue)
                        maxCost = value;
                    
                    ret[i, j] = value;
                }
            }

            return ret;
        }

        
        /// <summary>
        /// 2つのダイクストラマップを合成する
        /// </summary>
        /// <param name="dijkstraMap1">マップ1</param>
        /// <param name="dijkstraMap2">マップ2</param>
        /// <param name="map1Weight">マップ1の重み</param>
        /// <returns></returns>
        public static float[,] SynthesisDijkstraMap(float[,] dijkstraMap1, float[,] dijkstraMap2, float map1Weight)
        {
            float max2Weight = 1 - map1Weight;
            int height = dijkstraMap1.GetLength(0);
            int width = dijkstraMap1.GetLength(1);
            float[,] ret = new float[height, width];
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    // 重み付け
                    ret[i, j] = (dijkstraMap1[i, j] * map1Weight) + (dijkstraMap2[i, j] * max2Weight);
                }
            }

            return ret;
        }
        
        private static int[,] InitializeCostGrid(int height, int width)
        {
            int[,] costs = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    costs[i, j] = int.MaxValue;
                }
            }
            return costs;
        }

        private static bool IsOutOfBounds(Vector2 pos, int width, int height)
        {
            return pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height;
        }

        private static Vector2Int GetNodeWithMinimumCost(List<Vector2Int> nodes, int[,] costs)
        {
            Vector2Int minNode = nodes[0];
            int minCost = costs[minNode.y, minNode.x];
            foreach (var node in nodes)
            {
                if (costs[node.y, node.x] < minCost)
                {
                    minCost = costs[node.y, node.x];
                    minNode = node;
                }
            }
            return minNode;
        }
        
        private static List<Vector2Int> GetCanMoveNodes(int[,] grid, Vector2Int currentPos)
        {
            // この関数は移動可能なノードを返すように実装する
            // 周囲4方向のノードをチェックすることを仮定している
            List<Vector2Int> canMoveNodes = new List<Vector2Int>();
            int[] dx = { 0, 1, 0, -1 };
            int[] dy = { 1, 0, -1, 0 };
            
            int height = grid.GetLength(0);
            int width = grid.GetLength(1);

            for (int i = 0; i < 4; i++)
            {
                int newX = currentPos.x + dx[i];
                int newY = currentPos.y + dy[i];

                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                {
                    //地面がある場合
                    if (grid[newY, newX] != (int)StageObjectType.Wall)
                        canMoveNodes.Add(new Vector2Int(newX, newY));
                }
            }

            return canMoveNodes;
        }
    }
}
