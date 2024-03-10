using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessInfo
{
    //������ɫ
    internal ChessType chess_type;
    //����λ��
    internal ChessPos chess_pos;
}

public class Chess : MonoEntity
{
    public Sprite white_chess_Sprite;
    public Sprite black_chess_Sprite;

    //������ɫ
    internal ChessType chess_type;
    //����λ��
    internal ChessPos chess_pos;

    private Image _m_chess_img;

    private void Awake()
    {
        _m_chess_img = GetComponent<Image>();
    }

    internal void SetInfo(ChessType chess_type, ChessPos chess_pos, Vector2 anchor_pos)
    {
        this.chess_type = chess_type;
        this.chess_pos = chess_pos;
        SetImg(chess_type);
        SetPosition(chess_pos.x, chess_pos.y, anchor_pos);
    }

    internal void SetInfo(ChessType chess_type, int x, int y, Vector2 anchor_pos)
    {
        SetInfo(chess_type, new ChessPos(x, y), anchor_pos);
    }

    internal void SetImg(ChessType chess_type)
    {
        switch (chess_type)
        {
            case ChessType.WHITE:
                _m_chess_img.sprite = white_chess_Sprite;
                break;
            case ChessType.BLACK:
                _m_chess_img.sprite = black_chess_Sprite;
                break;
        }
    }
    public void SetPosition(int xPos, int yPos, Vector2 anchor_pos)
    {
        chess_pos.x = xPos;
        chess_pos.y = yPos;
        _m_chess_img.GetComponent<RectTransform>().anchoredPosition =
            anchor_pos;
    }
}
