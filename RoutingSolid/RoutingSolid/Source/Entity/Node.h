#ifndef _NODE_H_
#define _NODE_H_

#include "stdafx.h"
#include <list>
#include "Connection.h"

class Node
{
private:
	AcGePoint3d position;
public:
	std::list<Connection*> connections;
public:
	Node();
	~Node();

	AcGePoint3d GetPos() const;
};

#endif // !_NODE_H_
