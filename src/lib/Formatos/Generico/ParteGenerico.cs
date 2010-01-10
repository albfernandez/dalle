/*

	Dalle - A split/join file utility library
	Dalle.Formatos.Generico.ParteGenerico
	
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

using Dalle.Formatos;
using Dalle.Utilidades;

namespace Dalle.Formatos.Generico
{

	public class ParteGenerico : Parte
	{
		public ParteGenerico()
		{
			nombre = "generico";
			descripcion = "generico";
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
			InfoGenerico info = new InfoGenerico ();
			
			info.InitialFragment = 1;
			info.Digits = 3;
			info.OriginalFile = new FileInfo(fichero).Name;
			info.BaseName = info.OriginalFile;
			info.Directory = new DirectoryInfo (dir);
			
			
			Partir (fichero, sal1, dir, kb, info);
		}
		public  int Partir (string fichero, string sal1, string dir, long kb, InfoGenerico info)
		{
			long tamano = new FileInfo (fichero).Length;
			long tFragmento = 1024 * kb;
			long transferidos = 0;
			
			Stream fis = File.OpenRead(fichero);
			int leidos = 0;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int contador = 0;
			do {
				Stream fos = UtilidadesFicheros.CreateWriter (info.GetFragmentName(contador+1));
				int parcial = 0;
				while ((leidos = fis.Read (buffer, 0, Math.Min ((int)tFragmento - parcial, buffer.Length))) > 0)
				{
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
		
		protected override void _Unir (string fichero, string dirDest)
		{
			InfoGenerico info = InfoGenerico.GetFromFile (fichero);
			
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			long transferidos = 0;
			OnProgress (0, info.Length);
			Stream fos = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + info.OriginalFile);
			for (int i = 1; i <= info.FragmentsNumber; i++) 
			{
				Stream fis = File.OpenRead (info.GetFragmentName (i));
				fis.Seek (info.GetOffset (i), SeekOrigin.Begin);
				while ((leidos = fis.Read (buffer, 0, buffer.Length)) > 0) 
				{
					transferidos += leidos;
					fos.Write (buffer, 0, leidos);
					OnProgress (transferidos, info.Length);
				}
				fis.Close ();
			}
			fos.Close ();

		}

		/*
		public void Unir (string formato, int ini, int digitos)
		{
			Unir (formato, formato.Substring (0, formato.LastIndexOf('.')), 
				ini, digitos);
		}
		public void Unir (string formato, string destino, int ini, int digitos)
		{
			InfoGenerico info = new InfoGenerico ();
			
			info.Formato = formato;
			info.Digitos = digitos;
			info.Fragmento = ini;
			
			long total = 0;
			while (File.Exists (info.ToString ())) {
				total += new FileInfo (info.ToString ()).Length;
				info.Fragmento++;
			}
			
			long transferidos = 0;
			info.Fragmento = ini;
			OnProgress (0, total);
			
			FileStream fos = UtilidadesFicheros.CreateWriter (destino);
			int leidas = 0;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			while (File.Exists (info.ToString ())) {
				// TODO Tamaño de buffer
				FileStream fin = File.OpenRead (info.ToString ());
				while ((leidas = fin.Read (buffer, 0, buffer.Length)) > 0) {
					fos.Write (buffer, 0, leidas);
					transferidos += leidas;
					OnProgress (transferidos, total);
				}
				fin.Close ();
				info.Fragmento++;
				
			}
			fos.Close ();
		}
		*/

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
			}
			return false;
		}		
	}	
}
