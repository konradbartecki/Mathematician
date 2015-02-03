using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Mathematician.Controls
{
    public partial class GameGauge : UserControl
    {

        /// <summary>
        /// Text on game gauge.
        /// Enter equation here, ex: 512x8, 32/2
        /// </summary>
        //public string GaugeText
        //{
        //    get
        //    {
        //        return GaugeText;
        //    }
        //    set
        //    {
        //        Equation.Text = GaugeText;
        //    }
        //}

        /// <summary>
        /// Integer time on gauge from 0-100
        /// </summary>

        public GameGauge()
        {
              InitializeComponent();
        }
    }
}
