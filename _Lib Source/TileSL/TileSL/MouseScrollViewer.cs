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
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace TileSL
{
    public class MouseScrollViewer : Behavior<DependencyObject>
    {
        private Point _prevPoint;
        private bool _bAnimate;
        private int _viewportWidth = 0;

        Storyboard _storyboard;
        DoubleAnimation _animation;

        public MouseScrollViewer()
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

            /*if (AssociatedObject is ScrollViewer)
                _scrollViewer = AssociatedObject as ScrollViewer;
            else
                _scrollViewer = FindVisualChild<ScrollViewer> (AssociatedObject);
            
            if (_scrollViewer != null)
            {
                
                _scrollViewer.AddHandler(UIElement.MouseLeftButtonDownEvent, new (Object_MouseLeftButtonDown), true);
                _scrollViewer.AddHandler(UIElement.MouseLeftButtonDownEvent, new (Object_MouseLeftButtonUp), true);

                //_scrollViewer.MouseLeftButtonDown += new MouseButtonEventHandler(Object_MouseLeftButtonDown);
                //_scrollViewer.MouseLeftButtonUp += new MouseButtonEventHandler(Object_MouseLeftButtonUp);
                _scrollViewer.MouseMove += new MouseEventHandler(Object_MouseMove);
            }*/


            FrameworkElement fe = AssociatedObject as FrameworkElement;
            fe.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(Object_MouseLeftButtonDown), true);
            fe.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(Object_MouseLeftButtonUp), true);
            //fe.MouseLeftButtonDown +=  new MouseButtonEventHandler (Object_MouseLeftButtonDown);
            //fe.MouseLeftButtonUp +=  new MouseButtonEventHandler (Object_MouseLeftButtonUp);
            fe.MouseMove += new MouseEventHandler(Object_MouseMove);
            fe.MouseLeave += new MouseEventHandler(Object_MouseLeave);


            _viewportWidth = 1280;
        }

        protected override void OnDetaching()
        {
            /*if (_scrollViewer != null)
            {
                _scrollViewer.MouseLeftButtonDown -= Object_MouseLeftButtonDown;
                _scrollViewer.MouseLeftButtonUp -= Object_MouseLeftButtonUp;
                _scrollViewer.MouseMove -= Object_MouseMove;
            }
            _scrollViewer = null;*/
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
        }


        public static childItem FindVisualChild<childItem>(DependencyObject obj)
           where childItem : DependencyObject
        {
            // Search immediate children first (breadth-first)
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child != null && child is childItem)
                    return (childItem)child;

                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);

                    if (childOfChild != null)
                        return childOfChild;
                }
            }

            return null;
        }


        private void CreateAnimation()
        {
            _storyboard = new Storyboard();
            _animation = new DoubleAnimation()
            {
                Duration = new TimeSpan(0, 0, 0, 10, 0)
            };

            // Add the animation to the storyboard.
            _storyboard.Children.Add(_animation);
        }

        private void ScrollWithAnimation(double delta)
        {

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.By = 20 * delta;
            animation.AutoReverse = false;

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            FrameworkElement fe = AssociatedObject as FrameworkElement;

            // Create the TranslateTransform
            if ((fe.RenderTransform == null) || (fe.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                fe.RenderTransform = new TranslateTransform();
            }

            TranslateTransform transTransform = fe.RenderTransform as TranslateTransform;


            Storyboard.SetTarget(animation, transTransform);

            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));

            storyboard.Begin();

        }

        private void ScrollWithAnimation2(double delta)
        {
            FrameworkElement fe = AssociatedObject as FrameworkElement;
            ScrollViewer sv = fe.Parent as ScrollViewer;

            if ((fe.RenderTransform == null) || (fe.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                fe.RenderTransform = new TranslateTransform();
            }

            TranslateTransform transTransform = fe.RenderTransform as TranslateTransform;
            
            double toX = transTransform.X + (10 * delta);


            if (toX > 0)
                toX = 0;
            else if (toX < (0 - (fe.Width - sv.ViewportWidth)))
                toX = (0 - (fe.Width - sv.ViewportWidth));
            

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            animation.To = toX;
            animation.AutoReverse = false;

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);
                        
            Storyboard.SetTarget(animation, transTransform);

            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));

            storyboard.Begin();

        }

        private void ScrollWithAnimation3(double delta)
        {
            FrameworkElement fe = AssociatedObject as FrameworkElement;

            if ((fe.RenderTransform == null) || (fe.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                fe.RenderTransform = new TranslateTransform();
            }

            TranslateTransform transTransform = fe.RenderTransform as TranslateTransform;

            double toX = transTransform.X + (1 * delta);


            if (toX > 0)
                toX = 0;
            else if (toX < (0 - (fe.Width - _viewportWidth)))
                toX = (0 - (fe.Width - _viewportWidth));


            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(0));
            animation.To = toX;
            animation.AutoReverse = false;

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            Storyboard.SetTarget(animation, transTransform);

            Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));

            storyboard.Begin();

        }


        private void Object_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (_bAnimate == false)
                return;

            Point pt = e.GetPosition(null);
            //Debug.WriteLine("Point {0}, {1}", pt.X, pt.Y);

            Double delta = pt.X - _prevPoint.X;
            ScrollWithAnimation3(delta);
            //ScrollTo(pt.X);
            _prevPoint = pt;

        }

        private void Object_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            _bAnimate = false;
        }

        private void Object_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //MessageBox.Show ("MouseLeftButtonDown");

            _prevPoint = e.GetPosition(null);

            FrameworkElement fe = AssociatedObject as FrameworkElement;
            //fe.CaptureMouse();
            _bAnimate = true;
        }

        private void Object_MouseLeftButtonUp(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //_scrollViewer = null;
            _bAnimate = false;
            FrameworkElement fe = AssociatedObject as FrameworkElement;

            //fe.ReleaseMouseCapture();
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
