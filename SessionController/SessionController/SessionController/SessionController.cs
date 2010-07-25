
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
using System.Diagnostics;

namespace SessionController
{
    public interface IEvent
    {
        void StartRecordingSession( string id );
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
        DateTime start;

        public void StartRecordingSession( string id )
        {
            start = DateTime.Now;

            _isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            string fileName = "MS_Prototype_" + id + ".xml";
            _isoStream = _isoStore.OpenFile(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            _xmlSettings = new XmlWriterSettings();
            _xmlSettings.Indent = true;

            _xmlWriter = XmlWriter.Create(_isoStream, _xmlSettings);

            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("Session");
            _xmlWriter.WriteAttributeString("SessionId", id);

            _xmlWriter.WriteAttributeString("StartTime", start.ToString());

            Debug.WriteLine("Recording Started: " + id );
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
                Debug.WriteLine("Recording Stopped" );
            }
        }

        public void RecordEvent(string eventType, string eventName, String eventDetails )
        {
            if (_xmlWriter == null)
                return;

            _xmlWriter.WriteStartElement(eventType);

            _xmlWriter.WriteAttributeString("EventName", eventName);
            _xmlWriter.WriteAttributeString("Ticks", "" + ( DateTime.Now.Ticks - start.Ticks) );
            _xmlWriter.WriteAttributeString("Details", eventDetails );
            _xmlWriter.WriteEndElement();

            Debug.WriteLine("  Event : " + DateTime.Now.ToString() + ":" + eventName);
        }

    }
}

