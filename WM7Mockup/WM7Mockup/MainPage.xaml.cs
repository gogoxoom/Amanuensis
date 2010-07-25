using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Xml;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;





namespace WM7Mockup
{
	public partial class MainPage : UserControl
	{
		private FrameworkElement _CustomCursor = null;
		public MainPage()
		{
			// Required to initialize variables
			InitializeComponent();
			_CustomCursor = FindName ("CustomCursor") as FrameworkElement;
            
		}

		public void LayoutRoot_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
            if (_CustomCursor != null)
            {
                _CustomCursor.Opacity = 100;
            }


            /*UIElement elem = sender as UIElement;
            elem.
            if (elem.MouseMove != null)
            {
                elem.Dispatcher
            }

            IList<EventHandler<MouseEventArgs>> handlers = elem.GetValue(elem.MouseMove) as IList<EventHandler<MouseEventArgs>>;
            if (handlers != null)
            {
                foreach (EventHandler<MouseEventArgs> handler in handlers)
                {
                          handler(elem, e);
                }
            }*/

            /*FrameworkElement elem = sender as FrameworkElement;
            EventHandler<MouseEventArgs> handler = elem.Resources["MouseEnter"] as EventHandler<MouseEventArgs>;
            if (handler != null)
            {
                handler(sender, e);
            }*/

            /*BindingFlags myBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type myTypeBindingFlags = typeof(System.Windows.Controls.Button);
            EventInfo myEventBindingFlags = myTypeBindingFlags.GetEvent("MouseEnter", myBindingFlags);*/

            Type myType = sender.GetType();

            EventInfo myEvent = myType.GetEvent("MouseEnter",BindingFlags.Instance | BindingFlags.Public);



    	}

		private void LayoutRoot_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{

            if (_CustomCursor != null)
            {
                _CustomCursor.Opacity = 0;
            }
		}

		private void LayoutRoot_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
            if (_CustomCursor != null)
            {
                if ((_CustomCursor.RenderTransform == null) || (_CustomCursor.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
                {
                    _CustomCursor.RenderTransform = new TranslateTransform();
                }

                TranslateTransform transTransform = _CustomCursor.RenderTransform as TranslateTransform;
                Point pos = e.GetPosition(null);
				
				//The + 5 prevents that we trigger the MouseLeave event when the mouse moves over the CustomCursor.
                transTransform.X = pos.X - (_CustomCursor.ActualWidth / 2);
                transTransform.Y = pos.Y - (_CustomCursor.ActualHeight / 2);
            }
		}

		private void LayoutRoot_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
            if (this.MainScreenContentPresenter != null)
            {
                this.tokenList = (WM7Mockup.TokenList) this.MainScreenContentPresenter.Content;
            }
		}

