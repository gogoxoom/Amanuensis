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
using System.Diagnostics;
//using Microsoft.Expression.Interactivity.Core;

namespace MetroLibrary
{

/*    [Description("Select the Children for this layer"), Category("Children in Layer")]
    public class ParallaxLayer
    {
        public int _Layer { get; set; }
        public UIElementCollection ChildrenInLayerCollection { get; set; }
    }

    public class ParallaxLayerCollection : ObservableCollection <ParallaxLayer> {}
*/
	public class ParallaxBehavior : Behavior<Panel>
	{

        private FrameworkElement _BackgroundLayerElement;
        private FrameworkElement _TitleLayerElement;
        private FrameworkElement _SubTitleLayerElement;
        private FrameworkElement _PanelLayerElement;
        private FrameworkElement _ViewerElement;

        // These are the transforms of each of hte layer element
        private TranslateTransform _BackgroundLayerTransform;
        private TranslateTransform _TitleLayerTransform;
        private TranslateTransform _SubTitleLayerTransform;
        private TranslateTransform _PanelLayerTransform;

        
        #region Dependency Properties

        [Description("Select the Viewer element"), Category("Metro Parallax Behavior Layers")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string ViewerElementName
        {
            get;
            set;
        }

        [Description("Select the Background layer element"), Category("Metro Parallax Behavior Layers")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]  
        public string BackgroundLayerElement
        {
            get;
            set;
        }

        public static readonly DependencyProperty BackgroundLayerScrollFactorProperty = DependencyProperty.Register("BackgroundLayerScrollFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(0.1));
        [Description("Select the Background layer Scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double BackgroundLayerScrollFactor
        {
            get { return (double)GetValue(BackgroundLayerScrollFactorProperty); }
            set { SetValue(BackgroundLayerScrollFactorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundLayerTimeFactorProperty = DependencyProperty.Register("BackgroundLayerTimeFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(1.1));
        [Description("Set the Background layer scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double BackgroundLayerTimeFactor
        {
            get { return (double)GetValue(BackgroundLayerTimeFactorProperty); }
            set { SetValue(BackgroundLayerTimeFactorProperty, value); }
        }


        [Description("Select the Background layer element"), Category("Metro Parallax Behavior Layers")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string TitleLayerElement
        {
            get;
            set;
        }

        public static readonly DependencyProperty TitleLayerScrollFactorProperty = DependencyProperty.Register("TitleLayerScrollFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(0.5));
        [Description("Select the Title layer Scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double TitleLayerScrollFactor
        {
            get { return (double)GetValue(TitleLayerScrollFactorProperty); }
            set { SetValue(TitleLayerScrollFactorProperty, value); }
        }

        public static readonly DependencyProperty TitleLayerTimeFactorProperty = DependencyProperty.Register("TitleLayerTimeFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(1.7));
        [Description("Set the Title layer Time factor"), Category("Metro Parallax Behavior Layers")]
        public double TitleLayerTimeFactor
        {
            get { return (double)GetValue(TitleLayerTimeFactorProperty); }
            set { SetValue(TitleLayerTimeFactorProperty, value); }
        }


        [Description("Select the Sub Title layer element"), Category("Metro Parallax Behavior Layers")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string SubTitleLayerElement
        {
            get;
            set;
        }

        public static readonly DependencyProperty SubTitleLayerScrollFactorProperty = DependencyProperty.Register("SubTitleLayerScrollFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(1.0));
        [Description("Select the SubTitle layer Scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double SubTitleLayerScrollFactor
        {
            get { return (double)GetValue(SubTitleLayerScrollFactorProperty); }
            set { SetValue(SubTitleLayerScrollFactorProperty, value); }
        }

        public static readonly DependencyProperty SubTitleLayerTimeFactorProperty = DependencyProperty.Register("SubTitleLayerTimeFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(1.5));
        [Description("Set the SubTitle layer scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double SubTitleLayerTimeFactor
        {
            get { return (double)GetValue(SubTitleLayerTimeFactorProperty); }
            set { SetValue(SubTitleLayerTimeFactorProperty, value); }
        }

        [Description("Select the Panellayer element"), Category("Metro Parallax Behavior Layers")]
        [CustomPropertyValueEditorAttribute(CustomPropertyValueEditor.Element)]
        public string PanelLayerElement
        {
            get;
            set;
        }

        public static readonly DependencyProperty PanelLayerScrollFactorProperty = DependencyProperty.Register("PanelLayerScrollFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(1.1));
        [Description("Select the Panel layer Scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double PanelLayerScrollFactor
        {
            get { return (double)GetValue(PanelLayerScrollFactorProperty); }
            set { SetValue(PanelLayerScrollFactorProperty, value); }
        }

        public static readonly DependencyProperty PanelLayerTimeFactorProperty = DependencyProperty.Register("PanelLayerTimeFactor", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(1.3));
        [Description("Set the Panel layer scroll factor"), Category("Metro Parallax Behavior Layers")]
        public double PanelLayerTimeFactor
        {
            get { return (double)GetValue(PanelLayerTimeFactorProperty); }
            set { SetValue(PanelLayerTimeFactorProperty, value); }
        }

        
        public static readonly DependencyProperty EnablePanelSnappingProperty = DependencyProperty.Register("EnablePanelSnapping", typeof(bool), typeof(ParallaxBehavior), new PropertyMetadata(true));
        [Description("Enable Panel Layer Snapping on Children boundaries"), Category("Metro Parallax Behavior Properties")]
        public bool EnablePanelSnapping
        {
            get { return (bool)GetValue(EnablePanelSnappingProperty); }
            set { SetValue(EnablePanelSnappingProperty, value); }
        }

        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(ParallaxBehavior), new PropertyMetadata(Orientation.Horizontal));
        [Description("Sets the navigation direction"), Category("Metro Parallax Behavior Properties")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        public static readonly DependencyProperty NavigationEasingFunctionProperty = DependencyProperty.Register("NavigationEasingFunction", typeof(IEasingFunction), typeof(ParallaxBehavior), new PropertyMetadata(null));
        [Description("Select the Easing function Navigation"), Category("Metro Parallax Behavior Properties")]
        public IEasingFunction NavigationEasingFunction
        {
            get { return (IEasingFunction)GetValue(NavigationEasingFunctionProperty); }
            set { SetValue(NavigationEasingFunctionProperty, value); }
        }

        public static readonly DependencyProperty NavigationInertiaProperty = DependencyProperty.Register("NavigationEasingFunction", typeof(double), typeof(ParallaxBehavior), new PropertyMetadata(null));
        [Description("Set a inertia factor for navigation"), Category("Metro Parallax Behavior Properties")]
        public double NavigationInertia
        {
            get { return (double)GetValue(NavigationInertiaProperty); }
            set { SetValue(NavigationInertiaProperty, value); }
        }




        #endregion

        public ParallaxBehavior()
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

            //Add handlers
            AssociatedObject.Loaded += new RoutedEventHandler (AssociatedObject_Loaded);
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonDown), true);
            AssociatedObject.AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(AssociatedObject_MouseLeftButtonUp), true);
            AssociatedObject.MouseMove += new MouseEventHandler(AssociatedObject_MouseMove);
            AssociatedObject.MouseLeave += new MouseEventHandler(AssociatedObject_MouseLeave);
            AssociatedObject.MouseEnter += new MouseEventHandler(AssociatedObject_MouseEnter);
            
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();

			// Insert code that you would want run when the Behavior is removed from an object.
		}
        
        private TranslateTransform GetTranslateTransform (string elemName)
        {
            if (elemName == null)
                return null; 

            object obj = AssociatedObject.FindName(elemName);
            if (obj == null)
                return null;

            FrameworkElement fe = obj as FrameworkElement;

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
            if (BackgroundLayerElement != null)
                _BackgroundLayerElement = AssociatedObject.FindName(BackgroundLayerElement) as FrameworkElement;
            if (TitleLayerElement != null)
                _TitleLayerElement = AssociatedObject.FindName(TitleLayerElement) as FrameworkElement;
            if (SubTitleLayerElement != null)
                _SubTitleLayerElement = AssociatedObject.FindName(SubTitleLayerElement) as FrameworkElement;
            if (PanelLayerElement != null)
                _PanelLayerElement = AssociatedObject.FindName(PanelLayerElement) as FrameworkElement;

                       
            if (ViewerElementName != null)
            {
                _ViewerElement = AssociatedObject.FindName(ViewerElementName) as FrameworkElement;
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
                        _ViewerElement = container as FrameworkElement;
                        containerSize.Width = (container as FrameworkElement).ActualWidth;
                        containerSize.Height = (container as FrameworkElement).ActualHeight;
                    }
                    container = VisualTreeHelper.GetParent(container);
                }
            }
            
            _BackgroundLayerTransform = GetTranslateTransform(BackgroundLayerElement);
            _TitleLayerTransform = GetTranslateTransform(TitleLayerElement);
            _SubTitleLayerTransform = GetTranslateTransform(SubTitleLayerElement);
            _PanelLayerTransform = GetTranslateTransform(PanelLayerElement);
        }


        int _curChildIdx = 0;
        private Point _prevPoint;
        private double _prevDelta = 0;
        bool _MouseLeftButtonIsDown = false;
        Storyboard _storyboard = null;

        // Mouse Handlers
        public void AssociatedObject_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _prevPoint = e.GetPosition(null);

            _prevDelta = 0;
            _MouseLeftButtonIsDown = true;

        }


        public void AssociatedObject_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            if (_MouseLeftButtonIsDown)
            {
                _MouseLeftButtonIsDown = false;
                if (EnablePanelSnapping)
                    SnapToActiveChild();
            }

            _prevDelta = 0;
        }


        public void AssociatedObject_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_MouseLeftButtonIsDown == false)
                return;

            if (_storyboard != null)
                _storyboard.Pause();

            Point pt = e.GetPosition(null);

            Double delta = 0;

            if (Orientation == Orientation.Horizontal)
                delta = pt.X - _prevPoint.X;
            else
                delta = pt.Y - _prevPoint.Y;

            _prevPoint = pt;
            _prevDelta = delta;

            ScrollBy(delta);

        }

        public void AssociatedObject_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            _prevDelta = 0;
        }

        public void AssociatedObject_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            if (_MouseLeftButtonIsDown)
            {
                _MouseLeftButtonIsDown = false;
                if (EnablePanelSnapping)
                    SnapToActiveChild();
            }

            _prevDelta = 0;
        }

        private double ScrollElement(FrameworkElement elem, TranslateTransform trans, double delta, bool checkBounds)
        {
            if ((elem == null) || (trans == null) || (_ViewerElement == null))
                return 0;
           
            double to = 0;
            double actualDelta = delta;
            if (Orientation == Orientation.Horizontal)
            {
                double maxOffset = _ViewerElement.ActualWidth / 3;
                to = trans.X + delta;
                if (checkBounds)
                {
                    if (to > maxOffset)
                        to = maxOffset;
                    else if (to < ((maxOffset * 2) - elem.ActualWidth))
                        to = (maxOffset * 2) - elem.ActualWidth;
                }

                actualDelta = to - trans.X;
                trans.X = to;

            }
            else
            {
                double maxOffset = _ViewerElement.ActualHeight / 3;
                to = trans.Y + delta;
                if (checkBounds)
                {
                    if (to > maxOffset)
                        to = maxOffset;
                    else if (to < ((2 * maxOffset) - elem.ActualHeight))
                        to = (2 * maxOffset) - elem.ActualHeight;
                }

                actualDelta = to - trans.Y;
                trans.Y = to;
            }

            return actualDelta;
        }

        private void ScrollBy(double delta)
        {

            double actualDelta = 0;

            actualDelta = ScrollElement(_PanelLayerElement, _PanelLayerTransform, (delta * PanelLayerScrollFactor), true);

            ScrollElement(_BackgroundLayerElement, _BackgroundLayerTransform, (actualDelta * BackgroundLayerScrollFactor), false);
            ScrollElement(_TitleLayerElement, _TitleLayerTransform, (actualDelta * TitleLayerScrollFactor), false);
            ScrollElement(_SubTitleLayerElement, _SubTitleLayerTransform, (actualDelta * SubTitleLayerScrollFactor), false);
            
            
        }

        private void SnapToActiveChild()
        {

            if ((_PanelLayerElement == null) || !(_PanelLayerElement is Panel) || (_ViewerElement == null))
                return;

            //Determine which visual element is the active element in the viewport.
            

            Point centerPoint = new Point(_ViewerElement.ActualWidth/2, _ViewerElement.ActualHeight/2);

            int newChildIdx = _curChildIdx;
            int i = 0;
            foreach (UIElement child in ((Panel)_PanelLayerElement).Children)
            {

                GeneralTransform gt = child.TransformToVisual(_ViewerElement as UIElement);
                Point pointTopLeft = gt.Transform(new Point(0, 0));
                Point pointBottomRight = gt.Transform(new Point(((FrameworkElement)child).ActualWidth, ((FrameworkElement)child).ActualHeight));


                if (Orientation == Orientation.Horizontal)
                {
                    if ((pointTopLeft.X <= centerPoint.X) && (pointBottomRight.X >= centerPoint.X))
                    {
                        //We found the child that is in the viewport.
                        newChildIdx = i;
                        break;
                    }
                }
                else
                {
                    if ((pointTopLeft.Y <= centerPoint.Y) && (pointBottomRight.Y >= centerPoint.Y))
                    {
                        //We found the child that is in the viewport.
                        newChildIdx = i;
                        break;
                    }
                }

                i++;
            }

            
            _curChildIdx = newChildIdx;
            
            double offset = 0;
            offset = GetPanelScrollOffset(newChildIdx, _PanelLayerElement as Panel, _PanelLayerTransform, _ViewerElement, _prevDelta);
            //Debug.WriteLine("Active child: {0}, new offset {1}", newChildIdx, offset);

            if (_storyboard != null)
                _storyboard.Pause();


            //Now the Animation part
            _storyboard = new Storyboard();

            if (Orientation == Orientation.Horizontal)
            {

                AddLayerTransformAnimation(_storyboard, offset, 1000 * PanelLayerTimeFactor, _PanelLayerTransform, new PropertyPath(TranslateTransform.XProperty));
                AddLayerTransformAnimation(_storyboard, offset * BackgroundLayerScrollFactor, 1000 * BackgroundLayerTimeFactor, _BackgroundLayerTransform, new PropertyPath(TranslateTransform.XProperty));
                AddLayerTransformAnimation(_storyboard, offset * SubTitleLayerScrollFactor, 1000 * SubTitleLayerTimeFactor, _SubTitleLayerTransform, new PropertyPath(TranslateTransform.XProperty));
                AddLayerTransformAnimation(_storyboard, offset * TitleLayerScrollFactor, 1000 * TitleLayerTimeFactor, _TitleLayerTransform, new PropertyPath(TranslateTransform.XProperty));
            }
            else
            {
                AddLayerTransformAnimation(_storyboard, offset, 1000 * PanelLayerTimeFactor, _PanelLayerTransform, new PropertyPath(TranslateTransform.YProperty));
                AddLayerTransformAnimation(_storyboard, offset * BackgroundLayerScrollFactor, 1000 * BackgroundLayerTimeFactor, _BackgroundLayerTransform, new PropertyPath(TranslateTransform.YProperty));
                AddLayerTransformAnimation(_storyboard, offset * SubTitleLayerScrollFactor, 1000 * SubTitleLayerTimeFactor, _SubTitleLayerTransform, new PropertyPath(TranslateTransform.YProperty));
                AddLayerTransformAnimation(_storyboard, offset * TitleLayerScrollFactor, 1000 * TitleLayerTimeFactor, _TitleLayerTransform, new PropertyPath(TranslateTransform.YProperty));
            }

            _storyboard.Begin();

        }

        private void AddLayerTransformAnimation (Storyboard sb, double position, double miliSec, TranslateTransform trans, PropertyPath path)
        {
            if (trans == null)
                return;

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(miliSec));
            animation.To = position;
            //animation.FillBehavior = FillBehavior.
            //animation.By = 10;
            animation.AutoReverse = false;
            animation.EasingFunction = NavigationEasingFunction;

            
            Storyboard.SetTarget(animation, trans);
            Storyboard.SetTargetProperty(animation, path);
            sb.Children.Add(animation);
            
        }

        
        private double GetPanelScrollOffset (int idx, Panel panel, TranslateTransform panelTransform , FrameworkElement viewer, double motionDelta)
        {
            //Maybe we can add some inertia using motionDelta.

            double offset = 0;
            FrameworkElement fe = panel.Children[idx] as FrameworkElement;
            Point centerPoint = new Point(viewer.ActualWidth/2, viewer.ActualHeight/2);
            GeneralTransform gt = fe.TransformToVisual(viewer as UIElement);
            Point viewerTopLeft = gt.Transform(new Point(0, 0));
            Point viewerBottomRight = gt.Transform(new Point(fe.ActualWidth, fe.ActualHeight));

            gt = fe.TransformToVisual(panel as UIElement);
            Point panelTopLeft = gt.Transform(new Point(0, 0));
            Point panelBottomRight = gt.Transform(new Point(fe.ActualWidth, fe.ActualHeight));


            if (Orientation == Orientation.Horizontal)
            {
                if (fe.ActualWidth <= _ViewerElement.ActualWidth)
                {
                    offset = ((_ViewerElement.ActualWidth - fe.ActualWidth) / 2) - panelTopLeft.X;
                }
                else if ((viewerTopLeft.X >= 0) && (viewerTopLeft.X < viewer.ActualWidth))
                {
                    offset = -1 * panelTopLeft.X;
                }
                else if ((viewerBottomRight.X >= 0) && (viewerBottomRight.X < viewer.ActualWidth))
                {
                    offset = viewer.ActualWidth - panelBottomRight.X;
                }
                else
                {
                    offset = panelTransform.X + (NavigationInertia * motionDelta);
                    if ((Math.Sign(motionDelta) < 0) && (offset < (viewer.ActualWidth - panelBottomRight.X)))
                    {
                        offset = viewer.ActualWidth - panelBottomRight.X;
                    }
                    else if ((Math.Sign(motionDelta) > 0) && (offset > (-1 * panelTopLeft.X)))
                    {
                        offset = -1 * panelTopLeft.X;
                    }
                }
            }
            else
            {
                if (fe.ActualHeight <= _ViewerElement.ActualHeight)
                {
                    offset = ((_ViewerElement.ActualHeight - fe.ActualHeight) / 2) - panelTopLeft.Y;
                }
                else if ((viewerTopLeft.Y >= 0) && (viewerTopLeft.Y < viewer.ActualHeight))
                {
                    offset = -1 * panelTopLeft.Y;
                }
                else if ((viewerBottomRight.Y >= 0) && (viewerBottomRight.Y < viewer.ActualHeight))
                {
                    offset = viewer.ActualHeight - panelBottomRight.Y;
                }
                else
                {
                    offset = panelTransform.Y + (NavigationInertia * motionDelta);
                    if ((Math.Sign(motionDelta) < 0) && (offset < (viewer.ActualHeight - panelBottomRight.Y)))
                    {
                        offset = viewer.ActualHeight - panelBottomRight.Y;
                    }
                    else if ((Math.Sign(motionDelta) > 0) && (offset > (-1 * panelTopLeft.Y)))
                    {
                        offset = -1 * panelTopLeft.Y;
                    }
                }
            }

            return offset;
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