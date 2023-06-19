#region

using System;
using System.Runtime.Remoting.Contexts;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell.Settings;
using Microsoft.VisualStudio.Threading;
using PatternPal.Extension.ViewModels;

#endregion

namespace PatternPal.Extension.Views
{
    /// <summary>
    ///     Interaction logic for HomeView.xaml
    /// </summary>
    public partial class ConsentView
    {

        
        public ConsentView()
        {
            InitializeComponent();
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {

            // check subjectID input is not empty and if it is parse it as a GUID
            Guid subjectId = Guid.NewGuid();

            _ = Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ConsentViewModel context = ((ConsentViewModel)DataContext);
                // generate a random GUID
                context.ConfigPrivacy.SubjectId = subjectId.ToString();
                context.ConfigPrivacy.DoLogData = true;
                context.ConfigPrivacy.FirstTime = false;
                await context.ConfigPrivacy.SaveAsync();
                // Fire ICommand to navigate to the HomeView
                context.NavigateHomeCommand.Execute(null);
            });
        }



        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            _ = Task.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                ConsentViewModel context = ((ConsentViewModel)DataContext);
                // generate a random GUID
                context.ConfigPrivacy.SubjectId = Guid.Empty.ToString();
                context.ConfigPrivacy.DoLogData = false;
                context.ConfigPrivacy.FirstTime = false;
                await context.ConfigPrivacy.SaveAsync();
                // Fire ICommand to navigate to the HomeView
                context.NavigateHomeCommand.Execute(null);
            });
        }
    }
}
