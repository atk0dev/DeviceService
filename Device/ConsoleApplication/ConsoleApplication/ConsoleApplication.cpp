
#include "stdafx.h"
#include <string.h> 
#include <stdio.h>
#include <stdlib.h>

#ifndef __cplusplus 
typedef char bool;
#define true 1
#define false 0
#endif

char *device_names[3];
char dataToSend[200];

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


typedef struct
{
	list_element header;
	float value;
	char name[40];

} device;


list devices;

void print_data()
{
	for (device * a = (device *)list_begin(&devices); a; a = (device *)list_next(&a->header))
	{
		printf("device: %s - %f \n", a->name, a->value);
	}
}

device* find_device_by_name(char* deviceName)
{
	device* found = 0;

	for (device * a = (device *)list_begin(&devices); a; a = (device *)list_next(&a->header))
	{
		if (strcmp(a->name, deviceName) == 0)
		{
			found = a;
			break;
		}
	}

	return found;
}


void setup()
{
	list_init(&devices);

	print_data();

	printf("empty %d\n", list_empty(&devices));
}




device * read_device_data()
{
	
	float t = (float)rand() / (float)(RAND_MAX / 100);
	int rand_i = rand() % 3;
	char* n = device_names[rand_i];

	device * a = (device *)malloc(sizeof(device));
	a->value = t;
	strcpy_s(a->name, n);
	
	return a;
}


void loop()
{
	printf("\n");
	printf(">\n");

	device* a = read_device_data();
	
	device* existing = find_device_by_name(a->name);


	if (existing == 0)
	{
		list_push_back(&devices, &a->header);
	}
	else
	{
		existing->value = a->value;
		free(a);
	}
	
	
	print_data();

	printf("<\n");
	printf("\n");
}


void buildDataString()
{
	device* a;
	char buffer[20];
	int i = 1;
	strcpy_s(dataToSend, "");
	strcat_s(dataToSend, "DeviceId=");
	
	_itoa_s(1, buffer, 10);

	strcat_s(dataToSend, buffer);

	while (!list_empty(&devices))
	{
		a = (device *)list_pop_front(&devices);
		printf("%d\n", a->value);
		printf("empty %d\n", list_empty(&devices));

		strcat_s(dataToSend, "&Title");
		_itoa_s(i, buffer, 10);
		strcat_s(dataToSend, buffer);
		strcat_s(dataToSend, "=");

		strcat_s(dataToSend, a->name);
		
		strcat_s(dataToSend, "&Value");
		_itoa_s(i, buffer, 10);
		strcat_s(dataToSend, buffer);
		
		strcat_s(dataToSend, "=");

		//sprintf_s(buffer, "%f", a->value);
		
		char str_temp[6];
		dtostrf(a->value, 4, 2, str_temp);
		sprintf(buffer, "%s F", str_temp);

		
		strcat_s(dataToSend, buffer);

		free(a);
		i++;
	}
}

int main()
{
	device_names[0] = "device_1";
	device_names[1] = "device_2";
	device_names[2] = "device_3";

	int i = 0;

	setup();

	while (1)
	{
		loop();
		i++;

		if (i > 50)
		{
			break;
		}
	}

	buildDataString();

	printf("%s\n", dataToSend);

	return 0;

	
	//while (!list_empty(&devices))
	//{
	//	a = (device *)list_pop_front(&devices);
	//	printf("%d\n", a->value);
	//	printf("empty %d\n", list_empty(&devices));
	//	free(a);
	//}


}

