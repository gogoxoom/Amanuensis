using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using System.Windows.Media.Imaging;


namespace TileSL
{
    public class MetroPanel : Panel
    {

         #region Dependency Properties

        public static readonly DependencyProperty ColumnProperty = DependencyProperty.Register("Column", typeof(int), typeof(MetroPanel), new PropertyMetadata(2, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty RowProperty = DependencyProperty.Register("Row", typeof(int), typeof(MetroPanel), new PropertyMetadata(2, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register("Orientation", typeof(Orientation), typeof(MetroPanel), new PropertyMetadata(Orientation.Vertical, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty ChildrenMarginProperty = DependencyProperty.Register("ChildrenMargin", typeof(Thickness), typeof(MetroPanel), new PropertyMetadata(new Thickness(10), new PropertyChangedCallback(OnPropertyChanged)));
        
        public static readonly DependencyProperty NavigationEasingFunctionProperty = DependencyProperty.Register("NavigationEasingFunction", typeof(IEasingFunction), typeof(MetroPanel), new PropertyMetadata(null));
        public static readonly DependencyProperty EnableMouseDragProperty = DependencyProperty.Register("EnableMouseDrag", typeof(bool), typeof(MetroPanel), new PropertyMetadata(false));
        public static readonly DependencyProperty EnableZStackModeProperty = DependencyProperty.Register("EnableZStackMode", typeof(bool), typeof(MetroPanel), new PropertyMetadata(false, new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty ZStackingOffsetProperty = DependencyProperty.Register("ZStackingOffset", typeof(Point), typeof(MetroPanel), new PropertyMetadata(new Point(0,0), new PropertyChangedCallback(OnPropertyChanged)));
        public static readonly DependencyProperty EnablePanelSnappingProperty = DependencyProperty.Register("EnablePanelSnapping", typeof(bool), typeof(MetroPanel), new PropertyMetadata(false));

        public static readonly DependencyProperty BackgroundImageProperty = DependencyProperty.Register("BackgroundImage", typeof(ImageSource), typeof(MetroPanel), null);
        public static readonly DependencyProperty BackgroundImageScrollFactorProperty = DependencyProperty.Register("BackgroundImageScrollFactor", typeof(double), typeof(MetroPanel), new PropertyMetadata(-0.3));


        public static readonly DependencyProperty MyTileProperty = DependencyProperty.Register("MyTile", typeof(Tile), typeof(MetroPanel), null);

        [Category("Metro Control")]
        public Tile MyTile
        {
            get { return (Tile)GetValue(MyTileProperty); }
            set { SetValue(MyTileProperty, value); }
        }

        [Category("Metro Control")]
        public int Column
        {
            get { return (int)GetValue(ColumnProperty); }
            set { SetValue(ColumnProperty, value); }
        }

        [Category("Metro Control")]
        public int Row
        {
            get { return (int)GetValue(RowProperty); }
            set { SetValue(RowProperty, value); }
        }

        [Category("Metro Control")]
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }


        [Description("Margin for children"), Category("Metro Control")]
        public Thickness ChildrenMargin
        {
            get { return (Thickness)GetValue(ChildrenMarginProperty); }
            set { SetValue(ChildrenMarginProperty, value); }
        }

        [Description("Enable Animation when navigating"), Category("Metro Control")]
        public IEasingFunction NavigationEasingFunction
        {
            get { return (IEasingFunction)GetValue(NavigationEasingFunctionProperty); }
            set { SetValue(NavigationEasingFunctionProperty, value); }
        }

        [Description("Enable mouse drag"), Category("Metro Control")]
        public bool EnableMouseDrag
        {
            get { return (bool)GetValue(EnableMouseDragProperty); }
            set { SetValue(EnableMouseDragProperty, value); }
        }


        [Description("Enable Z Stacking Mode"), Category("Metro Control")]
        public bool EnableZStackMode
        {
            get { return (bool)GetValue(EnableZStackModeProperty); }
            set { SetValue(EnableZStackModeProperty, value); }
        }

        [Description("Z stacking offset"), Category("Metro Control")]
        public Point ZStackingOffset
        {
            get { return (Point)GetValue(ZStackingOffsetProperty); }
            set { SetValue(ZStackingOffsetProperty, value); }
        }

        [Description("When doing dragging, snap to panel"), Category("Metro Control")]
        public bool EnablePanelSnapping
        {
            get { return (bool)GetValue(EnablePanelSnappingProperty); }
            set { SetValue(EnablePanelSnappingProperty, value); }
        }

        [Description("Set the background image for this panel"), Category("Metro Control")]
        public ImageSource BackgroundImage
        {
            get { return (ImageSource)GetValue(BackgroundImageProperty); }
            set { SetValue(BackgroundImageProperty, value); }
        }

        [Description("Set the Background Image scroll factor"), Category("Metro Control")]
        public double BackgroundImageScrollFactor
        {
            get { return (double)GetValue(BackgroundImageScrollFactorProperty); }
            set { SetValue(BackgroundImageScrollFactorProperty, value); }
        }

        #endregion

        private double _childWidth = 0.0;
        private double _childHeight = 0.0;
        private Point _prevPoint;
        private double _prevDelta = 0;
        bool _MouseLeftButtonIsDown = false;
        int _curZOrder = 0;

        FrameworkElement _cursor;
        Image _backgroundImage;
        private MetroPanel _activeChild = null;
        private int _activeChildIdx = 0;

        private int _curRow = 0;
        public int CurRow
        {
            get { return _curRow; }
            set { _curRow = value; }
        }

        private int _curCol = 0;
        public int CurCol
        {
            get { return _curCol; }
            set { _curCol = value; }
        }


        public MetroPanel()
        {
            Loaded += new RoutedEventHandler(MetroPanelLoaded);
            AddHandler(UIElement.KeyDownEvent, new KeyEventHandler(MetroPanelKeyDown), true);

            GotFocus += new RoutedEventHandler(MetroPanelGotFocus);
            LostFocus += new RoutedEventHandler(MetroPanelLostFocus);
        }


        public void MetroPanelLoaded(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("MetroPanel {0} loaded", Name);
            foreach (UIElement child in Children)
            {
                if (child is MetroPanel)
                {
                    _activeChild = child as MetroPanel;
                    _activeChild.SetActive(0, 0);
                    break;
                }
            }

            if (BackgroundImage != null)
            {
                _backgroundImage = new Image();
                _backgroundImage.Source = BackgroundImage;
                _backgroundImage.Stretch = Stretch.UniformToFill;
                _backgroundImage.SetValue(Canvas.ZIndexProperty, -1);
                Children.Add(_backgroundImage);
            }
            
            //This is to handle the mouse drag
            if (EnableMouseDrag)
            {
                AddHandler(UIElement.MouseLeftButtonDownEvent, new MouseButtonEventHandler(MetroPanelMouseLeftButtonDown), true);
                AddHandler(UIElement.MouseLeftButtonUpEvent, new MouseButtonEventHandler(MetroPanelMouseLeftButtonUp), true);
                MouseMove += new MouseEventHandler(MetroPanelMouseMove);
                MouseLeave += new MouseEventHandler(MetroPanelMouseLeave);
                MouseLeave += new MouseEventHandler(MetroPanelMouseEnter);

            }

            _cursor = FindName("Cursor") as FrameworkElement;

            //_viewportWidth = (System.Windows.FrameworkElement)this.Parent.
            //_viewportHeight = ((System.Windows.FrameworkElement)(this.Parent._treeParent)).ActualHeight;
         }

        public void SetActive(int row, int col)
        {
            _curRow = row;
            _curCol = col;

            
            if (_curRow < 0)
                _curRow = 0;
            if (_curRow >= Row)
                _curRow = Row - 1;
            if (_curCol < 0)
                _curCol = 0;
            if (_curCol >= Column)
                _curCol = Column - 1;

            int childIdx = (_curRow * Column) + _curCol;
            if (childIdx >= Children.Count)
                childIdx = Children.Count - 1;
            
            if (Children[childIdx] is Control)
            {
                //Debug.WriteLine("MetroPanel {0}, Child Index {1}", this.Name, childIdx);
                ((Control)Children[childIdx]).Focus();
               

            }
            else if (Children[childIdx] is MetroPanel)
            {
                //Move to this Panel
                //Call activate on this panel
                ((MetroPanel)Children[childIdx]).SetActive(0, 0);
            }

            _activeChildIdx = childIdx;
        }


     

        public void MetroPanelKeyDown(Object sender, KeyEventArgs e)
        {
            //Debug.WriteLine("MetroPanelKeyUp: {0}", e.Key.ToString());
            if (e.Handled)
                return;

            if (e.Key == Key.Up)
            {
                _curRow--;
                if (_curRow < 0)
                {
                    _curRow = 0;
                    return;
                }
            }
            else if (e.Key == Key.Down)
            {
                _curRow++;
                if (_curRow >= Row)
                {
                    _curRow = Row - 1;
                    return;
                }
            }
            else if (e.Key == Key.Left)
            {
                _curCol--;
                if (_curCol < 0)
                {
                    _curCol = 0;
                    return;
                }
            }
            else if (e.Key == Key.Right)
            {
                _curCol++;
                if (_curCol >= Column)
                {
                    _curCol = Column - 1;
                    return;
                }
            }
            else
                return;

            int childIdx = ((_curRow * Column) + _curCol);
            if (childIdx >= Children.Count)
            {
                _curCol = (Children.Count - 1) % Column;
                _curRow = (int)Math.Floor ((Children.Count - 1) / Row) ;
                childIdx = (_curRow * Column) + _curCol;
            }
            else
                e.Handled = true;
            
            if (Children[childIdx] is Control)
            {
                //Debug.WriteLine("Child Index {0}", childIdx);
                ((Control)Children[childIdx]).Focus();
                
            }
            else if (Children[childIdx] is MetroPanel)
            {
                MetroPanel newActivePanel = Children[childIdx] as MetroPanel;
                //Move to this Panel
                //Activate the Panel
                int childRow = 0;
                int childCol = 0;
                if (_activeChild != null)
                {
                    if (e.Key == Key.Up)
                    {
                        childRow = newActivePanel.Row - 1;
                        childCol = _activeChild.CurCol;
                    }
                    else if (e.Key == Key.Down)
                    {
                        childRow = 0;
                        childCol = _activeChild.CurCol;
                    }
                    else if (e.Key == Key.Left)
                    {
                        childRow = _activeChild.CurRow;
                        childCol = newActivePanel.Column - 1;
                    }
                    else if (e.Key == Key.Right)
                    {
                        childRow = _activeChild.CurRow;
                        childCol = 0;
                    }
                }


                if (EnableZStackMode)
                {
                    _curZOrder++;
                    //AnimateZorderChange(_curZOrder);
                    newActivePanel.SetValue(Canvas.ZIndexProperty, _curZOrder);
                    
                }
                else
                {
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        AnimatedScrollTo(ChildrenMargin.Left - newActivePanel.Margin.Left);
                    else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                        AnimatedScrollTo(ChildrenMargin.Top - newActivePanel.Margin.Top);
                }

                newActivePanel.SetActive(childRow, childCol);


                _activeChild = newActivePanel;
            }
        }


        public void MetroPanelMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_MouseLeftButtonIsDown == false)
                return;

            Point pt = e.GetPosition(null);
            //Debug.WriteLine("Point {0}, {1}", pt.X, pt.Y);

            Double delta = 0;

            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
               delta = pt.X - _prevPoint.X;
            else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                delta = pt.Y - _prevPoint.Y;

            _prevPoint = pt;
            _prevDelta = delta;
            //AnimatedScrollBy(delta);
            ScrollBy(delta);
            
        }

        public void MetroPanelMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

            _prevDelta = 0;
        }

        public void MetroPanelMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

            _MouseLeftButtonIsDown = false;

            if (EnablePanelSnapping)
            {
                SnappingPositioning2();
            }
            else if (_prevDelta != 0)
            {
                AnimatedScrollBy(_prevDelta * 8);
            }

            _prevDelta = 0;
        }

        public void MetroPanelMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _prevPoint = e.GetPosition(null);

            //FrameworkElement fe = this as FrameworkElement;
            //fe.CaptureMouse();
            _prevDelta = 0;
            _MouseLeftButtonIsDown = true;
    
        }


        public void MetroPanelMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _MouseLeftButtonIsDown = false;
            //FrameworkElement fe = this as FrameworkElement;
            //fe.ReleaseMouseCapture();


            if (EnablePanelSnapping)
            {
                SnappingPositioning2();
            }
            else if (_prevDelta != 0)
            {
                AnimatedScrollBy(_prevDelta * 8);
            }

            _prevDelta = 0;

        }


        private void SnappingPositioning ()
        {
            // Snapping Animation to Next, Prev or Current child
            // 1. Determine what direction we moved.
            // 2. Determine whether the appropiate edge of the current child's edge has crossed the half point of the screen.
            // 3. If the child's edge has crossed the half way point of the screen and there is another child following then do a snap easing 
            //    animaiton to the next child.
            // 4. If there is no next child then snap back to the current child.


            // Create the TranslateTransform
            if ((RenderTransform == null) || (RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                return;
            }

            TranslateTransform transTransform = RenderTransform as TranslateTransform;

            if (_prevDelta < 0)
            { //We are moving left
                double trailingEdgePosition = 0;
                double treshhold = 0;

                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                {
                    trailingEdgePosition = ActualWidth - _activeChild.Margin.Right + transTransform.X;
                    treshhold = (0.5 * _childWidth);
                }
                else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                {
                    trailingEdgePosition = ActualHeight - _activeChild.Margin.Bottom + transTransform.Y;
                    treshhold = (0.5 * _childHeight);
                }

                if ((trailingEdgePosition < treshhold) && ((_activeChildIdx + 1) < Children.Count))
                {

                    _activeChildIdx++;
                    _activeChild = Children[_activeChildIdx] as MetroPanel;
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        AnimatedScrollTo(ChildrenMargin.Left - _activeChild.Margin.Left);
                    else if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        AnimatedScrollTo(ChildrenMargin.Top - _activeChild.Margin.Top);
                }
                else if (trailingEdgePosition < _childWidth)
                {
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        AnimatedScrollTo(ChildrenMargin.Left - _activeChild.Margin.Left);
                    else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                        AnimatedScrollTo(ChildrenMargin.Top - _activeChild.Margin.Top);
                }

            }
            else if (_prevDelta > 0)
            { //We are moving right
                double leadingEdgePosition = 0;
                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                    leadingEdgePosition = _activeChild.Margin.Left + transTransform.X;
                else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                    leadingEdgePosition = _activeChild.Margin.Top + transTransform.Y;
                if ((leadingEdgePosition > (0.5 * _childWidth)) && ((_activeChildIdx - 1) >= 0))
                {

                    _activeChildIdx--;
                    _activeChild = Children[_activeChildIdx] as MetroPanel;
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        AnimatedScrollTo(ChildrenMargin.Left - _activeChild.Margin.Left);
                    else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                        AnimatedScrollTo(ChildrenMargin.Top - _activeChild.Margin.Top);

                }
                else if (leadingEdgePosition > 0)
                {
                    if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                        AnimatedScrollTo(ChildrenMargin.Left - _activeChild.Margin.Left);
                    else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                        AnimatedScrollTo(ChildrenMargin.Top - _activeChild.Margin.Top);
                }
            }
        }


        private void SnappingPositioning2()
        {

            // Create the TranslateTransform
            if ((RenderTransform == null) || (RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                return;
            }

            TranslateTransform transTransform = RenderTransform as TranslateTransform;

            int newChildIdx = 0;
 
            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                newChildIdx = (int)Math.Abs(Math.Floor(((transTransform.X + (_childWidth / 2)) / _childWidth)));
            }
            else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                newChildIdx = (int)Math.Abs(Math.Floor(((transTransform.Y + (_childHeight / 2)) / _childHeight)));
                
            }

            if (newChildIdx >= Children.Count)
            {
                newChildIdx = Children.Count - 1;
            }

            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                if (newChildIdx >= (Column - 1))
                    newChildIdx = Column -1;

                if (newChildIdx >= Children.Count)
                    newChildIdx = Children.Count - 1;

                AnimatedScrollTo(ChildrenMargin.Left - ((FrameworkElement)Children[newChildIdx]).Margin.Left);
            }
            else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                if (newChildIdx >= (Row - 1))
                    newChildIdx = Row - 1;

                if (newChildIdx >= Children.Count)
                    newChildIdx = Children.Count - 1;

                AnimatedScrollTo(ChildrenMargin.Top - ((FrameworkElement)Children[newChildIdx]).Margin.Top);
            }
        }
        

