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
using System.Windows.Interactivity;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Diagnostics;
//using Microsoft.Expression.Interactivity.Core;

namespace MetroLibrary
{
    public class CustomCursorBehavior : Behavior<Panel>
    {

        #region Dependency Properties

        [Description("Select the Curson element"), Category("Metro Custom Cursor Behavior")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string CursorElementName
        {
            get;
            set;
        }

        #endregion

        private FrameworkElement _customCursor = null;
        private TranslateTransform _transTransform = null;
        public CustomCursorBehavior()
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

        private TranslateTransform GetTranslateTransform(FrameworkElement fe)
        {
            if (fe == null)
                return null;

            TranslateTransform transTransform = null;

            if (fe.RenderTransform == null)
            {
                fe.RenderTransform = new TranslateTransform();
                transTransform = fe.RenderTransform as TranslateTransform;
            }
            else if (fe.RenderTransform is TranslateTransform)
            {
                transTransform = fe.RenderTransform as TranslateTransform;
            }
            else if (fe.RenderTransform is TransformGroup)
            {
                TransformGroup tg = fe.RenderTransform as TransformGroup;

                transTransform = (from t in tg.Children
                                  where t is TranslateTransform
                                  select (TranslateTransform)t).FirstOrDefault();

                if (transTransform == null)
                {
                    transTransform = new TranslateTransform();
                    tg.Children.Add(transTransform);
                }
            }
            else
            {
                TransformGroup tg = new TransformGroup();
                var curTrans = fe.RenderTransform;
                fe.RenderTransform = null;
                tg.Children.Add(curTrans);
                transTransform = new TranslateTransform();
                tg.Children.Add(transTransform);
                fe.RenderTransform = tg;
            }

            return transTransform;
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

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            if (CursorElementName != null)
            {
                _customCursor = AssociatedObject.FindName(CursorElementName) as FrameworkElement;
                _transTransform = GetTranslateTransform(_customCursor);
                
            }

            if (_transTransform != null)
            {
                AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
                AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
                AssociatedObject.MouseEnter += new MouseEventHandler(AssociatedObject_MouseEnter);

                AssociatedObject.Cursor = Cursors.None;
                _customCursor.IsHitTestVisible = false;

            }
        }

        public void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _customCursor.Opacity = 100;
        }

        private void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
             _customCursor.Opacity = 0;
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Point pos = e.GetPosition(null);
            //The + 5 prevents that we trigger the MouseLeave event when the mouse moves over the CustomCursor.
            _transTransform.X = pos.X - (_customCursor.ActualWidth / 2);
            _transTransform.Y = pos.Y - (_customCursor.ActualHeight / 2);
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