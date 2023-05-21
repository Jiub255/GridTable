using UnityEngine;

public class ColorsButton : MonoBehaviour
{
	[SerializeField]
	private GameObject _colorsGameObject;

	public void ToggleColors()
    {
		_colorsGameObject.SetActive(!_colorsGameObject.activeInHierarchy);
    }
}