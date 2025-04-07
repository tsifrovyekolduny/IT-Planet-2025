using UnityEngine;

public class HidableFormVariant : MonoBehaviour
{
    public GameObject SpriteMaskHide;
    public GameObject SpriteMaskFull;
    public WithButtons withButtonsComponent;
    bool _isValid = true;
    private bool _isHiden = false;

    public bool IsHiden { get => _isHiden; set => _isHiden = value; }

    void Start()
    {
        if (withButtonsComponent == null)
        {
            withButtonsComponent = GetComponent<WithButtons>();
        }

        if (withButtonsComponent == null)
        {
            _isValid = false;
        }

        if (_isValid == false) return;

        UpdateShowWithState();
    }
    public void UpdateShowWithState(bool isHiden)
    {
        if (isHiden == IsHiden) return;
        _isHiden = isHiden;
        UpdateShowWithState();
    }
    void UpdateShowWithState()
    {
        if (IsHiden)
        {
            withButtonsComponent.ToggleVisibility(false);
            SpriteMaskHide.GetComponent<Collider>().enabled = true;
            SpriteMaskFull.GetComponent<Collider>().enabled = false;
        }
        else
        {
            withButtonsComponent.ToggleVisibility(true);
            SpriteMaskHide.GetComponent<Collider>().enabled = false;
            SpriteMaskFull.GetComponent<Collider>().enabled = true;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
