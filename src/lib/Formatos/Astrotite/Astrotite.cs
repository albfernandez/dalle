/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.Astrotite
          Join files in astrotite format.
	
    Copyright (C) 2004-2010
    Original author (C - code) - Daniel Martínez Contador <dmcontador@terra.es>
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

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
using System.Collections;
using System.Text;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Formatos.Astrotite.v2;

namespace Dalle.Formatos.Astrotite
{

	public struct Descript
	{
		public int length;
		public int blocks;
		public int namelength;
		public string name;
		
	}
	public struct Block 
	{
		public String block;
		public int size;
		public int crc;
	}
	
	public class Astrotite : Parte 
	{
		
		public Astrotite (): base ("astrotite", "Astrotite", "www.fantiusen.com/astrotite.html", false, false)
		{
		}


		protected override void _Unir (string fichero, string dirDest)
		{			

			AstrotiteV2InputStream instream = new AstrotiteV2InputStream (File.OpenRead (fichero));
			ArchiveExtractor.Extract (instream, dirDest, this, new FileInfo (fichero).Length);

				
		}

		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
		}
		
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) {
				return false;
			}
			if (!fichero.ToUpper ().EndsWith (".AST2")) {
				return false;
			}
			
			byte[] initbuffer = new byte[22];
			FileStream astReader = new FileStream (fichero, FileMode.Open);
			
			if (astReader.Read (initbuffer, 0, initbuffer.Length) < 22) {
				astReader.Close ();
				return false;
			}
			astReader.Close ();
			string tmp="";
			for (int i = 0; i < "AST2www.astroteam.tk".Length; i++){
				tmp += Convert.ToChar (initbuffer[i]);
			}
			if (tmp != "AST2www.astroteam.tk"){
				return false;
			}
			return true;
			
		}		
	}	
}
