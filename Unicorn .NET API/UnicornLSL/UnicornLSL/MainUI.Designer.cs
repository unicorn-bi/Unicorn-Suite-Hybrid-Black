namespace UnicornLSL
{
    partial class MainUI
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
            this.pnlLayout = new System.Windows.Forms.TableLayoutPanel();
            this.gbUnicorn = new System.Windows.Forms.GroupBox();
            this.btnStartStop = new System.Windows.Forms.Button();
            this.btnOpenClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblDevices = new System.Windows.Forms.Label();
            this.cbDevices = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbSplitSignals = new System.Windows.Forms.RadioButton();
            this.rbCombineSigbnals = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.tbStreamName = new System.Windows.Forms.TextBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.pnlLayout.SuspendLayout();
            this.gbUnicorn.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLayout
            // 
            this.pnlLayout.ColumnCount = 1;
            this.pnlLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.pnlLayout.Controls.Add(this.gbUnicorn, 0, 1);
            this.pnlLayout.Controls.Add(this.groupBox2, 0, 2);
            this.pnlLayout.Controls.Add(this.lblTitle, 0, 0);
            this.pnlLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLayout.Location = new System.Drawing.Point(0, 0);
            this.pnlLayout.Name = "pnlLayout";
            this.pnlLayout.RowCount = 3;
            this.pnlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
            this.pnlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.pnlLayout.Size = new System.Drawing.Size(284, 355);
            this.pnlLayout.TabIndex = 0;
            this.pnlLayout.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.pnlLayout_CellPaint);
            // 
            // gbUnicorn
            // 
            this.gbUnicorn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbUnicorn.AutoSize = true;
            this.gbUnicorn.Controls.Add(this.btnStartStop);
            this.gbUnicorn.Controls.Add(this.btnOpenClose);
            this.gbUnicorn.Controls.Add(this.btnRefresh);
            this.gbUnicorn.Controls.Add(this.lblDevices);
            this.gbUnicorn.Controls.Add(this.cbDevices);
            this.gbUnicorn.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbUnicorn.Location = new System.Drawing.Point(10, 90);
            this.gbUnicorn.Margin = new System.Windows.Forms.Padding(10);
            this.gbUnicorn.Name = "gbUnicorn";
            this.gbUnicorn.Size = new System.Drawing.Size(264, 117);
            this.gbUnicorn.TabIndex = 0;
            this.gbUnicorn.TabStop = false;
            this.gbUnicorn.Text = "Unicorn Settings";
            // 
            // btnStartStop
            // 
            this.btnStartStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnStartStop.FlatAppearance.BorderSize = 0;
            this.btnStartStop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnStartStop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnStartStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartStop.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStartStop.ForeColor = System.Drawing.Color.White;
            this.btnStartStop.Location = new System.Drawing.Point(135, 59);
            this.btnStartStop.Name = "btnStartStop";
            this.btnStartStop.Size = new System.Drawing.Size(120, 23);
            this.btnStartStop.TabIndex = 4;
            this.btnStartStop.Text = "Start";
            this.btnStartStop.UseVisualStyleBackColor = false;
            this.btnStartStop.EnabledChanged += new System.EventHandler(this.btnStartStop_EnabledChanged);
            this.btnStartStop.Click += new System.EventHandler(this.btnStartStop_Click);
            // 
            // btnOpenClose
            // 
            this.btnOpenClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnOpenClose.FlatAppearance.BorderSize = 0;
            this.btnOpenClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnOpenClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnOpenClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenClose.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenClose.ForeColor = System.Drawing.Color.White;
            this.btnOpenClose.Location = new System.Drawing.Point(9, 59);
            this.btnOpenClose.Name = "btnOpenClose";
            this.btnOpenClose.Size = new System.Drawing.Size(120, 23);
            this.btnOpenClose.TabIndex = 3;
            this.btnOpenClose.Text = "Open";
            this.btnOpenClose.UseVisualStyleBackColor = false;
            this.btnOpenClose.EnabledChanged += new System.EventHandler(this.btnOpenClose_EnabledChanged);
            this.btnOpenClose.Click += new System.EventHandler(this.btnOpenClose_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRefresh.FlatAppearance.BorderSize = 0;
            this.btnRefresh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Silver;
            this.btnRefresh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.btnRefresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.Location = new System.Drawing.Point(135, 32);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(120, 23);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.EnabledChanged += new System.EventHandler(this.btnRefresh_EnabledChanged);
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblDevices
            // 
            this.lblDevices.AutoSize = true;
            this.lblDevices.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDevices.Location = new System.Drawing.Point(6, 16);
            this.lblDevices.Name = "lblDevices";
            this.lblDevices.Size = new System.Drawing.Size(94, 13);
            this.lblDevices.TabIndex = 1;
            this.lblDevices.Text = "Available Devices";
            // 
            // cbDevices
            // 
            this.cbDevices.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.cbDevices.FormattingEnabled = true;
            this.cbDevices.Location = new System.Drawing.Point(9, 32);
            this.cbDevices.Name = "cbDevices";
            this.cbDevices.Size = new System.Drawing.Size(120, 21);
            this.cbDevices.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.rbSplitSignals);
            this.groupBox2.Controls.Add(this.rbCombineSigbnals);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tbStreamName);
            this.groupBox2.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(10, 227);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(10);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(264, 118);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "LSL Settings";
            // 
            // rbSplitSignals
            // 
            this.rbSplitSignals.AutoSize = true;
            this.rbSplitSignals.Location = new System.Drawing.Point(7, 89);
            this.rbSplitSignals.Name = "rbSplitSignals";
            this.rbSplitSignals.Size = new System.Drawing.Size(184, 17);
            this.rbSplitSignals.TabIndex = 7;
            this.rbSplitSignals.TabStop = true;
            this.rbSplitSignals.Text = "send each signal in one stream";
            this.rbSplitSignals.UseVisualStyleBackColor = true;
            // 
            // rbCombineSigbnals
            // 
            this.rbCombineSigbnals.AutoSize = true;
            this.rbCombineSigbnals.Location = new System.Drawing.Point(7, 68);
            this.rbCombineSigbnals.Name = "rbCombineSigbnals";
            this.rbCombineSigbnals.Size = new System.Drawing.Size(177, 17);
            this.rbCombineSigbnals.TabIndex = 6;
            this.rbCombineSigbnals.TabStop = true;
            this.rbCombineSigbnals.Text = "send all signals in one stream";
            this.rbCombineSigbnals.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Streamname";
            // 
            // tbStreamName
            // 
            this.tbStreamName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbStreamName.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbStreamName.Location = new System.Drawing.Point(6, 40);
            this.tbStreamName.Name = "tbStreamName";
            this.tbStreamName.Size = new System.Drawing.Size(246, 22);
            this.tbStreamName.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.White;
            this.lblTitle.Location = new System.Drawing.Point(3, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(157, 80);
            this.lblTitle.TabIndex = 2;
            this.lblTitle.Text = "Unicorn LSL";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(284, 355);
            this.Controls.Add(this.pnlLayout);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(300, 340);
            this.Name = "MainUI";
            this.Text = "Unicorn LSL";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainUI_FormClosed);
            this.Shown += new System.EventHandler(this.MainUI_Shown);
            this.pnlLayout.ResumeLayout(false);
            this.pnlLayout.PerformLayout();
            this.gbUnicorn.ResumeLayout(false);
            this.gbUnicorn.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel pnlLayout;
        private System.Windows.Forms.GroupBox gbUnicorn;
        private System.Windows.Forms.Label lblDevices;
        private System.Windows.Forms.ComboBox cbDevices;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnStartStop;
        private System.Windows.Forms.Button btnOpenClose;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbStreamName;
        private System.Windows.Forms.RadioButton rbSplitSignals;
        private System.Windows.Forms.RadioButton rbCombineSigbnals;
    }
}

