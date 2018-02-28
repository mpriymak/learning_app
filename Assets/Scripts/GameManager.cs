using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Threading;

public enum gameStatus
{
	mainMenu, categoriesPanel, scorePanel,
	listenAndGuessActivity, lookAndChooseActivity, readAndCompleteActivity, about
}
	
public class GameManager : Singleton<GameManager>
{

//	[SerializeField]
//	private Button _listenAndGuessBtn;
//	[SerializeField]
//	private Button _lookAndChooseBtn;
//	[SerializeField]
//	private Button _readAndCompleteBtn;
//	[SerializeField]
//	private GameObject _mainMenuPanel;
//	[SerializeField]
//	private GameObject _listenAndGuess;
//	[SerializeField]
//	private GameObject _lookAndChoose;
//	[SerializeField]
//	private GameObject _readAndComplete;
//	[SerializeField]
//	private GameObject _listenAndGuessActivity;
//	[SerializeField]
//	private GameObject _lookAndChooseActivity;
//	[SerializeField]
//	private GameObject _readAndCompleteActivity;
//	[SerializeField]
//	private GameObject _aboutPanel;


	[SerializeField]
	public int _totalNumberOfQuestions;

	public  Item[] _Animals;

    public  List<GameObject> _PanelList;


	private List<Item> _tempAnimalList;

	private List<GameObject> _tempSlotList; 
	private List<GameObject> _tempLetterList;

	private gameStatus _status;

    public gameStatus Status
    {
        get
        {
            return _status;
        }

        set
        {
            _status = value;
        }
    }

	private int _targetIndex;

	private Item _TargetItem;
    public Item TargetItem
    {
        get 
        {
            return _TargetItem;
        }
    }
	 

	private string _listenAndGuessActivityName = "Listen and Guess";
	private string _lookAndChooseActivityName = "Look and Choose";
	private string _readAndCompleteActivityName = "Read and Complete";

	private string _currentActivityName = null;
    public string CurrentActivityName
    {
        get
        {
            return _currentActivityName;
        }
    }

	private string _currentCategoryName = null;

	private int _currentQuestionNumber = 0;
	private int _correctAnswerNumber = 0;

  

    public int CurrentQuestionNumber
    {
        get
        {
            return _currentQuestionNumber;
        }
        set
        {
            _currentQuestionNumber = value;
        }
    }

    public int CorrectAnswerNumber
    {
        get
        {
            return _correctAnswerNumber;
        }
        set
        {
            _correctAnswerNumber = value;
        }
    }


	public delegate void Method();

	private GameObject _selectedActivity; 

	public GameObject SelectedActivity
	{
		get
		{
			return _selectedActivity;
		}
	}

	private List<Item> _RandomizedItems;

	private List<int> _Indexes;


	private int _minimumNumberOfLetters = 18;

	private int _maximumNumberOfLetters = 20;

	private List<char> _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToList<char>();

    public GameObject letterPrefab;
    public GameObject slotPrefab;

   
   
	// Use this for initialization
	void Start () 
	{

//        ScoreManager.Instance.ScoreList.Add(1);
//        ScoreManager.Instance.ScoreList.Add(2);
//        ScoreManager.Instance.ScoreList.Add(3);
//       
//        ScoreManager.Instance.SaveScore();


//        ScoreManager.Instance.LoadScore();

//        ScoreManager.Instance.StartTimer();
//
//        yield return new WaitForSeconds(5.0f);
//
//        ScoreManager.Instance.StopTimer();
//
//        Debug.Log("Timer Result: " + ScoreManager.Instance.TimeElapsed);



		DisableAllPanels();

		_status = gameStatus.mainMenu;

//		_selectedActivity = new GameObject();

		// assigns items to a temporary list from a global list of items
		// find a better place to put the following 2 lines
		_tempAnimalList = new List<Item>();
		_tempAnimalList.AddRange (_Animals); 

		ShowPanel();
	}



//    public void Update()
//    {
//        float currentTime = Time.time;
//
//        Debug.Log(currentTime);
//    }



	public void DisableAllPanels()
	{
		foreach(GameObject panel in _PanelList)
		{
			panel.SetActive(false);
		}
	}


