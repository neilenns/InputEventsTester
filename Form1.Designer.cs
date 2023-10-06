namespace InputEventsTester
{
    partial class Form1
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
            getInputEventsButton = new Button();
            eventsComboBox = new ComboBox();
            label1 = new Label();
            currentValueTextBox = new TextBox();
            label2 = new Label();
            newValueTextBox = new TextBox();
            submitButton = new Button();
            getCurrentValueButton = new Button();
            getParametersButton = new Button();
            parametersTextBox = new TextBox();
            SuspendLayout();
            // 
            // connectButton
            // 
            connectButton.Location = new Point(8, 7);
            connectButton.Margin = new Padding(2);
            connectButton.Name = "connectButton";
            connectButton.Size = new Size(78, 30);
            connectButton.TabIndex = 0;
            connectButton.Text = "Connect";
            connectButton.UseVisualStyleBackColor = true;
            connectButton.MouseClick += ConnectButton_MouseClick;
            // 
            // getInputEventsButton
            // 
            getInputEventsButton.Location = new Point(91, 7);
            getInputEventsButton.Margin = new Padding(2);
            getInputEventsButton.Name = "getInputEventsButton";
            getInputEventsButton.Size = new Size(116, 30);
            getInputEventsButton.TabIndex = 1;
            getInputEventsButton.Text = "Get InputEvents";
            getInputEventsButton.UseVisualStyleBackColor = true;
            getInputEventsButton.Click += GetInputEventsButton_Click;
            // 
            // eventsComboBox
            // 
            eventsComboBox.FormattingEnabled = true;
            eventsComboBox.Location = new Point(11, 41);
            eventsComboBox.Margin = new Padding(2);
            eventsComboBox.Name = "eventsComboBox";
            eventsComboBox.Size = new Size(292, 23);
            eventsComboBox.TabIndex = 2;
            eventsComboBox.SelectedIndexChanged += eventsComboBox_SelectedIndexChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(11, 117);
            label1.Margin = new Padding(2, 0, 2, 0);
            label1.Name = "label1";
            label1.Size = new Size(81, 15);
            label1.TabIndex = 3;
            label1.Text = "Current value:";
            // 
            // currentValueTextBox
            // 
            currentValueTextBox.Location = new Point(101, 114);
            currentValueTextBox.Margin = new Padding(2);
            currentValueTextBox.Name = "currentValueTextBox";
            currentValueTextBox.ReadOnly = true;
            currentValueTextBox.Size = new Size(106, 23);
            currentValueTextBox.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 144);
            label2.Margin = new Padding(2, 0, 2, 0);
            label2.Name = "label2";
            label2.Size = new Size(65, 15);
            label2.TabIndex = 5;
            label2.Text = "New value:";
            // 
            // newValueTextBox
            // 
            newValueTextBox.Location = new Point(101, 141);
            newValueTextBox.Margin = new Padding(2);
            newValueTextBox.Name = "newValueTextBox";
            newValueTextBox.Size = new Size(106, 23);
            newValueTextBox.TabIndex = 6;
            newValueTextBox.KeyPress += NewValueTextBox_KeyPress;
            // 
            // submitButton
            // 
            submitButton.Location = new Point(211, 141);
            submitButton.Margin = new Padding(2);
            submitButton.Name = "submitButton";
            submitButton.Size = new Size(78, 23);
            submitButton.TabIndex = 7;
            submitButton.Text = "Set";
            submitButton.UseVisualStyleBackColor = true;
            submitButton.Click += SubmitButton_Click;
            // 
            // getCurrentValueButton
            // 
            getCurrentValueButton.Location = new Point(211, 114);
            getCurrentValueButton.Margin = new Padding(2);
            getCurrentValueButton.Name = "getCurrentValueButton";
            getCurrentValueButton.Size = new Size(78, 23);
            getCurrentValueButton.TabIndex = 8;
            getCurrentValueButton.Text = "Get";
            getCurrentValueButton.UseVisualStyleBackColor = true;
            getCurrentValueButton.Click += GetCurrentValueButton_Click;
            // 
            // getParametersButton
            // 
            getParametersButton.Location = new Point(308, 41);
            getParametersButton.Name = "getParametersButton";
            getParametersButton.Size = new Size(100, 23);
            getParametersButton.TabIndex = 9;
            getParametersButton.Text = "Get parameters";
            getParametersButton.UseVisualStyleBackColor = true;
            getParametersButton.Click += getParametersButton_Click;
            // 
            // parametersTextBox
            // 
            parametersTextBox.Location = new Point(12, 69);
            parametersTextBox.Name = "parametersTextBox";
            parametersTextBox.ReadOnly = true;
            parametersTextBox.Size = new Size(291, 23);
            parametersTextBox.TabIndex = 10;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(560, 270);
            Controls.Add(parametersTextBox);
            Controls.Add(getParametersButton);
            Controls.Add(getCurrentValueButton);
            Controls.Add(submitButton);
            Controls.Add(newValueTextBox);
            Controls.Add(label2);
            Controls.Add(currentValueTextBox);
            Controls.Add(label1);
            Controls.Add(eventsComboBox);
            Controls.Add(getInputEventsButton);
            Controls.Add(connectButton);
            Margin = new Padding(2);
            Name = "Form1";
            Text = "InputEvents Tester";
            FormClosing += Form1_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button connectButton;
        private Button getInputEventsButton;
        private ComboBox eventsComboBox;
        private Label label1;
        private TextBox currentValueTextBox;
        private Label label2;
        private TextBox newValueTextBox;
        private Button submitButton;
        private Button getCurrentValueButton;
        private Button getParametersButton;
        private TextBox parametersTextBox;
    }
}