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
    public class FlowTransitionBehavior : TargetedTriggerAction<FrameworkElement>
    {
        #region Dependency Properties

        //Pre definedAnimations        
        public enum InboundTransitionAnimations
        {
            FlowFromRight = 0,
            FlowFromLeft,
            FlowFromTop,
            FlowFromBottom
        };


        public enum OutboundTransitionAnimations
        {
            FlowOutLeft = 0,
            FlowOutRight,
            FlowOutBottom,
            FlowOutTop
        };

        //Outbound elements and parameters
        public static readonly DependencyProperty OutboundAnimationProperty = DependencyProperty.Register("OutboundAnimation", typeof(OutboundTransitionAnimations), typeof(FlowTransitionBehavior), new PropertyMetadata(OutboundTransitionAnimations.FlowOutLeft));
        [Description("Select the easing function"), Category("Flow Transition Behavior Outbound")]
        public OutboundTransitionAnimations OutboundAnimation
        {
            get { return (OutboundTransitionAnimations)GetValue(OutboundAnimationProperty); }
            set { SetValue(OutboundAnimationProperty, value); }
        }


        public static readonly DependencyProperty OutboundAnimationEasingFunctionProperty = DependencyProperty.Register("OutboundAnimationEasingFunction", typeof(IEasingFunction), typeof(FlowTransitionBehavior), new PropertyMetadata(null));
        [Description("Select the easing function"), Category("Flow Transition Behavior Outbound")]
        public IEasingFunction OutboundAnimationEasingFunction
        {
            get { return (IEasingFunction)GetValue(OutboundAnimationEasingFunctionProperty); }
            set { SetValue(OutboundAnimationEasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty OutboundAnimationDelayProperty = DependencyProperty.Register("OutboundAnimationDelay", typeof(TimeSpan), typeof(FlowTransitionBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0)));
        [Description("Set the animation delay"), Category("Flow Transition Behavior Outbound")]
        public TimeSpan OutboundAnimationDelay
        {
            get { return (TimeSpan)GetValue(OutboundAnimationDelayProperty); }
            set { SetValue(OutboundAnimationDelayProperty, value); }
        }

        public static readonly DependencyProperty OutboundAnimationDurationProperty = DependencyProperty.Register("OutboundAnimationDuration", typeof(Duration), typeof(FlowTransitionBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2))));
        [Description("Set the animation duration"), Category("Flow Transition Behavior Outbound")]
        public Duration OutboundAnimationDuration
        {
            get { return (Duration)GetValue(OutboundAnimationDurationProperty); }
            set { SetValue(OutboundAnimationDurationProperty, value); }
        }


        //Inbound element and parameter
        [Description("Select the element to animate in"), Category("Flow Transition Behavior Inbound")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string InbountElementName
        {
            get;
            set;
        }

        [Description("Select the Viewer element"), Category("Flow Transition Behavior Inbound")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ViewerElementName
        {
            get;
            set;
        }

        public static readonly DependencyProperty InboundAnimationProperty = DependencyProperty.Register("InboundAnimation", typeof(InboundTransitionAnimations), typeof(FlowTransitionBehavior), new PropertyMetadata(InboundTransitionAnimations.FlowFromRight));
        [Description("Select the easing function"), Category("Flow Transition Behavior Inbound")]
        public InboundTransitionAnimations InboundAnimation
        {
            get { return (InboundTransitionAnimations)GetValue(InboundAnimationProperty); }
            set { SetValue(InboundAnimationProperty, value); }
        }


        public static readonly DependencyProperty InboundAnimationEasingFunctionProperty = DependencyProperty.Register("InboundAnimationEasingFunction", typeof(IEasingFunction), typeof(FlowTransitionBehavior), new PropertyMetadata(null));
        [Description("Select the easing function"), Category("Flow Transition Behavior Inbound")]
        public IEasingFunction InboundAnimationEasingFunction
        {
            get { return (IEasingFunction)GetValue(InboundAnimationEasingFunctionProperty); }
            set { SetValue(InboundAnimationEasingFunctionProperty, value); }
        }


        public static readonly DependencyProperty InboundAnimationDelayProperty = DependencyProperty.Register("InboundAnimationDelay", typeof(TimeSpan), typeof(FlowTransitionBehavior), new PropertyMetadata(TimeSpan.FromSeconds(0)));
        [Description("Set the animation delay"), Category("Flow Transition Behavior Inbound")]
        public TimeSpan InboundAnimationDelay
        {
            get { return (TimeSpan)GetValue(InboundAnimationDelayProperty); }
            set { SetValue(InboundAnimationDelayProperty, value); }
        }

        public static readonly DependencyProperty InboundAnimationDurationProperty = DependencyProperty.Register("InboundAnimationDuration", typeof(Duration), typeof(FlowTransitionBehavior), new PropertyMetadata(new Duration(TimeSpan.FromSeconds(2))));
        [Description("Set the animation duration"), Category("Flow Transition Behavior Inbound")]
        public Duration InboundAnimationDuration
        {
            get { return (Duration)GetValue(InboundAnimationDurationProperty); }
            set { SetValue(InboundAnimationDurationProperty, value); }
        }

        public static readonly DependencyProperty WaitForOutbounAnimationCompletionProperty = DependencyProperty.Register("WaitForOutbounAnimationCompletion", typeof(bool), typeof(FlowTransitionBehavior), new PropertyMetadata(true));
        [Description("Set whether the Outbound Animation should complete before the Inbound Animation starts"), Category("Flow Transition Behavior Inbound")]
        public bool WaitForOutbounAnimationCompletion
        {
            get { return (bool)GetValue(WaitForOutbounAnimationCompletionProperty); }
            set { SetValue(WaitForOutbounAnimationCompletionProperty, value); }
        }

        #endregion
        public FlowTransitionBehavior()
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

        private FrameworkElement FindViewerElement()
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

        private Storyboard _outboundStoryboard;
        private Storyboard _inboundStoryboard;

        protected override void Invoke(object parameter)
        {

            if ((Math.Abs(_mouseDownPoint.X - _mouseUpPoint.X) > 2) || (Math.Abs(_mouseDownPoint.Y - _mouseUpPoint.Y) > 2))
            {
                return;
            }

            if (Target == null)
                return;

            RunOutboundAnimation();
            if (!WaitForOutbounAnimationCompletion)
                RunInboundAnimation();
        }


        private void OutboundAnimation_Completed(object sender, EventArgs e)
        {
            if (WaitForOutbounAnimationCompletion)
                RunInboundAnimation();
        }

        private void InboundAnimation_Completed(object sender, EventArgs e)
        {
            //_outboundStoryboard.Stop();
        }

        private void RunOutboundAnimation()
        {

            FrameworkElement outFe = TargetObject as FrameworkElement;
            if (outFe == null)
            {
                outFe = AssociatedObject as FrameworkElement;
                if (outFe == null)
                    return;
            }

            FrameworkElement ve = FindViewerElement();
            if (ve == null)
                return;

            if (_outboundStoryboard != null)
                _outboundStoryboard.Pause();

            _outboundStoryboard = new Storyboard();
            
            GeneralTransform gt = ve.TransformToVisual(Application.Current.RootVisual);
            Point TopLeft = gt.Transform(new Point(0, 0));
            Point BottomRight = gt.Transform(new Point(ve.ActualWidth, ve.ActualHeight));


            //Let's make sure the element is aligned outside the viewer.
            TranslateTransform outTrans = GetTranslateTransform(outFe);
            double outTo = 0;
            PropertyPath prop = new PropertyPath(TranslateTransform.XProperty);
            if (OutboundAnimation == OutboundTransitionAnimations.FlowOutLeft)
            {
                outTo = (-1 * outFe.ActualWidth);
                prop = new PropertyPath(TranslateTransform.XProperty);
            }
            else if (OutboundAnimation == OutboundTransitionAnimations.FlowOutRight)
            {
                outTo = BottomRight.X;
                prop = new PropertyPath(TranslateTransform.XProperty);
            }
            else if (OutboundAnimation == OutboundTransitionAnimations.FlowOutBottom)
            {
                outTo = BottomRight.Y;
                prop = new PropertyPath(TranslateTransform.YProperty);
            }
            else if (OutboundAnimation == OutboundTransitionAnimations.FlowOutTop)
            {
                outTo = (-1 * outFe.ActualHeight);
                prop = new PropertyPath(TranslateTransform.YProperty);
            }

            //Set the margin to zero so that the positioning is with respect to the viewer.
            outFe.Width = outFe.ActualWidth;
            outFe.Height = outFe.ActualHeight;
            outFe.Margin = new Thickness(0, 0, 0, 0);
            outFe.VerticalAlignment = VerticalAlignment.Top;
            outFe.HorizontalAlignment = HorizontalAlignment.Left;


            DoubleAnimation outElemAnimation = new DoubleAnimation();
            outElemAnimation.To = outTo;
            outElemAnimation.BeginTime = OutboundAnimationDelay;
            outElemAnimation.Duration = OutboundAnimationDuration;
            outElemAnimation.EasingFunction = OutboundAnimationEasingFunction;
            outElemAnimation.Completed += new EventHandler(OutboundAnimation_Completed);
            outElemAnimation.FillBehavior = FillBehavior.HoldEnd;

            Storyboard.SetTarget(outElemAnimation, outTrans);
            Storyboard.SetTargetProperty(outElemAnimation, prop);
            _outboundStoryboard.Children.Add(outElemAnimation);

            _outboundStoryboard.Begin();
        }

        private void RunInboundAnimation()
        {
            FrameworkElement inFe = (AssociatedObject as FrameworkElement).FindName(InbountElementName) as FrameworkElement;
            if (inFe == null)
                return;

            FrameworkElement ve = FindViewerElement();
            if (ve == null)
                return;

            if (_inboundStoryboard != null)
                _inboundStoryboard.Pause();

            _inboundStoryboard = new Storyboard();
            
            GeneralTransform gt = ve.TransformToVisual(Application.Current.RootVisual);
            Point TopLeft = gt.Transform(new Point(0, 0));
            Point BottomRight = gt.Transform(new Point(ve.ActualWidth, ve.ActualHeight));


            //Let's make sure the element is aligned outside the viewer.
            TranslateTransform inTrans = GetTranslateTransform(inFe);
            
            
            PropertyPath prop = new PropertyPath(TranslateTransform.XProperty);
            if (InboundAnimation == InboundTransitionAnimations.FlowFromRight)
            {
                inTrans.X = BottomRight.X;
                inTrans.Y = 0;
                prop = new PropertyPath(TranslateTransform.XProperty);
            }
            else if (InboundAnimation == InboundTransitionAnimations.FlowFromLeft)
            {
                inTrans.X = (-1 * inFe.ActualWidth);
                inTrans.Y = 0;
                prop = new PropertyPath(TranslateTransform.XProperty);
            }
            else if (InboundAnimation == InboundTransitionAnimations.FlowFromTop)
            {
                inTrans.X = 0;
                inTrans.Y = (-1 * inFe.ActualHeight);
                prop = new PropertyPath(TranslateTransform.YProperty);
            }
            else if (InboundAnimation == InboundTransitionAnimations.FlowFromBottom)
            {
                inTrans.X = 0;
                inTrans.Y = BottomRight.Y;
                prop = new PropertyPath(TranslateTransform.YProperty);
            }

            //Set the margin to zero so that the positioning is with respect to the viewer.
            inFe.Width = inFe.ActualWidth;
            inFe.Height = inFe.ActualHeight;
            inFe.Margin = new Thickness(0, 0, 0, 0);
            inFe.VerticalAlignment = VerticalAlignment.Top;
            inFe.HorizontalAlignment = HorizontalAlignment.Left;


            DoubleAnimation inElemAnimation = new DoubleAnimation();
            inElemAnimation.BeginTime = InboundAnimationDelay;
            inElemAnimation.To = 0;
            inElemAnimation.Duration = InboundAnimationDuration;
            inElemAnimation.EasingFunction = InboundAnimationEasingFunction;
            inElemAnimation.Completed += new EventHandler(InboundAnimation_Completed);

            Storyboard.SetTarget(inElemAnimation, inTrans);
            Storyboard.SetTargetProperty(inElemAnimation, prop);
            _inboundStoryboard.Children.Add(inElemAnimation);

            _inboundStoryboard.Begin();
        }

    }
}