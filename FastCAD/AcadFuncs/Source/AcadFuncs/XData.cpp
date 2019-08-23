#include "XData.h"
#include "../wrap_header.h"
#include "../AcadFuncs/AcadFuncs.h"
#include "DBObject.h"
#include "../constants.h"

void XDataFuncs::AddXDataIntVal(const AcDbObjectId & id,
	const std::wstring & app_name, const int & value)
{
	if (AcDbObjectId::kNull == id)
		return;
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));

	if (NULL == obj_wrap.object)
		return;

	if (!Functions::HasRegAppName(acdbCurDwg(), app_name.c_str()))
		Functions::AddRegAppName(acdbCurDwg(), app_name.c_str());

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	if (NULL == res_wrap.res_buf)
	{
		res_wrap.res_buf = acutBuildList(
			AcDb::kDxfRegAppName, app_name.c_str(),
			RTNONE);

		struct resbuf* new_rb = acutNewRb(AcDb::kDxfXdInteger32);
		new_rb->resval.rint = value;
		res_wrap.res_buf->rbnext = new_rb;
	}
	else
	{
		resbuf* tmp_rb = res_wrap.res_buf;

		if (NULL != tmp_rb)
		{
			while (NULL != tmp_rb->rbnext)
			{
				tmp_rb = tmp_rb->rbnext;
			}
		}

		struct resbuf* new_rb = acutNewRb(AcDb::kDxfXdInteger32);
		new_rb->resval.rlong = value;
		tmp_rb->rbnext = new_rb;
	}
	obj_wrap.object->upgradeOpen();
	obj_wrap.object->setXData(res_wrap.res_buf);
	obj_wrap.object->downgradeOpen();
}

void XDataFuncs::AddXDataHandleVal(const AcDbObjectId & id,
	const std::wstring & app_name, const std::wstring& value)
{
	if (AcDbObjectId::kNull == id)
		return;
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));

	if (NULL == obj_wrap.object)
		return;

	if (!Functions::HasRegAppName(acdbCurDwg(), app_name.c_str()))
		Functions::AddRegAppName(acdbCurDwg(), app_name.c_str());

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	if (NULL == res_wrap.res_buf)
	{
		res_wrap.res_buf = acutBuildList(
			AcDb::kDxfRegAppName, app_name.c_str(),
			RTNONE);

		struct resbuf* new_rb = acutBuildList(
			AcDb::kDxfXdHandle, value.c_str(), RTNONE);
		res_wrap.res_buf->rbnext = new_rb;
	}
	else
	{
		resbuf* tmp_rb = res_wrap.res_buf;

		if (NULL != tmp_rb)
		{
			while (NULL != tmp_rb->rbnext)
			{
				tmp_rb = tmp_rb->rbnext;
			}
		}

		struct resbuf* new_rb = acutBuildList(
			AcDb::kDxfXdHandle, value.c_str(), RTNONE);
		tmp_rb->rbnext = new_rb;
	}
	obj_wrap.object->upgradeOpen();
	obj_wrap.object->setXData(res_wrap.res_buf);
	obj_wrap.object->downgradeOpen();
}

bool XDataFuncs::CheckXDataAppname(const AcDbObjectId & id, const std::wstring & app_name)
{
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return false;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));
	return NULL != res_wrap.res_buf ? true : false;
}

void XDataFuncs::AddXDataWstringVal(const AcDbObjectId & id,
	const std::wstring & app_name, const ACHAR * value)
{
	Acad::ErrorStatus ret = eOk;
	if (AcDbObjectId::kNull == id)
		return;
	ObjectWrap<AcDbObject>obj_wrap(OpenObjectId<AcDbObject>(id));

	if (NULL == obj_wrap.object)
		return;

	if (!Functions::HasRegAppName(acdbCurDwg(), app_name.c_str()))
		Functions::AddRegAppName(acdbCurDwg(), app_name.c_str());

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	if (NULL == res_wrap.res_buf)
	{

		res_wrap.res_buf = acutBuildList(
			AcDb::kDxfRegAppName, app_name.c_str(),
			RTNONE);

		struct resbuf* new_rb = acutBuildList(
			AcDb::kDxfXdAsciiString, value,
			RTNONE);

		res_wrap.res_buf->rbnext = new_rb;
	}
	else
	{
		resbuf* tmp_rb = res_wrap.res_buf;

		if (NULL != tmp_rb)
		{
			while (NULL != tmp_rb->rbnext)
			{
				tmp_rb = tmp_rb->rbnext;
			}
		}

		struct resbuf* new_rb = acutBuildList(
			AcDb::kDxfXdAsciiString, value,
			RTNONE);

		tmp_rb->rbnext = new_rb;
	}
	ret = obj_wrap.object->upgradeOpen();
	ret = obj_wrap.object->setXData(res_wrap.res_buf);
	ret = obj_wrap.object->downgradeOpen();
}

