﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//namespace Komodo.Runtime
//{
public class ChildTextCreateOnCall : MonoBehaviour
{
    public Transform transformToAddTextUnder;
    public GameObject textProfile;

    Dictionary<int, TMP_Text> clientIDsToLabelGO = new Dictionary<int, TMP_Text>();

    public TMP_Text mainClientName;

    public ShareMediaConnection shareMediaConnection;

    public GameObject CallReceivePanel;
    public TMP_Text customReceiveText;







    public void Start()
    {
        clientIDsToLabelGO = ClientSpawnManager.Instance.GetUsernameMenuDisplayDictionary();
        //  ReceivedCall(2);


    }

    public void CreateTextFromString(string clientTextLabel, int clientID, bool isLocalClient = false)
    {



        if (isLocalClient)
        {
            if (!clientIDsToLabelGO.ContainsKey(clientID))
                clientIDsToLabelGO.Add(clientID, mainClientName);
            else
                clientIDsToLabelGO[clientID] = mainClientName;


            mainClientName.text = "Logged in as: <b><color=white>" + clientTextLabel + "</color></b>";

            return;
        }


        if (!clientIDsToLabelGO.ContainsKey(clientID))
        {
            //wait to create text until position is situated
            var newObj = Instantiate(textProfile);
          


            //main client button
            //var newButton = newObj.GetComponent<Button>();
            //    newButton.onClick.AddListener(() =>
            //    {
            //        shareMediaConnection.SetClientForMediaShare(clientID);
            //    });

            var newText = newObj.GetComponentInChildren<TMP_Text>(true);

            clientIDsToLabelGO.Add(clientID, newText);

            newText.text = clientTextLabel;

            var references = newObj.GetComponent<ClientConnectionReferences>();
            references.clientID = clientID;


            references.MakeCallButton.onClick.AddListener(() =>
            {
                Debug.Log($"Calling client:{ clientID}");
                ShareMediaConnection.CallClient(ClientSpawnManager.Instance.GetUsername(clientID));

            });
            references.AcceptCallButton.onClick.AddListener(() =>
            {

                ShareMediaConnection.AnswerClientOffer(ClientSpawnManager.Instance.GetUsername(clientID));
            });





            newObj.transform.SetParent(transformToAddTextUnder, false);


            //ClientSpawnManager.Instance.AddToUsernameMenuLabelDictionary(clientID, newText);
        }
        else
            Debug.Log("CLIENT LABEL + " + clientTextLabel + " Already exist");
    }

    public void DeleteTextFromString(int clientID)
    {
        DeleteClientID_Await(clientID);
    }

    public async void DeleteClientID_Await(int clientID)
    {
        if (clientIDsToLabelGO.ContainsKey(clientID))
        {
            while (!clientIDsToLabelGO.ContainsKey(clientID)) //clientIDsToLabelGO[clientID] == null)
                await Task.Delay(1);

            //DELETE Button Parent not only test
            Destroy(clientIDsToLabelGO[clientID].transform.parent.gameObject);
            clientIDsToLabelGO.Remove(clientID);

        }
        else
            Debug.Log("Client Does not exist");

    }

    public void ReceivedCall(int fromClientID)
    {
        string nameOfClient = ClientSpawnManager.Instance.GetUsername(fromClientID);

        customReceiveText.text = $"Receiving Call From: \n {nameOfClient} ";

        CallReceivePanel.SetActive(true);

        var buttons = CallReceivePanel.GetComponentsInChildren<Button>(true);

        buttons[0].onClick.RemoveAllListeners();
        buttons[1].onClick.RemoveAllListeners();

        //reject call
        buttons[0].onClick.AddListener(() =>
        {

            CallReceivePanel.SetActive(false);
        });


        //accept call
        buttons[1].onClick.AddListener(() =>
        {

            ShareMediaConnection.AnswerClientOffer(nameOfClient);
            // AcceptCallFromClient(nameOfClient);
            CallReceivePanel.SetActive(false);
            shareMediaConnection.SetClientForMediaShare(fromClientID);
            shareMediaConnection.SetMediaShareType(2);// call
            shareMediaConnection.shareMediaUI.SetActive(true);
        });




    }






}
//}
