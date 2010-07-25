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
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Interactivity;
using System.Diagnostics;
using System.ComponentModel;
using System.Linq;

namespace MetroLibrary
{
    public class KeyPressNavigationBehavior : Behavior<Panel>
    {

        #region Dependency Properties

        enum SearchDirection
        {
            All,
            Up,
            Down,
            Right,
            Left
        }

        public static readonly DependencyProperty MoveUpProperty = DependencyProperty.Register("MoveUp", typeof(Key), typeof(KeyPressNavigationBehavior), new PropertyMetadata(Key.Up));
        [Description("Set key to move up"), Category("Metro Key Navigation Behavior")]
        public Key MoveUp
        {
            get { return (Key)GetValue(MoveUpProperty); }
            set { SetValue(MoveUpProperty, value); }
        }

        public static readonly DependencyProperty MoveDownProperty = DependencyProperty.Register("MoveDown", typeof(Key), typeof(KeyPressNavigationBehavior), new PropertyMetadata(Key.Down));
        [Description("Set key to move down"), Category("Metro Key Navigation Behavior")]
        public Key MoveDown
        {
            get { return (Key)GetValue(MoveDownProperty); }
            set { SetValue(MoveDownProperty, value); }
        }

        public static readonly DependencyProperty MoveRightProperty = DependencyProperty.Register("MoveRight", typeof(Key), typeof(KeyPressNavigationBehavior), new PropertyMetadata(Key.Right));
        [Description("Set key to move right"), Category("Metro Key Navigation Behavior")]
        public Key MoveRight
        {
            get { return (Key)GetValue(MoveRightProperty); }
            set { SetValue(MoveRightProperty, value); }
        }

        public static readonly DependencyProperty MoveLeftProperty = DependencyProperty.Register("MoveLeft", typeof(Key), typeof(KeyPressNavigationBehavior), new PropertyMetadata(Key.Left));
        [Description("Set key to move left"), Category("Metro Key Navigation Behavior")]
        public Key MoveLeft
        {
            get { return (Key)GetValue(MoveLeftProperty); }
            set { SetValue(MoveLeftProperty, value); }
        }