		private void BackButton_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			// TODO: Add event handler implementation here.
			VisualStateManager.GoToState (this.tokenList, "LockScreenState", true);
		}

		private void LayoutRoot_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
            if (sender == null)
                return;

            FrameworkElement fe = e.OriginalSource as FrameworkElement;

           //AutomationTriggerEvent(fe, e);
            //ExecuteHandler(fe, e);
		}

        private void AutomationTriggerEvent(object sender, MouseButtonEventArgs e)
        {

            if (sender == null)
                return;

            FrameworkElement fe = sender as FrameworkElement;
            
            FrameworkElementAutomationPeer AutoPeer = new FrameworkElementAutomationPeer(LayoutRoot);
            //UIElementAutomationPeer.CreatePeerForElement

            // Create an InvokeProvider

            IInvokeProvider invokeProvider = AutoPeer.GetPattern(PatternInterface.Invoke) as IInvokeProvider;
            if (invokeProvider != null)
            {
                invokeProvider.Invoke();
            }
            
            
            DependencyObject parent = VisualTreeHelper.GetParent(fe as DependencyObject);
            AutomationTriggerEvent(parent, e);
        }

        private void ExecuteHandler(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender == null)
                return;

            FrameworkElement fe = sender as FrameworkElement;

                     

            Type senderType = sender.GetType();
            EventInfo []eInfo = senderType.GetEvents ();
            EventInfo eInfo2 = senderType.GetEvent("MouseLeftButtonDown", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            MethodInfo[] mInfo2 = senderType.GetMethods();
            MethodInfo mInfo3 = eInfo2.GetRaiseMethod();

            //"Free" eventhandler so we can call call GetInvocationList
            //FieldInfo fInfo = sender.GetType().GetField("Background", BindingFlags.Public | BindingFlags.Instance);
            FieldInfo[] fInfo = sender.GetType().GetFields();
            if (fInfo != null)
            {
                Debug.WriteLine("I found MethodInfo.");
            }
            //EventHandler eh = field.GetValue(ctrl) as EventHandler;

            PropertyInfo[] pinfo = sender.GetType().GetProperties();
            
             //This snippet of code executes the LayoutRoot_MouseEnter fucntion.
            MethodInfo mInfo = sender.GetType().GetMethod("MetroPanelMouseLeftButtonDown", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            
            if (mInfo != null)
            {
                Debug.WriteLine("I found MethodInfo.");
                object[] mParms = new object[] { sender, e };
                mInfo.Invoke(sender, mParms);
                
            }

            ExecuteHandler(fe.Parent, e);
        }


        private void StartRecordingButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StartRecording();
        }

        private void StopRecordingButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            StopRecording();
        }

        XmlWriterSettings _xmlSettings;
        XmlWriter _xmlWriter;
        IsolatedStorageFile _isoStore;
        IsolatedStorageFileStream _isoStream;

        private void StartRecording()
        {
            //Make sure that we are not registering multiple events.
            AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RecordMouseLeftButtonDown), true);
            AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(RecordMouseLeftButtonUp), true);
            //MouseMove += new MouseEventHandler(RecordMouseMove);
            //MouseEnter += new MouseEventHandler(RecordMouseEnter);
            //MouseLeave += new MouseEventHandler(RecordMouseLeave);


            _isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            _isoStream = _isoStore.OpenFile ("bacaroo.xml", FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);

            _xmlSettings = new XmlWriterSettings();
            _xmlSettings.Indent = true;

            _xmlWriter = XmlWriter.Create(_isoStream, _xmlSettings);

            /*DateTime CurrTime = DateTime.Now;
            String str;

            str = CurrTime.Hour.ToString() + ":" + CurrTime.Minute.ToString() + ":" + CurrTime.Second.ToString();
            this.Time.Text = str;

			this.Weekday.Text = CurrTime.DayOfWeek.ToString();

            str = CurrTime.ToString("M");
            this.Date.Text = str;*/

            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("Session");
            _xmlWriter.WriteAttributeString("StartTime", DateTime.Now.ToString());
        }

        private void StopRecording()
        {
            
            RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RecordMouseLeftButtonDown));
            RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RecordMouseLeftButtonUp));

            //MouseMove -= RecordMouseMove;
            //MouseEnter -= RecordMouseEnter;
            //MouseLeave -= RecordMouseLeave;
            
            

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

        private void RecordMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RecordEvent("MouseButtonEvent", "MouseLeftButtonDown", e.GetPosition(null));
        }

        private void RecordMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            RecordEvent("MouseButtonEvent", "MouseLeftButtonUp", e.GetPosition(null));
        }

        private void RecordMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RecordEvent("MouseEvent", "MouseEnter", e.GetPosition(null));
        }

        private void RecordMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RecordEvent("MouseEvent", "MouseLeave", e.GetPosition(null));
        }

        private void RecordMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            RecordEvent("MouseEvent", "MouseMove", e.GetPosition(null));
        }

        private void RecordEvent(string eventType, string eventName, Point pt)
        {
            if (_xmlWriter == null)
                return;

            _xmlWriter.WriteStartElement(eventType);

            _xmlWriter.WriteAttributeString("EventName", eventName);
            _xmlWriter.WriteAttributeString("Time", DateTime.Now.ToString());
            _xmlWriter.WriteAttributeString("X", pt.X.ToString());
            _xmlWriter.WriteAttributeString("Y", pt.Y.ToString());
            _xmlWriter.WriteEndElement();
        }

            
        Thread _playbackThread = null;
        XmlReader _xmlReader = null;
        private void StartPlayback()
        {
            _playbackThread = new Thread(new ThreadStart(PlaybackEvents));
            _playbackThread.Start();
        }

        private void PlaybackEvents()
        {
            _isoStore = IsolatedStorageFile.GetUserStoreForApplication();
            _isoStream = _isoStore.OpenFile("bacaroo.xml", FileMode.Open, FileAccess.Read, FileShare.Read);
            _xmlReader = XmlReader.Create(_isoStream);
            DateTime _prevDateTime = new DateTime(0);

            while (_xmlReader.Read())
            {
                if (_xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (_xmlReader.Name == "Session")
                    {
                        string strTime = _xmlReader.GetAttribute ("StartTime");
                        _prevDateTime = DateTime.Parse(strTime);

                    }
                    else if ( (_xmlReader.Name == "MouseButtonEvent") ||  (_xmlReader.Name == "MouseEvent"))
                    {
                        // Get the time.
                        string strTime = _xmlReader.GetAttribute("Time");
                        DateTime currDateTime = DateTime.Parse(strTime);
                        TimeSpan sleepTime = currDateTime - _prevDateTime;

                        string strEventName = _xmlReader.GetAttribute("EventName");

                        Point pt = new Point(Double.Parse(_xmlReader.GetAttribute("X")), Double.Parse(_xmlReader.GetAttribute("Y")));

                        Thread.Sleep(sleepTime);

                        Dispatcher.BeginInvoke(()=>ExecuteEvent(strEventName, pt));
                        _prevDateTime = currDateTime;
                    }
                }
            }

            _xmlReader.Close();
            _isoStream.Close();
            _xmlReader = null;
            _isoStream = null;
        }

        private void ExecuteEvent(string mouseEvent, Point mousePoint)
        {
            //Debug.WriteLine("Event: {0}, Point: {1}", mouseEvent, mousePoint.ToString());
            foreach (UIElement elem in VisualTreeHelper.FindElementsInHostCoordinates(mousePoint, this))
            {

                FrameworkElement fe = elem as FrameworkElement;

                foreach (MethodInfo info in fe.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public))
                {

                    Debug.WriteLine("OjbectsType: {0}, Method: {1}", fe.GetType(), info.Name);
                    if ("TileMouseLeftButtonDown" == info.Name)
                    {
                        RoutedEventArgs e = new RoutedEventArgs();
                        
                        object[] methodParams = new object[] { fe, null};
                        info.Invoke(fe, methodParams);
                    }
                }
            }
            
        }

        private void StartPlaybackButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	StartPlayback ();
        }
	}
}