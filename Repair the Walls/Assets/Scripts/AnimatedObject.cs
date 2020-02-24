using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class AnimatedObject : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    #region Singleton
    private static AnimatedObject instance = null;
    public static AnimatedObject GetInstance()
    {
        return instance;
    }
    private void MakeSingleton()
    {
        if (!instance)
            instance = this;
    }
    #endregion

    private float durTime = 4.0f;

    private Test0 obj;
    private float src;
    private float targ;
    private bool isChanging = false;

    private void Awake()
    {
        MakeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {
        Test0 t0 = new Test0();
        PropertyWrapper pw = new PropertyWrapper(t0, "I", PropertyWrapper.PropertyType.Int);
        print(pw.Get());
    }

    // Update is called once per frame
    void Update()
    {
        //if (isChanging && GetComponent<Light>().intensity <= onIntensity)
        //{
        //    GetComponent<Light>().intensity = Mathf.Clamp(GetComponent<Light>().intensity + (onIntensity - offIntensity) / durTurn * Time.deltaTime, offIntensity, onIntensity);
        //}
        //else
        //{
        //    isChanging = false;
        //}
        //if (isChanging && GetComponent<Light>().intensity >= offIntensity)
        //{
        //    GetComponent<Light>().intensity = Mathf.Clamp(GetComponent<Light>().intensity + (offIntensity - onIntensity) / durTurn * Time.deltaTime, offIntensity, onIntensity);
        //}
        //else
        //{
        //    isChanging = false;
        //}
    }
}

class PropertyWrapper
{
    public enum PropertyType
    {
        Float,
        Int
    }

    public PropertyWrapper(object component, string strProp, PropertyType typeProp)
    {
        Init(component, component.GetType(), strProp, typeProp);
    }

    public PropertyWrapper(object component, Type typeComponent, string strProp, PropertyType typeProp)
    {
        Init(component, typeComponent, strProp, typeProp);
    }

    private void Init(object component, Type typeComponent, string strProp, PropertyType typeProp)
    {
        this.component = component;
        this.typeComponent = typeComponent;
        this.strProp = strProp;
        this.typeProp = typeProp;
    }

    public object component;
    public Type typeComponent;
    public string strProp;
    public PropertyType typeProp;

    public object Get()
    {
        return GetProperty(component, typeComponent, strProp);
    }

    public void Set(object val)
    {
        SetProperty(component, typeComponent, strProp, val);
    }

    private object GetProperty(object inst, Type type, string propName)
    {
        var prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (prop == null)
        {
            throw new Exception("Cannot find property!");
        }
        return prop.GetValue(inst);
    }

    private float GetPropertyFloat(object inst, Type type, string propName)
    {
        return (float)GetProperty(inst, type, propName);
    }

    private float GetPropertyInt(object inst, Type type, string propName)
    {
        return (int)GetProperty(inst, type, propName);
    }

    private void SetProperty(object inst, Type type, string propName, object value)
    {
        var prop = type.GetProperty(propName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        if (prop == null)
        {
            throw new Exception("Cannot find property!");
        }
        prop.SetValue(inst, value);
    }

}

class Test0
{
    private int I { get; set; }
    public int GetI()
    {
        return I;
    }
}
