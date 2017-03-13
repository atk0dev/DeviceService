
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

typedef struct
{
	list_element header;
	float value;
	char name[40];

} device;
