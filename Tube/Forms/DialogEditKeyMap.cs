using System.Windows.Forms;

namespace Glue.Forms
{
    public partial class DialogEditKeyMap : Form
    {
        private readonly FormSettingsHandler formSettingsHandler;

        public DialogEditKeyMap()
        {
            InitializeComponent();

            // Attach for form settings persistence
            formSettingsHandler = new FormSettingsHandler(this);
        }
    }
}
