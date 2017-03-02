#include "stdafx.h"
#include <string.h> 

#ifndef __cplusplus 
typedef char bool;
#define true 1
#define false 0
#endif

struct list_element
{
	struct list_element * next;
};
typedef struct list_element list_element;

typedef struct
{
	list_element * first;
	list_element * last;
} list;

void list_init(list * container)
{
	container->first = 0;
	container->last = 0;
}

bool list_empty(list * container)
{
	return 0 == container->first;
}

list_element * list_begin(list * container)
{
	return container->first;
}

list_element * list_next(list_element * element)
{
	return element->next;
}

list_element * list_pop_front(list * container)
{
	list_element * element = container->first;
	container->first = container->first->next;
	return element;
}

void list_push_back(list * container, list_element * element)
{
	if (list_empty(container))
	{
		container->first = element;
		container->last = element;
	}
	else
	{
		container->last->next = element;
		container->last = element;
	}

	element->next = 0;
}

#include <stdio.h>
#include <stdlib.h>

typedef struct
{
	list_element header;
	float value;
	char* name;

} apple;


list apples;

void print_data()
{
	for (apple * a = (apple *)list_begin(&apples); a; a = (apple *)list_next(&a->header))
	{
		printf("apple: %s %d\n", a->name, a->value);
	}
}

apple* find_apple_by_name(char* appleName)
{
	apple* found = 0; 

	for (apple * a = (apple *)list_begin(&apples); a; a = (apple *)list_next(&a->header))
	{
		if (strcmp(a->name, appleName) == 0)
		{
			found = a;
			break;
		}
		
	}

	return found;
}


void setup()
{
	list_init(&apples);

	//for (int i = 0; i < deviceCount; i++)
	//{
	//	apple * a = (apple *)malloc(sizeof(apple));

	//	a->value = 0;
	//	a->name = "unknown";

	//	list_push_back(&apples, &a->header);
	//}

	print_data();

	/*
	apple * b = (apple *)malloc(sizeof(apple));
	apple * c = (apple *)malloc(sizeof(apple));

	

	b->value = 2;
	b->name = "b";

	c->value = 3;
	c->name = "c";

	

	
	list_push_back(&apples, &b->header);
	list_push_back(&apples, &c->header);
*/
	printf("empty %d\n", list_empty(&apples));
}

apple * read_device_data()
{
	float t = 10;
	char* n = "dev_1";

	apple * a = (apple *)malloc(sizeof(apple));
	a->value = t;
	a->name = n;
	
	return a;
}


void loop()
{
	printf("\n");
	printf(">\n");

	apple* a = read_device_data();
	
	apple* existing = find_apple_by_name(a->name);


	if (existing == 0)
	{
		list_push_back(&apples, &a->header);
	}
	else
	{
		existing->value = a->value;
	}
	
	
	print_data();


	printf("<\n");
	printf("\n");
}

int main()
{
	int i = 0;

	setup();

	while (1)
	{
		loop();
		i++;

		if (i > 5)
		{
			break;
		}
	}

	return 0;

	
	

	


	

	



	
	//while (!list_empty(&apples))
	//{
	//	a = (apple *)list_pop_front(&apples);
	//	printf("%d\n", a->value);
	//	printf("empty %d\n", list_empty(&apples));
	//	free(a);
	//}


}
