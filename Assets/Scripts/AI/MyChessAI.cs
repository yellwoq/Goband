using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

enum ChildType
{
    /// 标记当前节点为对手节点，会选择使我方得分最小的走势
    MIN,

    /// 标记当前节点为我方节点，会选择使我方得分最大的走势
    MAX
}

class ChessNode
{
    /// 当前节点的棋子
    public ChessInfo current;
    /// 当前节点的父节点
    public ChessNode parentNode;
    /// 当前节点的所有子节点
    public List<ChessNode> childrenNode = new List<ChessNode>();
    /// 当前节点的值
    public int value = int.MinValue;
    /// 当前节点的类型(我方/敌方)
    public ChildType type;
    /// 当前节点值的上限
    public int maxValue;
    /// 当前节点值的下限
    public int minValue;
    /// 当前节点的层深度
    public int depth = 0;
    /// 用于根节点记录选择的根下子节点
    public ChessInfo checkedCurrent;
}


// 棋子AI
public class MyChessAI : MonoBehaviour
{
    public ChessType m_ChessType;
    public ChessType AIChessType
    {
        get
        {
            return m_ChessType;
        }

        set
        {
            m_ChessType = value;
        }
    }

    public AIType m_Type;
    private ChessType EnemyType
    {
        get
        {
            ChessType enem_type = m_ChessType == ChessType.BLACK ? ChessType.WHITE : ChessType.BLACK;
            return enem_type;
        }
    }

    private ChessBoard m_chessBoard;

    public void SetBelongChessboard(ChessBoard chessBoard)
    {
        m_chessBoard = chessBoard;
    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent(
            EventType.ON_AI_START, OnAIStart);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEvent(
            EventType.ON_AI_START, OnAIStart);
    }

    public void OnAIStart()
    {
        ChessPos next_pos = NextByAI();
        Debug.Log($"OnAIStart{next_pos}");
        EventManager.Instance.PublicEvent(EventType.ON_AI_FINISHED, next_pos);
    }

    public ChessPos NextByAI()
    {
        //如果评分出现ALIVE4的级别，直接下
        ChessPos pos = NeedDefenses();
        if (pos != ChessPos.none)
        {
            return pos;
        }

        ChessPos position = ChessPos.zero;
        if (m_Type == AIType.EASY)
        {
            // 取我方,敌方 各5个最优点位置,
            // 防中带攻: 如果判断应该防守,则在敌方5个最优位置中找出我方优势最大的点落子
            // 攻中带防: 如果判断应该进攻,则在己方5个最优位置中找出敌方优势最大的点落子
            ChessBufferMap ourPositions = CheckCurrentStateHelper.MyBetterPosition(m_chessBoard, m_ChessType);
            ChessBufferMap enemyPositions = CheckCurrentStateHelper.EnemyBetterPosition(m_chessBoard, EnemyType);
            position = BestPosition(ourPositions, enemyPositions);
        }
        else if(m_Type == AIType.MIDDLE)
        {
            ChessNode root = CreateGameTree();
            MaxMinSearch(root);
            position = root.checkedCurrent.chess_pos;
        }
        else if(m_Type == AIType.DIFFICULT)
        {
            ChessNode root = CreateGameTree();
            AlphaBetaSearch(root);
            position = root.checkedCurrent.chess_pos;
        }
            
        return position;
    }

    ChessPos NeedDefenses()
    {
        ChessBufferMap enemy = CheckCurrentStateHelper.EnemyBetterPosition(m_chessBoard, EnemyType);
        ChessPos defensesPosition = ChessPos.none;
        foreach (int key in enemy.GetkeySet())
        {
            if (key >= EvalateConst.ALIVE4)
            {
                defensesPosition = enemy[key];
                break;
            }
        }
        return defensesPosition;
    }

    //基础AI，没有涉及算法
    //遍历当前棋盘上的空位置，然后逐个计算该空位的得分(位置分+组合分)，然后取分数最高的点落子
    ChessPos BestPosition(ChessBufferMap ourPositions, ChessBufferMap enemyPositions)
    {
        ChessPos position = ChessPos.zero;
        double maxScore = 0;

        ///当对手的最优位置得分 / 我方最优位置得分 > 1.5 防守，反之进攻
        if (enemyPositions.MaxKey() / ourPositions.MaxKey() > 1.5)
        {
            foreach (int key in enemyPositions.GetkeySet())
            {
                int attackScore = CheckCurrentStateHelper.ChessmanGrade(m_chessBoard, EnemyType, enemyPositions[key]);
                double score = key * 1.0 + attackScore * 0.8;
                if (score >= maxScore)
                {
                    maxScore = score;
                    position = enemyPositions[key];
                }
            }
        }
        else
        {
            foreach (int key in ourPositions.GetkeySet())
            {
                int defenseScore = CheckCurrentStateHelper.ChessmanGrade(m_chessBoard, m_ChessType, ourPositions[key]);
                double score = key * 1.0 + defenseScore * 0.8;
                if (score >= maxScore)
                {
                    maxScore = score;
                    position = ourPositions[key]!;
                }
            }
        }
        return position;
    }