bool XDataFuncs::CheckXDataIntVal(const AcDbObjectId & id, const std::wstring & app_name, const int & value)
{
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return false;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	/*if (NULL != res_wrap.res_buf)
		tmp_res = res_wrap.res_buf->rbnext;*/

	while (NULL != tmp_res)
	{
		if ((AcDb::kDxfXdInteger16 == tmp_res->restype || AcDb::kDxfXdInteger32 == tmp_res->restype) && value == tmp_res->resval.rint)
			return true;
		tmp_res = tmp_res->rbnext;
	}
	return false;
}

bool XDataFuncs::CheckXDataDoubleVal(const AcDbObjectId & id, const std::wstring & app_name, const double & value)
{
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return false;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	//if (NULL != res_wrap.res_buf)
	//	tmp_res = res_wrap.res_buf->rbnext;

	while (NULL != tmp_res)
	{
		if (AcDb::kDxfXdReal == tmp_res->restype &&  value == tmp_res->resval.rreal)
			return true;
		tmp_res = tmp_res->rbnext;
	}
	return false;
}

bool XDataFuncs::CheckXDataStringVal(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & value)
{
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return false;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	//if (NULL != res_wrap.res_buf)
	//	tmp_res = res_wrap.res_buf->rbnext;

	while (NULL != tmp_res)
	{
		if (AcDb::kDxfXdAsciiString == tmp_res->restype && value == tmp_res->resval.rstring)
			return true;
		tmp_res = tmp_res->rbnext;
	}
	return false;
}

void XDataFuncs::SetXData(AcDbDatabase* db, const AcDbObjectId & id, const resbuf * xdata)
{
	AcDbRegAppTable* reg_app_tbl = NULL;
	if (Acad::eOk == db->getRegAppTable(reg_app_tbl, AcDb::kForWrite))
	{
		if (Adesk::kFalse == reg_app_tbl->has(xdata->resval.rstring))
		{
			AcDbRegAppTableRecord *reg_tbl_rcd = new AcDbRegAppTableRecord;
			reg_tbl_rcd->setName(xdata->resval.rstring);
			Acad::ErrorStatus err = reg_app_tbl->add(reg_tbl_rcd);
			if (Acad::eDuplicateRecordName == err)
			{
				reg_tbl_rcd->erase(true);
			}
			reg_tbl_rcd->close();
		}
		reg_app_tbl->close();
	}

	ObjectWrap<AcDbEntity> ent(OpenObjectId<AcDbEntity>(id));
	ent.object->upgradeOpen();

	Acad::ErrorStatus err = ent.object->setXData(xdata);
	ent.object->close();
}

void XDataFuncs::AddXDataDoubleVal(const AcDbObjectId & id, const std::wstring & app_name, const double & value)
{
	if (AcDbObjectId::kNull == id)
		return;
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));

	if (NULL == obj_wrap.object)
		return;

	if (!Functions::HasRegAppName(acdbCurDwg(), app_name.c_str()))
		Functions::AddRegAppName(acdbCurDwg(), app_name.c_str());

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	if (NULL == res_wrap.res_buf)
	{
		res_wrap.res_buf = acutBuildList(
			AcDb::kDxfRegAppName, app_name.c_str(),
			RTNONE);

		struct resbuf* new_rb = acutNewRb(AcDb::kDxfXdReal);
		new_rb->resval.rreal = value;
		res_wrap.res_buf->rbnext = new_rb;
	}
	else
	{
		resbuf* tmp_rb = res_wrap.res_buf;

		if (NULL != tmp_rb)
		{
			while (NULL != tmp_rb->rbnext)
			{
				tmp_rb = tmp_rb->rbnext;
			}
		}

		struct resbuf* new_rb = acutNewRb(AcDb::kDxfXdReal);
		new_rb->resval.rreal = value;
		tmp_rb->rbnext = new_rb;
	}
	obj_wrap.object->upgradeOpen();
	obj_wrap.object->setXData(res_wrap.res_buf);
	obj_wrap.object->downgradeOpen();
}