        // Animates to the appropiate panel
        private void AnimatedScrollTo(double pos)
        {

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
            animation.To = pos;
            animation.By = 10;
            animation.AutoReverse = false;
            animation.EasingFunction = NavigationEasingFunction;


            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            FrameworkElement fe = this as FrameworkElement;

            // Create the TranslateTransform
            if ((fe.RenderTransform == null) || (fe.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                fe.RenderTransform = new TranslateTransform();
            }

            TranslateTransform transTransform = fe.RenderTransform as TranslateTransform;

            Storyboard.SetTarget(animation, transTransform);

            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));
            else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.YProperty));


            TranslateTransform bgImageImageTransform = null;
            if (_backgroundImage != null)
            {
                DoubleAnimation bgImageAnimation = new DoubleAnimation();
                bgImageAnimation.Duration = new Duration(TimeSpan.FromMilliseconds(900));
                bgImageAnimation.To = (BackgroundImageScrollFactor * pos);
                bgImageAnimation.By = 10;
                bgImageAnimation.AutoReverse = false;
                bgImageAnimation.EasingFunction = NavigationEasingFunction;

                storyboard.Children.Add(bgImageAnimation);

                if ((_backgroundImage.RenderTransform == null) || (_backgroundImage.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
                {
                    _backgroundImage.RenderTransform = new TranslateTransform();
                }

                bgImageImageTransform = _backgroundImage.RenderTransform as TranslateTransform;

                Storyboard.SetTarget(bgImageAnimation, bgImageImageTransform);

                if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
                    Storyboard.SetTargetProperty(bgImageAnimation, new PropertyPath(TranslateTransform.XProperty));
                else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
                    Storyboard.SetTargetProperty(bgImageAnimation, new PropertyPath(TranslateTransform.YProperty));

            }

            storyboard.Begin();

        }

        private void ScrollBy(double delta)
        {
            // Create the TranslateTransform
            if ((RenderTransform == null) || (RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                RenderTransform = new TranslateTransform();
            }

            TranslateTransform transTransform = RenderTransform as TranslateTransform;

            TranslateTransform bgImageImageTransform = null;
            if (_backgroundImage != null)
            {
                if ((_backgroundImage.RenderTransform == null) || (_backgroundImage.RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
                {
                    _backgroundImage.RenderTransform = new TranslateTransform();
                }
                bgImageImageTransform = _backgroundImage.RenderTransform as TranslateTransform;
            }

            


            double to = 0;

            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                to = transTransform.X + (1.1 * delta);
                if (to > (_childWidth / 3))
                    to = (_childWidth / 3);
                else if (to < ((_childWidth / 3) - ActualWidth))
                    to = (_childWidth / 3) - ActualWidth;

                transTransform.X = to;

                if (bgImageImageTransform != null)
                {
                    to = bgImageImageTransform.X + (BackgroundImageScrollFactor * delta);
                    bgImageImageTransform.X = to;
                }
            }
            else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                to = transTransform.Y + (1.1 * delta);
                if (to > (_childHeight / 3))
                    to = (_childHeight / 3);
                else if (to < ((_childHeight / 3) - ActualHeight))
                    to = (_childHeight / 3) - ActualHeight;

                transTransform.Y = to;
            }


        } 

        private void AnimatedScrollBy(double delta)
        {

            DoubleAnimation animation = new DoubleAnimation();
            animation.Duration = new Duration(TimeSpan.FromMilliseconds(500));
            
            animation.AutoReverse = false;
            animation.EasingFunction = NavigationEasingFunction;

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(animation);

            // Create the TranslateTransform
            if ((RenderTransform == null) || (RenderTransform.GetType().ToString() != "System.Windows.Media.TranslateTransform"))
            {
                RenderTransform = new TranslateTransform();
            }

            TranslateTransform transTransform = RenderTransform as TranslateTransform;

            if (this.Orientation == System.Windows.Controls.Orientation.Horizontal)
            {
                double toX = transTransform.X + (1.2 * delta);

                if (toX > 0)
                    toX = 0;
                else if (toX < (0 - (ActualWidth - _childWidth)))
                    toX = (0 - (ActualWidth - _childWidth));

                animation.To = toX;

                Storyboard.SetTarget(animation, transTransform);

                Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.XProperty));
            }
            else if (this.Orientation == System.Windows.Controls.Orientation.Vertical)
            {
                double toY = transTransform.Y + (1.2 * delta);

                if (toY > 0)
                    toY = 0;
                else if (toY < (0 - (ActualHeight - _childHeight)))
                    toY = (0 - (ActualHeight - _childHeight));

                animation.To = toY;

                Storyboard.SetTarget(animation, transTransform);

                Storyboard.SetTargetProperty(animation, new PropertyPath(TranslateTransform.YProperty));
            }

            storyboard.Begin();

        }

        
        public void MetroPanelGotFocus(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("MetroPanelGotFocus: {0}", this.Name);
        }

        public void MetroPanelLostFocus(Object sender, RoutedEventArgs e)
        {
            //Debug.WriteLine("MetroPanelLostFocus: {0}", this.Name);
        }

        public static void OnPropertyChanged (DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            MetroPanel pan = obj as MetroPanel;
            pan.InvalidateMeasure();
            
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            double maxWidth = availableSize.Width;
            double maxHeight = availableSize.Height;

            
            if (Double.IsInfinity(availableSize.Width) || Double.IsNaN(availableSize.Width))
                   maxWidth = 640;
            if (Double.IsInfinity(availableSize.Height) || Double.IsNaN(availableSize.Height))
                  maxHeight = 480;


            _childWidth = Math.Ceiling(maxWidth / Column);
            _childHeight = Math.Ceiling(maxHeight / Row);
      
            foreach (UIElement child in this.Children)
            {
                child.Measure(new Size (maxWidth, maxHeight));
            }

            return new Size (maxWidth, maxHeight);
        }



        protected override Size ArrangeOverride(Size finalSize)
        {
            if (this.Children == null || this.Children.Count == 0)
                return finalSize;


            if (EnableZStackMode)
                return ArrangeInStack(finalSize);
            else if (Orientation == Orientation.Horizontal)
                return ArrangeInColumns(finalSize);
            else if (Orientation == Orientation.Vertical)
                return ArrangeInRows(finalSize);
                        
            return finalSize;
        }


        protected Size ArrangeInColumns(Size finalSize)
        {

            int numOfColumns = Column;
            int curColum = 0;

            double maxHeight = 0.0;
            double curX = 0.0;
            double curY = 0.0;
            
            foreach (FrameworkElement child in Children)
            {
                double childWidth = 0.0;
                double childHeight = 0.0;

                if (child == _backgroundImage)
                {
                    child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                    continue;
                }

                if (child.DesiredSize.Width > 0)
                    childWidth = _childWidth; //child.DesiredSize.Width;
                else
                    childWidth = _childWidth;

                if (child.DesiredSize.Height > 0)
                    childHeight = _childHeight;//child.DesiredSize.Height;
                else
                    childHeight = _childHeight;

                if (curColum >= numOfColumns)
                {
                    curY += maxHeight;
                    curX = 0;
                    curColum = 0;
                }

                
                //Debug.WriteLine("{0} rect ({1},{2},{3},{4})", child.ToString(), curX, curY, childWidth, childHeight);
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                child.VerticalAlignment = VerticalAlignment.Stretch;
                child.HorizontalAlignment = HorizontalAlignment.Stretch;
                //child.Margin = new Thickness(curX + ChildrenMargin.Left + ChildrenMargin.Right, curY + ChildrenMargin.Top + ChildrenMargin.Bottom, (finalSize.Width + ChildrenMargin.Left + ChildrenMargin.Right) - (curX + childWidth), (finalSize.Height + ChildrenMargin.Top + ChildrenMargin.Bottom) - (curY + childHeight));
                child.Margin = new Thickness(curX + ChildrenMargin.Left, curY + ChildrenMargin.Top, (finalSize.Width + ChildrenMargin.Right) - (curX + childWidth), (finalSize.Height + ChildrenMargin.Bottom) - (curY + childHeight));
                
                curX += childWidth;
                maxHeight = Math.Max(maxHeight, childHeight);

                curColum++;
               
            }

            curY += maxHeight;

            return new Size(finalSize.Width, finalSize.Height);
        }


        protected Size ArrangeInRows(Size finalSize)
        {

            int numOfRows = Row;
            int curRow = 0;

            double maxWidth = 0.0;
            double curX = 0.0;
            double curY = 0.0;

            foreach (FrameworkElement child in Children)
            {
                double childWidth = 0.0;
                double childHeight = 0.0;

                if (child.DesiredSize.Width > 0)
                    childWidth = _childWidth; //child.DesiredSize.Width;
                else
                    childWidth = _childWidth;

                if (child.DesiredSize.Height > 0)
                    childHeight = _childHeight;//child.DesiredSize.Height;
                else
                    childHeight = _childHeight;

                if (curRow >= numOfRows)
                {
                    curX += maxWidth;
                    curY = 0;
                    curRow = 0;
                }


                //Debug.WriteLine("{0} rect ({1},{2},{3},{4})", child.ToString(), curX, curY, childWidth, childHeight);
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                child.VerticalAlignment = VerticalAlignment.Stretch;
                child.HorizontalAlignment = HorizontalAlignment.Stretch;
                
                child.Margin = new Thickness(curX + ChildrenMargin.Left, curY + ChildrenMargin.Top, (finalSize.Width + ChildrenMargin.Right) - (curX + childWidth), (finalSize.Height + ChildrenMargin.Bottom) - (curY + childHeight));

                curY += childHeight;
                maxWidth = Math.Max(maxWidth, childWidth);

                curRow++;

            }

            curX += maxWidth;

            return new Size(finalSize.Width, finalSize.Height);
        }

        protected Size ArrangeInStack(Size finalSize)
        {

            int numOfColumns = Column;
            int curColum = 0;

            double maxHeight = 0.0;
            double curX = 0.0;
            double curY = 0.0;
            double curYOffset = 0.0;
            int zorder = 0;
            
            foreach (FrameworkElement child in Children)
            {
                double childWidth = 0.0;
                double childHeight = 0.0;

                if (child.DesiredSize.Width > 0)
                    childWidth = _childWidth; //child.DesiredSize.Width;
                else
                    childWidth = _childWidth;

                if (child.DesiredSize.Height > 0)
                    childHeight = _childHeight;//child.DesiredSize.Height;
                else
                    childHeight = _childHeight;

                if (curColum >= numOfColumns)
                {
                    curY += maxHeight;
                    curY += curYOffset;
                    curYOffset = 0;
                    curX = 0;
                    curColum = 0;
                    zorder = 0;
                }

                
                //Debug.WriteLine("{0} rect ({1},{2},{3},{4})", child.ToString(), curX, curY, childWidth, childHeight);
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                child.VerticalAlignment = VerticalAlignment.Stretch;
                child.HorizontalAlignment = HorizontalAlignment.Stretch;
                child.Margin = new Thickness(curX + ChildrenMargin.Left + ZStackingOffset.X, curY + ChildrenMargin.Top + curYOffset, (finalSize.Width + ChildrenMargin.Right + ZStackingOffset.X) - (curX + childWidth), (finalSize.Height + ChildrenMargin.Bottom) - (curY + childHeight + curYOffset));
                child.SetValue(Canvas.ZIndexProperty, zorder);
                zorder--;
                curX += ZStackingOffset.X;
                curYOffset += ZStackingOffset.Y;
                maxHeight = Math.Max(maxHeight, childHeight);

                curColum++;
               
            }

            curY += maxHeight;

            return new Size(finalSize.Width, finalSize.Height);
        }

        protected Size ArrangeInStackUsingProjection(Size finalSize)
        {

            int numOfColumns = Column;
            int curColum = 0;

            double maxHeight = 0.0;
            double curX = 0.0;
            double curY = 0.0;
            int zorder = 0;

            foreach (FrameworkElement child in Children)
            {
                double childWidth = 0.0;
                double childHeight = 0.0;

                if (child.DesiredSize.Width > 0)
                    childWidth = _childWidth; //child.DesiredSize.Width;
                else
                    childWidth = _childWidth;

                if (child.DesiredSize.Height > 0)
                    childHeight = _childHeight;//child.DesiredSize.Height;
                else
                    childHeight = _childHeight;

                if (curColum >= numOfColumns)
                {
                    curY += maxHeight;
                    curX = 0;
                    curColum = 0;
                    zorder = 0;
                }


                //Debug.WriteLine("{0} rect ({1},{2},{3},{4})", child.ToString(), curX, curY, childWidth, childHeight);
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                child.VerticalAlignment = VerticalAlignment.Stretch;
                child.HorizontalAlignment = HorizontalAlignment.Stretch;
                child.Margin = new Thickness(ChildrenMargin.Left, ChildrenMargin.Top, (finalSize.Width + ChildrenMargin.Right) - (childWidth), (finalSize.Height + ChildrenMargin.Bottom) - (curY + childHeight));
                //child.SetValue (Canvas.ZIndexProperty
                //child.Projection = new PlaneProjection (
                zorder--;
                curX += ZStackingOffset.X;
                curY += ZStackingOffset.Y;
                maxHeight = Math.Max(maxHeight, childHeight);

                curColum++;

            }

            curY += maxHeight;

            return new Size(finalSize.Width, finalSize.Height);
        }
     }
}