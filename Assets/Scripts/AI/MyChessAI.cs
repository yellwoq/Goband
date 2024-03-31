using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

enum ChildType
{
    /// ��ǵ�ǰ�ڵ�Ϊ���ֽڵ㣬��ѡ��ʹ�ҷ��÷���С������
    MIN,

    /// ��ǵ�ǰ�ڵ�Ϊ�ҷ��ڵ㣬��ѡ��ʹ�ҷ��÷���������
    MAX
}

class ChessNode
{
    /// ��ǰ�ڵ������
    public ChessInfo current;
    /// ��ǰ�ڵ�ĸ��ڵ�
    public ChessNode parentNode;
    /// ��ǰ�ڵ�������ӽڵ�
    public List<ChessNode> childrenNode = new List<ChessNode>();
    /// ��ǰ�ڵ��ֵ
    public int value = int.MinValue;
    /// ��ǰ�ڵ������(�ҷ�/�з�)
    public ChildType type;
    /// ��ǰ�ڵ�ֵ������
    public int maxValue;
    /// ��ǰ�ڵ�ֵ������
    public int minValue;
    /// ��ǰ�ڵ�Ĳ����
    public int depth = 0;
    /// ���ڸ��ڵ��¼ѡ��ĸ����ӽڵ�
    public ChessInfo checkedCurrent;
}