	public void ShowPanel()
	{
//		Debug.Log("STATUS: " + _status);

        if(_PanelList == null)
        {
            throw new UnityException("Panel List is not assigned");
        }

//        for(int i = 0; i<_PanelList.Count; i++)
//        {
//            Debug.Log(_PanelList[i].name);
//        }





		switch(_status)
		{
            case gameStatus.mainMenu:
                _selectedActivity = _PanelList[0];
				break;
            case gameStatus.categoriesPanel:
                _selectedActivity = _PanelList[1];
				break;
			case gameStatus.listenAndGuessActivity:
                _selectedActivity = _PanelList[2];
                break;
			case gameStatus.lookAndChooseActivity:
                _selectedActivity = _PanelList[3];
				break;
			case gameStatus.readAndCompleteActivity:
                _selectedActivity = _PanelList[4];	
				break;
			case gameStatus.about:
                _selectedActivity = _PanelList[5];	
				break;
            case gameStatus.scorePanel:
                _selectedActivity = _PanelList[6];
                break;
		}

       
//        int i = 0;
//        if (_selectedActivity.tag != "ScorePanel")
//        {
            foreach (GameObject item in _PanelList)
            {
                //			Debug.Log(item.name);

                if (item.tag != _selectedActivity.tag)
                {
                    item.SetActive(false);

                    //                i++;
                }
                //            else
                //            {
                ////                Debug.Log("Enabled item: " + item.name);
                //            }
            }
//        }

//        Debug.Log("Disabled count: " + i);
//
//        for(int i=0;i<_PanelList.Count;i++)
//        {
//            if(_PanelList[i].name != _selectedActivity.name)
//            {
//                _PanelList[i].SetActive(false);
//            }
//        }


		_selectedActivity.SetActive(true);

//        Debug.Log("Enabled item: " + _selectedActivity.name);
//        Debug.Log("Enabled status in hierarchy: " + _selectedActivity.activeInHierarchy);
//        Debug.Log("Enabled status: " + _selectedActivity.activeSelf);

//        Debug.Log("Previous: " + previousStatus);
//        Debug.Log("Current: " + _selectedActivity.activeSelf);

        AssignLabels();
	}


    private void AssignLabels()
    {
        if(_selectedActivity.transform.Find("Top Bar")!=null)
        {
            _selectedActivity.transform.Find("Top Bar").Find("Label").GetComponent<Text>().text = _currentActivityName;

            if(_currentCategoryName != null)
            {
                _selectedActivity.transform.Find("Top Bar").Find("Label").GetComponent<Text>().text += (": " + _currentCategoryName);
            }
        }

        if (_selectedActivity.transform.Find("Question Counter") != null)
        {
            _selectedActivity.transform.Find("Question Counter").GetComponent<Text>().text = "Current Question: " + _currentQuestionNumber + "/" + _totalNumberOfQuestions;
        }

        if (_selectedActivity.transform.Find("Correct Counter") != null)
        {
            _selectedActivity.transform.Find("Correct Counter").GetComponent<Text>().text = "Correct: " + _correctAnswerNumber + "/" + _totalNumberOfQuestions;
        }
    }


	public void MenuButtonClicked()
	{
		SoundManager.Instance.PlayClick();

		string buttonTag = EventSystem.current.currentSelectedGameObject.tag;

//		Debug.Log("CLICKED BUTTON NAME: " + buttonTag);

		switch (buttonTag)
		{
			case "ListenAndGuessButton":
				_status = gameStatus.categoriesPanel;
				_currentActivityName = _listenAndGuessActivityName;
				break;
			case "LookAndChooseButton":
				_status = gameStatus.categoriesPanel;
				_currentActivityName = _lookAndChooseActivityName;
				break;
			case "ReadAndCompleteButton":
				_status = gameStatus.categoriesPanel;
				_currentActivityName = _readAndCompleteActivityName;
				break;
            case "AboutButton":
                _status = gameStatus.about;
                _currentActivityName = null;
				break;
            case "HomeButton":
                _status = gameStatus.mainMenu;
                ResetGameParameters();
                ResetLabels();
				break;
			default:
				throw new UnityException("MenuButtonClicked() exception");
		}

//        Debug.Log("ACTIVITY SELECTED: " + _currentActivityName);

//        Debug.Log("STATUS: " + _status);

		ShowPanel ();
	}


