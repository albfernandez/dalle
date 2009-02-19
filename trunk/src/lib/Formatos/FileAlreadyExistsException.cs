/*

    Dalle - A split/join file utility library	
	
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

namespace Dalle.Formatos {

	public class FileAlreadyExistsException : System.Exception {
		private string _fileName;
		public string FileName {
			get {
				return this._fileName;
			}
		}
		
		public FileAlreadyExistsException () 
		{
		}
		public FileAlreadyExistsException (string mensaje) : base(mensaje)
		{
		}
		public FileAlreadyExistsException (string mensaje, string fileName): base(mensaje)
		{
			this._fileName = fileName;
		}
		public FileAlreadyExistsException (string mensaje, Exception e): base (mensaje, e)
		{
		}	
	}
}
