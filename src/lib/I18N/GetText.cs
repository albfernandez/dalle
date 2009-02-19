/*

    Dalle - A split/join file utility library	
    Dalle.I18N.GetText - Internationalization related stuff.
	
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
using System.Resources;
using System.Reflection;
using System.Collections;
using System.Globalization;

namespace Dalle.I18N
{	
	/// <remarks>Clase que se encarga de gestionar los recursos 
	/// de internacionalizaci�n de texto de la aplicaci�n.</remarks>
	
	public class GetText 
	{
		/// <summary> 
		/// Lista de los car�cteres inv�lidos como clave de b�squeda. 
		/// </summary>
		
		private static char [] invalidChars = new Char [] {'=', ' '};		
		
		/// <summary>
		/// Prefijo de los nombres de los recursos de idiomas.
		/// </summary>
		/// <remarks>El formato de los nombres de los recursos localizados
		/// es (prefijo).(codigoLenguage)</remarks>
		
		public const string defaultResourceName = "strings";
	
	
		/// <summary>Recursos del idioma activo.</summary>
		
		private ResourceManager resMan = null;
		
		
		/// <summary>Tabla con las instancias de la clase.</summary>
		/// <remarks>Se crea una entrada en la tabla para cada Assembly
		/// que hace uso de esta clase.</remarks>
		
		private static Hashtable table = new Hashtable();
		
		/// <summary>Constructor privado. No se permite crear instancis 
		/// de esta clase exteriormetne. Patr�n singleton.</summary>
		/// <param name="assembly">El assembly donde se encuentran los recursos
		/// de traducciones que se leer�n.</param>
		
		private GetText (Assembly assembly): this (assembly, defaultResourceName)
		{
		}
		
		/// <summary>Constructor privado. No se permite crear instancias
		/// de esta clase exteriormente. Patr�n singleton.</summary>
		/// <remarks>Intenta cargar los recursos del idioma actual, si no 
		/// carga los que vienen por defecto (strings.resources).</remarks>
		/// <param name="assembly">El assembly donde se encuentran los recursos
		/// de traducciones que se leer�n.</param>
		/// <param name="baseName">El prefijo del nombre de los recursos de
		/// traducciones.</param>
				
		private GetText (Assembly assembly, string baseName)
		{			
			
			string lang = GetLanguage();			
			string resourceName = baseName + "." + lang;
			
			try {
				resMan = new ResourceManager (resourceName, assembly);
				string prueba = "_";
				resMan.GetString (prueba);
			}
			catch (MissingManifestResourceException){
				resMan = new ResourceManager (baseName, assembly);
			}
			catch (Exception){
			}
		}
		
		/// <summary>Obtiene la instancia de la clase correspondiente al 
		/// Assembly que llama</summary>
		/// <returns>La instancia de la clase correspondiente</returns>
		
		
		public static GetText Instance{
			get{ return GetInstance (Assembly.GetCallingAssembly()); }
		}
		
		/// <summary>Obtiene la instancia de la clase correspondiente al 
		/// Assembly que llama</summary>		
		/// <param name="resourceName">El prefijo del nombre de los recursos de
		/// localizaci�n.</param>
		/// <returns>La instancia de la clase correspondiente</returns>
		
		public static GetText GetInstance (string resourceName)
		{
			return GetInstance(	Assembly.GetCallingAssembly(), resourceName);
		}
		
		/// <summary>Obtiene la instancia de la clase correspondiente al 
		/// Assembly dado</summary>		
		/// <param name="assembly">El Assembly que contiene los recursos</param>
		/// <returns>La instancia de la clase correspondiente</returns>
		
		private static GetText GetInstance (Assembly assembly)
		{
			return GetInstance (assembly, defaultResourceName);
		}
		
		/// <summary>Obtiene la instancia de la clase correspondiente al 
		/// Assembly dado</summary>		
		/// <param name="resource">El prefijo del nombre de los recursos de
		/// localizaci�n.</param>
		/// <param name="assembly">El Assembly que contiene los recursos</param>
		/// <returns>La instancia de la clase correspondiente</returns>
		
		private static GetText GetInstance (Assembly assembly, string resource)
		{
			if (!table.ContainsKey(assembly)){
				table.Add(assembly,	new GetText (assembly, resource));
			}
			return table[assembly] as GetText;
		}
		
		/// <summary>Obtiene el lenguaje del sistema.</summary>
		/// <returns>El lenguaje del sistema.</returns>
		
		public static String GetLanguage ()
		{
		
			// Deber�a ser algo as� como
			// return System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName
			// Pero en Mono CultureInfo.CurrentCulture siempre devuelve InvariantCulture,
			// (a no ser que se instale ICU).
			
			if (CultureInfo.CurrentUICulture != CultureInfo.InvariantCulture)
				return CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
		
			String lang =
				System.Environment.GetEnvironmentVariable ("LANG");
			String language =
				System.Environment.GetEnvironmentVariable ("LANGUAGE");

			if (language == null)
				language = lang;
			if ( (language == null) || (language.Length < 2) )
				language = "en";
			
			if (language.Length < 2)
				return "en";
			else
				return language.Substring (0,2);
		}	
				
		/// <summary>
		/// Obtiene una clave de b�squeda v�lida a partir de una dada.
		/// </summary>
		/// <remarks>Sustituye los espacios y los '=' por '_'.</remarks>
		/// <param name="key">La clave de b�squeda</param>
		/// <returns>Una clave de b�squeda v�lida.</returns>
		
		private static string GetMangled (string key)
		{
			string k = key.Replace ('=', '_');
			k = k.Replace (' ', '_');
			return k;
		}
				
		/// <summary>Obtiene el texto para la clave indicada en el idioma
		/// activo. Primero intenta con el idioma del sistema. Si
		/// no est� disponible, devuelve la clave de b�squeda.</summary>
		/// <param name="clave">Cadena de texto con la clave de b�squeda
		/// </param>
		/// <returns>El texto correspondiente localizado.
		/// </returns>
		
		public string GetMessage (string key)
		{
			
			string k = key;
			if (key.IndexOfAny (invalidChars) != -1)
				k = GetMangled (key);
			string result;
			try{
				result = resMan.GetString (k);
			}
			catch (Exception){
				return (key);
			}
			if ( (result == null) || (result == k) )
				return (key);
			else
				return (result);
		}
		
		/// <summary>Obtiene el texto para la clave indicada en el idioma
		/// activo. Primero intenta con el idioma del sistema. Si
		/// no est� disponible, devuelve la clave de b�squeda.</summary>
		/// <param name="clave">Cadena de texto con la clave de b�squeda
		/// </param>
		/// <returns>El texto correspondiente localizado.
		/// </returns>		
		
		public static string _(string clave)
		{
			return GetInstance(Assembly.GetCallingAssembly()).GetMessage(clave);
		}	
	}
}
