namespace p2pchat
{
    partial class ClientWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            connectButton = new Button();
            serverAddressLabel = new Label();
            enterUsernameBox = new TextBox();
            enterServerAddressBox = new TextBox();
            userNameLabel = new Label();
            availableUsersList = new ListBox();
            clientStatusBox = new RichTextBox();
            chatButton = new Button();
            clientStatusLabel = new Label();
            availableUsersLabel = new Label();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            connectButton.Location = new Point(90, 186);
            connectButton.Margin = new Padding(100, 3, 100, 3);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(500, 65);
            connectButton.TabIndex = 0;
            connectButton.Text = "Connect to Server";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.Click += connectButton_Click;
            // 
            // serverAddressLabel
            // 
            serverAddressLabel.AutoSize = true;
            serverAddressLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            serverAddressLabel.Location = new Point(172, 102);
            serverAddressLabel.Name = "serverAddressLabel";
            serverAddressLabel.Size = new Size(139, 28);
            serverAddressLabel.TabIndex = 1;
            serverAddressLabel.Text = "Server address";
            // 
            // enterUsernameBox
            // 
            enterUsernameBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            enterUsernameBox.Location = new Point(320, 32);
            enterUsernameBox.Name = "enterUsernameBox";
            enterUsernameBox.Size = new Size(245, 34);
            enterUsernameBox.TabIndex = 2;
            // 
            // enterServerAddressBox
            // 
            enterServerAddressBox.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            enterServerAddressBox.Location = new Point(320, 96);
            enterServerAddressBox.Name = "enterServerAddressBox";
            enterServerAddressBox.Size = new Size(245, 34);
            enterServerAddressBox.TabIndex = 3;
            // 
            // userNameLabel
            // 
            userNameLabel.AutoSize = true;
            userNameLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            userNameLabel.Location = new Point(207, 39);
            userNameLabel.Name = "userNameLabel";
            userNameLabel.Size = new Size(104, 28);
            userNameLabel.TabIndex = 4;
            userNameLabel.Text = "User name";
            // 
            // availableUsersList
            // 
            availableUsersList.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            availableUsersList.FormattingEnabled = true;
            availableUsersList.ItemHeight = 28;
            availableUsersList.Location = new Point(275, 341);
            availableUsersList.Name = "availableUsersList";
            availableUsersList.Size = new Size(315, 312);
            availableUsersList.TabIndex = 5;
            // 
            // clientStatusBox
            // 
            clientStatusBox.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            clientStatusBox.Location = new Point(90, 711);
            clientStatusBox.Name = "clientStatusBox";
            clientStatusBox.Size = new Size(500, 66);
            clientStatusBox.TabIndex = 6;
            clientStatusBox.Text = "";
            // 
            // chatButton
            // 
            chatButton.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            chatButton.Location = new Point(90, 382);
            chatButton.Margin = new Padding(100, 3, 100, 3);
            chatButton.Name = "chatButton";
            chatButton.Size = new Size(166, 65);
            chatButton.TabIndex = 7;
            chatButton.Text = "Chat";
            chatButton.UseVisualStyleBackColor = true;
            chatButton.Click += chatButton_Click;
            // 
            // clientStatusLabel
            // 
            clientStatusLabel.AutoSize = true;
            clientStatusLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            clientStatusLabel.Location = new Point(90, 679);
            clientStatusLabel.Name = "clientStatusLabel";
            clientStatusLabel.Size = new Size(65, 28);
            clientStatusLabel.TabIndex = 8;
            clientStatusLabel.Text = "Status";
            // 
            // availableUsersLabel
            // 
            availableUsersLabel.AutoSize = true;
            availableUsersLabel.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point);
            availableUsersLabel.Location = new Point(275, 298);
            availableUsersLabel.Name = "availableUsersLabel";
            availableUsersLabel.Size = new Size(141, 28);
            availableUsersLabel.TabIndex = 9;
            availableUsersLabel.Text = "Available users";
            // 
            // ClientWindow
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(682, 813);
            Controls.Add(availableUsersLabel);
            Controls.Add(clientStatusLabel);
            Controls.Add(chatButton);
            Controls.Add(clientStatusBox);
            Controls.Add(availableUsersList);
            Controls.Add(userNameLabel);
            Controls.Add(enterServerAddressBox);
            Controls.Add(enterUsernameBox);
            Controls.Add(serverAddressLabel);
            Controls.Add(connectButton);
            Name = "ClientWindow";
            Text = "P2PChat Client";
            FormClosed += ClientWindow_FormClosed;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button connectButton;
        private Label serverAddressLabel;
        private TextBox enterUsernameBox;
        private TextBox enterServerAddressBox;
        private Label userNameLabel;
        private ListBox availableUsersList;
        private RichTextBox clientStatusBox;
        private Button chatButton;
        private Label clientStatusLabel;
        private Label availableUsersLabel;
    }
}