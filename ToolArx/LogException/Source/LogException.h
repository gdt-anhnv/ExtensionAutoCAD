#ifndef _LOG_EXCEPTION_
#define _LOG_EXCEPTION_

#include <string>
#include <exception>
#include <iostream>

class LogException : std::exception
{
private:
	std::wstring file_name;
	std::wstring message;
	bool is_exit;
	bool is_export;
public:
	LogException(const wchar_t* fn, std::wstring message, bool ix, bool isExit = false);
	~LogException();

	void Handle() const;
	virtual const std::wstring LogMessage() const;
};

#endif // _LOG_EXCEPTION_
