using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ProjectLincoln.Entity;
using ProjectLincoln.Services;

namespace ProjectLincoln.Objects {
    public class TruckPanel {

        #region Properties
        public Truck truck { get; private set; }
        public Tamcn truckTamcn { get; private set; }
        public Trailer trailer { get; private set; }
        public Panel panel { get; private set; }
        public List<Person> pax { get; private set; }
        public Dictionary<String, Attachment> attachments { get; private set; }
        public Weapon weapon { get; private set; }

        private FormMain frmMain;
        #endregion

        #region Draggable
        private bool isDragging = false;
        private Point dragStartLocation;
        private int dragStartIndex;
        private int dragLastTop;
        private int dragLastIndex;
        private Color dragColor = SystemColors.Highlight;
        private float movementTime = 0;
        private float movementIncrements;
        private bool previewMove = true;
        #endregion

        #region UI
        private PictureBox pbHandle, pbIcon;
        private Label lblSerialText, lblSerial,
            lblNomenText, lblNomen,
            lblPaxText, lblPax,
            lblTrailerText, lblTrailer,
            lblWeaponText, lblWeapon,
            lblBFTText, lblBFT,
            lblDGGRText, lblDGGR,
            lblComText, lblCom;
        #endregion

        #region UI Settings
        // rows and cols
        private static int[] rows = new int[] { 5, 28 };
        private static int[,] cols = new int[,] {
            { 70, 145 },
            { 310, 355 },
            { 500, 560 },
            { 730, 780 }
        };

        // panel, handle, and icon
        private static Size panelDimensions = new Size(1000, 50);
        private static Size handleSize = new Size(30, 20);
        private static Point handleLocation = new Point(-7, 15);
        private static Size iconDimensions = new Size(50, 50);
        private static Point iconLocation = new Point(20, 0);

        // labels
        private static FontFamily fontFamily = FontFamily.GenericSerif;
        private static FontStyle boldFont = FontStyle.Bold;
        private static float bigFontSize = 18;
        private static float regularFontSize = 12;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="truck">The truck dao</param>
        public TruckPanel (Truck truck) {
            this.truck = truck;
            pax = new List<Person>();

            // Create and preload the attachments dictionary
            attachments = new Dictionary<string, Attachment>() {
                { "weapon-optics", null },
                { "com", null },
                { "bft", null },
                { "dggr", null }
            };

            // Get the tamcn information
            truckTamcn = TamcnService.retrieveTamcn(truck.TamcnId);

            // Get FormMain's instance
            frmMain = FormMain.frmMain;

            // Create the timer
            Timer tmrMove = new Timer();
            tmrMove.Interval = 100;
            tmrMove.Tick += Timer_Tick;

            // Create the panel
            doInitPanel();
        }

        #region PAX management
        /// <summary>
        /// Adds a person to the truck
        /// </summary>
        /// <param name="pack">The person dao to add</param>
        /// <returns>The add status</returns>
        public bool addPax (Person pack) {
            bool addStatus = false;

            // Check if the truck is full
            if ((pax.Count + 1) <= truck.PAX) {
                pax.Add(pack);
                addStatus = true;
            }

            // Update the UI
            setPaxValue();

            return addStatus;
        }

        /// <summary>
        /// Updates the UI with the personnel information
        /// </summary>
        private void setPaxValue () {
            // Set the ?/? label
            lblPax.Text = pax.Count.ToString() + "/" + truck.PAX.ToString();

            // Set the tooltip for said label
            string toolTip = "";
            for (int i = 0;i < pax.Count;i++) {
                // Add the first part of each line
                switch (i) {
                    case 0:
                        toolTip += "Driver: ";
                        break;
                    case 1:
                        toolTip += "A-Driver: ";
                        break;
                    default:
                        toolTip += "Passenger: ";
                        break;
                }

                // Add the name
                toolTip += string.Format("{0}, {1} {2}",
                    pax[i].LastName,
                    pax[i].FirstName, 
                    (pax[i].MiddleInitial != null) ? pax[i].MiddleInitial + "." : "");

                // Add a new line to the string if not the last name
                if (i != (pax.Count - 1)) { toolTip += "\n"; }
            }

            // Update the tooltip with the new text
            FormMain.frmMain.ttPax.SetToolTip(lblPax, toolTip);
        }
        #endregion

        #region Weapon
        #endregion

        #region Attachments
        #endregion

