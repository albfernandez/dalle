
MONO = mono
MCS = mcs
MAKEWEB = MakeWeb.exe
TEMPLATE = src/template/index.html.in
SOURCEDIR = src
DESTDIR = deploy

all: $(MAKEWEB) $(SOURCEDIR)/NEWS $(SOURCEDIR)/web.xml $(TEMPLATE) $(DESTDIR)/styles.css
	$(MONO) $(MAKEWEB) $(SOURCEDIR)/web.xml $(TEMPLATE)  $(DESTDIR)

$(MAKEWEB) : MakeWeb.cs
	$(MCS) MakeWeb.cs
$(DESTDIR)/styles.css: $(SOURCEDIR)/template/styles.css
	cp $< $@
	
$(SOURCEDIR)/NEWS: ../../../NEWS
	cp $< $@
	
clean:
	rm -f $(DESTDIR)/*.html *.exe

upload:
	 scp -r deploy/*.* infjaf@shell.sf.net:/home/groups/d/da/dalle/htdocs
	

.PHONY: clean
