using System.Windows.Forms;

namespace Glue.Forms
{
    public partial class DialogEditTriggers : Form
    {
        private readonly FormSettingsHandler formSettingsHandler;

        public DialogEditTriggers()
        {
            InitializeComponent();

            // Attach for form settings persistence
            formSettingsHandler = new FormSettingsHandler(this);
        }
    }
}
