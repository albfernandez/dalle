/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Zip.Zip - Basic support for compressed zip files.
	
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


using System;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip;	

using Dalle.Utilidades;

using Dalle.Streams;

namespace Dalle.Formatos.Zip
{
	public class ZipJoinInfo : IJoinInfo {

		private string baseName;
		private int numberOfFragments;
		private long length;
		public ZipJoinInfo (string zipFile) {
			calculateBaseName(zipFile);
			calculateNumberOfFragmentsAndLength();
			
		}
		private void calculateBaseName(string zipFile) 
		{
			this.baseName = zipFile.Substring (0, zipFile.LastIndexOf ('.'));
		}
		private void calculateNumberOfFragmentsAndLength()
		{
			int fragmento = 0;
			while (new FileInfo(GetFragmentName(fragmento)).Exists) {
				length += new FileInfo(GetFragmentName(fragmento)).Length;
				fragmento ++;
			}
			this.numberOfFragments = fragmento;
		}
		public int FragmentsNumber {
			get { return numberOfFragments; }
		}
		public long Length {
			get { return length; }
		}
		public int GetOffset (int fragment)
		{
			return 0;
		}
		public string GetFragmentName (int fragment) {
			if (fragment == 0) {
				return baseName + ".zip";
			}
			return baseName + ".z" +  UtilidadesCadenas.Format (fragment, 2);
		}
		public string GetFragmentFullPath(int fragment) {
			return GetFragmentName(fragment);
		}
	}


}
