using Assets.Scripts.Core.Interfaces.Effects;
using Assets.Scripts.Core.Interfaces.Events;
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
