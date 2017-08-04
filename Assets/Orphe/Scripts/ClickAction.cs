using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity.InputModule;
namespace Orpe
{
    public class ClickAction : MonoBehaviour, IInputClickHandler
    {
        public void OnInputClicked(InputClickedEventData eventData)
        {
            SendMessageUpwards("IdClicked", GetComponent<TextMesh>().text);
        }
    }
}

