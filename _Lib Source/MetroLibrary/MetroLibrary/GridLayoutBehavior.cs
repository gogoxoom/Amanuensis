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
using System.Diagnostics;
using System.ComponentModel;
//using Microsoft.Expression.Interactivity.Core;

namespace MetroLibrary
{
    public class GridLayoutBehavior : Behavior<Canvas>
    {

        public static readonly DependencyProperty ChildrenMarginProperty = DependencyProperty.Register("ChildrenMargin", typeof(Thickness), typeof(GridLayoutBehavior), new PropertyMetadata(new Thickness(10)));
        [Description("Set the Marging for the children"), Category("Metro Grid Layout Behavior Properties")]
        public Thickness ChildrenMargin
        {
            get { return (Thickness)GetValue(ChildrenMarginProperty); }
            set { SetValue(ChildrenMarginProperty, value); }
        }

        public GridLayoutBehavior()
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
            //Debug.WriteLine("GridLayoutBehavior () called");
        }

        protected override void OnAttached()
        {
            //Debug.WriteLine("OnAttached () called");

            base.OnAttached();

            AssociatedObject.LayoutUpdated += new EventHandler(AssociatedObject_LayoutUpdated);
        }
               
        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
        }

        void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
        {
            GridLayout();
        }

        void RandomLayout()
        {
            Random randomNumber = new Random();
            foreach (FrameworkElement element in AssociatedObject.Children)
            {
                double xPos = randomNumber.NextDouble();
                double yPos = randomNumber.NextDouble();

                Canvas.SetLeft(element, (xPos * 100));
                Canvas.SetTop(element, (yPos * 100));
            }
        }

        void GridLayout()
        {
            
            double maxHeight = 0.0;
            double curX = ChildrenMargin.Left;
            double curY = ChildrenMargin.Top;

            foreach (FrameworkElement child in AssociatedObject.Children)
            {
                if ((curX > ChildrenMargin.Left) && ((curX + child.ActualWidth) > AssociatedObject.ActualWidth))
                {
                    curX = ChildrenMargin.Left;
                    curY += maxHeight + ChildrenMargin.Top + ChildrenMargin.Bottom;
                    maxHeight = 0;
                }
                                
                Canvas.SetLeft(child, curX);
                Canvas.SetTop(child, curY);

                curX += ChildrenMargin.Left + ChildrenMargin.Right;
                
                maxHeight = Math.Max(maxHeight, child.ActualHeight);

                curX += child.ActualWidth;
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