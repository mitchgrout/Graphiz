namespace Graphiz
{
    partial class FormGraphInfo
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxOrder = new System.Windows.Forms.TextBox();
            this.textBoxSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxDegreeSeq = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxVertices = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.richTextBoxEdges = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Order";
            // 
            // textBoxOrder
            // 
            this.textBoxOrder.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxOrder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxOrder.Location = new System.Drawing.Point(99, 11);
            this.textBoxOrder.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxOrder.Name = "textBoxOrder";
            this.textBoxOrder.ReadOnly = true;
            this.textBoxOrder.Size = new System.Drawing.Size(264, 22);
            this.textBoxOrder.TabIndex = 1;
            // 
            // textBoxSize
            // 
            this.textBoxSize.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSize.Location = new System.Drawing.Point(99, 43);
            this.textBoxSize.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxSize.Name = "textBoxSize";
            this.textBoxSize.ReadOnly = true;
            this.textBoxSize.Size = new System.Drawing.Size(264, 22);
            this.textBoxSize.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 43);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 75);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 32);
            this.label3.TabIndex = 4;
            this.label3.Text = "Degree\r\nSequence";
            // 
            // textBoxDegreeSeq
            // 
            this.textBoxDegreeSeq.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxDegreeSeq.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxDegreeSeq.Location = new System.Drawing.Point(99, 80);
            this.textBoxDegreeSeq.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxDegreeSeq.Name = "textBoxDegreeSeq";
            this.textBoxDegreeSeq.ReadOnly = true;
            this.textBoxDegreeSeq.Size = new System.Drawing.Size(264, 22);
            this.textBoxDegreeSeq.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 118);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 16);
            this.label4.TabIndex = 6;
            this.label4.Text = "Vertex Set";
            // 
            // textBoxVertices
            // 
            this.textBoxVertices.BackColor = System.Drawing.SystemColors.Control;
            this.textBoxVertices.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxVertices.Location = new System.Drawing.Point(99, 118);
            this.textBoxVertices.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.textBoxVertices.Name = "textBoxVertices";
            this.textBoxVertices.ReadOnly = true;
            this.textBoxVertices.Size = new System.Drawing.Size(264, 22);
            this.textBoxVertices.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 150);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(64, 16);
            this.label5.TabIndex = 8;
            this.label5.Text = "Edge Set";
            // 
            // richTextBoxEdges
            // 
            this.richTextBoxEdges.BackColor = System.Drawing.SystemColors.Control;
            this.richTextBoxEdges.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBoxEdges.Location = new System.Drawing.Point(99, 150);
            this.richTextBoxEdges.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.richTextBoxEdges.Name = "richTextBoxEdges";
            this.richTextBoxEdges.ReadOnly = true;
            this.richTextBoxEdges.Size = new System.Drawing.Size(264, 123);
            this.richTextBoxEdges.TabIndex = 9;
            this.richTextBoxEdges.Text = "";
            // 
            // FormGraphInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 283);
            this.Controls.Add(this.richTextBoxEdges);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxVertices);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxDegreeSeq);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxOrder);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "FormGraphInfo";
            this.Text = "Graph Information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxOrder;
        private System.Windows.Forms.TextBox textBoxSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxDegreeSeq;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxVertices;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RichTextBox richTextBoxEdges;
    }
}