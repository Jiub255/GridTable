using System.Collections.Generic;
using UnityEngine;

public class SwitchColors : MonoBehaviour
{
	private Vector2 _originalPositionOfDraggedSquare;

	private Transform _transform;
    private bool _dragging = false;

   // private Dictionary<Vector2, GameObject> _coordinateAndSquares = new();
    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    private RandomColor _randomColor;

/*    private void OnEnable()
    {
        RandomColor.OnCoordinatesMade += CopyCoordinateList;
    }

    private void OnDisable()
    {
        RandomColor.OnCoordinatesMade -= CopyCoordinateList;
    }

    private void CopyCoordinateList(Dictionary<Vector2, GameObject> coordinateAndSquares)
    {
        _coordinateAndSquares.Clear();
        foreach (KeyValuePair<Vector2, GameObject> coordinateAndSquare in coordinateAndSquares)
        {
            _coordinateAndSquares.Add(coordinateAndSquare.Key, coordinateAndSquare.Value);
        }
    }*/

    private void Start()
    {
        _transform = transform;
        _camera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _randomColor = FindObjectOfType<RandomColor>();
    }

    // Called when you start holding down LMB on square. 
    public void OnHoldDownRMB()
    {
        _dragging = true;
		_originalPositionOfDraggedSquare = _transform.position;
        _spriteRenderer.sortingOrder = 10;
    }

    public void OnReleaseRMB()
    {
        _dragging = false;
        _spriteRenderer.sortingOrder = 1;

        float distance = float.MaxValue;
        KeyValuePair<Vector2,GameObject> newKVP = new();
        foreach (KeyValuePair<Vector2, GameObject> kvp in _randomColor.CoordinatesAndSquares)
        {
            if (Vector2.Distance(kvp.Key, _transform.position) < distance)
            {
                distance = Vector2.Distance(kvp.Key, _transform.position);
                newKVP = kvp;
            }
        }

        Debug.Log($"New kvp: ({newKVP.Key}, {newKVP.Value}");

        // If the nearest coordinate has a square on it... 
        if (newKVP.Value != null)
        {
            // Move new square to original's position (dictionary is unchanged). 
            newKVP.Value.transform.position = _originalPositionOfDraggedSquare;

            // Change old kvp. 
            _randomColor.CoordinatesAndSquares[_originalPositionOfDraggedSquare] = newKVP.Value;

            // Move old square to new one's position (dictionary is still unchanged). 
            _transform.position = newKVP.Key;

            // Change new kvp.
            _randomColor.CoordinatesAndSquares[newKVP.Key] = _transform.gameObject;
        }
        // TODO - This is giving an extra non null value in the dictionary each time you switch a square with an empty tile. 
        else
        {
            // TODO - Not getting set to null here?
            Debug.Log($"Before value of original kvp: {_randomColor.CoordinatesAndSquares[_originalPositionOfDraggedSquare]}");
            // Change old kvp. 
            _randomColor.CoordinatesAndSquares[_originalPositionOfDraggedSquare] = null;
            Debug.Log($"After value of original kvp: {_randomColor.CoordinatesAndSquares[_originalPositionOfDraggedSquare]}");

            // Move old square to new one's position (dictionary is unchanged). 
            _transform.position = newKVP.Key;

            // Change new kvp.
            _randomColor.CoordinatesAndSquares[newKVP.Key] = _transform.gameObject;
        }

        int numberOfSquaresInDict = 0;
        foreach (KeyValuePair<Vector2, GameObject> kvp in _randomColor.CoordinatesAndSquares)
        {
            if (kvp.Value != null)
            {
                numberOfSquaresInDict++;
            }
        }
        Debug.Log($"Number of squares in dictionary after switch: {numberOfSquaresInDict}");
    }

    private void Update()
    {
        if (_dragging)
        {
            Vector3 cameraPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0f);

            if (!Input.GetMouseButton(0))
            {
                OnReleaseRMB();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform == _transform)
                {
                    OnHoldDownRMB();
                }
            }
        }
    }
}