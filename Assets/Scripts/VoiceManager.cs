﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Windows.Speech;
using HoloToolkit.Unity;
using System.Linq;

public class VoiceManager : MonoBehaviour {

    Dictionary<string, System.Action> keywords;
    KeywordRecognizer keywordRecognizer = null;

    private GameObject hologramCollection;
    private SmartLightManager slm;

    [Tooltip("Brightness range 0 - 254 (Default: 160)")]
    public int dimValue = 100;
    [Tooltip("Brightness range 0 - 254 (Default: 80)")]
    public int dimMoreValue = 50;

    // assigned upon initialization of initBrightness call from SmartLightManager
    private SmartLight currentLight;
    // position of light in lights array. Needed as Hue API starts light array at '1'.
    private int arrayId;

    // Use this for initialization
    void Start () {
        Debug.Log("VoiceMgr Start");
        if (GameObject.Find("HologramCollection") != null)
        {
            hologramCollection = GameObject.Find("HologramCollection");
        }
        else
        {
            Debug.LogError("No GameObject name HologramCollection can be found. This object should contain all holograms and the SmartLightManager");
        }
    }
	
	// Update is called once per frame
	void Update () {
        var focusObject = GestureManager.Instance.FocusedObject;
        if (focusObject != null)
        {
            //slm.UpdateLightState(focusObject.name, param, value);
            //SmartLightManager.light
            if (focusObject.tag != "Untagged")
            {
                var idTag = focusObject.tag;
                //// Ignores focusObject if it does not have a valid id assigned to tag
                if (int.TryParse(idTag, out arrayId))
                {
                    currentLight = SmartLightManager.lights[arrayId];
                }
            }
        }
    }

    public void RegisterPhrases()
    {
        // called outside of Start() to ensure the SmartLightManager has been loaded first 
        slm = hologramCollection.GetComponent<SmartLightManager>();

        keywords = new Dictionary<string, System.Action>();

        // Global light commands
        keywords.Add("Normal Lights", () =>
        {
            slm.SetLightsToDefault();
        });

        // On/Off commands
        keywords.Add("Light On", () =>
        {
            buildUpdateCall("On", 0);
        });

        keywords.Add("Light Off", () =>
        {
            buildUpdateCall("Off", 0);
        });

        // color change commands
        keywords.Add("Set To Red", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Red");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To Orange", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Orange");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To Yellow", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Yellow");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To Green", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Green");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To White", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("White");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To Blue", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Blue");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To Purple", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Purple");

            buildUpdateCall("hue", hue);
        });

        keywords.Add("Set To Pink", () =>
        {
            int hue;
            hue = ColorService.GetHueByColor("Pink");

            buildUpdateCall("hue", hue);
        });

        // Brightness adjustment commands
        keywords.Add("Dim Light", () =>
        {
            buildUpdateCall("bri", dimValue);
        });

        keywords.Add("Dim Light More", () =>
        {
            buildUpdateCall("bri", dimMoreValue);
        });

        keywords.Add("Full Brightness", () =>
        {
            buildUpdateCall("bri", 254);
        });

        // flashes the corresponding light of the currently focused gameObject
        keywords.Add("Identify Light", () =>
        {
            buildUpdateCall("alert", 1);
        });

        // stops flashing of light prior to 15 second default time
        keywords.Add("OK That's Enough", () =>
        {
            buildUpdateCall("alert", 0);
        });

        // system voice commands
        keywords.Add("Show Voice Menu", () =>
        {
            //showVCMenu(true);
        });

        keywords.Add("Hide Voice Menu", () =>
        {
            //showVCMenu(false);
        });

        // Tell the KeywordRecognizer about our keywords.
        keywordRecognizer = new KeywordRecognizer(keywords.Keys.ToArray());

        // Register a callback for the KeywordRecognizer and start recognizing!
        keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
        keywordRecognizer.Start();

        // populates voice control help menu with available commands
        //buildMenu(keywords);
    }

    private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        System.Action keywordAction;
        if (keywords.TryGetValue(args.text, out keywordAction))
        {
            Debug.Log("Here is args.text: " + args.text);
            keywordAction.Invoke();
        }
    }

    void buildUpdateCall(string param, int value)
    {
        var focusedObject = GestureManager.Instance.FocusedObject;
        //Debug.Log("here is focused object: " + focusObject);
        //if (focusObject != null)
        //{
        //    slm.UpdateLightState(focusObject.name, param, value);
        //    //SmartLightManager.light
        //}
        if (focusedObject != null)
        {
            // retrieves array index (arrayId) from the tag assigned in SmartLightManager
            if (focusedObject.tag != "Untagged")
            {
                var idTag = focusedObject.tag;
                //// Ignores focusedObject if it does not have a valid id assigned to tag
                if (int.TryParse(idTag, out arrayId))
                {
                    currentLight = SmartLightManager.lights[arrayId];
                    if (param == "On")
                    {
                        currentLight.State.On = true;
                    }
                    else if (param == "Off")
                    {
                        currentLight.State.On = false;
                    }
                    else if (param == "hue")
                    {
                        currentLight.State.Hue = value;
                        currentLight.State.Sat = 254;
                    }
                    else if (param == "bri")
                    {
                        currentLight.State.Bri = value;
                    }
                    else if (param == "alert")
                    {
                        if (value == 0)
                        {
                            currentLight.State.Alert = "none";
                        }
                        else
                        {
                            currentLight.State.Alert = "lselect";
                        }
                    }
                    // hueAPI.UpdateLight(currentLight);
                    SmartLightManager.UpdateLightState(arrayId);
                    currentLight.State.Alert = "none";
                }
                else
                {
                    Debug.Log("a tag with a valid array index (arrayId) could not be found on focusedObject");
                }
            }
            else
            {
                Debug.Log("No tag containing arrayId was found on this focusedObject.");
            }
        }
    }
}
