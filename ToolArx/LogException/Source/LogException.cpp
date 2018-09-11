#include "LogException.h"
#include <ctime>
#include <wchar.h>
#include <stdio.h>
#include <string>
#include <iostream>
#include <clocale>
#include <fstream>
#include <CString>
#ifdef CPP11
#include <codecvt>
#endif

#include <windows.h>
#include <stdio.h>


//#include <chrono>
#include <ctime>

using namespace std;
//using namespace std::chrono;

static void WriteLog(const wchar_t* file_name, const std::wstring message);


LogException::LogException(const wchar_t* fn, std::wstring mess, bool ix, bool isExit) :
	std::exception(),
	file_name(std::wstring(fn)), message(mess), is_exit(isExit), is_export(ix)
{}

LogException::~LogException()
{
}

void LogException::Handle() const
{
	if (is_export)
		WriteLog(file_name.c_str(), LogMessage());

	if(is_exit)
		exit(EXIT_FAILURE);
}

const std::wstring LogException::LogMessage() const
{
	return message.c_str();
}

void WriteLog(const wchar_t* file_name, const std::wstring mess)
{
#ifdef CPP11
	std::ofstream f(file_name, std::ios::app);

	std::wstring_convert<std::codecvt_utf8<wchar_t>, wchar_t> utf8_conv;
	std::string str = utf8_conv.to_bytes(mess.c_str());

	f << str;
	f.close();

#endif
}