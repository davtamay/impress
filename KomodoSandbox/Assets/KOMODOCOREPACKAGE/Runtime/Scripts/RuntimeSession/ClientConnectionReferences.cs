
using UnityEngine;
using UnityEngine.UI;

public class ClientConnectionReferences : MonoBehaviour
{
    public int clientID;
    public Toggle selectedToggle ()=> GetComponent<Toggle>();

    //public Button AcceptCallButton;
    //public Button cancelCall;
    //public Button MakeCallButton;
    //public Button privateMessageText;
    //public Toggle micOn;
}
