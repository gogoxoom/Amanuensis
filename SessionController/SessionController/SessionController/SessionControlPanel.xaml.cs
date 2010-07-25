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


namespace SessionController
{
	public partial class SessionControlPanel : UserControl
	{
		public SessionControlPanel()
		{
			// Required to initialize variables
			InitializeComponent();
           
        }

        [Import]
        public IEvent Event { get; set; }
		
		
		private void LayoutRoot_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
        	CompositionInitializer.SatisfyImports(this);
        }

        void go()
        {
			if (Event != null)
            	Event.StartRecordingSession( SessionId.Text );
        }

        void stop()
        {
			if (Event != null)
            	Event.StopRecordingSession();
        }

        private void StartBlock_Click(object sender, RoutedEventArgs e)
        {
          go();
        }

        private void StopBlock_Click(object sender, RoutedEventArgs e)
        {
          stop();
        }

        private void Submit_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	if (Event != null)
				Event.RecordEvent ("Comment", "Comment", this.Comments.Text);
        }

	}
}