﻿#pragma checksum "C:\Users\jewolfe\Desktop\Pattern Lib\SessionController\SilverlightApplication12\SilverlightApplication12\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "8D1C315660308F68131A14FE0D32DAC5"
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


namespace SilverlightApplication12 {
    
    
    public partial class MainPage : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.TextBox SessionId;
        
        internal System.Windows.Controls.Button StartBlock;
        
        internal System.Windows.Controls.Button StopBlock;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/SilverlightApplication12;component/MainPage.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.SessionId = ((System.Windows.Controls.TextBox)(this.FindName("SessionId")));
            this.StartBlock = ((System.Windows.Controls.Button)(this.FindName("StartBlock")));
            this.StopBlock = ((System.Windows.Controls.Button)(this.FindName("StopBlock")));
        }
    }
}
