using System;
using System.Reflection;

namespace Schedule
{

    /// <summary>
    /// IMethodCall represents a partially specified parameter data list and a method.  This allows methods to be 
    /// dynamically late invoked for things like timers and other event driven frameworks.
    /// </summary>
    public interface IMethodCall
    {
        ParameterSetterCollection CollParam { get; }

        Object Execute();
        Object Execute(IParameterSetter paramSetter);

        IAsyncResult BeginExecute(AsyncCallback callback, Object obj);
        IAsyncResult BeginExecute(IParameterSetter Params, AsyncCallback callback, Object obj);
    }

    public delegate Object Exec();

    public delegate Object ExecParam(IParameterSetter Params);

    /// <summary>
    /// This is a base class that handles the Parameter list management for the 2 dynamic method call methods.
    /// </summary>
    public class MethodCallBase
    {
        public ParameterSetterCollection CollParam { get; private set; }

        public MethodCallBase()
        {
            CollParam = new ParameterSetterCollection();
        }

        protected Object[] GetParameterList(MethodInfo methodInfo)
        {
            CollParam.Reset();
            return CollParam.GetParameters(methodInfo);
        }

        protected Object[] GetParameterList(MethodInfo methodInfo, IParameterSetter paramSetter)
        {
            CollParam.Reset();
            return CollParam.GetParameters(methodInfo, paramSetter);
        }
    }

    /// <summary>
    /// Method call captures the data required to do a defered method call.
    /// </summary>
    public class DelegateMethodCall : MethodCallBase, IMethodCall
    {
        public Delegate Func { get; set; }

        public MethodInfo MethodInfo
        {
            get { return Func.Method; }
        }

        public DelegateMethodCall(Delegate func)
        {
            Func = func;
        }

        public DelegateMethodCall(Delegate func, params Object[] parameters)
        {
            if (func.Method.GetParameters().Length < parameters.Length)
                throw new ArgumentException("Too many parameters specified for delegate", "func");

            Func = func;
            CollParam.Add(new OrderdParameterSetter(parameters));
        }

        public DelegateMethodCall(Delegate func, IParameterSetter paramSetter)
        {
            Func = func;
            CollParam.Add(paramSetter);
        }

        public Object Execute()
        {
            return Func.DynamicInvoke(GetParameterList(MethodInfo));
        }

        public Object Execute(IParameterSetter paramSetter)
        {
            return Func.DynamicInvoke(GetParameterList(MethodInfo, paramSetter));
        }

        public IAsyncResult BeginExecute(AsyncCallback callback, Object obj)
        {
            return new Exec(Execute).BeginInvoke(callback, null);
        }

        public IAsyncResult BeginExecute(IParameterSetter paramSetter, AsyncCallback callback, Object obj)
        {
            return new ExecParam(Execute).BeginInvoke(paramSetter, callback, obj);
        }
    }

    public class DynamicMethodCall : MethodCallBase, IMethodCall
    {
        readonly Object _Obj;

        public MethodInfo MethodInfo { get; set; }

        public DynamicMethodCall(MethodInfo method)
        {
            _Obj = null;
            MethodInfo = method;
        }

        public DynamicMethodCall(Object obj, MethodInfo methodInfo)
        {
            _Obj = obj;
            MethodInfo = methodInfo;
        }

        public DynamicMethodCall(Object obj, MethodInfo methodInfo, IParameterSetter paramSetter)
        {
            _Obj = obj;
            MethodInfo = methodInfo;
            CollParam.Add(paramSetter);
        }

        public Object Execute()
        {
            return MethodInfo.Invoke(_Obj, GetParameterList(MethodInfo));
        }

        public Object Execute(IParameterSetter paramSetter)
        {
            return MethodInfo.Invoke(_Obj, GetParameterList(MethodInfo, paramSetter));
        }

        public IAsyncResult BeginExecute(AsyncCallback callback, Object obj)
        {
            return new Exec(Execute).BeginInvoke(callback, null);
        }

        public IAsyncResult BeginExecute(IParameterSetter paramSetter, AsyncCallback callback, Object obj)
        {
            return new ExecParam(Execute).BeginInvoke(paramSetter, callback, null);
        }
    }
}
