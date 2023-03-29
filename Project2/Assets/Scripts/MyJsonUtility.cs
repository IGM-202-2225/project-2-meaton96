using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public abstract class MyJsonUtility {
    public static string fullJsonText;
    public static string[] jsonSplitByBracket;
    public static Dictionary<string, AgentData> classData = new();
    public static Dictionary<string, List<SimpleSphereCollider>> colliderList = new();
    public static void SetJsonText(string json) {
        fullJsonText = json;
    }
    
    public static void ParseAllJson() {
        string[] classes = fullJsonText.Split('?');

        for (int x = 1; x < classes.Length; x++) {
            
            string className = "";
            char c = classes[x][0];
            int index = 1;

            //grab the class name from the start of the classes string
            while (c != '"') {
                className += c;
                c = classes[x][index];
                index++;
            }
            //trim the class name off
            classes[x] = classes[x][classes[x].IndexOf("{")..(classes[x].Length - 1)];

            //split into seperate class data pieces
            //class data vs colliders
            string[] classFields = classes[x].Split("},");

            for (int y = 0; y < classFields.Length; y++) {
                //split strings again by open bracket
                //this leaves you with just the info inside the brackets that you want to read into data objects

                string[] data = classFields[y].Split('{');

                for (int z = 0; z < data.Length; z++) {
                    //check to cleanup any hanging close brackets to make the next part clean
                    int i = data[z].IndexOf('}');
                    if (i != -1) {
                        data[z] = data[z][0..i];
                    }
                }

                for (int z = 0; z < data.Length; z++) {
                    
                    //grab class data
                    if (data[z].Contains("classdata")) {
                        string cd = "{" + data[z + 1] + "}";
                        classData.Add(className, JsonUtility.FromJson<AgentData>(cd));
                    }
                    //grab all the colliders
                    else if (data[z].Contains("collider")) {
                        string cd = "{" + data[z + 1] + "}";
                        if (!colliderList.ContainsKey(className)) {
                            colliderList.Add(className, new List<SimpleSphereCollider> { JsonUtility.FromJson<SimpleSphereCollider>(cd) });
                        }
                        else {
                            colliderList[className].Add(JsonUtility.FromJson<SimpleSphereCollider>(cd));
                        }
                    }
                }
                  
            }
            //Debug.Log(classes[x]);
        }


    }


}
