using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickVFXTrigger : MonoBehaviour
{
   
    [SerializeField] GameObject _vfx;



    private void OnEnable()
    {
        BoardController.OnTokenDropped += PlayVFX;
    }

    private void OnDisable()
    {
        BoardController.OnTokenDropped -= PlayVFX;
    }

    void PlayVFX(MoveData eventData)
    {
        Vector3 location = GridComponent.GetBoard().GridToWorldPosition(eventData.tokenCoords) 
            + GridComponent.GetBoard().GetCellOffset();

        Instantiate(_vfx, location, Quaternion.identity);
    }
}
