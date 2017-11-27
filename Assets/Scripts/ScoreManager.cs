using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading;
using UnityEditor;


public class ScoreManager : Singleton<ScoreManager>
{
    public List<int> ScoreList;

    private int _lowScoreCeiling = 3000;
    private int _mediumScoreCeiling = 7000;
    private int _highScoreCeiling = 10000;

    private string _scoreString; 

    private bool _timerRunning = false;

    private float _tempTime = 0;
	
    private IDictionary<string, float> _timeScoreMultipliers = new Dictionary<string, float>(){
        {"Listen and Guess", 0.5f},
        {"Look and Choose", 0.5f},
        {"Read and Complete", 1.0f}
    };

    private IDictionary<string, int> _congratulatoryList = new Dictionary<string, int>(){
        {"Amazing!", 2}, 
        {"Astounding!", 3}, 
        {"Brilliant!", 3}, 
        {"Great!", 2}, 
        {"Impressive!", 1}, 
        {"Marvelous!", 3}, 
        {"Outstanding!", 1}, 
        {"Phenomenal!", 2}, 
        {"Remarkable!", 1}, 
        {"Well Done!", 1}, 
        {"Fantastic!", 2}, 
        {"Sensational!", 2}, 
        {"Spectacular!", 2}, 
        {"Superb!", 3}, 
        {"Top-Notch!", 2}, 
        {"Excellent!", 2}, 
        {"Very Good!", 1}
    };
      
    private float _accuracyScoreConstant = 5000;
    private float _timeScoreConstant = 5000;
      
    private float _totalScore = 0;
    public float TotalScore
    {
        get
        {
            if (_totalScore == 0)
            {
                throw new UnityException("ScoreManager exception");
            }

            return _totalScore;
        }
    }

    private float _targetTimeScore = 0;
    private float _currentTimeScore = 0;
    private float _timeScore = 0;
    private float _modifiedTimeScore = 0;
    public float TimeScore
    {
        get
        {
            if (_modifiedTimeScore == 0)
            {
                throw new UnityException("ScoreManager exception");
            }

            return _modifiedTimeScore;
        }
    }

    private float _targetAccuracyScore = 0;
    private float _currentAccuracyScore = 0;
    private float _accuracyScore = 0;
    private float _modifiedAccuracyScore = 0;
    public float AccuracyScore
    {
        get
        {
            if (_modifiedAccuracyScore == 0)
            {
                throw new UnityException("ScoreManager exception");
            }

            return _modifiedAccuracyScore;
        }
    }
        

    private float _timeElapsed = 0; 
    public float TimeElapsed
    {
        get
        {
            if (_timerRunning == false)
            {
                return _timeElapsed;
            }
            else
            {   
                throw new UnityException("ScoreManager exception");
            }
        }
    }
	


    // increments maximum accuracy score and current accuracy score if the guess is correct
    public void IncrementScores(bool correct)
    {
        int wordLength = GameManager.Instance.TargetItem.Name.Length;

        string currentActivity = GameManager.Instance.CurrentActivityName;

        float timeMultiplier;

        _timeScoreMultipliers.TryGetValue(currentActivity, out timeMultiplier);

        float correctAccuracyIncrementAmount = (float)wordLength;
        float correctTimeIncrementAmount = (float)wordLength * timeMultiplier;

        float incorrectAccuracyPenalty = Random.Range(0.09f, 0.11f);

        float incorrectTimePenalty = Random.Range(1.9f, 2.1f);

        float incorrectAccuracyIncrementAmount = (float)wordLength * incorrectAccuracyPenalty;
        float incorrectTimeIncrementAmount = 0;

        if (_timeElapsed > correctTimeIncrementAmount)
        {
            incorrectTimeIncrementAmount = _timeElapsed * incorrectTimePenalty;
        }
        else
        {
            incorrectTimeIncrementAmount = correctTimeIncrementAmount * incorrectTimePenalty;
        }

//        Debug.Log("Incorrect Accuracy Amount: " + incorrectAccuracyIncrementAmount);
//        Debug.Log("Incorrect Time Amount: " + incorrectTimeIncrementAmount);
       
        if (_timerRunning == false)
        {
            _targetTimeScore += correctTimeIncrementAmount;

            _targetAccuracyScore += correctAccuracyIncrementAmount;

            if (correct == true)
            {
                _currentAccuracyScore += correctAccuracyIncrementAmount;

                _currentTimeScore += _timeElapsed;
            }
            else
            {
                _currentAccuracyScore += incorrectAccuracyIncrementAmount;

                _currentTimeScore += incorrectTimeIncrementAmount;
            }
        }
        else
        {
            throw new UnityException("ScoreManager exception");
        }
           
//        Debug.Log("Word Length: " + wordLength + " Target Time Score: " + _targetTimeScore + " Current Time Score: " + _currentTimeScore + " Target Accuracy Score: " + _targetAccuracyScore + " Current Accuracy Score: " + _currentAccuracyScore);

        return;
    }
        

