namespace IA
{
    partial class IA_Userpanel
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
            this.Console_Send = new System.Windows.Forms.Button();
            this.Console_Inputfield = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Console_Send
            // 
            this.Console_Send.Location = new System.Drawing.Point(434, 140);
            this.Console_Send.Name = "Console_Send";
            this.Console_Send.Size = new System.Drawing.Size(114, 23);
            this.Console_Send.TabIndex = 0;
            this.Console_Send.Text = "Send";
            this.Console_Send.UseVisualStyleBackColor = true;
            this.Console_Send.Click += new System.EventHandler(this.Console_Send_Click);
            // 
            // Console_Inputfield
            // 
            this.Console_Inputfield.Location = new System.Drawing.Point(12, 142);
            this.Console_Inputfield.Name = "Console_Inputfield";
            this.Console_Inputfield.Size = new System.Drawing.Size(416, 20);
            this.Console_Inputfield.TabIndex = 1;
            // 
            // IA_Userpanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(560, 175);
            this.Controls.Add(this.Console_Inputfield);
            this.Controls.Add(this.Console_Send);
            this.Name = "IA_Userpanel";
            this.Text = "IA_Userpanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Console_Send;
        private System.Windows.Forms.TextBox Console_Inputfield;
    }
}