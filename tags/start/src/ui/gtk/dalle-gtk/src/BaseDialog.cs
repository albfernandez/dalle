/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.BaseDialog - 
		Abstract base dialog for Split and Join dialogs
		
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
using System.IO;

using Gtk;
using GtkSharp;

using Dalle.Formatos;
using I = Dalle.I18N.GetText;

 

namespace Dalle.UI.DalleGtk
{
	public abstract class BaseDialog : Gtk.Dialog
	{
	
		private Gtk.FileSelection fileSelect;
		protected Gtk.Button ActionButton;
		protected Gtk.Button CloseButton;
		protected Gtk.Button BrowseButton;
		protected Gtk.Entry  FileEntry;
		protected CustomProgressBar Progress;
		protected bool running = false;
		
		//private Gtk.FileSelection fileSelect;
		//private Gtk.Label lblFichero;
		//private Gtk.Label lblInfo;
		//private Gtk.Button btnBuscar;
		//private Gtk.Button btnCerrar;
		//private Gtk.Button btnPegar;
		//private MiBarraDeProgreso progress;
		//private Gtk.Entry nombreFichero;
	
		public BaseDialog (Gtk.Window parent) : 
			base ("", parent, Gtk.DialogFlags.DestroyWithParent)
		{
			InitComponents();
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			this.DeleteEvent += new DeleteEventHandler (this.HideDialog);
			
			
			TransientFor = parent;
			Modal = true;
			this.SetSizeRequest (400,200);
			Resizable = false;
			BorderWidth = 15;
			HasSeparator = true;
			
		}
		
		protected Gtk.FileSelection FileSelection{
			get{
				if (fileSelect == null){
					fileSelect = CreateFileSelection();
				}	
				return fileSelect;				
			}
		}
		

		private void InitComponents()
		{
			CloseButton = new Gtk.Button (Gtk.Stock.Close);
			FileEntry = new Gtk.Entry ();
			BrowseButton = new Gtk.Button (I._("Browse..."));
			ActionButton = CreateActionButton();
			Progress = new CustomProgressBar ();
			
			
			CloseButton.Clicked += new EventHandler (this.CloseButtonClicked);
			ActionButton.Clicked += new EventHandler (this.ActionButtonClicked);
			BrowseButton.Clicked += new EventHandler (this.BrowseButtonClicked);
		}
				
		protected virtual Gtk.FileSelection CreateFileSelection ()
		{
			Gtk.FileSelection f = new Gtk.FileSelection(I._("Select_a_file"));
			
			f.TransientFor = this;
			f.Modal = true;
			f.DeleteEvent += new DeleteEventHandler (DalleGtk.HideWindow);
			f.OkButton.Clicked += new EventHandler (FileSelOkClicked);
			f.CancelButton.Clicked += new EventHandler (FileSelCancelClicked);	
			return f;
		}
		private void FileSelCancelClicked (object o, System.EventArgs args)
		{			
			this.FileSelection.Hide();
		}
		private void FileSelOkClicked (object o, System.EventArgs args)
		{
			this.FileEntry.Text = this.FileSelection.Filename;
			this.FileSelection.Hide();
		}
		
		protected abstract Gtk.Button CreateActionButton ();
		
		protected abstract void ExecuteAction();
		
		protected void LayoutActionArea ()
		{
			HBox hbox = new HBox (false, 10);
			hbox.PackStart (this.CloseButton);
			hbox.PackStart (this.ActionButton);

			this.ActionArea.Add(hbox);
		}
		
		protected void BrowseButtonClicked (object sender, EventArgs args)
		{
			this.FileSelection.ShowAll();
		}
		
		protected void ActionButtonClicked (object sender, EventArgs args)
		{
		
			if (!File.Exists (FileEntry.Text)){
				Gtk.MessageDialog d = new Gtk.MessageDialog (
					this, 
					Gtk.DialogFlags.DestroyWithParent,
					Gtk.MessageType.Error,
					Gtk.ButtonsType.Ok,
					"Selecciona un fichero valido");
				d.Run();
				d.Destroy();
				return;
			}
				
					
					
				
			running = true;
			OnBegin();
			try {
				ExecuteAction();
			}
			catch (Exception e){
				//TODO: Soportar todas las excepciones que pueda lanzar.
				//TODO: Y mostrar un diálogo en lugar de esto.
				Console.WriteLine ("Exception  ......");
				Console.WriteLine (e.Message);
			}
			OnFinish();
			running = false;
		}
		protected void CloseButtonClicked (object sender, EventArgs args)
		{
			RequestHideDialog ();
		}
		protected void HideDialog (object sender, DeleteEventArgs args)
		{	
			if (args != null)
				args.RetVal = true;
			RequestHideDialog ();
		}
		protected void RequestHideDialog ()
		{
		
			if (running){
				Gtk.MessageDialog d = new Gtk.MessageDialog (
					this, 
					Gtk.DialogFlags.DestroyWithParent,
					Gtk.MessageType.Question,
					Gtk.ButtonsType.YesNo,
					"¿Desea cerrar la ventana y detener el trabajo con los ficheros?");
				int ret = d.Run();
				d.Destroy();
				
				switch (ret){
				
				case (int) Gtk.ResponseType.No:
					Console.WriteLine ("No");
					break;
				case (int) Gtk.ResponseType.Yes:
					Console.WriteLine ("Yes");
					Manager.Instance.Stop();
					this.Hide();
					break;				
				}
			}
			else
				this.Hide();
		}
		public new void ShowAll()
		{
			Progress.Fraction = 0.0;
			base.ShowAll();
		}
		private void BuscarClicked (object o, System.EventArgs args)
		{
			this.FileSelection.ShowAll();
		}
		protected virtual void OnProgress (long done, long total)
		{
			double fraction = ((double) done) / ((double)total);
			this.Progress.Fraction = fraction;
			DalleGtk.DoEvents();
		}
		protected virtual void OnFinish()
		{
			this.FileEntry.Sensitive = true;
			//this.CloseButton.Sensitive = true;
			this.BrowseButton.Sensitive = true;
			this.ActionButton.Sensitive  = true;
			DalleGtk.DoEvents();
		}
		protected virtual void OnBegin()
		{
			this.FileEntry.Sensitive = false;
			this.ActionButton.Sensitive = false;
			this.BrowseButton.Sensitive = false;
			//this.CloseButton.Sensitive  = false;
			DalleGtk.DoEvents();
		}				
	}
}
