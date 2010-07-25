using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;
using System.Threading;



namespace TileSL
{
    [Description("A Metro like Tile.")]
    [TemplatePart(Name = "TileContainer", Type = typeof (Grid))]
    [TemplatePart(Name = "TileText", Type = typeof(TextBlock))]
    [TemplateVisualState(Name = "UnselectedState", GroupName= "TileVisualStateGroup")]
    [TemplateVisualState(Name = "SelectedState", GroupName = "TileVisualStateGroup")]
    public class Tile : ContentControl
    {
        public Tile()
        {
            
            AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(TileMouseLeftButtonUp), true);
            AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(TileMouseLeftButtonDown), true);
            AddHandler(UIElement.KeyUpEvent, new KeyEventHandler(TileKeyUp), true);
            //MouseLeftButtonDown += new MouseButtonEventHandler(TileMouseLeftButtonDown);
            //MouseLeftButtonUp += new MouseButtonEventHandler(TileMouseLeftButtonUp);

                        
            // Cannot use AddHandler because AddHandler is only used for routed events and Loaded is not a true routed event.
            Loaded += new RoutedEventHandler(OnTileLoaded);
            
            GotFocus += new RoutedEventHandler(TileGotFocus);
            LostFocus += new RoutedEventHandler(TileLostFocus);


