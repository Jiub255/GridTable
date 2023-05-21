using System.Collections.Generic;
using UnityEngine;

public class AddColor : MonoBehaviour
{
    [SerializeField]
    private GameObject _squarePrefab;

    private Transform _transform;
    private bool _dragging = false;

    private Camera _camera;
    private SpriteRenderer _spriteRenderer;
    private RandomColor _randomColor;

    private GameObject _clone;
    private SpriteRenderer _cloneSpriteRenderer;

    private void Start()
    {
        _camera = Camera.main;
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _randomColor = FindObjectOfType<RandomColor>();
    }

    // Called when you start holding down LMB on square. 
    public void OnHoldDownLMB()
    {
        // TODO - Not detecting click, why? It's very similar to the other one which is working fine. 
      //  Debug.Log("Click detected.");

        // Clone this square so that there's an endless stack of them to grab from. 
        _clone = Instantiate(_squarePrefab, transform.position, Quaternion.identity, transform.parent);
        _cloneSpriteRenderer = _clone.GetComponent<SpriteRenderer>();
        _cloneSpriteRenderer.color = _spriteRenderer.color;
        _cloneSpriteRenderer.sortingOrder = 3;
        _transform = _clone.transform;
        _transform.localScale = new Vector3(_randomColor.SquareSize, _randomColor.SquareSize, 1f);

        _dragging = true;
    }

    public void OnReleaseLMB()
    {
        _dragging = false;
        _cloneSpriteRenderer.sortingOrder = 1;

        float distance = float.MaxValue;
        KeyValuePair<Vector2, GameObject> newKVP = new();
        foreach (KeyValuePair<Vector2, GameObject> kvp in _randomColor.CoordinatesAndSquares)
        {
            if (Vector2.Distance(kvp.Key, _transform.position) < distance)
            {
                distance = Vector2.Distance(kvp.Key, _transform.position);
                newKVP = kvp;
            }
        }

        // If you release LMB within
        if (distance < _randomColor.SquareSize)
        {
            // If the nearest coordinate has a square on it... 
            if (newKVP.Value != null)
            {
                // Change that square's color to this square's color.
                newKVP.Value.GetComponent<SpriteRenderer>().color = _spriteRenderer.color;

                // Destroy this square. No need to do anything to the dictionary. 
                Destroy(_transform.gameObject);
            }
            // Else if the nearest coordinate is empty... 
            else
            {
                // Change this position's kvp value to this square. 
                _randomColor.CoordinatesAndSquares[newKVP.Key] = _transform.gameObject;

                // Set this square here. 
                _transform.position = newKVP.Key;
            }
        }
        // Else if you release LMB further than one square width away from the edge...
        else
        {
            Destroy(_transform.gameObject);
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
                if (raycastHit.transform == transform)
                {
                    OnHoldDownLMB();
                }
            }
        }
    }
}