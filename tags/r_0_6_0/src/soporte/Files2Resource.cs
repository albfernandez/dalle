/*

	Dalle - A split/join file utility library
	Files2Resource - Convert a set of files to a byte[] resource file.
	
    Copyright (C) 2003  Alberto Fernández <infjaf00@yahoo.es>

    This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation; either version 2 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program; if not, write to the Free Software
    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

*/




using System;
using System.IO;
using System.Resources;

/// <remarks><para>Esta clase se encarga de leer un conjunto de archivos
/// y guardarlos como arrays de bytes en un archivo de recursos.</para>
/// <para>Útil para guardar imágenes como recursos en el assembly.
/// </para></remarks>

public class Files2Resource
{

	/// <summary>Constructor privado. No se permite crear instancias
	/// de esta clase.</summary>
	
	private Files2Resource ()
	{
	}

	/// <summary>Función principal del programa.</summary>
	/// <param name="args">Los argumentos pasados desde la línea de
	/// comandos. El último de ellos es el fichero de recursos a generar
	/// y los anteriores los ficheros a incluir.</param>
	
	public static void Main (String[] args)
	{

		if (args.Length < 2){
			Console.WriteLine ("Parámetros incorrectos");
			Console.WriteLine ("<archivos a incluir> <archivo destino>");
			Environment.Exit (-1);
		}
		String ficheroSalida = args[args.Length - 1];
		ResourceWriter rsw = new ResourceWriter (ficheroSalida);
		for (int i = 0; i < args.Length - 1; i++){
			try{
				byte[]img = CargarFichero (args[i]);
				rsw.AddResource (CrearNombre (args[i]), img);
			}
			catch (FileNotFoundException ex){
				Console.WriteLine 
					("ERROR. Fichero no encontrado: {0}", args[i]);
				rsw.Close ();
				File.Delete (ficheroSalida);
				Environment.Exit (-1);
			}
		}
		rsw.Close ();
		Console.WriteLine 
			("Leído{1} {0} archivo{1} (recurso{1}).", 
			args.Length - 1,
			(args.Length > 2) ? "s" : ""
			);
		Console.WriteLine 
			("Escrito el archivo de recursos {0}",  ficheroSalida);
	}
	
	/// <summary>Crea un nombre para el recurso a partir del nombre
	/// del fichero de imagen de origen.</summary>
	/// <param name="fichero">El nombre del archivo.</param>
	/// <returns>El nombre para el recurso generado a partir de <c>
	/// fichero</c>.</returns>
	
	public static String CrearNombre (String fichero){
		int pos = fichero.LastIndexOf ('/');
		return fichero.Substring (pos + 1);
	}
	
	/// <summary>Lee un fichero y devuelve su contenido como un array
	/// de bytes.</summary>
	/// <param name="filename">El nombre del fichero a leer.</param>
	/// <returns>El contenido del fichero en un array de bytes.</returns>
	
	public static byte[] CargarFichero (String filename){
		byte[]contenido = null;
		FileStream file = File.OpenRead (filename);
		BinaryReader br = new BinaryReader (file);
		contenido = br.ReadBytes ((int) file.Length);
		return contenido;
	}
}
