using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

// ����
public class ChessBoard : MonoEntity
{
    public int board_x_size = 16;
    public int board_y_size = 15;
    public GameObject chess_prefab;

    private Transform touchBoard;
    private Transform chessParent;
    private Vector2 refrenceSolution;

    public Vector2 RefrenceSolution
    {
        get
        {
            return refrenceSolution;
        }
        set
        {
            refrenceSolution = value;
        }
    }

    //�������ݼ���
    internal Dictionary<ChessPos, string> chess_map_dic = new Dictionary<ChessPos, string>();

    //������Ϸʤ�����
    public Dictionary<ChessType, List<ChessInfo>> chess_type_pos_map = new Dictionary<ChessType, List<ChessInfo>>();

    private void Awake()
    {
        chessParent = transform.Find("Chesses");
        touchBoard = transform.Find("TouchBoard");

    }

    private void OnEnable()
    {
        EventManager.Instance.RegisterEvent<int, int, Vector2>(
            EventType.OnScreenChanged, OnScreenSizeChanged);
    }

    private void OnDisable()
    {
        EventManager.Instance.UnRegisterEvent<int, int, Vector2>(
            EventType.OnScreenChanged, OnScreenSizeChanged);
    }

    private void Update()
    {
        //Vector2 loc_pos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //  chessParent.GetComponent<RectTransform>(), Input.mousePosition, null, out loc_pos);

        //Debug.Log("���ӵ���Ļ���꣺" + loc_pos);
    }

    //��������
    public void DrawChessboard()
    {
        //foreach (var item in chess_map_dic)
        //{
        //    for (int i = 0; i < item.Value.Count; i++)
        //    {
        //        item.Value
        //    }
        //}
    }

    //��λ�÷���һö����
    public bool PlaceChess(ChessType chessType, int xIndex, int yIndex)
    {
        ChessPos chessPos = new ChessPos(xIndex, yIndex);
        if (!chess_map_dic.ContainsKey(chessPos))
        {
            string entity_id;
            GameObject chess_obj = Instantiate(chess_prefab, chessParent);
            chess_obj.name = chessType.ToString() + $"({xIndex}, {yIndex})";
            chess_obj.SetActive(true);
            Chess chess = ChessFactory.Instance.CreatEntity<Chess>(EntityType.Chess, out entity_id, chess_obj);
            chess.SetInfo(chessType, xIndex, yIndex, CaculateAnchorPos(xIndex, yIndex));
            chess_map_dic.Add(chessPos, entity_id);

            if (!chess_type_pos_map.ContainsKey(chessType))
            {
                chess_type_pos_map.Add(chessType, new List<ChessInfo>());
            }
            chess_type_pos_map[chessType].Add(chess.m_chessInfo);

            if (CheckResultIsSuccess(chessType, chessPos) || CheckIsHeChess())
            {
                Debug.Log(chessType + "YING LE");
                EventManager.Instance.PublicEvent(EventType.GameStateChanged, false);
                string win_chess = chessType == ChessType.WHITE ? "����" : "����";
                string tip_str = $"��ϲ!! {win_chess}Ӯ��,�����ť����һ��!!";
                if(CheckIsHeChess())
                {
                    win_chess = "��";
                    tip_str = $"������,�����ť����һ��!!";
                }
                ControllerUI controller_ui = UIManager.Instance.ShowPopup("ControllerUI") as ControllerUI;
                controller_ui.SetShowTxt(tip_str);
                controller_ui.ClickAddListener(OnFinish);
            }
            return true;
        }
        return false;
    }

    private void OnFinish()
    {
        ClearBoard();
    }

    private Vector2 CaculateAnchorPos(int xIndex, int yIndex)
    {
        float xoffset = UIConsts.DEFAULT_X_OFFSET_POS;
        float yoffset = UIConsts.DEFAULT_Y_OFFSET_POS;
        float xPos = UIConsts.DEFAULT_LEFT_X_POS + xoffset * xIndex;
        float yPos = UIConsts.DEFAULT_UP_Y_POS + yoffset * yIndex;
        return new Vector2(xPos, yPos);
    }

    // �������Ƿ�������һ��������
    public ChessPos CaculateAnchorPosIndex(Vector2 inputPos, out bool is_on_right_pos)
    {
        float xoffset = UIConsts.DEFAULT_X_OFFSET_POS;
        float yoffset = UIConsts.DEFAULT_Y_OFFSET_POS;
        RectTransform chess_RectTrans = chess_prefab.GetComponent<RectTransform>();
        Vector2 screen_ui_anchor_point;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            chess_RectTrans, Input.mousePosition, null, out screen_ui_anchor_point);
        screen_ui_anchor_point.x += chess_RectTrans.offsetMax.x - chess_RectTrans.rect.width / 2;
        screen_ui_anchor_point.y += chess_RectTrans.offsetMax.y - chess_RectTrans.rect.height / 2;

