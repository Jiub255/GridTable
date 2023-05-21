using UnityEngine;

public class DeleteSquare : MonoBehaviour
{
    private RandomColor _randomColor;

    private void Awake()
    {
        _randomColor = FindObjectOfType<RandomColor>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit raycastHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out raycastHit, 100f))
            {
                if (raycastHit.transform == transform)
                {
                    _randomColor.CoordinatesAndSquares[transform.position] = null;
                    Destroy(gameObject);
                }
            }
        }
    }
}