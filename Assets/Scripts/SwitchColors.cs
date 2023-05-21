using System.Collections.Generic;
using UnityEngine;

public class SwitchColors : MonoBehaviour
{
	private Vector2 _originalPositionOfDraggedSquare;

	private Transform _transform;
    private bool _dragging = false;

    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    private RandomColor _randomColor;

    private void Start()
    {
        _transform = transform;
        _camera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _randomColor = FindObjectOfType<RandomColor>();
    }

    // Called when you start holding down LMB on square. 
    public void OnHoldDownLMB()
    {
        _dragging = true;
		_originalPositionOfDraggedSquare = _transform.position;
        _spriteRenderer.sortingOrder = 10;
    }

    public void OnReleaseLMB()
    {
        _dragging = false;
        _spriteRenderer.sortingOrder = 1;

        // Find the closest coordinate in the dictionary. 
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

        // If the distance is less than a square width from the coordinate...
        if (distance < _randomColor.SquareSize)
        {
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
            // Else if the nearest coordinate is empty... 
            else
            {
                // Change old kvp. 
                _randomColor.CoordinatesAndSquares[_originalPositionOfDraggedSquare] = null;

                // Move old square to new one's position (dictionary is unchanged). 
                _transform.position = newKVP.Key;

                // Change new kvp.
                _randomColor.CoordinatesAndSquares[newKVP.Key] = _transform.gameObject;
            }
        }
        // Else if the square is further than one square width from the edges, destroy it. 
        else
        {
            _randomColor.CoordinatesAndSquares[transform.position] = null;
            Destroy(gameObject);
        }

    }

    private void Update()
    {
        if (_dragging)
        {
            Vector3 cameraPosition = _camera.ScreenToWorldPoint(Input.mousePosition);
            _transform.position = new Vector3(cameraPosition.x, cameraPosition.y, 0f);

            if (!Input.GetMouseButton(0))
            {
                OnReleaseLMB();
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
                    OnHoldDownLMB();
                }
            }
        }
    }
}