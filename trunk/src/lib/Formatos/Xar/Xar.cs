/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010
	Alberto Fernández  <infjaf@gmail.com>

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
using Dalle.Archivers.xar;

namespace Dalle.Formatos.Xar
{


	public class Xar : Parte
	{
		
		
		

		public Xar ():base ("xar", "xar", "http://code.google.com/p/xar/", false, false)
		{
		}
		public override bool PuedeUnir (string fichero)
		{
			// TODO : Mejorar la deteccion
			return fichero.ToLower ().EndsWith ("xar");
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			XarArchiveInputStream instream = new XarArchiveInputStream (File.OpenRead (fichero));
			ArchiveExtractor.Extract (instream, dirDest, this, new FileInfo (fichero).Length);
		}
		
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			throw new System.NotImplementedException();
		}
		
		
		
	}
}