    public void CalculateScore()
    {
        _accuracyScore = _targetAccuracyScore / _currentAccuracyScore;

//        Debug.Log("_targetAccuracyScore " + _targetAccuracyScore + " _currentAccuracyScore: " + _currentAccuracyScore + " _accuracyScoreConstant: " + _accuracyScoreConstant + " _accuracyScore: " + _accuracyScore);

        if (_targetTimeScore >= _currentTimeScore)
        {
            _timeScore = 1.0f;
        }
        else
        {
            _timeScore = _currentTimeScore / _targetTimeScore;
        }

        _modifiedAccuracyScore = 1.0f / _accuracyScore * _accuracyScoreConstant;

        _modifiedTimeScore = 1.0f / _timeScore * _timeScoreConstant;

        _totalScore = _modifiedTimeScore + _modifiedAccuracyScore;



//        Debug.Log("Modified Accuracy Score: " + _modifiedAccuracyScore);
//
//        Debug.Log("Modified Time Score: " + _modifiedTimeScore);
//
//        Debug.Log("Modified Total Score: " + _totalScore);

        return;
    }



    public void DisplayScore()
    {
        GameManager.Instance.Status = gameStatus.scorePanel;

        GameManager.Instance.SetInteractionStatus(GameManager.Instance.SelectedActivity, false);

        GameManager.Instance.ShowPanel();

        DisplayCongratulations();

        DisplayTotalScore();

        DisplayTopScores();

        FillScoreImages();
    }


    private void FillScoreImages()
    {
        float accuracyFillAmount = _modifiedAccuracyScore/_accuracyScoreConstant;
        float timeFillAmount = _modifiedTimeScore/_timeScoreConstant;

        GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("AccuracyImage").FindChild("Filler").GetComponent<Image>().fillAmount = accuracyFillAmount;
        GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("TimeImage").FindChild("Filler").GetComponent<Image>().fillAmount = timeFillAmount;
    }
        