void XDataFuncs::DeleteXData(const AcDbObjectId & id, const std::wstring & app_name)
{
	ObjectWrap<AcDbEntity> ent_wrap(OpenObjectId<AcDbEntity>(id));
	if (NULL == ent_wrap.object)
		return;
	ent_wrap.object->upgradeOpen();
	ResBufWrap xdata(ent_wrap.object->xData(app_name.c_str()));
	if(NULL != xdata.res_buf)
	{
		resbuf *xdata_next = xdata.res_buf->rbnext;
		xdata.res_buf->rbnext = NULL;
		ent_wrap.object->setXData(xdata.res_buf);
		xdata.res_buf->rbnext = xdata_next;
	}
}

bool XDataFuncs::RemoveXData(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & xdata_tag)
{
	bool ret = false;

	ObjectWrap<AcDbEntity> ent_wrap(OpenObjectId<AcDbEntity>(id));
	if (ent_wrap.object == NULL)
	{
		return false;
	}

	ResBufWrap rb_wrap(ent_wrap.object->xData(app_name.c_str()));
	if (rb_wrap.res_buf == NULL)
	{
		return false;
	}
	resbuf *pre_res = rb_wrap.res_buf;
	resbuf* tmp_res = rb_wrap.res_buf->rbnext;

	while (NULL != tmp_res)
	{
		if (tmp_res->restype == AcDb::kDxfXdAsciiString
			&& tmp_res->resval.rstring == xdata_tag)
		{
			if (NULL != tmp_res->rbnext)
			{

				pre_res->rbnext = tmp_res->rbnext->rbnext;

				//free resbuf
				{
					resbuf * tmp = tmp_res;
					tmp->rbnext->rbnext = NULL;
					acutRelRb(tmp_res);
				}

				tmp_res = pre_res;
			}
		}
		pre_res = tmp_res;
		tmp_res = tmp_res->rbnext;
	}
	ErrorStatus err = ent_wrap.object->upgradeOpen();
	if (err == ErrorStatus::eOk)
	{
		err = ent_wrap.object->setXData(rb_wrap.res_buf);
		return true;
	}
	return ret;

}

bool XDataFuncs::ChangeXdataDoubleVal(AcDbObjectId id,
	const std::wstring & app_name, const std::wstring & xdata_tag, const double & value)
{
	ObjectWrap<AcDbEntity> ent_wrap(OpenObjectId<AcDbEntity>(id));
	if (ent_wrap.object == NULL)
	{
		return false;
	}

	ResBufWrap rb_wrap(ent_wrap.object->xData(app_name.c_str()));
	if (rb_wrap.res_buf == NULL)
	{
		return false;
	}

	{
		resbuf* tmp_res = rb_wrap.res_buf->rbnext;
		bool is_existed = false;
		while (NULL != tmp_res)
		{
			if (tmp_res->restype == AcDb::kDxfXdAsciiString
				&& wcscmp(tmp_res->resval.rstring, xdata_tag.c_str()) == 0)
			{
				is_existed = true;
				break;
			}
			tmp_res = tmp_res->rbnext;
		}
		if (!is_existed)
			return false;
		ResBufWrap new_resbuf(acutBuildList(
			AcDb::kDxfXdReal, value,
			RTNONE));
		{
			resbuf * old_resbuf = tmp_res->rbnext;
			new_resbuf.res_buf->rbnext = old_resbuf->rbnext;
			tmp_res->rbnext = new_resbuf.res_buf;
			old_resbuf->rbnext = NULL;
			acutRelRb(old_resbuf);
		}
	}
	ErrorStatus err = ent_wrap.object->upgradeOpen();
	if (err == ErrorStatus::eOk)
	{
		ent_wrap.object->setXData(rb_wrap.res_buf);
		return true;
	}
	return false;
}

