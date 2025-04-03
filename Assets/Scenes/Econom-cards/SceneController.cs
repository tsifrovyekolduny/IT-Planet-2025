using Assets.Scenes.Econom_cards.Interfaces.Effects;
using Assets.Scenes.Econom_cards.Interfaces.Events;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    public GameObject taper;

    IListObjectMover listObjectMover;
    IBrainEvent brainEvent;

    void Start()
    {
        listObjectMover = taper.GetComponent<IListObjectMover>();
        brainEvent = taper.GetComponent<IBrainEvent>();
        brainEvent.BrainEvent.AddListener(BrainEvent);
        listObjectMover.AllObjectsMovedEvent.AddListener(AllObjectsMoved);
    }

    private void BrainEvent()
    {
        listObjectMover.MoveNext();
    }

    private void AllObjectsMoved()
    {
        Debug.Log("AllObjectsMoved");
    }

    void Update()
    {   
    }


}
