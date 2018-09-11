#include "ToolArxFuncs.h"

static bool IsRightOrder(const AcDbObjectId& id1, const AcDbObjectId& id2)
{
	ObjectWrap<AcDbText> txt1(DBObject::OpenObjectById<AcDbText>(id1));
	ObjectWrap<AcDbText> txt2(DBObject::OpenObjectById<AcDbText>(id2));

	return txt1.object->position().y > txt2.object->position().y;
}

static std::wstring GetRetStr(const std::wstring& ls, const std::wstring& us, std::wstring ds, const std::wstring& ns)
{
	std::wstring ret = std::wstring(L"***-");

	if (ls.length() > 0)
	{
		ret.append(ls);
		ret.append(L"-");
	}

	if (us.length() > 0)
	{
		ret.append(us);
		ret.append(L"-");
	}

	{
		do
		{
			wchar_t wc = ds.at(ds.length() - 1);
			if (L' ' == wc)
				ds.erase(ds.length() - 1);
			else
				break;
		} while (1);
	}

	ret.append(ds);

	if (0 != ns.length())
	{
		ret.append(L"-");
		ret.append(ns);
	}

	{
		do
		{
			wchar_t wc = ret.at(ret.length() - 1);
			if (L' ' == wc)
				ret.erase(ret.length() - 1);
			else
				break;
		} while (1);
	}

	while (std::wstring::npos != ret.find(L"  "))
	{
		size_t index = ret.find(L"  ");
		ret.erase(index, 1);
	}

	while (std::wstring::npos != ret.find(L" "))
	{
		ret[ret.find(L" ")] = L'-';
	}

	return std::move(ret);
}

static AcDbObjectIdArray NearRightTexts(const AcDbObjectId& br_id)
{
	AcDbExtents extents = AcDbExtents();
	{
		ObjectWrap<AcDbEntity> br = DBObject::OpenObjectById<AcDbEntity>(br_id);
		br.object->getGeomExtents(extents);
		double h = extents.maxPoint().y - extents.minPoint().y;
		double w = extents.maxPoint().x - extents.minPoint().x;

		extents.set(extents.minPoint(), AcGePoint3d(extents.maxPoint().x + w * 0.25, extents.maxPoint().y - h * 0.5, 0.0));
	}

	{
		AcDbObjectIdArray ids = ARXFuncs::GetObjIdsInWindow(extents.maxPoint(), extents.minPoint());
		for (int i = 0; i < ids.length(); )
		{
			ObjectWrap<AcDbText> txt = DBObject::OpenObjectById<AcDbText>(ids.at(i));
			if (nullptr == txt.object)
				ids.removeAt(i);
			else
				i++;
		}

		return ids;
	}

	return AcDbObjectIdArray();
}

static AcDbObjectIdArray NearLeftTexts(const AcDbObjectId& br_id)
{
	AcDbExtents extents = AcDbExtents();
	{
		ObjectWrap<AcDbEntity> ent = DBObject::OpenObjectById<AcDbEntity>(br_id);
		ent.object->getGeomExtents(extents);
		double h = extents.maxPoint().y - extents.minPoint().y;
		double w = extents.maxPoint().x - extents.minPoint().x;

		extents.set(AcGePoint3d(extents.minPoint().x - w, extents.minPoint().y + h * 0.5, 0.0), extents.maxPoint());
	}

	{
		AcDbObjectIdArray ids = ARXFuncs::GetObjIdsInWindow(extents.maxPoint(), extents.minPoint());
		for (int i = 0; i < ids.length(); )
		{
			ObjectWrap<AcDbText> txt = DBObject::OpenObjectById<AcDbText>(ids.at(i));
			if (nullptr == txt.object)
				ids.removeAt(i);
			else
				i++;
		}

		return ids;
	}

	return AcDbObjectIdArray();
}

static AcDbObjectIdArray GetEntsInsideEnt(const AcDbObjectId& id)
{
	AcDbExtents extents = AcDbExtents();

	{
		ObjectWrap<AcDbEntity> ent(DBObject::OpenObjectById<AcDbEntity>(id));
		if (nullptr == ent.object)
			throw int(1);

		ent.object->getGeomExtents(extents);
	}

	AcDbObjectIdArray ids = ARXFuncs::GetObjIdsInWindow(extents.maxPoint(), extents.minPoint());
	for (int i = 0; i < ids.length(); )
	{
		ObjectWrap<AcDbText> txt(DBObject::OpenObjectById<AcDbText>(ids.at(i)));
		if (nullptr == txt.object)
		{
			ids.removeAt(i);
			continue;
		}

		AcDbExtents sub_extents = AcDbExtents();
		txt.object->getGeomExtents(sub_extents);
		AcGePoint3d cen_pnt = AcGePoint3d(
			(sub_extents.minPoint().x + sub_extents.maxPoint().x) * 0.5,
			(sub_extents.minPoint().y + sub_extents.maxPoint().y) * 0.5,
			(sub_extents.minPoint().z + sub_extents.maxPoint().z) * 0.5);

		if (cen_pnt.x < extents.minPoint().x || cen_pnt.x > extents.maxPoint().x ||
			cen_pnt.y < extents.minPoint().y || cen_pnt.y > extents.maxPoint().y)
		{
			ids.removeAt(i);
			continue;
		}
		i++;
	}

	return ids;
}

