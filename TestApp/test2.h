typedef struct
{
	union
	{
		int a;
		float b;
	} uni;

	struct
	{
		int value;
		int anotherValue;
	} array[2];

	int simpleValue;

	union
	{
		int a;
		float b;
	} uniArray[2];

	union
	{
		int y;
		float z;
	};

	struct
	{
		int m;
		float n;
		struct
		{
			int m2;
			float n2;
		};
	};
} TestStruct;