using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace OpenInNvim.Options
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(SettingsPage.PackageGuidString)]
    [ProvideOptionPage(typeof(General), "Open In Neovim", "General", 0, 0, true)]
    [ProvideOptionPage(typeof(Windows), "Open In Neovim", "Windows", 0, 0, true)]
    public sealed class SettingsPage : AsyncPackage
    {
        /// <summary>
        /// OptionsPage GUID string.
        /// </summary>
        public const string PackageGuidString = "41bf1714-efd5-4c00-9596-9ebd3de05c0c";

        private TPage GetPage<TPage>() where TPage : DialogPage => GetDialogPage(typeof(TPage)) as TPage;

        public string TerminalPath => GetPage<General>().TerminalPath;
        public Boolean MinimizeVSWindows => GetPage<Windows>().MinimizeVSWindows;


        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        }

        #endregion
    }
}