            DefaultStyleKey = typeof(Tile);

        }

        
        Point _mouseDownPoint; 

        //Timers and helper objects
        DispatcherTimer _timer;
        DispatcherTimer _timer2;
        TextBlock _tileText;
        int _count = 0;
        Random _rand;
        
        #region Dependency Properties

        /*public static readonly DependencyProperty BackgroundImageProperty = DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(Tile), new PropertyMetadata(new PropertyChangedCallback(OnBackgroudImageChanged)));
        [Category("Metro Control")]
        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }*/

        public static readonly DependencyProperty TileTitleProperty = DependencyProperty.Register("TileTitle", typeof(String), typeof(Tile), new PropertyMetadata("Title"));//, new PropertyChangedCallback(TileTitleChanged)));
        [Category("Metro Control")]
        public String TileTitle
        {
            get { return (String)GetValue(TileTitleProperty); }
            set { SetValue(TileTitleProperty, value); }
        }

        static void TileTitleChanged(DependencyObject property, DependencyPropertyChangedEventArgs args)
        {
            //Tile tile = (Tile)property;
            //tile.GetTemplateChild("TileTitle)
        }

        public static readonly DependencyProperty FlipXProperty =
          DependencyProperty.Register("FlipX", typeof(Boolean), typeof(Tile), new PropertyMetadata(true));
        [Category("Metro Control")]
        public Boolean FlipX
        {
            get { return (Boolean)GetValue(FlipXProperty); }
            set { SetValue(FlipXProperty, value); }
        }

        public static readonly DependencyProperty FlipYProperty =
          DependencyProperty.Register("FlipY", typeof(Boolean), typeof(Tile), new PropertyMetadata(null));
        [Category("Metro Control")]
        public Boolean FlipY
        {
            get { return (Boolean)GetValue(FlipYProperty); }
            set { SetValue(FlipYProperty, value); }
        }

        public static readonly DependencyProperty EnableTileTextProperty =
          DependencyProperty.Register("EnableTileText", typeof(Boolean), typeof(Tile), new PropertyMetadata(false));
        [Category("Metro Control")]
        public Boolean EnableTileText
        {
            get { return (Boolean)GetValue(EnableTileTextProperty); }
            set { SetValue(EnableTileTextProperty, value); }
        }

        public static readonly DependencyProperty EnablePressEffectProperty =
        DependencyProperty.Register("EnablePressEffect", typeof(Boolean), typeof(Tile), new PropertyMetadata(false));
        [Category("Metro Control")]
        public Boolean EnablePressEffect
        {
            get { return (Boolean)GetValue(EnablePressEffectProperty); }
            set { SetValue(EnablePressEffectProperty, value); }
        }
        #endregion

       
        void OnTileLoaded(Object sender, RoutedEventArgs e)
        {
            //Launch the first timer.
            _rand = new Random();
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0, 0, 30);
            _timer.Tick += new EventHandler(BeginUpdateTileText);
            _timer.Start();
            //_tileText = (TextBlock)FindName("TileText");
        }

        private void BeginUpdateTileText(object sender, EventArgs e)
        {

            if (_tileText != null)
            {
                if (!EnableTileText)
                    _tileText.Text = "";
                else
                {
                    SpinNumberIncrease();
                }
            }
        }
        

        private void SpinNumberIncrease ()
        {
            
            if ((_count == 0) && (_timer2 != null))
                _timer2.Stop();
               
            _count = _rand.Next (0, 10);
            _timer2 = new DispatcherTimer();
            _timer2.Interval = new TimeSpan(0, 0, 0, 0, 500);
        
            _timer2.Tick += new EventHandler(UpdateTileText);
            _timer2.Start();

        }

        private void UpdateTileText(object sender, EventArgs e)
        {
            if ((_count <= 0) || (_tileText == null))
                _timer2.Stop();

            _count--;

            int num = 0;
            try
            {
                num = Convert.ToInt32(_tileText.Text);
            }
            catch (FormatException ex)
            {
                num = 0;
            }

            if (num > 99)
                num = 0;
            
            num++;
            _tileText.Text = num.ToString();
            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _tileText = GetTemplateChild("TileText") as TextBlock;
        }

        private static void OnBackgroudImageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //Debug.WriteLine("Background Image Changed");
        }

        public void TileMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _mouseDownPoint = e.GetPosition (this);
            //Debug.WriteLine("MouseLeftButtonDown Point {0} {1}", _mouseDownPoint.X, _mouseDownPoint.Y);
            
        }

        public void TileMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {

            Point mouseUpPoint = e.GetPosition(this);

            //VisualStateManager.GoToState(this, "UnselectedState", true);

            //Debug.WriteLine("MouseLeftButtonUp Point {0} {1}", mouseUpPoint.X, mouseUpPoint.Y);

            if ((Math.Abs(mouseUpPoint.X - _mouseDownPoint.X) > 2) || (Math.Abs(mouseUpPoint.Y - _mouseDownPoint.Y) > 2))
            {
                // This is here so that the GoToStateAction behavior doesn't handle a move of the mouse as a click.
                //e.Handled = true;
                return;
            }

            Focus();

            var TileContainer = (FrameworkElement)GetTemplateChild("TileContainer");

            if (EnablePressEffect)
            {
                AnimatedPressEffect(TileContainer, mouseUpPoint);
            }
            else
            {
                Storyboard sb;
                sb = (Storyboard)TileContainer.Resources["sbXFlip"];
                if ((sb != null) && FlipX)
                {
                    sb.Begin();
                }

                sb = (Storyboard)TileContainer.Resources["sbYFlip"];
                if ((sb != null) && FlipY)
                {
                    sb.Begin();
                }
            }
        }

        private void AnimatedPressEffect(FrameworkElement fe, Point point)
        {
            DoubleAnimation xRotationAnimation = new DoubleAnimation();
            DoubleAnimation yRotationAnimation = new DoubleAnimation();
            DoubleAnimation zOffsetAnimation = new DoubleAnimation();


            xRotationAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            xRotationAnimation.From = 0;
            yRotationAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            yRotationAnimation.From = 0;
            zOffsetAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(100));
            zOffsetAnimation.From = 0;

            double ydist = (fe.ActualHeight / 2) - point.Y;
            double xdist = (fe.ActualWidth / 2) - point.X;

            double xCenter = fe.ActualWidth / 2;
            double yCenter = fe.ActualHeight / 2;

            double xMargin = fe.ActualWidth / 4;
            double yMargin = fe.ActualHeight / 4;

            if (point.X > (xCenter + xMargin))
                yRotationAnimation.To = -15;
            else if (point.X < (xCenter - xMargin))
                yRotationAnimation.To = 15;

            if (point.Y > (yCenter + yMargin))
                xRotationAnimation.To = 15;
            else if (point.Y < (yCenter - yMargin))
                xRotationAnimation.To = -15;

            zOffsetAnimation.To = -40;

            xRotationAnimation.AutoReverse = true;
            xRotationAnimation.EasingFunction = new System.Windows.Media.Animation.PowerEase();

            yRotationAnimation.AutoReverse = true;
            yRotationAnimation.EasingFunction = new System.Windows.Media.Animation.PowerEase();

            zOffsetAnimation.AutoReverse = true;
            zOffsetAnimation.EasingFunction = new System.Windows.Media.Animation.PowerEase();

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(yRotationAnimation);
            storyboard.Children.Add(xRotationAnimation);
            storyboard.Children.Add(zOffsetAnimation);


            // Create the TranslateTransform
            if ((fe.Projection == null) || (fe.Projection.GetType().ToString() != "System.Windows.Media.PlaneProjection"))
            {
                fe.Projection = new PlaneProjection();
            }

            PlaneProjection projection = fe.Projection as PlaneProjection;

            projection.SetValue(PlaneProjection.CenterOfRotationXProperty, 0.5);
            projection.SetValue(PlaneProjection.CenterOfRotationYProperty, 0.5);
            
            Storyboard.SetTarget(yRotationAnimation, projection);
            Storyboard.SetTarget(xRotationAnimation, projection);
            Storyboard.SetTarget(zOffsetAnimation, projection);

            

            Storyboard.SetTargetProperty(xRotationAnimation, new PropertyPath(PlaneProjection.RotationXProperty));
            Storyboard.SetTargetProperty(yRotationAnimation, new PropertyPath(PlaneProjection.RotationYProperty));
            Storyboard.SetTargetProperty(zOffsetAnimation, new PropertyPath(PlaneProjection.GlobalOffsetZProperty));

            storyboard.Begin();
        }

        private void TileKeyUp(Object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("TileKeyUp: {0}", e.Key.ToString());
        }

        private void TileGotFocus(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("TileGotFocus");
            VisualStateManager.GoToState(this, "SelectedState", true);
        }

        private void TileLostFocus(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("TileLostFocus");
            VisualStateManager.GoToState(this, "UnselectedState", true);
        }
    }
}