    public void ResetGameParameters()
    {
        _correctAnswerNumber = 0;
        _currentQuestionNumber = 0;
    }

    public void ResetLabels()
    {
        _currentActivityName = null;
        _currentCategoryName = null;
    }

    public void DisableCorrectIncorrect()
    {
        _selectedActivity.transform.Find ("Correct").gameObject.SetActive (false);
        _selectedActivity.transform.Find ("Incorrect").gameObject.SetActive (false);
    }

    public void DisableInteractionUntilPronunciationPlayed()
    {
        if(_currentActivityName == "Listen and Guess")
        {
            _selectedActivity.transform.Find("Item 1").GetComponent<Button>().interactable = false;
            _selectedActivity.transform.Find("Item 2").GetComponent<Button>().interactable = false;
            _selectedActivity.transform.Find("Item 3").GetComponent<Button>().interactable = false;
            _selectedActivity.transform.Find("Item 4").GetComponent<Button>().interactable = false;
        }
    }

	public void AnimalsBtnPressed()
	{
        SoundManager.Instance.PlayClick();

        _currentCategoryName = "Animals";

        AssignLabels();

		switch (_currentActivityName) 
		{
			case "Listen and Guess":
				_status = gameStatus.listenAndGuessActivity;
				break;
			case "Look and Choose":
				_status = gameStatus.lookAndChooseActivity;
				break;
			case "Read and Complete":
				_status = gameStatus.readAndCompleteActivity;
				break;
		}

		ClearGameObjectList(_tempLetterList);

		ClearGameObjectList(_tempSlotList);

		ShowPanel();

        DisableCorrectIncorrect();

		SetInteractionStatus (_selectedActivity, true);

        DisableInteractionUntilPronunciationPlayed();

        if (_currentActivityName != "Listen and Guess")
        {
            ScoreManager.Instance.StartTimer();
        }
    
//		Selects a random item from the temporary list and removes it from this list to avoid selecting duplicates in the future
		int randomIndex = Random.Range (0, _tempAnimalList.Count );
		_TargetItem = _tempAnimalList[randomIndex];
		_tempAnimalList.RemoveAt(randomIndex);

		_tempSlotList = new List<GameObject>();
		_tempLetterList = new List<GameObject>();

		if ((_status == gameStatus.listenAndGuessActivity) || (_status == gameStatus.lookAndChooseActivity)) 
		{
			//		selects 3 random unique items from a list and assigns to a list
			_RandomizedItems = new List<Item> (3);
			_Indexes = GenerateRandom (3, 0, _tempAnimalList.Count - 1);
			for (int j = 0; j < _Indexes.Count; j++) 
			{
				_RandomizedItems.Add (_tempAnimalList [_Indexes [j]]);
			}

			//		inserts target item into the randomized list of 3 items at random index
			_RandomizedItems.Insert (Random.Range (0, 4), _TargetItem);

			_targetIndex = _RandomizedItems.IndexOf (_TargetItem);
		}
		else 
		{
			List<char> capitalizedName = new List<char> ();

			capitalizedName = _TargetItem.Name.ToUpper ().ToList<char> ();

			List<char> capitalizedNameNoSpace = new List<char> ();

			capitalizedNameNoSpace = _TargetItem.Name.Replace (" ", string.Empty).ToUpper ().ToList<char> (); 

			int numberOfLettersToGenerate;

			if (capitalizedNameNoSpace.Count < _minimumNumberOfLetters) 
			{
				numberOfLettersToGenerate = _minimumNumberOfLetters - capitalizedNameNoSpace.Count;
			} 
			else if(capitalizedNameNoSpace.Count > _minimumNumberOfLetters && capitalizedNameNoSpace.Count < _maximumNumberOfLetters) 
			{
				numberOfLettersToGenerate = _maximumNumberOfLetters - capitalizedNameNoSpace.Count;
			} 
			else 
			{
				throw new UnityException ("Number of letters to generate is too large");
			}

			List<char> alphabetRemainder = _alphabet.ToList ();

			foreach (char letter in capitalizedNameNoSpace) 
			{
				alphabetRemainder.Remove (letter);
			}

			List<int> randomLetterIndexes = GenerateRandom (numberOfLettersToGenerate, 0, alphabetRemainder.Count - 1); 

			List<char> generatedRandomLetters = randomLetterIndexes.Select (k => alphabetRemainder [k]).ToList ();
           
			List<char> randomLetters = capitalizedNameNoSpace.Concat (generatedRandomLetters).ToList ();

			Shuffle<char> (randomLetters);  

			foreach (char letter in capitalizedName) 
			{
				if (char.IsWhiteSpace (letter)) 
				{
					GameObject letterSlot = new GameObject ();

					letterSlot.transform.SetParent (_selectedActivity.transform.Find ("Item Spelling").transform);

					letterSlot.AddComponent<LayoutElement> ();

					_tempSlotList.Add(letterSlot);
				} 
				else 
				{
					GameObject letterSlot = Instantiate (slotPrefab, _selectedActivity.transform.Find ("Item Spelling").transform);

					letterSlot.GetComponentInChildren<Text> ().text = letter.ToString ();

					letterSlot.transform.Find ("Letter").gameObject.SetActive (false);

					_tempSlotList.Add(letterSlot);
				}
			}

			DropHandler.wordLength = capitalizedName.Count;

			foreach (char letter in randomLetters) 
			{
				GameObject randomLetter = Instantiate (letterPrefab, _selectedActivity.transform.Find ("Letters").transform);

				randomLetter.GetComponentInChildren<Text> ().text = letter.ToString ();

				_tempLetterList.Add(randomLetter);
			}
		}
			
		switch (_status) 
		{
			case gameStatus.listenAndGuessActivity:
				_selectedActivity.transform.Find ("Item 1").gameObject.GetComponent<Image> ().sprite = _RandomizedItems [0].Image;
				_selectedActivity.transform.Find ("Item 2").gameObject.GetComponent<Image> ().sprite = _RandomizedItems [1].Image;
				_selectedActivity.transform.Find ("Item 3").gameObject.GetComponent<Image> ().sprite = _RandomizedItems [2].Image;
				_selectedActivity.transform.Find ("Item 4").gameObject.GetComponent<Image> ().sprite = _RandomizedItems [3].Image;
				break;
			case gameStatus.lookAndChooseActivity:
				_selectedActivity.transform.Find ("Button 1").gameObject.GetComponentInChildren<Text> ().text = _RandomizedItems [0].Name;
				_selectedActivity.transform.Find ("Button 2").gameObject.GetComponentInChildren<Text> ().text = _RandomizedItems [1].Name;
				_selectedActivity.transform.Find ("Button 3").gameObject.GetComponentInChildren<Text> ().text = _RandomizedItems [2].Name;
				_selectedActivity.transform.Find ("Button 4").gameObject.GetComponentInChildren<Text> ().text = _RandomizedItems [3].Name;

				_selectedActivity.transform.Find ("Item").gameObject.GetComponent<Image> ().preserveAspect = true;
				_selectedActivity.transform.Find ("Item").gameObject.GetComponent<Image> ().sprite = _RandomizedItems [_targetIndex].Image;
				break;
			case gameStatus.readAndCompleteActivity:
				_selectedActivity.transform.Find ("Item Image").gameObject.GetComponent<Image> ().preserveAspect = true;
				_selectedActivity.transform.Find ("Item Image").gameObject.GetComponent<Image> ().sprite = _TargetItem.Image;
				break;
		}
			
	}

