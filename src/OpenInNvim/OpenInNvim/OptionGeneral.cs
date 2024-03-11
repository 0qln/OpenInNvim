using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenInNvim
{
    public class OptionGeneral : DialogPage
    {
        private string _terminalPath = "powershell.exe";

        [Category("General")]
        [DisplayName("Terminal path")]
        [Description("The file path to the executable of the terminal, from which you want to run Neovim.")]
        public string TerminalPath
        {
            get
            {
                return _terminalPath;
            }

            set
            {
                _terminalPath = value;
            }
        }
    }
}
