using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;

namespace Schedule
{
    /// <summary>
    /// ParameterSetterList maintains a collection of IParameterSetter objects and applies them in order to each
    /// parameter of the method.  Each time a match occurs the next parameter is tried starting with the first 
    /// setter object until it is matched.
    /// </summary>
    public class ParameterSetterCollection : Collection<IParameterSetter>
    {
        public List<IParameterSetter> Setters
        {
            get { return Items as List<IParameterSetter>; }
        }

        public void Reset()
        {
            foreach (var setter in Items)
                setter.Reset();
        }

        public object[] GetParameters(MethodInfo methodInfo)
        {
            var parameters = methodInfo.GetParameters();
            var Values = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; ++i)
            //for (var i = parameters.Length - 1; i >= 0; --i)
                SetValue(parameters[i], i, ref Values[i]);

            return Values;
        }

        public object[] GetParameters(MethodInfo methodInfo, IParameterSetter lastSetter)
        {
            var parameters = methodInfo.GetParameters();
            var Values = new object[parameters.Length];
            for (var i = 0; i < parameters.Length; ++i)
            //for (var i = parameters.Length - 1; i >= 0; --i)
            {
                if (!SetValue(parameters[i], i, ref Values[i]))
                    lastSetter.GetParameterValue(parameters[i], i, ref Values[i]);
            }
            return Values;
        }

        bool SetValue(ParameterInfo paramInfo, int i, ref object Value)
        {
            foreach (var setter in Items)
            {
                if (setter.GetParameterValue(paramInfo, i, ref Value))
                    return true;
            }
            return false;
        }
    }
}
