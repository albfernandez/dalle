/*

	Dalle - A split/join file utility library
	Dalle.Crcs - A little command-line utility for file verification.		
	
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

using Mono.Unix;

namespace Dalle {

	public class Crcs
	{
	
		public const int BUFFER_SIZE = 2048;
	
		public static void Main(String[] args)
		{
			Dalle.I18N.GettextCatalog.Init();
			if (args.Length == 0){
				MostrarAyuda();
				Environment.Exit (1);
			}
			if (args[0] == "--help"){
				MostrarAyuda();
				Environment.Exit (0);
			}
			foreach (String s in args){
				CalcularCrcs (s);
			}		
			Console.WriteLine ("\n");
		}
		public static void MostrarAyuda()
		{
			//TODO: Show help
			//Help;
		}
		public static void CalcularCrcs (string s)
		{
		
			Console.WriteLine ("\n");
			if (!File.Exists (s)){
				if (!Directory.Exists(s))
					Console.WriteLine (Catalog.GetString("File not found: {0}"), s);
				else
					Console.WriteLine (Catalog.GetString("{0} is a directory"), s);
				return;
			}
		
			Hashtable table = new Hashtable();
			table.Add ("Adler32", new Adler32());
			table.Add ("CRC32", new Crc32());
			table.Add ("StrangeCRC", new StrangeCRC());
			table.Add ("HachaCRC", new HachaCRC (new FileInfo(s).Length));
			table.Add ("SplitFileCRC", new SplitFileCRC());
			table.Add ("Kamaleon2CRC", new Kamaleon2CRC());
			
			foreach (Object ics in table.Values){
				(ics as IChecksum).Reset();
			}
			FileStream reader = File.OpenRead (s);
			byte[] buffer = new byte[BUFFER_SIZE];
			int leidos = 0;
			do{
				leidos = (int) reader.Read (buffer, 0, buffer.Length);
				foreach (IChecksum ics in table.Values)
					ics.Update (buffer, 0, leidos);
			} while (leidos > 0);
			reader.Close();
			
			
			Console.WriteLine (Catalog.GetString("crcs of file {0}:"), s);
			foreach (string key in table.Keys){
				Console.Write ("{0,-25} ", key); 
				Console.WriteLine ("{0}", (table[key] as IChecksum).Value.ToString("X"));
			}
		}
	}
}
