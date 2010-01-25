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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Threading;

using Dalle;
using Dalle.Formatos;


namespace Dalle.UI.DalleSWF
{

	public class JoinDialog : Form
	{
	
		private Label label1;
		private OpenFileDialog openFileDialog1;
		private TextBox txtFilename;
		private Button btnBrowse;
		private ProgressBar pb;
		private Button btnClose;
		private Button btnJoin;
		private Thread t;
		private bool requestStop = false;
		
		private static JoinDialog instance = null;
		
		public static JoinDialog Instance {
			get {
				if (instance == null){
					instance = new JoinDialog();
				}
				return instance;
			}
		}
		
		private JoinDialog ()
		{
			InitComponent();
			this.Closing += new CancelEventHandler (closing);
		}	
		private void InitComponent ()
		{
			this.MinimizeBox = false;
			this.MaximizeBox = false;
			this.Size = new Size (350, 165);
			this.MaximumSize = this.Size;
			this.MinimumSize = this.Size;
			//this.Text = Catalog.GetString("Merge");
			this.Text = "Merge";
			
			this.SuspendLayout ();
			
			this.label1 = new Label ();
			this.label1.Location = new System.Drawing.Point (16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size (160, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select the file to Join:";
			
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
			
			this.pb = new ProgressBar ();
			this.pb.Location = new System.Drawing.Point (8, 70);
			this.pb.Name = "pb";
			this.pb.Size = new System.Drawing.Size (320, 24);
			this.pb.Step = 1;
			this.pb.TabIndex = 11;
			this.pb.Minimum = 0;
			this.pb.Maximum = 100;
			this.pb.Value = 0;
			//this.pb.Visible = false;
			
			this.btnClose = new Button ();
			this.btnClose.Location = new Point (225, 100);
			this.btnClose.Size = new System.Drawing.Size (50, 24);
			this.btnClose.Text = "Close";
			this.btnClose.Click += new EventHandler (btnCloseClick);
			
			this.btnJoin = new Button ();
			this.btnJoin.Location = new Point (275, 100);
			this.btnJoin.Size = new System.Drawing.Size (50, 24);
			this.btnJoin.Text = "Join";
			this.btnJoin.Click += new EventHandler (btnJoinClick);
			
			this.Controls.AddRange (new Control[] {
				label1,
				txtFilename,
				btnBrowse,
				pb,
				btnClose,
				btnJoin
				
			});
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			this.ResumeLayout (false);
			
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
		
		private void closing (object sender, CancelEventArgs args)
		{
			if (t != null) {
				DialogResult dr = MessageBox.Show ("Join operation running, Stop?", "Stop?", MessageBoxButtons.YesNo);
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
			if (t != null)
			{
				DialogResult dr = 
					MessageBox.Show ("Join operation running, Stop?", "Stop?", MessageBoxButtons.YesNo);
				if (dr == DialogResult.Yes) {
					this.requestStop = true;
				}
				
			}
			else {
				this.Visible = false;			
			}
		}
		
		private void btnJoinClick (object sender, EventArgs args)
		{
			if (Manager.Instance.GetFormatoFichero (txtFilename.Text) == null) {
				MessageBox.Show ("File format not supported", "Done");
				return;
			}
			if (t == null)
			{
				this.requestStop = false;
				this.t = new Thread(new ThreadStart(joinFile));
				this.t.Start();
			}
			
		}
		protected void joinFile ()
		{
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			this.DisableElements ();
			Manager.Instance.Unir (txtFilename.Text);
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
		private void DisableElements ()
		{
			this.txtFilename.Enabled = false;
			this.btnBrowse.Enabled = false;
			this.btnJoin.Enabled = false;
		}
		private void EnableElements ()
		{
			this.txtFilename.Enabled = true;
			this.btnBrowse.Enabled = true;
			this.btnJoin.Enabled = true;
		}
	
	}
}
