
using UnityEngine;
using UnityEngine.EventSystems;

public class Selectable : MonoBehaviour, ISelectHandler, ISubmitHandler
{
    [SerializeField]
    AudioClip SelectSound;

    [SerializeField]
    AudioClip SubmitSound;

    AudioService audioReference;

    private void Start()
    {
        audioReference = ServiceLocator.Instance.GetService<AudioService>();
    }

    public void OnSelect(BaseEventData data)
    {
        audioReference.PlaySFX(SelectSound);
    }




    public void OnSubmit(BaseEventData data)
    {
        audioReference.PlaySFX(SubmitSound);
    }


}
