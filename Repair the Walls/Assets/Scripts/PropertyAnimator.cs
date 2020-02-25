using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Reflection;

//TODO: Construct thread safety

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

    public enum EndPropertyState
    {
        Src,
        Targ,
        Keep
    }

    private List<PropertyUpdater<float>> updatersFloat = new List<PropertyUpdater<float>>();
    private List<PropertyUpdater<Vector2>> updatersVector2 = new List<PropertyUpdater<Vector2>>();
    private List<PropertyUpdater<Vector3>> updatersVector3 = new List<PropertyUpdater<Vector3>>();
    private List<PropertyUpdater<int>> updatersInt = new List<PropertyUpdater<int>>();
    private List<PropertyUpdater<Vector2Int>> updatersVector2Int = new List<PropertyUpdater<Vector2Int>>();
    private List<PropertyUpdater<Vector3Int>> updatersVector3Int = new List<PropertyUpdater<Vector3Int>>();

    //private List<UnityEvent> eventsStart = new List<UnityEvent>();
    //private List<UnityEvent> eventsPause = new List<UnityEvent>();
    //private List<UnityEvent> eventsResume = new List<UnityEvent>();
    //private List<UnityEvent> eventsEnd = new List<UnityEvent>();

    private Dictionary<Tuple<object, string>, UnityEvent> eventsDicStart = new Dictionary<Tuple<object, string>, UnityEvent>();
    private Dictionary<Tuple<object, string>, UnityEvent> eventsDicPause = new Dictionary<Tuple<object, string>, UnityEvent>();
    private Dictionary<Tuple<object, string>, UnityEvent> eventsDicResume = new Dictionary<Tuple<object, string>, UnityEvent>();
    private Dictionary<Tuple<object, string>, UnityEvent> eventsDicEnd = new Dictionary<Tuple<object, string>, UnityEvent>();

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
        foreach (var updater in updatersVector2)
        {
            updater.Update();
        }
        foreach (var updater in updatersVector3)
        {
            updater.Update();
        }
        foreach (var updater in updatersInt)
        {
            updater.Update();
        }
        foreach (var updater in updatersVector2Int)
        {
            updater.Update();
        }
        foreach (var updater in updatersVector3Int)
        {
            updater.Update();
        }
    }

    //TODO: 构造 开始，暂停，继续，停止  对应事件

    #region StartAnimatingProperty
    /// <summary>
    /// Begin to animate a property of an object
    /// </summary>
    /// <param name="component">Object containing the prop</param>
    /// <param name="strProp">Prop in string</param>
    /// <param name="src">Start value</param>
    /// <param name="targ">End value</param>
    /// <param name="durTime">Duration</param>
    /// <returns>Is successful?</returns>
    public bool StartAnimatingPropertyFloat(object component, string strProp, float src, float targ, float durTime)
    {
        return StartAnimatingPeoperty<float>(updatersFloat, component, strProp, src, targ, durTime, PropertyUpdater<float>.GetValAtUpdateFloat);
    }
    public bool StartAnimatingPropertyVector2(object component, string strProp, Vector2 src, Vector2 targ, float durTime)
    {
        return StartAnimatingPeoperty<Vector2>(updatersVector2, component, strProp, src, targ, durTime, PropertyUpdater<float>.GetValAtUpdateVector2);
    }
    public bool StartAnimatingPropertyVector3(object component, string strProp, Vector3 src, Vector3 targ, float durTime)
    {
        return StartAnimatingPeoperty<Vector3>(updatersVector3, component, strProp, src, targ, durTime, PropertyUpdater<float>.GetValAtUpdateVector3);
    }
    public bool StartAnimatingPropertyInt(object component, string strProp, int src, int targ, float durTime)
    {
        return StartAnimatingPeoperty<int>(updatersInt, component, strProp, src, targ, durTime, PropertyUpdater<int>.GetValAtUpdateInt);
    }
    public bool StartAnimatingPropertyVector2Int(object component, string strProp, Vector2Int src, Vector2Int targ, float durTime)
    {
        return StartAnimatingPeoperty<Vector2Int>(updatersVector2Int, component, strProp, src, targ, durTime, PropertyUpdater<Vector2Int>.GetValAtUpdateVector2Int);
    }
    public bool StartAnimatingPropertyVector3Int(object component, string strProp, Vector3Int src, Vector3Int targ, float durTime)
    {
        return StartAnimatingPeoperty<Vector3Int>(updatersVector3Int, component, strProp, src, targ, durTime, PropertyUpdater<Vector3Int>.GetValAtUpdateVector3Int);
    }

    private bool StartAnimatingPeoperty<T>(List<PropertyUpdater<T>> updaters, object component, string strProp, T src, T targ, float durTime, PropertyUpdater<T>.GetValAtUpdate getValAtUpdate)
    {
        if(FindAnimatingProperty<T>(updaters, component, strProp) != null)
        {
            Debug.LogWarning("Property is already in animating.");
            return false;
        }
        PropertyWrapper<T> wrapper = new PropertyWrapper<T>(component, strProp);
        PropertyUpdater<T> updater = new PropertyUpdater<T>(wrapper, src, targ, durTime, getValAtUpdate);
        updaters.Add(updater);
        updater.Start();
        return true;
    }
    #endregion

    #region PauseAnimatingProperty
    /// <summary>
    /// Pause animating the property
    /// </summary>
    /// <param name="component">Object containing the prop</param>
    /// <param name="strProp">Prop in string</param>
    /// <returns>Is successful?</returns>
    public bool PauseAnimatingPropertyFloat(object component, string strProp)
    {
        return PauseAnimatingProperty<float>(updatersFloat, component, strProp);
    }
    public bool PauseAnimatingPropertyVector2(object component, string strProp)
    {
        return PauseAnimatingProperty<Vector2>(updatersVector2, component, strProp);
    }
    public bool PauseAnimatingPropertyVector3(object component, string strProp)
    {
        return PauseAnimatingProperty<Vector3>(updatersVector3, component, strProp);
    }
    public bool PauseAnimatingPropertyInt(object component, string strProp)
    {
        return PauseAnimatingProperty<int>(updatersInt, component, strProp);
    }
    public bool PauseAnimatingPropertyVector2Int(object component, string strProp)
    {
        return PauseAnimatingProperty<Vector2Int>(updatersVector2Int, component, strProp);
    }
    public bool PauseAnimatingPropertyVector3Int(object component, string strProp)
    {
        return PauseAnimatingProperty<Vector3Int>(updatersVector3Int, component, strProp);
    }

    private bool PauseAnimatingProperty<T>(List<PropertyUpdater<T>> updaters, object component, string strProp)
    {
        PropertyUpdater<T> updater = FindAnimatingProperty<T>(updaters, component, strProp);
        if (updater == null)
            return false;
        updater.Pause();
        return true;
    }
    #endregion

    #region ResumeAnimatingProperty
    /// <summary>
    /// Resume animating the property
    /// </summary>
    /// <param name="component">Object containing the prop</param>
    /// <param name="strProp">Prop in string</param>
    /// <returns>Is successful?</returns>
    public bool ResumeAnimatingPropertyFloat(object component, string strProp)
    {
        return ResumeAnimatingProperty<float>(updatersFloat, component, strProp);
    }
    public bool ResumeAnimatingPropertyVector2(object component, string strProp)
    {
        return ResumeAnimatingProperty<Vector2>(updatersVector2, component, strProp);
    }
    public bool ResumeAnimatingPropertyVector3(object component, string strProp)
    {
        return ResumeAnimatingProperty<Vector3>(updatersVector3, component, strProp);
    }
    public bool ResumeAnimatingPropertyInt(object component, string strProp)
    {
        return ResumeAnimatingProperty<int>(updatersInt, component, strProp);
    }
    public bool ResumeAnimatingPropertyVector2Int(object component, string strProp)
    {
        return ResumeAnimatingProperty<Vector2Int>(updatersVector2Int, component, strProp);
    }
    public bool ResumeAnimatingPropertyVector3Int(object component, string strProp)
    {
        return ResumeAnimatingProperty<Vector3Int>(updatersVector3Int, component, strProp);
    }

    private bool ResumeAnimatingProperty<T>(List<PropertyUpdater<T>> updaters, object component, string strProp)
    {
        PropertyUpdater<T> updater = FindAnimatingProperty<T>(updaters, component, strProp);
        if (updater == null)
            return false;
        updater.Resume();
        return true;
    }
    #endregion

    #region EndAnimatingProperty
    /// <summary>
    /// 
    /// </summary>
    /// <param name="component">Object containing the prop</param>
    /// <param name="strProp">Prop in string</param>
    /// <param name="endState">Set the state of property value when animating ended</param>
    /// <returns>Is successful?</returns>
    public bool EndAnimatingPropertyFloat(object component, string strProp, EndPropertyState endState)
    {
        return EndAnimatingProperty<float>(updatersFloat, component, strProp, endState);
    }
    public bool EndAnimatingPropertyVector2(object component, string strProp, EndPropertyState endState)
    {
        return EndAnimatingProperty<Vector2>(updatersVector2, component, strProp, endState);
    }
    public bool EndAnimatingPropertyVector3(object component, string strProp, EndPropertyState endState)
    {
        return EndAnimatingProperty<Vector3>(updatersVector3, component, strProp, endState);
    }
    public bool EndAnimatingPropertyInt(object component, string strProp, EndPropertyState endState)
    {
        return EndAnimatingProperty<int>(updatersInt, component, strProp, endState);
    }
    public bool EndAnimatingPropertyVector2Int(object component, string strProp, EndPropertyState endState)
    {
        return EndAnimatingProperty<Vector2Int>(updatersVector2Int, component, strProp, endState);
    }
    public bool EndAnimatingPropertyVector3Int(object component, string strProp, EndPropertyState endState)
    {
        return EndAnimatingProperty<Vector3Int>(updatersVector3Int, component, strProp, endState);
    }

    private bool EndAnimatingProperty<T>(List<PropertyUpdater<T>> updaters, object component, string strProp, EndPropertyState endState)
    {
        PropertyUpdater<T> updater = FindAnimatingProperty<T>(updaters, component, strProp);
        if (updater == null)
            return false;
        switch (endState)
        {
            case EndPropertyState.Src:
                updater.Pause();
                updater.SetStartVal();
                break;
            case EndPropertyState.Targ:
                updater.End();
                break;
            case EndPropertyState.Keep:
                updater.Pause();
                break;
        }
        updaters.Remove(updater);
        return true;
    }
    #endregion

    private PropertyUpdater<T> FindAnimatingProperty<T>(List<PropertyUpdater<T>> updaters, object component, string strProp)
    {
        return updaters.Find(delegate (PropertyUpdater<T> updater)
        {
            return updater.prop.component == component && updater.prop.strProp == strProp;
        });
    }
    private UnityEvent FindPropertyEvent<T>(Dictionary<Tuple<object, string>, UnityEvent> events, object component, string strProp)
    {
        Tuple<object, string> key = new Tuple<object, string>(component, strProp);
        UnityEvent uevent;
        if (!events.TryGetValue(key, out uevent))
            return null;
        return uevent;
    }
}

