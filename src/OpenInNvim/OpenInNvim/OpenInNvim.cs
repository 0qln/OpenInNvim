using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Process = System.Diagnostics.Process;
using Task = System.Threading.Tasks.Task;
using System.Windows.Forms;
using System.IO;
using Microsoft.VisualStudio.Threading;

namespace OpenInNvim
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class OpenInNvim
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("c0e4ae84-0953-4f9a-bdde-5f9fd1527b13");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenInNvim"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private OpenInNvim(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new MenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OpenInNvim Instance
        {
            get;
            private set;
        }

        private static DTE dte { get; set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in OpenInNvim's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            Instance = new OpenInNvim(package, commandService);
            dte = await package.GetServiceAsync(typeof(DTE)) as DTE;

        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            var options = package.GetDialogPage(typeof(OptionGeneral)) as OptionGeneral;

            string target = $"{dte?.ActiveDocument?.FullName}" ?? ".";
            // Parsing in `nvim -- "file_name_with_spaces"` confuses the `Arguments` Parameter of the ProcessStartInfo. Thus we let the start args generate the parameter at runtime at store it temporarily in a file.
            File.WriteAllText("neovim_targets.txt", target);
            
            Process terminal = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = options.TerminalPath,
                    WorkingDirectory = Directory.GetParent(dte?.Solution?.FileName ?? "C:\\").FullName,
                    Arguments = "Get-Content neovim_targets.txt | % { nvim $_ }"
                }
            };

            terminal.Start();
            terminal.WaitForExit();
        }

    }
}
