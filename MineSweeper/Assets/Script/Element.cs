using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

using UnityEngine.UI;


public class Element : MonoBehaviour
{
    public Sprite[] ChangeSpriteArray = null;
    private SpriteRenderer m_SpritRender = null;
    public Sprite MineSprite = null;
    public Sprite FlagSprite = null;
    protected GridManager GridLinkManager = null;
    private bool check;
    
    [SerializeField]
    private bool m_IsMine = false;

    public bool IsMine
    {
        get { return m_IsMine; }
        protected set { m_IsMine = value; }
    }

    public void SetElementDatas(bool p_ismine)
    {
        m_IsMine = p_ismine;
    }
    public void SetChangeTexture(int p_index)
    {
        m_SpritRender.sprite = ChangeSpriteArray[p_index];
    }


    void Start()
    {
        m_SpritRender = this.GetComponent<SpriteRenderer>();
        GridLinkManager = GameObject.FindObjectOfType<GridManager>();
        GridLinkManager.game_over = false;
        //확인용
        //SetChangeTexture(0);
    }

     
    void OnMouseDown()
    {
        //check = GridLinkManager.Game_over_get();
        UnityEngine.Debug.LogFormat("블럭이 눌렸습니다 : {0}", this.name);
        if(GetComponent<SpriteRenderer>().sprite != ChangeSpriteArray[9]&& !GridLinkManager.game_over)
        {
        if (m_IsMine)
            {
                //게임오버
                GridLinkManager.game_over = true;
                UnityEngine.Debug.LogFormat("게임오버");
                m_SpritRender.sprite = MineSprite;
                //GridLinkManager.ReseGame();
            }
            else
            {
                int x = (int)this.transform.localPosition.x;
                int y = (int)this.transform.localPosition.y;

                //주변 지뢰 탐색
                //int rountminecount = GridLinkManager.GetRountMines(1, 1);
                SetChangeTexture(GridLinkManager.GetRountMines(x, y));

                bool[,] visitbool = new bool[GridLinkManager.WidthBlock, GridLinkManager.HeightBlock];

                GridLinkManager.FFuncCover(x, y, visitbool);

                if (GridLinkManager.IsFinised())
                {
                    UnityEngine.Debug.LogFormat("스테이지 클리어!");
                    //GridLinkManager.ReseGame();
                }
            }
        }
    }

    public void Flag()
    {
        if (GetComponent<SpriteRenderer>().sprite == ChangeSpriteArray[10])
        {
            SetChangeTexture(9);
            GridLinkManager.Text_print(1);
        }
        else if(GetComponent<SpriteRenderer>().sprite == ChangeSpriteArray[9])
        {
            GridLinkManager.Text_print(0);
            SetChangeTexture(10);
        }
    }



    public bool IsCovered()
    {
        return m_SpritRender.sprite.texture.name == "Idle";
    }

    void OnDrawGizmos()
    {
        if (m_IsMine)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(this.transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        }
    }


}
