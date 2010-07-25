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
//using Microsoft.Expression.Interactivity.Core;

namespace MetroLibrary
{
    public class StackLayoutBehavior : Behavior<Canvas>
    {


        public static readonly DependencyProperty OriginProperty = DependencyProperty.Register("Origin", typeof(Point), typeof(StackLayoutBehavior), new PropertyMetadata(new Point(0, 0)));
        [Description("Set the Origin of Stck"), Category("Metro Stack Layout Behavior Properties")]
        public Point Origin
        {
            get { return (Point)GetValue(OriginProperty); }
            set { SetValue(OriginProperty, value); }
        }

        public static readonly DependencyProperty StackOffsetProperty = DependencyProperty.Register("StackOffset", typeof(Point), typeof(StackLayoutBehavior), new PropertyMetadata(new Point(20, 20)));
        [Description("Set the Stack Offsets"), Category("Metro Stack Layout Behavior Properties")]
        public Point StackOffset
        {
            get { return (Point)GetValue(StackOffsetProperty); }
            set { SetValue(StackOffsetProperty, value); }
        }

   

        public StackLayoutBehavior()
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

            // Insert code that you would want run when the Behavior is attached to an object.
            AssociatedObject.LayoutUpdated += new EventHandler(AssociatedObject_LayoutUpdated);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
        }

        void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
        {
            StackLayout();
        }

   
        void StackLayout()
        {
            double curX = Origin.X;
            double curY = Origin.Y;
            int zIndex = -1;
            double scaleFactor = 1;
            foreach (FrameworkElement child in AssociatedObject.Children)
            {
                Canvas.SetLeft(child, curX);
                Canvas.SetTop(child, curY);
                Canvas.SetZIndex(child, zIndex);

                child.RenderTransform = new ScaleTransform();
                ((ScaleTransform)child.RenderTransform).ScaleX = scaleFactor;
                ((ScaleTransform)child.RenderTransform).ScaleY = scaleFactor;
                                
                curX += StackOffset.X;
                curY += StackOffset.Y;
                zIndex--;
                scaleFactor -= 0.1;
                
            }
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