
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using System.Xml;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;


namespace SessionController
{
    public interface IEvent
    {
        void StartRecordingSession();
        void StopRecordingSession();
        void RecordEvent(string eventType, string eventName, String eventDetails);
    }
    
    [Export(typeof(IEvent))]
    public class SessionController : IEvent
    {
        XmlWriterSettings _xmlSettings;
        XmlWriter _xmlWriter;
        IsolatedStorageFile _isoStore;
        IsolatedStorageFileStream _isoStream;

        public void StartRecordingSession()
        {
            _isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            _isoStream = _isoStore.OpenFile("bacaroo.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            _xmlSettings = new XmlWriterSettings();
            _xmlSettings.Indent = true;

            _xmlWriter = XmlWriter.Create(_isoStream, _xmlSettings);

            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("Session");
            _xmlWriter.WriteAttributeString("StartTime", DateTime.Now.ToString());
        }

        public void StopRecordingSession()
        {
            if (_xmlWriter != null)
            {
                _xmlWriter.WriteEndElement();
                _xmlWriter.WriteEndDocument();
                _xmlWriter.Flush();
                _xmlWriter.Close();
                _isoStream.Close();
                _xmlWriter = null;
                _isoStream = null;
            }
        }

        public void RecordEvent(string eventType, string eventName, String eventDetails )
        {
            if (_xmlWriter == null)
                return;

            _xmlWriter.WriteStartElement(eventType);

            _xmlWriter.WriteAttributeString("EventName", eventName);
            _xmlWriter.WriteAttributeString("Time", DateTime.Now.ToString());
            _xmlWriter.WriteAttributeString("Details", eventDetails );
            _xmlWriter.WriteEndElement();

            Console.WriteLine("Event : " + DateTime.Now.ToString() + ":" + eventName);
        }

    }
}

