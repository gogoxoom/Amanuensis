using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Linq;
using System.Diagnostics;
using System.ComponentModel;
//using Microsoft.Expression.Interactivity.Core;

namespace MetroLibrary
{
    
    public class ScrollNavigationBehavior : Behavior<Panel>
    {

        #region Dependency Properties



        public static readonly DependencyProperty EnablePanelSnappingProperty = DependencyProperty.Register("EnablePanelSnapping", typeof(bool), typeof(ScrollNavigationBehavior), new PropertyMetadata(true));
        [Description("Enable Snapping on Children boundaries"), Category("Metro Mouse Navigation Behavior Properties")]
        public bool EnablePanelSnapping
        {
            get { return (bool)GetValue(EnablePanelSnappingProperty); }
            set { SetValue(EnablePanelSnappingProperty, value); }
        }

        public static readonly DependencyProperty EnableNavigationVerticalProperty = DependencyProperty.Register("EnableNavigationVertical", typeof(bool), typeof(ScrollNavigationBehavior), new PropertyMetadata(true));
        [Description("Enable Navigation in the Vertical direction"), Category("Metro Mouse Navigation Behavior Properties")]
        public bool EnableNavigationVertical
        {
            get { return (bool)GetValue(EnableNavigationVerticalProperty); }
            set { SetValue(EnableNavigationVerticalProperty, value); }
        }

        public static readonly DependencyProperty EnableNavigationHorizontalProperty = DependencyProperty.Register("EnableNavigationHorizontal", typeof(bool), typeof(ScrollNavigationBehavior), new PropertyMetadata(true));
        [Description("Enable Navigation in the Horizontal direction"), Category("Metro Mouse Navigation Behavior Properties")]
        public bool EnableNavigationHorizontal
        {
            get { return (bool)GetValue(EnableNavigationHorizontalProperty); }
            set { SetValue(EnableNavigationHorizontalProperty, value); }
        }       

        public static readonly DependencyProperty NavigationEasingFunctionProperty = DependencyProperty.Register("NavigationEasingFunction", typeof(IEasingFunction), typeof(ScrollNavigationBehavior), new PropertyMetadata(null));
        [Description("Select the Easing function Navigation"), Category("Metro Mouse Navigation Behavior Properties")]
        public IEasingFunction NavigationEasingFunction
        {
            get { return (IEasingFunction)GetValue(NavigationEasingFunctionProperty); }
            set { SetValue(NavigationEasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty NavigationInertiaProperty = DependencyProperty.Register("NavigationEasingFunction", typeof(double), typeof(ScrollNavigationBehavior), new PropertyMetadata(null));
        [Description("Set a inertia factor for navigation"), Category("Metro Mouse Navigation Behavior Properties")]
        public double NavigationInertia
        {
            get { return (double)GetValue(NavigationInertiaProperty); }
            set { SetValue(NavigationInertiaProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(ScrollNavigationBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(1))));
        [Description("Set the animation duration"), Category("Metro Mouse Navigation Behavior Properties")]
        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        [Description("Select the Viewer element"), Category("Metro Mouse Navigation Behavior Properties")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ViewerElementName
        {
            get;
            set;
        }

        [Description("Select the Scrolling element"), Category("Metro Mouse Navigation Behavior Properties")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ScrollingElementName
        {
            get;
            set;
        }

        public static readonly DependencyProperty MouseTrackingFactorProperty = DependencyProperty.Register("MouseTrackingFactor", typeof(double), typeof(ScrollNavigationBehavior), new PropertyMetadata(1.1));
        [Description("Mouse Tracking multiplier"), Category("Metro Mouse Navigation Behavior Properties")]
        public double MouseTrackingFactor
        {
            get { return (double)GetValue(MouseTrackingFactorProperty); }
            set { SetValue(MouseTrackingFactorProperty, value); }
        }


        #endregion

        private FrameworkElement _ViewerElement = null;
        private FrameworkElement _ScrollElement = null;
        private TranslateTransform _ScrollElementTransform;

        public ScrollNavigationBehavior()
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
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown), true);
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp), true);
            AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
            AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
            AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseEnter);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
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

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {

            if (ScrollingElementName != null)
            {
                _ScrollElement = AssociatedObject.FindName(ScrollingElementName) as FrameworkElement;
                _ScrollElementTransform = GetTranslateTransform(_ScrollElement);
            }

            if (ViewerElementName != null)
            {
                _ViewerElement = AssociatedObject.FindName(ViewerElementName) as FrameworkElement;
            }
            else
            {
                //We iterate thought the visual tree to find which is the smaller container of this object. This becomes the 
                // Viewer.
                Size containerSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
                DependencyObject container = VisualTreeHelper.GetParent (AssociatedObject);
                while (container != null)
                {
                    if (((container as FrameworkElement).ActualWidth <= containerSize.Width) || ((container as FrameworkElement).ActualHeight <= containerSize.Height))
                    {
                        _ViewerElement = container as FrameworkElement;
                        containerSize.Width = (container as FrameworkElement).ActualWidth;
                        containerSize.Height = (container as FrameworkElement).ActualHeight;
                    }
                    container = VisualTreeHelper.GetParent(container);
                }
            }
        }

        int _curChildIdx = 0;
        private Point _prevPoint;
        private Point _deltas = new Point(0, 0);
        bool _MouseLeftButtonIsDown = false;
        Storyboard _storyboard = null;

        // Mouse Handlers
        public void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _prevPoint = e.GetPosition(null);

            _deltas.X = 0;
            _deltas.Y = 0;
            _MouseLeftButtonIsDown = true;

        }


