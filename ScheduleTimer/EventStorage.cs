/***************************************************************************
 * Copyright Andy Brummer 2004-2005
 * 
 * This code is provided "as is", with absolutely no warranty expressed
 * or implied. Any use is at your own risk.
 *
 * This code may be used in compiled form in any way you desire. This
 * file may be redistributed unmodified by any means provided it is
 * not sold for profit without the authors written consent, and
 * providing that this notice and the authors name is included. If
 * the source code in  this file is used in any commercial application
 * then a simple email would be nice.
 * 
 **************************************************************************/

using System;
using System.Globalization;
using System.Xml;

namespace Schedule
{
    /// <summary>
    /// IEventStorage is used to provide persistance of schedule during service shutdowns.
    /// </summary>
    public interface IEventStorage
    {
        void RecordLastTime(DateTime Time);
        DateTime ReadLastTime();
    }

    /// <summary>
    /// Null event strorage disables error recovery by returning now for the last time an event fired.
    /// </summary>
    public class NullEventStorage : IEventStorage
    {
        public void RecordLastTime(DateTime Time)
        {
        }

        public DateTime ReadLastTime()
        {
            return DateTime.Now;
        }
    }

    /// <summary>
    /// Local event strorage keeps the last time in memory so that skipped events are not recovered.
    /// </summary>
    public class LocalEventStorage : IEventStorage
    {
        DateTime _dtLast;

        public LocalEventStorage()
        {
            _dtLast = DateTime.MaxValue;
        }

        public void RecordLastTime(DateTime datetime)
        {
            _dtLast = datetime;
        }

        public DateTime ReadLastTime()
        {
            if (DateTime.MaxValue == _dtLast) _dtLast = DateTime.Now;
            return _dtLast;
        }
    }

    /// <summary>
    /// FileEventStorage saves the last time in an XmlDocument so that recovery will include periods that the 
    /// process is shutdown.
    /// </summary>
    public class FileEventStorage : IEventStorage
    {
        readonly String _FileName;
        
        readonly String _XPath;
        
        readonly XmlDocument _xmlDoc;

        public FileEventStorage(String fileName, String xpath)
        {
            _FileName = fileName;
            _XPath = xpath;
            _xmlDoc = new XmlDocument();
        }

        public void RecordLastTime(DateTime datetime)
        {
            var xmlNode = _xmlDoc.SelectSingleNode(_XPath);
            if (null != xmlNode) xmlNode.Value = datetime.ToString(CultureInfo.InvariantCulture);
            _xmlDoc.Save(_FileName);
        }

        public DateTime ReadLastTime()
        {
            _xmlDoc.Load(_FileName);
            var xmlNode = _xmlDoc.SelectSingleNode(_XPath);
            if (null != xmlNode)
            {
                var value = xmlNode.Value;
                return String.IsNullOrEmpty(value) ? DateTime.Now : DateTime.Parse(value);
            }
            return DateTime.Now;
        }

    }
}
