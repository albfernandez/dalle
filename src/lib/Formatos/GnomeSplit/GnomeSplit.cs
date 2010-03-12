/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2004-2010
    Original java code: commons-commpress, from apache (http://commons.apache.org/compress/)
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
using System.Security.Cryptography;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Streams;

namespace Dalle.Formatos.GnomeSplit
{


	public class GnomeSplit : Parte
	{

		
		

		public GnomeSplit ()
		{
			nombre = "gnomesplit";
			descripcion = "Gnome Split";
			web = "http://www.gnome-split.org/index.html";
			parteFicheros = true;
			compatible = false;
	
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			throw new System.NotImplementedException ();
		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) 
			{
				return false;
			}
			return fichero.ToLower ().EndsWith (".gsp");
		}
		
		protected override void _Unir (string fichero, string dirDest)
		{
			OnProgress (0, 1);
			string nombre = new FileInfo (fichero).Name.Replace (".001.gsp", "");
			string baseName = fichero.Replace (".001.gsp", "");
			string fSalida = dirDest + Path.DirectorySeparatorChar + nombre;
			Stream salida = UtilidadesFicheros.CreateWriter (fSalida);
			HashStream hs = new HashStream (salida, MD5.Create ());
			
			int i = 0;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int bytesRead = 0;
			long parcial = 0;
			long total = 0;
			int fragmentosTotales = 0;
			bool hasMd5 = false;
			string fileMd5 = "";
			string headerVersion;
			string headerFileName;
			
			for (i = 1; File.Exists (String.Format (baseName + ".{0:000}.gsp", i)); i++)
			{
				Stream fileInStream = File.OpenRead (String.Format (baseName + ".{0:000}.gsp", i));				
				Stream inStream = fileInStream;
				
				if (i == 1) 
				{
					// Read header
					bytesRead = inStream.Read (buffer, 0, 69);
					if (bytesRead != 69) 
					{
						throw new IOException ("Premature end of file");
					}
					hasMd5 = (buffer[0x38] != 0);
					fragmentosTotales = UtArrays.LeerInt32BE (buffer, 0x39);
					headerVersion = UtArrays.LeerTexto (buffer, 1, 4);
					headerFileName = UtArrays.LeerTexto (buffer, 6, 50);
					total = UtArrays.LeerInt64BE(buffer, 0x3D);					
				}
				
				if (i == fragmentosTotales && hasMd5) 
				{
					inStream = new SizeLimiterStream (inStream, total - parcial);
				}
				
				while ((bytesRead = inStream.Read (buffer, 0, buffer.Length)) > 0) 
				{
					hs.Write (buffer, 0, bytesRead);
					parcial += bytesRead;
					OnProgress(parcial, total);
				}
				if (i == fragmentosTotales && hasMd5) {
					bytesRead = fileInStream.Read (buffer, 0, 32);
					if (bytesRead != 32) {
						throw new IOException ("Premature end of file");
					}
					fileMd5 = UtArrays.LeerTexto (buffer, 0, 32);					
				}
				fileInStream.Close ();
			}
			hs.Close ();
			if (hasMd5 && !fileMd5.ToLower().Equals (hs.Hash.ToLower()))
			{
				throw new IOException ("md5 verification failed");
			}
			OnProgress (parcial, total);
		}		
	}
}