    /// <summary>
    /// 生成临时棋局
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    List<ChessInfo> CreateTempChessmanList(ChessNode node)
    {
        List<ChessInfo> temp = new List<ChessInfo>
        {
            node.current
        };

        ChessNode current = node.parentNode; 
        while (current != null && current.current != null)
        {
            temp.Add(current.current);
            current = current.parentNode;
            Debug.Log("current");
        }
        return temp;
    }

    /// <summary>
    ///  局势评估
    /// </summary>
    /// <param name="chessmanList"></param>
    /// <returns></returns>
    int StatusScore(List<ChessInfo> chessmanList)
    {
        int score = 0;
        for (int i = 0; i < chessmanList.Count; i++)
        {
            score += ChessmanScore(chessmanList[i], chessmanList);
        }
        return score;
    }

    /// <summary>
    /// 对棋局中的某个棋子打分
    /// </summary>
    /// <param name="chessman"></param>
    /// <param name="chessmanList"></param>
    /// <returns></returns>
    int ChessmanScore(ChessInfo chessman, List<ChessInfo> chessmanList)
    {
        ChessPos current = chessman.chess_pos;
        List<ChessPos> score4 = new List<ChessPos>();
        List<ChessPos> score3 = new List<ChessPos>();

        //0°
        ChessPos right = new ChessPos(current.x + 1, current.y);
        ChessPos right2 = new ChessPos(current.x + 2, current.y);
        if (CheckCurrentStateHelper.IsEffectivePosition(right))
        {
            score4.Add(right);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(right2))
        {
            score3.Add(right2);
        }

        //45°方向
        ChessPos rightTop = new ChessPos(current.x + 1, current.y - 1);
        ChessPos rightTop2 = new ChessPos(current.x + 2, current.y - 2);
        if (CheckCurrentStateHelper.IsEffectivePosition(rightTop))
        {
            score4.Add(rightTop);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(rightTop2))
        {
            score3.Add(rightTop2);
        }

        //90°方向
        ChessPos centerTop = new ChessPos(current.x, current.y - 1);
        ChessPos centerTop2 = new ChessPos(current.x, current.y - 2);
        if (CheckCurrentStateHelper.IsEffectivePosition(centerTop))
        {
            score4.Add(centerTop);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(centerTop2))
        {
            score3.Add(centerTop2);
        }

        //135°
        ChessPos leftTop = new ChessPos(current.x - 1, current.y - 1);
        ChessPos leftTop2 = new ChessPos(current.x - 2, current.y - 2);
        if (CheckCurrentStateHelper.IsEffectivePosition(leftTop))
        {
            score4.Add(leftTop);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(leftTop2))
        {
            score3.Add(leftTop2);
        }

        //180°
        ChessPos left = new ChessPos(current.x - 1, current.y);
        ChessPos left2 = new ChessPos(current.x - 2, current.y);
        if (CheckCurrentStateHelper.IsEffectivePosition(left))
        {
            score4.Add(left);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(left2))
        {
            score3.Add(left2);
        }

        //225°
        ChessPos leftBottom = new ChessPos(current.x - 1, current.y + 1);
        ChessPos leftBottom2 = new ChessPos(current.x - 2, current.y + 2);
        if (CheckCurrentStateHelper.IsEffectivePosition(leftBottom))
        {
            score4.Add(leftBottom);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(leftBottom2))
        {
            score3.Add(leftBottom2);
        }

        //270°
        ChessPos bottom = new ChessPos(current.x, current.y + 1);
        ChessPos bottom2 = new ChessPos(current.x, current.y + 2);
        if (CheckCurrentStateHelper.IsEffectivePosition(bottom))
        {
            score4.Add(bottom);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(bottom2))
        {
            score3.Add(bottom2);
        }

        //315°
        ChessPos rightBottom = new ChessPos(current.x + 1, current.y + 1);
        ChessPos rightBottom2 = new ChessPos(current.x + 1, current.y + 2);
        if (CheckCurrentStateHelper.IsEffectivePosition(rightBottom))
        {
            score4.Add(rightBottom);
        }
        if (CheckCurrentStateHelper.IsEffectivePosition(rightBottom2))
        {
            score3.Add(rightBottom2);
        }

        int result = 0;
        for (int i = 0; i < score4.Count; i++)
        {
            ChessPos offset = score4[i];
            ChessInfo owner = GetChessmanOwnerByPosition(offset, chessmanList);
            if (owner == null)
            {
                //是个空位置
            }
            else if (owner.chess_type == chessman.chess_type)
            {
                //是自己的棋子
                result += 4;
            }
            else
            {
                //是对方的棋子
                result -= 4;
            }
        }

        for (int i = 0; i < score3.Count; i++)
        {
            ChessPos offset = score3[i];
            ChessInfo owner = GetChessmanOwnerByPosition(offset, chessmanList);
            if (owner == null)
            {
                //是个空位置
            }
            else if (owner.chess_type == chessman.chess_type)
            {
                //是自己的棋子
                result += 3;
            }
            else
            {
                //是对方的棋子
                result -= 3;
            }
        }
        score4.Clear();
        score3.Clear();
        return result;
    }