// ����AI
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
        //������ֳ���ALIVE4�ļ���ֱ����
        ChessPos pos = NeedDefenses();
        if (pos != ChessPos.none)
        {
            return pos;
        }

        ChessPos position = ChessPos.zero;
        if (m_Type == AIType.EASY)
        {
            // ȡ�ҷ�,�з� ��5�����ŵ�λ��,
            // ���д���: ����ж�Ӧ�÷���,���ڵз�5������λ�����ҳ��ҷ��������ĵ�����
            // ���д���: ����ж�Ӧ�ý���,���ڼ���5������λ�����ҳ��з��������ĵ�����
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

    //����AI��û���漰�㷨
    //������ǰ�����ϵĿ�λ�ã�Ȼ���������ÿ�λ�ĵ÷�(λ�÷�+��Ϸ�)��Ȼ��ȡ������ߵĵ�����
    ChessPos BestPosition(ChessBufferMap ourPositions, ChessBufferMap enemyPositions)
    {
        ChessPos position = ChessPos.zero;
        double maxScore = 0;

        ///�����ֵ�����λ�õ÷� / �ҷ�����λ�õ÷� > 1.5 ���أ���֮����
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
    /// ������ʱ���
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
    ///  ��������
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
    /// ������е�ĳ�����Ӵ��
    /// </summary>
    /// <param name="chessman"></param>
    /// <param name="chessmanList"></param>
    /// <returns></returns>
    int ChessmanScore(ChessInfo chessman, List<ChessInfo> chessmanList)
    {
        ChessPos current = chessman.chess_pos;
        List<ChessPos> score4 = new List<ChessPos>();
        List<ChessPos> score3 = new List<ChessPos>();

        //0��
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

        //45�㷽��
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

        //90�㷽��
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

        //135��
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

        //180��
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

        //225��
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

        //270��
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

        //315��
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
                //�Ǹ���λ��
            }
            else if (owner.chess_type == chessman.chess_type)
            {
                //���Լ�������
                result += 4;
            }
            else
            {
                //�ǶԷ�������
                result -= 4;
            }
        }

        for (int i = 0; i < score3.Count; i++)
        {
            ChessPos offset = score3[i];
            ChessInfo owner = GetChessmanOwnerByPosition(offset, chessmanList);
            if (owner == null)
            {
                //�Ǹ���λ��
            }
            else if (owner.chess_type == chessman.chess_type)
            {
                //���Լ�������
                result += 3;
            }
            else
            {
                //�ǶԷ�������
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
    ///  ���ɲ������ӽڵ�
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="maxDepth"></param>
    void CreateChildren(ChessNode parent, int maxDepth = 4)
    {
        if (parent == null)
        {
            return;
        }

        // �ж��Ƿ�ﵽ�����ȣ�������������ֹ�ֵ������
        if (parent.depth > maxDepth)
        {
            List<ChessInfo> tem_list = CreateTempChessmanList(parent);
            parent.value = StatusScore(tem_list);
            return;
        }

        // ȷ����ǰ��Һ��ӽڵ�����
        ChessType currentPlayer =
            parent.current!.chess_type == ChessType.BLACK ? ChessType.WHITE : ChessType.BLACK;
        ChildType type = parent.type == ChildType.MAX ? ChildType.MIN : ChildType.MAX;

        // ������ʱ�����б�, �����ڵ�λ�ø��ų���ȥ
        var list = CreateTempChessmanList(parent);

        // ������������λ��
        var hig_start = DateTime.Now;
        BufferChessmanList enemyPosList = HighScorePosition(currentPlayer, list);
        Debug.LogFormat("HIGHT_SCORE:{0}", DateTime.Now - hig_start);

        // ����������λ�÷����б���
        List<ChessPos> result = enemyPosList.ToList();
        List<int> scores = enemyPosList.ToScoreList();

        int index = 0;
        // ������������λ�ã������ӽڵ�
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

            // �ݹ���� createChildren ���������ӽڵ���ӽڵ㣬ֱ���ﵽ�����Ȼ��޷��������ӽڵ�Ϊֹ��
            var start_time = DateTime.Now;
            CreateChildren(node, maxDepth);
            var end_time = DateTime.Now;
            Debug.LogFormat("{0}Create_Children_Time:{1}", end_time - start_time, parent.depth);
        }
    }

    //���������岩����
    ChessNode CreateGameTree()
    {
        //�������ڵ� root������������ֵ�����Ϊ0����ֵΪNaN���ڵ�����Ϊ ChildType.MAX����СֵΪ��������ֵΪ�����
        ChessNode root = new ChessNode();
        root.parentNode = null;
        root.current = null;
        root.depth = 0;
        root.value = int.MinValue;
        root.type = ChildType.MAX;
        root.minValue = int.MinValue;
        root.maxValue = int.MaxValue;

        //���ҵз���������λ�ã���������洢�� enemyPosList �����С�
        //Ȼ�󣬽� enemyPosList ת��Ϊ ChessPosList ����
        //�ٽ���ת��Ϊ��ͨ�б����� List<ChessPos> ������Щλ�ý����ڴ�����һ���ӽڵ㡣
        List<ChessInfo> chessInfoList = new List<ChessInfo>();
        if (m_chessBoard.chess_type_pos_map.ContainsKey(EnemyType))
        {
            chessInfoList = m_chessBoard.chess_type_pos_map[EnemyType];
        }
        Debug.Log($"chessInfoList{chessInfoList.Count}");
        // �ط������ڵ�����λ��
        BufferChessmanList enemyPosList = HighScorePosition(EnemyType, chessInfoList);
        List<ChessPos> result = enemyPosList.ToList();
        List<int> scores = enemyPosList.ToScoreList();

        int index = 0;
        //ͨ������ result �б�Ϊÿ��λ�� position ����һ���µ����� chessman ��һ���µ��ӽڵ� node
        //Ȼ���ӽڵ� node ��ӵ����ڵ���ӽڵ��б� root.childrenNode ��
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

            print($"������һ���{index}���ڵ��ʱ��{create - start}");
        }
        return root;
    }

    /// <summary>
    /// ��С����㷨
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
    int MaxMinSearch(ChessNode root)
    {
        if (root.childrenNode.Count == 0)
        {
            return root.value; // ����Ҷ�ӽڵ�Ĺ�ֵ
        }
        List<ChessNode> children = root.childrenNode;
        if (root.type == ChildType.MIN)
        {
            // ����Ƕ���ִ�в���
            foreach (ChessNode node in children)
            {
                // ��ǰ�ڵ�ȡ��С
                if (MaxMinSearch(node) < root.maxValue)
                {
                    // �ж��ӽڵ�Ĺ�ֵ�Ƿ�С�ڵ�ǰ�ڵ�����ֵ
                    root.maxValue = node.value; // ���µ�ǰ�ڵ�����ֵ
                    root.value = node.value; // ���µ�ǰ�ڵ�Ĺ�ֵ
                    root.checkedCurrent = node.current!; // ���µ�ǰ�ڵ��ѡ����
                 } 
                else
                {
                    continue; // �������������һ���ӽڵ�
                }
            }
         } 
        else
        {
            // ������Լ�ִ�в���
            foreach (ChessNode node in children)
            {
                // ��ǰ�ڵ�ȡ���
                if (MaxMinSearch(node) > root.minValue)
                {
                    // �ж��ӽڵ�Ĺ�ֵ�Ƿ���ڵ�ǰ�ڵ����Сֵ
                    root.minValue = node.value; // ���µ�ǰ�ڵ����Сֵ
                    root.value = node.value; // ���µ�ǰ�ڵ�Ĺ�ֵ
                    root.checkedCurrent = node.current!; // ���µ�ǰ�ڵ��ѡ����
                } 
                else
                {
                    continue; // �������������һ���ӽڵ�
                }
            }
        }
        return root.value; // ���ص�ǰ�ڵ�Ĺ�ֵ
    }

    /// <summary>
    /// alpha-beta ��֦�㷨
    /// </summary>
    /// <param name="current"></param>
    /// <returns></returns>
    int AlphaBetaSearch(ChessNode current) 
    {
        if (current.childrenNode.Count==0)
        {
            // �����ǰ�ڵ�û���ӽڵ㣬��ΪҶ�ӽڵ�
          return current.value; // ���ظýڵ��ֵ
        }

        if (current.parentNode != null && !current.parentNode!.childrenNode.Contains(current))
        {
            ChessNode parent = current.parentNode!;

            // ������ڵ�����Ҹ��ڵ���ӽڵ㲻������ǰ�ڵ㣬˵����֦�Ѿ������������ظ��ڵ�����/��Сֵ
            return parent.type == ChildType.MAX ? parent.minValue : parent.maxValue;
        }

        List<ChessNode> children = current.childrenNode; // ��ȡ��ǰ�ڵ���ӽڵ�

        if (current.type == ChildType.MIN) 
        {
           // ��ǰ�ڵ�ΪMIN�ڵ�
          int parentMin = current.parentNode?.minValue ?? int.MinValue; // ��ȡ���ڵ����Сֵ���������ڸ��ڵ�������Ϊ�������
          int index = 0; // ����������

          foreach(ChessNode node in children)
          {
            index++; // ��������

            int newCurrentMax = Mathf.Min(current.maxValue, AlphaBetaSearch(node)); // ���㵱ǰ�ӽڵ�����ֵ

            if (newCurrentMax <= parentMin) {
              // �����ǰ�ӽڵ�����ֵС�ڵ��ڸ��ڵ����Сֵ����˵����֦���Ա���ȫ����
              current.childrenNode.RemoveRange(index + 1, current.childrenNode.Count - index - 1); // ����ǰ�ڵ���ӽڵ��б�ض�����ǰ����λ��
              return parentMin; // ���ظ��ڵ����Сֵ
            }

            if (newCurrentMax < current.maxValue) {
              // �����ǰ�ӽڵ�����ֵС�ڵ�ǰ�ڵ�����ֵ������µ�ǰ�ڵ�����ֵ��ֵ�;���·����λ����Ϣ
              current.maxValue = newCurrentMax;
              current.value = node.value;
              current.checkedCurrent = node.current!;
            }
          }

          if (current.maxValue > parentMin) {
            // �����ǰ�ڵ�����ֵ���ڸ��ڵ����Сֵ������¸��ڵ����Сֵ��ֵ�;���·����λ����Ϣ
            current.parentNode.minValue = current.maxValue;
            current.parentNode.value = current.value;
            current.parentNode.checkedCurrent = current.current!;
          }

          return current.maxValue; // ���ص�ǰ�ڵ�����ֵ��Ϊ�ýڵ����������еļ�ֵ
        } 
        else 
        { 
            // ��ǰ�ڵ�ΪMAX�ڵ�
          int parentMax = current.parentNode?.maxValue ?? int.MinValue; // ��ȡ���ڵ�����ֵ���������ڸ��ڵ�������Ϊ�������
          int index = 0; // ����������

          foreach(ChessNode node in children) 
           {
            index++; // ��������

            int newCurrentMin = Mathf.Max(current.minValue, AlphaBetaSearch(node)); // ���㵱ǰ�ӽڵ����Сֵ

            if (parentMax < newCurrentMin) 
            {
                // ������ڵ�����ֵС�ڵ�ǰ�ӽڵ����Сֵ����˵����֦���Ա���ȫ����
                current.childrenNode.RemoveRange(index + 1, current.childrenNode.Count - index - 1);// ����ǰ�ڵ���ӽڵ��б�ض�����ǰ����λ��
                return parentMax; // ���ظ��ڵ�����ֵ
            }

            if (newCurrentMin > current.minValue) {
              // �����ǰ�ӽڵ����Сֵ���ڵ�ǰ�ڵ����Сֵ������µ�ǰ�ڵ����Сֵ��ֵ�;���·����λ����Ϣ
              current.minValue = newCurrentMin;
              current.value = node.value;
              current.checkedCurrent = node.current!;
            }
          }

          if (current.minValue < parentMax) {
            // �����ǰ�ڵ����СֵС�ڸ��ڵ�����ֵ������¸��ڵ�����ֵ��ֵ�;���·����λ����Ϣ
            current.parentNode.maxValue = current.minValue;
            current.parentNode.value = current.value;
            current.parentNode.checkedCurrent = current.current!;
          }

        return current.minValue; // ���ص�ǰ�ڵ����Сֵ��Ϊ�ýڵ����������еļ�ֵ
        }
    }

    /// <summary>
    /// ���ҵ��˸߷�λ��
    /// </summary>
    /// <param name="chessType"></param>
    /// <param name="currentChessmanList"></param>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    BufferChessmanList HighScorePosition(ChessType chessType, List<ChessInfo> currentChessmanList, int maxCount = 5) 
    {
        //�߷�
        BufferChessmanList list = new BufferChessmanList(maxCount);
        int LINE_COUNT = m_chessBoard.board_y_size;
        int COL_COUNT = m_chessBoard.board_x_size;
        var start_time = DateTime.Now;
        for (int x = 0; x <= LINE_COUNT - 1; x++) 
        {
            for (int y = 0; y <= COL_COUNT - 1; y++) 
            {
                ChessPos pos = new ChessPos(x, y);
                // ����ʣ��Ŀ�λ�ã�ѡ���������ߵ�
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
    /// �����Ŀǰ�Ѿ��µ�λ���Ƿ��ǿ�λ��
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
