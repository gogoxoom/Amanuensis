using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;


namespace WM7Mockup
{
	public partial class LockScreen : UserControl
	{
		public LockScreen()
		{
			// Required to initialize variables
			InitializeComponent();
		}
		
		DispatcherTimer _timer;

		private void LayoutRoot_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
            _timer = new DispatcherTimer();
			_timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += new EventHandler(UpdateDateAndTime);
            _timer.Start();
		}
		
		private void UpdateDateAndTime(object sender, EventArgs e)
        {
          	DateTime CurrTime = DateTime.Now;
            String str;

            str = CurrTime.Hour.ToString() + ":" + CurrTime.Minute.ToString() + ":" + CurrTime.Second.ToString();
            this.Time.Text = str;

			this.Weekday.Text = CurrTime.DayOfWeek.ToString();

            str = CurrTime.ToString("M");
            this.Date.Text = str;
			
        }
	}
}