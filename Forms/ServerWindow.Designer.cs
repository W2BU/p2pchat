namespace p2pchat
{
    partial class ServerWindow
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
            serverLogBox = new RichTextBox();
            serverStatusLabel = new Label();
            serverStopButton = new Button();
            SuspendLayout();
            // 
            // serverLogBox
            // 
            serverLogBox.Location = new Point(39, 89);
            serverLogBox.Name = "serverLogBox";
            serverLogBox.ReadOnly = true;
            serverLogBox.Size = new Size(504, 539);
            serverLogBox.TabIndex = 0;
            serverLogBox.Text = "";
            // 
            // serverStatusLabel
            // 
            serverStatusLabel.AutoSize = true;
            serverStatusLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            serverStatusLabel.Location = new Point(240, 30);
            serverStatusLabel.Name = "serverStatusLabel";
            serverStatusLabel.Size = new Size(105, 28);
            serverStatusLabel.TabIndex = 1;
            serverStatusLabel.Text = "Server Log";
            // 
            // serverStopButton
            // 
            serverStopButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            serverStopButton.Location = new Point(39, 661);
            serverStopButton.Margin = new Padding(30, 3, 30, 3);
            serverStopButton.Name = "serverStopButton";
            serverStopButton.Size = new Size(504, 57);
            serverStopButton.TabIndex = 2;
            serverStopButton.Text = "Stop Server";
            serverStopButton.UseVisualStyleBackColor = true;
            serverStopButton.Click += serverStopButton_Click;
            // 
            // ServerWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(582, 753);
            Controls.Add(serverStopButton);
            Controls.Add(serverStatusLabel);
            Controls.Add(serverLogBox);
            Name = "ServerWindow";
            Text = "P2Pchat server";
            FormClosed += ServerWindow_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox serverLogBox;
        private Label serverStatusLabel;
        private Button serverStopButton;
    }
}