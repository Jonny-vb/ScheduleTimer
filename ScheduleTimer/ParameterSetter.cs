using System;
using System.Collections;
using System.Reflection;

namespace Schedule
{
    /// <summary>
    /// IParameterSetter represents a serialized parameter list.  This is used to provide a partial specialized
    /// method call.  This is useful for remote invocation of method calls.  For example if you have a method with
    /// 3 parameters.  The first 2 might represent static data such as a report and a storage location.  The third
    /// might be the time that the report is invoked, which is only known when the method is invoked.  Using this,
    /// you just pass the method and the first 2 parameters to a timer Object, which supplies the 3rd parameter.
    /// Without these objects, you would have to generate a custom Object type for each method you wished to 
    /// execute in this manner and store the static parameters as instance variables.  
    /// </summary>
    public interface IParameterSetter
    {
        /// <summary>
        /// This resets the setter to the beginning.  It is used for setters that rely on positional state
        /// information.  It is called prior to setting any method values.
        /// </summary>
        void Reset();
        /// <summary>
        /// This method is used to both query support for setting a parameter and actually set the value.
        /// True is returned if the parameter passed in is updated.
        /// </summary>
        /// <param name="pi">The reflection information about this parameter.</param>
        /// <param name="ParameterLoc">The location of the prameter in the parameter list.</param>
        /// <param name="parameter">The parameter Object</param>
        /// <returns>true if the parameter is matched and false otherwise</returns>
        bool GetParameterValue(ParameterInfo pi, int ParameterLoc, ref Object parameter);
    }

    /// <summary>
    /// This setter Object takes a simple Object array full of parameter data.  It applys the objects in order
    /// to the method parameter list.
    /// </summary>
    public class OrderdParameterSetter : IParameterSetter
    {
        readonly Object[] _arrParams;
        int _Counter;

        public OrderdParameterSetter(params Object[] arrParams)
        {
            _arrParams = arrParams;
        }

        public void Reset()
        {
            _Counter = 0;
        }

        public bool GetParameterValue(ParameterInfo paramInfo, int paramLoc, ref Object parameter)
        {
            if (_Counter >= _arrParams.Length) return false;
            parameter = _arrParams[_Counter++];
            return true;
        }
    }

    /// <summary>
    /// This setter Object stores the parameter data in a Hashtable and uses the hashtable keys to match 
    /// the parameter names of the method to the parameter data.  This allows methods to be called like 
    /// stored procedures, with the parameters being passed in independent of order.
    /// </summary>
    public class NamedParameterSetter : IParameterSetter
    {
        readonly Hashtable _hashParams;

        public NamedParameterSetter(Hashtable hashParam)
        {
            _hashParams = hashParam;
        }

        public void Reset()
        {
        }

        public bool GetParameterValue(ParameterInfo paramInfo, int paramLoc, ref Object parameter)
        {
            var ParamName = paramInfo.Name;
            if (!_hashParams.ContainsKey(ParamName)) return false;
            parameter = _hashParams[ParamName];
            return true;
        }
    }

}
