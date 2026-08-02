struct MyStruct
{
	float a, b;
};

#define EXTERN extern "C"
#define DLL_EXPORT __declspec(dllexport)
#define API_EXPORT DLL_EXPORT EXTERN

API_EXPORT void Test1(const char* value, float val[3]);