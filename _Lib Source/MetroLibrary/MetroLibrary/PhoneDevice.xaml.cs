using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace MetroLibrary
{
	public partial class PhoneDevice : UserControl
    {

        #region Dependency Properties

        public static readonly DependencyProperty DeviceContentProperty = DependencyProperty.Register("DeviceContent", typeof(FrameworkElement), typeof(PhoneDevice), new PropertyMetadata(null,new PropertyChangedCallback (OnDeviceContentPropertyChanged)));
        [Description("Set Content of the Phone Device"), Category("Metro Phone Device")]
        public FrameworkElement DeviceContent
        {
            get { return (FrameworkElement)GetValue(DeviceContentProperty); }
            set { SetValue(DeviceContentProperty, value); }
        }

        #endregion 

        private static void OnDeviceContentPropertyChanged (DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            (source as PhoneDevice).UpdateContent(e.NewValue as FrameworkElement);
        }

        private void UpdateContent(FrameworkElement content)
        {
            MainScreenContentPresenter.Content = content;
        }

        public PhoneDevice()
		{
			// Required to initialize variables
			InitializeComponent();
		}

        private void ResetButton_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
        	/*if (MainScreenContentPresenter.Content == null)
				return;
			
			Type ot = MainScreenContentPresenter.Content.GetType();
            Assembly a = Assembly.GetExecutingAssembly();
			object fe = a.CreateInstance(ot.FullName);// as FrameworkElement;
			
			if (fe != null)
				MainScreenContentPresenter.Content = fe;*/
        }
	}
}