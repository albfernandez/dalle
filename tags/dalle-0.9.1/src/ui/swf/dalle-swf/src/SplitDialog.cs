/*

    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.


*/


using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;

using Dalle;
using Dalle.Formatos;


namespace Dalle.UI.DalleSWF
{

	public class SplitDialog : Form
	{
		
		private Label label1;
		private OpenFileDialog openFileDialog1;
		private TextBox txtFilename;
		private Button btnBrowse;
		private ProgressBar pb;
		private Button btnClose;
		private Button btnSplit;
		private Thread t;
		private bool requestStop = false;
		private NumericUpDown nudSize;
		private NumericUpDown nudFragments;
		private ComboBox cbFormatos;
	
		private static SplitDialog instance = null;
		
		public static SplitDialog Instance {
			get {
				if (instance == null){
					instance = new SplitDialog();
				}
				return instance;
			}
		}
		private SplitDialog()
		{
			InitComponent();
			this.Closing += new CancelEventHandler (closing);
		}	
		private void InitComponent ()
		{
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.Size = new Size (340, 270);
			this.MinimumSize = this.Size;
			this.MaximumSize = this.Size;
			//this.Text = Catalog.GetString("Merge");
			this.Text = "Split";
			
			this.SuspendLayout ();
			
			this.label1 = new Label ();
			this.label1.Location = new System.Drawing.Point (16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (160, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select the file to Split:";
			
			this.txtFilename = new TextBox ();
			this.txtFilename.Location = new System.Drawing.Point (8, 32);
			this.txtFilename.Name = "txtFilename";
			this.txtFilename.Size = new System.Drawing.Size (208, 20);
			this.txtFilename.TabIndex = 10;
			this.txtFilename.Text = "";
			this.txtFilename.TextChanged += new System.EventHandler (this.txtFilename_TextChanged);
			
			this.btnBrowse = new Button ();
			this.btnBrowse.Location = new System.Drawing.Point (232, 32);
			this.btnBrowse.Name = "cmdBrowse";
			this.btnBrowse.Size = new System.Drawing.Size (96, 24);
			this.btnBrowse.TabIndex = 1;
			this.btnBrowse.Text = "Browse...";
			this.btnBrowse.Click += new System.EventHandler (this.btnBrowseClick);
			
			
			this.nudFragments = new System.Windows.Forms.NumericUpDown ();
			this.nudFragments.Location = new System.Drawing.Point (8, 70);
			this.nudFragments.Size = new System.Drawing.Size (320, 24);
			this.nudFragments.Minimum = 1;
			this.nudFragments.Maximum = 100000;
			this.nudFragments.ValueChanged += new EventHandler (OnNudFragmentsChanged);
			
			this.nudSize = new System.Windows.Forms.NumericUpDown ();
			this.nudSize.Location = new System.Drawing.Point (8, 100);
			this.nudSize.Size = new System.Drawing.Size (320, 24);
			this.nudSize.Minimum = 1;
			this.nudSize.Maximum = 1000000;
			this.nudSize.ValueChanged += new EventHandler (OnNudSizeChanged);
			
			this.cbFormatos = new ComboBox ();
			this.cbFormatos.Location = new Point (8, 130);
			this.cbFormatos.Size = new Size (320, 24);
			ArrayList listaFormatos = Dalle.Formatos.Manager.Instance.GetFormatosParte ();
			foreach (IParte p in listaFormatos) {
				this.cbFormatos.Items.Add (p.Nombre);
			}
			this.cbFormatos.AutoCompleteMode = AutoCompleteMode.None;
			this.cbFormatos.DropDownStyle = ComboBoxStyle.DropDownList;
					
			this.pb = new ProgressBar ();
			this.pb.Location = new System.Drawing.Point (8, 170);
			this.pb.Name = "pb";
			this.pb.Size = new System.Drawing.Size (320, 24);
			this.pb.Step = 1;
			this.pb.TabIndex = 11;
			this.pb.Minimum = 0;
			this.pb.Maximum = 100;
			this.pb.Value = 0;
			
			this.btnClose = new Button ();
			this.btnClose.Location = new Point (220, 210);
			this.btnClose.Size = new System.Drawing.Size (50, 24);
			this.btnClose.Text = "Close";
			this.btnClose.Click += new EventHandler (btnCloseClick);
			
			this.btnSplit = new Button ();
			this.btnSplit.Location = new Point (270, 210);
			this.btnSplit.Size = new System.Drawing.Size (50, 24);
			this.btnSplit.Text = "Split";
			this.btnSplit.Click += new EventHandler (btnSplitClick);
			
			this.Controls.AddRange (new Control[] {
				label1,
				txtFilename,
				btnBrowse,
				pb,
				btnClose,
				btnSplit,
				nudFragments,
				nudSize,
				cbFormatos
			});
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			this.ResumeLayout (false);
			
		}
		private bool locked = false;
		private void OnNudFragmentsChanged (object sender, System.EventArgs args)
		{
			if (locked) 
			{
				return;	
			}
			if (!File.Exists(this.txtFilename.Text))
			{
				return;
			}
			long tamano = new FileInfo(this.txtFilename.Text).Length;
			decimal nValue = Math.Ceiling(tamano/ (this.nudFragments.Value * 1024));
			Console.WriteLine("nValue=" + nValue);
			locked = true;
			this.nudSize.Value = nValue;
			locked = false;
		}
		private void OnNudSizeChanged (object sender, System.EventArgs args)
		{
			if (locked)
			{
				return;
			}
			if (!File.Exists (this.txtFilename.Text))
			{
				return;
			}
			long tamano = new FileInfo (this.txtFilename.Text).Length;
			decimal nValue = Math.Ceiling (tamano / (this.nudSize.Value * 1024));
			this.locked=true;
			this.nudFragments.Value = nValue;
			this.locked = false;
		}
		private void closing (object sender, CancelEventArgs args)
		{
			if (t != null) {
				DialogResult dr = MessageBox.Show ("Split operation running, Stop?", "Stop?", MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes) {
					this.requestStop = true;
				}
				
			} else {
				this.Visible = false;
			}
			args.Cancel = true;
		}
		private void btnCloseClick (object sender, EventArgs args)
		{
			if (t != null) {
				DialogResult dr = MessageBox.Show ("Split operation running, Stop?", "Stop?", MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes) {
					this.requestStop = true;
				}
				
			} else {
				this.Visible = false;
			}
		}

		private void btnSplitClick (object sender, EventArgs args)
		{

			if (!File.Exists (this.txtFilename.Text))
			{
				MessageBox.Show ("The file '" + this.txtFilename.Text + "' does not exists");
				return;
			}
			if (this.cbFormatos.SelectedIndex < 0)
			{
				MessageBox.Show ("You must select a format");
				return;
			}
			
			if (t == null && File.Exists (this.txtFilename.Text)) {
				this.requestStop = false;
				this.t = new Thread (new ThreadStart (splitFile));
				this.t.Start ();
			}
			
		}
		private void DisableElements ()
		{
			this.btnSplit.Enabled = false;
			this.txtFilename.Enabled = false;
			this.btnBrowse.Enabled = false;
			this.nudFragments.Enabled = false;
			this.nudSize.Enabled = false;
			this.cbFormatos.Enabled = false;
		}
		private void EnableElements ()
		{
			this.btnSplit.Enabled = true;
			this.txtFilename.Enabled = true;
			this.btnBrowse.Enabled = true;
			this.nudFragments.Enabled = true;
			this.nudSize.Enabled = true;
			this.cbFormatos.Enabled = true;
		}
		protected void splitFile ()
		{
			
			string format = this.cbFormatos.SelectedItem.ToString ();
			string filename = this.txtFilename.Text;
			int kbs = (int)this.nudSize.Value;
			this.DisableElements ();
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			Manager.Instance.Partir (format, filename, "", kbs);
			
			this.EnableElements ();
			this.requestStop = false;
			t = null;

		}


		/// <summary>
		/// Click handler for the cmdBrowse button.
		/// Browses for the input file to be chopped.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnBrowseClick (object sender, System.EventArgs e)
		{
			if (openFileDialog1 == null) {
				openFileDialog1 = new OpenFileDialog ();
			}
			openFileDialog1.CheckFileExists = true;
			openFileDialog1.Filter = "All files (*.*)|*.*";
			openFileDialog1.ShowDialog ();
			
			txtFilename.Text = openFileDialog1.FileName;
		}
		private void txtFilename_TextChanged (object sender, System.EventArgs e)
		{
			//p_UpdateStatus();
		}
		protected virtual void OnProgress (long done, long total)
		{
			double fraction = ((double) done) / ((double)total);
			Console.WriteLine("progreso=" + done + "/" + total +"=" + fraction);
			this.pb.Value = (int) Math.Floor(100*fraction);
			//Console.WriteLine("barra=" + this.pb.Value);
			//this.Refresh();
			//System.Threading.Thread.Sleep (500);
			if (requestStop)
			{
				Manager.Instance.Stop();
				this.EnableElements();
			}
		}

	}
}
