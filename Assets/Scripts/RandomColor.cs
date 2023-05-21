using System;
using System.Collections.Generic;
using UnityEngine;

public class RandomColor : MonoBehaviour
{
	public static event Action<int, int> OnFrequencyChanged;

	[SerializeField, Header("Colors")]
	private Color _backgroundColor;
	[SerializeField]
	private List<ColorAndRelativeFrequency> _colors;

	[SerializeField, Header("Layout")]
	private float _lineThickness = 0.25f;
	[SerializeField]
	private float _squareSize = 1.5f;
	[SerializeField]
	private int _numberOfSquaresToPaint = 25;

	[SerializeField, Header("Dont Worry About These")]
	private GameObject _squarePrefab;
	[SerializeField]
	private GameObject _colorButtonPrefab;
	[SerializeField]
	private Transform _colorButtonsParent;

	private List<GameObject> _squares = new();
	public Dictionary<Vector2, GameObject> CoordinatesAndSquares { get; set; }
	public float SquareSize { get { return _squareSize; } set { _squareSize = value; } }

	public void RaiseRelativeFrequency(int index)
    {
		_colors[index].RelativeFrequency++;
		OnFrequencyChanged?.Invoke(index, _colors[index].RelativeFrequency);
    }

	public void LowerRelativeFrequency(int index)
    {
		_colors[index].RelativeFrequency--;
		if (_colors[index].RelativeFrequency < 0)
        {
			_colors[index].RelativeFrequency = 0;
        }
		OnFrequencyChanged?.Invoke(index, _colors[index].RelativeFrequency);
	}

	private void Awake()
    {
		DrawGrid();
		SetupColorButtons();
	}

	private void SetupColorButtons()
    {
		foreach (Transform child in _colorButtonsParent)
        {
			Destroy(child.gameObject);
        }

        for (int i = 0; i < _colors.Count; i++)
        {
			GameObject colorButton = Instantiate(_colorButtonPrefab, _colorButtonsParent);
			ColorsButton colorsButton = colorButton.GetComponent<ColorsButton>();
			colorsButton.RandomColor = this;
			colorsButton.Index = i;
			colorsButton.RelativeFrequencyText.text = _colors[i].RelativeFrequency.ToString();
			colorsButton.ButtonImage.color = _colors[i].Color;
        }
    }

	public void DrawGrid()
    {
		foreach (GameObject square in _squares)
        {
			Destroy(square);
        }
		_squares.Clear();
		CoordinatesAndSquares = new();

		// Color background. 
		GetComponent<SpriteRenderer>().color = _backgroundColor;

		float x = _lineThickness;
		float y = _lineThickness;

		int numberOfColumns = Mathf.FloorToInt((23.75f - _lineThickness) / (_squareSize + _lineThickness));
		int numberOfRows = Mathf.FloorToInt((15.75f - _lineThickness) / (_squareSize + _lineThickness));

		float spacing = _lineThickness + _squareSize;

		// Make list of square coordinates.
		for (int i = 0; i <= numberOfColumns; i++)
		{
			for (int j = 0; j <= numberOfRows; j++)
			{
				CoordinatesAndSquares.Add(
					new Vector2(
						_lineThickness + (_squareSize / 2) + (i * spacing),
						_lineThickness + (_squareSize / 2) + (j * spacing)),
					null);
			}
		}

		// Randomly color squares
		if (_numberOfSquaresToPaint > CoordinatesAndSquares.Count)
		{
			_numberOfSquaresToPaint = CoordinatesAndSquares.Count;
		}

		// Make list of untaken spots. 
		List<Vector2> untakenSpots = new();
		foreach (KeyValuePair<Vector2, GameObject> kvp in CoordinatesAndSquares)
        {
			if (kvp.Value == null)
            {
				untakenSpots.Add(kvp.Key);
            }
        }

		// Place and paint the squares. 
		for (int i = 0; i < _numberOfSquaresToPaint; i++)
		{
			// Put square at random coordinate. 
			int randomCoordinateInt = UnityEngine.Random.Range(0, untakenSpots.Count);
			Vector2 key = untakenSpots[randomCoordinateInt];
			GameObject square = Instantiate(_squarePrefab, key, Quaternion.identity);

			// Remove coordinate from list so you don't choose it again. 
			untakenSpots.RemoveAt(randomCoordinateInt);

			// Update dictionary. 
			CoordinatesAndSquares[key] = square;

			// Scale square. 
			square.transform.localScale = new Vector3(_squareSize, _squareSize, 1f);

			// Add to squares list to destroy them on refresh. 
			_squares.Add(square);

			// Randomly color square. 
			int totalProbability = 0;
			foreach (ColorAndRelativeFrequency colorAndRelativeFrequency in _colors)
			{
				totalProbability += colorAndRelativeFrequency.RelativeFrequency;
			}
			int randomColorInt = UnityEngine.Random.Range(0, totalProbability);
			int cumulativeProbability = 0;
			Color color = new Color();
			foreach (ColorAndRelativeFrequency colorAndRelativeFrequency in _colors)
			{
				cumulativeProbability += colorAndRelativeFrequency.RelativeFrequency;
				if (randomColorInt < cumulativeProbability)
				{
					color = colorAndRelativeFrequency.Color;
					break;
				}
			}
			square.GetComponent<SpriteRenderer>().color = color;
		}
	}
}

[Serializable]
public class ColorAndRelativeFrequency
{
	public string Name;
	public Color Color;
	public int RelativeFrequency;
}