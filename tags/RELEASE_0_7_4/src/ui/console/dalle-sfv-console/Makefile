# This program is free software; you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation; either version 2 of the License, or
# (at your option) any later version.
#
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU Library General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA 02111-1307, USA.


BASEPATH=../../../..
include $(BASEPATH)/rules.make
PROGNAME=dalle-sfv-console.exe
WRAPPERNAME=dalle-sfv-console
TARGET=$(BUILDDIR)/$(PROGNAME)
WRAPPER=$(BUILDDIR)/$(WRAPPERNAME)

LANGS=en es
LANGDIR=languages
TRAD=$(LANGDIR)/strings.resources $(addprefix $(LANGDIR)/strings., $(addsuffix .resources, $(LANGS)))
REC_TRAD= $(foreach res,$(TRAD), $(addprefix -resource:,$(res)),$(notdir $(res)))


all: $(TARGET)

$(TARGET): $(LIBRARY) $(TRAD) src/*.cs
	@echo $(LINEA)
	@echo "Compilando interfaz por linea de comandos (dalle-console)."
	@echo $(LINEA)
	$(MCS) $(MCS_FLAGS) $(REC_TRAD) -o $(TARGET) -r $(LIBRARY) "-recurse:src/*.cs"
	@echo "#!/bin/sh" > $(WRAPPER)
	@echo "" >> $(WRAPPER)
	
	@echo -n "$(MONO) $(MONO_FLAGS) $(PREFIX)/bin/$(PROGNAME) " >>$(WRAPPER)
	@echo -n "\"$$" >> $(WRAPPER)
	@echo -n "@\"" >> $(WRAPPER)
	@echo "" >> $(WRAPPER)

$(TRAD):$(LANGDIR)/strings%resources: $(LANGDIR)/strings%txt
	@echo $(LINEA)
	@echo "Generando los recursos de lenguajes: $@."
	@echo $(LINEA)
	install -d $(BUILDDIR)
	$(RESGEN) $< $@


$(LANGDIR)/strings.txt: src/*.cs
	$(GETSTRINGS) src/*.cs > $@

install: $(TARGET)
	$(INSTALL) $(TARGET) $(DESTDIR)/$(PREFIX)/bin
	$(INSTALL) $(WRAPPER) $(DESTDIR)/$(PREFIX)/bin
	
uninstall:
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(WRAPPERNAME)
	$(DELETE) $(DESTDIR)/$(PREFIX)/bin/$(PROGNAME)


clean:
	rm -f $(TARGET) $(WRAPPER) $(LANGDIR)/*.resources

.PHONY: clean uninstall
