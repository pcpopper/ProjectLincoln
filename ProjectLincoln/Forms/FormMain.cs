using System;
using System.Windows.Forms;

using ProjectLincoln.Helpers.DB;
using ProjectLincoln.Forms;

namespace ProjectLincoln {
    public partial class FormMain : Form {
        public SQLite localDb = null;
        public MySQL cloudDb = null;

        public FormMain () {
            InitializeComponent();
        }

        private void Form1_Load (object sender, EventArgs e) {
            //Hide();
            //Splash splash = new Splash(this); splash.Show();
        }

        public void initializeClass (params Object[] args) {

        }

        private void newToolStripMenuItem_Click (object sender, EventArgs e) {
            Manifest manifest = new Manifest();
            manifest.MdiParent = this;
            manifest.Show();
        }

        private void arrangeIconsToolStripMenuItem_Click (object sender, EventArgs e) {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void cascadeToolStripMenuItem_Click (object sender, EventArgs e) {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void tileHorizontallyToolStripMenuItem_Click (object sender, EventArgs e) {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void tileVerticallyToolStripMenuItem_Click (object sender, EventArgs e) {
            LayoutMdi(MdiLayout.TileVertical);
        }
    }
}
