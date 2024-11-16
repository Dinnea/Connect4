using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GetRestartKey : MonoBehaviour
{
    private void Awake()
    {
        TextMeshProUGUI textMesh = GetComponent<TextMeshProUGUI>();
        textMesh.text = "Press " + GameManager.GetRestartKey().ToString() + " to restart.";
    }
}
