using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonTest : MonoBehaviour {

    Vector2 tmp = new Vector2(0f,0f);
    Queue<int[]> numarray;

    // Update is called once per frame

    private void Awake()
    {
        queueTest();
    }


    private void Update()
    {


    }


    void queueTest()
    {
        numarray = new Queue<int[]>();
        putin(1,2);
        putin(3, 4);
        putin(5, 6);
        int i = 1;
        while (numarray.Count > 0)
        {
            int[] output = numarray.Dequeue();
            Debug.Log(i.ToString()+":"+output[0].ToString() + "," + output[1].ToString());
        }


    }

    void putin(int a,int b)
    {
        int[] array =new int[2] { a,b};
        numarray.Enqueue(array);

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
