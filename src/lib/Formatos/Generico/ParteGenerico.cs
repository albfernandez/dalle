/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.ParteGenerico
	
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

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Streams;

namespace Dalle.Formatos.Generico
{

	public class ParteGenerico : Parte
	{
		public ParteGenerico()
		{
			nombre = "generic";
			descripcion = "generic";
			web = "_";
			compatible = true;
			parteFicheros= true;
		}
		
		/// <returns>El numero de fragmentos que se han creado.</returns>
		// TODO: Cambiar el orden de los parametros (String, st, long, int, int).
		
		

		
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			
			if ((sal1 == null) || (sal1 == string.Empty))
			{
				sal1 = new FileInfo (fichero).Name;
			}
			JoinInfo info = new JoinInfo ();
			
			info.InitialFragment = 1;
			info.Digits = 3;
			info.OriginalFile = new FileInfo (fichero).Name;
			//info.BaseName = info.OriginalFile;
			info.BaseName = sal1 +".";
			info.Directory = new DirectoryInfo (dir);
			
			
			Stream stream = new SplitStream (
				info, 
				kb * 1024,
				dir + Path.DirectorySeparatorChar + info.BaseName + "sha512sum", 
				"SHA512");
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			long transferidos = 0;
			long tamano = new FileInfo (fichero).Length;
			OnProgress (0, tamano);
			Stream fis = File.OpenRead (fichero);
			while ((leidos = fis.Read (buffer, 0, buffer.Length)) > 0) {
				stream.Write (buffer, 0, leidos);
				transferidos += leidos;
				OnProgress (transferidos, tamano);
			}
			fis.Close ();
			stream.Close ();
			
			//Partir (fichero, sal1, dir, kb, info);
		}
		
		// deprecated
		public int Partir (string fichero, string sal1, string dir, long kb, JoinInfo info)
		{
			long tamano = new FileInfo (fichero).Length;
			long tFragmento = 1024 * kb;
			long transferidos = 0;
			
			Stream fis = File.OpenRead (fichero);
			int leidos = 0;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int contador = 0;
			do {
				
				string fullOutputFilename = info.Directory.FullName +
					Path.DirectorySeparatorChar + 
					info.GetFragmentName (contador + 1);
				Stream fos = UtilidadesFicheros.CreateWriter (fullOutputFilename);
				long parcial = 0;
				while ((leidos = fis.Read (buffer, 0, (int)Math.Min (tFragmento - parcial, buffer.Length))) > 0)
				{
					fos.Write (buffer, 0, leidos);
					parcial += leidos;
					transferidos += leidos;
					OnProgress (transferidos, tamano);
				}
				fos.Close();
				contador++;
			} while (transferidos < tamano);
			fis.Close();
			info.FragmentsNumber = contador;
			return contador;
		}
		
		public void Unir (string fichero, string dirDest, JoinInfo info)
		{
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			long transferidos = 0;
			OnProgress (0, info.Length);
			Stream fos = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + info.OriginalFile);
			Stream fis = new JoinStream (info);
			while ((leidos = fis.Read(buffer, 0, buffer.Length)) > 0) {
				transferidos += leidos;
				fos.Write (buffer, 0, leidos);
				OnProgress (transferidos, info.Length);
			}
			fis.Close ();
			fos.Close ();
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			JoinInfo info = JoinInfo.GetFromFile (fichero);
			Unir (fichero, dirDest, info);
		}


		public override bool PuedeUnir (String fichero)
		{
			if (!File.Exists (fichero))
			{
				return false;
			}
			
			if (fichero.LastIndexOf (".") < 0) {
				return false;
			}
			string bas = fichero.Substring (0, fichero.LastIndexOf ("."));
			
			for (int longitud = 1; longitud <= 5; longitud++) 
			{
				if (File.Exists (bas + "." + UtilidadesCadenas.Format (0, longitud))) 
				{
					return true;
				}
				if (File.Exists (bas + "." + UtilidadesCadenas.Format (1, longitud))) 
				{
					return true;
				}
				
				// Soporte ultraSplitter
				
				if (File.Exists (bas + ".u" + UtilidadesCadenas.Format (1, longitud))) 
				{
					return true;
				}
			}
			return false;
		}		
	}	
}
