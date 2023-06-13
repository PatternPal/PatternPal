#region

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Community.VisualStudio.Toolkit;

#endregion

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
        [Category("Privacy")]
        [DisplayName("Log data")]
        [Description(
            "Whether PatternPal can log your data. The data which gets logged are your actions and your source code. This is used for research. This option is turned off by default.")]
        public bool DoLogData { get; set; } = false;

        [Category("Privacy")]
        [DisplayName("First Time")]
        [Description("Whether this is the first time PatternPal is used. This is used for research.")]
        public bool FirstTime { get; set; } = true;

        [Category("Privacy")]
        [DisplayName("Subject ID")]
        [Description("The ID of the current subject. This can be customized for research purposes.")]
        public string SubjectId { get; set; } = Guid.Empty.ToString();
    }
}
