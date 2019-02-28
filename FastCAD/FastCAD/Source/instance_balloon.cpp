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

static AcDbObjectIdArray GetNextEnts(const AcDbObjectId & id)
{
	AdsNameWrap flag;
	acdbGetAdsName(flag.ads, id);

	AcDbObjectIdArray ids = AcDbObjectIdArray();
	{
		AcDbObjectId tmp_id = AcDbObjectId::kNull;
		do
		{
			acdbEntNext(flag.ads, flag.ads);

			tmp_id = AcDbObjectId::kNull;

			if (Acad::eOk == acdbGetObjectId(tmp_id, flag.ads))
			{
				ids.append(tmp_id);
			}

		} while (AcDbObjectId::kNull != tmp_id);
	}
	return ids;
}

struct BalloonData
{
	AcDbObjectId part_id;
	AcGePoint3d position;

	BalloonData(AcDbObjectId pid, AcGePoint3d pos) :
		part_id(pid),
		position(pos)
	{}
};

static void Draw(std::list<BalloonData>& test_ids);
std::list<BalloonData> test_ids = std::list<BalloonData>();
#define STEP_EACH_PART			80.0
void InstanceBalloon::DrawBalloon()
{
	try
	{
		test_ids.clear();
		int num_balloon = 0;
		num_balloon = UserFuncs::GetInt(L"Number balloon:");

		AcDbObjectId source_part_id = AcDbObjectId::kNull;
		{
			AcDbObjectId flag_id = AcDbObjectId::kNull;
			{
				ObjectWrap<AcDbPoint> flag_wrap(new AcDbPoint());
				ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
				model_space.object->upgradeOpen();

				model_space.object->appendAcDbEntity(flag_wrap.object);
				flag_id = flag_wrap.object->id();
			}

			ads_point pt1 = { 20.0, 20.0 };
			acedCommandS(RTSTR, L"_.ampartref", RTPOINT, pt1, RTNONE);

			AcDbObjectIdArray ids = GetNextEnts(flag_id);
			source_part_id = ids[0];
			test_ids.push_back(BalloonData(ids[0], AcGePoint3d(20.0, 20.0, 0.0)));

			ObjectWrap<AcDbPoint> flag_pnt(DBObject::OpenObjectById<AcDbPoint>(flag_id));
			flag_pnt.object->upgradeOpen();
			flag_pnt.object->erase();
		}

		for (int i = 0; i < num_balloon; i++)
		{
			AcDbObjectId flag_id = AcDbObjectId::kNull;
			{
				ObjectWrap<AcDbPoint> flag_wrap(new AcDbPoint());
				ObjectWrap<AcDbBlockTableRecord> model_space(DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase()));
				model_space.object->upgradeOpen();

				model_space.object->appendAcDbEntity(flag_wrap.object);
				flag_id = flag_wrap.object->id();
			}

			ads_name ads;
			acedSSAdd(NULL, NULL, ads);

			ads_name tmp;
			acdbGetAdsName(tmp, source_part_id);
			acedSSAdd(tmp, ads, ads);
			acedCommandS(RTSTR, _T("_copybase"),
				RTSTR, _T("20.0,20.0"),
				RTPICKS, ads, RTSTR, _T(""),
				RTSTR, _T(""), RTNONE);

			//ads_point paste_pnt = { (i + 1) * STEP_EACH_PART + 20.0, 20.0 };
			std::wstring paste_pnt_str = std::wstring();
			paste_pnt_str.append(std::to_wstring((i + 1) * STEP_EACH_PART + 20.0));
			paste_pnt_str.append(L",20.0");
			acedCommandS(RTSTR, _T("_pasteclip"), RTSTR, paste_pnt_str.c_str(), RTSTR, _T(""), RTSTR, _T(""), RTNONE);
			AcDbObjectIdArray ids = GetNextEnts(flag_id);
			if (0 == ids.length())
				throw int(1);
			test_ids.push_back(BalloonData(ids[0], AcGePoint3d((i + 1) * STEP_EACH_PART + 20.0, 20.0, 0.0)));

			ObjectWrap<AcDbPoint> flag_pnt(DBObject::OpenObjectById<AcDbPoint>(flag_id));
			flag_pnt.object->upgradeOpen();
			flag_pnt.object->erase();
		}

		//acedCommandS(RTSTR, _T("_TEST"), RTNONE);

		Draw(test_ids);

		ObjectWrap<AcDbObject> last_part(DBObject::OpenObjectById<AcDbObject>(test_ids.back().part_id));
		if (nullptr != last_part.object)
		{
			last_part.object->upgradeOpen();
			last_part.object->erase();
		}
	}
	catch (...)
	{
	}
}

void InstanceBalloon::Test()
{
	Draw(test_ids);
}

static void Draw(std::list<BalloonData>& test_ids)
{
	for (auto iter = test_ids.begin(); iter != test_ids.end(); iter++)
	{
		ads_name sel;
		acdbGetAdsName(sel, iter->part_id);

		ads_point pt2 = { iter->position.x, 20.0 };
		ads_point pt3 = { iter->position.x, 200.0 };
		std::wstring pt3_str = std::wstring(std::to_wstring(iter->position.x));
		pt3_str.append(L",200.0");
		int ret = acedCommandS(RTSTR, _T("_amballoon"),
			RTLB, RTENAME, sel, RTPOINT, pt2, RTLE,
			//RTPOINT, pt3,
			RTSTR, pt3_str.c_str(),
			RTSTR, _T(""),
			RTSTR, _T(""),
			RTNONE);
	}
}
