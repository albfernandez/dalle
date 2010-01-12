
using System;
using System.IO;
using System.Text.RegularExpressions;

using Dalle.Formatos.Generico;

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
			parteFicheros = true;
		}
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			throw new System.NotImplementedException ();
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			FileInfo fi = new FileInfo (fichero);
			string[] partes = GetPartesNombre (fi.Name);
			InfoGenerico info = new InfoGenerico ();
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
