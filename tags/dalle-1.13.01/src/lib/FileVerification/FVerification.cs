/*

	Dalle - A split/join file utility library
	Dalle.FileVerification.FVerification - 	
		Abstract base class for all supported file verification types
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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




using System.Collections;
using System.IO;

namespace Dalle.FileVerification
{
	public abstract class FVerification : IFVerification 
	{
		private string name;
		private bool canCreate;

		protected FVerification (string name, bool canCreate)
		{
			this.name = name;
			this.canCreate = canCreate;
		}
		
		public string Name{
			get { return name; }
		}
		public bool CanCreate{
			get { return canCreate; }
		}
		
		public void CreateSFV (string[] files, TextWriter writer)
		{
			CreateSFV (files, false, writer);
		}
		
		public virtual void CreateSFV (string[] files, bool recursive, TextWriter writer)
		{
						
		}
		
		public void VerifySFV (string file)
		{
			ArrayList list = GenerateSFVFileList (file);
			foreach (SFVElement e in list){
				if (! File.Exists (e.FileName))
					this.OnFileEvent (e, FileCheckResult.NotFound);
				else if (Directory.Exists (e.FileName))
					this.OnFileEvent (e, FileCheckResult.IsDirectory);		
				else if (e.IsOk())
					this.OnFileEvent (e, FileCheckResult.Ok);
				else
					this.OnFileEvent (e , FileCheckResult.Failed);
			}
		}
		
		protected abstract ArrayList GenerateSFVFileList (string file);
		
		protected void OnFileEvent (SFVElement e, FileCheckResult r)
		{
			if (this.FileCheck != null)
				this.FileCheck (e, r);
		}
		
				
		public event FileCheckEventHandler FileCheck;
		public event InitialListEventHandler InitialList;
		
		public virtual bool IsFormatRecognized (string file)
		{
			return false;
		}
		
	}
}