static std::list<std::wstring> ParseTexts(std::wstring txt)
{
	std::list<std::wstring> ret = std::list<std::wstring>();

	while (std::wstring::npos != txt.find(L","))
	{
		ret.push_back(txt.substr(0, txt.find(L",")));
		txt = txt.substr(txt.find(L",") + 1);
	}

	ret.push_back(txt);

	for (std::list<std::wstring>::iterator iter = ret.begin(); iter != ret.end(); )
	{
		if (3 == iter->length() && std::wstring::npos != iter->find(L"-"))
		{
			std::wstring tmp_txt = *iter;
			ret.erase(iter);

			//parse
			{
				wchar_t fc = tmp_txt.at(0);
				wchar_t lc = tmp_txt.at(2);

				int delta = std::abs(fc - lc);
				int min = fc > lc ? lc : fc;

				for (int i = 0; i < delta + 1; i++)
				{
					std::wstring wt = L"1";
					wt[0] = min + i;
					ret.push_back(wt);
				}
			}

			continue;
		}

		iter++;
	}

	return ret;
}

static bool IsInsideOtherEnt(const AcDbObjectId& ent_id)
{
	ObjectWrap<AcDbText> txt = DBObject::OpenObjectById<AcDbText>(ent_id);
	if (nullptr == txt.object)
		return false;

	AcDbObjectIdArray ids = AcDbObjectIdArray();
	AcGePoint3d checkout_pnt = AcGePoint3d();
	{
		AcDbExtents extents = AcDbExtents();
		txt.object->getGeomExtents(extents);

		checkout_pnt.x = (extents.minPoint().x + extents.maxPoint().x) * 0.5;
		checkout_pnt.y = (extents.minPoint().y + extents.maxPoint().y) * 0.5;
		checkout_pnt.z = (extents.minPoint().z + extents.minPoint().z) * 0.5;

		double w = extents.maxPoint().x - extents.minPoint().x;
		double h = extents.maxPoint().y - extents.minPoint().y;

		extents.set(AcGePoint3d(extents.minPoint().x - w, extents.minPoint().y - h, 0.0),
			AcGePoint3d(extents.maxPoint().x + w, extents.maxPoint().y + h, 0.0));

		ids = ARXFuncs::GetObjIdsInWindow(extents.maxPoint(), extents.minPoint());
	}

	for (int i = 0; i < ids.length(); i++)
	{
		if (ids.at(i) == ent_id)
			continue;

		ObjectWrap<AcDbEntity> ent = DBObject::OpenObjectById<AcDbEntity>(ids.at(i));
		if (ent.object->isKindOf(AcDbBlockReference::desc()) || ent.object->isKindOf(AcDbCircle::desc()))
		{
			AcDbExtents extents = AcDbExtents();
			ent.object->getGeomExtents(extents);
			if (extents.minPoint().x < checkout_pnt.x && extents.maxPoint().x > checkout_pnt.x &&
				extents.minPoint().y < checkout_pnt.y && extents.maxPoint().y > checkout_pnt.y)
				return true;
		}
	}

	return false;
}

