
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
		public static void Extract (ArchiveInputStream stream, string outDir, Parte p, long totalData)
		{
			ArchiveEntry e = null;
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			while ((e = stream.GetNextEntry ()) != null) 
			{
				if (e.IsDirectory) {
					Console.WriteLine ("Directory:" + e.Name);
					continue;
				}
				Console.WriteLine (e.Name);
				leidos = 0;
				Stream s = Dalle.Utilidades.UtilidadesFicheros.CreateWriter (outDir + Path.DirectorySeparatorChar + e.Name);
				while ((leidos = stream.Read (buffer)) > 0) 
				{
					s.Write (buffer, 0, leidos);
				}
				if (p != null)
				{
					p.OnProgress (stream.Position, totalData);
				}
				s.Close ();
			
			}
			stream.Close ();
			if (p != null) 
			{
				p.OnProgress (totalData, totalData);
			}
		}
	}
}
