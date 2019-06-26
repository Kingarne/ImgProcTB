namespace ImgProcTB
{
    partial class ControlForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.initButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyGrid.Location = new System.Drawing.Point(12, 8);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(291, 400);
            this.propertyGrid.TabIndex = 2;
            // 
            // initButton
            // 
            this.initButton.Location = new System.Drawing.Point(12, 414);
            this.initButton.Name = "initButton";
            this.initButton.Size = new System.Drawing.Size(75, 23);
            this.initButton.TabIndex = 3;
            this.initButton.Text = "Init";
            this.initButton.UseVisualStyleBackColor = true;
            this.initButton.Click += new System.EventHandler(this.InitButton_Click);
            // 
            // ControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 450);
            this.Controls.Add(this.initButton);
            this.Controls.Add(this.propertyGrid);
            this.Name = "ControlForm";
            this.Text = "ControlForm";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Button initButton;
    }
}