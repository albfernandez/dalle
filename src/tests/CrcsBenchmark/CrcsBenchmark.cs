/*

	Dalle - A split/join file utility library
	Dalle.CrcsBenchmark - A little bench of crcs used by dalle.		
	
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
using System.Collections;
using System.IO;

using ICSharpCode.SharpZipLib.Checksums;

using Dalle.Formatos.Hacha;
using Dalle.Formatos.SplitFile;
using Dalle.Formatos.Kamaleon;


namespace Dalle
{
	
	public class CrcsBenchmark 
	{
	
		public const int BUFFER_SIZE = 2048;
		public const int TEST_LENGTH = 0x40000000; // 1 Gb
		public static void Main ()
		{
		
			long inicio, fin;
			Hashtable table = new Hashtable();
			table.Add ("Adler32", new Adler32());
			table.Add ("CRC32", new Crc32());
			table.Add ("StrangeCRC", new StrangeCRC());
			table.Add ("HachaCRC", new HachaCRC (TEST_LENGTH));
			table.Add ("SplitFileCRC", new SplitFileCRC ());
			table.Add ("Kamaleon2CRC", new Kamaleon2CRC ());
			
			Console.WriteLine ("Calculating crcs. This may take a while...");
			
			foreach (string s in table.Keys){
				inicio = Environment.TickCount;
				CalcularCrc (table[s] as IChecksum);
				fin = Environment.TickCount;
				Console.WriteLine ("{0, -25} time {1}", s, fin - inicio);
			}
			
		}
		public static long CalcularCrc (IChecksum chk)
		{
			chk.Reset ();
			for (int i=0; i < TEST_LENGTH; i++){
				chk.Update (0xF);
			}
			return chk.Value;		
		}	
	}
}