        for (int i = 0; i < board_x_size; i++)
        {
            for (int j = 0; j < board_y_size; j++)
            {
                float xPos = UIConsts.DEFAULT_LEFT_X_POS + xoffset * i;
                float yPos = UIConsts.DEFAULT_UP_Y_POS + yoffset * j;
   
                if(Mathf.Abs(screen_ui_anchor_point.x - xPos) <= chess_RectTrans.rect.width / 2 &&
                    Mathf.Abs(screen_ui_anchor_point.y - yPos) <= chess_RectTrans.rect.height / 2)
                {
                    is_on_right_pos = true;
                    Debug.Log("�ҵ�������λ�ã�����");
                    return new ChessPos(i, j);
                }
            }
        }
        is_on_right_pos = false;
        return ChessPos.zero;
    }

    //�������
    public void ClearBoard()
    {
        foreach (var item in chess_map_dic)
        {
            string entity_id = item.Value;
            bool has_entity;
            Chess chess = ChessFactory.Instance.GetEntity<Chess>(entity_id, out has_entity);
            if(has_entity)
            {
                ChessFactory.Instance.RemoveEntity(entity_id);
                Destroy(chess.gameObject);
            }
        }
        chess_map_dic.Clear();
        chess_type_pos_map.Clear();
    }

    //�ж��Ƿ�ʤ��
    bool CheckResultIsSuccess(ChessType chessType, ChessPos pos)
    {
        int currentX = pos.x;
        int currentY = pos.y;

        int count = 0;
        List<ChessPos> winResult = new List<ChessPos>();
        int LINE_COUNT = board_y_size;
        int COL_COUNT = board_x_size;

        ///��
        /// o o o o o
        /// o o o o o
        /// x x x x x
        /// o o o o o
        /// o o o o o
        winResult.Clear();
        // ѭ��������ǰ�е�ǰ���ĸ�λ�ã�������ڣ�������Ƿ����ض�������������������
        //�ж� currentX - 4 > 0 ʱ��������˼���ж����� 4 ��λ���Ƿ��������ڡ�
        //��� currentX - 4 >= 0�����ʾ���� 4 ��λ���������ڣ�
        //���򣬼� currentX - 4 < 0����ʾ���� 4 ��λ���Ѿ����������̱߽硣
        for (int i = (currentX - 4 >= 0 ? currentX - 4 : 0);
            i <= (currentX + 4 < LINE_COUNT ? currentX + 4 : LINE_COUNT - 1);
            i++)
        {
           // ���㵱ǰλ�õ�����
           ChessPos position = new ChessPos(i, currentY);

            // ��鵱ǰλ���Ƿ����ʤ��������
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                // ����������ӵ�ʤ������б��У������Ӽ�����
                winResult.Add(position);
                count++;
            }
            else
            {
                // ����������ض������ӣ����ʤ������б���������������Ϊ0
                winResult.Clear();
                count = 0;
            }

            // ����������������ﵽ5����ʾ���������������ʤ������Ϣ������true
            if (count >= 5)
            {
                return true;
            }
        }

        ///��
        /// o o x o o
        /// o o x o o
        /// o o x o o
        /// o o x o o
        /// o o x o o
        count = 0;
        winResult.Clear();
        for (int j = (currentY - 4 >= 0 ? currentY - 4 : 0);
            j <= (currentY + 4 > COL_COUNT ? COL_COUNT - 1 : currentY + 4);
            j++)
        {
            ChessPos position = new ChessPos(currentX, j);
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                winResult.Add(position);
                count++;
            }
            else
            {
                winResult.Clear();
                count = 0;
            }
            if (count >= 5)
            {
                return true;
            }
        }

        ///��б
        /// o o o o x
        /// o o o x o
        /// o o x o o
        /// o x o o o
        /// x o o o o
        count = 0;
        winResult.Clear();
        ChessPos offset2 = new ChessPos(currentX - 4, currentY + 4);
        for (int i = 0; i < 10; i++)
        {
            ChessPos position = new ChessPos(offset2.x + i, offset2.y - i);
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                winResult.Add(position);
                count++;
            }
            else
            {
                winResult.Clear();
                count = 0;
            }
            if (count >= 5)
            {
                return true;
            }
        }

        ///��б
        /// x o o o o
        /// o x o o o
        /// o o x o o
        /// o o o x o
        /// o o o o x
        count = 0;
        winResult.Clear();
        ChessPos offset = new ChessPos(currentX - 4, currentY - 4);
        for (int i = 0; i < 10; i++)
        {
            ChessPos position = new ChessPos(offset.x + i, offset.y + i);
            if (CheckCurrentStateHelper.ExistSpecificChessman(this, position, chessType))
            {
                winResult.Add(position);
                count++;
            }
            else
            {
                winResult.Clear();
                count = 0;
            }
            if (count >= 5)
            {
                return true;
            }
        }
        winResult.Clear();
        return false;
    }

    bool CheckIsHeChess()
    {
        return chess_map_dic.Count >= board_x_size * board_y_size;
    }

    public List<ChessInfo> GetChessPosListByChessType(ChessType chessType)
    {
        if (!chess_type_pos_map.ContainsKey(chessType))
        {
            return null;
        }
        return chess_type_pos_map[chessType];
    }

    public Chess GetChessByPos(ChessPos pos)
    {
        if (!chess_map_dic.ContainsKey(pos))
            return null;
        string entity_id = chess_map_dic[pos];
        bool has_entity;
        Chess chess = ChessFactory.Instance.GetEntity<Chess>(entity_id, out has_entity);
        return chess;
    }

    void OnScreenSizeChanged(int x, int y, Vector2 referenceResolution)
    {
        //����Ļ�����ı�ʱ����
        Debug.Log("��Ļ�������˸ı�");
    }

}
