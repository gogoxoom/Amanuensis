﻿#pragma checksum "C:\Users\jewolfe\Documents\My Dropbox\Pattern Lib\SessionController\SessionController\SessionController\SessionControlPanel.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "AE71A10BC72611765962A5B8EE16FD9B"
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


namespace SessionController {
    
    
    public partial class SessionControlPanel : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.Button StartBlock;
        
        internal System.Windows.Controls.Button StopBlock;
        
        internal System.Windows.Controls.TextBox SessionId;
        
        internal System.Windows.Controls.TextBox Comments;
        
        internal System.Windows.Controls.Button Submit;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/SessionController;component/SessionControlPanel.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.StartBlock = ((System.Windows.Controls.Button)(this.FindName("StartBlock")));
            this.StopBlock = ((System.Windows.Controls.Button)(this.FindName("StopBlock")));
            this.SessionId = ((System.Windows.Controls.TextBox)(this.FindName("SessionId")));
            this.Comments = ((System.Windows.Controls.TextBox)(this.FindName("Comments")));
            this.Submit = ((System.Windows.Controls.Button)(this.FindName("Submit")));
        }
    }
}