        public void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (_MouseLeftButtonIsDown)
            {
                _MouseLeftButtonIsDown = false;
                if (EnablePanelSnapping)
                    SnapToActiveChild();
                else
                {
                    ScrollBy(new Point ((_deltas.X * NavigationInertia), (_deltas.Y * NavigationInertia)), true);
                }
            }

            _deltas.X = 0;
            _deltas.Y = 0;
        }


        public void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_MouseLeftButtonIsDown == false)
                return;

            if (_storyboard != null)
                _storyboard.Pause();

            Point pt = e.GetPosition(null);

            _deltas.X = (pt.X - _prevPoint.X) * MouseTrackingFactor;
            _deltas.Y = (pt.Y - _prevPoint.Y) * MouseTrackingFactor;
            _prevPoint = pt;
            
            ScrollBy(_deltas, false);

        }

        public void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            _deltas.X = 0;
            _deltas.Y = 0;
        }

        public void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (_MouseLeftButtonIsDown)
            {
                _MouseLeftButtonIsDown = false;
                if (EnablePanelSnapping)
                    SnapToActiveChild();
                else
                {
                    ScrollBy(new Point((_deltas.X * NavigationInertia), (_deltas.Y * NavigationInertia)), true);
                }
            }

            _deltas.X = 0;
            _deltas.Y = 0;
        }

        private void ScrollElement(FrameworkElement elem, TranslateTransform trans, Point deltas, bool checkBounds, bool animated)
        {
            if ((elem == null) || (trans == null) || (_ViewerElement == null))
                return;


            if (_storyboard != null)
                _storyboard.Pause();

            //Now the Animation part
            if (animated)
            {
                _storyboard = new Storyboard();
            }

            double to = 0;
            if ((deltas.X != 0) && EnableNavigationHorizontal)
            {
                double maxOffset = _ViewerElement.ActualWidth / 3;
                to = trans.X + deltas.X;
                if (checkBounds)
                {
                    if (to > maxOffset)
                        to = maxOffset;
                    else if (to < ((maxOffset * 2) - elem.ActualWidth))
                        to = (maxOffset * 2) - elem.ActualWidth;
                }

                if (animated)
                    AddTransformAnimation(_storyboard, to, AnimationDuration, _ScrollElementTransform, new PropertyPath(TranslateTransform.XProperty));
                else
                    trans.X = to;
            }

            if ((deltas.Y != 0) && EnableNavigationVertical)
            {
                double maxOffset = _ViewerElement.ActualHeight / 3;
                to = trans.Y + deltas.Y;
                if (checkBounds)
                {
                    if (to > maxOffset)
                        to = maxOffset;
                    else if (to < ((2 * maxOffset) - elem.ActualHeight))
                        to = (2 * maxOffset) - elem.ActualHeight;
                }


                if (animated)
                    AddTransformAnimation(_storyboard, to, AnimationDuration, _ScrollElementTransform, new PropertyPath(TranslateTransform.YProperty));
                else
                    trans.Y = to;
            }

            if (animated)
                _storyboard.Begin();
        }

        private void ScrollBy(Point deltas, bool animated)
        {
            ScrollElement(_ScrollElement, _ScrollElementTransform, deltas, true, animated);
        }


      

        private void SnapToActiveChild()
        {

            if ((_ScrollElement == null) || !(_ScrollElement is Panel) || (_ViewerElement == null))
                return;

            //Determine which visual element is the active element in the viewport.


            Point centerPoint = new Point(_ViewerElement.ActualWidth / 2, _ViewerElement.ActualHeight / 2);

            int newChildIdx = _curChildIdx;
            int i = 0;
            foreach (UIElement child in ((Panel)_ScrollElement).Children)
            {

                GeneralTransform gt = child.TransformToVisual(_ViewerElement as UIElement);
                Point pointTopLeft = gt.Transform(new Point(0, 0));
                Point pointBottomRight = gt.Transform(new Point(((FrameworkElement)child).ActualWidth, ((FrameworkElement)child).ActualHeight));


                if ((pointTopLeft.X <= centerPoint.X) && (pointBottomRight.X >= centerPoint.X) && 
                    (pointTopLeft.Y <= centerPoint.Y) && (pointBottomRight.Y >= centerPoint.Y))
                {
                    //We found the child that is in the viewport.
                    newChildIdx = i;
                    break;
                }
                
                i++;
            }

            _curChildIdx = newChildIdx;

            Point offsets;
            offsets = GetPanelScrollOffset(newChildIdx, _ScrollElement as Panel, _ScrollElementTransform, _ViewerElement, _deltas);

            if (_storyboard != null)
                _storyboard.Pause();


            //Now the Animation part
            _storyboard = new Storyboard();

            if (EnableNavigationHorizontal)
                AddTransformAnimation(_storyboard, offsets.X, AnimationDuration, _ScrollElementTransform, new PropertyPath(TranslateTransform.XProperty));
            if (EnableNavigationVertical)
                AddTransformAnimation(_storyboard, offsets.Y, AnimationDuration, _ScrollElementTransform, new PropertyPath(TranslateTransform.YProperty));
            
 
            _storyboard.Begin();

        }

        private void AddTransformAnimation(Storyboard sb, double position, Duration duration, TranslateTransform trans, PropertyPath path)
        {
            if (trans == null)
                return;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = duration;
            animation.To = position;
            //animation.FillBehavior = FillBehavior.
            //animation.By = 10;
            animation.AutoReverse = false;
            animation.EasingFunction = NavigationEasingFunction;


            Storyboard.SetTarget(animation, trans);
            Storyboard.SetTargetProperty(animation, path);
            sb.Children.Add(animation);

        }


        private Point GetPanelScrollOffset(int idx, Panel panel, TranslateTransform panelTransform, FrameworkElement viewer, Point motionDeltas)
        {
            //Maybe we can add some inertia using motionDelta.

            Point offsets = new Point (0,0);
            FrameworkElement fe = panel.Children[idx] as FrameworkElement;
            Point centerPoint = new Point(viewer.ActualWidth / 2, viewer.ActualHeight / 2);
            GeneralTransform gt = fe.TransformToVisual(viewer as UIElement);
            Point viewerTopLeft = gt.Transform(new Point(0, 0));
            Point viewerBottomRight = gt.Transform(new Point(fe.ActualWidth, fe.ActualHeight));

            gt = fe.TransformToVisual(panel as UIElement);
            Point panelTopLeft = gt.Transform(new Point(0, 0));
            Point panelBottomRight = gt.Transform(new Point(fe.ActualWidth, fe.ActualHeight));


            //Calculate the X offsetl
            if (fe.ActualWidth <= _ViewerElement.ActualWidth)
            {
                offsets.X = ((_ViewerElement.ActualWidth - fe.ActualWidth) / 2) - panelTopLeft.X;
            }
            else if ((viewerTopLeft.X >= 0) && (viewerTopLeft.X < viewer.ActualWidth))
            {
                offsets.X = -1 * panelTopLeft.X;
            }
            else if ((viewerBottomRight.X >= 0) && (viewerBottomRight.X < viewer.ActualWidth))
            {
                offsets.X = viewer.ActualWidth - panelBottomRight.X;
            }
            else
            {
                offsets.X = panelTransform.X + (NavigationInertia * motionDeltas.X);
                if ((Math.Sign(motionDeltas.X) < 0) && (offsets.X < (viewer.ActualWidth - panelBottomRight.X)))
                {
                    offsets.X = viewer.ActualWidth - panelBottomRight.X;
                }
                else if ((Math.Sign(motionDeltas.X) > 0) && (offsets.X > (-1 * panelTopLeft.X)))
                {
                    offsets.X = -1 * panelTopLeft.X;
                }
            }

            //Calculate the Y offset
            if (fe.ActualHeight <= _ViewerElement.ActualHeight)
            {
                offsets.Y = ((_ViewerElement.ActualHeight - fe.ActualHeight) / 2) - panelTopLeft.Y;
            }
            else if ((viewerTopLeft.Y >= 0) && (viewerTopLeft.Y < viewer.ActualHeight))
            {
                offsets.Y = -1 * panelTopLeft.Y;
            }
            else if ((viewerBottomRight.Y >= 0) && (viewerBottomRight.Y < viewer.ActualHeight))
            {
                offsets.Y = viewer.ActualHeight - panelBottomRight.Y;
            }
            else
            {
                offsets.Y = panelTransform.Y + (NavigationInertia * motionDeltas.Y);
                if ((Math.Sign(motionDeltas.Y) < 0) && (offsets.Y < (viewer.ActualHeight - panelBottomRight.Y)))
                {
                    offsets.Y = viewer.ActualHeight - panelBottomRight.Y;
                }
                else if ((Math.Sign(motionDeltas.Y) > 0) && (offsets.Y > (-1 * panelTopLeft.Y)))
                {
                    offsets.Y = -1 * panelTopLeft.Y;
                }
            }


            return offsets;
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