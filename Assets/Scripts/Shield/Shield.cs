using UnityEngine;

[RequireComponent (typeof(SpriteRenderer))]
public class Shield : MonoBehaviour
{
    // Internals
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

    /// <summary>
    /// Property to expose whether the shield is in parry state or not
    /// </summary>
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
