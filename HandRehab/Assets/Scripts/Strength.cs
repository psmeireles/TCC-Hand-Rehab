using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using SimpleJSON;

public class Strength : MonoBehaviour
{
    public float maxStength;
    public float strength;
    public Slider strengthBar;
    public Image fill;
    public CharType type;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Fallback hp
        maxStength = maxStength == 0 ? 1 : maxStength;
        strength = strength == 0 ? 1 : strength;

        //strength = maxStength;
        if (strengthBar != null)
        {
            strengthBar.maxValue = maxStength;
            strengthBar.value = strength;
            fill.color = Color.blue;
        }
        if (type == null)
        {
            type = new CharType(Element.NORMAL);
        }



    }

    // Update is called once per frame
    protected virtual void Update(){}


    // Set strength based on recieved value
    public void setStrength(float newStrength)
    {
        strength = newStrength;
    }

    public IEnumerator GetMyoData(string address)
    {
        // Request GET from server
        UnityWebRequest www = UnityWebRequest.Get(address);
        yield return www.SendWebRequest();


        // Verify if response has an error
        if (www.isHttpError)
            Debug.LogError(www.error);

        // Proccess Response from text to a JSON
        else ProccessServerResponse(www.downloadHandler.text);
    }

    void ProccessServerResponse(string rawResponse)
    {
        // That text, is actually a JSON info, so we need to parse that into something we can navigate.
        JSONNode node = JSON.Parse(rawResponse);

        // Changes current strength value in case the server response was successfull
        if (node["meta"]["success"] == true)
            setStrength(node["data"]["strength"]);

    }


    /*
    public void Hit(float strength)
    {
        if (strengthBar != null)
        {
            strengthBar.value = this.strength;

            if (strength > maxStength)
            {
                this.strength = maxStength;
                strengthBar.value = maxStength;
                fill.color = Color.black;
            }
            else if (strength > 0.5)
            {
                fill.color = Color.blue;
            }
            else if (strength > 0.25)
            {
                fill.color = Color.yellow;
            }
            else
            {
                fill.color = Color.red;
            }
        }

    }*/



}
