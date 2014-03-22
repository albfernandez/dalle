/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.InfoGenerico
	
    Copyright (C) 2003-2014  Alberto Fernández <infjaf@gmail.com>

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

namespace Dalle.Streams
{
	public class JoinInfo : IJoinInfo
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
		public bool IsCutKiller {
			get { return this.isCutKiller; }
			set { this.isCutKiller = value; }
		}

		

		public JoinInfo () 
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
			if (this.format != null && !String.Empty.Equals (this.format))
			{
				return String.Format (this.format, (fragment - (1 - this.initialFragment)));
			}
			return this.baseName + UtilidadesCadenas.Format (fragment - (1 - this.initialFragment), this.Digits);
		}
		public string GetFragmentFullPath (int fragment)
		{
			return this.Directory.FullName + Path.DirectorySeparatorChar + GetFragmentName (fragment);

		}

		public void CalculateLength ()
		{

			this.Length = 0;
			for (int i = 1; i <= this.FragmentsNumber; i++) 
			{
				string n = this.GetFragmentFullPath (i);
				if (File.Exists (n)) 
				{
					FileInfo fileInfo = new FileInfo(n);
					this.Length +=  fileInfo.Length - GetOffset(i) ;
				}
			}
		}

		public static JoinInfo GetFromFile (string fichero)
		{
			JoinInfo info = new JoinInfo ();
			info.Directory = new FileInfo (fichero).Directory;
			string original = new FileInfo (fichero).Name;
			string extensionParte = original.Substring (original.LastIndexOf (".")+1);
			info.BaseName = original.Substring (0, original.LastIndexOf ('.') + 1);
			info.OriginalFile = original.Substring (0, original.LastIndexOf ('.'));
			
			// Soporte ultrasplit
			if (extensionParte.StartsWith ("u")) {
				info.BaseName = original.Substring (0, original.LastIndexOf ('.') + 2);
			}
			info.Digits = original.Length - info.BaseName.Length;		
			
			

			
			
			if (File.Exists (info.Directory.FullName + Path.DirectorySeparatorChar + info.BaseName + UtilidadesCadenas.Format (0, info.Digits))) 
			{
				info.InitialFragment = 0;
			}
			else if (File.Exists (info.Directory.FullName + Path.DirectorySeparatorChar + info.BaseName  + UtilidadesCadenas.Format (1, info.Digits)))
			{
				info.InitialFragment = 1;
			}
			else {
				return null;
			}
			int contador = 1;
			
			
			while (File.Exists (info.GetFragmentFullPath (contador)))
			{
				FileInfo fi = new FileInfo (info.GetFragmentFullPath (contador));
				info.Length += fi.Length;
				contador++;
			}
			info.FragmentsNumber = contador - 1;
			
			// Comprobar cutkiller
			if (info.InitialFragment == 1 && info.Digits == 3) 
			{				
				byte[] buffer = UtilidadesFicheros.LeerSeek (info.GetFragmentFullPath(1), 0, 8);
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
						info.IsCutKiller = true;
						info.OriginalFile = info.OriginalFile + "." + extension;
						info.Length -= 8;
					}
				}					
			}
			info.CalculateLength();
			return info;
		}

	}
}
