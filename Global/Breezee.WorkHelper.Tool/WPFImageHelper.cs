using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Breezee.WorkHelper.Tool
{
    public class WPFImageHelper
    {
        public static ImageBrush GetImage(string sPath)
        {
            return new ImageBrush(new BitmapImage(new Uri(sPath, UriKind.Relative)));
        }
    }

   
}