    ChessInfo GetChessmanOwnerByPosition(ChessPos position, List<ChessInfo> chessmanList)
    {
        return chessmanList.Find(c => c.chess_pos == position);
    }

    /// <summary>
    ///  生成博弈树子节点
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="maxDepth"></param>
    void CreateChildren(ChessNode parent, int maxDepth = 4)
    {
        if (parent == null)
        {
            return;
        }

        // 判断是否达到最大深度，如果是则计算棋局估值并返回
        if (parent.depth > maxDepth)
        {
            List<ChessInfo> tem_list = CreateTempChessmanList(parent);
            parent.value = StatusScore(tem_list);
            return;
        }

        // 确定当前玩家和子节点类型
        ChessType currentPlayer =
            parent.current!.chess_type == ChessType.BLACK ? ChessType.WHITE : ChessType.BLACK;
        ChildType type = parent.type == ChildType.MAX ? ChildType.MIN : ChildType.MAX;

        // 创建临时棋子列表, 将父节点位置给排除出去
        var list = CreateTempChessmanList(parent);

        // 查找最优落子位置
        var hig_start = DateTime.Now;
        BufferChessmanList enemyPosList = HighScorePosition(currentPlayer, list);
        Debug.LogFormat("HIGHT_SCORE:{0}", DateTime.Now - hig_start);

        // 将最优落子位置放入列表中
        List<ChessPos> result = enemyPosList.ToList();
        List<int> scores = enemyPosList.ToScoreList();

        int index = 0;
        // 遍历最优落子位置，生成子节点
        for (; index < result.Count; index++)
        {
            ChessPos position = result[index];
            int score = scores[index];
            ChessInfo chessman = new ChessInfo(currentPlayer, position, score);
            ChessNode node = new ChessNode();
            node.parentNode = parent;
            node.current = chessman;
            node.depth = parent.depth + 1;
            node.value = score;
            node.type = type;
            node.minValue = parent.maxValue;
            node.maxValue = parent.maxValue;

            parent.childrenNode.Add(node);

            // 递归调用 createChildren 方法生成子节点的子节点，直到达到最大深度或无法再生成子节点为止。
            var start_time = DateTime.Now;
            CreateChildren(node, maxDepth);
            var end_time = DateTime.Now;
            Debug.LogFormat("{0}Create_Children_Time:{1}", end_time - start_time, parent.depth);
        }
    }

