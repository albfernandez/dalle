/*

    Copyright (C) 2003-2009  Alberto Fernández <infjaf00@yahoo.es>

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

namespace Dalle.UI.DalleSWF
{
	public class DalleSWF : System.Windows.Forms.Form {


		private static DalleSWF instance = null;

		public static DalleSWF Instance {
			get{
				if (instance == null){
					instance = new DalleSWF();
				}
				return instance;
			}
		}


		private Button btnExecute;
		private Button btnExit;
		private Button btnAbout;
		private Label lblWelcome;
		private Label lblWhat;
		
		private RadioButton rbSplit;
		private RadioButton rbMerge;


		private DalleSWF (){
			InitializeComponent();
		}
		private void InitializeComponent(){
		
			// Crear los objetos
			
			// ...
			this.btnExecute = new Button();
			this.btnAbout = new Button();
			this.btnExit = new Button();
			this.lblWelcome = new Label();
			this.lblWhat = new Label();
			this.rbSplit = new RadioButton();
			this.rbMerge = new RadioButton();
			
			
			this.SuspendLayout();
			
			
			this.btnExecute.Location = new Point(300,265);
			this.btnExecute.Name = "btnExecute";		
			this.btnExecute.Size = new Size (100, 30);
			this.btnExecute.TabIndex = 5;
			//this.btnExecute.Text = Catalog.GetString("Execute");
			this.btnExecute.Text = "Execute";
			
			
			this.btnExit.Location = new Point (200,265);
			this.btnExit.Name = "btnExit";
			this.btnExit.Size = new Size (100, 30);
			this.btnExit.TabIndex = 4;
			//this.btnExit.Text = Catalog.GetString("Exit");
			this.btnExit.Text = "Exit";
			
			
			this.btnAbout.Location = new Point (100, 265);
			this.btnAbout.Name = "btnAbout";
			this.btnAbout.Size = new Size(100, 30);
			this.btnAbout.TabIndex = 3;
			//this.btnAbout.Text = Catalog.GetString("About");
			this.btnAbout.Text = "About";
			
			
			this.rbSplit.Location = new Point(25,100);
			this.rbSplit.Name = "rbSplit";
			this.rbSplit.Size = new Size (100, 30);
			this.rbSplit.TabIndex = 1;
			//this.rbSplit.Text = Catalog.GetString("Split");
			this.rbSplit.Text  = "Split";
			this.rbSplit.Checked = true;
			
			this.rbMerge.Location = new Point (25, 125);
			this.rbMerge.Name = "rbJoin";
			this.rbMerge.Size = new Size(100,30);
			this.rbMerge.TabIndex = 2;
			//this.rbMerge.Text = Catalog.GetString("Merge");
			this.rbMerge.Text = "Join";
			
			this.lblWhat.Location = new Point (25,50);
			this.lblWhat.Name = "lblWhat";
			this.lblWhat.Size = new Size (200, 40);
			//this.lblWhat.Text = Catalog.GetString("What do you want to do?");
			this.lblWhat.Text = "What do you want to do?";
			
			
			this.lblWelcome.Location = new Point (25, 25);
			this.lblWelcome.Name = "lblWelcome";
			this.lblWelcome.Size = new Size (200,40);
			//this.lblWelcome.Text = Catalog.GetString("Welcome to dalle");
			this.lblWelcome.Text = "Welcome to dalle";
			
			

			
			
			// Tamaño
			this.ClientSize  = new System.Drawing.Size(400,300);
			//this.MaximumSize = new System.Drawing.Size(400,350);
			//this.MinimumSize = new System.Drawing.Size(400,350);
			
			
			this.Controls.AddRange( new Control[] {
					this.btnExecute, 
					this.btnAbout,
					this.btnExit,
					this.lblWelcome,
					this.lblWhat,
					this.rbSplit,
					this.rbMerge});
			
			// Titulo y resumir layout
			//this.Text = Catalog.GetString("Dalle-swf");
			this.Text = "Dalle-swf";
			this.ResumeLayout(false);
			this.ShowInTaskbar = false;
			
			// Conectar los eventos.
			
			this.btnExecute.Click += new EventHandler (execute_clicked);
			this.btnAbout.Click += new EventHandler (about_clicked);
			this.btnExit.Click += new EventHandler (exit_clicked);
			
			this.ShowInTaskbar = false;
		
		}
		
		private void execute_clicked (object sender, EventArgs args)
		{
			if (this.rbSplit.Checked){
				SplitDialog.Instance.Visible = true;
			}
			else if (this.rbMerge.Checked){
				JoinDialog.Instance.Visible = true;
			}

		}
		private void about_clicked (object sender, EventArgs args)
		{
			AboutDialog.Instance.Visible = true;
		}
		private void exit_clicked (object sender, EventArgs args)
		{
			ExitApplication();
		}
		private void ExitApplication ()
		{
			System.Environment.Exit(0);
		}
		[STAThread]
		public static void Main (String[] args){
			//Dalle.I18N.GettextCatalog.Init();
			Application.Run(DalleSWF.Instance);			
		
		}
	}
}
