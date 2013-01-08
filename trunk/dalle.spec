%define name dalle
%define version 1.12.11
%define release 1
%define prefix /usr

Summary: 		Corta y une archivos en varios formatos.
Name: 			%{name}
Version: 		%{version}
Release: 		%{release}
License: 		GPL
Group: 			Utilities/File
Url: 			http://dalle.sourceforge.net
Vendor:     	Alberto Fernandez  <infjaf@gmail.com>
Source: 		%{name}-%{version}.tar.bz2
BuildRoot:  	/var/tmp/%{name}-%{version}
BuildArch:		noarch
Prefix:			%{prefix}
Distribution:   Any
Packager:       Alberto Fernandez  <infjaf@gmail.com>
Requires:   	mono-core >= 2.0, gtk-sharp2 >= 2.0

%description

%prep
%setup

%build

nant -nologo build

%install

nant -nologo -D:DESTDIR=%{buildroot}  install
chmod 755 %{buildroot}/usr/bin/*



# menu-entry
%__install -dm 755 %{buildroot}%{_datadir}/applications

%__cat > %{buildroot}%{_datadir}/applications/%{name}-gtk.desktop << EOF
[Desktop Entry]
Encoding=UTF-8
Name=%{name}
Comment=Corta y une archivos en varios formatos.
Exec=/usr/bin/dalle-gtk
Icon=dalle.xpm
Terminal=false
Type=Application
Categories=Utility;
EOF

%__install -dm 755 %{buildroot}%{_datadir}/icons/
%__install -m 644 pixmaps/dalle.xpm %{buildroot}%{_datadir}/icons/dalle.xpm

%clean
rm -rf %{buildroot}

%post

# Reconstruimos todos los wrappers para poder tenerlo relocalizable.

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-console
echo "mono --debug $RPM_INSTALL_PREFIX/lib/dalle/dalle-console.exe \"\$@\"" >> "$RPM_INSTALL_PREFIX"/bin/dalle-console
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-console

echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-gtk
echo "mono --debug $RPM_INSTALL_PREFIX"/lib/dalle/dalle-gtk.exe >> "$RPM_INSTALL_PREFIX"/bin/dalle-gtk
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-gtk


echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk
echo "mono --debug $RPM_INSTALL_PREFIX/lib/dalle/openhacha-gtk.exe" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk
echo "" >> "$RPM_INSTALL_PREFIX"/bin/openhacha-gtk


echo "#!/bin/sh" > "$RPM_INSTALL_PREFIX"/bin/dalle-sfv-console
echo "mono --debug $RPM_INSTALL_PREFIX/lib/dalle/dalle-sfv-console.exe " >> "$RPM_INSTALL_PREFIX"/bin/dalle-sfv-console
echo "" >> "$RPM_INSTALL_PREFIX"/bin/dalle-svf-console



%files
%doc README COPYING ChangeLog NEWS


/usr/bin/openhacha-gtk
/usr/bin/dalle-console
/usr/bin/dalle-gtk
/usr/bin/dalle-sfv-console

/usr/lib/dalle/dalle-sfv-console.exe
/usr/lib/dalle/dalle-console.exe
/usr/lib/dalle/dalle-gtk.exe
/usr/lib/dalle/openhacha-gtk.exe
/usr/lib/dalle/libDalle.dll
/usr/lib/dalle/liblzma.dll

/usr/share/locale/es/LC_MESSAGES/dalle.mo
/usr/share/locale/gl/LC_MESSAGES/dalle.mo

/usr/share/applications/dalle-gtk.desktop
/usr/share/icons/dalle.xpm

%changelog

* Mon Jan 7 2013 Alberto Fernandez <infjaf@gmail.com>
- Version 1.13.01

* Wed Nov 14 2012 Alberto Fernandez <infjaf@gmail.com>
- Version 1.12.11

* Mon May 28 2012 Alberto Fernandez <infjaf@gmail.com>
- Version 1.12.04

* Sat Apr 9 2011 Alberto Fernandez <infjaf@gmail.com>
- Version 0.10.11

* Tue Nov 2 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.10.11

* Tue Oct 19 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.10.10

* Wed Feb 3 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.10.4

* Wed Feb 3 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.10.1

* Mon Jan 25 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.9.1
- LZMA support

* Thu Jan 21 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.9.0

* Mon Jan 4 2010 Alberto Fernandez <infjaf@gmail.com>
- Version 0.7.9

* Fri Jan 30 2009 Alberto Fernandez <infjaf@gmail.com>
- Version 0.7.8

* Sat Jun 11 2005 Alberto Fernandez <infjaf@gmail.com>
- Version 0.7.4

* Sun Feb 15 2004 Alberto Fernandez <infjaf@gmail.com>
- Version 0.7.1

* Sun Feb 8 2004 Alberto Fernandez <infjaf@gmail.com>
- Incluido el nuevo dalle-sfv-console
- Version 0.7.0

* Tue Jan 27 2004 Alberto Fernandez <infjaf@gmail.com>
- Version 0.6.0

* Mon Jan 26 2004 Alberto Fernandez <infjaf@gmail.com>
- Por fin es relocalizable real.

* Mon Dec 29 2003 Alberto Fernandez <infjaf@gmail.com>
- Incluido openhacha-text.
- Construye sin opciones de depuracion.

* Mon Dec 22 2003 Alberto Fernandez <infjaf@gmail.com>
- Paquete adaptado al nuevo sistema de construccion.

* Thu Dec 18 2003 Alberto Fernandez <infjaf@gmail.com>
- Interfaz grafica actualizada a OpenHacha 0.2.

* Mon Dec 15 2003 Alberto Fernandez <infjaf@gmail.com>
- Interfaz grafica basada en OpenHacha 0.1.

* Fri Dec 12 2003 Alberto Fernandez <infjaf@gmail.com>
- El paquete RPM es relocalizable.

* Thu Dec 04 2003 Alberto Fernandez <infjaf@gmail.com>
- Primer paquete RPM
