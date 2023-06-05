using System.Threading.Tasks;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;

namespace PatternPal.Extension
{
    [Command(PackageIds.ExtensionViewCommand) ]
    internal sealed class PatternPalWindowCommand : BaseCommand<PatternPalWindowCommand>
    {
        protected override async Task ExecuteAsync(OleMenuCmdEventArgs e)
        {
            await ExtensionWindow.ShowAsync();
        }
    }
}
