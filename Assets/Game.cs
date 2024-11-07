using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class Game : MonoBehaviour
{
    enum State
    {
        Start = 0,
        Game,
        End
    }

    enum Turn{
        I = 0,
        You
    }

    enum Stone
    {
        None = 0,
        White,
        Black
    }

    Tcp tcp;
    public InputField ip;

    public Texture texBoard;
    public Texture texWhite;
    public Texture texBlack;

    int[] board = new int[9];
    State state;

    Stone stoneTurn;
    Stone stoneI;
    Stone stoneYou;
    Stone stoneWinner;

    // Start is called before the first frame update
    void Start()
    {
        tcp = GetComponent<Tcp>();

        state = State.Start;

        for (int i = 0; i < board.Length; i++)
        {
            board[i] = (int)Stone.None;
        }
    }

    public void ServerStart()
    {
        tcp.StartServer(10000, 10);
    }

    public void ClientStart()
    {
        tcp.Connect(ip.text, 10000);
    }



    // Update is called once per frame
    void Update()
    {
        if (!tcp.IsConnect()) return;

        if (state == State.Start)
        {
            UpdateStart();
        }

        if (state == State.Game)
        {
            UpdateGame();
        }

        if (state == State.End)
        {
            UpdateEnd();
        }

    }

    void UpdateStart()
    {
        state = State.Game;
        stoneTurn = Stone.White;

        if(tcp.IsServer())
        {
            stoneI = Stone.White;
            stoneYou = Stone.Black;
        }
        else{
            stoneI = Stone.Black;
            stoneYou = Stone.White;
        }
    }




    void UpdateGame()
    {
        bool bSet = false;

        if(stoneTurn == stoneI)
        {
            bSet = MyTurn();
        }

    }

    bool SetStone(int i, Stone stone)
    {
        if(board[i] == (int)Stone.None)
        {
            board[i] = (int)stone;
            return true;    
        }
        return false;
    }

    int PosToNumber(Vector3 pos)
    {
        float x = pos.x - 50;
        float y = Screen.height - pos.y - 50;

        if( x<0.0f || x>=300.0f)
        {
            return -1;
        }

        if (y < 0.0f || y >= 300.0f)
        {
            return -1;
        }

        int h = (int)(x / 100.0f);
        int v = (int)(y / 100.0f);

        int i = v * 3 + h;

        return i;
    }



    bool MyTurn()
    {
        bool bClick = Input.GetMouseButtonDown(0);
        if (!bClick)
        {
            return false;
        }

        Vector3 pos = Input.mousePosition;

        int i = PosToNumber(pos);
        if (i == -1)
            return false;

        bool bSet = SetStone(i, stoneI);
        if(bSet==false)
            return false;

        byte[] data = new byte[1];
        data[0] = (byte)i;
        tcp.Send(data, data.Length);

        Debug.Log("º¸³¿: " + i);

        return true;
    }

    void UpdateEnd()
    {

    }



    private void OnGUI()
    {
        if (!Event.current.type.Equals(EventType.Repaint))
        {
            return;
        }

        Graphics.DrawTexture(new Rect(0, 0, 400, 400), texBoard);

        for(int i=0; i<board.Length; ++i)
        {
            if(board[i]!=(int)Stone.None){
                float x = (i % 3) * 100 + 50;
                float y = (i / 3) * 100 + 50;

                Texture tex = (board[i] == (int)Stone.White) ?
                                texWhite : texBlack;
                Graphics.DrawTexture(new Rect(x, y, 100, 100), tex);
            }
        }

    }
}
