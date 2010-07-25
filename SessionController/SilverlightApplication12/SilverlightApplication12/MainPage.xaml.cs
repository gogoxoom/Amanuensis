using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using SessionController;

namespace SilverlightApplication12
{
	public partial class MainPage : UserControl
	{
		public MainPage()
		{
			// Required to initialize variables
			InitializeComponent();
            CompositionInitializer.SatisfyImports(this);
        }

        [Import]
        public IEvent Event { get; set; }

        void go()
        {			
            Event.StartRecordingSession( SessionId.Text );

            AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RecordMouseLeftButtonDown), true);
            AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(RecordMouseLeftButtonUp), true);
        }

        void stop()
        {
            RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RecordMouseLeftButtonDown));
            RemoveHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(RecordMouseLeftButtonUp));

            Event.StopRecordingSession();
        }

        private void RecordMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Event.RecordEvent("MouseButtonEvent", "MouseLeftButtonDown", e.GetPosition(null).ToString() );
        }

        private void RecordMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Event.RecordEvent("MouseButtonEvent", "MouseLeftButtonUp", e.GetPosition(null).ToString() );
        }

        private void StartBlock_Click(object sender, RoutedEventArgs e)
        {
          go();
        }

        private void StopBlock_Click(object sender, RoutedEventArgs e)
        {
          stop();
        }
    }
}