        /// <summary>
        /// Creates the panel for the truck
        /// </summary>
        private void doInitPanel () {
            panel = new Panel();
            panel.Size = panelDimensions;
            panel.BorderStyle = BorderStyle.FixedSingle;

            // Handle
            pbHandle = new PictureBox();
            pbHandle.ImageLocation = @"Resources\Images\handle.png";
            pbHandle.Size = handleSize;
            pbHandle.Location = handleLocation;
            pbHandle.SizeMode = PictureBoxSizeMode.Zoom;
            pbHandle.BackColor = Color.Transparent;
            pbHandle.Cursor = Cursors.SizeAll;
            pbHandle.MouseDown += new MouseEventHandler(this.PanelTruck_MouseDown);
            pbHandle.MouseMove += new MouseEventHandler(this.PanelTruck_MouseMove);
            pbHandle.MouseUp += new MouseEventHandler(this.PanelTruck_MouseUp);

            // Icon
            pbIcon = new PictureBox();
            pbIcon.ImageLocation = @"Resources\Images\" + truckTamcn.ImageName;
            pbIcon.Size = iconDimensions;
            pbIcon.Location = iconLocation;
            pbIcon.SizeMode = PictureBoxSizeMode.Zoom;
            pbIcon.BackColor = Color.Transparent;

            // Serial
            lblSerialText = new Label();
            lblSerialText.Text = "Serial:";
            lblSerialText.Font = new Font(fontFamily, bigFontSize, boldFont);
            lblSerialText.Location = new Point(cols[0,0], (rows[0] - 2));
            lblSerial = new Label();
            lblSerial.Text = truck.TruckSerial;
            lblSerial.Font = new Font(fontFamily, bigFontSize, boldFont);
            lblSerial.Location = new Point(cols[0, 1], (rows[0] - 2));
            lblSerial.AutoSize = true;

            // Nomen
            lblNomenText = new Label();
            lblNomenText.Text = "Nomen:";
            lblNomenText.Font = new Font(fontFamily, regularFontSize);
            lblNomenText.Location = new Point(cols[0, 0], rows[1]);
            lblNomen = new Label();
            lblNomen.Text = truckTamcn.ShortDesc;
            lblNomen.Font = new Font(fontFamily, regularFontSize);
            lblNomen.Location = new Point(cols[0, 1], rows[1]);
            lblNomen.AutoSize = true;

            // PAX
            lblPaxText = new Label();
            lblPaxText.Text = "PAX:";
            lblPaxText.Font = new Font(fontFamily, regularFontSize);
            lblPaxText.Location = new Point(cols[1, 0], rows[0]);
            lblPax = new Label();
            lblPax.Font = new Font(fontFamily, regularFontSize);
            lblPax.Location = new Point(cols[1, 1], rows[0]);
            lblPax.AutoSize = true;

            // Trailer
            lblTrailerText = new Label();
            lblTrailerText.Text = "Trailer:";
            lblTrailerText.Font = new Font(fontFamily, regularFontSize);
            lblTrailerText.Location = new Point(cols[1, 0], rows[1]);
            lblTrailer = new Label();
            lblTrailer.Text = "M149: 1234";
            lblTrailer.Font = new Font(fontFamily, regularFontSize);
            lblTrailer.Location = new Point(cols[1, 1], rows[1]);
            lblTrailer.AutoSize = true;

            // Weapon
            lblWeaponText = new Label();
            lblWeaponText.Text = "Weapon:";
            lblWeaponText.Font = new Font(fontFamily, regularFontSize);
            lblWeaponText.Location = new Point(cols[2, 0], rows[0]);
            lblWeapon = new Label();
            lblWeapon.Text = "MK-19: 12345";
            lblWeapon.Font = new Font(fontFamily, regularFontSize);
            lblWeapon.Location = new Point(cols[2, 1], rows[0]);
            lblWeapon.AutoSize = true;

            // COMM
            lblComText = new Label();
            lblComText.Text = "Comm:";
            lblComText.Font = new Font(fontFamily, regularFontSize);
            lblComText.Location = new Point(cols[2, 0], rows[1]);
            lblCom = new Label();
            lblCom.Text = "PRK-143: 123456";
            lblCom.Font = new Font(fontFamily, regularFontSize);
            lblCom.Location = new Point(cols[2, 1], rows[1]);
            lblCom.AutoSize = true;

            // BFT
            lblBFTText = new Label();
            lblBFTText.Text = "BFT:";
            lblBFTText.Font = new Font(fontFamily, regularFontSize);
            lblBFTText.Location = new Point(cols[3, 0], rows[0]);
            lblBFT = new Label();
            lblBFT.Text = "MEF-MLG-CLB4-A-COC";
            lblBFT.Font = new Font(fontFamily, regularFontSize);
            lblBFT.Location = new Point(cols[3, 1], rows[0]);
            lblBFT.AutoSize = true;

            // DGGR
            lblDGGRText = new Label();
            lblDGGRText.Text = "DAGR:";
            lblDGGRText.Font = new Font(fontFamily, regularFontSize);
            lblDGGRText.Location = new Point(cols[3, 0], rows[1]);
            lblDGGR = new Label();
            lblDGGR.Text = "123456";
            lblDGGR.Font = new Font(fontFamily, regularFontSize);
            lblDGGR.Location = new Point(cols[3, 1], rows[1]);
            lblDGGR.AutoSize = true;

            // Add the controls
            panel.Controls.AddRange(new Control[] {
                lblDGGR,
                lblDGGRText,
                lblBFT,
                lblBFTText,
                lblCom,
                lblComText,
                lblWeapon,
                lblWeaponText,
                lblTrailer,
                lblTrailerText,
                lblPax,
                lblPaxText,
                lblNomen,
                lblNomenText,
                lblSerial,
                lblSerialText,
                pbIcon,
                pbHandle
            });

            // Load the images and text
            pbHandle.Load();
            pbIcon.Load();
            setPaxValue();
        }

