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


namespace SilverlightApplication12
{
    public interface ILog
    {
        void Log(string message);
    }
    
    [Export(typeof(ILog))]
    public class SessionController : ILog
    {
        XmlWriterSettings _xmlSettings;
        XmlWriter _xmlWriter;
        IsolatedStorageFile _isoStore;
        IsolatedStorageFileStream _isoStream;

        public void Log(string message)
        {
            Console.WriteLine("Logging" + message);
        }
    }
}
