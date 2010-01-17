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
using System.ComponentModel;
using System.Reflection;


namespace Dalle.UI.DalleSWF
{

	public class AboutDialog : Form
	{
	
		private static AboutDialog instance = null;
		
		public static AboutDialog Instance {
			get {
				if (instance == null){
					instance = new AboutDialog();
				}
				return instance;
			}
		}
		private AboutDialog()
		{
			InitComponent();
			this.Closing += new CancelEventHandler (closing);
		}	
		private void InitComponent ()
		{
			
			this.SuspendLayout();
			this.Size = new Size (450, 350);
			//this.Text = Catalog.GetString("Dalle-Swf") + " "  + Assembly.GetExecutingAssembly().GetName().Version;
			this.Text = "Dalle-Swf " + Assembly.GetExecutingAssembly().GetName().Version;
			
			String[] authors = new String[] {
				"Alberto Fernandez <infjaf@gmail.com>",
				"Ramon Rey Vicente <ramon.rey@hispalinux.es>",
				"Alvaro Pena <apg@esware.com>",
				"Eduardo Garcia Cebollero <kiwnix@yahoo.es>", 
				"Daniel Martinez Contador <dmcontador@terra.es>",
				"Dai SET <dai_set@yahoo.com>"};
			
			int ypos = 100;
			
			Label lbl0 = new Label();
			lbl0.Size = new Size (300, 25);
			//lbl0.Text = Catalog.GetString("Dalle-Swf") + " "  + Assembly.GetExecutingAssembly().GetName().Version;
			lbl0.Text = "Dalle-Swf " + Assembly.GetExecutingAssembly().GetName().Version;
			lbl0.Location = new Point (25, 25);
			this.Controls.Add(lbl0);
			
			
			for (int i=0; i < authors.Length; i++){
				Label lbl = new Label();
				lbl.Size = new Size (400, 25);
				lbl.Text = authors[i];
				lbl.Location = new Point (25, ypos);
				ypos += 25;
				this.Controls.Add(lbl);
			}
			
			Button btnClose = new Button();
			btnClose.Size = new Size (60, 30);
			btnClose.Location = new Point (375, ypos + 20);
			//btnClose.Text = Catalog.GetString("Close");
			btnClose.Text = "Close";
			btnClose.Click += new EventHandler (btnCloseClicked);
			this.Controls.Add(btnClose); 
			this.ResumeLayout(false);
		}
		private void btnCloseClicked (object sender, EventArgs args)
		{
			this.Visible = false;
		}
		private void closing (object sender, CancelEventArgs args)
		{
			this.Visible = false;
			args.Cancel = true;			
		}	
	}
}