        #region Panel Movement
        public void moveTruck (int newIndex) {
            // Calculate the new final location
            int finalLocation = panelDimensions.Height * newIndex;

            // Check if this is going to be moved in increments
            if (movementTime > 0) {

            } else {
                panel.Top = finalLocation;
            }
        }

        private void Timer_Tick (object sender, EventArgs e) {

        }

        /// <summary>
        /// Starts the movement of the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelTruck_MouseDown (object sender, MouseEventArgs e) {
            // Check which mouse buton is pressed
            if (e.Button == MouseButtons.Left) {
                // Set the current location of the panel in case it gets moved
                dragStartLocation = e.Location;
            }
        }

        /// <summary>
        /// Moves the panel with to the proper slot
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelTruck_MouseMove (object sender, MouseEventArgs e) {
            // Ensure that the left mouse button is down when dragging
            if (e.Button == MouseButtons.Left) {
                #region First execution
                // Check to see if this is the first execution of this method
                if (!isDragging) {
                    // Show that the panel is being drug
                    isDragging = true;

                    // Get this panel's list index
                    dragStartIndex = dragLastIndex = frmMain.getTruckPanelIndex(this);

                    // Change the background color
                    panel.BackColor = dragColor;

                    // Bring to the front, on top of all other controls
                    panel.BringToFront();
                    panel.Invalidate();
                }
                #endregion

                // Calculate the new y position
                int y = panel.Top + handleLocation.Y + e.Y - dragStartLocation.Y;

                // Ensure that the panel stays inside the bounds
                if (y >= 0 && y <= ((frmMain.truckPanels.Count * panelDimensions.Height) - panelDimensions.Height)) {
                    // Update the index
                    int newIndex = setNewTruckPanelIndex(y);

                    if (previewMove) {
                        // Set the new (or same) y location
                        //panel.Top = y;
                    } else {
                        // Set the new (or same) y location
                        //panel.Top = panelDimensions.Height * newIndex;
                    }

                    // Move the other panels if the index has just changed
                    if (newIndex != dragLastIndex) {
                        frmMain.label1.Text = string.Format("old {0} new {1}", dragLastIndex, newIndex);

                        // Rearange the panels
                        //frmMain.moveTruckPanel(dragLastIndex, newIndex);

                        // Update the index
                        //dragLastIndex = newIndex;
                    }

                    // Set the last top to the new top
                    dragLastTop = y;
                }
            }
        }

        /// <summary>
        /// Finishes the movement of the panel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelTruck_MouseUp (object sender, MouseEventArgs e) {
            // Check if the left mouse button is down and if the panel is being drug
            if (e.Button == MouseButtons.Left && isDragging) {
                // Set the bool to false to signify that it is no longer being drug
                isDragging = false;

                // Reorganize the indices of the truck panel list

                // Reset the panel's background color
                panel.BackColor = SystemColors.Control;
            }
        }

        /// <summary>
        /// Sets the new index of the dragged panel
        /// </summary>
        /// <param name="y">The new y location of the panel</param>
        /// <returns>The new (or same) index</returns>
        private int setNewTruckPanelIndex (int y) {
            int newIndex;

            // Shortcuts
            int h = panelDimensions.Height;
            float h2 = (h / 2) + handleLocation.Y;

            // Check which way the panel is being drug
            if (y < dragLastTop) {
                // Move the panel up one if newly above halfway mark
                newIndex = (((dragLastTop % h) >= h2) && ((y % h) < h2)) ? --dragLastIndex : dragLastIndex;
            } else {
                // Move the panel down one if newly below halfway mark
                newIndex = (((dragLastTop % h) <= h2) && ((y % h) > h2)) ? ++dragLastIndex : dragLastIndex;
            }

            frmMain.label1.Text = string.Format("y {0} la {1} d {2}", y, dragLastTop, (y < dragLastTop));

            return newIndex;
        }
        #endregion
    }
}
