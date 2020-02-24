using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;

public class PropertyAnimator : MonoBehaviour
{
    /// <summary>
    /// Singleton
    /// </summary>
    #region Singleton
    private static PropertyAnimator instance = null;
    public static PropertyAnimator GetInstance()
    {
        return instance;
    }
    private void MakeSingleton()
    {
        if (!instance)
            instance = this;
    }
    #endregion

    private List<PropertyUpdaterFloat> updatersFloat = new List<PropertyUpdaterFloat>();
    private List<PropertyUpdaterVector2> updatersVec2 = new List<PropertyUpdaterVector2>();
    private List<PropertyUpdaterVector3> updatersVec3 = new List<PropertyUpdaterVector3>();

    private void Awake()
    {
        MakeSingleton();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        foreach(var updater in updatersFloat)
        {
            updater.Update();
        }
        foreach (var updater in updatersVec2)
        {
            updater.Update();
        }
        foreach (var updater in updatersVec3)
        {
            updater.Update();
        }
    }

    //TODO: 写 开始，暂停，继续，停止  并构造对应事件

    public void StartAnimatingPropertyFloat(object component, string strProp, float src, float targ, float durTime)
    {
        PropertyWrapper<float> wrapper = new PropertyWrapper<float>(component, strProp);
        PropertyUpdaterFloat updater = new PropertyUpdaterFloat(wrapper, src, targ, durTime);
        updatersFloat.Add(updater);
        updater.Start();
    }
    public void StartAnimatingPropertyVector2(object component, string strProp, Vector2 src, Vector2 targ, float durTime)
    {
        PropertyWrapper<Vector2> wrapper = new PropertyWrapper<Vector2>(component, strProp);
        PropertyUpdaterVector2 updater = new PropertyUpdaterVector2(wrapper, src, targ, durTime);
        updatersVec2.Add(updater);
        updater.Start();
    }
    public void StartAnimatingPropertyVector3(object component, string strProp, Vector3 src, Vector3 targ, float durTime)
    {
        PropertyWrapper<Vector3> wrapper = new PropertyWrapper<Vector3>(component, strProp);
        PropertyUpdaterVector3 updater = new PropertyUpdaterVector3(wrapper, src, targ, durTime);
        updatersVec3.Add(updater);
        updater.Start();
    }


    private PropertyUpdaterFloat FindAnimatingPropertyFloat(object component, string strProp)
    {
        return updatersFloat.Find(delegate (PropertyUpdaterFloat updater)
                            {
                                return updater.prop.component == component && updater.prop.strProp == strProp;
                            });
    }
}

abstract class PropertyUpdater<T> 
{
    public PropertyWrapper<T> prop;
    public T src;
    public T targ;
    public float durTime;
    //public bool treatAsInt;
    protected bool isChanging = false;

    public PropertyUpdater(PropertyWrapper<T> prop, T src, T targ, float durTime)//, bool treatAsInt)
    {
        this.prop = prop;
        this.src = src;
        this.targ = targ;
        this.durTime = durTime;
        //this.treatAsInt = treatAsInt;
    }

    /// <summary>
    /// 在Mono的Update中调用
    /// </summary>
    /// <returns>Is update done?</returns>
    public abstract bool Update();

    public virtual void Start()
    {
        prop.Set(src);
        Resume();
    }

    public virtual void End()
    {
        Pause();
        prop.Set(targ);
    }

    public virtual void Pause()
    {
        isChanging = false;
    }

    public virtual void Resume()
    {
        isChanging = true;
    }
}

class PropertyUpdaterFloat : PropertyUpdater<float>
{
    public PropertyUpdaterFloat(PropertyWrapper<float> prop, float src, float targ, float durTime):base(prop, src, targ, durTime){}

    public override bool Update()
    {
        var inter = prop.Get();
        if (isChanging && Mathf.Abs(inter - src) <= Mathf.Abs(targ - src))
        {
            inter = Mathf.Clamp(inter + (targ - src) / durTime * Time.deltaTime, src, targ);
            prop.Set(inter);
            return false;
        }
        else
        {
            isChanging = false;
            return true;
        }
    }
}

class PropertyUpdaterVector2 : PropertyUpdater<Vector2>
{
    private float time = 0f;

    public PropertyUpdaterVector2(PropertyWrapper<Vector2> prop, Vector2 src, Vector2 targ, float durTime) : base(prop, src, targ, durTime) { }

    public override bool Update()
    {
        var inter = prop.Get();
        if (isChanging && time <= durTime)
        {
            time += Time.deltaTime;
            inter = Vector2.Lerp(targ, src, time / durTime);
            prop.Set(inter);
            return false;
        }
        else
        {
            isChanging = false;
            return true;
        }
    }

    public override void Start()
    {
        time = 0f;
        base.Start();
    }
}

class PropertyUpdaterVector3: PropertyUpdater<Vector3>
{
    private float time = 0f;

    public PropertyUpdaterVector3(PropertyWrapper<Vector3> prop, Vector3 src, Vector3 targ, float durTime) : base(prop, src, targ, durTime) { }

    public override bool Update()
    {
        var inter = prop.Get();
        if (isChanging && time <= durTime)
        {
            time += Time.deltaTime;
            inter = Vector3.Lerp(targ, src, time / durTime);
            prop.Set(inter);
            return false;
        }
        else
        {
            isChanging = false;
            return true;
        }
    }

    public override void Start()
    {
        time = 0f;
        base.Start();
    }
}

class PropertyWrapper<T>
{

    public PropertyWrapper(object component, string strProp)//, Type typeProp)
    {
        Init(component, component.GetType(), strProp);//, typeProp);
    }

    public PropertyWrapper(object component, Type typeComponent, string strProp)//, Type typeProp)
    {
        Init(component, typeComponent, strProp);//, typeProp);
    }

    private void Init(object component, Type typeComponent, string strProp)//, Type typeProp)
    {
        this.component = component;
        this.typeComponent = typeComponent;
        this.strProp = strProp;
        //this.typeProp = typeProp;
    }

    public object component;
    public Type typeComponent;
    public string strProp;
    //public Type typeProp;

    public T Get()//object Get()
    {
        return (T)GetProperty(component, typeComponent, strProp);
    }

    public void Set(T val)
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
