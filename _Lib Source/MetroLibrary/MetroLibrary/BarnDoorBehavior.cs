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
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
//using Microsoft.Expression.Interactivity.Core;

namespace MetroLibrary
{

    [DefaultTrigger(typeof(FrameworkElement), typeof(System.Windows.Interactivity.EventTrigger), "MouseLeftButtonUp")]
    public class BarnDoorBehavior : TargetedTriggerAction<FrameworkElement>
    {
        #region Dependency Properties

        public static readonly DependencyProperty OutboundAnimationEasingFunctionProperty = DependencyProperty.Register("OutboundAnimationEasingFunction", typeof(IEasingFunction), typeof(BarnDoorBehavior), new PropertyMetadata(null));
        [Description("Select the easing function"), Category("Metro Barn Door Behavior Outboud")]
        public IEasingFunction OutboundAnimationEasingFunction
        {
            get { return (IEasingFunction)GetValue(OutboundAnimationEasingFunctionProperty); }
            set { SetValue(OutboundAnimationEasingFunctionProperty, value); }
        }


        public static readonly DependencyProperty OutboundAnimationDurationProperty = DependencyProperty.Register("OutboundAnimationDuration", typeof(Duration), typeof(BarnDoorBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2))));
        [Description("Set the animation duration"), Category("Metro Barn Door Behavior Outboud")]
        public Duration OutboundAnimationDuration
        {
            get { return (Duration)GetValue(OutboundAnimationDurationProperty); }
            set { SetValue(OutboundAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty DistanceTimeScalerProperty = DependencyProperty.Register("DistanceTimeScaler", typeof(double), typeof(BarnDoorBehavior), new PropertyMetadata(4.0));
        [Description("Set the DistanceTime scaler"), Category("Metro Barn Door Behavior Outboud")]
        public double DistanceTimeScaler
        {
            get { return (double)GetValue(DistanceTimeScalerProperty); }
            set { SetValue(DistanceTimeScalerProperty, value); }
        }
  
        public static readonly DependencyProperty BarnDoorAxisProperty = DependencyProperty.Register("BarnDoorAxis", typeof(double), typeof(BarnDoorBehavior), new PropertyMetadata(-1.5));
        [Description("Set the Axis around which the Barn Door effect will take place"), Category("Metro Barn Door Behavior Outboud")]
        public double BarnDoorAxis
        {
            get { return (double)GetValue(BarnDoorAxisProperty); }
            set { SetValue(BarnDoorAxisProperty, value); }
        }

        public static readonly DependencyProperty BarnDoorOrientationProperty = DependencyProperty.Register("BarnDoorOrientation", typeof(Orientation), typeof(ParallaxBehavior), new PropertyMetadata(Orientation.Horizontal));
        [Description("Sets the Orientation of the Bard Door effect"), Category("Metro Barn Door Behavior Outboud")]
        public Orientation BarnDoorOrientation
        {
            get { return (Orientation)GetValue(BarnDoorOrientationProperty); }
            set { SetValue(BarnDoorOrientationProperty, value); }
        }

        public static readonly DependencyProperty BarnDoorAngleProperty = DependencyProperty.Register("BarnDoorAngle", typeof(double), typeof(BarnDoorBehavior), new PropertyMetadata(100.0));
        [Description("Set Angle that the Barn Door will swing"), Category("Metro Barn Door Behavior Outboud")]
        public double BarnDoorAngle
        {
            get { return (double)GetValue(BarnDoorAngleProperty); }
            set { SetValue(BarnDoorAngleProperty, value); }
        }


        //Inbound Dependency properties
        public enum InboundAnimations
        {
            FlowInFromRight = 0,
            FlowInFromLeft,
            FlowInFromTop,
            FlowInFromBottom
        };

        [Description("Select the element to animate in"), Category("Metro Barn Door Behavior Inbound")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string InbountElementName
        {
            get;
            set;
        }

        [Description("Select the Viewer element"), Category("Metro Barn Door Behavior Inbound")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ViewerElementName
        {
            get;
            set;
        }

        public static readonly DependencyProperty InboundAnimationProperty = DependencyProperty.Register("InboundAnimation", typeof(InboundAnimations), typeof(BarnDoorBehavior), new PropertyMetadata(InboundAnimations.FlowInFromRight));
        [Description("Select the easing function"), Category("Metro Barn Door Behavior Inbound")]
        public InboundAnimations InboundAnimation
        {
            get { return (InboundAnimations)GetValue(InboundAnimationProperty); }
            set { SetValue(InboundAnimationProperty, value); }
        }


        public static readonly DependencyProperty InboundAnimationEasingFunctionProperty = DependencyProperty.Register("InboundAnimationEasingFunction", typeof(IEasingFunction), typeof(BarnDoorBehavior), new PropertyMetadata(null));
        [Description("Select the easing function"), Category("Metro Barn Door Behavior Inbound")]
        public IEasingFunction InboundAnimationEasingFunction
        {
            get { return (IEasingFunction)GetValue(InboundAnimationEasingFunctionProperty); }
            set { SetValue(InboundAnimationEasingFunctionProperty, value); }
        }


        public static readonly DependencyProperty InboundAnimationDelayProperty = DependencyProperty.Register("InboundAnimationDelay", typeof(TimeSpan), typeof(BarnDoorBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0)));
        [Description("Set the animation delay"), Category("Metro Barn Door Behavior Inbound")]
        public TimeSpan InboundAnimationDelay
        {
            get { return (TimeSpan)GetValue(InboundAnimationDelayProperty); }
            set { SetValue(InboundAnimationDelayProperty, value); }
        }

        public static readonly DependencyProperty InboundAnimationDurationProperty = DependencyProperty.Register("InboundAnimationDuration", typeof(Duration), typeof(BarnDoorBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2))));
        [Description("Set the animation duration"), Category("Metro Barn Door Behavior Inbound")]
        public Duration InboundAnimationDuration
        {
            get { return (Duration)GetValue(InboundAnimationDurationProperty); }
            set { SetValue(InboundAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty WaitForOutbounAnimationCompletionProperty = DependencyProperty.Register("WaitForOutbounAnimationCompletion", typeof(bool), typeof(BarnDoorBehavior), new PropertyMetadata(true));
        [Description("Set whether the Outbound Animation should complete before the Inbound Animation starts"), Category("Metro Barn Door Behavior Inbound")]
        public bool WaitForOutbounAnimationCompletion
        {
            get { return (bool)GetValue(WaitForOutbounAnimationCompletionProperty); }
            set { SetValue(WaitForOutbounAnimationCompletionProperty, value); }
        }

        #endregion

        
        public BarnDoorBehavior()
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

            ((FrameworkElement)AssociatedObject).AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown), true);
            ((FrameworkElement)AssociatedObject).AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp), true);
        }


        Point _mouseDownPoint = new Point(0, 0);
        Point _mouseUpPoint = new Point(0, 0);

        private void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouseDownPoint = e.GetPosition(null);
        }

        private void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouseUpPoint = e.GetPosition(null);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
        }

        private Storyboard _barndoorStoryboard;
        private Storyboard _inboundStoryboard;

        protected override void Invoke(object parameter)
        {

            if ((Math.Abs(_mouseDownPoint.X - _mouseUpPoint.X) > 2) || (Math.Abs(_mouseDownPoint.Y - _mouseUpPoint.Y) > 2))
            {
                return;
            }

            if (TargetObject == null)
                 return;

            _barndoorStoryboard = new Storyboard();

            FrameworkElement assocElem = AssociatedObject as FrameworkElement;
            FrameworkElement parent = VisualTreeHelper.GetParent(assocElem) as FrameworkElement;
            GeneralTransform gt = assocElem.TransformToVisual(parent);
            Point center = gt.Transform(new Point(assocElem.ActualWidth / 2, assocElem.ActualHeight / 2));

            if (TargetObject is Panel)
            {
                bool bUseCallback = true;
                foreach (UIElement child in (TargetObject as Panel).Children)
                {
                    FrameworkElement fe = child as FrameworkElement;
                    if (fe == null)
                        continue;

                    AddBarnDoorAnimation(_barndoorStoryboard, center, fe, parent as FrameworkElement, bUseCallback);
                    bUseCallback = false;
                }
            }
            else
            {
                AddBarnDoorAnimation(_barndoorStoryboard, center, AssociatedObject as FrameworkElement, AssociatedObject as FrameworkElement, true);
            }

            _barndoorStoryboard.Begin();

            if (!WaitForOutbounAnimationCompletion)
                RunInboundAnimation();

        }

        //Add Barn Door animation based on the proximity of the sibling to the associated object.
        private void AddBarnDoorAnimation (Storyboard sb, Point center,  FrameworkElement sibling, FrameworkElement parent, bool bUseCallback)
        {
            GeneralTransform gt = sibling.TransformToVisual(parent);
            Point siblingCenter = gt.Transform(new Point(sibling.ActualWidth / 2, sibling.ActualHeight / 2));

            double Distance = Math.Sqrt(Math.Pow((center.X - siblingCenter.X), 2) + Math.Pow((center.Y - siblingCenter.Y), 2));
            DoubleAnimation animation = new DoubleAnimation();
            animation.From = 0;
            animation.To = BarnDoorAngle;
            animation.EasingFunction = OutboundAnimationEasingFunction;

            if (bUseCallback)
            {
                animation.Completed += new EventHandler(BarnDoorAnimation_Completed);
            }

            TimeSpan timeAdjustment = TimeSpan.FromMilliseconds (DistanceTimeScaler * Distance);
            if (timeAdjustment < OutboundAnimationDuration)
            {
                animation.Duration = OutboundAnimationDuration.Subtract(timeAdjustment);
            }
            else
            {
                animation.Duration = TimeSpan.FromMilliseconds(100);
            }
            
          
            if (sibling.Projection == null)
            {
                sibling.Projection = new PlaneProjection();
            }

            if (BarnDoorOrientation == Orientation.Horizontal)
                sibling.Projection.SetValue(PlaneProjection.CenterOfRotationXProperty, BarnDoorAxis);
            else
                sibling.Projection.SetValue(PlaneProjection.CenterOfRotationYProperty, BarnDoorAxis);
            

            _barndoorStoryboard.Children.Add(animation);

            Storyboard.SetTarget(animation, sibling.Projection);
            if (BarnDoorOrientation == Orientation.Horizontal)
                Storyboard.SetTargetProperty(animation, new PropertyPath(PlaneProjection.RotationYProperty));
            else
                Storyboard.SetTargetProperty(animation, new PropertyPath(PlaneProjection.RotationXProperty));
         
        }
        

        private void BarnDoorAnimation_Completed(object sender, EventArgs e)
        {
            if (WaitForOutbounAnimationCompletion)
                RunInboundAnimation();
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

        private FrameworkElement FindViewerElement ()
        {
            FrameworkElement ViewerElement = null;
            if (ViewerElementName != null)
            {
                ViewerElement = (AssociatedObject as FrameworkElement).FindName(ViewerElementName) as FrameworkElement;
            }
            else
            {
                //We iterate thought the visual tree to find which is the smaller container of this object. This becomes the 
                // Viewer.
                Size containerSize = new Size(Double.PositiveInfinity, Double.PositiveInfinity);
                DependencyObject container = VisualTreeHelper.GetParent(AssociatedObject);
                while (container != null)
                {
                    if (((container as FrameworkElement).ActualWidth <= containerSize.Width) || ((container as FrameworkElement).ActualHeight <= containerSize.Height))
                    {
                        ViewerElement = container as FrameworkElement;
                        containerSize.Width = (container as FrameworkElement).ActualWidth;
                        containerSize.Height = (container as FrameworkElement).ActualHeight;
                    }
                    container = VisualTreeHelper.GetParent(container);
                }
            }

            return ViewerElement;
        }

        private void SideAnimation_Completed(object sender, EventArgs e)
        {
            _barndoorStoryboard.Stop();
        }
        
        private void RunInboundAnimation()
        {
            FrameworkElement inFe = (AssociatedObject as FrameworkElement).FindName(InbountElementName) as FrameworkElement;
            if (inFe == null)
                return;

            FrameworkElement outFe = TargetObject as FrameworkElement;
            if (outFe == null)
                return;

            FrameworkElement ve = FindViewerElement ();
            if (ve == null)
                return;

            

            GeneralTransform gt = ve.TransformToVisual(Application.Current.RootVisual);
            Point TopLeft = gt.Transform(new Point(0,0));
            Point BottomRight = gt.Transform(new Point(ve.ActualWidth,ve.ActualHeight));
            

            //Let's make sure the element is aligned outside the viewer.
            TranslateTransform inTrans = GetTranslateTransform(inFe);
            TranslateTransform outTrans = GetTranslateTransform(outFe);
            double outTo = 0;
            PropertyPath prop = new PropertyPath(TranslateTransform.XProperty);
            if (InboundAnimation == InboundAnimations.FlowInFromRight)
            {
                inTrans.X = BottomRight.X;
                inTrans.Y = 0;
                outTo = (-1 * outFe.ActualWidth);
                prop = new PropertyPath(TranslateTransform.XProperty);
            }
            else if (InboundAnimation == InboundAnimations.FlowInFromLeft)
            {
                inTrans.X = (-1 * inFe.ActualWidth);
                inTrans.Y = 0;
                outTo = BottomRight.X;
                prop = new PropertyPath(TranslateTransform.XProperty);
            }
            else if (InboundAnimation == InboundAnimations.FlowInFromTop)
            {
                inTrans.X = 0;
                inTrans.Y = (-1 * inFe.ActualHeight);
                outTo = BottomRight.Y;
                prop = new PropertyPath(TranslateTransform.YProperty);
            }
            else if (InboundAnimation == InboundAnimations.FlowInFromBottom)
            {
                inTrans.X = 0;
                inTrans.Y = BottomRight.Y;
                outTo = (-1 * outFe.ActualHeight);
                prop = new PropertyPath(TranslateTransform.YProperty);
            }

            //Set the margin to zero so that the positioning is with respect to the viewer.
            inFe.Width = inFe.ActualWidth;
            inFe.Height = inFe.ActualHeight;
            inFe.Margin = new Thickness(0, 0, 0, 0);
            inFe.VerticalAlignment = VerticalAlignment.Top;
            inFe.HorizontalAlignment = HorizontalAlignment.Left;

            outFe.Width = outFe.ActualWidth;
            outFe.Height = outFe.ActualHeight;
            outFe.Margin = new Thickness(0, 0, 0, 0);
            outFe.VerticalAlignment = VerticalAlignment.Top;
            outFe.HorizontalAlignment = HorizontalAlignment.Left;

            if (_inboundStoryboard != null)
                _inboundStoryboard.Pause();

            _inboundStoryboard = new Storyboard();
            
            DoubleAnimation inElemAnimation = new DoubleAnimation();
            inElemAnimation.BeginTime = InboundAnimationDelay;
            inElemAnimation.To = 0;
            inElemAnimation.Duration = InboundAnimationDuration;
            inElemAnimation.EasingFunction = InboundAnimationEasingFunction;
            inElemAnimation.Completed += new EventHandler(SideAnimation_Completed);

            Storyboard.SetTarget(inElemAnimation, inTrans);
            Storyboard.SetTargetProperty(inElemAnimation, prop);
            _inboundStoryboard.Children.Add(inElemAnimation);


            DoubleAnimation outElemAnimation = new DoubleAnimation();
            outElemAnimation.BeginTime = InboundAnimationDelay;
            outElemAnimation.To = outTo;
            outElemAnimation.Duration = InboundAnimationDuration;
            outElemAnimation.EasingFunction = InboundAnimationEasingFunction;
            Storyboard.SetTarget(outElemAnimation, outTrans);
            Storyboard.SetTargetProperty(outElemAnimation, prop);
            _inboundStoryboard.Children.Add(outElemAnimation);

            _inboundStoryboard.Begin();
        }
        
    }
}