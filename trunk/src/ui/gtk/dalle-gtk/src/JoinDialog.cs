/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.JoinDialog - Dialog to join files
	
    Copyright (C) 2003-2013  Alberto Fernández <infjaf@gmail.com>

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


using Gtk;
using Gdk;
using GtkSharp;
using System;
using System.IO;

using Dalle.Formatos;
using System.Reflection;
using Mono.Unix;


namespace Dalle.UI.DalleGtk{
	public class JoinDialog : BaseDialog{
	
		private static JoinDialog instance = null;
		
		public static JoinDialog Instance {
			get {
				if (instance == null) {
					instance = new JoinDialog (DalleGtk.Instance);
				}
				return instance;
			}
		}
		
		private JoinDialog (Gtk.Window parent) : base (parent) {
			this.SetSizeRequest (500, 330);
			this.Title = Catalog.GetString("Join File Fragments");
			this.LayoutComponents();

		}
		private void LayoutComponents()	{
			Gtk.Image img = null;
						
			Gtk.HBox hbox1 = new Gtk.HBox (false, 6);
			hbox1.BorderWidth = 0;
			
			try{	
				img = new Gtk.Image (new Gdk.Pixbuf (Assembly.GetExecutingAssembly(), "join.gif"));
				img.Xalign = 0.5f;
				img.Yalign = 0.0f;
				hbox1.PackStart (img, false, false, 0);			
			}
			catch (Exception){
			}

			
			
			
			Gtk.VBox vbox1 = new Gtk.VBox (false, 12);
			
			Gtk.Label lbl = new Gtk.Label (String.Empty);
			lbl.Markup = String.Format (
				"<big><b>{0}</b></big>\n{1}",
				Catalog.GetString("Join the fragments of a file"),
				Catalog.GetString("Re-create a file from its fragments"));
			lbl.Xalign = 0.0f;
			lbl.Yalign = 0.0f;
			
			vbox1.PackStart (lbl, false, false, 0);
			
			Gtk.HBox hbox2 = new Gtk.HBox (false, 6);
			hbox2.BorderWidth = 0;
			
			Gtk.Label lbl2 = new Gtk.Label (Catalog.GetString("Fragment to join"));
			
			hbox2.PackStart (lbl2, false, false, 2);
			
			hbox2.PackStart (FileEntry, true, true ,2);
			hbox2.PackStart (BrowseButton, false, false, 3);

			vbox1.PackStart (hbox2, true, true, 2);


			Gtk.HBox hbox3 = new Gtk.HBox();
			hbox3.PackStart(CheckOutputDir,false,false,10);
			hbox3.PackStart(LabelOutputDir,false,false,0);
			hbox3.PackStart(new HBox(), true, true, 0);
			
			Gtk.HBox hbox4 = new Gtk.HBox(false, 2);
			hbox4.PackStart(FileOutputDirEntry);
			
			Gtk.HBox hbox5 = new Gtk.HBox(false, 12);
			hbox5.PackStart (Progress);
			
			hbox1.PackStart (vbox1, true, true, 0);
			
			this.VBox.PackStart (hbox1, false, false, 15);
			this.VBox.PackStart (hbox3, false, false, 0);
			this.VBox.PackStart (hbox4);
			this.VBox.PackStart (hbox5, false, false, 3);
			
			
			LayoutActionArea();
		}

		protected override Gtk.Button CreateActionButton ()	{
			return new Gtk.Button (Gtk.Stock.Paste);
		}
		protected override void ExecuteAction ()
		{
			String mensaje = null;
			if (String.Empty.Equals (FileEntry.Text)){
				mensaje = Catalog.GetString ("You must select a file");
			}
			if (mensaje == null && !File.Exists (FileEntry.Text)) {
				mensaje = String.Format(Catalog.GetString ("File not found: {0}"), FileEntry.Text);
			}

			// Comprobar que el formato existe
			
			if (Manager.Instance.GetFormatoFichero (FileEntry.Text) == null) {
				Gtk.MessageDialog d = new Gtk.MessageDialog (
					this, 
					Gtk.DialogFlags.DestroyWithParent,
					Gtk.MessageType.Error,
					Gtk.ButtonsType.Ok,
					Catalog.GetString ("Unknown file format"));
				d.Run ();
				d.Destroy ();
				return;
			
			}

			if (!CheckOutputDir.Active){
				Manager.Instance.Unir (FileEntry.Text);
				return;
			}

			if (FileOutputDirEntry.Text == "") {
				return;
			}
			if (File.Exists(FileOutputDirEntry.Text)){
				return;
			}
			if (!Directory.Exists (FileOutputDirEntry.Text)){
				Directory.CreateDirectory(FileOutputDirEntry.Text);
			}

			if ( Directory.Exists(FileOutputDirEntry.Text)){
				Manager.Instance.Unir (FileEntry.Text, FileOutputDirEntry.Text);
			}
		}					
	}
}