class PropertyUpdater<T>
{
    public PropertyWrapper<T> prop;
    public T src;
    public T targ;
    public float durTime;
    protected float time = 0f;
    protected bool isChanging = false;
    public delegate T GetValAtUpdate(T src, T targ, float time, float durTime);
    protected GetValAtUpdate getValAtUpdate;

    public PropertyUpdater(PropertyWrapper<T> prop, T src, T targ, float durTime, GetValAtUpdate getValAtUpdate)
    {
        this.prop = prop;
        this.src = src;
        this.targ = targ;
        this.durTime = durTime;
        this.getValAtUpdate = getValAtUpdate;
    }

    /// <summary>
    /// Update property value. Called in Update in MonoBehaviour
    /// </summary>
    /// <returns>Is update done?</returns>
    public virtual bool Update()
    {
        if (isChanging && time <= durTime)
        {
            time += Time.deltaTime;
            var inter = getValAtUpdate(src, targ, time, durTime);
            prop.Set(inter);
            return false;
        }
        else
        {
            if (time < durTime)
                //Paused
                return false;
            else
                //End
                return true;
        }
    }

    public virtual void Start()
    {
        time = 0f;
        SetStartVal();
        Resume();
    }

    public virtual void End()
    {
        Pause();
        SetEndVal();
    }

    public virtual void SetStartVal()
    {
        prop.Set(src);
    }

    public virtual void SetEndVal()
    {
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

    // Static methods for init the delegate
    public static float GetValAtUpdateFloat(float src, float targ, float time, float durTime) { return Mathf.Lerp(src, targ, time / durTime); }
    public static Vector2 GetValAtUpdateVector2(Vector2 src, Vector2 targ, float time, float durTime) { return Vector2.Lerp(src, targ, time / durTime); }
    public static Vector3 GetValAtUpdateVector3(Vector3 src, Vector3 targ, float time, float durTime) { return Vector3.Lerp(src, targ, time / durTime); }
    public static int GetValAtUpdateInt(int src, int targ, float time, float durTime) { return Mathf.FloorToInt(Mathf.Lerp(src, targ, time / durTime)); }
    public static Vector2Int GetValAtUpdateVector2Int(Vector2Int src, Vector2Int targ, float time, float durTime) { return Vector2Int.FloorToInt(Vector2.Lerp(src, targ, time / durTime)); }
    public static Vector3Int GetValAtUpdateVector3Int(Vector3Int src, Vector3Int targ, float time, float durTime) { return Vector3Int.FloorToInt(Vector3.Lerp(src, targ, time / durTime)); }
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
