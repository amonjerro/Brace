using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class Shield : MonoBehaviour
{
    bool _isParry;
    SpriteRenderer _spriteRenderer;
    Character character;
    public AbsParryEffector effector;

    private void Start()
    {
        character = GetComponentInParent<Character>();
        effector = ParryFactory.MakeParryFactory(character);
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (_isParry) {
            _spriteRenderer.color = Color.cyan;
        } else
        {
            _spriteRenderer.color = Color.magenta;
        }
    }

    public bool IsParry
    {
        get
        {
            return _isParry;
        }

        set { 
            _isParry = value;
        }
    }
}
