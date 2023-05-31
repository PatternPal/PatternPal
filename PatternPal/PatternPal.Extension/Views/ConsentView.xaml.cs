#region

using System;
using System.Windows;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

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

            consentSubjectID.Text = consentSubjectID.Text.Trim();

            if (consentSubjectID.Text.Length == 0)
            {
                MessageBox.Show("Please enter a valid subject ID");
                return;
            }

            try
            {
                Guid subjectID = Guid.Parse(consentSubjectID.Text);
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter a valid subject ID");
                return;
            }
        }

        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            // close the window
        }
    }
}