    private void DisplayPrize(int place)
    {
//        Debug.Log("TEST1");

        Sprite prize = new Sprite();

        List<Sprite> prizeArray = new List<Sprite>();

        Debug.Log("TEST1");

        Debug.Log("Place: " + place );
        Debug.Log("ScoreList count: " + ScoreList.Count);

        if (place == 0)
        {
            prize = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/first_place.png", typeof(Sprite));
        }
        else if (place == 1)
        {
            prize = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/second_place.png", typeof(Sprite));
        }
        else if (place == 2)
        {
            prize = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/third_place.png", typeof(Sprite));
        }
        else
        {
            if (_totalScore > (0.9 * ScoreList[2]))
            {
                prize = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/gold_cup.png", typeof(Sprite));

            }
            else if (_totalScore > (0.8 * ScoreList[2]))
            {
                prize = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/silver_cup.png", typeof(Sprite));

            }
            else if (_totalScore > (0.7 * ScoreList[2]))
            {
                prize = (Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/bronze_cup.png", typeof(Sprite));
            }
            else 
            {
                if (_totalScore > (0.5 * ScoreList[2]))
                {
                    prizeArray.Add((Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/ribbon1.png", typeof(Sprite)));
                    prizeArray.Add((Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/ribbon2.png", typeof(Sprite)));
                    prizeArray.Add((Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/ribbon3.png", typeof(Sprite)));
                }
                else
                {
                    prizeArray.Add((Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/award1.png", typeof(Sprite)));
                    prizeArray.Add((Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/award2.png", typeof(Sprite)));
                    prizeArray.Add((Sprite)AssetDatabase.LoadAssetAtPath("Assets/Sprites/Background/Prizes/award3.png", typeof(Sprite)));
                }

                prize = prizeArray[Random.Range(0, 3)];
            }
        }

        Debug.Log("TEST2");

        GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("Prize").GetComponent<Image>().preserveAspect = true;
        GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("Prize").GetComponent<Image>().sprite = prize;

        Debug.Log("TEST3");
    }



    private void DisplayCongratulations()
    {
        int scoreIndex;

        if (_totalScore < _lowScoreCeiling)
        {
            scoreIndex = 1;
        }
        else if (_totalScore < _mediumScoreCeiling && _totalScore >= _lowScoreCeiling)
        {
            scoreIndex = 2;
        }
        else if (_totalScore <= _highScoreCeiling && _totalScore >= _mediumScoreCeiling)
        {
            scoreIndex = 3;
        }
        else
        {
            throw new UnityException("ScoreManager exception");
        }
            
        var keys = from entry in _congratulatoryList
                where entry.Value == scoreIndex
            select entry.Key;

//        foreach (var key in keys)
//        {
//            Debug.Log(key);
//        }
            
        int randomIndex = Random.Range (0, keys.Count<string>());

        string remark = keys.ElementAt<string>(randomIndex);

        Debug.Log(remark);

        GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("CongratulatoryRemark").GetComponent<Text>().text = remark;
    }


    private void DisplayTotalScore()
    {
        int totalScore = (int)_totalScore;

        string totalScoreString = "Total Score: " + totalScore.ToString();

        GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("TotalScoreText").GetComponent<Text>().text = totalScoreString;
    }



    public void DisplayTopScores()
    {
        LoadScores();

        bool inserted = false;

        int indexOfReplacement = -1;

        if (ScoreList.Count == 0)
        {
            ScoreList.Add((int)_totalScore);

            inserted = true;

            indexOfReplacement = 0;

//            Debug.Log("TEST CASE 1");
        }
        else
        {
//            Debug.Log("TEST CASE 2");

//            Debug.Log("ScoreList Count: " + ScoreList.Count);

            foreach (int element in ScoreList)
            {
//                Debug.Log("Element is: " + element.ToString());

                if ((int)_totalScore > element)
                {
//                    Debug.Log("TEST CASE 3");

                    indexOfReplacement = ScoreList.IndexOf(element);

//                    Debug.Log("Index is: " + indexOfReplacement.ToString());

                    ScoreList.Insert(indexOfReplacement, (int)_totalScore);

//                    Debug.Log("Total Score of: " + (int)_totalScore + " inserted into ScoreList");

                    inserted = true;

                    break;
                }
            }
        }

        Debug.Log("Insert status: " + inserted);
        Debug.Log("Insertion index: " + indexOfReplacement);

        if (inserted == false)
        {
            ScoreList.Add((int)_totalScore);

            indexOfReplacement = ScoreList.IndexOf((int)_totalScore);
        }

        if (ScoreList.Count > 3)
        {
            ScoreList.Take(3);

            indexOfReplacement = -1;
        }
            
//        Debug.Log("Final Score List length is: " + ScoreList.Count);
//
        foreach(int scoreElement in ScoreList)
        {
            Debug.Log("Score Element is: " + scoreElement.ToString());
        }

        string score;

//        Debug.Log("Score Panel reference is: " + scorePanel);

        for (int i=0; i<3; i++)
        {
            score = ScoreList.ElementAtOrDefault(i).ToString();

            if (score == "0")
            {
                score = "-";
            }
                
//            Debug.Log("Text score to insert into Top Scores is: " + score);

            GameManager.Instance.SelectedActivity.transform.FindChild("Score").FindChild("Top Score Canvas").GetChild(i).GetComponent<Text>().text = score;
        }

        DisplayPrize(indexOfReplacement);

        SaveScores();
    }


    public void SaveScores()
    {
        if (ScoreList == null || ScoreList.Count == 0)
        {
            throw new UnityException("SaveScore() Exception");
        }
          
//        foreach(int element in ScoreList)
//        {
//            Debug.Log("Element #" + ScoreList.IndexOf(element) + " = " + element);
//        }

        _scoreString = string.Empty;

        foreach (int score in ScoreList)
        {
            _scoreString += score.ToString() + ";";
        }

        Debug.Log("Saved Scores are: " + _scoreString);

        PlayerPrefs.SetString("Scores", _scoreString);

        ScoreList.Clear();

        _scoreString = null;  
    }
       

    public void LoadScores()
    {
//        ResetSavedScores();

        _scoreString = PlayerPrefs.GetString("Scores");

        Debug.Log("Loaded Scores are: " + _scoreString);

        if( _scoreString == null )
        {
            throw new UnityException("LoadScore() exception");
        }
            
        char splitterCharacter = ';';

        List<string> scoreStrings = new List<string> (_scoreString.Split(splitterCharacter));

        scoreStrings.Remove("");

        ScoreList = new List<int>();

        foreach(string s in scoreStrings)
        {
            ScoreList.Add(int.Parse(s));
        }

//        foreach (int element in ScoreList)
//        {
//            Debug.Log("Score is: " + element.ToString());
//        }
    }
        

    public void ResetSavedScores()
    {
        PlayerPrefs.DeleteAll();
    }

    public void ResetScoreManager()
    {
        ScoreList = null;
        _scoreString = null; 
        _timerRunning = false;
        _tempTime = 0;
        _totalScore = 0;
        _targetTimeScore = 0;
        _currentTimeScore = 0;
        _timeScore = 0;
        _modifiedTimeScore = 0;
        _targetAccuracyScore = 0;
        _currentAccuracyScore = 0;
        _accuracyScore = 0;
        _modifiedAccuracyScore = 0;
        _timeElapsed = 0; 
    }

    public void ResetTimer()
    {
        _timerRunning = false;

        _tempTime = 0;

        _timeElapsed = 0;
    }

    public void StartTimer()
    {
        if (_timerRunning == true)
        {
            throw new UnityException("StartTimer() exception");
        }

        ResetTimer();

        _timerRunning = true;

        _tempTime = Time.time;

//        Debug.Log("Start time: " + Time.time);

//        Thread.Sleep(5000);
    }

    public void StopTimer()
    {
        if (_timerRunning == false)
        {
            throw new UnityException("StopTimer() exception");
        }

        _timerRunning = false;

        _timeElapsed += Time.time - _tempTime;

//        Debug.Log("Stop time: " + Time.time);
    }
}
