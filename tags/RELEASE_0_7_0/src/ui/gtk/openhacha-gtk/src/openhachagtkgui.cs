/*

    OpenHacha, a "free as in freedom" implementation of Hacha.	
    Copyright (C) 2003  Ramón Rey Vicente <ramon.rey@hispalinux.es>

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
using System.IO;
using System.Text;

using Gtk;
using Glade;
using Gnome;
using GtkSharp;

namespace OpenHachaGtkGui 
{
	public class OpenHachaGui
	{
		[Glade.Widget] Gtk.Window MainWindow;
		[Glade.Widget] Gtk.RadioButton SplitOption;
		[Glade.Widget] Gtk.RadioButton PasteOption;
		
		public static void Main (string[] args) 
		{
			Application.Init();
			OpenHachaGui OpenHachaGtkMain = new OpenHachaGui();
			Application.Run();
		}

		public OpenHachaGui() 
		{
			Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "MainWindow", null);
			gxml.Autoconnect (this);
		}
		
		public void OnQuitButtonClicked (object o, EventArgs args) 
		{
                	Application.Quit ();
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
		[Glade.Widget] Gtk.Dialog SplitDialog;
		[Glade.Widget] Gtk.SpinButton SplitDialogSpinButton;
		[Glade.Widget] Gtk.Entry SplitDialogComboEntry;
		[Glade.Widget] Gtk.SpinButton SplitDialogNumberSpinButton; 
		[Glade.Widget] Gtk.OptionMenu FileFormat;
		[Glade.Widget] Gtk.Button SplitDialogSplitButton;
		
		private enum hachaformat
		{
			hacha1,
			hacha2,
			hachapro
		}

		private hachaformat format;

		public OpenHachaSplitDialog () 
		{
		       	Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "SplitDialog", null);
			gxml.Autoconnect (this);
			
			SplitDialogSplitButton.Sensitive = false;

			this.format = (hachaformat)FileFormat.History;
			Console.WriteLine(this.format);
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
					Convert.ToString(this.format), 
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

		public void OnHachaV1Activate (System.Object b, EventArgs e)
		{
			this.format = hachaformat.hacha1;
			Console.WriteLine(this.format);			
		}
		
		public void OnHachaV2Activate (System.Object b, EventArgs e)
		{
			this.format = hachaformat.hacha2;
			Console.WriteLine(this.format);			
		}
		
		public void OnHachaProActivate (System.Object b, EventArgs e)
		{
			this.format = hachaformat.hachapro;
			Console.WriteLine(this.format);
		}
	}

	public class OpenHachaPasteDialog 
	{
		[Glade.Widget] Gtk.Dialog PasteDialog;
		[Glade.Widget] Gtk.Entry PasteDialogComboEntry;
		[Glade.Widget] Gtk.Button PasteDialogPasteButton;
		
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
		[Glade.Widget] Gnome.About AboutDialog;
		
		/*
		 * FIXME: The About dialog appears without icon and label 
		 * on "credits" button
		 * */
		public OpenHachaAboutDialog()
		{
			Glade.XML gxml = new Glade.XML (null, "openhacha.glade", "AboutDialog", null);
			gxml.Autoconnect (this);
			
			AboutDialog.Name = "OpenHacha";
			AboutDialog.Version = "0.5";
			AboutDialog.Copyright = "Copyright © 2003 Ramón Rey Vicente <ramon.rey@hispalinux.es>";
			AboutDialog.Comments = "OpenHacha, a \"free as in freedom\" implementation of Hacha";
		}
	}
}