    public string ListToString<T>(List<T> list)
    {
        StringBuilder sb = new StringBuilder();

        foreach(T item in list)
        {
            sb.Append(item);
        }

        return sb.ToString();
    }

	public void PlayPronunciation()
	{
		SoundManager.Instance.PlayClip (_TargetItem.Audio);

        ScoreManager.Instance.StartTimer();

        _selectedActivity.transform.Find("Item 1").GetComponent<Button>().interactable = true;
        _selectedActivity.transform.Find("Item 2").GetComponent<Button>().interactable = true;
        _selectedActivity.transform.Find("Item 3").GetComponent<Button>().interactable = true;
        _selectedActivity.transform.Find("Item 4").GetComponent<Button>().interactable = true;
	}


//	// Base Implementation: waits for a set number of seconds before continuing
//	public IEnumerator Wait(float duration)
//	{
//		yield return new WaitForSeconds (duration);
//	}

    // wait for a set number of seconds before executing a method passed in as a delegate
    public IEnumerator Wait(float duration, Method executeThis)
    {
        yield return new WaitForSeconds (duration);

        executeThis ();
    }

  

	public void QuestionComplete(bool isCorrect)
	{
        Method method;

        if (isCorrect == true) 
    	{
    		SoundManager.Instance.PlayEffect ("correct");

    		_selectedActivity.transform.Find ("Correct").gameObject.SetActive (true);
    		_selectedActivity.transform.Find ("Incorrect").gameObject.SetActive (false);

    		_correctAnswerNumber += 1;
    	} 
    	else if (isCorrect == false) 
    	{
    		SoundManager.Instance.PlayEffect ("incorrect");

    		_selectedActivity.transform.Find ("Correct").gameObject.SetActive (false);
    		_selectedActivity.transform.Find ("Incorrect").gameObject.SetActive (true);
    	} 
    	else 
    	{
    		throw new UnityException ("GameManager exception");
    	}

        ScoreManager.Instance.StopTimer();

        ScoreManager.Instance.IncrementScores(isCorrect);
       
    	_currentQuestionNumber += 1;

        if (_currentQuestionNumber == _totalNumberOfQuestions)
        {
            //show splash screen with score here


            _status = gameStatus.categoriesPanel;

            ScoreManager.Instance.CalculateScore();

            Debug.Log("Accuracy Score: " + ScoreManager.Instance.AccuracyScore);

            Debug.Log("Time Score: " + ScoreManager.Instance.TimeScore);

            Debug.Log("Total Score: " + ScoreManager.Instance.TotalScore);

            ScoreManager.Instance.DisplayScore();

            ScoreManager.Instance.ResetScoreManager();

            ResetGameParameters();

            method = new Method(ShowPanel);
        }
        else
        {
            method = new Method(AnimalsBtnPressed);
        }

        StartCoroutine(Wait(0.8f, method));

        SetInteractionStatus(_selectedActivity, false);
    }



