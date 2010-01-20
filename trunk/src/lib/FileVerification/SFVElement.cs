/*

	Dalle - A split/join file utility library
	
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

using Dalle.FileVerification.FileHashers;

namespace Dalle.FileVerification
{

	public class SFVElement {
	
		private string file;
		private string hash;
		private IFileHasher hasher;
		private string realHash;
	
		public SFVElement (string file, string hash, IFileHasher hasher)
		{
			this.file = file;
			this.hash = hash;
			this.hasher = hasher;
		}
		
		public string FileName {
			get { return file; }
		}
		public string ExpectedHash {
			get { return hash; }
		}
		public string RealHash {
			get { return realHash; }
		}
		public string GenerateHash ()
		{
			realHash = hasher.GenerateHash (file);
			return realHash;
		}
		public bool IsOk ()
		{
			realHash = GenerateHash();
			return (realHash.ToUpper() == hash.ToUpper());
		}
	}
}
