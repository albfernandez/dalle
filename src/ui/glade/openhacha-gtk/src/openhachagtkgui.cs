/*

    OpenHacha, a "free as in freedom" implementation of Hacha.	
    Copyright (C) 2003, 2004  Ramón Rey Vicente <ramon.rey@hispalinux.es>

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
using System.IO;
using System.Text;
using System.Reflection;

using Gtk;
using Glade;
using Gnome;
using GtkSharp;

namespace OpenHachaGtkGui 
{
	public class OpenHachaGui : Program
	{
		[Glade.Widget] Gtk.RadioButton SplitOption = null;
		[Glade.Widget] Gtk.RadioButton PasteOption = null;
		
		public static void Main (string[] args) 
		{
			new OpenHachaGui();
		}

		public OpenHachaGui() : base ("OpenHacha", "0.7", Gnome.Modules.UI, new string[0])
		{
			
			Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "MainWindow", null);
			gxml.Autoconnect (this);
			this.Run();
		}
		
		public void OnQuitButtonClicked (object o, EventArgs args) 
		{
                	this.Quit ();
		}

		public void OnAboutButtonClicked (object o, EventArgs args) 
		{
			new OpenHachaAboutDialog();
		}

		public void OnRunButtonClicked (object o, EventArgs args) 
		{
			if (SplitOption.Active)
				new OpenHachaSplitDialog();
			if (PasteOption.Active)
				new OpenHachaPasteDialog();
		}
	}

	public class OpenHachaSplitDialog 
	{
		[Glade.Widget] Gtk.Dialog SplitDialog = null;
		[Glade.Widget] Gtk.SpinButton SplitDialogSpinButton = null;
		[Glade.Widget] Gtk.Entry SplitDialogComboEntry = null;
		[Glade.Widget] Gtk.SpinButton SplitDialogNumberSpinButton = null; 
		[Glade.Widget] Gtk.ComboBox FileFormat = null;
		[Glade.Widget] Gtk.Button SplitDialogSplitButton = null;
		

		public OpenHachaSplitDialog () 
		{
		    Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "SplitDialog", null);
			gxml.Autoconnect (this);			
			SplitDialogSplitButton.Sensitive = false;
			FileFormat.Active = 0;
		}
	
		public void OnSplitDialogComboEntryChanged (System.Object b, EventArgs e)
		{
			SplitDialogSplitButton.Sensitive = true;			
		}
		
		public void OnSplitDialogSplitButtonClicked (System.Object b, EventArgs e) 
		{
			string file = SplitDialogComboEntry.Text;
			if (File.Exists(file)) {
				Dalle.Formatos.Manager.GetInstance().Partir (
					FileFormat.ActiveText, 
					file,
					file.Substring(0, file.LastIndexOf('.')),
					(int)SplitDialogSpinButton.Value);
			}
			
			SplitDialogSplitButton.Sensitive = false;
		}
			
		public void OnSplitDialogNumberSpinButtonChanged(System.Object b, EventArgs e)
		{
			if (SplitDialogComboEntry.Text == "")
				return;
			
			FileInfo FileSelected = new FileInfo(SplitDialogComboEntry.Text);
			
			if (File.Exists(SplitDialogComboEntry.Text)) {
				SplitDialogSpinButton.Value = (FileSelected.Length/1024)/SplitDialogNumberSpinButton.Value;
			}
		}

		public void OnSplitDialogCloseButtonClicked (System.Object b, EventArgs e)
		{
			SplitDialog.Destroy ();
		}
	}

	public class OpenHachaPasteDialog 
	{
		[Glade.Widget] Gtk.Dialog PasteDialog = null;
		[Glade.Widget] Gtk.Entry PasteDialogComboEntry = null;
		[Glade.Widget] Gtk.Button PasteDialogPasteButton = null;
		
		public OpenHachaPasteDialog ()
		{
			Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "PasteDialog", null);
			gxml.Autoconnect (this);

			PasteDialogPasteButton.Sensitive = false;
		}
		
		public void OnPasteDialogPasteButtonClicked (System.Object b, EventArgs e)
		{
			string file = PasteDialogComboEntry.Text;
			
			if (File.Exists(file)) {
				Dalle.Formatos.Manager.GetInstance().Unir(
					file,
					new FileInfo (file).DirectoryName);
			}
			
			PasteDialogPasteButton.Sensitive = false;			
		}

		public void OnPasteDialogComboEntryChanged (System.Object b, EventArgs e)
		{
			PasteDialogPasteButton.Sensitive = true;
		}			
		
		public void OnPasteDialogCloseButtonClicked (System.Object b, EventArgs e)
		{
			PasteDialog.Destroy ();
		}
	}

	public class OpenHachaAboutDialog
	{
		[Glade.Widget] Gtk.AboutDialog AboutDialog = null;
		
		public OpenHachaAboutDialog()
		{
			Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "AboutDialog", null);
			gxml.Autoconnect (this);
			
			/*
			Assembly asm = Assembly.GetExecutingAssembly ();
			
			AboutDialog.Name = (asm.GetCustomAttributes (
               typeof (AssemblyTitleAttribute), false) [0]
               as AssemblyTitleAttribute).Title;
			AboutDialog.Version = asm.GetName ().Version.ToString ();
			*/
			
			AboutDialog.Response += new ResponseHandler (close_click);

		}
		private void close_click (object sender, EventArgs args)
		{
			if (AboutDialog != null)
				AboutDialog.Destroy();
		}
	}
}
