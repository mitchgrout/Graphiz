namespace Graphiz
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.panelRender = new System.Windows.Forms.Panel();
            this.toolStripMain = new System.Windows.Forms.ToolStrip();
            this.buttonPointer = new System.Windows.Forms.ToolStripButton();
            this.buttonVertices = new System.Windows.Forms.ToolStripButton();
            this.buttonEdges = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonClear = new System.Windows.Forms.ToolStripButton();
            this.buttonComplement = new System.Windows.Forms.ToolStripButton();
            this.buttonColour = new System.Windows.Forms.ToolStripButton();
            this.toolStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelRender
            // 
            this.panelRender.BackColor = System.Drawing.SystemColors.ControlLight;
            this.panelRender.Location = new System.Drawing.Point(0, 25);
            this.panelRender.Name = "panelRender";
            this.panelRender.Size = new System.Drawing.Size(784, 536);
            this.panelRender.TabIndex = 0;
            this.panelRender.Paint += new System.Windows.Forms.PaintEventHandler(this.panelRender_Paint);
            this.panelRender.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelRender_MouseDown);
            this.panelRender.MouseHover += new System.EventHandler(this.panelRender_MouseHover);
            this.panelRender.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelRender_MouseMove);
            this.panelRender.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelRender_MouseUp);
            // 
            // toolStripMain
            // 
            this.toolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonPointer,
            this.buttonVertices,
            this.buttonEdges,
            this.toolStripSeparator1,
            this.buttonClear,
            this.buttonComplement,
            this.buttonColour});
            this.toolStripMain.Location = new System.Drawing.Point(0, 0);
            this.toolStripMain.Name = "toolStripMain";
            this.toolStripMain.Size = new System.Drawing.Size(784, 25);
            this.toolStripMain.TabIndex = 1;
            this.toolStripMain.Text = "Tool Strip";
            // 
            // buttonPointer
            // 
            this.buttonPointer.CheckOnClick = true;
            this.buttonPointer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonPointer.Image = ((System.Drawing.Image)(resources.GetObject("buttonPointer.Image")));
            this.buttonPointer.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonPointer.Name = "buttonPointer";
            this.buttonPointer.Size = new System.Drawing.Size(49, 22);
            this.buttonPointer.Text = "Pointer";
            this.buttonPointer.Click += new System.EventHandler(this.buttonPointer_Click);
            // 
            // buttonVertices
            // 
            this.buttonVertices.CheckOnClick = true;
            this.buttonVertices.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonVertices.Image = ((System.Drawing.Image)(resources.GetObject("buttonVertices.Image")));
            this.buttonVertices.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonVertices.Name = "buttonVertices";
            this.buttonVertices.Size = new System.Drawing.Size(52, 22);
            this.buttonVertices.Text = "Vertices";
            this.buttonVertices.Click += new System.EventHandler(this.buttonVertices_Click);
            // 
            // buttonEdges
            // 
            this.buttonEdges.CheckOnClick = true;
            this.buttonEdges.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonEdges.Image = ((System.Drawing.Image)(resources.GetObject("buttonEdges.Image")));
            this.buttonEdges.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonEdges.Name = "buttonEdges";
            this.buttonEdges.Size = new System.Drawing.Size(42, 22);
            this.buttonEdges.Text = "Edges";
            this.buttonEdges.Click += new System.EventHandler(this.buttonEdges_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // buttonClear
            // 
            this.buttonClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonClear.Image = ((System.Drawing.Image)(resources.GetObject("buttonClear.Image")));
            this.buttonClear.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(38, 22);
            this.buttonClear.Text = "Clear";
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // buttonComplement
            // 
            this.buttonComplement.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonComplement.Image = ((System.Drawing.Image)(resources.GetObject("buttonComplement.Image")));
            this.buttonComplement.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonComplement.Name = "buttonComplement";
            this.buttonComplement.Size = new System.Drawing.Size(81, 22);
            this.buttonComplement.Text = "Complement";
            this.buttonComplement.Click += new System.EventHandler(this.buttonComplement_Click);
            // 
            // buttonColour
            // 
            this.buttonColour.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonColour.Image = ((System.Drawing.Image)(resources.GetObject("buttonColour.Image")));
            this.buttonColour.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonColour.Name = "buttonColour";
            this.buttonColour.Size = new System.Drawing.Size(47, 22);
            this.buttonColour.Text = "Colour";
            this.buttonColour.Click += new System.EventHandler(this.buttonColour_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.toolStripMain);
            this.Controls.Add(this.panelRender);
            this.Name = "FormMain";
            this.Text = "Graphiz";
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.toolStripMain.ResumeLayout(false);
            this.toolStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panelRender;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton buttonPointer;
        private System.Windows.Forms.ToolStripButton buttonVertices;
        private System.Windows.Forms.ToolStripButton buttonEdges;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton buttonClear;
        private System.Windows.Forms.ToolStripButton buttonComplement;
        private System.Windows.Forms.ToolStripButton buttonColour;
    }
}

