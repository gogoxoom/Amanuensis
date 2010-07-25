
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
using System.Windows.Interactivity;
using System.ComponentModel.Composition;
using SessionController;

//using Microsoft.Expression.Interactivity.Core;

namespace SessionController
{
    public class LogBehavior : Behavior<FrameworkElement>
    {
        [Import]
        public IEvent Event { get; set; }
		
        #region Dependency Properties

        /*[Description("Select the Log Element"), Category("Metro Log Behavior")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string LogElementName
        {
            get;
            set;
        }*/
		
		
		public static readonly DependencyProperty LogMouseLeftButtonDownProperty = DependencyProperty.Register("LogMouseLeftButtonDown", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log MouseLeftButtonDown Event"), Category("Metro Log Behavior")]
        public bool LogMouseLeftButtonDown
        {
            get { return (bool)GetValue(LogMouseLeftButtonDownProperty); }
            set { SetValue(LogMouseLeftButtonDownProperty, value); }
        }
		
		public static readonly DependencyProperty LogMouseLeftButtonUpProperty = DependencyProperty.Register("LogMouseLeftButtonUp", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log MouseLeftButtonUp Event"), Category("Metro Log Behavior")]
        public bool LogMouseLeftButtonUp
        {
            get { return (bool)GetValue(LogMouseLeftButtonUpProperty); }
            set { SetValue(LogMouseLeftButtonUpProperty, value); }
        }
		
		public static readonly DependencyProperty LogMouseEnterProperty = DependencyProperty.Register("LogMouseEnter", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log MouseEnter Event"), Category("Metro Log Behavior")]
        public bool LogMouseEnter
        {
            get { return (bool)GetValue(LogMouseEnterProperty); }
            set { SetValue(LogMouseEnterProperty, value); }
        }
		
		public static readonly DependencyProperty LogMouseLeaveProperty = DependencyProperty.Register("LogMouseLeave", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log MouseLeave Event"), Category("Metro Log Behavior")]
        public bool LogMouseLeave
        {
            get { return (bool)GetValue(LogMouseLeaveProperty); }
            set { SetValue(LogMouseLeaveProperty, value); }
        }
				
		public static readonly DependencyProperty LogMouseMoveProperty = DependencyProperty.Register("LogMouseMove", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log MouseMove Event"), Category("Metro Log Behavior")]
        public bool LogMouseMove
        {
            get { return (bool)GetValue(LogMouseMoveProperty); }
            set { SetValue(LogMouseMoveProperty, value); }
        }

        public static readonly DependencyProperty LogKeyDownProperty = DependencyProperty.Register("LogKeyDown", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log KeyDown Event"), Category("Metro Log Behavior")]
        public bool LogKeyDown {
            get { return (bool)GetValue(LogKeyDownProperty); }
            set { SetValue(LogKeyDownProperty, value); }
        }

        public static readonly DependencyProperty LogKeyUpProperty = DependencyProperty.Register("LogKeyUp", typeof(bool), typeof(LogBehavior), new PropertyMetadata(false));
        [Description("Log KeyUp Event"), Category("Metro Log Behavior")]
        public bool LogKeyUp {
            get { return (bool)GetValue(LogKeyUpProperty); }
            set { SetValue(LogKeyUpProperty, value); }
        }
		

		#endregion

        public LogBehavior()
        {
            // Insert code required on object creation below this point.

            //
            // The line of code below sets up the relationship between the command and the function
            // to call. Uncomment the below line and add a reference to Microsoft.Expression.Interactions
            // if you choose to use the commented out version of MyFunction and MyCommand instead of
            // creating your own implementation.
            //
            // The documentation will provide you with an example of a simple command implementation
            // you can use instead of using ActionCommand and referencing the Interactions assembly.
            //
            //this.MyCommand = new ActionCommand(this.MyFunction);
        }

       protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
            
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }
		
		public void testMethod()
		{
		}

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {

            CompositionInitializer.SatisfyImports(this);    
            if (LogMouseLeftButtonDown)
			{
     			AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown), true);
            }
			
			if (LogMouseLeftButtonUp)
			{
     			AssociatedObject.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp), true);
            }
			
			if (LogMouseMove)
			{
				AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
			}
			
			if (LogMouseLeave)
			{
				AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
			}
			
			if (LogMouseEnter)
			{
				AssociatedObject.MouseEnter += new MouseEventHandler(AssociatedObject_MouseEnter);
			}

            if (LogKeyDown) {
                AssociatedObject.KeyDown += new KeyEventHandler(AssociatedObject_KeyDown);
            }

            if (LogKeyUp) {
                AssociatedObject.KeyUp += new KeyEventHandler(AssociatedObject_KeyUp);
            }
                
        }
		
		
		public void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Event != null)
                Event.RecordEvent("MouseButtonEvent", "MouseLeftButtonDown", e.GetPosition(null).ToString() );
        }
		
		private void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Event != null)
                Event.RecordEvent("MouseButtonEvent", "MouseLeftButtonUp", e.GetPosition(null).ToString() );
        }

        public void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Event != null)
                Event.RecordEvent("MouseEvent", "MouseEnter", e.GetPosition(null).ToString() );
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Event != null)
                Event.RecordEvent("MouseEvent", "MouseLeave", e.GetPosition(null).ToString() );
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Event != null)
			    Event.RecordEvent("MouseEvent", "MouseMove", e.GetPosition(null).ToString() );            
        }

        private void AssociatedObject_KeyUp(object sender, System.Windows.Input.KeyEventArgs e) {
            if (Event != null)
                Event.RecordEvent("KeyEvent", "KeyUp", e.Key.ToString());
        }

        private void AssociatedObject_KeyDown(object sender, System.Windows.Input.KeyEventArgs e) {
            if (Event != null)
                Event.RecordEvent("KeyEvent", "KeyDown", e.Key.ToString());
        }

        /*
        public ICommand MyCommand
        {
            get;
            private set;
        }
		 
        private void MyFunction()
        {
            // Insert code that defines what the behavior will do when invoked.
        }
        */
    }
}