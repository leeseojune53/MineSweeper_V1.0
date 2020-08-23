using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    public Element CloneBlock = null;

    public int WidthBlock = 10;
    public int HeightBlock = 13;
    public int CountofMines = 10;
    public int RemineCountofMines = 0;
    float xSize, ySize;
    public Text Timetxt;
    public bool game_over;
    private Element Clone_Block;
    public Element Easy, Middle, Hard;

    public int Easy_Mine;

    public Element[,] ElementArray = null;

    public Text txt; // 지뢰개수 - 깃발개수

    public int flag_count;
    private float time;

    public bool Game_over_get()
    {
        return game_over;
    }

    public void Easy_count()
    {
        CloneBlock = Easy;
        CountofMines = Easy_Mine;
    }

    public void Game_start()
    {
        game_over = false;
    }

    public void Game_overc()
    {
        game_over = true;
    }

    private void Awake()
    {
        ElementArray = new Element[WidthBlock, HeightBlock];
        time = 0f;
    }

    Element CloneElement(int p_x, int p_y)
    {
        GameObject copyobj = GameObject.Instantiate(CloneBlock.gameObject);
        copyobj.transform.SetParent(this.transform);
        Vector2 temppos = new Vector2(p_x, p_y);
        copyobj.transform.localPosition = temppos;
        copyobj.name = "CloneBlock_" + p_x.ToString() + "_" + p_y.ToString();

        return copyobj.GetComponent<Element>();
    }

    void GeneratorMineSweeper()
    {
        Vector2 temppos = Vector2.zero;
        for(int yy = 0; yy < HeightBlock; ++yy)
        {
            for(int xx = 0; xx < WidthBlock; ++xx)
            {
                   ElementArray[xx,yy] = CloneElement(xx,yy);
            }
        }
        //지뢰세팅
        SetMinesSetting();

    }

    List<Element> m_TempElementList = new List<Element>();
    void SetMinesSetting()
    {
        m_TempElementList.Clear();
        //m_TempElementList.AddRange(ElementArray.GetEnumerator());

        foreach(var item in ElementArray)
        {
            m_TempElementList.Add(item);
        }
        int randomindex = -1;

        for(int i = 0; i < CountofMines; i++)
        {
            randomindex = Random.Range(0,m_TempElementList.Count);

            m_TempElementList[randomindex].SetElementDatas(true);

            m_TempElementList.RemoveAt(randomindex);
        }

    }
    
    bool GetMineAt(int p_x, int p_y)
    {
        if((p_x>=0 && p_x<WidthBlock)&&(p_y>=0 && p_y < HeightBlock))
        {
            return ElementArray[p_x, p_y].IsMine;
        }
        return false;
    }
    public int GetRountMines(int p_x, int p_y)
    {
        int outcount = 0;
        //상단
        if (GetMineAt(p_x - 1, p_y + 1)){ ++outcount; }
        if (GetMineAt(p_x, p_y + 1)) { ++outcount; }
        if (GetMineAt(p_x + 1, p_y + 1)) { ++outcount; }
        //가운데
        if (GetMineAt(p_x - 1, p_y)) { ++outcount; }
        if (GetMineAt(p_x + 1, p_y)) { ++outcount; }
        //하단 
        if (GetMineAt(p_x - 1, p_y - 1)) { ++outcount; }
        if (GetMineAt(p_x, p_y - 1)) { ++outcount; }
        if (GetMineAt(p_x + 1, p_y - 1)) { ++outcount; }


        return outcount;
    }

    public void FFuncCover(int p_x, int p_y, bool[,] p_visited)
    {

        if ((p_x >= 0 && p_x < WidthBlock) && (p_y >= 0 && p_y < HeightBlock))
        {
            if (p_visited[p_x, p_y])
                return;

            int aroundcount = GetRountMines(p_x, p_y);
            ElementArray[p_x, p_y].SetChangeTexture(aroundcount);//
            if (aroundcount > 0)
                return;

            p_visited[p_x, p_y] = true;

            FFuncCover(p_x + 1, p_y, p_visited);
            FFuncCover(p_x - 1, p_y, p_visited);
            FFuncCover(p_x, p_y + 1, p_visited);
            FFuncCover(p_x, p_y - 1, p_visited);
            FFuncCover(p_x + 1, p_y + 1, p_visited);
            FFuncCover(p_x - 1, p_y + 1, p_visited);
            FFuncCover(p_x - 1, p_y - 1, p_visited);
            FFuncCover(p_x + 1, p_y - 1, p_visited);
        }
    }

    public bool IsFinised()
    {
        foreach(var item in ElementArray)
        {
            if(item.IsCovered() && !item.IsMine)//
            {
                
                return false;
            }
        }
        return true;
    }

    public void UnCoverMines()
    {
        foreach(var item in ElementArray)
        {
            if (item.IsMine)
            {
                item.SetChangeTexture(0);//
            }
        }
        ReseGame();

    }

    public void Text_print(int n)
    {
        if (n==1)
        {
            --flag_count;
            txt.text = flag_count.ToString();
        }
        else
        {
            ++flag_count;
            txt.text = flag_count.ToString();
        }
    }

    void Start()
    {
        flag_count = CountofMines;
        txt.text = flag_count.ToString();
        GeneratorMineSweeper();
        xSize = GetSpriteSize(CloneBlock.gameObject).x;
        ySize = GetSpriteSize(CloneBlock.gameObject).y;
        game_over = false;
    }

    public Vector3 GetSpriteSize(GameObject _target)
    {
        Vector3 worldSize = Vector3.zero;
        if (_target.GetComponent<SpriteRenderer>())
        {
            Vector2 spriteSize = _target.GetComponent<SpriteRenderer>().sprite.rect.size;
            Vector2 localSpriteSize = spriteSize / _target.GetComponent<SpriteRenderer>().sprite.pixelsPerUnit;
            worldSize = localSpriteSize;
            worldSize.x *= _target.transform.lossyScale.x;
            worldSize.y *= _target.transform.lossyScale.y;
        }
        else
        {
            Debug.Log("SpriteRenderer Null");
        }
        return worldSize;
    }

    private void Update()
    {
        if (!game_over&&!IsFinised())
        {
            time += Time.deltaTime;
            Timetxt.text = Mathf.Ceil(time).ToString();
            if (Input.GetMouseButtonDown(1))//마우스 우클릭(깃발)
            {
                ElementArray[
                        Mathf.RoundToInt((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).x /
                                         xSize),
                        Mathf.RoundToInt((Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).y /
                                         ySize)].Flag();
            }
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReseGame();
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }


    public void ReseGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        game_over = false;
    }
}