bool XDataFuncs::ChangeXdataIntVal(AcDbObjectId id,
	const std::wstring & app_name, const std::wstring & xdata_tag, const int & value)
{
	bool ret = false;

	ObjectWrap<AcDbEntity> ent_wrap(OpenObjectId<AcDbEntity>(id));
	if (ent_wrap.object == NULL)
	{
		return false;
	}

	ResBufWrap rb_wrap(ent_wrap.object->xData(app_name.c_str()));
	if (rb_wrap.res_buf == NULL)
	{
		return false;
	}

	{
		resbuf* tmp_res = rb_wrap.res_buf->rbnext;
		while (NULL != tmp_res)
		{
			if (tmp_res->restype == AcDb::kDxfXdAsciiString
				&& wcscmp(tmp_res->resval.rstring, xdata_tag.c_str()) == 0)
			{
				tmp_res = tmp_res->rbnext;
				if (tmp_res->restype == AcDb::kDxfXdInteger32)
				{
					tmp_res->resval.rlong = value;
					ret = true;
				}
				break;
			}
			tmp_res = tmp_res->rbnext;
		}
	}
	ErrorStatus err = ent_wrap.object->upgradeOpen();
	if (err == ErrorStatus::eOk)
	{
		err = ent_wrap.object->setXData(rb_wrap.res_buf);
		return true;
	}
	return ret;
}

void XDataFuncs::AddXdataHandle(const AcDbObjectId & des_id,
	const std::wstring & app_name, AcDbObjectId source_id)
{
	if (AcDbObjectId::kNull == des_id)
		return;

	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(des_id));
	if (NULL == obj_wrap.object)
		return;

	if (!Functions::HasRegAppName(acdbCurDwg(), app_name.c_str()))
		Functions::AddRegAppName(acdbCurDwg(), app_name.c_str());

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	if (NULL == res_wrap.res_buf)
	{
		res_wrap.res_buf = acutBuildList(
			AcDb::kDxfRegAppName, app_name.c_str(),
			RTNONE);

		struct resbuf* new_rb = acutBuildList(
			AcDb::kDxfXdHandle,
			DBObject::GetHandleStr(source_id.handle()).constPtr(),
			RTNONE);

		res_wrap.res_buf->rbnext = new_rb;
	}
	else
	{
		resbuf* tmp_rb = res_wrap.res_buf;

		if (NULL != tmp_rb)
		{
			while (NULL != tmp_rb->rbnext)
			{
				tmp_rb = tmp_rb->rbnext;
			}
		}

		struct resbuf* new_rb = acutBuildList(
			AcDb::kDxfXdHandle,
			DBObject::GetHandleStr(source_id.handle()).constPtr(),
			RTNONE);
		tmp_rb->rbnext = new_rb;
	}
	ErrorStatus ret = ErrorStatus::eOk;
	ret = obj_wrap.object->upgradeOpen();
	ret = obj_wrap.object->setXData(res_wrap.res_buf);
	ret = obj_wrap.object->downgradeOpen();
}

AcDbObjectId XDataFuncs::GetObjId(resbuf * rb)
{
	ads_name entries = { 0, 0 };
	acdbHandEnt(rb->resval.rstring, entries);
	AcDbObjectId obj_id = AcDbObjectId::kNull;
	acdbGetObjectId(obj_id, entries);

	return obj_id;
}

AcDbObjectIdArray XDataFuncs::GetObjectsByXdataHandle(const AcDbObjectId & id,
	const std::wstring & app_name, const std::wstring & handle_tag)
{
	ObjectWrap<AcDbEntity> ent(OpenObjectId<AcDbEntity>(id));
	if (NULL == ent.object)
		return AcDbObjectIdArray();

	ResBufWrap rb_wrap(ent.object->xData(app_name.c_str()));
	if (NULL == rb_wrap.res_buf)
		return AcDbObjectIdArray();

	struct resbuf* tmp = rb_wrap.res_buf;
	while (NULL != tmp->rbnext)
	{
		if (AcDb::kDxfXdAsciiString == tmp->restype &&
			0 == std::wstring(handle_tag).compare(tmp->resval.rstring))
		{
			AcDbObjectIdArray ids = AcDbObjectIdArray();
			struct resbuf* link_rb = tmp->rbnext;
			while (NULL != link_rb && AcDb::kDxfXdHandle == link_rb->restype)
			{
				ids.append(XDataFuncs::GetObjId(link_rb));
				link_rb = link_rb->rbnext;
			}
			return ids;
		}
		tmp = tmp->rbnext;
	}
	return AcDbObjectIdArray();
}

