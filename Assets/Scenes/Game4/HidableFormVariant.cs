using UnityEngine;

public class HidableFormVariant : MonoBehaviour
{
    public GameObject SpriteMaskHide;
    public GameObject SpriteMaskFull;
    public WithButtons withButtonsComponent;
    bool _isValid = true;
    private bool _isHiden = true;

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
            SpriteMaskHide.GetComponent<PolygonCollider2D>().enabled = true;
            SpriteMaskFull.GetComponent<PolygonCollider2D>().enabled = false;
        }
        else
        {
            withButtonsComponent.ToggleVisibility(true);
            SpriteMaskHide.GetComponent<PolygonCollider2D>().enabled = false;
            SpriteMaskFull.GetComponent<PolygonCollider2D>().enabled = true;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }
}
