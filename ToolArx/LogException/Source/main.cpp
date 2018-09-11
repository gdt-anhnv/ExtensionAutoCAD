//#include <ctime>
//#include <exception>
//#include <string>
//#include <iostream>
//using namespace std;
//
//class My_Exception : std::exception {
//
//public:
//	explicit My_Exception(ERROR_NAME _name) :
//		_err(_name)
//	{
//		action();
//	}
//
//	//virtual const char* what() const throw ()
//	//{
//	//	return msg_.c_str();
//	//}
//
//	void action()
//	{
//		switch (_err)
//		{
//			case ERROR_NAME::eNULL: {
//				writeLog("hello world");
//				exit(EXIT_FAILURE);
//			}; break;
//		}
//
//	}
//
//	void writeLog(const char* content)
//	{
//		time_t t = time(0);
//		struct tm * now = localtime(&t);
//
//		freopen("err", "w", stderr);
//
//		cerr << (now->tm_year + 1900) << '-'
//			<< (now->tm_mon + 1) << '-'
//			<< now->tm_mday
//			<< endl;
//		cerr << content << endl;
//
//		//delete now;
//	}
//
//protected:
//	//std::string msg_;
//	ERROR_NAME _err;
//};

#include "LogException.h"
#include <iostream>
using namespace std;

int main()
{
	int a = 4;

	try
	{
		if(a == 4)
			throw new LogException(L"DK_AUTO_DRAWING_yyyyMMdd.log", L"Notthing wrong", true);
	} catch (LogException* error) {
		cout << "start catch" << endl;
		//wcout << error->getMessage() << endl;
		error->Handle();
	}

	cout << "Failed";
	system("pause");
	return 0;
}