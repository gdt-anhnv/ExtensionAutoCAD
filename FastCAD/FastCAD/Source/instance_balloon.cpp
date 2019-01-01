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

#define STEP_EACH_PART			80.0
void InstanceBalloon::DrawBalloon()
{
	try
	{
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

			ads_point pt1 = { 0.0, 0.0 };
			acedCommandS(RTSTR, L"_.ampartref", RTPOINT, pt1, RTNONE);

			AcDbObjectIdArray ids = GetNextEnts(flag_id);
			source_part_id = ids[0];

			ObjectWrap<AcDbPoint> flag_pnt(DBObject::OpenObjectById<AcDbPoint>(flag_id));
			flag_pnt.object->upgradeOpen();
			flag_pnt.object->erase();

			ads_name ads;
			acedSSAdd(NULL, NULL, ads);

			ads_name tmp;
			acdbGetAdsName(tmp, source_part_id);
			acedSSAdd(tmp, ads, ads);
		}

		for (int i = 0; i < num_balloon; i++)
		{
			{
				ads_name sel;
				acdbGetAdsName(sel, source_part_id);

				ads_point pt2 = { i * STEP_EACH_PART, 0.0 };
				ads_point pt3 = { i * STEP_EACH_PART, 200.0 };
				int ret = acedCommandS(RTSTR, _T("_amballoon"),
					RTLB, RTENAME, sel, RTPOINT, pt2, RTLE,
					RTPOINT, pt3,
					RTSTR, _T(""),
					RTSTR, _T(""),
					RTNONE);
			}

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

				ads_point base_pnt = { i * STEP_EACH_PART, 0.0 };
				std::wstring base_pnt_str = std::wstring();
				base_pnt_str.append(std::to_wstring(i * STEP_EACH_PART));
				base_pnt_str.append(L",");
				base_pnt_str.append(L"0.0");
				ads_point paste_pnt = { (i + 1) * STEP_EACH_PART, 0.0 };
				std::wstring paste_pnt_str = std::wstring();
				paste_pnt_str.append(std::to_wstring((i + 1) * STEP_EACH_PART));
				paste_pnt_str.append(L",0.0");
				acedCommandS(RTSTR, _T("_copybase"),
					RTSTR, _T("0.0,0.0"),
					RTPICKS, ads, RTSTR, _T(""),
					RTSTR, _T(""), RTNONE);
				acedCommandS(RTSTR, _T("_pasteblock"), RTSTR, paste_pnt_str.c_str(), RTSTR, _T(""), RTSTR, _T(""), RTNONE);
				AcDbObjectIdArray ids = GetNextEnts(flag_id);
				if (0 == ids.length())
					throw int(1);
				source_part_id = ids[0];

				ObjectWrap<AcDbPoint> flag_pnt(DBObject::OpenObjectById<AcDbPoint>(flag_id));
				flag_pnt.object->upgradeOpen();
				flag_pnt.object->erase();
			}

		}
	}
	catch (...)
	{
	}
}
