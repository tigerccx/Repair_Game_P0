using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class Tester : MonoBehaviour
{
    public int i;
    //Ref<int> refInt;
    Test0 t0;

    // Start is called before the first frame update
    void Start()
    {
        //refInt = new Ref<int>(() => i, z => { i = z; });
        //refInt.Value = 10;
        //print(i);

        t0 = new Test0();
        print(nameof(t0.V2));
        //PropertyAnimator.GetInstance().StartAnimatingPropertyVector2(t0, nameof(t0.V2), new Vector2(0.4f,0.4f), new Vector2(2.4f,2.4f), 10.0f);
        PropertyAnimator.GetInstance().StartAnimatingPropertyInt(t0, nameof(t0.I), 0, 5, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //print(t0.V2.x+" "+t0.V2.y);
        print(t0.I);
    }
}

class Test0
{
    public int I { get; set; }
    public float F { get; set; }
    public Vector2 V2 { get; set; }
}
