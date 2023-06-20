namespace p2pchat
{
    partial class ChooseMode
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
            serverBtn = new Button();
            clientBtn = new Button();
            SuspendLayout();
            // 
            // serverBtn
            // 
            serverBtn.Location = new Point(29, 41);
            serverBtn.Margin = new Padding(20, 3, 3, 3);
            serverBtn.Name = "serverBtn";
            serverBtn.Size = new Size(232, 65);
            serverBtn.TabIndex = 0;
            serverBtn.Text = "Server";
            serverBtn.UseVisualStyleBackColor = true;
            serverBtn.Click += serverBtn_Click;
            // 
            // clientBtn
            // 
            clientBtn.Location = new Point(321, 41);
            clientBtn.Margin = new Padding(3, 3, 20, 3);
            clientBtn.Name = "clientBtn";
            clientBtn.Size = new Size(232, 65);
            clientBtn.TabIndex = 1;
            clientBtn.Text = "Client";
            clientBtn.UseVisualStyleBackColor = true;
            clientBtn.Click += clientBtn_Click;
            // 
            // ChooseMode
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 153);
            Controls.Add(clientBtn);
            Controls.Add(serverBtn);
            Name = "ChooseMode";
            Text = "Choose operating mode";
            ResumeLayout(false);
        }

        #endregion

        private Button serverBtn;
        private Button clientBtn;
    }
}