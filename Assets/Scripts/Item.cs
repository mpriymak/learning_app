using UnityEngine;

public class Item : MonoBehaviour
{
	[SerializeField]
	private string _category;
	[SerializeField]
	private string _name;
	[SerializeField]
	private AudioClip _audio;
	[SerializeField]
	private Sprite _image;

	public string Category
	{ 
		get
		{
			return _category;
		}
		set
		{
			_category = value;
		}
	}

	public string Name
	{ 
		get
		{
			return _name;
		}
		set
		{
			_name = value;
		}
	}

	public AudioClip Audio
	{ 
		get
		{
			return _audio;
		}
		set
		{
			_audio = value;
		}
	}

	public Sprite Image
	{ 
		get
		{
			return _image;
		}
		set
		{
			_image = value;
		}
	}
}
	