	// Clears a list of instantiated objects
	public void ClearGameObjectList(List<GameObject> objectList)
	{
		if(objectList != null)
		{
			foreach(GameObject item in objectList)
			{
				Destroy(item);
			}
		}
	}




	public void ItemSelected()
	{
        string buttonTag = EventSystem.current.currentSelectedGameObject.tag;

        if(buttonTag == _targetIndex.ToString())
		{
			QuestionComplete (true);
		}
		else
		{
			QuestionComplete (false);
		}
	}
        




	// Sets interaction status of all button in activity GameObject
	public void SetInteractionStatus(GameObject activity, bool status)
	{
		Button[] buttons = activity.GetComponentsInChildren<Button> ();

		foreach (Button button in buttons) 
		{
			button.GetComponent<Image> ().raycastTarget = status;
		
			if (status) 
			{
//				Debug.Log (button.name + " Enabled");
				button.transition = Selectable.Transition.ColorTint;
			} 
			else 
			{
//				Debug.Log (button.name + " Disabled");
				button.transition = Selectable.Transition.None;
			}
		}
	}

	//generates a list (length = counter) of non-repeatingintegers between min and max inclusively
	public List<int> GenerateRandom(int counter, int min, int max)
	{
		HashSet<int> candidates = new HashSet<int>();

		while(candidates.Count < counter)
		{
			candidates.Add(Random.Range(min, (max+1)));
		}

		// load them into a list
		List<int> result = new List<int>();
		result.AddRange (candidates);


		Shuffle<int> (result);

		return result;
	}



	// Method to shuffle contents (based on Fisher-Yates shuffle algorithm). 
	public void Shuffle<T>(List<T> list)  
	{  
		int n = list.Count;  

		while (n > 1) 
		{  
			n--;  
			int k = Random.Range(0, n + 1);  
			T value = list[k];  
			list[k] = list[n];  
			list[n] = value;  
		}  
	}




}





