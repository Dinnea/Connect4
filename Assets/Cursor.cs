using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Personal.Utilities;
using System.Drawing;
using System;

public class Cursor : MonoBehaviour
{
    [SerializeField] LayerMask layer;

    public static Action<Vector3> OnBoardClicked;
    
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {           
            OnBoardClicked?.Invoke(VectorMath.GetMousePositionWorld(Camera.main, layer));
        }
    }

    
}
