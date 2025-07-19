#ifndef MAIN_H
#define MAIN_H

enum Foo : unsigned int
{
};

typedef unsigned long long uint64;

struct Test
{
	Foo a : 8;
	Foo b : 8;
	Foo c : 8;
	Foo d : 8;
	uint64 x;
	unsigned int e : 8;
	unsigned int f : 25;
	unsigned int g : 8;
	uint64 h;
};

#endif