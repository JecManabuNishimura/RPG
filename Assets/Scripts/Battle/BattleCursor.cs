using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleCursor : MonoBehaviour
{
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