bool XDataFuncs::AddXDataAppName(const AcDbObjectId & id, const std::wstring & app_name)
{
	bool ret = false;

	if (AcDbObjectId::kNull == id)
		return false;
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));

	if (NULL == obj_wrap.object)
		return false;

	if (!Functions::HasRegAppName(acdbCurDwg(), app_name.c_str()))
		Functions::AddRegAppName(acdbCurDwg(), app_name.c_str());

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	if (NULL == res_wrap.res_buf)
	{
		res_wrap.res_buf = acutBuildList(
			AcDb::kDxfRegAppName, app_name.c_str(),
			RTNONE);
	}
	else
		return false;
	
	obj_wrap.object->upgradeOpen();
	Acad::ErrorStatus err = obj_wrap.object->setXData(res_wrap.res_buf);
	obj_wrap.object->downgradeOpen();
	return Acad::eOk == ret;
}

std::wstring XDataFuncs::GetXData(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & tag)
{
	std::wstring result = L"";
	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return result;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	/*if (NULL != res_wrap.res_buf)
		tmp_res = res_wrap.res_buf->rbnext;*/

	while (NULL != tmp_res)
	{
		if (AcDb::kDxfXdAsciiString == tmp_res->restype && tag == tmp_res->resval.rstring)
		{
			struct resbuf* next_res = tmp_res->rbnext;

			if (NULL != next_res)
			{


				if (AcDb::kDxfXdAsciiString == next_res->restype)
				{
					result = next_res->resval.rstring;
				}

				if (AcDb::kDxfXdInteger16 == next_res->restype)
				{
					//result = next_res->resval.int;
				}
			}

		}
		tmp_res = tmp_res->rbnext;
	}
	return result;
}

std::list<std::wstring> XDataFuncs::GetXDataWstringVal(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & tag)
{
	std::list<std::wstring> result = std::list<std::wstring>();

	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return result;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	/*if (NULL != res_wrap.res_buf)
	tmp_res = res_wrap.res_buf->rbnext;*/

	while (NULL != tmp_res)
	{
		if (AcDb::kDxfXdAsciiString == tmp_res->restype && tag == tmp_res->resval.rstring)
		{
			struct resbuf* next_res = tmp_res->rbnext;

			if (NULL != next_res)
			{
				if (AcDb::kDxfXdAsciiString == next_res->restype)
				{
					result.push_back(next_res->resval.rstring);
				}
			}

		}
		tmp_res = tmp_res->rbnext;
	}
	return result;
}

std::list<int> XDataFuncs::GetXDataIntVal(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & tag)
{
	std::list<int> result = std::list<int>();

	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return result;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	/*if (NULL != res_wrap.res_buf)
	tmp_res = res_wrap.res_buf->rbnext;*/

	while (NULL != tmp_res)
	{
		if ((AcDb::kDxfXdAsciiString == tmp_res->restype || AcDb::kDxfRegAppName == tmp_res->restype) && tag == tmp_res->resval.rstring)
		{
			struct resbuf* next_res = tmp_res->rbnext;

			if (NULL != next_res)
			{
				if (AcDb::kDxfXdInteger16 == next_res->restype || AcDb::kDxfXdInteger32 == next_res->restype)
				{
					result.push_back(next_res->resval.rint);
				}
			}

		}
		tmp_res = tmp_res->rbnext;
	}
	return result;
}

std::list<double> XDataFuncs::GetXDataDoubleVal(const AcDbObjectId & id, const std::wstring & app_name, const std::wstring & tag)
{
	std::list<double> result = std::list<double>();

	ObjectWrap<AcDbObject> obj_wrap(OpenObjectId<AcDbObject>(id));
	if (NULL == obj_wrap.object)
		return result;

	ResBufWrap res_wrap(obj_wrap.object->xData(app_name.c_str()));

	struct resbuf* tmp_res = res_wrap.res_buf;
	/*if (NULL != res_wrap.res_buf)
	tmp_res = res_wrap.res_buf->rbnext;*/

	while (NULL != tmp_res)
	{
		if ((AcDb::kDxfXdAsciiString == tmp_res->restype || AcDb::kDxfRegAppName == tmp_res->restype) && tag == tmp_res->resval.rstring)
		{
			struct resbuf* next_res = tmp_res->rbnext;

			if (NULL != next_res)
			{
				if (AcDb::kDxfXdReal == next_res->restype)
				{
					result.push_back(next_res->resval.rreal);
				}
			}
		}
		tmp_res = tmp_res->rbnext;
	}
	return result;
}

