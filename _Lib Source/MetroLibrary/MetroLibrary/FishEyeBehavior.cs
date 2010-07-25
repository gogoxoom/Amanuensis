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

namespace MetroLibrary
{
    public class FishEyeBehavior : Behavior<Panel>
    {
        #region Dependency Properties

        public static readonly DependencyProperty ScaleFactorProperty = DependencyProperty.Register("ScaleFactor", typeof(double), typeof(FishEyeBehavior), new PropertyMetadata(1.0));
        [Description("Scale factor"), Category("Metro Fish Eye Behavior")]
        public double ScaleFactor
        {
            get { return (double)GetValue(ScaleFactorProperty); }
            set { SetValue(ScaleFactorProperty, value); }
        }

        public static readonly DependencyProperty MinScaleFactorProperty = DependencyProperty.Register("MinScaleFactor", typeof(double), typeof(FishEyeBehavior), new PropertyMetadata(1.0));
        [Description("Mininum Scale factor"), Category("Metro Fish Eye Behavior")]
        public double MinScaleFactor
        {
            get { return (double)GetValue(MinScaleFactorProperty); }
            set { SetValue(MinScaleFactorProperty, value); }
        }

        public static readonly DependencyProperty EnableWithMouseEventsProperty = DependencyProperty.Register("EnableWithMouseEventsProperty", typeof(bool), typeof(FishEyeBehavior), new PropertyMetadata(true));
        [Description("Enables the Fish Eye Behavior on Mouse Events"), Category("Metro Fish Eye Behavior")]
        public bool EnableWithMouseEvents
        {
            get { return (bool)GetValue(EnableWithMouseEventsProperty); }
            set { SetValue(EnableWithMouseEventsProperty, value); }
        }


        public static readonly DependencyProperty EnableWithKeyEventsProperty = DependencyProperty.Register("EnableWithKeyEventsProperty", typeof(bool), typeof(FishEyeBehavior), new PropertyMetadata(true));
        [Description("Enables the Fish Eye Behavior on Key Events"), Category("Metro Fish Eye Behavior")]
        public bool EnableWithKeyEvents
        {
            get { return (bool)GetValue(EnableWithMouseEventsProperty); }
            set { SetValue(EnableWithMouseEventsProperty, value); }
        }

        #endregion

        public FishEyeBehavior()
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

            if (EnableWithMouseEvents)
            {
                AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
                AssociatedObject.MouseEnter += new MouseEventHandler(AssociatedObject_MouseMove);
                AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
            }

            if (EnableWithKeyEvents)
            {
                //AssociatedObject.AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(AssociatedObject_KeyUp), true);
                AssociatedObject.LayoutUpdated += new EventHandler(AssociatedObject_LayoutUpdated);
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            // Insert code that you would want run when the Behavior is removed from an object.
        }

        private void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Point pt = e.GetPosition(AssociatedObject);
            FishEyeProcess(pt);
        }

        
        private void AssociatedObject_KeyUp(Object sender, KeyEventArgs e)
        {
            AssociatedObject.UpdateLayout();
        }

        private void AssociatedObject_LayoutUpdated (Object sender, EventArgs e)
        {
            FrameworkElement fe = FocusManager.GetFocusedElement() as FrameworkElement;
            if (fe == null)
                return;

            GeneralTransform gt = fe.TransformToVisual(AssociatedObject as UIElement);
            Point pt = gt.Transform(new Point(fe.ActualWidth / 2, fe.ActualHeight / 2));

            FishEyeProcess(pt);
        }

        public void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Reset();
        }

        private ScaleTransform GetScaleTransform(FrameworkElement fe)
        {
            if (fe == null)
                return null;

            ScaleTransform scaleTransform = null;

            if (fe.RenderTransform == null)
            {
                fe.RenderTransform = new ScaleTransform();
                scaleTransform = fe.RenderTransform as ScaleTransform;
            }
            else if (fe.RenderTransform is ScaleTransform)
            {
                scaleTransform = fe.RenderTransform as ScaleTransform;
            }
            else if (fe.RenderTransform is TransformGroup)
            {
                TransformGroup tg = fe.RenderTransform as TransformGroup;

                scaleTransform = (from t in tg.Children
                                  where t is ScaleTransform
                                  select (ScaleTransform)t).FirstOrDefault();

                if (scaleTransform == null)
                {
                    scaleTransform = new ScaleTransform();
                    tg.Children.Add(scaleTransform);
                }
            }
            else
            {
                TransformGroup tg = new TransformGroup();
                var curTrans = fe.RenderTransform;
                fe.RenderTransform = null;
                tg.Children.Add(curTrans);
                scaleTransform = new ScaleTransform();
                tg.Children.Add(scaleTransform);
                fe.RenderTransform = tg;
            }

            return scaleTransform;
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

        private void Scale(FrameworkElement fe, double scaleFactor)
        {
            ScaleTransform trans = GetScaleTransform(fe);
            trans.ScaleX = scaleFactor;
            trans.ScaleY = scaleFactor;
        }

        private void Translate(FrameworkElement fe, double X, double Y)
        {
            TranslateTransform trans = GetTranslateTransform(fe);
            trans.X = X;
            trans.Y = Y;
        }

        private void Reset()
        {
            foreach (FrameworkElement elem in AssociatedObject.Children)
            {
                Scale(elem, 1);
                Translate(elem, 0, 0);
            }
        }

        private double Distance(Point pt1, Point pt2)
        {
            return Math.Sqrt(Math.Pow((pt1.X - pt2.X), 2) + Math.Pow((pt1.Y - pt2.Y), 2));
        }

        private void FishEyeProcess(Point pt)
        {
            double maxDistance = Distance(new Point(0, 0), new Point(AssociatedObject.ActualWidth, AssociatedObject.ActualHeight));
            foreach (FrameworkElement child in AssociatedObject.Children)
            {
                GeneralTransform childGt = child.TransformToVisual(AssociatedObject as UIElement);
                Point childCenterPt = childGt.Transform(new Point(child.ActualWidth / 2, child.ActualHeight / 2));

                double childDistance = Distance(childCenterPt, pt);
                double childScaleFactor = (-1 * (childDistance / maxDistance)) + ScaleFactor;

                if (childScaleFactor < MinScaleFactor)
                    childScaleFactor = MinScaleFactor;
                
                Scale(child, childScaleFactor);

                double XTranslate = (1 - (childScaleFactor)) * (pt.X - childCenterPt.X) / 2;
                double YTranslate = (1 - (childScaleFactor)) * (pt.Y - childCenterPt.Y) / 2;

                if (AssociatedObject is Canvas)
                {

                    XTranslate += (child.ActualWidth - (child.ActualWidth * childScaleFactor)) / 2;
                    YTranslate += (child.ActualHeight - (child.ActualHeight * childScaleFactor)) / 2;
                    //Translate(child, ((child.ActualWidth - (child.ActualWidth * childScaleFactor)) / 2), ((child.ActualHeight - (child.ActualHeight * childScaleFactor)) / 2));
                }

                Translate(child, XTranslate, YTranslate);
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