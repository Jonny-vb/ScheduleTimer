using System;
using System.Xml;

namespace TransClock
{
    class Config
    {
        readonly String _FileName;
        readonly XmlDocument _Doc = new XmlDocument();

        public Config(String StrFile)
        {
            _FileName = StrFile;
            _Doc.Load(_FileName);
        }

        public String GetSetting(String key, String defaults = null)
        {
            if (null == defaults) defaults = String.Empty;
            var node = _Doc.SelectSingleNode("configuration/appSettings/add[@key='" + key + "']");
            return null == node ? defaults : ReadWithDefault(node.Attributes["value"].Value, defaults);
        }

        public void SetSetting(String key, String defaults)
        {
            var node = _Doc.SelectSingleNode("configuration/appSettings/add[@key='" + key + "']");
            if (null != node) node.Attributes["value"].Value = defaults;
            _Doc.Save(_FileName);
        }

        public static String ReadWithDefault(String StrValue, String StrDefault)
        {
            return StrValue ?? StrDefault;
        }
    }
}
