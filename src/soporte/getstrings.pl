#!/usr/bin/perl

use strict;
#use warnings;

my $fname = $ARGV[0];
my $linenum = 0;
my $k;
my $v;
my @array;

my %totales;
while (<ARGV>) {
	if ($fname ne $ARGV) {
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
		
		
		#print "# $ARGV: $linenum\n";
		#print "$k = $v\n\n";
		
		$totales{"$k = $v"} = $totales{"$k = $v"} . "# $ARGV: $linenum\n";
	}
}
#print "---\n";
my @claves = keys (%totales);
foreach (keys(%totales)) {
	print $totales{$_} . $_ . "\n\n";
}