    //生成五子棋博弈树
    ChessNode CreateGameTree()
    {
        //创建根节点 root，设置其属性值：深度为0，估值为NaN，节点类型为 ChildType.MAX，最小值为负无穷，最大值为正无穷。
        ChessNode root = new ChessNode();
        root.parentNode = null;
        root.current = null;
        root.depth = 0;
        root.value = int.MinValue;
        root.type = ChildType.MAX;
        root.minValue = int.MinValue;
        root.maxValue = int.MaxValue;

        //查找敌方最优落子位置，并将结果存储在 enemyPosList 变量中。
        //然后，将 enemyPosList 转换为 ChessPosList 对象
        //再将其转换为普通列表类型 List<ChessPos> 对象。这些位置将用于创建第一层子节点。
        List<ChessInfo> chessInfoList = new List<ChessInfo>();
        if (m_chessBoard.chess_type_pos_map.ContainsKey(EnemyType))
        {
            chessInfoList = m_chessBoard.chess_type_pos_map[EnemyType];
        }
        Debug.Log($"chessInfoList{chessInfoList.Count}");
        // 地方会下在的最优位置
        BufferChessmanList enemyPosList = HighScorePosition(EnemyType, chessInfoList);
        List<ChessPos> result = enemyPosList.ToList();
        List<int> scores = enemyPosList.ToScoreList();

        int index = 0;
        //通过遍历 result 列表，为每个位置 position 创建一个新的棋子 chessman 和一个新的子节点 node
        //然后将子节点 node 添加到根节点的子节点列表 root.childrenNode 中
        for (; index < result.Count; index++)
        {
            ChessPos position = result[index];
            int score = scores[index];
            ChessInfo chessman = new ChessInfo(EnemyType, position, scores[index]);
            ChessNode node = new ChessNode();
            node.depth = root.depth + 1;
            node.parentNode = root;
            node.value = scores[index];
            node.type = ChildType.MIN;
            node.minValue = root.minValue;
            node.maxValue = root.maxValue;
            node.current = chessman;

            root.childrenNode.Add(node);
            var start = DateTime.Now;
            CreateChildren(node);
            var create = DateTime.Now;

            print($"创建第一层第{index}个节点耗时：{create - start}");
        }
        return root;
    }

    /// <summary>
    /// 最小最大算法
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    int MaxMinSearch(ChessNode root)
    {
        if (root.childrenNode.Count == 0)
        {
            return root.value; // 返回叶子节点的估值
        }
        List<ChessNode> children = root.childrenNode;
        if (root.type == ChildType.MIN)
        {
            // 如果是对手执行操作
            foreach (ChessNode node in children)
            {
                // 当前节点取最小
                if (MaxMinSearch(node) < root.maxValue)
                {
                    // 判断子节点的估值是否小于当前节点的最大值
                    root.maxValue = node.value; // 更新当前节点的最大值
                    root.value = node.value; // 更新当前节点的估值
                    root.checkedCurrent = node.current!; // 更新当前节点的选择步骤
                 } 
                else
                {
                    continue; // 否则继续遍历下一个子节点
                }
            }
         } 
        else
        {
            // 如果是自己执行操作
            foreach (ChessNode node in children)
            {
                // 当前节点取最大
                if (MaxMinSearch(node) > root.minValue)
                {
                    // 判断子节点的估值是否大于当前节点的最小值
                    root.minValue = node.value; // 更新当前节点的最小值
                    root.value = node.value; // 更新当前节点的估值
                    root.checkedCurrent = node.current!; // 更新当前节点的选择步骤
                } 
                else
                {
                    continue; // 否则继续遍历下一个子节点
                }
            }
        }
        return root.value; // 返回当前节点的估值
    }

