using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ColorsButton : MonoBehaviour
{
	// Have RandomColor set these variables when it instantiates these gameobjects. 
	public RandomColor RandomColor { get; set; }
	public int Index { get; set; }
    public TextMeshProUGUI RelativeFrequencyText;
    public Image ButtonImage;

    private void OnEnable()
    {
        RandomColor.OnFrequencyChanged += UpdateFrequencyText;
    }

    private void OnDisable()
    {
        RandomColor.OnFrequencyChanged -= UpdateFrequencyText;
    }

    private void UpdateFrequencyText(int index, int frequency)
    {
        if (index == Index)
        {
            RelativeFrequencyText.text = frequency.ToString();
        }
    }

    public void IncreaseFrequency()
    {
		RandomColor.RaiseRelativeFrequency(Index);
    }

	public void LowerFrequency()
    {
		RandomColor.LowerRelativeFrequency(Index);
    }
}