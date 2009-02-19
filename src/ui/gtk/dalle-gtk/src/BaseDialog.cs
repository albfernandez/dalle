/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.BaseDialog - 
		Abstract base dialog for Split and Join dialogs
		
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
using System.IO;
using System.Reflection;

using Gtk;
using GtkSharp;

using Dalle.Formatos;
using Mono.Unix;

 

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
		
		public BaseDialog (Gtk.Window parent) : 
			base ("", parent, Gtk.DialogFlags.DestroyWithParent)
		{
			InitComponents();
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
			this.DeleteEvent += new DeleteEventHandler (this.HideDialog);
			try{
				this.Icon = new Gdk.Pixbuf (Assembly.GetExecutingAssembly(), "gears.png");
			}
			catch (Exception){
			}		
			TransientFor = parent;
			Modal = true;
			Resizable = false;
			BorderWidth = 10;
			HasSeparator = false;
			this.WindowPosition = Gtk.WindowPosition.CenterOnParent;
			
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
			BrowseButton = new Gtk.Button (Catalog.GetString("Browse..."));
			ActionButton = CreateActionButton();
			Progress = new CustomProgressBar ();
			
			
			CloseButton.Clicked += new EventHandler (this.CloseButtonClicked);
			ActionButton.Clicked += new EventHandler (this.ActionButtonClicked);
			BrowseButton.Clicked += new EventHandler (this.BrowseButtonClicked);
		}
				
		protected virtual Gtk.FileSelection CreateFileSelection ()
		{
			Gtk.FileSelection f = new Gtk.FileSelection(Catalog.GetString("Select a file"));
			
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
					Catalog.GetString("Selected file does not exist"));
				d.Run();
				d.Destroy();
				return;
			}
				
					
					
				
			running = true;
			OnBegin();
			String mensajeError = null;
			try {
				ExecuteAction();
			}
			catch (System.IO.FileNotFoundException e) {
				mensajeError = String.Format(Catalog.GetString("File not found: {0}"), e.FileName);
			}
			catch (Dalle.Formatos.FileFormatException){
				mensajeError = Catalog.GetString("Couldn't determine file type or the file is corrupted");
			}
			catch (Dalle.Formatos.FileAlreadyExistsException e){
				mensajeError = String.Format(Catalog.GetString("The file {0} already exists"), e.FileName);
			}
			catch (Dalle.Formatos.ChecksumVerificationException) {
				mensajeError = Catalog.GetString("The checksum is invalid");
			}
			catch (Exception e){
				mensajeError = e.Message;
			}
			if (mensajeError != null) {
				Gtk.MessageDialog d = new Gtk.MessageDialog (
					this, 
					Gtk.DialogFlags.DestroyWithParent,
					Gtk.MessageType.Error,
					Gtk.ButtonsType.Ok,
					mensajeError);
				d.Run();
				d.Destroy();
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
					Catalog.GetString("Do you want to stop file operation?"));
				int ret = d.Run();
				d.Destroy();
				
				switch (ret){
				
				case (int) Gtk.ResponseType.No:
					break;
				case (int) Gtk.ResponseType.Yes:
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
		protected virtual void OnProgress (long done, long total)
		{
			double fraction = ((double) done) / ((double)total);
			this.Progress.Fraction = fraction;
			DalleGtk.DoEvents();
		}
		protected virtual void OnFinish()
		{
			this.FileEntry.Sensitive = true;
			this.BrowseButton.Sensitive = true;
			this.ActionButton.Sensitive  = true;
			DalleGtk.DoEvents();
		}
		protected virtual void OnBegin()
		{
			this.FileEntry.Sensitive = false;
			this.ActionButton.Sensitive = false;
			this.BrowseButton.Sensitive = false;
			DalleGtk.DoEvents();
		}				
	}
}
