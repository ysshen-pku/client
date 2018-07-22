using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour {

    Vector2 tmp = new Vector2(0f,0f);


    // Update is called once per frame

    private void Awake()
    {
        jsonTest();
    }


    private void Update()
    {


    }



    void jsonTest()
    {

        List<object> tmp= new List<object>();
        string str = "test";
        Vector2 pos =new Vector2 (0, 0);
        tmp.Add(str);
        tmp.Add(pos);

        string jsondict = JsonUtility.ToJson(new Serialization<object>(tmp));
        Debug.Log(jsondict);

        List<object> recv = JsonUtility.FromJson<Serialization<object>>(jsondict).ToList();

        if (recv != null)
        {
            foreach (var item in recv)
            {
                Debug.Log(item.ToString());
            }
  
        }


        
    }
}
