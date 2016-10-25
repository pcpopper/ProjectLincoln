using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using ProjectLincoln.Helpers.DB;

using ProjectLincoln.Entity;
using ProjectLincoln.Objects;

namespace ProjectLincoln {
    public partial class FormMain : Form {

        public static FormMain frmMain;

        public SQLite localDb { get; private set; }
        public MySQL cloudDb { get; private set; }
        public List<TruckPanel> truckPanels { get; private set; }

        public FormMain () {
            frmMain = this;

            InitializeComponent();

            truckPanels = new List<TruckPanel>();
        }

        #region Truck Panels
        /// <summary>
        /// Gets the location index of the given panel
        /// </summary>
        /// <param name="clazz">The class to check for</param>
        /// <returns>-1 is not found, else the index</returns>
        public int getTruckPanelIndex (TruckPanel clazz) {
            // Default the returned index to -1 (not found)
            int index = -1;

            // Loop through the list
            for (int i = 0; i < truckPanels.Count; i++) {
                // Check if the class instances match
                if (truckPanels[i] == clazz) {
                    // Set the index
                    index = i;
                    break;
                }
            }

            return index;
        }

        public void moveTruckPanel (int oldIndex, int newIndex) {
            TruckPanel tempPanel = truckPanels[newIndex];
            truckPanels[newIndex] = truckPanels[oldIndex];
            truckPanels[oldIndex] = tempPanel;

            tempPanel.moveTruck(oldIndex);
        }
        #endregion

        private void Form1_Load (object sender, EventArgs e) {
            //Hide();
            //Splash splash = new Splash(this); splash.Show();

            truckPanels.AddRange(new List<TruckPanel>() {
                new TruckPanel(new Truck(null, "634176", 5, 55, null, 7)),
                new TruckPanel(new Truck(null, "634177", 5, 55, null, 7)),
                new TruckPanel(new Truck(null, "634178", 5, 55, null, 7))
            });
            for (int i = 0; i < truckPanels.Count; i++) {
                truckPanels[i].panel.Location = new Point(0, 50 * i);
                pnlTruckHolder.Controls.Add(truckPanels[i].panel);
            }
        }

        private void Form1_KeyDown (object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Escape) {
                this.Close();
            } else if (e.KeyCode == Keys.Q) {
                truckPanels[0].addPax(new Person(null, "abc", "First", "M", "Last1", BloodTypes.ABPOS, Genders.M, 3531, 7, null, 1));
                truckPanels[0].addPax(new Person(null, "abc", "First", "M", "Last2", BloodTypes.ABPOS, Genders.M, 3531, 7, null, 1));
            }
        }

        private void pnlTruckHolder_MouseHover (object sender, EventArgs e) {

        }

        private void pnlTruckHolder_MouseDown (object sender, MouseEventArgs e) {

        }
    }
}
