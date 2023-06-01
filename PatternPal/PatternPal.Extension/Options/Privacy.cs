using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;
using Microsoft.VisualStudio.Shell;
using PatternPal.Extension.Commands;

namespace PatternPal.Extension
{
    internal partial class OptionsProvider
    {
        // Register the options with this attribute on your package class:
        // [ProvideOptionPage(typeof(OptionsProvider.PrivacyOptions), "PatternPal.Extension", "Privacy", 0, 0, true, SupportsProfiles = true)]
        [ComVisible(true)]
        public class PrivacyOptions : BaseOptionPage<Privacy> { }
    }

    public class Privacy : BaseOptionModel<Privacy>
    {
        private bool _doLogData;
        private string _subjectId;
        private bool _firstTime = true;


        [Category("Privacy")]
        [DisplayName("Log data")]
        [Description(
            "Whether PatternPal can log your data. The data which gets logged are your actions and your source code. This is used for research. This option is turned off by default.")]
        public bool DoLogData
        {
            get { return _doLogData; }
            set
            {
                // Set is triggered both when changing the field value, as well as when clicking on the OK button.
                // This prevents the code from being triggered twice.
                if (value == _doLogData) return;

                ThreadHelper.JoinableTaskFactory.Run(async () =>
                {
                    await (value
                        ? SubscribeEvents.SubscribeEventHandlersAsync()
                        : SubscribeEvents.UnsubscribeEventHandlersAsync());
                });

                // The SessionId has to be reset if the option for logging data is changed to prevent logging without a session id. 
                // In other words, a new session has to be started.
                ThreadHelper.ThrowIfNotOnUIThread();
                if (value)
                {
                    SubscribeEvents.OnSessionStart();
                    SubscribeEvents.OnProjectOpen();
                }
                else
                {
                    SubscribeEvents.OnSessionEnd();
                    SubscribeEvents.OnProjectClose();
                }

                _doLogData = value;
            }
        }

        [Category("Privacy")]
        [DisplayName("Subject ID")]
        [Description("The ID of the current subject. This is used for research.")]
        public string SubjectId { get => _subjectId; set => _subjectId = value; }

        [Category("Privacy")]
        [DisplayName("First Time")]
        [Description("Whether this is the first time PatternPal is used. This is used for research.")]
        public bool FirstTime { get => _firstTime; set => _firstTime = value; }
    }
}
