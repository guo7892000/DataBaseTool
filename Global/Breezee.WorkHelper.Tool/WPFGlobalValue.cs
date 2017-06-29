using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;

namespace Breezee.WorkHelper.Tool
{
    public class WPFGlobalValue
    {
        public bool IsXBAP
        {
            get
            {
                return BrowserInteropHelper.IsBrowserHosted;
            }

        }
    }
}
