using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Wall : MonoBehaviour
{

    
    private void OnMouseDown()
    {
        
    }

    /// <summary>
    /// 销毁墙
    /// </summary>
    private void Destroy()
    {
        print("Wall goes boom!");
    }
}
