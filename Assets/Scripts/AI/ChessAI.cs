using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

// ����AI
public class ChessAI : MonoBehaviour
{
    private ChessType m_ChessType;
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

    private ChessBoard m_chessBoard;

    public void SetBelongChessboard(ChessBoard chessBoard)
    {
        m_chessBoard = chessBoard;
    }

    private ChessPos GetRandomChessPos(List<ChessPos> chessPos)
    {
        if (chessPos.Count == 0) return ChessPos.none;

        int pos_index = Random.Range(0, chessPos.Count);
        return chessPos[pos_index];
    }

    /// <summary>
    /// ��ȡ��Ҫʤ��������λ�ü���
    /// </summary>
    /// <returns></returns>
    private List<ChessPos> GetWillWinPos()
    {
        List<ChessPos> current_has_place_chessPos = m_chessBoard.GetChessPosListByChessType(m_ChessType);
        if (current_has_place_chessPos != null || current_has_place_chessPos.Count == 0)
        {
            return null;
        }

        List<ChessPos>  win_pos_list = new List<ChessPos>();
        for (int i = 0; i < current_has_place_chessPos.Count; i++)
        {
            ChessPos chessPos = current_has_place_chessPos[i];
            if()
        }

    }

    public ChessPos GetNextChessPos()
    {
        //1���ж��ҷ��Ƿ���λ�ÿ���ֱ��ʤ�����߿�Ҫʤ���ˣ� ��������ʤ����λ�ã�û�������2

        //2���жϵ�ǰ�Է����Ӱڷ�����û�У��������������������һ��λ�ã� �н���3

        //3���жϵ�ǰ�Է�����ǰ���µ�����λ�õĸ����������ҡ����¡����ϡ����ϣ� �ж��ǲ����Ѿ���3�ż����������ˣ�����ǣ���ô����4
        //    ���ǣ� ����5
        //4����ʱ��Ҫ��ס�Է������ӣ������м��������
        //1���Է������Ŀţ�����AI�������أ� �м�û�п�λ����ʱ������õ������������λ��
        //2���Է��������ţ���û��AI�������أ� �м�û�п�λ����ʱ������õ��Ŀ���������λ�ã�
        //***���ף� ���ȡ���������˵�λ�á�
        // �У��ж�������һ����Ӯ�ĸ��ʸ���    
        //�ѣ��ж�������һ����Ӯ�ĸ��ʸ��������Ϳ�   ��
        //3���Է������Ŀţ���û��AI�������أ� �м��Ƿ��п�λ��
        //	�У��жϸ��Ŀ������м��λ���� ��λ��ԽС��Խ����
        //��
        //	***���ף� ��λ��ԽС��Խ���� ��
        // 	�У�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ� �����ӿ�λ������ж�����Ӯ�ĸ��ʴ�
        //	�ѣ�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ������ӿ�λ������ж�����Ӯ�ĸ��ʴ�
        //	���ж��ĸ�����������   ��

        //	û�У�ֱ���ж�Ϊ��
        //4���Է������Ŀţ�����AI�������أ� �м��п�λ��
        //	�У���
        //	***���ף� ��λ��ԽС��Խ���� ��
        // 	�У�ֻ��1�ŵ�ʱ�򷵻��м����λ�û�������û���ص����һ�������ż������ж�����Ӯ�ĸ��ʴ�
        //	�ѣ�ֻ��1�ŵ�ʱ�򷵻��м����λ�û�������û���ص����һ�������ż������ж�����Ӯ�ĸ��ʴ����ж��ĸ�����������   ��

        //5���Է��������ţ���û��AI�������أ� �м��п�λ
        //    �У���

        //    ***���ף� ��λ��ԽС��Խ���� ��
        // 	�У�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ� �����ӿ�λ������ж�����Ӯ�ĸ��ʴ�
        //	�ѣ�ֻ��1�ŵ�ʱ�򷵻��м����λ�û��������������һ�����ж�����Ӯ�ĸ��ʴ������ӿ�λ������ж�����Ӯ�ĸ��ʴ�
        //	���ж��ĸ�����������   ��

        //5�����������***���ף� ���ȡ������û�б��¹���λ�á�    �У�������ǰ�������Ѿ��µ�λ�ó����� �ж��ĸ������ϵ�������ӽ�ʤ������������ѡȡһ�� �ѣ�������ǰ�������Ѿ��µ�λ�ó����� �ж��ĸ������ϵ�������ӽ�ʤ������������ѡȡһ�� �� ������ͣ�ÿ�����ѡһ������ȥ���֡�
        if (!m_chessBoard)
        {
            return ChessPos.zero;
        }
        if (CheckByStep(p, array, 0, 1))    //����ֱ���ж�
            return true;
        if (CheckByStep(p, array, 1, 0))    //����ֱ���ж�
            return true;
        if (CheckByStep(p, array, 1, 1))    //�ҳ���ֱ���ж�
            return true;
        if (CheckByStep(p, array, -1, 1))   //�ҳ���ֱ���ж�
            return true;
    }

    private bool CheckByStep(ChessPos p, List<ChessPos> chess_array, int xdiff, int ydiff)
    {
           
        ChessPos tmp = new ChessPos(0, 0);
        int i;
        int cnt = 0;

        //�򷴷����ҵ���ɫ��ͬ�ĵ�
        for (i = 1; i < 5; i++)
        {
            tmp.x = p.x - xdiff * i;
            tmp.y = p.y - ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        for (i = 1; i < 5; i++)
        {
            tmp.x = p.x + xdiff * i;
            tmp.y = p.y + ydiff * i;
            if (!chess_array.Contains(tmp))
                break;
            cnt++;
        }

        if (cnt >= 4)
            return true;
        return false;
    }
}
