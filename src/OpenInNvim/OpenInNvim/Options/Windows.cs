using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenInNvim.Options
{
    public class Windows : DialogPage
    {
        private bool _minimizeVSWindows = true;

        [Category("Windows")]
        [DisplayName("Minimize visual studio windows")]
        [Description("If this option is enabled, all windows of your currently focused Visual Studio instance will get minimized on every request to open the current document or workspace in Neovim.")]
        public bool MinimizeVSWindows
        {
            get
            {
                return _minimizeVSWindows;
            }
            set
            {
                _minimizeVSWindows = value;
            }
        }
    }
}
