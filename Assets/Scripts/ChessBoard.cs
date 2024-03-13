using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

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
    private Dictionary<ChessType, List<ChessPos>> chess_type_pos_map = new Dictionary<ChessType, List<ChessPos>>();

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
    public void PlaceChess(ChessType chessType, int xIndex, int yIndex)
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
                chess_type_pos_map.Add(chessType, new List<ChessPos>());
            }
            chess_type_pos_map[chessType].Add(chessPos);

            if (CheckIsSuccess(chessType, chessPos) || chess_map_dic.Count >= board_x_size * board_y_size)
            {
                Debug.Log(chessType + "YING LE");
                EventManager.Instance.PublicEvent(EventType.GameStateChanged, false);
                string win_chess = chessType == ChessType.WHITE ? "����" : "����";
                string tip_str = $"��ϲ!! {win_chess}Ӯ��,�����ť����һ��!!";
                if(chess_map_dic.Count >= board_x_size * board_y_size)
                {
                    win_chess = "��";
                    tip_str = $"������,�����ť����һ��!!";
                }
                ControllerUI controller_ui = UIManager.Instance.ShowPopup("ControllerUI") as ControllerUI;
                controller_ui.SetShowTxt(tip_str);
                controller_ui.ClickAddListener(OnFinish);
            }
        }
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

    //�ж�����ĳ�������Ƿ�����ָ������������
    private bool CheckByStep(ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff, int check_count=5)
    {
        ChessPos tmp = new ChessPos(0, 0);
        int i;
        int cnt = 0;

        //�򷴷����ҵ���ɫ��ͬ�ĵ�
        for (i = 1; i < check_count; i++)
        {
            tmp.x = p.x - xdiff * i;
            tmp.y = p.y - ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        for (i = 1; i < check_count; i++)
        {
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        if (cnt >= check_count-1)
            return true;
        return false;
    }

    private List<ChessPos> GetPosListByStep(
        ChessType chessType, ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff, int check_count = 5)
    {
        List<ChessPos> chessPosList = new List<ChessPos>();
        if(check_count <= 1)
        {
            return chessPosList;
        }
        ChessPos tmp = new ChessPos(0, 0);
        int i;

        int cnt = 0;
        int cnt_other = 0;
        List<ChessPos> cnt_blank_pos_list = new List<ChessPos>();

        int rev_cnt = 0;
        int rev_cnt_other = 0;
        List<ChessPos> rev_cnt_blank_pos_list = new List<ChessPos>();

        bool cnt_can_break = false;
        bool rev_can_break = false;
        // 1���ӵ�ǰ�������������ֱ��������һ���Է����ӻ��߿ո������ϵ�ǰ������������check_count-1
        for (i = -check_count + 1; i < check_count; i++)
        {
            if(i == 0)
            {
                continue;
            }
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;

            if((i < 0 && !rev_can_break) || (i > 0 && !cnt_can_break))
                if (!chess_array.Contains(tmp))
                {
                    string chess_uid;
                    chess_map_dic.TryGetValue(tmp, out chess_uid);
                    bool has_entity;
                    Chess chess = ChessFactory.Instance.GetEntity<Chess>(chess_uid, out has_entity);
                    cnt_can_break = i > 0 && has_entity;
                    rev_can_break = i < 0 && has_entity;
                    if (has_entity)
                    {
                        if(i < 0 && !rev_can_break)
                        {
                            rev_cnt_other++;
                        }
                        else if(i > 0 && !cnt_can_break)
                        {
                            cnt_other++;
                        }
                    }
                    else
                    {
                        _ = i < 0 ? rev_cnt_blank_pos_list.Add(tmp) : cnt_blank_pos_list.Add(tmp);
                    }
                    cnt_can_break |= cnt + cnt_blank_pos_list.Count >= check_count - 1;
                    rev_can_break |= rev_cnt + rev_cnt_blank_pos_list.Count >= check_count - 1;
                    continue;
                }

                if (i > 0 && !cnt_can_break)
                {
                    cnt++;
                }
                else if(i < 0 && !rev_can_break)
                {
                    rev_cnt++;
                }
        }

        //2�����ݿո����ͼ�������λ�ã����������
        //1��ĳһ�ߵļ�����������һ�ߵļ������� check_count-1����ʱ�Ѿ�û��λ�ÿ�������
        //2��ĳһ�ߵļ�����������һ�ߵļ�������С�� check_count-1��˵�����п������µĵط���������֮������check_count��������
        //3��ֱ�Ӽ�����пո�λ���Ƿ�����CheckByStep����ⷶΧ���ǵ�ǰ�ķ�Χ�� ������ӽ�ȥ


        if (cnt >= check_count - 1)
            return true;
        return false;
    }


    public bool GetNextCanPlacePosByChessCount(ChessType chessType, ChessPos p, int chessCount, out List<ChessPos> winChessPos)
    {
        List<ChessPos> array = chess_type_pos_map[chessType];

        if (CheckByStep(p, array, 0, 1, chessCount))    //����ֱ���ж�
            return true;
        if (CheckByStep(p, array, 1, 0))    //����ֱ���ж�
            return true;
        if (CheckByStep(p, array, 1, 1))    //�ҳ���ֱ���ж�
            return true;
        if (CheckByStep(p, array, -1, 1))   //�ҳ���ֱ���ж�
            return true;
        return false;
    }

    public bool CheckIsSuccess(ChessType chessType, ChessPos p)
    {
        List<ChessPos> array = chess_type_pos_map[chessType];

        if (CheckByStep(p, array, 0, 1))    //����ֱ���ж�
            return true;
        if (CheckByStep(p, array, 1, 0))    //����ֱ���ж�
            return true;
        if (CheckByStep(p, array, 1, 1))    //�ҳ���ֱ���ж�
            return true;
        if (CheckByStep(p, array, -1, 1))   //�ҳ���ֱ���ж�
            return true;
        return false;
    }

    public List<ChessPos> GetChessPosListByChessType(ChessType chessType)
    {
        if (!chess_type_pos_map.ContainsKey(chessType))
        {
            return null;
        }
        return chess_type_pos_map[chessType];
    }

    void OnScreenSizeChanged(int x, int y, Vector2 referenceResolution)
    {
        //����Ļ�����ı�ʱ����
        Debug.Log("��Ļ�������˸ı�");
    }

}
