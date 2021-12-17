using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text BestScore;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;

    public int HighestScore = 0;
    public string HighestName;

    private bool m_GameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }
        ScoreText.text = MenuUIHandler.theName + "'s " + $"Score : {m_Points}";
        //load the Best score and name, and display now.
        LoadScore();

    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = MenuUIHandler.theName + "'s " + $"Score : {m_Points}";
    }

    public void GameOver()
    {
        SaveHighestScore();
        m_GameOver = true;
        GameOverText.SetActive(true);
        //note: compare the current score with the best score,and display Best score name and best score now.
        //save the above data to json if necessary
    }

    [System.Serializable]
    class SaveData
    {
        public int HighestScore;
        public string HighestName;
    }

    public void SaveHighestScore()
    {
        if (m_Points > HighestScore)
        {
            SaveScore();
        }
    }
    
    public void SaveScore()
    {
        SaveData data = new SaveData();
                
        data.HighestScore = m_Points;
        data.HighestName = MenuUIHandler.theName;

        string json = JsonUtility.ToJson(data);

        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
    }

    public void LoadScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            HighestScore = data.HighestScore;
            HighestName = data.HighestName;

            BestScore.text = $"BestScore : {HighestName} : {HighestScore}";
        }

    }

}