        [Description("Select the Viewer element"), Category("Metro Key Navigation Behavior")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ViewerElementName
        {
            get;
            set;
        }

        [Description("Select the Scrolling element"), Category("Metro Key Navigation Behavior")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ScrollingElementName
        {
            get;
            set;
        }

        public static readonly DependencyProperty NavigationEasingFunctionProperty = DependencyProperty.Register("NavigationEasingFunction", typeof(IEasingFunction), typeof(KeyPressNavigationBehavior), new PropertyMetadata(null));
        [Description("Select the Easing Function for Navigation"), Category("Metro Key Navigation Behavior")]
        public IEasingFunction NavigationEasingFunction
        {
            get { return (IEasingFunction)GetValue(NavigationEasingFunctionProperty); }
            set { SetValue(NavigationEasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register("AnimationDuration", typeof(Duration), typeof(KeyPressNavigationBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(1))));
        [Description("Set the animation duration"), Category("Metro Key Navigation Behavior")]
        public Duration AnimationDuration
        {
            get { return (Duration)GetValue(AnimationDurationProperty); }
            set { SetValue(AnimationDurationProperty, value); }
        }

        #endregion

        private const double _edgeMargin = 10.0;
        private FrameworkElement _ViewerElement = null;
        private FrameworkElement _ScrollElement = null;
        //private bool bFindFocusedElement = true;
        //FrameworkElement _focusedObject = null;

        public KeyPressNavigationBehavior()
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
            AssociatedObject.Loaded += new RoutedEventHandler(AssociatedObject_Loaded);
            AssociatedObject.AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(AssociatedObject_KeyDown), false);
            
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
        }

        
        public void AssociatedObject_Loaded(Object sender, RoutedEventArgs e)
        {
            if (AssociatedObject.Children.Count > 0)
            {
                FrameworkElement fe = AssociatedObject.Children[0] as FrameworkElement;
                if (fe is Control)
                {
                    ((Control)fe).Focus();
                }
            }

            if (ScrollingElementName != null)
            {
                _ScrollElement = AssociatedObject.FindName(ScrollingElementName) as FrameworkElement;
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

       
        private FrameworkElement GetClosestObject (Panel parent, FrameworkElement child, SearchDirection dir)
        {
            GeneralTransform childGt = child.TransformToVisual(AssociatedObject as UIElement);
            Point childRightPt = childGt.Transform (new Point (child.ActualWidth, child.ActualHeight/2));
            Point childLeftPt = childGt.Transform (new Point(0, child.ActualHeight / 2));
            Point childTopPt = childGt.Transform(new Point(child.ActualWidth / 2, 0));
            Point childBottomPt = childGt.Transform(new Point(child.ActualWidth / 2, child.ActualHeight));
            Point childCenterPt = childGt.Transform(new Point(child.ActualWidth / 2, child.ActualHeight / 2));

            FrameworkElement closestObject = null;
            double objectDistance = double.PositiveInfinity;
            foreach (UIElement elem in parent.Children)
            {

                UIElement actualElem = elem;

                if ((actualElem == child) || (actualElem == child.Parent))
                    continue;

                if (actualElem is Panel)
                {
                     actualElem = GetClosestObject(actualElem as Panel, child, dir);
                }

                if ((actualElem == null) || !(actualElem is Control))
                    continue;


                GeneralTransform gt = actualElem.TransformToVisual(AssociatedObject as UIElement);
                Point currPt = new Point(0, 0); //Point currPt = gt.Transform(new Point(((actualElem as FrameworkElement).ActualWidth / 2), (actualElem as FrameworkElement).ActualHeight / 2));

                if (dir == SearchDirection.Right)
                {
                    currPt = gt.Transform(new Point(0, (actualElem as FrameworkElement).ActualHeight / 2));
                    if (currPt.X <= childRightPt.X)
                        continue;
                }
                else if (dir == SearchDirection.Left)
                {
                    currPt = gt.Transform(new Point((actualElem as FrameworkElement).ActualWidth, (actualElem as FrameworkElement).ActualHeight / 2));
                    if (currPt.X >= childLeftPt.X)
                        continue;
                }
                else if (dir == SearchDirection.Up)
                {
                    currPt = gt.Transform(new Point(((actualElem as FrameworkElement).ActualWidth / 2), (actualElem as FrameworkElement).ActualHeight));
                    if (currPt.Y >= childTopPt.Y)
                        continue;
                }
                else if (dir == SearchDirection.Down)
                {
                    currPt = gt.Transform(new Point(((actualElem as FrameworkElement).ActualWidth / 2), 0));
                    if (currPt.Y <= childBottomPt.Y)
                        continue;
                }
                

                double curDistance = Distance(childCenterPt, currPt);
                if (curDistance < objectDistance)
                {
                    closestObject = actualElem as FrameworkElement;
                    objectDistance = curDistance;
                }

            }

            return closestObject;
        }

        public void AssociatedObject_KeyDown(Object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("Key Press: {0}", e.Key);

            FrameworkElement activeObject = FocusManager.GetFocusedElement() as FrameworkElement;
            /*while (AssociatedObject != activeObject.Parent)
            {
                activeObject = activeObject.Parent as FrameworkElement;
            }*/

            if (activeObject == null)
            {
               return;
            }

            FrameworkElement newActiveObject = null;

            switch (e.Key)
            {
                case Key.Right:
                    newActiveObject = GetClosestObject(AssociatedObject, activeObject, SearchDirection.Right);
                    break;
                case Key.Left:
                    newActiveObject = GetClosestObject(AssociatedObject, activeObject, SearchDirection.Left);
                    break;
                case Key.Down:
                    newActiveObject = GetClosestObject(AssociatedObject, activeObject, SearchDirection.Down);
                    break;
                case Key.Up:
                    newActiveObject = GetClosestObject(AssociatedObject, activeObject, SearchDirection.Up);
                    break;
                default:
                    return;
            }
                                   
            if (newActiveObject != null)
            {
                activeObject = newActiveObject;
                if (activeObject is Control)
                {
                    ((Control)activeObject).Focus();
                    SnapToFocusedObject(activeObject.Parent as Panel, activeObject as Control);
                    e.Handled = true;
                    //bFindFocusedElement = false;
                 }
      /*          else if (activeObject is Panel)
                {
                    ///Control co = GetClosestObject(activeObject as Panel, FocusManager.GetFocusedElement() as FrameworkElement, SearchDirection.All) as Control;
                    Control co = null; 
                    switch (e.Key)
                    {
                        case Key.Right:
                            co = GetClosestObject(activeObject as Panel, FocusManager.GetFocusedElement() as FrameworkElement, SearchDirection.Right) as Control;
                            break;
                        case Key.Left:
                            co = GetClosestObject(activeObject as Panel, FocusManager.GetFocusedElement() as FrameworkElement, SearchDirection.Left) as Control;
                            break;
                        case Key.Down:
                            co = GetClosestObject(activeObject as Panel, FocusManager.GetFocusedElement() as FrameworkElement, SearchDirection.Down) as Control;
                            break;
                        case Key.Up:
                            co = GetClosestObject(activeObject as Panel, FocusManager.GetFocusedElement() as FrameworkElement, SearchDirection.Up) as Control;
                            break;
                        default:
                            return;
                    }
                    co.Focus();
                    SnapToFocusedObject(activeObject as Panel, co);
                    //bFindFocusedElement = true;
                }*/
            }
        }

        

        Storyboard _storyboard = null;

        private void SnapToFocusedObject(Panel panel, Control control)
        {
            if ((_ViewerElement == null) || (_ScrollElement == null))
                return;

            GeneralTransform controlGT = control.TransformToVisual(_ViewerElement as UIElement);
            Point controlTopLeft = controlGT.Transform(new Point(0, 0));
            Point controlBottomRight = controlGT.Transform(new Point(control.ActualWidth, control.ActualHeight));

            GeneralTransform panelGT = panel.TransformToVisual(_ViewerElement as UIElement);
            Point panelTopLeft = panelGT.Transform(new Point(0, 0));
            Point panelBottomRight = panelGT.Transform(new Point(panel.ActualWidth, panel.ActualHeight));


            //Debug.WriteLine("Pt1: {0} {1} Pt2: {2} {3}", ptTopLeft.X, ptTopLeft.Y, ptBottomRight.X, ptBottomRight.Y);

            bool bAdjustX = false;
            bool bAdjustY = false;
            double offsetX = 0;
            double offsetY = 0;

            TranslateTransform trans = GetTranslateTransform(_ScrollElement);

            if (controlTopLeft.X < 0)
            {
                offsetX = trans.X - panelBottomRight.X + _ViewerElement.ActualWidth;

                // Center the Panel since it is smaller
                if (panel.ActualWidth < _ViewerElement.ActualWidth)
                {
                    offsetX -= (_ViewerElement.ActualWidth - panel.ActualWidth) / 2;
                }
                else
                    offsetX -= _edgeMargin;

                //If the Control is not whithin the boundaries of the viewport
                //then we center the control on the viewport
                if ((controlTopLeft.X + (offsetX - trans.X)) < 0)
                {
                    offsetX = trans.X + (_ViewerElement.ActualWidth / 2 - controlTopLeft.X - (control.ActualWidth / 2));
                }
                bAdjustX = true;
            }
            else if (controlBottomRight.X > _ViewerElement.ActualWidth)
            {
                offsetX = trans.X - panelTopLeft.X;

                if (panel.ActualWidth < _ViewerElement.ActualWidth)
                {
                    offsetX += (_ViewerElement.ActualWidth - panel.ActualWidth) / 2;
                }
                else
                    offsetX += _edgeMargin;

                //If the Control is not whithin the boundaries of the viewport
                //then we center the control on the viewport
                if ((controlBottomRight.X + (offsetX - trans.X)) > _ViewerElement.ActualWidth)
                {
                    offsetX = trans.X + (_ViewerElement.ActualWidth / 2 - controlBottomRight.X + (control.ActualWidth / 2));
                }
                bAdjustX = true;
            }

            if (controlTopLeft.Y < 0)
            {
                offsetY = trans.Y - panelBottomRight.Y + _ViewerElement.ActualHeight;

                // Center the Panel since it is smaller
                if (panel.ActualHeight < _ViewerElement.ActualHeight)
                {
                    offsetY -= (_ViewerElement.ActualHeight - panel.ActualHeight) / 2;
                }
                else
                    offsetY -= _edgeMargin;

                //If the Control is not whithin the boundaries of the viewport
                //then we center the control on the viewport
                if ((controlTopLeft.Y + (offsetY - trans.Y)) < 0)
                {
                    offsetY = trans.Y + (_ViewerElement.ActualHeight / 2 - controlTopLeft.Y - (control.ActualHeight / 2));
                }
                bAdjustY = true;
            }
            else if (controlBottomRight.Y > _ViewerElement.ActualHeight)
            {
                offsetY = trans.Y - panelTopLeft.Y;

                if (panel.ActualHeight < _ViewerElement.ActualHeight)
                {
                    offsetY += (_ViewerElement.ActualHeight - panel.ActualHeight) / 2;
                }
                else
                    offsetY += _edgeMargin;

                //If the Control is not whithin the boundaries of the viewport
                //then we center the control on the viewport
                if ((controlBottomRight.Y + (offsetY - trans.Y)) > _ViewerElement.ActualHeight)
                {
                    offsetY = trans.Y + (_ViewerElement.ActualHeight / 2 - controlBottomRight.Y + (control.ActualHeight / 2));
                }
                bAdjustY = true;
            }

            /*if (ptTopLeft.X < 0)
            {
                offsetX = trans.X - ptTopLeft.X;
                bAdjustX = true;
            }
            else if (ptBottomRight.X > _ViewerElement.ActualWidth)
            {
                offsetX = trans.X + (_ViewerElement.ActualWidth - ptBottomRight.X);
                bAdjustX = true;
            }

            if (ptTopLeft.Y < 0)
            {
                offsetY = trans.Y - ptTopLeft.Y;
                bAdjustY = true;
            }
            else if (ptBottomRight.Y > _ViewerElement.ActualHeight)
            {
                offsetY = trans.Y + (_ViewerElement.ActualHeight - ptBottomRight.Y);
                bAdjustY = true;
            }*/
                               

            if (_storyboard != null)
                _storyboard.Pause();

            

            if (bAdjustX || bAdjustY)
            {
                _storyboard = new Storyboard();

                if (bAdjustX)
                {
                    DoubleAnimation xAnimation = new DoubleAnimation();
                    xAnimation.Duration = AnimationDuration;
                    xAnimation.To = offsetX;
                    xAnimation.EasingFunction = NavigationEasingFunction;
                    Storyboard.SetTarget(xAnimation, trans);
                    Storyboard.SetTargetProperty(xAnimation, new PropertyPath(TranslateTransform.XProperty));
                    _storyboard.Children.Add(xAnimation);
                }

                if (bAdjustY)
                {
                    DoubleAnimation yAnimation = new DoubleAnimation();
                    yAnimation.Duration = AnimationDuration;
                    yAnimation.To = offsetY;
                    yAnimation.EasingFunction = NavigationEasingFunction;
                    Storyboard.SetTarget(yAnimation, trans);
                    Storyboard.SetTargetProperty(yAnimation, new PropertyPath(TranslateTransform.YProperty));
                    _storyboard.Children.Add(yAnimation);
                }
                _storyboard.Begin();
            }
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
        

        private double Distance(Point pt1, Point pt2)
        {
            return  Math.Sqrt(Math.Pow((pt1.X - pt2.X), 2) + Math.Pow((pt1.Y - pt2.Y), 2));
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