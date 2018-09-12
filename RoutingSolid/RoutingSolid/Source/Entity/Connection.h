#ifndef _CONNECTION_H_
#define _CONNECTION_H_

#include "Node.h"

class Connection
{
public:
	Node *first;
	Node *second;

public:
	Connection();
	~Connection();
};


#endif // !_CONNECTION_H_