/*
    Dalle - A split/join file utility library	
    Dalle.Checksums.NullCRC - CRC class that does nothing
	
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


using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using Dalle.Utilidades;


namespace Dalle.FileVerification.FileHashers
{
	public class FileHasherCrc32 : IFileHasher
	{
		private Crc32 crc = new Crc32 ();
		public FileHasherCrc32 ()
		{
		}
		public string GenerateHash (string fileName)
		{
			crc.Reset ();
			UtilidadesFicheros.GenerateHash (fileName, crc);		
			string ret = crc.Value.ToString ("X").ToUpper();
			while (ret.Length < 8)
			{
				ret = "0" + ret;
			}
			return ret;
		}
	}
}
