﻿#pragma checksum "C:\Users\rafael\Documents\Expression\Blend 4 Beta\Projects\TabletScreenGrid\MetroPanelTestWithClipping\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "967A50E2FF1D13ABBA02313D541F3995"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;
using TileSL;


namespace MetroPanelTestWithClipping {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal TileSL.MetroPanel LayoutRoot;
        
        internal System.Windows.VisualStateGroup ShowAppVisualStateGroup;
        
        internal System.Windows.VisualState NavigationState;
        
        internal System.Windows.VisualState GridApplicationState;
        
        internal System.Windows.VisualState StackApplicationState;
        
        internal System.Windows.VisualStateGroup StackModeVisualStateGroup;
        
        internal System.Windows.VisualState StackModeNavigationState;
        
        internal System.Windows.VisualState StackModeApplicationState;
        
        internal TileSL.MetroPanel metroPanel;
        
        internal TileSL.Tile tile1;
        
        internal TileSL.Tile tile;
        
        internal Microsoft.Expression.Interactivity.Core.GoToStateAction GridMode_GoToStateAction;
        
        internal TileSL.MetroPanel metroPanel1;
        
        internal TileSL.MetroPanel metroPanel2;
        
        internal System.Windows.Controls.Grid grid;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/MetroPanelTestWithClipping;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((TileSL.MetroPanel)(this.FindName("LayoutRoot")));
            this.ShowAppVisualStateGroup = ((System.Windows.VisualStateGroup)(this.FindName("ShowAppVisualStateGroup")));
            this.NavigationState = ((System.Windows.VisualState)(this.FindName("NavigationState")));
            this.GridApplicationState = ((System.Windows.VisualState)(this.FindName("GridApplicationState")));
            this.StackApplicationState = ((System.Windows.VisualState)(this.FindName("StackApplicationState")));
            this.StackModeVisualStateGroup = ((System.Windows.VisualStateGroup)(this.FindName("StackModeVisualStateGroup")));
            this.StackModeNavigationState = ((System.Windows.VisualState)(this.FindName("StackModeNavigationState")));
            this.StackModeApplicationState = ((System.Windows.VisualState)(this.FindName("StackModeApplicationState")));
            this.metroPanel = ((TileSL.MetroPanel)(this.FindName("metroPanel")));
            this.tile1 = ((TileSL.Tile)(this.FindName("tile1")));
            this.tile = ((TileSL.Tile)(this.FindName("tile")));
            this.GridMode_GoToStateAction = ((Microsoft.Expression.Interactivity.Core.GoToStateAction)(this.FindName("GridMode_GoToStateAction")));
            this.metroPanel1 = ((TileSL.MetroPanel)(this.FindName("metroPanel1")));
            this.metroPanel2 = ((TileSL.MetroPanel)(this.FindName("metroPanel2")));
            this.grid = ((System.Windows.Controls.Grid)(this.FindName("grid")));
        }
    }
}

