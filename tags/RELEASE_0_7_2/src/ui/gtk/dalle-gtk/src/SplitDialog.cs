/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.SplitDialog - Dialog to split files
	
    Copyright (C) 2003  Alberto Fernández <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/

using System;
using System.Collections;
using System.Reflection;
using System.IO;

using Gtk;
using Gdk;
using GtkSharp;

using Dalle.Formatos;
using I = Dalle.I18N.GetText;


namespace Dalle.UI.DalleGtk
{
	public class SplitDialog : BaseDialog
	{
	
		private const string FORMATO_DEFECTO = "generico";
		
		private Gtk.OptionMenu Formats;
		private Gtk.SpinButton numberSpin;
		private Gtk.SpinButton sizeSpin;
		private ArrayList listaFormatos;
		
		private static SplitDialog instance;
		
		public static SplitDialog Instance{
			get{
				if (instance == null)
					instance = new SplitDialog (DalleGtk.Instance);
				return instance;
			}
		}
		
		private SplitDialog (Gtk.Window parent) : base (parent)
		{
			this.Title = I._("Split Files");
			this.SetSizeRequest (550,300);
			this.FileEntry.Changed += new EventHandler (OnEntryChanged);
			Formats = CreateFormatsOptionMenu();
			LayoutComponents();
		}
		private void LayoutComponents()
		{			
			
			Gtk.HBox hbox1 = new Gtk.HBox (false, 12);
			hbox1.BorderWidth = 0;
			
			Gtk.Image img = new Gtk.Image (new Gdk.Pixbuf (null, "split.gif"));
			img.Xalign = 0.5f;
			img.Yalign = 0.0f;			
			hbox1.PackStart (img, false, false, 0);
			
			
			Gtk.VBox vbox1 = new Gtk.VBox (false, 12);
			
						
			Gtk.Label lbl = new Gtk.Label (String.Empty);
			lbl.Markup = String.Format (
				"<big><b>{0}</b></big>\n{1}",
				I._("Split a File"),
				I._("Split a file in small pieces"));
			
			lbl.Xalign = 0.0f;
			lbl.Yalign = 0.0f;
			
			vbox1.PackStart (lbl, false, false, 0);
			
			
			Gtk.Table table = new Gtk.Table (4, 2, false);
			table.ColumnSpacing = 6;
			table.RowSpacing = 6;
			
			Gtk.Label lbl1 = new Gtk.Label (I._("Select a file to split"));
			lbl1.Xalign = 0.0f;
			lbl1.Yalign = 0.5f;
			
			table.Attach (lbl1, 0, 1, 0,1);
			
			Gtk.HBox hbox2 = new Gtk.HBox (false, 6);
			hbox2.PackStart (FileEntry);
			hbox2.PackStart (BrowseButton);
			
			table.Attach (hbox2, 1,2, 0,1);
			
			
			Gtk.Label lbl2 = new Gtk.Label (I._("Number of fragments"));
			lbl2.Xalign = 0.0f;
			lbl2.Yalign = 0.5f;
			table.Attach (lbl2, 0,1, 1,2);
			
			
			Gtk.Adjustment adj = new Gtk.Adjustment (1.0,1.0,999.0, 1.0, 10.0, 10.0);
			numberSpin = new Gtk.SpinButton (adj, 1.0, 0);
			numberSpin.ValueChanged += new EventHandler (this.OnNumberSpinChanged);
			table.Attach (numberSpin, 1,2, 1,2);
			
			Gtk.Label lbl3 = new Gtk.Label (I._("Size of fragments"));
			lbl3.Xalign = 0.0f;
			lbl3.Yalign = 0.5f;
			table.Attach (lbl3, 0, 1, 2, 3);
			
			Gtk.Adjustment adj2 = new Gtk.Adjustment (1420.0, 1.0, 1.024e6, 10.0,100.0,100.0); 
			sizeSpin = new Gtk.SpinButton (adj2, 10.0, 0);
			sizeSpin.ValueChanged += new EventHandler (this.OnSizeSpinChanged);
			Gtk.HBox hbox3 = new Gtk.HBox (false, 6);
			hbox3.PackStart (sizeSpin, true, true, 0);
			hbox3.PackStart (new Gtk.Label (I._("KiB")), false, false, 0);
			table.Attach (hbox3, 1,2, 2, 3);
			
			Gtk.Label lbl4 = new Gtk.Label (I._("File Format"));
			lbl4.Xalign = 0.0f;
			lbl3.Yalign = 0.5f;
			table.Attach (lbl4, 0, 1, 3, 4);
			
			table.Attach (Formats, 1,2,3,4);
			
			
			
			
			
			vbox1.PackStart (table, true, true, 0);
			hbox1.PackStart (vbox1, true, true, 0);
			
			
			this.VBox.PackStart (hbox1, false, false, 0);
			this.VBox.PackStart(Progress, true, false, 0);
			
			LayoutActionArea();
		}
		protected override Gtk.Button CreateActionButton ()
		{
			return new Gtk.Button (Gtk.Stock.Cut);
		}
		protected override void ExecuteAction()
		{
			string format = (listaFormatos [Formats.History] as IParte).Nombre;
			
			Manager.Instance.Partir (format, FileEntry.Text, "", sizeSpin.ValueAsInt);
		}
		
		protected void OnSizeSpinChanged (object o, EventArgs args)
		{
			OnEntryChanged (o, args);
		}
		protected void OnNumberSpinChanged (object o, EventArgs args)
		{
			if (! File.Exists (this.FileEntry.Text) )
				return;
			long tamano = new FileInfo(this.FileEntry.Text).Length;
			sizeSpin.Value = Math.Ceiling ((double) (tamano) / (numberSpin.ValueAsInt * 1024));
		}
		
		protected void OnEntryChanged (object sender, EventArgs args)
		{
			if (! File.Exists (this.FileEntry.Text) )
				return;
			long tamano = new FileInfo(this.FileEntry.Text).Length;
			numberSpin.Value = Math.Ceiling ((double) (tamano) / (sizeSpin.ValueAsInt * 1024));
			
		}
		private Gtk.OptionMenu CreateFormatsOptionMenu()
		{
			int defecto = 0;
			Gtk.OptionMenu m = new Gtk.OptionMenu();
			
			Gtk.Menu menu = new Gtk.Menu();
			
			listaFormatos = Dalle.Formatos.Manager.Instance.GetFormatosParte();
			int c = 0;
			foreach (IParte p in listaFormatos){
				MenuItem it = new MenuItem (p.Nombre);
				menu.Append (it);
				if (p.Nombre == FORMATO_DEFECTO)
					defecto = c;
				c++;
			}
			m.Menu = menu;
			
			m.SetHistory ((uint) defecto);
			return m;			
		}
	}
}
