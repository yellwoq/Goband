using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// 分数评估
/// </summary>
public sealed class EvalateConst
{
    public const int WIN = 10000;
    //黑棋为o，白棋为x, 从黑棋出发
    //死二 xoox
    public const int DEEP_DEATH2 = 2;

    //低级死二 xoo
    public const int LOWER_DEATH2 = 4;
    //死三 xooox
    public const int DEEP_DEATH3 = 3;
    //低级死三 xooo
    public const int LOWER_DEATH3 = 6;

    //死四 xoooox
    public const int DEEP_DEATH4 = 4;
    //低级死四 xoooo
    public const int LOWER_DEATH4 = 32;

    //活二 oo
    public const int ALIVE2 = 10;
    //跳活二 o o
    public const int JUMP_ALIVE2 = 2;
    //活三 ooo
    public const int ALIVE3 = 100;
    //跳活三 oo o
    public const int JUMP_ALIVE3 = 10;
    //活四 oooo
    public const int ALIVE4 = 5000;
    //跳活四 （1跳3或者3跳1或2跳2） o ooo || ooo o || oo oo
    public const int JUMP_ALIVE4 = 90;
}

/// <summary>
/// 缓冲（记录最近几次的棋子状态）
/// </summary>
/// <typeparam name="V">棋子偏移</typeparam>
public class ChessBufferMap
{
    //设置缓冲区为3
    private int maxCount = 3;
    public ChessBufferMap(int maxCount)
    {
        this.maxCount = maxCount;
    }

    private Dictionary<int, ChessPos> buffer = new Dictionary<int, ChessPos>();

    public ChessPos this[int key] {
        get { return buffer[key]; }
        set { buffer[key] = value; }
    
    }


    // 添加元素（key存的是每个棋子的分数，value是每个棋子的offset）
    public void Put(int key, ChessPos value)
    {
        if(!buffer.ContainsKey(key))
        {
            buffer[key] = value;
        }
        buffer[key] = value;
        _CheckSize();
    }

    // 批量添加元素
    public void PutAll(ChessBufferMap map)
    {
        foreach(var entry in map.buffer)
        {
            Put(entry.Key, entry.Value);
        }
    }

    // 检查并缩减缓冲区大小
    void _CheckSize()
    {
        //将缓冲区的所有键转换成列表，并赋值给变量 list，按照从大到小排列
        var list = buffer.Keys.ToList();
        list.Sort((a, b) => {
            return b.CompareTo(a);
        });
        while (buffer.Count > maxCount)
        {
            buffer.Remove(list.Last());
        }
    }

    // 将缓冲区转为Map
    public Dictionary<int, ChessPos> ToMap()
    {
        return buffer;
    }

    // 获取所有元素的值
    IEnumerable<ChessPos> Values()
    {
        return buffer.Values;
    }

    public List<ChessPos> ToList()
    {
        return buffer.Values.ToList();
    }

    // 获取缓存元素个数
    public int Size()
    {
        return buffer.Count;
    }

    // 转为字符串表示
    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append("{");
        var keys = buffer.Keys.ToList();
        keys.Sort((a, b) => {
            return b.CompareTo(a);
        });

        foreach (var i in keys)
        {
            sb.Append($"[{i} , {buffer[i]}] ,");
        }

        return $"{sb.ToString().Substring(0, sb.ToString().Length - 2)}";
     }

    // 获取第一个元素的值
    public ChessPos GetFirst()
    {
        var keys = buffer.Keys.ToList();
        keys.Sort((a, b) => {
            return b.CompareTo(a);
        });
        return buffer[keys.First()];
    }

    // 获取键的最小值
    public int MinKey()
    {
        if (buffer.Count==0)
        {
            return int.MinValue;
        }
        var list = buffer.Keys.ToList();
        list.Sort((a, b) => {
            return b.CompareTo(a);
        });
        return list.Last();
    }

    // 获取键值最小的元素
    public ChessPos Min()
    {
        int min_key = MinKey();
        if (!buffer.ContainsKey(min_key))
        {
            return ChessPos.none;
        }
        return buffer[min_key];
    }

    // 获取所有键的列表
    public List<int> GetkeySet()
    {
        if (buffer.Count==0) return new List<int>();

        var sortedKeys = buffer.Keys.ToList();
        sortedKeys.Sort((a, b) => {
            return (b - a);
        });

        return sortedKeys;
    }

    // 获取键的最大值
    // 最优位置得分
    public int MaxKey()
    {
        if (buffer.Count==0)
        {
            return int.MinValue;
        }
        var list = buffer.Keys.ToList();
        list.Sort((a, b) => {
            return b.CompareTo(a);
        });
        return list.First();
      }


    // 获取键值最大的元素
    public ChessPos Max()
    {
        int max_key = MaxKey();
        if (!buffer.ContainsKey(max_key))
        {
            return ChessPos.none;
        }
        return buffer[max_key];
    }
}

public class BufferChessmanList
{
    List<ChessInfo> buffer = new List<ChessInfo>();
    int maxCount = 5;

    public void Add(ChessInfo chessman)
    {
        buffer.Add(chessman);
        _checkSize();
    }

    public BufferChessmanList(int maxCount)
    {
        this.maxCount = maxCount;
    }