    /// <summary>
    /// alpha-beta 剪枝算法
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    int AlphaBetaSearch(ChessNode current) 
    {
        if (current.childrenNode.Count==0)
        {
            // 如果当前节点没有子节点，即为叶子节点
          return current.value; // 返回该节点的值
        }

        if (current.parentNode != null && !current.parentNode!.childrenNode.Contains(current))
        {
            ChessNode parent = current.parentNode!;

            // 如果父节点存在且父节点的子节点不包含当前节点，说明该枝已经被剪掉，返回父节点的最大/最小值
            return parent.type == ChildType.MAX ? parent.minValue : parent.maxValue;
        }

        List<ChessNode> children = current.childrenNode; // 获取当前节点的子节点

        if (current.type == ChildType.MIN) 
        {
           // 当前节点为MIN节点
          int parentMin = current.parentNode?.minValue ?? int.MinValue; // 获取父节点的最小值，若不存在父节点则设置为负无穷大
          int index = 0; // 索引计数器

          foreach(ChessNode node in children)
          {
            index++; // 索引递增

            int newCurrentMax = Mathf.Min(current.maxValue, AlphaBetaSearch(node)); // 计算当前子节点的最大值

            if (newCurrentMax <= parentMin) {
              // 如果当前子节点的最大值小于等于父节点的最小值，则说明该枝可以被完全剪掉
              current.childrenNode.RemoveRange(index + 1, current.childrenNode.Count - index - 1); // 将当前节点的子节点列表截断至当前索引位置
              return parentMin; // 返回父节点的最小值
            }

            if (newCurrentMax < current.maxValue) {
              // 如果当前子节点的最大值小于当前节点的最大值，则更新当前节点的最大值、值和经过路径的位置信息
              current.maxValue = newCurrentMax;
              current.value = node.value;
              current.checkedCurrent = node.current!;
            }
          }

          if (current.maxValue > parentMin) {
            // 如果当前节点的最大值大于父节点的最小值，则更新父节点的最小值、值和经过路径的位置信息
            current.parentNode.minValue = current.maxValue;
            current.parentNode.value = current.value;
            current.parentNode.checkedCurrent = current.current!;
          }

          return current.maxValue; // 返回当前节点的最大值作为该节点在搜索树中的价值
        } 
        else 
        { 
            // 当前节点为MAX节点
          int parentMax = current.parentNode?.maxValue ?? int.MinValue; // 获取父节点的最大值，若不存在父节点则设置为正无穷大
          int index = 0; // 索引计数器

          foreach(ChessNode node in children) 
           {
            index++; // 索引递增

            int newCurrentMin = Mathf.Max(current.minValue, AlphaBetaSearch(node)); // 计算当前子节点的最小值

            if (parentMax < newCurrentMin) 
            {
                // 如果父节点的最大值小于当前子节点的最小值，则说明该枝可以被完全剪掉
                current.childrenNode.RemoveRange(index + 1, current.childrenNode.Count - index - 1);// 将当前节点的子节点列表截断至当前索引位置
                return parentMax; // 返回父节点的最大值
            }

            if (newCurrentMin > current.minValue) {
              // 如果当前子节点的最小值大于当前节点的最小值，则更新当前节点的最小值、值和经过路径的位置信息
              current.minValue = newCurrentMin;
              current.value = node.value;
              current.checkedCurrent = node.current!;
            }
          }

          if (current.minValue < parentMax) {
            // 如果当前节点的最小值小于父节点的最大值，则更新父节点的最大值、值和经过路径的位置信息
            current.parentNode.maxValue = current.minValue;
            current.parentNode.value = current.value;
            current.parentNode.checkedCurrent = current.current!;
          }

        return current.minValue; // 返回当前节点的最小值作为该节点在搜索树中的价值
        }
    }

    /// <summary>
    /// 查找敌人高分位置
    /// </summary>
    /// <param name="chessType"></param>
    /// <param name="currentChessmanList"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    BufferChessmanList HighScorePosition(ChessType chessType, List<ChessInfo> currentChessmanList, int maxCount = 5) 
    {
        //高分
        BufferChessmanList list = new BufferChessmanList(maxCount);
        int LINE_COUNT = m_chessBoard.board_y_size;
        int COL_COUNT = m_chessBoard.board_x_size;
        var start_time = DateTime.Now;
        for (int x = 0; x <= LINE_COUNT - 1; x++) 
        {
            for (int y = 0; y <= COL_COUNT - 1; y++) 
            {
                ChessPos pos = new ChessPos(x, y);
                // 看看剩余的空位置，选五个分数最高的
                if (IsBlankPosition(pos, currentChessmanList)) 
                {
                    ChessInfo chessman = new ChessInfo(chessType, pos, 0);
                    int chessScore = ChessmanScore(chessman, currentChessmanList);
                    int posScore = CheckCurrentStateHelper.PositionScore(m_chessBoard, pos);
                    int score = chessScore + posScore;
                    if (list.MinScore() < score) 
                    {
                        chessman.score = score;
                        list.Add(chessman);
                    }
                }
            }
        }
        var end_time = DateTime.Now;
        Debug.LogFormat("HighScorePositionInner:{0}", end_time - start_time);
        return list;
    }

    /// <summary>
    /// 相对于目前已经下的位置是否是空位置
    /// </summary>
    /// <param name="position"></param>
    /// <param name="chessmanList"></param>
    /// <returns></returns>
    bool IsBlankPosition(ChessPos position, List<ChessInfo> chessmanList)
    {
        if (!CheckCurrentStateHelper.IsEffectivePosition(position))
        {
            return false;
        }
        if (m_chessBoard.chess_map_dic.ContainsKey(position))
        {
            return false;
        }
        if (chessmanList.Count == 0)
        {
            return true;
        }
        foreach (ChessInfo chessman in chessmanList)
        {
            if (chessman.chess_pos == position)
            {
                return false;
            }
        }
        return true;
    }

}
