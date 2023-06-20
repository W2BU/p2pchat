namespace p2pchat.Forms
{
    partial class ChatWindow
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
            chatWithLabel = new Label();
            messageBox = new TextBox();
            sendMessageButton = new Button();
            dialogueBox = new RichTextBox();
            SuspendLayout();
            // 
            // chatWithLabel
            // 
            chatWithLabel.AutoSize = true;
            chatWithLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            chatWithLabel.Location = new Point(251, 24);
            chatWithLabel.Name = "chatWithLabel";
            chatWithLabel.Size = new Size(94, 28);
            chatWithLabel.TabIndex = 0;
            chatWithLabel.Text = "Chat with";
            // 
            // messageBox
            // 
            messageBox.Location = new Point(12, 618);
            messageBox.Name = "messageBox";
            messageBox.Size = new Size(411, 27);
            messageBox.TabIndex = 2;
            // 
            // sendMessageButton
            // 
            sendMessageButton.Location = new Point(429, 585);
            sendMessageButton.Name = "sendMessageButton";
            sendMessageButton.Size = new Size(146, 93);
            sendMessageButton.TabIndex = 3;
            sendMessageButton.Text = "Send message";
            sendMessageButton.UseVisualStyleBackColor = true;
            sendMessageButton.Click += sendMessageButton_Click;
            // 
            // dialogueBox
            // 
            dialogueBox.Location = new Point(12, 64);
            dialogueBox.Name = "dialogueBox";
            dialogueBox.ReadOnly = true;
            dialogueBox.Size = new Size(563, 515);
            dialogueBox.TabIndex = 4;
            dialogueBox.Text = "";
            // 
            // ChatWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(587, 690);
            Controls.Add(dialogueBox);
            Controls.Add(sendMessageButton);
            Controls.Add(messageBox);
            Controls.Add(chatWithLabel);
            Name = "ChatWindow";
            Text = "ChatWindow";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label chatWithLabel;
        private TextBox messageBox;
        private Button sendMessageButton;
        private RichTextBox dialogueBox;
    }
}