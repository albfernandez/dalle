%define name dalle
%define version 0.7.9
%define release 1
%define prefix /usr

Summary: 		Corta y une archivos en varios formatos.
Name: 			%{name}
Version: 		%{version}
Release: 		%{release}
License: 		GPL
Group: 			Utilities/File
Url: 			http://dalle.sourceforge.net
Vendor:     	Alberto Fernandez  <infjaf00@yahoo.es>
Source: 		%{name}-%{version}.tar.bz2
BuildRoot:  	/var/tmp/%{name}-%{version}
BuildArch:		noarch
Prefix:			%{prefix}
Distribution:   Any
Packager:       Alberto Fernandez  <infjaf00@yahoo.es>
Requires:   	mono-core >= 2.0, gtk-sharp2 >= 2.0

%description

%prep
%setup

%build
./configure --prefix=%{prefix} --build-debug --mono-path=/usr/lib/mono/2.0:/usr/lib/mono/gtk-sharp-2.0
#make
nant -nologo build

%install
#make install DESTDIR=%{buildroot}
nant -nologo -D:DESTDIR=%{buildroot}  install
chmod 755 %{buildroot}/usr/bin/*

%clean
rm -rf %{buildroot}

%post

# Reconstruimos todos los wrappers para poder tenerlo relocalizable.

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-console
echo "mono $RPM_INSTALL_PREFIX/lib/dalle/dalle-console.exe \"\$@\"" >> "$RPM_INSTALL_PREFIX"/bin/dalle-console
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-console

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-gtk
echo "mono $RPM_INSTALL_PREFIX"/lib/dalle/dalle-gtk.exe >> "$RPM_INSTALL_PREFIX"/bin/dalle-gtk
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-gtk


echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk
echo "mono $RPM_INSTALL_PREFIX/lib/dalle/openhacha-gtk.exe" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk
echo "" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk


echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-sfv-console
echo "mono $RPM_INSTALL_PREFIX/lib/dalle/dalle-sfv-console.exe " >> "$RPM_INSTALL_PREFIX"/bin/dalle-sfv-console
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-svf-console



%files
%doc README COPYING ChangeLog NEWS


#/usr/bin/dalle-swf
/usr/bin/openhacha-gtk
/usr/bin/dalle-console
/usr/bin/dalle-gtk
/usr/bin/dalle-sfv-console

/usr/lib/dalle/dalle-sfv-console.exe
#/usr/lib/dalle/dalle-swf.exe
/usr/lib/dalle/dalle-console.exe
/usr/lib/dalle/dalle-gtk.exe
/usr/lib/dalle/openhacha-gtk.exe
/usr/lib/dalle/libDalle.dll

/usr/share/locale/es/LC_MESSAGES/dalle.mo
/usr/share/locale/gl/LC_MESSAGES/dalle.mo

%changelog

* Mon Jan 4 2010 Alberto Fernandez <infjaf00@yahoo.es>
- Version 0.7.9

* Fri Jan 30 2009 Alberto Fernandez <infjaf00@yahoo.es>
- Version 0.7.8

* Sat Jun 11 2005 Alberto Fernandez <infjaf00@yahoo.es>
- Version 0.7.4

* Sun Feb 15 2004 Alberto Fernandez <infjaf00@yahoo.es>
- Version 0.7.1

* Sun Feb 8 2004 Alberto Fernandez <infjaf00@yahoo.es>
- Incluido el nuevo dalle-sfv-console
- Version 0.7.0

* Tue Jan 27 2004 Alberto Fernandez <infjaf00@yahoo.es>
- Version 0.6.0

* Mon Jan 26 2004 Alberto Fernandez <infjaf00@yahoo.es>
- Por fin es relocalizable real.

* Mon Dec 29 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Incluido openhacha-text.
- Construye sin opciones de depuracion.

* Mon Dec 22 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Paquete adaptado al nuevo sistema de construccion.

* Thu Dec 18 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Interfaz grafica actualizada a OpenHacha 0.2.

* Mon Dec 15 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Interfaz grafica basada en OpenHacha 0.1.

* Fri Dec 12 2003 Alberto Fernandez <infjaf00@yahoo.es>
- El paquete RPM es relocalizable.

* Thu Dec 04 2003 Alberto Fernandez <infjaf00@yahoo.es>
- Primer paquete RPM
