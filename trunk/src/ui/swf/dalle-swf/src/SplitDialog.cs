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


using Mono.Unix;
using System;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;


namespace Dalle.UI.DalleSWF
{

	public class SplitDialog : Form
	{
	
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
			this.Size = new Size (400, 300);
			this.Text = Catalog.GetString("Split");
		
		}
		private void closing (object sender, CancelEventArgs args)
		{
			this.Visible = false;
			args.Cancel = true;			
		}	
	}
}
