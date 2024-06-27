using System.Collections;
using System.Collections.Generic;
using CollectItem.Stage;
using UnityEngine;

namespace Algorithm
{
    public class RadiationEvaluation
    {
        /// <summary>
        /// 2点間の直線上のすべてのピクセル座標を計算する
        /// </summary>
        /// <param name="from"></param>>
        /// <param name="to"></param>
        /// <returns></returns>
        private static List<(int, int)> Bresenham(Vector2Int from, Vector2Int to)
        {
            List<(int, int)> points = new List<(int, int)>();
            int dx = Mathf.Abs(to.x - from.x);
            int dy = Mathf.Abs(to.y - from.y);
            int sx = from.x < to.x ? 1 : -1;
            int sy = from.y < to.y ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                points.Add((from.x, from.y));
                if (from.x == to.x && from.y == to.y)
                    break;
                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    from.x += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    from.y += sy;
                }
            }

            return points;
        }

        /// <summary>
        /// キャラクターの位置から視線の終点までの直線上に障害物がないかを確認
        /// </summary>
        /// <param name="grid">グリッド</param>
        /// <param name="start">始点</param>
        /// <param name="end">終点</param>
        /// <returns>障害物がない場合:TRUE ある場合:FALSE</returns>
        static void IsLineOfSightClear(int[,] grid, Vector2Int start, Vector2Int end, ref int[,] results)
        {
            List<(int, int)> linePoints = Bresenham(start, end);

            foreach ((int x, int y) point in linePoints)
            {
                if (grid[point.y, point.x] == (int)StageObjectType.Wall)  // 障害物がある場合
                    return;
                
                results[point.y, point.x] = 1;
            }
        }

        public static int[,] EvaluateAllDirections(int[,] grid, Vector2Int characterPosition, int radius)
        {
            int height = grid.GetLength(0);
            int width = grid.GetLength(1);
            int[,] results = new int[height, width];
            int cx = characterPosition.x;
            int cy = characterPosition.y;

            for (int angle = 0; angle < 360; angle++)
            {
                float rad = Mathf.PI * angle / 180.0f;
                int dx = (int)(radius * Mathf.Cos(rad));
                int dy = (int)(radius * Mathf.Sin(rad));
                int end_x = cx + dx;
                int end_y = cy + dy;

                //視線の終点がマップの範囲内か確認
                if(end_x < 0) end_x = 0;
                if(end_y < 0) end_y = 0;
                if (end_x >= width) end_x = width - 1;
                if(end_y >= height) end_y = height - 1;

                Vector2Int start = new Vector2Int(cx, cy);
                Vector2Int end = new Vector2Int(end_x, end_y);
                IsLineOfSightClear(grid, start, end, ref results);
            }

            return results;
        }
    }
}
