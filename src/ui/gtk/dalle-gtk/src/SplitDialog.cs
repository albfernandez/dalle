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

using Gtk;
using Gdk;
using GtkSharp;

using Dalle.Formatos;
using I = Dalle.I18N.GetText;


namespace Dalle.UI.DalleGtk
{
	public class SplitDialog : BaseDialog
	{
	
		private const string FORMATO_DEFECTO = "hacha1";
		
		private Gtk.OptionMenu Formats;
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
			this.Title = I._("SplitDialog.Title");
			this.VBox.PackStart (new Gtk.Label ("Split"));
			Formats = CreateFormatsOptionMenu();
			this.VBox.PackStart (Formats);
			LayoutComponents();
		}
		private void LayoutComponents()
		{
			
			LayoutActionArea();
		}
		protected override Gtk.Button CreateActionButton ()
		{
			return new Gtk.Button (Gtk.Stock.Cut);
		}
		protected override void ExecuteAction()
		{
			string format = (listaFormatos [Formats.History] as IParte).Nombre;
			//Manager.Instance.Partir (format, FileEntry.Text, "salida", 1440);
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
