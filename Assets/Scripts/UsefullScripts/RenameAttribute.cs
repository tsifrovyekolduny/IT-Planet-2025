using UnityEngine;

public class RenameAttribute : PropertyAttribute
{
    public string DisplayName { get; }

    public RenameAttribute(string displayName)
    {
        DisplayName = displayName;
    }
}