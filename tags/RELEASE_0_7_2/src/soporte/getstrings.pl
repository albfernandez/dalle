#!/usr/bin/perl
my $fname = $ARGV[0];
my $linenum = 0;

while (<ARGV>) {
	if ($fname != $ARGV) {
		$linenum = 1;
		$fname = $ARGV;
	} else {
		$linenum++;
	}
	
	if (/I\._\(".*"\)/) {
		@array = split ("\"", $_);
		$k = $array[1];
		$v = $k;
		$k =~ s/ /_/g;
		$k =~ s/=/_/g;
		
		print "# $ARGV: $linenum\n";
		print "$k = $v\n\n";
	}
}
