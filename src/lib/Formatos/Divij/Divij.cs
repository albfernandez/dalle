/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Divij.Divij
          Join files in Divij format.
	
    Copyright (C) 2003-2010 Alberto Fernández  <infjaf@gmail.com>

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
using System.Text.RegularExpressions;

using Dalle.Formatos.Generico;
using Dalle.Streams;

namespace Dalle.Formatos.Divij
{


	public class Divij : Parte
	{

		public Divij ()
		{
			nombre = "divij";
			descripcion = "Divij";
			web = "http://www.zzmultimedia.com/software.htm";
			compatible = true;
			parteFicheros = false;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			throw new System.NotImplementedException ();
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			FileInfo fi = new FileInfo (fichero);
			string[] partes = GetPartesNombre (fi.Name);
			JoinInfo info = new JoinInfo ();
			info.Digits = 3;
			info.InitialFragment = 1;
			info.BaseName = partes[0];
			info.OriginalFile = info.BaseName;
			info.FragmentsNumber = Int32.Parse (partes[2]);
			info.Format = info.BaseName + "{0:D3}-" + info.FragmentsNumber;
			info.Directory = fi.Directory;
			info.CalculateLength ();
			
			new ParteGenerico ().Unir (fichero, dirDest, info);

		}
		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero))
			{
				return false;
			}
			return GetPartesNombre (new FileInfo(fichero).Name) != null;
		}
		public static string[] GetPartesNombre (string original)
		{
			string matchExp = @"^(.*)(\d{3})-(\d+)$";
			Match theMatch  = Regex.Match(original,matchExp);
			if (theMatch.Success)
			{
				return new string[]{
					theMatch.Groups[1].Value, 
					theMatch.Groups[2].Value, 
					theMatch.Groups[3].Value
				};				
			}
			return null;

		}

	}
}
