using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;


// using SimpleJSON;

public class MyoTalker : MonoBehaviour
{

    // public ChainLightning lightning;
    void Start() {
        // lightning = GetComponent<ChainLightning>();
        StartCoroutine( GetMyoData ("http://localhost:8000/strength") );
    }

    public void ProccessServerResponse ( string rawResponse )
    { 
        // That text, is actually a JSON info, so we need to parse that into something we can navigate.

        JSONNode node = JSON.Parse( rawResponse );

        // Output some stuff to the console so that we know that it worked.

        if (node["meta"]["success"] == true) {
            // lightning.setStrength(node["data"]["streamEMG"]);
        }
    }   

    public IEnumerator GetMyoData( string address ) 
    {
        UnityWebRequest www = UnityWebRequest.Get( address );
        yield return www.SendWebRequest();


        if ( www.isHttpError ) {
            Debug.Log(www.error);
        }
        else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
 
            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
       
    }
}
