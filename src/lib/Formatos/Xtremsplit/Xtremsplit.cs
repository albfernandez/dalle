/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
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
using System.Security.Cryptography;

using Dalle.Formatos;
using Dalle.Utilidades;

namespace Dalle.Formatos.Xstremsplit
{


	public class Xtremsplit : Parte
	{

		public Xtremsplit ()
		{
			nombre = "xtremsplit";
			descripcion = "Xtremsplit";
			web = "http://www.xtremsplit.fr/";
			parteFicheros = false;
			compatible = false;
		
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			XtremsplitInfo info = XtremsplitInfo.GetFromFile (fichero);
			if (info == null)
			{
				throw new FileFormatException ("");
			}
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			long transferidos = 0;
			OnProgress (0, info.Length);

			
			string[] hashesAlmacenados = new string[info.Fragments];
			string[] hashesCalculados = new string[info.Fragments];
			Stream fos = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + info.OriginalFileName);
			for (int i = 1; i <= info.Fragments; i++) 
			{
				Stream fis = File.OpenRead (info.GetFragmentName (i));
				fis.Seek (info.GetOffset (i), SeekOrigin.Begin);
				Stream cis = fis;
				if (info.IsExe && i == 1) 
				{
					cis = new SizeLimiterStream (cis, info.DataSizeInExe);
				}
				
				while ((leidos = cis.Read (buffer, 0, (int)Math.Min (buffer.Length, info.Length - transferidos))) > 0)
				{
					fos.Write (buffer, 0, leidos);
					transferidos += leidos;
					OnProgress (transferidos, info.Length);
				}
				
				if (transferidos == info.Length && info.IsMd5)
				{
					
					byte[] hash = new byte[32];
					for (int j = 0; j < hashesAlmacenados.Length; j++) {
						int lei = fis.Read (hash, 0, hash.Length);
						if (lei == 32) 
						{
							hashesAlmacenados[j] = UtArrays.LeerTexto (hash, 0).ToUpper ();
						}
					}
				}
				
				fis.Close ();
			}
			fos.Close ();
			
			if (info.IsMd5)
			{
				HashAlgorithm md5 = MD5.Create ();
				
				for (int i = 1; i <= info.Fragments; i++) 
				{
					Stream fis = File.OpenRead (info.GetFragmentName (i));
					Stream cis = fis;
					if (info.IsExe && i == 1) {
						fis.Seek (info.GetOffset (i) - XtremsplitInfo.HEADER_SIZE, SeekOrigin.Begin);
						cis = new SizeLimiterStream (cis, info.DataSizeInExe + XtremsplitInfo.HEADER_SIZE);
					}
					if (i == info.Fragments)
					{
						cis = new SizeLimiterStream (cis, new FileInfo (info.GetFragmentName (i)).Length - info.Fragments * 32);
					}
					byte[] res = md5.ComputeHash (cis);
					fis.Close ();
					hashesCalculados[i - 1] = UtilidadesCadenas.FormatHexHash (res).ToUpper ();
				}
				for (int i = 0; i < info.Fragments; i++) 
				{
					if (!hashesCalculados[i].Equals (hashesAlmacenados[i]))
					{
						throw new ChecksumVerificationException (info.GetFragmentName(i+1));
					}
				}
				
				// Comparamos los resultados
			}

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
			if (fichero.ToLower ().EndsWith (".001.exe") || fichero.ToLower ().EndsWith (".001.xtm"))
			{
				return XtremsplitInfo.GetFromFile (fichero) != null;
			}
			return false;
		}

	}
}
