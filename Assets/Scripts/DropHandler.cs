using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class DropHandler : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	private DragHandler _dragScript;

	private DropHandler _dropScript;

	private PointerEventData _eventData;

	public Vector3 DefaultSize = new Vector3 (1.0f, 1.0f, 0.0f);

	public Vector3 EnlargedSize = new Vector3 (1.3f, 1.3f, 0.0f);

	public bool occupied = false;

	public bool correct = false;

	public static int _numberOfCorrectLetters = 0;
	public static int _numberOfDroppedLetters = 0;

	public static int wordLength = 0;




	#region IPointerEnterHandler implementation

	public void OnPointerEnter (PointerEventData eventData)
	{
		Scale (EnlargedSize);
	}

	#endregion




	#region IPointerExitHandler implementation

	public void OnPointerExit (PointerEventData eventData)
	{
		Scale (DefaultSize);
	}

	#endregion





	#region IDropHandler implementation

	public void OnDrop (PointerEventData eventData)
	{
		_dragScript = eventData.pointerDrag.GetComponent<DragHandler> ();

		_dropScript = this.GetComponent<DropHandler> ();

		_eventData = eventData;

		CountLetters ();
	
		AssignNewParent ();

		Scale (DefaultSize);

		CheckRoundStatus ();
	}

	#endregion



	private void CountLetters()
	{
		string slotLetter = this.transform.Find ("Letter").GetComponent<Text> ().text;
		string dropLetter = _eventData.pointerDrag.transform.Find ("Letter").GetComponent<Text> ().text;

		if(_dragScript.originalParent.name == "Letters")
		{
			_numberOfDroppedLetters += 1;

			if (slotLetter == dropLetter) 
			{
				_numberOfCorrectLetters += 1;

				_dropScript.correct = true;
			}
		}
		else if(_dragScript.originalParent.parent.name == "Item Spelling")
		{
			if (slotLetter == dropLetter) 
			{
				if (_dropScript.correct == false) 
				{
					_numberOfCorrectLetters += 1;

					_dropScript.correct = true;
				}
			} 
		}
	}



	private void AssignNewParent ()
	{
		if (_dragScript != null && _dropScript != null) 
		{
			_dragScript.originalParent = this.transform;

			_dropScript.enabled = false;
		} 
		else 
		{
			throw new UnityException ("DropHandler exception");
		}

		_dropScript.occupied = true;
	}




	private void Scale(Vector3 newScaleFactor)
	{
		this.transform.localScale = newScaleFactor;
	}



	private void CheckRoundStatus()
	{
		if (wordLength > 0) 
		{
			if( (_numberOfCorrectLetters < 0) || (_numberOfDroppedLetters < 0) || (_numberOfCorrectLetters > _numberOfDroppedLetters) || (_numberOfDroppedLetters > wordLength) )
			{
				throw new UnityException("DropHandler exception");
			}

			if(_numberOfCorrectLetters == wordLength)
			{
				_numberOfCorrectLetters = 0;
				_numberOfDroppedLetters = 0;
				wordLength = 0;

				GameManager.Instance.QuestionComplete (true);
			}
		}
		else
		{
			throw new UnityException("DropHandler exception");
		}
	}

}
