/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.InfoGenerico
	
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
	
using Dalle.Utilidades;

namespace Dalle.Formatos.Generico
{	
	
	public class InfoGenerico 
	{
		
		
		private int initialFragment = 0;
		private int digits = 3;
		private string originalFile = "";
		private string baseName = "";
		private string format = "";
		private DirectoryInfo directory = null;
		private long length = 0;
		private int fragmentsNumber = 0;
		
		private bool isCutKiller = false;
		
		public String OriginalFile 
		{
			get { return originalFile; }
			set { originalFile = value; }
		}
		public int InitialFragment 
		{
			get { return initialFragment; }
			set { initialFragment = value; }
		}
		public int Digits 
		{
			get { return digits; }
			set { digits = value; }
		}
		public string BaseName {
			get { return baseName; }
			set { baseName = value; }
		}
		public string Format {
			get { return format; }
			set { format = value; }
		}
		public DirectoryInfo Directory {
			get { return directory; }
			set { directory = value; }
		}
		public long Length {
			get { return length; }
			set { length = value; }
		}
		public int FragmentsNumber {
			get { return fragmentsNumber; }
			set { fragmentsNumber = value;}
		}
		

		public InfoGenerico ()
		{
		}

		public int GetOffset (int fragment)
		{
			if (isCutKiller && fragment == 1) 
			{
				return 8;
			}
			return 0;
		}
		public string GetFragmentName (int fragment)
		{
			return this.baseName + "." + UtilidadesCadenas.Format (fragment - (1-this.initialFragment), this.Digits);
		}
		public static InfoGenerico GetFromFile (string fichero)
		{
			InfoGenerico info = new InfoGenerico ();
			
			info.baseName = fichero.Substring (0, fichero.LastIndexOf ('.'));
			info.digits = fichero.Length - info.baseName.Length - 1;
			string original = new FileInfo (fichero).Name;
			original = original.Substring (0, original.LastIndexOf ('.'));
			info.originalFile = original;
			info.Directory = new FileInfo (fichero).Directory;
			if (File.Exists (info.BaseName + "." + UtilidadesCadenas.Format (0, info.Digits))) 
			{
				info.initialFragment = 0;
			}
			else if (File.Exists (info.BaseName + "." + UtilidadesCadenas.Format (1, info.Digits)))
			{
				info.initialFragment = 1;
			}
			else {
				return null;
			}
			int contador = 1;
			
			
			while (File.Exists (info.GetFragmentName (contador + 1)))
			{
				FileInfo fi = new FileInfo (info.GetFragmentName (contador + 1));
				info.Length += fi.Length;
				contador++;
			}
			info.fragmentsNumber = contador;
			
			// Comprobar cutkiller
			if (info.initialFragment == 1 && info.Digits == 3) 
			{				
				byte[] buffer = UtilidadesFicheros.LeerSeek (info.GetFragmentName (1), 0, 8);
				string extension = UtArrays.LeerTexto (buffer, 0, 3);
				string fragmentos = UtArrays.LeerTexto (buffer, 3, 5);
				if (extension.Length > 0 && fragmentos.Length == 5) 
				{					
					if (
						Char.IsWhiteSpace (fragmentos[0]) &&
						Char.IsWhiteSpace (fragmentos[1]) &&
						Char.IsDigit (fragmentos[2]) &&
						Char.IsDigit (fragmentos[3]) &&
						Char.IsDigit (fragmentos[4]) && 
						Int32.Parse (fragmentos.Trim ()) == info.FragmentsNumber
		
					) 
					{
						info.isCutKiller = true;
						info.OriginalFile = info.OriginalFile + "." + extension;
						info.Length -= 8;
					}
				}					
			}
			return info;
		}

		
		
	}
}
