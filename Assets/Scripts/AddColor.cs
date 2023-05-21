using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddColor : MonoBehaviour
{
    [SerializeField]
    private GameObject _squarePrefab;

    private Transform _cloneTransform;
    private bool _dragging = false;

    private Camera _camera;
    private Image _image;
    private RandomColor _randomColor;

    private GameObject _clone;
    private SpriteRenderer _cloneSpriteRenderer;

    private void Start()
    {
        _camera = Camera.main;
        _image = GetComponent<Image>();
        //_spriteRenderer = GetComponent<SpriteRenderer>();
        _randomColor = FindObjectOfType<RandomColor>();
    }

    // Called when you start holding down LMB on square. 
    public void OnHoldDownLMB()
    {
        Debug.Log("OnHoldDownLMB Called.");
        // Clone this square so that there's an endless stack of them to grab from. 
        // On the clone's SwitchColors script, _dragging will be false when instantiated, so it won't 
        // run any of its methods until after it gets set down. 
        _clone = Instantiate(
            _squarePrefab, 
            _camera.ScreenToWorldPoint(Input.mousePosition), 
            Quaternion.identity);
        _cloneSpriteRenderer = _clone.GetComponent<SpriteRenderer>();
        _cloneSpriteRenderer.color = _image.color;
        _cloneSpriteRenderer.sortingOrder = 3;
        _cloneTransform = _clone.transform;
        _cloneTransform.localScale = new Vector3(_randomColor.SquareSize, _randomColor.SquareSize, 1f);

        _dragging = true;
    }

    public void OnReleaseLMB()
    {
        Debug.Log("OnReleaseLMB Called.");
        _dragging = false;
        _cloneSpriteRenderer.sortingOrder = 1;

        // Find the closest coordinate in the dictionary. 
        float distance = float.MaxValue;
        KeyValuePair<Vector2, GameObject> newKVP = new();
        foreach (KeyValuePair<Vector2, GameObject> kvp in _randomColor.CoordinatesAndSquares)
        {
            if (Vector2.Distance(kvp.Key, _cloneTransform.position) < distance)
            {
                distance = Vector2.Distance(kvp.Key, _cloneTransform.position);
                newKVP = kvp;
            }
        }

        Debug.Log($"Coordinate: {newKVP.Key.ToString()}, Clone Position: {_cloneTransform.position.ToString()}");

        // If you release LMB within one square width of a coordinate,,,
        if (distance < _randomColor.SquareSize)
        {
            // And if the nearest coordinate has a square on it... 
            if (newKVP.Value != null)
            {
                // Change that square's color to this square's color.
                newKVP.Value.GetComponent<SpriteRenderer>().color = _image.color;

                // Destroy this square. No need to do anything to the dictionary. 
                Destroy(_clone);
            }
            // But if the nearest coordinate is empty... 
            else
            {
                // Change this position's kvp value to this square. 
                _randomColor.CoordinatesAndSquares[newKVP.Key] = _clone;

                // Add square to RandomColor's squares list.
                _randomColor.AddToSquaresList(_clone);

                // Set this square here. 
                _cloneTransform.position = newKVP.Key;
            }
        }
        // Else if you release LMB further than one square width away from the edge...
        else
        {
            Destroy(_cloneTransform.gameObject);
        }
    }

    private void Update()
    {
        if (_dragging)
        {
            Debug.Log("_dragging == true");
            Vector3 cameraPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _cloneTransform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0f);

            if (!Input.GetMouseButton(0))
            {
                OnReleaseLMB();
            }
        }
    }
}