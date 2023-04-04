#region

using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;

using Microsoft.VisualStudio.Shell;

#endregion

namespace PatternPal.Extension.Commands
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed class ExtensionWindowCommand
    {
        private const int CommandId = 0x0100;

        private static readonly Guid CommandSetGUID = new Guid("5eb4c6e9-8015-4fa4-8670-f71284492188");

        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtensionWindowCommand" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private ExtensionWindowCommand(
            AsyncPackage package,
            OleMenuCommandService commandService)
        {
            this._package = package ?? throw new ArgumentNullException(nameof( package ));
            commandService = commandService ?? throw new ArgumentNullException(nameof( commandService ));

            CommandID menuCommandId = new CommandID(
                CommandSetGUID,
                CommandId);
            MenuCommand menuItem = new MenuCommand(
                Execute,
                menuCommandId);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static ExtensionWindowCommand Instance { get; private set; }

        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(
            AsyncPackage package)
        {
            // Switch to the main thread - the call to AddCommand in ExtensionWindowCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof( IMenuCommandService )) as OleMenuCommandService;
            Instance = new ExtensionWindowCommand(
                package,
                commandService);
        }

        /// <summary>
        ///     Shows the tool window when the menu item is clicked.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event args.</param>
        private void Execute(
            object sender,
            EventArgs e)
        {
            _package.JoinableTaskFactory.Run(
                async () =>
                {
                    ToolWindowPane window = await _package.ShowToolWindowAsync(
                        typeof( ExtensionWindow ),
                        0,
                        true,
                        _package.DisposalToken);
                    if (window?.Frame == null)
                    {
                        throw new NotSupportedException("Cannot create tool window");
                    }
                });
        }
    }
}