static std::list<std::wstring> GetTexts(const AcDbObjectId& ent_id)
{
	AcDbObjectIdArray ids = GetEntsInsideEnt(ent_id);
	if (ids.isEmpty())
		return std::list<std::wstring>();

	AcDbObjectId txt_id1 = AcDbObjectId::kNull;
	AcDbObjectId txt_id2 = AcDbObjectId::kNull;
	for (int i = 0; i < ids.length(); i++)
	{
		ObjectWrap<AcDbText> txt = DBObject::OpenObjectById<AcDbText>(ids.at(i));
		if (nullptr == txt.object)
			continue;

		if (AcDbObjectId::kNull == txt_id1)
			txt_id1 = ids.at(i);
		else if (AcDbObjectId::kNull == txt_id2)
			txt_id2 = ids.at(i);
		else
			break;
	}

	if (AcDbObjectId::kNull == txt_id1 || AcDbObjectId::kNull == txt_id2)
	{
		if (AcDbObjectId::kNull != txt_id1)
		{
			std::wstring txt = L"I-";
			ObjectWrap<AcDbText> tmp_ent = DBObject::OpenObjectById<AcDbText>(txt_id1);
			txt.append(tmp_ent.object->textString());
			std::list<std::wstring> rets = std::list<std::wstring>();
			rets.push_back(txt);
			return rets;
		}

		return std::list<std::wstring>();
	}

	bool is_right_order = IsRightOrder(txt_id1, txt_id2);

	std::wstring upper_txt = std::wstring();
	std::wstring down_txt = std::wstring();
	std::wstring near_right_txt = std::wstring();
	std::wstring near_left_txt = std::wstring();
	{
		ObjectWrap<AcDbText> txt1 = DBObject::OpenObjectById<AcDbText>(txt_id1);
		ObjectWrap<AcDbText> txt2 = DBObject::OpenObjectById<AcDbText>(txt_id2);

		upper_txt = is_right_order ? txt1.object->textString() : txt2.object->textString();
		down_txt = is_right_order ? txt2.object->textString() : txt1.object->textString();

		{
			AcDbObjectIdArray near_txt_ids = NearRightTexts(ent_id);
			for (int i = 0; i < near_txt_ids.length(); )
			{
				if (near_txt_ids.at(i) == txt_id1 || near_txt_ids.at(i) == txt_id2)
					near_txt_ids.removeAt(i);
				else
					i++;
			}
			if (0 != near_txt_ids.length())
			{
				ObjectWrap<AcDbText> near_txt_ent = DBObject::OpenObjectById<AcDbText>(near_txt_ids.at(0));
				near_right_txt = near_txt_ent.object->textString();
			}
		}

		{
			AcDbObjectIdArray near_txt_ids = NearLeftTexts(ent_id);
			for (int i = 0; i < near_txt_ids.length(); )
			{
				if (near_txt_ids.at(i) == txt_id1 || near_txt_ids.at(i) == txt_id2 || IsInsideOtherEnt(near_txt_ids.at(i)))
					near_txt_ids.removeAt(i);
				else
					i++;
			}

			for (int i = 0; i < near_txt_ids.length(); i++)
			{
				ObjectWrap<AcDbText> near_txt_ent = DBObject::OpenObjectById<AcDbText>(near_txt_ids.at(i));
				near_left_txt.append(near_txt_ent.object->textString());
			}
		}
	}

	std::list<std::wstring> parse_near_txt = ParseTexts(near_right_txt);

	std::list<std::wstring> ret_txt = std::list<std::wstring>();
	if (0 == parse_near_txt.size())
		ret_txt.push_back(GetRetStr(near_left_txt, upper_txt, down_txt, near_right_txt));
	else
	{
		for (std::list<std::wstring>::iterator iter = parse_near_txt.begin(); iter != parse_near_txt.end(); iter++)
			ret_txt.push_back(GetRetStr(near_left_txt, upper_txt, down_txt, *iter));
	}
	return ret_txt;
}

void ToolArxFuncs::GetTextNear()
{
	try
	{
		AcDbObjectIdArray picked_ids = UserFuncs::UserGetEnts();
		if (0 == picked_ids.length())
			return;

		AcGePoint3d pos = UserFuncs::UserGetPoint(L"Chọn điểm insert:");

		double txt_height = 1.0;
		for (int i = 0; i < picked_ids.length(); i++)
		{
			ObjectWrap<AcDbText> txt_ent(DBObject::OpenObjectById<AcDbText>(picked_ids.at(i)));
			if (nullptr != txt_ent.object)
			{
				txt_height = txt_ent.object->height();
				break;
			}
		}

		for (int i = 0; i < picked_ids.length(); i++)
		{
			AcDbObjectId tmp_id = picked_ids.at(i);
			{
				bool is_br = false;
				{
					ObjectWrap<AcDbBlockReference> br_wrap(DBObject::OpenObjectById<AcDbBlockReference>(tmp_id));
					if (nullptr != br_wrap.object)
						is_br = true;
				}

				bool is_circle = false;
				{
					ObjectWrap<AcDbCircle> cir_wrap(DBObject::OpenObjectById<AcDbCircle>(tmp_id));
					if (nullptr != cir_wrap.object)
						is_circle = true;
				}

				if (!(is_br || is_circle))
					continue;
			}

			std::list<std::wstring> txts = GetTexts(tmp_id);

			ObjectWrap<AcDbBlockTableRecord> model_space = DBObject::GetModelSpace(acdbHostApplicationServices()->workingDatabase());
			model_space.object->upgradeOpen();
			for (std::list<std::wstring>::iterator iter = txts.begin(); iter != txts.end(); iter++)
			{
				AcDbText* ret_ent = new AcDbText(pos, iter->c_str());
				if (Acad::eOk == model_space.object->appendAcDbEntity(ret_ent))
				{
					ret_ent->setHeight(txt_height);
					ret_ent->close();
				}
				else
					delete ret_ent;

				double h = ret_ent->height();
				pos.y -= ret_ent->height() * 1.5;
			}
		}
	}
	catch (...) {}
}