AcDbObjectIdArray XDataFuncs::FindObjectsByXData(const std::wstring & app_name, const std::wstring & val)
{
	AcDbObjectIdArray result = AcDbObjectIdArray();

	IteratorWrap<AcDbBlockTableRecordIterator> iter = NULL;
	{
		WrapBlkTblRcd rcd_wrap(
			DBObject::GetModelSpace(acdbCurDwg()));
		if (NULL == rcd_wrap.object)
			return NULL;

		rcd_wrap.object->newIterator(iter.pointer);
	}
	while (!iter.pointer->done())
	{
		AcDbObjectId ent_id = AcDbObjectId::kNull;
		iter.pointer->getEntityId(ent_id);

		if (true == CheckXDataStringVal(ent_id, app_name, val))
			result.append(ent_id);

		iter.pointer->step();
	}
	return result;
}

bool XDataFuncs::ChangeXDataWstringVal(const AcDbObjectId & id, const std::wstring & app_name,
	const std::wstring & xdata_tag, const std::wstring & val)
{
	bool ret = false;

	ObjectWrap<AcDbEntity> ent_wrap(OpenObjectId<AcDbEntity>(id));
	if (ent_wrap.object == NULL)
		return false;

	ResBufWrap rb_wrap(ent_wrap.object->xData(app_name.c_str()));
	if (NULL == rb_wrap.res_buf)
		return false;

	{
		resbuf* tmp_res = rb_wrap.res_buf->rbnext;
		while (NULL != tmp_res)
		{
			if (tmp_res->restype == AcDb::kDxfXdAsciiString
				&& wcscmp(tmp_res->resval.rstring, xdata_tag.c_str()) == 0)
			{
				tmp_res = tmp_res->rbnext;
				if (tmp_res->restype == AcDb::kDxfXdAsciiString)
				{
					wchar_t* data = new wchar_t[val.length() + 1];
					std::wcscpy(data, val.c_str());
					tmp_res->resval.rstring = data;
					ret = true;
				}
				break;
			}
			tmp_res = tmp_res->rbnext;
		}
	}
	ErrorStatus err = ent_wrap.object->upgradeOpen();
	if (err == ErrorStatus::eOk)
	{
		err = ent_wrap.object->setXData(rb_wrap.res_buf);
		return true;
	}
	return ret;

}

bool XDataFuncs::ChangeXDataHandleVal(const AcDbObjectId & id, const std::wstring & app_name,
	const std::wstring & xdata_tag, const std::wstring & val)
{
	bool ret = false;

	ObjectWrap<AcDbEntity> ent_wrap(OpenObjectId<AcDbEntity>(id));
	if (ent_wrap.object == NULL)
		return false;

	ResBufWrap rb_wrap(ent_wrap.object->xData(app_name.c_str()));
	if (NULL == rb_wrap.res_buf)
		return false;

	{
		resbuf* tmp_res = rb_wrap.res_buf->rbnext;
		while (NULL != tmp_res)
		{
			if (tmp_res->restype == AcDb::kDxfXdAsciiString
				&& wcscmp(tmp_res->resval.rstring, xdata_tag.c_str()) == 0)
			{
				if (AcDb::kDxfXdHandle == tmp_res->rbnext->restype)
				{
					struct resbuf* nrb = acutBuildList(AcDb::kDxfXdHandle, val.c_str(), RTNONE);
					nrb->rbnext = tmp_res->rbnext->rbnext;

					ResBufWrap del_wrap(tmp_res->rbnext);
					del_wrap.res_buf->rbnext = NULL;

					tmp_res->rbnext = nrb;
					ret = true;
				}
				break;
			}
			tmp_res = tmp_res->rbnext;
		}
	}
	ErrorStatus err = ent_wrap.object->upgradeOpen();
	if (err == ErrorStatus::eOk)
	{
		err = ent_wrap.object->setXData(rb_wrap.res_buf);
		return true;
	}
	return ret;

}
