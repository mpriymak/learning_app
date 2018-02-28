using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{

	public Transform originalParent = null;

	private Transform _tempParent = null; 

	private GameObject _placeHolder = null;


	#region IBeginDragHandler implementation

	public void OnBeginDrag (PointerEventData eventData)
	{
		originalParent = this.transform.parent;

		_tempParent = originalParent;

		if (originalParent.parent.name == "Item Spelling") 
		{
			originalParent.GetComponent<DropHandler> ().enabled = true;

			originalParent.GetComponent<DropHandler> ().occupied = false;
		}
		else 
		{
			_placeHolder = new GameObject ();

			_placeHolder.transform.SetParent (this.transform.parent);

			_placeHolder.transform.SetSiblingIndex (this.transform.GetSiblingIndex());

			LayoutElement le = _placeHolder.AddComponent<LayoutElement> ();

			le.flexibleHeight = 1.0f;

			le.flexibleWidth = 1.0f;
		}

		this.transform.SetParent (GameManager.Instance.SelectedActivity.transform);

		this.GetComponent<CanvasGroup> ().blocksRaycasts = false;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag (PointerEventData eventData)
	{
		this.transform.position = new Vector3 (eventData.position.x, eventData.position.y, 0);
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag (PointerEventData eventData)
	{
		int siblingIndex;

		if (_placeHolder != null) 
		{
			if (originalParent.name == "Letters") 
			{
				siblingIndex = _placeHolder.transform.GetSiblingIndex ();
			} 
			else 
			{
				siblingIndex = originalParent.childCount;
			}

			Destroy (_placeHolder);
		} 
		else
		{
			if (_tempParent == originalParent) 
			{
				if (originalParent.GetComponent<DropHandler> ().occupied != true) 
				{
					if (originalParent.GetComponent<DropHandler> ().correct == true) 
					{
						originalParent.GetComponent<DropHandler> ().correct = false;

						DropHandler._numberOfCorrectLetters -= 1;
					}

					originalParent = GameManager.Instance.SelectedActivity.transform.Find ("Letters").transform;

					DropHandler._numberOfDroppedLetters -= 1;
				} 
			} 
			else 
			{
				_tempParent.GetComponent<DropHandler> ().occupied = false;

				if(_tempParent.GetComponent<DropHandler>().correct == true)
				{
					DropHandler._numberOfCorrectLetters -= 1;

					_tempParent.GetComponent<DropHandler> ().correct = false;
				}
			}

			siblingIndex = originalParent.childCount;
		}
			
		this.transform.SetParent (originalParent);

		this.transform.SetSiblingIndex(siblingIndex);	
			
		this.transform.position = originalParent.position;

		this.GetComponent<CanvasGroup> ().blocksRaycasts = true;
	}

	#endregion
}

