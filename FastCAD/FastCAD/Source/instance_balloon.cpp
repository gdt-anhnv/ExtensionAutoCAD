#include "../stdafx.h"
#include "instance_balloon.h"
#include "acad_mechanical_inc.h"

#include "../../AcadFuncs/Source/acad_funcs_header.h"
#include "../../AcadFuncs/Source/Wrap/acad_obj_wrap.h"

#include <iostream>
#include <list>
#include <vector>

InstanceBalloon::InstanceBalloon()
{
}

InstanceBalloon::~InstanceBalloon()
{
}

void InstanceBalloon::DrawBalloon()
{
	ads_point pt1 = { 0.0, 0.0 };
	//acedCommandS(RTSTR, L"_.insert", RTSTR, L"_ACMFILLEDHALF", RTPOINT, pt1, RTNONE);
	acedCommandS(RTSTR, L"_.ampartref", RTPOINT, pt1, RTNONE);
}
