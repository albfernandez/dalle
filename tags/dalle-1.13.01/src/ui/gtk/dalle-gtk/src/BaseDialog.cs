/*

	Dalle - A split/join file utility library
	Dalle.UI.DalleGtk.BaseDialog - 
		Abstract base dialog for Split and Join dialogs
		
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


using System;
using System.IO;
using System.Reflection;

using Gtk;
using GtkSharp;

using Dalle.Formatos;
using Mono.Unix;

 

namespace Dalle.UI.DalleGtk {
	public abstract class BaseDialog : Gtk.Dialog {
		protected Gtk.Button ActionButton;
		protected Gtk.Button CloseButton;
		protected Gtk.Button BrowseButton;
		protected Gtk.Entry  FileEntry;
		protected CustomProgressBar Progress;
		protected Gtk.CheckButton CheckOutputDir;
		protected Gtk.EventBox LabelOutputDir;
		protected Gtk.Entry FileOutputDirEntry;
		protected bool running = false;
		protected string currentFolder = null;
		public BaseDialog (Gtk.Window parent) : 
			base ("", parent, Gtk.DialogFlags.DestroyWithParent) {
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

		private void InitComponents () {
			CloseButton = new Gtk.Button (Gtk.Stock.Close);
			FileEntry = new Gtk.Entry ();
			BrowseButton = new Gtk.Button (Catalog.GetString ("Browse..."));
			ActionButton = CreateActionButton ();
			Progress = new CustomProgressBar ();
			CheckOutputDir = new Gtk.CheckButton ();
			LabelOutputDir = new Gtk.EventBox();
			LabelOutputDir.Add(new Gtk.Label(Catalog.GetString ("Output directory")));
			LabelOutputDir.ButtonPressEvent += HandleLabelOutputDirClicked;

			FileOutputDirEntry = new Gtk.Entry ();
			CloseButton.Clicked += new EventHandler (this.CloseButtonClicked);
			ActionButton.Clicked += new EventHandler (this.ActionButtonClicked);
			BrowseButton.Clicked += new EventHandler (this.BrowseButtonClicked);
			// Targets
			TargetEntry [] te = new TargetEntry [] {
				new TargetEntry ("STRING", 0, 1),
			};
			Gtk.Drag.DestSet (this, DestDefaults.All, te, Gdk.DragAction.Copy | Gdk.DragAction.Move );
			
			FileEntry.DragDataReceived += new DragDataReceivedHandler(DropHandler);
			FileOutputDirEntry.DragDataReceived += new DragDataReceivedHandler(DropHandler2);
			FileOutputDirEntry.Sensitive = false;
			this.DragDataReceived += new DragDataReceivedHandler (DropHandler);			
			CheckOutputDir.Toggled += HandleCheckOutputDirToggled;			

		}
		
		private void HandleLabelOutputDirClicked(object sender, EventArgs e) {
			CheckOutputDir.Click();
		}

		private void HandleCheckOutputDirToggled (object sender, EventArgs e) {
			try {
				Gtk.CheckButton s = (Gtk.CheckButton)sender;
				if (s.Active) {
					FileOutputDirEntry.Sensitive = true;
					FileOutputDirEntry.Text = "";
				}
				else {
					FileOutputDirEntry.Sensitive = false;
					FileOutputDirEntry.Text = "";
				}
			}
			catch (Exception) {
				
			}
			
		}


		private string FormatDropString (string source)	{
			if (source.StartsWith ("file:///")){
				source = source.Substring ("file://".Length);
			}
			if (source.EndsWith ("\r\n"))
				{
				source = source.Substring (0, source.Length - 2);
			}
			
			if (source.EndsWith ("\n"))	{
				source = source.Substring (0, source.Length - 1);
			}
			return source;
		}
		protected void DropHandler2 (object o, DragDataReceivedArgs args){
			try {
				string fichero = FormatDropString (args.SelectionData.Text);			
				this.FileOutputDirEntry.Text = fichero;			
			}
			catch (Exception)
			{
				
			}
		}
		
		protected void DropHandler (object o, DragDataReceivedArgs args){
			try {
				string fichero = FormatDropString (args.SelectionData.Text);
				this.FileEntry.Text = fichero;			
			}
			catch (Exception)
			{
				
			}
		}
		
	
		protected abstract Gtk.Button CreateActionButton ();
		
		protected abstract void ExecuteAction();
		
		protected void LayoutActionArea (){
			HBox hbox = new HBox (false, 10);
			hbox.PackStart (this.CloseButton);
			hbox.PackStart (this.ActionButton);

			this.ActionArea.Add(hbox);
		}
		
		protected void BrowseButtonClicked (object sender, EventArgs args){
			Gtk.FileChooserDialog fc = new Gtk.FileChooserDialog (
				Catalog.GetString("Choose the file to open"), 
				this, 
				FileChooserAction.Open, 
				Catalog.GetString("Cancel"), ResponseType.Cancel, 
				Catalog.GetString("Open"), ResponseType.Accept
			);
			if (currentFolder != null && !String.Empty.Equals (currentFolder)){
				fc.SetCurrentFolder (currentFolder);
			}
			
			if (fc.Run () == (int)ResponseType.Accept) {
				this.currentFolder = fc.CurrentFolder;
				this.FileEntry.Text = fc.Filename;
			}
			fc.Destroy ();
		}
		
		protected void ActionButtonClicked (object sender, EventArgs args){
		
			if (!File.Exists (FileEntry.Text)) {
				Gtk.MessageDialog d = new Gtk.MessageDialog (
					this, 
					Gtk.DialogFlags.DestroyWithParent,
					Gtk.MessageType.Error,
					Gtk.ButtonsType.Ok,
					Catalog.GetString ("Selected file does not exist"));
				d.Run ();
				d.Destroy ();
				return;
			}
						
			running = true;
			OnBegin ();
			String mensajeError = null;
			try {
				ExecuteAction ();
			}
			catch (System.IO.FileNotFoundException e) {
				mensajeError = String.Format (Catalog.GetString ("File not found: {0}"), e.FileName);
			}
			catch (Dalle.Formatos.FileFormatException) {
				mensajeError = Catalog.GetString ("Couldn't determine file type or the file is corrupted");
			}
			catch (Dalle.Formatos.FileAlreadyExistsException e) {
				mensajeError = String.Format (Catalog.GetString ("The file {0} already exists"), e.FileName);
			}
			catch (Dalle.Formatos.ChecksumVerificationException ex) {
				mensajeError = Catalog.GetString ("The checksum is invalid");
				if (ex.File != null) {
					mensajeError += ":" + ex.File;
				}
			}
			catch (Exception e) {
				Console.WriteLine (e);
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
		protected void CloseButtonClicked (object sender, EventArgs args){
			RequestHideDialog ();
		}
		protected void HideDialog (object sender, DeleteEventArgs args)	{	
			if (args != null) {
				args.RetVal = true;
			}
			RequestHideDialog ();
		}
		protected void RequestHideDialog ()	{
		
			if (running) {
				Gtk.MessageDialog d = new Gtk.MessageDialog (
					this, 
					Gtk.DialogFlags.DestroyWithParent,
					Gtk.MessageType.Question,
					Gtk.ButtonsType.YesNo,
					Catalog.GetString ("Do you want to stop file operation?"));
				int ret = d.Run ();
				d.Destroy ();
				
				switch (ret) {
				
				case (int) Gtk.ResponseType.No:
					break;
				case (int) Gtk.ResponseType.Yes:
					Manager.Instance.Stop ();
					this.Hide ();
					break;				
				}
			} else {
				this.Hide ();
			}
		}
		public new void ShowAll (){
			Progress.Fraction = 0.0;
			CheckOutputDir.Active = false;
			FileOutputDirEntry.Sensitive = this.CheckOutputDir.Active;
			FileOutputDirEntry.Text = "";
			FileEntry.Text = "";
			FileEntry.Sensitive = true;
			base.ShowAll();
		}
		protected virtual void OnProgress (long done, long total){
			double fraction = ((double) done) / ((double)total);
			this.Progress.Fraction = fraction;
			DalleGtk.DoEvents();
		}
		protected virtual void OnFinish (){
			this.FileEntry.Sensitive = true;			
			this.FileOutputDirEntry.Sensitive = this.CheckOutputDir.Active;
			this.BrowseButton.Sensitive = true;
			this.ActionButton.Sensitive  = true;
			DalleGtk.DoEvents();
		}
		protected virtual void OnBegin (){
			this.FileEntry.Sensitive = false;
			this.FileOutputDirEntry.Sensitive = false;
			this.ActionButton.Sensitive = false;
			this.BrowseButton.Sensitive = false;
			DalleGtk.DoEvents ();
		}
		
	}
}
