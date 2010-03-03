/*

    Dalle - A split/join file utility library	
	
    Copyright (C) 2010  Alberto Fernández <infjaf@gmail.com>

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

using Dalle.Archivers;

namespace Dalle.Formatos
{


	public class ArchiveExtractor
	{

		public ArchiveExtractor ()
		{
		}
		public static void Extract (ArchiveInputStream stream, string outDir, Parte p)
		{
			Extract (stream, outDir, p, -1);	
		}
		public static void Extract (ArchiveInputStream stream, string outDir, Parte p, long totalData)
		{
			ArchiveEntry e = null;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			p.OnProgress (0, 1);
			while ((e = stream.GetNextEntry ()) != null) 
			{
				
				if (e.IsDirectory) {
					continue;
				}
				leidos = 0;
			
				Stream s = Dalle.Utilidades.UtilidadesFicheros.CreateWriter (outDir + Path.DirectorySeparatorChar + e.Name);
				while ((leidos = stream.Read (buffer)) > 0) 
				{
					s.Write (buffer, 0, leidos);
					
					if (leidos > 0 && p != null) {
						if (totalData > 0) {
							p.OnProgress (stream.Position, totalData);
						} else if (stream.Length > 0) {
							p.OnProgress (stream.Position, stream.Length);
						}
					}
				}
				
				s.Close ();
			
			}
			if (p != null) {
				if (totalData > 0) {
					p.OnProgress (stream.Position, totalData);
				} else if (stream.Length > 0) {
					p.OnProgress (stream.Position, stream.Length);
				} else {
					p.OnProgress (1, 1);
				}
			}
			stream.Close ();

		}
	}
}
