using UnityEngine;
using UnityEngine.UIElements;

public class AngleController : MonoBehaviour
{
    [SerializeField] private Transform _playerPosition;
    public Vector2 PointerPosition { get; set; }

    private void Update()
    {
        print(PointerPosition);
        transform.right = (PointerPosition - (Vector2)_playerPosition.position);
    }
}