    void _checkSize()
    {
        buffer.Sort((a, b) => {
            return b.score - a.score;
        });

        while (buffer.Count > maxCount)
        {
            buffer.Remove(buffer.Last());
        }
    }

    public List<ChessPos> ToList()
    {
        List<ChessPos> list = new List<ChessPos>();
        foreach (ChessInfo c in buffer)
        {
            list.Add(c.chess_pos);
        }
        return list;
    }

    public List<int> ToScoreList()
    {
        List<int> list = new List<int>();
        foreach (ChessInfo c in buffer)
        {
            list.Add(c.score);
        }
        return list;
    }

    public int MinScore()
    {
        if (buffer.Count == 0)
        {
            return int.MinValue;
        }
        buffer.Sort((a, b) => {
            return b.score - a.score;
        });
        return buffer.Last().score;
    }
}

/// <summary>
/// 用来判断是那种棋局情况
/// </summary>
public static class CheckCurrentStateHelper
{

    #region 棋盘状态判断
    /// <summary>
    /// 死二 xoo
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsLowerDeath2(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 2) return false;
        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[0], list[1]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) ||
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// 活二 oo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsAlive2(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 2) return false;
        //把两颗棋子传入
        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[0], list[1]);

        return IsEffectivePosition(offset1) &&
            IsEffectivePosition(offset2) &&
            IsBlankPosition(board, offset1) &&
            IsBlankPosition(board, offset2);
    }

    /// <summary>
    /// 死三 xooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsLowerDeath3(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 3) return false;
        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[1], list[2]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) ||
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// 活三 ooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsAlive3(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 3) return false;

        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[1], list[2]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) &&
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// 死四 xoooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsLowerDeath4(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 4) return false;

        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[2], list[3]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) ||
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// 活四 oooo
    /// </summary>
    /// <param name="list"></param>
    /// <returns></returns>
    static bool IsAlive4(ChessBoard board, List<ChessPos> list)
    {
        if (list.Count != 4) return false;

        ChessPos offset1 = NextChessman(list[1], list[0]);
        ChessPos offset2 = NextChessman(list[2], list[3]);
        return (IsEffectivePosition(offset1) && IsBlankPosition(board, offset1)) &&
            (IsEffectivePosition(offset2) && IsBlankPosition(board, offset2));
    }

    /// <summary>
    /// 获取跳活2数量
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    static int GetJumpAlive2Count(ChessBoard board, List<ChessPos> list, ChessType chessType)
    {
        if (list.Count != 1)
        {
            return 0;
        }
        int count = 0;

        if (list.First().x >= 3)
        {
            //边界， 水平方向， 左
            ChessPos left = new ChessPos(list.First().x - 2, list.First().y);
            count += ExistSpecificChessman(board, left, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() {
                new ChessPos(list.First().x + 1, list.First().y),
                new ChessPos(left.x - 1, left.y)
            }) ? 1 : 0;
        }

        int LINE_COUNT = board.board_y_size;
        if (list.First().x <= LINE_COUNT - 4)
        {
            //边界， 水平方向， 右
            ChessPos right = new ChessPos(list.First().x + 2, list.First().y);
            count += ExistSpecificChessman(board, right, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x - 1, list.First().y),
                new ChessPos(right.x + 1, right.y)
            }) ? 1 : 0;
        }

        if (list.First().y >= 3)
        {
            //边界， 垂直方向， 上
            ChessPos top = new ChessPos(list.First().x, list.First().y - 2);
            count += ExistSpecificChessman(board, top, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() {
                new ChessPos(list.First().x, list.First().y + 1),
                new ChessPos(top.x, top.y - 1)
            }) ? 1 : 0;
        }

        int COL_COUNT = board.board_x_size;
        if (list.First().y <= COL_COUNT - 4)
        {
            //边界， 垂直方向， 下
            ChessPos bottom = new ChessPos(list.First().x, list.First().y + 2);
            count += ExistSpecificChessman(board, bottom, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() {
                new ChessPos(list.First().x, list.First().y - 1),
                new ChessPos(bottom.x, bottom.y + 1)
            }) ? 1 : 0;
        }

        if (list.First().x >= 3 && list.First().y >= 3)
        {
            //边界， 左上方向， 上
            ChessPos leftTop = new ChessPos(list.First().x - 2, list.First().y - 2);
            count += ExistSpecificChessman(board, leftTop, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x + 1, list.First().y + 1),
                new ChessPos(leftTop.x - 1, leftTop.y - 1)
            }) ? 1 : 0;
        }

        if (list.First().x >= 3 && list.First().y <= COL_COUNT - 4)
        {
            //边界， 左下方向， 下
            ChessPos leftBottom = new ChessPos(list.First().x - 2, list.First().y + 2);
            count += ExistSpecificChessman(board, leftBottom, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x + 1, list.First().y - 1),
                new ChessPos(leftBottom.x - 1, leftBottom.y + 1)
            }) ? 1 : 0;
        }

        if (list.First().x <= LINE_COUNT - 4 && list.First().y >= 3)
        {
            //边界， 右上方向， 上
            ChessPos rightTop = new ChessPos(list.First().x + 2, list.First().y - 2);
            count += ExistSpecificChessman(board, rightTop, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x - 1, list.First().y + 1),
                new ChessPos(rightTop.x + 1, rightTop.y - 1)
            }) ? 1 : 0;
        }

        if (list.First().x <= LINE_COUNT - 4 && list.First().y <= LINE_COUNT - 4)
        {
            //边界， 右下方向， 下
            ChessPos rightBottom = new ChessPos(list.First().x + 2, list.First().y + 2);
            count += ExistSpecificChessman(board, rightBottom, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>(){
                new ChessPos(list.First().x - 1, list.First().y - 1),
                new ChessPos(rightBottom.x + 1, rightBottom.y + 1)
            }) ? 1 : 0;
        }
        return count;
    }

    /// <summary>
    /// 获取跳活3数量
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    static int GetJumpAlive3Count(ChessBoard board, List<ChessPos> list, ChessType chessType)
    {
        if (list.Count != 1 && list.Count != 2)
            return 0;
        int count = 0;
        if (list.Count == 1)
        {
            //1跳2 活3
            /// leftBlank left2 left1 blank list.First() rightBlank
            if (list.First().x >= 4)
            {
                //棋盘边界, 水平， 左
                ChessPos left1 = new ChessPos(list.First().x - 2, list.First().y);
                ChessPos left2 = new ChessPos(left1.x - 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos leftBlank = new ChessPos(left2.x - 1, list.First().y);
                ChessPos rightBlank = new ChessPos(list.First().x + 1, list.First().y);

                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { left1, left2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { blank, leftBlank, rightBlank })
                ? 1 : 0;
            }

            int LINE_COUNT = board.board_y_size;
            ///leftBlank list.First()  blank right1  right2 rightBlank
            if (list.First().x <= LINE_COUNT - 5)
            {
                //棋盘边界, 水平， 右
                ChessPos leftBlank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y);
                ChessPos right1 = new ChessPos(blank.x + 1, blank.y);
                ChessPos right2 = new ChessPos(right1.x + 1, blank.y);
                ChessPos rightBlank = new ChessPos(right2.x + 1, blank.y);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { right1, right2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBlank, blank, rightBlank })
                ? 1 : 0;
            }

            /// topBlank
            /// top2
            /// top1
            /// blank
            /// list.First()
            /// bottomBlank

            if (list.First().y >= 4)
            {
                //棋盘边界, 垂直方向， 上
                ChessPos blank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos top1 = new ChessPos(list.First().x, blank.y - 1);
                ChessPos top2 = new ChessPos(list.First().x, top1.y - 1);
                ChessPos topBlank = new ChessPos(list.First().x, top2.y - 1);
                ChessPos bottomBlank = new ChessPos(list.First().x, list.First().y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { top1, top2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { topBlank, blank, bottomBlank })
                ? 1 : 0;
            }

            int COL_COUNT = board.board_x_size;
            /// topBlank
            /// list.First()
            /// blank
            /// top1
            /// top2
            /// bottomBlank
            if (list.First().y <= COL_COUNT - 5)
            {
                //棋盘边界, 垂直方向， 下
                ChessPos topBlank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x, list.First().y + 1);
                ChessPos top1 = new ChessPos(list.First().x, blank.y + 1);
                ChessPos top2 = new ChessPos(list.First().x, top1.y + 1);
                ChessPos bottomBlank = new ChessPos(list.First().x, top2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { top1, top2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { topBlank, blank, bottomBlank })
                ? 1 : 0;
            }

            ///左上
            /// |leftTopBlank
            /// |          leftTop2
            /// |                   leftTop1
            /// |                           blank
            /// |                                 list.First()
            /// |                                          rightBottomBlank
            if (list.First().x >= 4 && list.First().y >= 4)
            {
                //棋盘边界, 左上方向， 上
                ChessPos rightBottomBlank = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos leftTop1 = new ChessPos(blank.x - 1, blank.y - 1);
                ChessPos leftTop2 = new ChessPos(leftTop1.x - 1, leftTop1.y - 1);
                ChessPos leftTopBlank = new ChessPos(leftTop2.x - 1, leftTop2.y - 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { leftTop1, leftTop2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightBottomBlank, blank, leftTopBlank })
            ? 1 : 0;
            }

            ///左下
            ///  |                                                 rightTopBlank1
            ///  |                                       list.First()
            ///  |                                   blank
            ///  |                        leftBottom1
            ///  |            leftBottom2
            ///  |leftBottomBlank
            if (list.First().x >= 4 && list.First().y <= COL_COUNT - 5)
            {
                ChessPos rightTopBlank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos leftBottom1 = new ChessPos(blank.x - 1, blank.y + 1);
                ChessPos leftBottom2 = new ChessPos(leftBottom1.x - 1, leftBottom1.y + 1);
                ChessPos leftBottomBlank = new ChessPos(leftBottom2.x - 1, leftBottom2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { leftBottom1, leftBottom2 }, chessType) &&
                        IsAllBlankPosition(board, new List<ChessPos>() { rightTopBlank, blank, leftBottomBlank })
                ? 1 : 0;
            }

            ///右上
            ///                                             rightTopBlank|
            ///                                     rightTop2            |
            ///                             rightTop1                    |
            ///                         blank                            |
            ///               list.First()                                 |
            /// leftBottomBlank                                          |
            if (list.First().x <= LINE_COUNT - 5 && list.First().y >= 4)
            {
                ChessPos leftBottomBlank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos rightTop1 = new ChessPos(blank.x - 1, blank.y + 1);
                ChessPos rightTop2 = new ChessPos(rightTop1.x - 1, rightTop1.y + 1);
                ChessPos rightTopBlank = new ChessPos(rightTop2.x - 1, rightTop2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { rightTop1, rightTop2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBottomBlank, blank, rightTopBlank })
                ? 1 : 0;
            }

            ///右下
            /// leftTopBlank                                       |
            ///             list.First()                             |
            ///                       blank                        |
            ///                          leftTop1                  |
            ///                                 leftTop2           |
            ///                                        rightBottom |
            ///
            if (list.First().x <= LINE_COUNT - 4 && list.First().y <= COL_COUNT - 4)
            {
                ChessPos leftTopBlank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos leftTop1 = new ChessPos(blank.x + 1, blank.y + 1);
                ChessPos leftTop2 = new ChessPos(leftTop1.x + 1, leftTop1.y + 1);
                ChessPos rightBottom = new ChessPos(leftTop2.x + 1, leftTop2.y + 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { leftTop1, leftTop2 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftTopBlank, blank, rightBottom })
                ? 1 : 0;
            }
        }
        else if (list.Count == 2)
        {
            //2跳1 活3
            /// next1Next1Blank next1 next1blank list[0] list[1] next2Blank next2 next2Next2Blank
            ChessPos next1blank = NextChessman(list[1], list[0]);
            ChessPos next1 = NextChessman(list[0], next1blank);
            ChessPos next1Next1Blank = NextChessman(next1blank, next1);
            ChessPos next2Blank = NextChessman(list[0], list[1]);
            ChessPos next2 = NextChessman(list[1], next2Blank);
            ChessPos next2Next2Blank = NextChessman(next2Blank, next2);

            count += ExistSpecificChessman(board, next1, chessType) &&
                    IsAllBlankPosition(board, new List<ChessPos>() { next1Next1Blank, next1blank, next2Blank })
          ? 1 : 0;
            count += ExistSpecificChessman(board, next2, chessType) &&
                    IsAllBlankPosition(board, new List<ChessPos>() { next1blank, next2Blank, next2Next2Blank })
          ? 1 : 0;
        }
        return count;
    }

    /// <summary>
    /// 获取跳活4数量
    /// </summary>
    /// <param name="board"></param>
    /// <param name="list"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    static int GetJumpAlive4Count(ChessBoard board, List<ChessPos> list, ChessType chessType)
    {
        if (list.Count <= 0 || list.Count >= 4)
        {
            return 0;
        }
        int count = 0;
        int LINE_COUNT = board.board_y_size;
        int COL_COUNT = board.board_x_size;

        if (list.Count == 1)
        {
            ///左
            ///leftBlank left3 left2 left1 blank list.First() rightBlank
            if (list.First().x >= 5)
            {
                ChessPos rightBlank = new ChessPos(list.First().x + 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos left1 = new ChessPos(blank.x - 1, list.First().y);
                ChessPos left2 = new ChessPos(left1.x - 1, list.First().y);
                ChessPos left3 = new ChessPos(left2.x - 1, list.First().y);
                ChessPos leftBlank = new ChessPos(left3.x - 1, list.First().y);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { left1, left2, left3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightBlank, blank, leftBlank })
                ? 1 : 0;
            }

            ///右
            ///leftBlank list.First() blank right1 right2 right3 rightBlank
            if (list.First().x <= LINE_COUNT - 6)
            {
                ChessPos leftBlank = new ChessPos(list.First().x - 1, list.First().y);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y);
                ChessPos right1 = new ChessPos(blank.x + 1, blank.y);
                ChessPos right2 = new ChessPos(right1.x + 1, blank.y);
                ChessPos right3 = new ChessPos(right2.x + 1, blank.y);
                ChessPos rightBlank = new ChessPos(right3.x + 1, blank.y);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { right1, right2, right3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBlank, blank, rightBlank })
                ? 1 : 0;
            }

            ///上
            /// topBlank
            /// top3
            /// top2
            /// top1
            /// blank
            /// list.First()
            /// bottomBlank
            if (list.First().y >= 5)
            {
                ChessPos bottomBlank = new ChessPos(list.First().x, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos top1 = new ChessPos(blank.x, blank.y - 1);
                ChessPos top2 = new ChessPos(top1.x, blank.y - 1);
                ChessPos top3 = new ChessPos(top2.x, blank.y - 1);
                ChessPos topBlank = new ChessPos(top3.x, blank.y - 1);
                count += ExistSpecificChessmanAll(board, new List<ChessPos>() { top1, top2, top3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { bottomBlank, blank, topBlank })
                ? 1 : 0;
            }

            /// 下
            /// topBlank
            /// list.First()
            /// blank
            /// bottom1
            /// bottom2
            /// bottom3
            /// bottomBlank
            if (list.First().y <= LINE_COUNT - 5)
            {
                ChessPos topBlank = new ChessPos(list.First().x, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x, list.First().y + 1);
                ChessPos bottom1 = new ChessPos(blank.x, blank.y + 1);
                ChessPos bottom2 = new ChessPos(bottom1.x, bottom1.y + 1);
                ChessPos bottom3 = new ChessPos(bottom2.x, bottom2.y + 1);
                ChessPos bottomBlank = new ChessPos(bottom3.x, bottom3.y + 1);
                count +=
                ExistSpecificChessmanAll(board, new List<ChessPos>() { bottom1, bottom2, bottom3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { topBlank, blank, bottomBlank })
                ? 1 : 0;
            }

            /// 左上
            /// leftTopBlank
            ///             leftTop3
            ///                    leftTop2
            ///                          leftTop1
            ///                                 blank
            ///                                     list.First()
            ///                                             rightBottom

            if (list.First().x >= 5 && list.First().y >= 5)
            {
                ChessPos rightBottom = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos leftTop1 = new ChessPos(blank.x - 1, blank.y - 1);
                ChessPos leftTop2 = new ChessPos(leftTop1.x - 1, leftTop1.y - 1);
                ChessPos leftTop3 = new ChessPos(leftTop2.x - 1, leftTop2.y - 1);
                ChessPos leftTopBlank = new ChessPos(leftTop3.x - 1, leftTop3.y - 1);
                count +=
                ExistSpecificChessmanAll(board, new List<ChessPos>() { leftTop1, leftTop2, leftTop3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightBottom, blank, leftTopBlank })
                ? 1 : 0;
            }

            ///左下
            ///                                                 rightTopBlank
            ///                                           list.First()
            ///                                       blank
            ///                              leftBottom1
            ///                       leftBottom2
            ///               leftBottom3
            /// leftBottomBlank
            if (list.First().x >= 5 && list.First().y <= LINE_COUNT - 5)
            {
                ChessPos rightTopBlank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos leftBottom1 = new ChessPos(blank.x - 1, blank.y + 1);
                ChessPos leftBottom2 = new ChessPos(leftBottom1.x - 1, leftBottom1.y + 1);
                ChessPos leftBottom3 = new ChessPos(leftBottom2.x - 1, leftBottom2.y + 1);
                ChessPos leftBottomBlank = new ChessPos(leftBottom3.x - 1, leftBottom3.y + 1);
                count += ExistSpecificChessmanAll(
                board, new List<ChessPos>() { leftBottom1, leftBottom2, leftBottom3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { rightTopBlank, blank, leftBottomBlank })
                ? 1 : 0;
            }

            /// 右上
            ///                                             rightTopBlank
            ///                                       rightTop3
            ///                                 rightTop2
            ///                         rightTop1
            ///                     blank
            ///             list.First()
            /// leftBottomBlank
            if (list.First().x <= LINE_COUNT - 5 && list.First().y >= 5)
            {
                ChessPos leftBottomBlank = new ChessPos(list.First().x - 1, list.First().y + 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y - 1);
                ChessPos rightTop1 = new ChessPos(blank.x + 1, blank.y - 1);
                ChessPos rightTop2 = new ChessPos(rightTop1.x + 1, rightTop1.y - 1);
                ChessPos rightTop3 = new ChessPos(rightTop2.x + 1, rightTop2.y - 1);
                ChessPos rightTopBlank = new ChessPos(rightTop3.x + 1, rightTop3.y - 1);
                count += ExistSpecificChessmanAll(
                board, new List<ChessPos>() { rightTop1, rightTop2, rightTop3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftBottomBlank, blank, rightTopBlank })
                ? 1 : 0;
            }

            /// 右下
            /// leftTopBlank
            ///           list.First()
            ///                    blank
            ///                        rightBottom1
            ///                               rightBottom2
            ///                                      rightBottom3
            ///                                            rightBottomBlank
            if (list.First().x <= LINE_COUNT - 5 && list.First().y <= LINE_COUNT - 5)
            {
                ChessPos leftTopBlank = new ChessPos(list.First().x - 1, list.First().y - 1);
                ChessPos blank = new ChessPos(list.First().x + 1, list.First().y + 1);
                ChessPos rightBottom1 = new ChessPos(blank.x + 1, blank.y + 1);
                ChessPos rightBottom2 = new ChessPos(rightBottom1.x + 1, rightBottom1.y + 1);
                ChessPos rightBottom3 = new ChessPos(rightBottom2.x + 1, rightBottom2.y + 1);
                ChessPos rightBottomBlank =
                    new ChessPos(rightBottom3.x + 1, rightBottom3.y + 1);
                count += ExistSpecificChessmanAll(
                board, new List<ChessPos>() { rightBottom1, rightBottom2, rightBottom3 }, chessType) &&
                IsAllBlankPosition(board, new List<ChessPos>() { leftTopBlank, blank, rightBottomBlank })
                ? 1 : 0;
            }
        }
        else if (list.Count == 2)
        {
            //2跳2
            /// next2Blank next2 next1 next1Blank list[0] list[1]  next3Blank next3 next4 next4Blank
            ChessPos next1Blank = NextChessman(list[1], list[0]);
            ChessPos next1 = NextChessman(list[0], next1Blank);
            ChessPos next2 = NextChessman(next1Blank, next1);
            ChessPos next2Blank = NextChessman(next1, next2);
            ChessPos next3Blank = NextChessman(list[0], list[1]);
            ChessPos next3 = NextChessman(list[1], next3Blank);
            ChessPos next4 = NextChessman(next3Blank, next3);
            ChessPos next4Blank = NextChessman(next3, next4);

            count += ExistSpecificChessmanAll(board, new List<ChessPos>() { next2, next1 }, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next2Blank, next1Blank, next3Blank })
            ? 1 : 0;
            count += ExistSpecificChessmanAll(board, new List<ChessPos>() { next3, next4 }, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next1Blank, next3Blank, next4Blank })
            ? 1 : 0;
        }
        else if (list.Count == 3)
        {
            //3跳1
            ///next1Next1Blank next1 next1Blank list[0] list[1] list[2] next2Blank next2 next2Next2Blank
            ChessPos next1Blank = NextChessman(list[1], list[0]);
            ChessPos next1 = NextChessman(list[0], next1Blank);
            ChessPos next1Next1Blank = NextChessman(next1Blank, next1);
            ChessPos next2Blank = NextChessman(list[1], list[2]);
            ChessPos next2 = NextChessman(list[2], next2Blank);
            ChessPos next2Next2Blank = NextChessman(next2Blank, next2);

            count += ExistSpecificChessman(board, next1, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next1Next1Blank, next1Blank, next2Blank })
            ? 1 : 0;
            count += ExistSpecificChessman(board, next2, chessType) &&
            IsAllBlankPosition(board, new List<ChessPos>() { next1Blank, next2Blank, next2Next2Blank })
            ? 1 : 0;
        }
        return count;
    }
    #endregion

    //输入的first和second返回下一个棋子的位置偏移量。
    static ChessPos NextChessman(ChessPos first, ChessPos second)
    {
        //检查first和second的y值是否相等。
        //如果相等，表示棋子在水平方向上移动。那么下一个棋子的位置偏移量将在水平方向上向右或向左移动一格，取决于first的x是否大于second的x。
        //如果first.x > second.x，则向左移动一格，即second.x - 1；否则，向右移动一格，即second.x + 1。纵坐标保持不变，即为first.y
        if (first.y == second.y)
        {
            return new ChessPos(
                first.x > second.x ? second.x - 1 : second.x + 1, first.y);
        }
        //如果first.x和second.x相等，表示棋子在垂直方向上移动。那么下一个棋子的位置偏移量将在垂直方向上向上或向下移动一格，
        //取决于first的y是否大于second的y。如果first.y > second.y，则向上移动一格，即second.y - 1；否则，向下移动一格，
        //即second.y + 1。横坐标保持不变，即为first.x。
        //如果以上两种情况都不满足，那么表示棋子在斜对角线方向上移动。根据first.x和second.x的大小关系，
        //以及first.y和second.y的大小关系，决定下一个棋子的位置偏移量。
        else if (first.x == second.x)
        {
            return new ChessPos(
                first.x, first.x > second.y ? second.y - 1 : second.y + 1);
        }
        else if (first.x > second.x)
        {
            // 左下角
            if (first.y > second.y)
            {
                return new ChessPos(second.x - 1, second.x - 1);
            }
            //左上角
            else
            {
                return new ChessPos(second.x - 1, second.y + 1);
            }
        }
        else
        {
            //右下角
            if (first.y > second.y)
            {
                return new ChessPos(second.x + 1, second.y - 1);
            }
            //右上角
            else
            {
                return new ChessPos(second.x + 1, second.y + 1);
            }
        }
    }

    //判断该位置是否有效。
    public static bool IsEffectivePosition(ChessPos offset)
    {
        return offset != ChessPos.none;
    }

    static bool ExistSpecificChessmanAll(ChessBoard board, List<ChessPos> positions, ChessType chessType)
    {
        if (positions.Count == 0)
        {
            return false;
        }

        bool flag = true;
        foreach (ChessPos of in positions)
        {
            flag &= ExistSpecificChessman(board, of, chessType);
        }
        return flag;
    }

    // 检查给定位置是否存在特定的棋子
    public static bool ExistSpecificChessman(ChessBoard board, ChessPos position, ChessType chessType)
    {
        if (!board.chess_map_dic.ContainsKey(position))
        { return false; }

        string entity_id = board.chess_map_dic[position];
        bool has_entity;
        Chess chess = ChessFactory.Instance.GetEntity<Chess>(entity_id, out has_entity);

        return chess && chess.chess_type == chessType;
    }

    static bool IsAllBlankPosition(ChessBoard board, List<ChessPos> list)
    {
        foreach (ChessPos o in list)
        {
            if (!IsBlankPosition(board, o))
            {
                return false;
            }
        }
        return true;
    }

    //isBlankPosition是用于判断某个位置上是否没有棋子，写法逻辑和用户交互能否落子差不多
    static bool IsBlankPosition(ChessBoard board, ChessPos position)
    {
        return !board.chess_map_dic.ContainsKey(position);
    }

    //将给定的数限制在最大值为2的范围内
    static int LimitMax(int num)
    {
        return num >= 2 ? 2 : num;
    }

    /// <summary>
    /// 对每一种情况进行估分
    /// </summary>
    /// <param name="first"></param>
    /// <param name="myChessman"></param>
    /// <param name="chessType"></param>
    /// <returns></returns>
    public static int GetScoring(ChessBoard board, List<ChessPos> myChessman, ChessType chessType) {
        if (myChessman.Count >= 5)
        {
            return EvalateConst.WIN;
        }
        int score = 0;
        switch (myChessman.Count)
        {
            case 1:
                break;
            case 2:
                if (IsAlive2(board, myChessman))
                {
                    score += EvalateConst.ALIVE2;
                    score +=
                        LimitMax(GetJumpAlive3Count(board, myChessman, chessType)) * EvalateConst.JUMP_ALIVE3;
                    score +=
                        LimitMax(GetJumpAlive4Count(board, myChessman, chessType)) * EvalateConst.JUMP_ALIVE4;

                    Debug.Log("$printMsg 活2成立, 得分+$ALIVE2");
                    
                }
                else if (IsLowerDeath2(board, myChessman))
                {
                    score += EvalateConst.LOWER_DEATH2;
                    Debug.Log("$printMsg 低级死2成立 ,得分+$LOWER_DEATH2");
                }
                else
                {
                    score += EvalateConst.DEEP_DEATH2;
                    Debug.Log("$printMsg 死2成立 ,得分+$DEEP_DEATH2");
                }
                break;
            case 3:
                if (IsAlive3(board, myChessman))
                {
                    score += EvalateConst.ALIVE3;
                    score +=
                        LimitMax(GetJumpAlive4Count(board, myChessman, chessType)) * EvalateConst.JUMP_ALIVE4;
                    Debug.Log("$printMsg 活3成立, 得分+$ALIVE3");
                }
                else if (IsLowerDeath3(board, myChessman))
                {
                    score += EvalateConst.LOWER_DEATH3;
                    Debug.Log("$printMsg 低级死3成立 ,得分+$LOWER_DEATH3");
                }
                else
                {
                    score += EvalateConst.DEEP_DEATH3;
                    Debug.Log("$printMsg 死3成立 ,得分+$DEEP_DEATH3");
                }
                break;

            case 4:
                if (IsAlive4(board, myChessman))
                {
                    score += EvalateConst.ALIVE4;
                    Debug.Log("$printMsg 活4成立, 得分+$ALIVE4");
                }
                else if (IsLowerDeath4(board, myChessman))
                {
                    score += EvalateConst.LOWER_DEATH4;
                    Debug.Log("$printMsg 低级死4成立 ,得分+$LOWER_DEATH4");
                }
                else
                {
                    score += EvalateConst.DEEP_DEATH4;
                    Debug.Log("$printMsg 死4成立 ,得分+$DEEP_DEATH4");
                }
                break;

            case 5:
            default:
                score += EvalateConst.WIN;
                break;
        }
        return score;
    }


    /// <summary>
    /// 孤子价值(检查四周）
    /// </summary>
    /// <param name="offset"></param>
    /// <returns></returns>
    public static int GetAlongScoring(ChessBoard board, ChessPos offset, ChessType chessType)
    {
        int score = 0;
        List<ChessPos> list = new List<ChessPos>()
        { 
          new ChessPos(offset.x - 1, offset.y),
          new ChessPos(offset.x + 1, offset.y),
          new ChessPos(offset.x, offset.y + 1),
          new ChessPos(offset.x, offset.y - 1),
          new ChessPos(offset.x - 1, offset.y - 1),
          new ChessPos(offset.x - 1, offset.y + 1),
          new ChessPos(offset.x + 1, offset.y - 1),
          new ChessPos(offset.x + 1, offset.y + 1),

        };
        foreach(var pos in list)
        {
            if (IsEffectivePosition(pos) && IsBlankPosition(board, pos))
            {
                score++;
            }
        }

        return score + PositionScore(board, offset);
    }

    ///位置得分(越靠近中心得分越高)
    public static int PositionScore(ChessBoard board, ChessPos offset)
    {
        //这个值是通过对(offset.x - 7.5)^2 + (offset.y - 7.5)^2进行运算得到的。
        //其中，^表示乘方操作，即取平方，可以把棋盘上每颗棋子的位置想成一个圆锥，越靠近中心位置越高
        //参考点被设定为(7.5, 7.5)，棋盘的中心
        double mid_value = Mathf.Pow((float)(board.board_y_size * 0.5), 2) +
            Mathf.Pow((float)(board.board_x_size * 0.5), 2);
        double z = -(
            Mathf.Pow((float)(offset.x - board.board_y_size * 0.5), 2) +
            Mathf.Pow((float)(offset.y - board.board_x_size * 0.5), 2)) + mid_value;
        z /= 10;
        return (int)z;
    }

    /// <summary>
    /// 计算某个棋子对于 ownerPlayer 的分值
    /// </summary>
    /// <param name="chessmanPosition"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public static int ChessmanGrade(ChessBoard board, ChessType ownerPlayer, ChessPos chessmanPosition)
    {
        int score = 0;
        List<ChessPos> myChenssman = new List<ChessPos>();
        ChessPos offset;
        ChessPos first = chessmanPosition;
        ChessType player = ownerPlayer;

        ///横向
        //横向(左)
        offset = new ChessPos(first.x - 1, first.y);
        myChenssman.Clear();
        myChenssman.Add(first);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x - 1, offset.y);
        }

        //横向(右)
        offset = new ChessPos(first.x + 1, first.y);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x + 1, offset.y);
        }
        //要将x从小到大排序
        myChenssman.Sort((a, b) => {
            return (a.x - b.x);
        });
        score += GetScoring(board, myChenssman, player);

        ///竖向
        //竖向(上)
        myChenssman.Clear();
        myChenssman.Add(first);
        offset = new ChessPos(first.x, first.y - 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x, offset.y - 1);
        }

        //竖向(下)
        offset = new ChessPos(first.x, first.y + 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x, offset.y + 1);
        }
        myChenssman.Sort((a, b) => {
            return (a.y - b.y);
        });
        score += GetScoring(board, myChenssman, player);

        ///正斜(第三象限 -> 第一象限)
        //正斜(第三象限)
        myChenssman.Clear();
        myChenssman.Add(first);
        offset = new ChessPos(first.x - 1, first.y + 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x - 1, offset.y + 1);
        }

        //正斜(第一象限)
        offset = new ChessPos(first.x + 1, first.y - 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x + 1, offset.y - 1);
        }
        myChenssman.Sort((a, b) => {
            return (a.x - b.x) + (a.y - b.y);
        });
        score += GetScoring(board, myChenssman, player);

        ///反斜(第二象限 -> 第四象限)
        //反斜(第二象限)
        myChenssman.Clear();
        myChenssman.Add(first);
        offset = new ChessPos(first.x - 1, first.y - 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x - 1, offset.y - 1);
        }

        //反斜(第四象限)
        offset = new ChessPos(first.x + 1, first.y + 1);
        while (ExistSpecificChessman(board, offset, player))
        {
            myChenssman.Add(offset);
            offset = new ChessPos(offset.x + 1, offset.y + 1);
        }
        myChenssman.Sort((a, b) => {
            return (a.x - b.x) + (a.y + b.y);
        });
        score += GetScoring(board, myChenssman, player);

        int ss = score + GetAlongScoring(board, first, player);
        int jumpAlive4Count = GetJumpAlive4Count(board, new List<ChessPos>() { first}, player);
        int jumpAlive3Count = GetJumpAlive4Count(board, new List<ChessPos>() { first }, player);
        int jumpAlive2Count = GetJumpAlive4Count(board, new List<ChessPos>() { first }, player);
        score += LimitMax(jumpAlive4Count) * EvalateConst.JUMP_ALIVE4 +
                 LimitMax(jumpAlive3Count) * EvalateConst.JUMP_ALIVE3 +
                 LimitMax(jumpAlive2Count) * EvalateConst.JUMP_ALIVE2;

        //Debug.Log($"该子分值为: ${ss} ,其中单子得分:${GetAlongScoring(board, first, player)}, 组合得分:${score}");

        return score + GetAlongScoring(board, first, player);
      }

    /// <summary>
    /// 计算我方下一步较好的位置
    /// </summary>
    /// <returns></returns>
    public static ChessBufferMap MyBetterPosition(ChessBoard board, ChessType m_type, int max_count = 5)
    {
        ChessPos offset = ChessPos.zero;
        ChessBufferMap ourMap = new ChessBufferMap(max_count);
        int LINE_COUNT = board.board_y_size;
        int COL_COUNT = board.board_x_size;
        for (int i = 0; i <= LINE_COUNT; i++)
        {
            for (int j = 0; j <= COL_COUNT; j++)
            {
                offset = new ChessPos(i, j);
                if (IsBlankPosition(board, offset))
                {
                    int score = ChessmanGrade(board, m_type, offset);
                    if (ourMap.MinKey() < score)
                    {
                        ourMap.Put(score, offset);
                    }
                }
            }
        }
        return ourMap;
    }

    /// <summary>
    /// 计算敌方下一步较好的位置
    /// </summary>
    /// <param name="board"></param>
    /// <param name="en_type"></param>
    /// <param name="max_count"></param>
    /// <returns></returns>
    public static ChessBufferMap EnemyBetterPosition(ChessBoard board, ChessType en_type, int max_count = 5) 
    {
        ChessPos offset = ChessPos.zero;
        ChessBufferMap enemyMap = new ChessBufferMap(max_count);

        int count = 0;
        int LINE_COUNT = board.board_y_size;
        int COL_COUNT = board.board_x_size;
        for (int i = 0; i <= LINE_COUNT; i++) 
        {
            for (int j = 0; j <= COL_COUNT; j++) 
            {
                offset = new ChessPos(i, j);
                if (IsBlankPosition(board, offset))
                {
                    DateTime start = DateTime.Now;
                    int score = ChessmanGrade(board, en_type, offset);
                    DateTime end = DateTime.Now;
                    count++;
                    int time = end.Millisecond - start.Millisecond;
                    if (time > 5)
                    {
                        Debug.Log($"查找敌方最优落子位置耗时：{time}");
                    }
                    if (enemyMap.MinKey() < score)
                    {
                        enemyMap.Put(score, offset);
                    }
                }
            }
         }
        Debug.Log($"查找敌方最优落子位置次数：{count}");
        return enemyMap;
    }

}