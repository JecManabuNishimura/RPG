using UnityEngine;

public class SpriteSearchAttribute : PropertyAttribute
{
    public string FolderPath { get; }

    public SpriteSearchAttribute(string folderPath)
    {
        FolderPath = folderPath;
    }
}